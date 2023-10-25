using CtaDNI_Premios_CargaDeTabla.DAL;
using CtaDNI_Premios_CargaDeTabla.GFService;
using CtaDNI_Premios_CargaDeTabla.Model;

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
                    DatosCliente datosCliente = ObtenerDatosCliente(ent);

                    //impacto en la tabla destino
                    dal.InsertarRegistro(ent, datosCliente);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Hubo un problema y no se insertó el registro " + ent.id + " " + e.Message);
                }

            }
        }

        private DatosCliente ObtenerDatosCliente(Entry ent)
        {
            DatosCliente datosCliente = new DatosCliente();
            //obtener CUIT
            datosCliente.CUIT = dal.ObtenerCUIT(ent._24);
            //obtengo su cbu
            datosCliente.CBU = dal.ObtenerCBU(ent._24);
            //obtengo su fecha de última acreditación BIP
            datosCliente.fechaUltAcreditacionBIP = dal.ObtenerFechaAcreditacion(datosCliente.CUIT);

            //Información del Ejecutivo

            //Nombre completo
            datosCliente.EjecutivoAsociado = dal.ObtenerNombreCompletoEjecutivo(ent._24);
            //Branch
            datosCliente.BranchEjecutivo = dal.ObtenerBranchEjecutivo(ent._24);
     

            return datosCliente;
        }

        internal async Task<GFFormModel> ObtenerRegistros()
        {
            GFFormModel entries = await _gravityFormsApiClient.GetFormEntries(45);
            return entries;
        }
    }
}
