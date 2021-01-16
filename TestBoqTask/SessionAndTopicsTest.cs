using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using BoqTask.Controllers;

namespace TestBoqTask
{
    [TestClass]
    public class SessionAndTopicsTest
    {
        private string SubscriptionKey;
        private HttpClient client;

        [TestInitialize]
        public void Initialise()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            SubscriptionKey = config["SubscriptionKey"].ToString();

            client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
            client.DefaultRequestHeaders.Add("Api-version", "v1");
        }

        [TestMethod]
        public void TestSessionFilteringAndTopicInsertion()
        {
            var model = SessionAndTopicsController.AddFilteredSession(client, "Jon Skeet", "04 December 2013 10:20 - 11:20");
            Assert.IsTrue(model.collection.items.Count == 1); // Ensure there is only one session

            model = SessionAndTopicsController.AddSessionTopics(client, model);
            Assert.IsTrue(model.collection.items[0].items.Count != 0); // Ensure topics have been added
        }

        [TestMethod]
        public void NoSessionFoundDueToSpeakerName()
        {
            var model = SessionAndTopicsController.AddFilteredSession(client, "Aaa", "04 December 2013 10:20 - 11:20");
            Assert.IsTrue(model.collection.items.Count == 0); // Ensure there is no session found
        }

        [TestMethod]
        public void NoSessionFoundDueToDateTimeSlot()
        {
            var model = SessionAndTopicsController.AddFilteredSession(client, "Jon Skeet", "04 FakeMonth 2013 10:20 - 11:20");
            Assert.IsTrue(model.collection.items.Count == 0); // Ensure there is no session found
        }

        [TestMethod]
        public void NoTopicsFoundForSession()
        {
            var model = SessionAndTopicsController.AddFilteredSession(client, "Dan North", "04 December 2013 09:00 - 10:00");
            Assert.IsTrue(model.collection.items.Count == 1); // Ensure there is only one session

            // This session has no associated topics
            model = SessionAndTopicsController.AddSessionTopics(client, model);
            Assert.IsTrue(model.collection.items[0].items.Count == 0); // Ensure no topics have been added (and no exception is thrown)
        }
    }
}
