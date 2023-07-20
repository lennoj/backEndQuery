// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;

namespace BackendChallenge.Function
{
    public static class QryChangeEventHandler
    {
        [FunctionName("QryChangeEventHandler")]
        public static void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation("QryChangeEventHandler: " + eventGridEvent.Data.ToString());
        } 
    }
}
