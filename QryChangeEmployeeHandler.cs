using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Azure;
using Azure.Messaging.EventGrid;
using Newtonsoft.Json;


namespace BackendChallenge.Function
{
    public static class QryChangeEmployeeHandler
    {

        private static void AddEventToTopic(EventGridPublisherClient eventGridClient, Document inputDocument ,  ILogger log)
        {            
            try {
                string data = JsonConvert.SerializeObject(inputDocument);
                log.LogInformation("Logging data added/updated: " + data);
                EventGridEvent eventGridItem = new EventGridEvent("Cosmos DB - Employee Container Update", "CosmosDB.Update", "1.0" , data);
                eventGridClient.SendEvent(eventGridItem);
            }   catch(Exception ex)    {
                log.LogError($"An error occure upon sending event grid event:  {ex.Message} \n {ex.StackTrace}" );
            }
        }

        [FunctionName("QryChangeEmployeeHandler")]
        public static void Run([CosmosDBTrigger(
            databaseName: "TestDB",
            collectionName: "Employee",
            ConnectionStringSetting = "CosmosDbConnectionString",
            LeaseCollectionName = "leases")] IReadOnlyList<Document> inputDocuments,
            ILogger log)
        {
            log.LogInformation("QryChangeEmployeeHandler : Start");
            string url = Environment.GetEnvironmentVariable("backEndEventGridTopic");
            string key = Environment.GetEnvironmentVariable("backEndEventGridTopicSetting");

            // log.LogInformation($"topic-Url : {url}" );
            // log.LogInformation($"topic-Key : {key}" );

            Uri eventGridTopiccUri = new Uri(url);
            AzureKeyCredential eventGridTopicKeySetting = new AzureKeyCredential(key);            
            EventGridPublisherClient eventGridClient = new EventGridPublisherClient(eventGridTopiccUri, eventGridTopicKeySetting);
                    
            if (inputDocuments != null && inputDocuments.Count > 0)
            {
                log.LogInformation("Documents modified " + inputDocuments.Count);

                foreach(var inputDocumentItem in inputDocuments)    {
                    AddEventToTopic(eventGridClient, inputDocumentItem, log);
                }
            }
            log.LogInformation("QryChangeEmployeeHandler : End");

        }
    }
}
