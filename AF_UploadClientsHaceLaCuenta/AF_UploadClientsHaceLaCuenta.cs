using System;
using CtaDNI_Premios_CargaDeTabla.DAL;
using CtaDNI_Premios_CargaDeTabla.GFService;
using CtaDNI_Premios_CargaDeTabla.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AF_UploadClientsHaceLaCuenta
{
    public class AF_UploadClientsHaceLaCuenta
    {
        private readonly ILogger _logger;

        public AF_UploadClientsHaceLaCuenta(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AF_UploadClientsHaceLaCuenta>();
         
        }

        [Function("AF_UploadClientsHaceLaCuenta")]
        public void Run([TimerTrigger("0 0 0 */1 * *")] MyInfo myTimer)
        {
            RunAsync().Wait();
        }

        public async Task RunAsync()
        {
            try
            {
                
                DAL dAL = new DAL();
                GravityFormsApiClient _gravityFormsApiClient = new GravityFormsApiClient();
                BusinessLayer businessLayer = new BusinessLayer(dAL, _gravityFormsApiClient);
                GFFormModel registros = await businessLayer.ObtenerRegistros();
                businessLayer.GenerarRegistrosEnTablaDestino(registros.entries);

                Console.WriteLine("Fin del programa");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("El programa finalizó con una excepción" + e.Message);
                Console.ReadLine();
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
