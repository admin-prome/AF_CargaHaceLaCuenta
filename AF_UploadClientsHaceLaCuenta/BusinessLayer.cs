using AF_UploadClientsHaceLaCuenta.Model;
using CtaDNI_Premios_CargaDeTabla.DAL;
using CtaDNI_Premios_CargaDeTabla.GFService;
using CtaDNI_Premios_CargaDeTabla.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AF_UploadClientsHaceLaCuenta
{
    public class BusinessLayer
    {
        private GravityFormsApiClient _gravityFormsApiClient;
        private DAL dal;
        private readonly ILogger _logger;

        public BusinessLayer(DAL dAL, GravityFormsApiClient gravityFormsApiClient, ILogger logger)
        {
            _logger = logger;
            _gravityFormsApiClient = gravityFormsApiClient;
            dal = dAL;
        }

        public EntriesProcessingResult GenerarRegistrosEnTablaDestino(List<Entry> entries)
        {
            EntriesProcessingResult entriesProcessingResult = new EntriesProcessingResult();
            entriesProcessingResult.TotalEntries = entries.Count;
            int insertedEntries = 0;

            foreach (Entry ent in entries)
            {
                ProcessedEntry processedEntry = new ProcessedEntry() { Id = ent.id };

                try
                {
                    _logger.LogInformation("Procesando registro: " + ent.id);
                    DatosCliente datosCliente = ObtenerDatosCliente(ent);

                    if (datosCliente != null)
                    {
                        // Insertar datos del cliente en la tabla de destino
                        if (InsertarDatosCliente(ent, datosCliente))
                        {
                            insertedEntries++;
                            processedEntry.Inserted = true;
                        }
                        else
                        {
                            processedEntry.Inserted = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    processedEntry.Inserted = false;
                    _logger.LogError("Error al procesar el registro " + ent.id + ": " + e.Message);
                }
                entriesProcessingResult.ProcessedEntries.Add(processedEntry);
            }
            entriesProcessingResult.InsertedEntries = insertedEntries;
            _logger.LogInformation("Procesamiento completo. " + insertedEntries + " registros insertados.");
            return entriesProcessingResult;
        }

        private DatosCliente ObtenerDatosCliente(Entry ent)
        {
            try
            {
            _logger.LogInformation("Obteniendo datos del cliente para el registro " + ent.id);
            DatosCliente datosCliente = new DatosCliente();

            // Obtener datos del cliente desde el DAL
            datosCliente.CUIT = dal.ObtenerCUIT(ent._24);
            datosCliente.CBU = dal.ObtenerCBU(ent._24);
            datosCliente.fechaUltAcreditacionBIP = dal.ObtenerFechaAcreditacion(datosCliente.CUIT);
            datosCliente.EjecutivoAsociado = dal.ObtenerNombreCompletoEjecutivo(ent._24);
            datosCliente.BranchEjecutivo = dal.ObtenerBranchEjecutivo(ent._24);

            _logger.LogInformation("Datos del cliente obtenidos para el registro " + ent.id);
                return datosCliente;
            }
            catch(Exception e) {
                _logger.LogInformation("Error obteniendo datos del cliente  para el registro " + ent.id + " "+e.Message);
                return null;
            }

        }

        private bool InsertarDatosCliente(Entry ent, DatosCliente datosCliente)
        {
            try
            {
                _logger.LogInformation("Insertando registro " + ent.id );
                // Insertar datos del cliente en la tabla de destino
                dal.InsertarRegistro(ent, datosCliente);
                _logger.LogInformation("Registro " + ent.id + " insertado exitosamente.");
                return true;
            }
            catch (Exception e)
            {
                // Manejar el error de inserción
                _logger.LogError("Error al insertar datos para el registro " + ent.id + ": " + e.Message);
                return false;
            }
        }

        internal async Task<GFFormModel> ObtenerRegistros()
        {
            string formId = Environment.GetEnvironmentVariable("FormId")!;
            GFFormModel entries = await _gravityFormsApiClient.GetFormEntries(int.Parse(formId));
            entries.total_count = entries.total_count;
            _logger.LogInformation("Se recuperaron " + entries.entries.Count + " registros desde la API de Gravity Forms.");
            return entries;
        }
    }
}
