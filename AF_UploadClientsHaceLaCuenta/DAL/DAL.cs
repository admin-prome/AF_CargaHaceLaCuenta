using CtaDNI_Premios_CargaDeTabla.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CtaDNI_Premios_CargaDeTabla.DAL
{
    public  class DAL
    {
       
        public DAL() 
        {
           
        }
        public void InsertarRegistro(Entry ent, DatosCliente datosCliente)
        {
            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings:DW2.ProvMicroDesa")))
            {
                SqlCommand command = new SqlCommand("InsertCuentaDNIPremios", connection);
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                // Add parameters and set their values
                 command.Parameters.AddWithValue("@Nombre", ent._1);
                 command.Parameters.AddWithValue("@Apellido", ent._7);
                 command.Parameters.AddWithValue("@DNI", ent._24);
                command.Parameters.AddWithValue("@CUIT", datosCliente.CUIT);
                command.Parameters.AddWithValue("@CorreoElectronico", ent._11);
                 command.Parameters.AddWithValue("@TipoDeTelefono", ent._14);
                 command.Parameters.AddWithValue("@CodigoArea", ent._15);
                 command.Parameters.AddWithValue("@NroTelefono", ent._72);
                 command.Parameters.AddWithValue("@PeriodoPromocion", ent._97);
                 command.Parameters.AddWithValue("@FechaLiquidacionPrestamo", ent._96);
                 command.Parameters.AddWithValue("@MontoVentasCtaDNI", ent._91);
                string idAdjunto = Uri.EscapeUriString(ent._23);
                int idAdjuntoLength=idAdjunto.Length;
                command.Parameters.Add("@IDAdjunto", SqlDbType.VarChar, 300).Value= idAdjunto;
                command.Parameters.AddWithValue("@CBU", datosCliente.CBU);
                command.Parameters.AddWithValue("@FechaCreditoBIP", datosCliente.fechaUltAcreditacionBIP);
                command.Parameters.AddWithValue("@AnalisisSegmento", "Pendiente");
                command.Parameters.AddWithValue("@AnalisisFinanzasAdm", "Pendiente");
                command.Parameters.AddWithValue("@PagoRealizado", "No");
                command.Parameters.AddWithValue("@ImporteATransferir", "");
                command.Parameters.AddWithValue("@EjecutivoComercial", datosCliente.EjecutivoAsociado);
                command.Parameters.AddWithValue("@SucursalEjecutivoComercial", datosCliente.BranchEjecutivo);
                //Agregar las de ejecutivo

                //Quitar observaciones


                command.ExecuteNonQuery();
            }
        }

      

        public string ObtenerCBU(string dni)
        {
            string cbu = String.Empty;
            
            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings:DW2.ProvMicroDesa")))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("GetCBUByDNI", connection);
                
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@DNI", dni);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cbu = reader.GetString(0); // Assuming the column is of string data type
                    }
                }


                
            }
            
            return cbu;
            
        }

        public string ObtenerFechaAcreditacion(string cuit)
        {
            string date = String.Empty;
            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings:DW2.ProvMicroDesa")))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("GetUltLiqBIPPorCUIT", connection);

                command.CommandType = CommandType.StoredProcedure;

                // Add parameters and set their values

                command.Parameters.AddWithValue("@CUIT", cuit);


                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        date = reader.GetString(0); // Assuming the column is of string data type
                    }
                }


            }
            return date;
            
        }

        internal string ObtenerBranchEjecutivo(string dni)
        {
            string branch = "";
            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings:DW2.ProvMicroDesa")))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SP_getExecutiveBranchAssociatedToDni", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@DNIs", dni);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        branch= reader.GetString(0);
                    }
                }
            }
            return branch; 
        }
        internal string ObtenerCUIT(string dni)
        {
            string cuit = String.Empty;
            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings:DW2.ProvMicroDesa")))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("GetCUITByDNI", connection);

                command.CommandType = CommandType.StoredProcedure;

                // Add parameters and set their values

                command.Parameters.AddWithValue("@DNI", dni);
                

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cuit = reader.GetString(0); 
                    }
                }



            }
            return cuit;
        }

        internal string ObtenerNombreCompletoEjecutivo(string dni)
        {
            string fullName = "";

            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings:DW2.ProvMicroDesa")))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SP_getExecutiveAssociatedToDni", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@DNIs", dni);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        fullName = reader.GetString(0); 
                    }
                }
            }

            return fullName;

        }
    }
}
