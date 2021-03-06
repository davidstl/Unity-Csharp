using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using System;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestRedemptionCode : TestFixtureBase
    {
        private readonly string _lastUsedCodeStatName = "lastCodeUsed";
        private readonly string _codeType = "default";

        [Test]
        public void TestRedeemCode()
        {
            TestResult tr = new TestResult(_bc);

            _bc.RedemptionCodeService.RedeemCode(
                GetValidCode().ToString(),
                _codeType,
                Helpers.CreateJsonPair("test", 127),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetRedeemedCodes()
        {
            TestResult tr = new TestResult(_bc);

            _bc.RedemptionCodeService.GetRedeemedCodes(
                _codeType,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        private long GetValidCode()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GlobalStatisticsService.IncrementGlobalStats(
                Helpers.CreateJsonPair(_lastUsedCodeStatName, "+1"),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Dictionary<string, object> stats = Helpers.GetDataFromJsonResponse(tr.m_response)["statistics"] as Dictionary<string, object>;

            return Convert.ToInt64(stats[_lastUsedCodeStatName]);
        }
    }
}