using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;
using System;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestEvent : TestFixtureBase
    {
        private readonly string _eventType = "test";
        private readonly string _eventDataKey = "testData";

        private bool _callbackRan = false;

        [Test]
        public void TestSendEvent()
        {
            TestResult tr = new TestResult(_bc);
            string eventId = "";

            _bc.Client.RegisterEventCallback(EventCallback);

            _bc.EventService.SendEvent(
                GetUser(Users.UserA).ProfileId,
                _eventType,
                Helpers.CreateJsonPair(_eventDataKey, 117),
                tr.ApiSuccess, tr.ApiError);

            if (tr.Run())
            {
                eventId = (((Dictionary<string, object>)(tr.m_response["data"]))["evId"]) as string;
            }

            Assert.IsTrue(_callbackRan);

            CleanupIncomingEvent(eventId);
            _bc.Client.DeregisterEventCallback();
        }

        //[Test] //TODO Fix
        //public void TestSendEvent()
        //{
        //    TestResult tr = new TestResult(_bc);
        //    ulong eventId = 0;

        //    _bc.EventService.SendEvent(
        //        GetUser(Users.UserB).ProfileId,
        //        _eventType,
        //        Helpers.CreateJsonPair(_eventDataKey, 117),
        //        true,
        //        tr.ApiSuccess, tr.ApiError);

        //    if (tr.Run())
        //    {
        //        eventId = System.Convert.ToUInt64(((Dictionary<string, object>)(tr.m_response["data"]))["eventId"]);
        //    }

        //    _bc.PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
        //    tr.Run();
        //    _bc.AuthenticationService.ClearSavedProfileID();

        //    _bc.RegisterEventCallback(EventCallback);

        //    _bc.AuthenticationService.AuthenticateUniversal(GetUser(Users.UserB).Id, GetUser(Users.UserB).Password, false, tr.ApiSuccess, tr.ApiError);
        //    tr.Run();

        //    CleanupIncomingEvent(eventId);
        //}

        public void EventCallback(string jsonResponse)
        {
            Console.WriteLine("Events received: " + jsonResponse);

            var response = JsonReader.Deserialize<Dictionary<string, object>>(jsonResponse);
            var events = (object[])(response["events"]);

            Assert.Greater(events.Length, 0);

            _callbackRan = true;
        }
        
        [Test]
        public void TestDeleteIncomingEvent()
        {
            TestResult tr = new TestResult(_bc);

            string eventId = SendDefaultMessage();

            _bc.EventService.DeleteIncomingEvent(
                eventId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateIncomingEventData()
        {
            TestResult tr = new TestResult(_bc);

            string eventId = SendDefaultMessage();

            _bc.EventService.UpdateIncomingEventData(
                eventId,
                Helpers.CreateJsonPair(_eventDataKey, 343),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            CleanupIncomingEvent(eventId);
        }

        [Test]
        public void TestGetEvents()
        {
            TestResult tr = new TestResult(_bc);

            string eventId = SendDefaultMessage();

            _bc.EventService.GetEvents(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            CleanupIncomingEvent(eventId);
        }

        #region Helpers

        private string SendDefaultMessage()
        {
            TestResult tr = new TestResult(_bc);
            string eventId = "";

            _bc.EventService.SendEvent(
                GetUser(Users.UserA).ProfileId,
                _eventType,
                Helpers.CreateJsonPair(_eventDataKey, 117),
                tr.ApiSuccess, tr.ApiError);

            if (tr.Run())
            {
                eventId = (((Dictionary<string, object>)(tr.m_response["data"]))["evId"]) as string;
            }

            return eventId;
        }

        private void CleanupIncomingEvent(string eventId)
        {
            TestResult tr = new TestResult(_bc);

            _bc.EventService.DeleteIncomingEvent(
                eventId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        #endregion
    }
}