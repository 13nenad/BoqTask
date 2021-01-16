using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace BoqTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SessionAndTopicsController : ControllerBase
    {
        private IConfiguration configuration;
        private readonly string SubscriptionKey;

        public SessionAndTopicsController(IConfiguration iConfig)
        {
            configuration = iConfig;
            SubscriptionKey = configuration.GetValue<string>("SubscriptionKey");
        }

        [HttpGet("{speakerName}/{dateTimeSlot}")]
        public ActionResult<CollectionModel> Get(string speakerName, string dateTimeSlot)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
            client.DefaultRequestHeaders.Add("Api-version", "v1");

            var model = AddFilteredSession(client, speakerName, dateTimeSlot);
            if (model.collection.items.Count == 0) return NotFound();
            model = AddSessionTopics(client, model);    

            return Ok(model);
        }

        /// <summary>
        /// Since a speaker can only perform one presentation at any given time, the filtering requirements
        /// will always give back only one session.
        /// </summary>
        /// <param name="client">The HTTP client to call API</param>
        /// <returns>The collection model with the filtered session</returns>
        public static CollectionModel AddFilteredSession(HttpClient client, string speakerName, string dateTimeSlot)
        {
            // If the speakername filtering on the sessions resource worked then I could use it here.
            // But because it doesn't, I am retrieving all sessions and filtering it myself
            var sessionsUrl = "https://apicandidates.azure-api.net/conference/sessions";

            var jsonString = client.GetAsync(sessionsUrl).Result.Content.ReadAsStringAsync().Result;
            var rootModel = JsonConvert.DeserializeObject<CollectionModel>(jsonString);

            // Filter the session down to only one using speakername and date-timeslot
            // For some reason some sessions don't have all 3 attributes, checking that as well
            var session = rootModel.collection.items.FirstOrDefault(
                m => m.data.Count == 3 && 
                m.data[2].value == speakerName && 
                m.data[1].value == dateTimeSlot);

            rootModel.collection.items.Clear();
            if (session != null) rootModel.collection.items.Add(session);
            return rootModel;
        }

        /// <summary>
        /// Add all of the topics assocaited with the session.
        /// </summary>
        /// <param name="client">The HTTP client to call API</param>
        /// <returns>The collection model with all associated topics added</returns>
        public static CollectionModel AddSessionTopics(HttpClient client, CollectionModel rootModel)
        {
            string jsonString;
            CollectionModel topicsModel;
            foreach (Item sessionItem in rootModel.collection.items)
            {
                string topicsUrl = sessionItem.links.Where(m => m.rel == "http://tavis.net/rels/topics").First().href;
                jsonString = client.GetAsync(topicsUrl).Result.Content.ReadAsStringAsync().Result;

                topicsModel = JsonConvert.DeserializeObject<CollectionModel>(jsonString);
                sessionItem.items = new List<Item>();
                sessionItem.items.AddRange(topicsModel.collection.items);
            }

            return rootModel;
        }
    }
}
