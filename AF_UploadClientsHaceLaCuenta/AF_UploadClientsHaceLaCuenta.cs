using CtaDNI_Premios_CargaDeTabla.DAL;
using CtaDNI_Premios_CargaDeTabla.GFService;
using CtaDNI_Premios_CargaDeTabla.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AF_UploadClientsHaceLaCuenta
{
    public class AF_UploadClientsHaceLaCuenta
    {
        private readonly ILogger _logger;

        public AF_UploadClientsHaceLaCuenta(ILogger<AF_UploadClientsHaceLaCuenta> logger)
        {
            _logger = logger;
         
        }

        [Function("AF_UploadClientsHaceLaCuenta")]
        public void Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestData req)
        {
            RunAsync().Wait();
        }

        public async Task<ActionResult> RunAsync()
        {
            try
            { 
                DAL dAL = new DAL();
                GravityFormsApiClient _gravityFormsApiClient = new GravityFormsApiClient(_logger);
                BusinessLayer businessLayer = new BusinessLayer(dAL, _gravityFormsApiClient);
                GFFormModel registros = await businessLayer.ObtenerRegistros();
                businessLayer.GenerarRegistrosEnTablaDestino(registros.entries);

                return new OkObjectResult("Function executed successfully");
            }
            catch (Exception e)
            {
                _logger.LogError($"Function failed with exception: {e.Message}");
                return new BadRequestObjectResult($"Function failed with exception: {e.Message}");
            }
        }
    }

    

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
