using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace StripeInterface
{
    public static class AzureFunction
    {
        [FunctionName("GetCustomerEvent")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Read contents from Post stream
            var content = await new StreamReader(req.Body).ReadToEndAsync();

            // Deserialise the event message
            var msg = JsonConvert.DeserializeObject<Root>(content);

            // Connect to Power Platform
            try
            {
                // Connect to Power Platform Source
                ServiceClient service = DataverseConnection();

                // Test connection
                Entity contact = new Entity("contact");
                {
                    contact["firstname"] = msg.Data.Object.Name;
                    contact["description"] = msg.ToString();

                    service.Create(contact);
                }
            }
            catch (Exception e)
            {
                log.LogInformation(e.ToString());
            }

            return new OkObjectResult($"Event captured for: {msg.Data.Object.Name}");
        }

        private static ServiceClient DataverseConnection()
        {
            string clientId = Environment.GetEnvironmentVariable("client");
            string clientSecret = Environment.GetEnvironmentVariable("secret");
            string environment = Environment.GetEnvironmentVariable("environment");

            var connectionString = @$"Url=https://{environment}.dynamics.com;AuthType=ClientSecret;ClientId={clientId};ClientSecret={clientSecret};RequireNewInstance=true";
            var service = new ServiceClient(connectionString);
            return service;
        }
    }

    public class InvoiceSettings
    {
        [JsonProperty("custom_fields")]
        public object CustomFields { get; set; }

        [JsonProperty("default_payment_method")]
        public object DefaultPaymentMethod { get; set; }

        [JsonProperty("footer")]
        public object Footer { get; set; }
    }

    public class Metadata
    {
    }

    public class Object
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string ObjectType { get; set; }

        [JsonProperty("address")]
        public object Address { get; set; }

        [JsonProperty("balance")]
        public int Balance { get; set; }

        [JsonProperty("created")]
        public int Created { get; set; }

        [JsonProperty("currency")]
        public object Currency { get; set; }

        [JsonProperty("default_source")]
        public object DefaultSource { get; set; }

        [JsonProperty("delinquent")]
        public bool Delinquent { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("discount")]
        public object Discount { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("invoice_prefix")]
        public string InvoicePrefix { get; set; }

        [JsonProperty("invoice_settings")]
        public InvoiceSettings InvoiceSettings { get; set; }

        [JsonProperty("livemode")]
        public bool Livemode { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone")]
        public object Phone { get; set; }

        [JsonProperty("preferred_locales")]
        public List<object> PreferredLocales { get; set; }

        [JsonProperty("shipping")]
        public object Shipping { get; set; }

        [JsonProperty("tax_exempt")]
        public string TaxExempt { get; set; }

        [JsonProperty("test_clock")]
        public object TestClock { get; set; }
    }

    public class Data
    {
        [JsonProperty("object")]
        public Object Object { get; set; }
    }

    public class Request
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("idempotency_key")]
        public string IdempotencyKey { get; set; }
    }

    public class Root
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("api_version")]
        public string ApiVersion { get; set; }

        [JsonProperty("created")]
        public int Created { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("livemode")]
        public bool Livemode { get; set; }

        [JsonProperty("pending_webhooks")]
        public int PendingWebhooks { get; set; }

        [JsonProperty("request")]
        public Request Request { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}