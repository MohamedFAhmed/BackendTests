using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MiTrendzBETest
{
    class DataModel
    {
    }
    public class APITestResult
    {
        //branch1
        public APITestResult()
        {
            Id = ObjectId.Empty;
            startTime = endTime = DateTime.UtcNow;
            averageLatency = 0;
            maxLaency = 0;
            minLatency = 0;
            SuccessNum = 0;
            failedNum = 0;


        }
        //test_branch comment
        //branch1
        public ObjectId Id { set; get; }
        public DateTime startTime { set; get; }
        public DateTime endTime { set; get; }
        public Int32 averageLatency { set; get; }
        public Int32 minLatency { set; get; }
        public Int32 maxLaency { set; get; }
        public Int32 SuccessNum { set; get; }
        public Int32 failedNum { set; get; }
    }

    public class APICallResult
    {
        public APICallResult()
        {
            Id = ObjectId.Empty;
            returnedCode = 200;
            errorMessage = string.Empty;
            APICall = string.Empty;
            callTime = DateTime.UtcNow;
            latency = 0;
        }
        public ObjectId Id { set; get; }
        public Int16 returnedCode { set; get; }
        public string errorMessage { set; get; }
        public string APICall { set; get; }
        public DateTime callTime { set; get; }
        public Int32 latency { set; get; }
    }
}
