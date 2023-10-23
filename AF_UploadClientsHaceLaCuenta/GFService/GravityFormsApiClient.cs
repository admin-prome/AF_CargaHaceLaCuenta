using CtaDNI_Premios_CargaDeTabla.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace CtaDNI_Premios_CargaDeTabla.GFService
{
    public class GravityFormsApiClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://provinciamicrocreditos.com/wp-json/gf/v2/";
        private string apiKey;
        private string apiSecret;
        


        public GravityFormsApiClient()
        {
            this.apiKey = Environment.GetEnvironmentVariable("ApiKey")!;
            this.apiSecret = Environment.GetEnvironmentVariable("ApiSecret")!;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<GFFormModel> GetFormEntries(int formId)
        {
            string requestUrl = $"forms/{formId}/entries";
            string authToken = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{this.apiKey}:{this.apiSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();
            
            string responseBody = await response.Content.ReadAsStringAsync();
            GFFormModel gFFormModelResponse = ObtenerFGGormModelResponse(responseBody);
            
            /**/
            DateTime twentyFourHoursAgo = DateTime.UtcNow.AddHours(-24);
            gFFormModelResponse.entries = gFFormModelResponse.entries
                .Where(entry => DateTime.Parse(entry.date_created) > twentyFourHoursAgo)
                .ToList();
            
            return gFFormModelResponse;
        }

        private GFFormModel ObtenerFGGormModelResponse(string responseBody)
        {
           
             JObject responseObject = JObject.Parse(responseBody);
            var entries = responseObject["entries"];
            GFFormModel formModelResponse = new GFFormModel();
            foreach(var entry in entries) 
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
