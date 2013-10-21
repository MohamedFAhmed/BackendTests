using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MiTrendzBETest
{
    public class DbConstants
    {
        public const string dbName = "MiTrendzBETests";
        //Collections Names
        public const string APIsTestResultsCollection = "APIsTestResults";
        public const string APICallResultsCollection = "APICallResults";
    }

    public class TestRepository
    {
        private MongoClient mongoclient = null;
        private MongoServer server = null;
        private MongoDatabase db = null;

        public TestRepository()
        {
            var connectionString = "mongodb://98.225.46.100";
            mongoclient = new MongoClient(connectionString);
            server = mongoclient.GetServer();
            db = server.GetDatabase(DbConstants.dbName);
            EnsureIndexes();
        }

        private void EnsureIndexes()
        {

        }

        public void LogAPITestResults(APITestResult testResult)
        {
            var apiTestCollect = db.GetCollection<APITestResult>(DbConstants.APIsTestResultsCollection);
            apiTestCollect.Save(testResult);
        }
        public void LogAPICallResult(APICallResult callResult)
        {
            var apiCallsCollect = db.GetCollection<APICallResult>(DbConstants.APICallResultsCollection);
            apiCallsCollect.Save(callResult);
        }
    }

}
