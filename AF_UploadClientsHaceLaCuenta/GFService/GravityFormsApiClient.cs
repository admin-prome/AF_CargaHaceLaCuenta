using CtaDNI_Premios_CargaDeTabla.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace CtaDNI_Premios_CargaDeTabla.GFService
{
    public class GravityFormsApiClient
    {
        private readonly IRestClient _restClient;
        private const string baseUrl = "https://www.provinciamicrocreditos.com.ar/wp-json/gf/v2/";
        private string apiKey;
        private string apiSecret;
        private readonly ILogger _logger;
        public GravityFormsApiClient(ILogger logger)
        {
            _logger = logger;
            this.apiKey = Environment.GetEnvironmentVariable("ApiKey")!;
            this.apiSecret = Environment.GetEnvironmentVariable("ApiSecret")!;
            //Set up options for OAuth1.
            var restClientOptions = new RestClientOptions(baseUrl)
            {
                Authenticator = new OAuth1Authenticator()
                {
                    ConsumerKey=apiKey,
                    ConsumerSecret=apiSecret
                }
            };

            _restClient = new RestClient(restClientOptions);
        }

        public async Task<GFFormModel> GetFormEntries(int formId)
        {
            // Define the API endpoint for retrieving form entries.
            string entriesEndpoint = $"forms/{formId}/entries";

            // Calculate the start and end dates for the request (last 24 hours).
            DateTime now = DateTime.Now;
            DateTime endDate = now;
            DateTime startDate = now.AddHours(-24);
            string startDateFormat = startDate.ToString("yyyy-MM-ddTHH:mm:ss");
            string endDateFormat = endDate.ToString("yyyy-MM-ddTHH:mm:ss");

            // Create a new REST request to retrieve form entries.
            var request = new RestRequest(entriesEndpoint, Method.Get);
            // Add parameter to get the entries from the last 24 hours.
            request.AddParameter("search", $"{{\"start_date\":\"{startDateFormat}\", \"end_date\":\"{endDateFormat}\"}}");
            _logger.LogInformation(_restClient.BuildUri(request).ToString());

            //Execute Request.
            RestResponse response =await _restClient.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                string responseBody = response.Content!;
                _logger.LogInformation("Request successful.");
                GFFormModel gFFormModelResponse = ObtenerFGGormModelResponse(responseBody);
                return gFFormModelResponse;
            }
            else
            {
                throw new Exception($"Request failed with status code: {response.StatusCode}");
            }
        }
        private GFFormModel ObtenerFGGormModelResponse(string responseBody)
        {
            JObject responseObject = JObject.Parse(responseBody);
            var entries = responseObject["entries"];
            GFFormModel formModelResponse = new GFFormModel();
            _logger.LogInformation("Reading Response...");
            foreach (var entry in entries)
            {
                var entryString = JsonConvert.SerializeObject(entry, Formatting.Indented);
                JObject entryJOBject = JObject.Parse(entryString);

                Entry entryRecord = new Entry();
                entryRecord.id= entryJOBject["id"].ToString();
                entryRecord._1 = entryJOBject["1"].ToString();
                entryRecord._11 = entryJOBject["11"].ToString();
                entryRecord._14= entryJOBject["14"].ToString();
                entryRecord._15= entryJOBject["15"].ToString();
                entryRecord._23= entryJOBject["23"].ToString();
                entryRecord._24 = entryJOBject["24"].ToString();
                entryRecord._7= entryJOBject["7"].ToString();
                entryRecord._72= entryJOBject["72"].ToString();
                entryRecord._91= entryJOBject["91"].ToString();
                entryRecord._96 = entryJOBject["96"].ToString();
                entryRecord._97= entryJOBject["97"].ToString();
                entryRecord.date_created = entryJOBject["date_created"].ToString();
                formModelResponse.entries.Add(entryRecord);
            }

            return formModelResponse;
        }
    }
}