using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Backend.Repository;
using Common.DataModel;
using MongoDB.Bson;
using ServiceStack.ServiceClient.Web;

namespace MiTrendzBETest
{
    public class PublicAPIsTester
    {
        public ObjectId[] sources = {};
        private MiTrendzRepository dataRepository = null;
        private TestRepository testRepository = null;
        public PublicAPIsTester()
        {
            dataRepository = new MiTrendzRepository();
            testRepository = new TestRepository();
        }
        /// <summary>
        /// Initiates tesing our public APIs. We will test:
        /// 1- User Management APIs
        /// 2- Categories & packs retrieval APIs
        /// 3- Clouds Retrieval APIs.
        /// </summary>
        public void StartTesting()
        {
            RunCloudAPIsTests();
            RunCategoriesAPIsTests();
            RunUsersAPIsTests();
        }

        private void RunUsersAPIsTests()
        {
            //added in my test_branch work!
        }

        private void RunCategoriesAPIsTests()
        {
            
        }

        /// <summary>
        /// Will initially run simple stress testing
        /// </summary>
        private async Task  RunCloudAPIsTests()
        {
            APITestResult result = new APITestResult();
            result.startTime = DateTime.UtcNow;
            //Let's get all sources we have in the database
            var sources = dataRepository.getAllRSSFeeds();
            List<Task<APITestResult>> testTasks = new List<Task<APITestResult>>(); 
            for (var i = 0; i < 100; i++)
            {
                   //Let's randomely select the size of cloud sources
                Random random = new Random();
                var sourcesNum = random.Next(10, sources.Count() - 1);
                //Let's retrieve data and measure latency
                Task<APITestResult> apiTestTask = Task.Run(() => DoAPITest(GetRandomSources(sourcesNum, sources), DateTime.UtcNow - TimeSpan.FromDays(1), DateTime.UtcNow ));
                testTasks.Add(apiTestTask);
                
            }
            var testResults = await Task.WhenAll(testTasks.ToArray());
            result.endTime = DateTime.UtcNow;
            Int32 maxLatency = Int32.MinValue, minLatency = Int32.MaxValue, averageLatency = Int32.MaxValue;
            foreach (var singleResult in testResults)
            {
                if (singleResult.maxLaency > maxLatency)
                {
                    maxLatency = singleResult.maxLaency;
                }
                if (singleResult.minLatency < minLatency)
                {
                    minLatency = singleResult.minLatency;
                }
                averageLatency = singleResult.averageLatency;
                result.SuccessNum += singleResult.SuccessNum;
                result.failedNum += singleResult.failedNum;
            }
            result.maxLaency = maxLatency;
            result.minLatency = minLatency;
            result.averageLatency = averageLatency/testResults.Count();
            testRepository.LogAPITestResults(result);
        }
        private List<RssFeed> GetRandomSources(Int32 Num, List<RssFeed> fullList)
        {
            Random random = new Random();
            Dictionary<ObjectId, RssFeed> selectedSources = new Dictionary<ObjectId, RssFeed>();
            int counter = 0;
            while (counter <= Num)
            {
                var index = random.Next(0, fullList.Count());
                if (!selectedSources.ContainsKey(fullList[index].Id))
                {
                    selectedSources.Add(fullList[index].Id, fullList[index]);
                    counter++;
                }
            }

            return selectedSources.Values.ToList();
        } 
        private async Task<APITestResult> DoAPITest(List<RssFeed> sources, DateTime startTime, DateTime endTime)
        {
            var testResult = new APITestResult();
            APICallResult callresult = new APICallResult();
            try
            {

                testResult.startTime = callresult.callTime = DateTime.UtcNow;

                CloudRequest request = new CloudRequest();
                request.selectedSources = sources.Select(source => source.Id).ToList();
                request.startTime = startTime;
                request.endTime = endTime;
                request.userId = ObjectId.Empty;
                request.socialNetworks = new List<SocialNetwork>();
                request.socialNetworks.Add(SocialNetwork.MiTrendz);
                var listeningOn = ConfigurationManager.AppSettings["ListeningOn"];
                var client = new JsonServiceClient(listeningOn);
                var response = client.Send<Dictionary<SocialNetwork, KeywordsCloud>>("GET", "/kwcloud", request);

                testResult.endTime = DateTime.UtcNow;
                testResult.maxLaency =
                    testResult.minLatency =
                    testResult.averageLatency = callresult.latency =
                                                Convert.ToInt32(
                                                    (testResult.endTime - testResult.startTime).TotalMilliseconds);
                testResult.SuccessNum = 1;
                callresult.returnedCode = 200;

            }
            catch (WebServiceException webEx)
            {
                callresult.returnedCode = Convert.ToInt16(webEx.ErrorCode);
                callresult.errorMessage = webEx.ErrorMessage;
                testResult.endTime = DateTime.UtcNow;
                testResult.maxLaency =
                    testResult.minLatency = callresult.latency =
                                            testResult.averageLatency =
                                            Convert.ToInt32(
                                                (testResult.endTime - testResult.startTime).TotalMilliseconds);
                testResult.failedNum = 1;
            }
            catch (WebException ex)
            {
                callresult.returnedCode = Convert.ToInt16(ex.Status);
                callresult.errorMessage = ex.Message;
                testResult.endTime = DateTime.UtcNow;
                testResult.maxLaency =
                    testResult.minLatency = callresult.latency =
                                            testResult.averageLatency =
                                            Convert.ToInt32(
                                                (testResult.endTime - testResult.startTime).TotalMilliseconds);
                testResult.failedNum = 1;
            }
            testRepository.LogAPICallResult(callresult);
            return testResult;


        }

    }
}
