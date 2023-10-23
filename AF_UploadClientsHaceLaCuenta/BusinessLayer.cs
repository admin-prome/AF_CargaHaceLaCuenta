using CtaDNI_Premios_CargaDeTabla.DAL;
using CtaDNI_Premios_CargaDeTabla.GFService;
using CtaDNI_Premios_CargaDeTabla.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AF_UploadClientsHaceLaCuenta
{
    public class BusinessLayer
    {
        private GravityFormsApiClient _gravityFormsApiClient;
        private DAL dal;

        public BusinessLayer(DAL dAL, GravityFormsApiClient gravityFormsApiClient)
        {
            _gravityFormsApiClient = gravityFormsApiClient;
            dal = dAL;
        }

        public void GenerarRegistrosEnTablaDestino(List<Entry> entries)
        {

            foreach (Entry ent in entries)
            {
                try
                {
                    DatosCliente datosCliente = ObtenerDatosCliente(ent, configuration);

                    //impacto en la tabla destino
                    dal.InsertarRegistro(ent, datosCliente);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Hubo un problema y no se insertó el registro " + ent.id + " " + e.Message);
                }

            }
        }

        private DatosCliente ObtenerDatosCliente(Entry ent, IConfiguration configuration)
        {
            DatosCliente datosCliente = new DatosCliente();
            //obtener CUIT
            datosCliente.CUIT = dal.ObtenerCUIT(ent._24, configuration);
            //obtengo su cbu
            datosCliente.CBU = dal.ObtenerCBU(ent._24, configuration);
            //obtengo su fecha de última acreditación BIP
            datosCliente.fechaUltAcreditacionBIP = dal.ObtenerFechaAcreditacion(datosCliente.CUIT, configuration);

            //Información del Ejecutivo

            //Nombre completo
            string fullNameCliente = ent._1 + " " + ent._7;
            string nombreCompletoEjecutivo = dal.ObtenerNombreCompletoEjecutivo(configuration, fullNameCliente);

            //Branch
            string branchEjecutivo = dal.ObtenerBranchEjecutivo(configuration, ent._24);

            return datosCliente;
        }

        internal async Task<GFFormModel> ObtenerRegistros()
        {
            GFFormModel entries = await _gravityFormsApiClient.GetFormEntries(45);
            return entries;
        }
    }
}
