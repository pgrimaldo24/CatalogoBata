using CapaEntidad.PasarelaPago;
using CapaEntidad.Persona;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.PasarelaPago
{
    public class Dat_PasarelaPago
    {
        public string vendor_id()
        {
            string sqlquery = "USP_MERCADO_PAGO_VENDOR_ID";
            string vendorid = "C2508642-79CF-44E4-A205-284A4F4DE04C";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    try
                    {
                        if (cn.State == 0) cn.Open();
                        using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@VENDOR_ID", SqlDbType.VarChar, 50);
                            cmd.Parameters["@VENDOR_ID"].Direction = ParameterDirection.Output;
                            cmd.ExecuteNonQuery();

                            vendorid =Convert.ToString(cmd.Parameters["@VENDOR_ID"].Value);
                        }
                    }
                    catch 
                    {

                        
                    }
                    if (cn != null)
                        if (cn.State == ConnectionState.Open) cn.Close();
                }
            }
            catch (Exception)
            {

                
            }
            return vendorid;
        }
        public List<EntEstados> ListaEstadoMercadoPago(string entidadKey)
        {
            try
            {
                var estados = new List<EntEstados>();
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("USP_ListarEstados", cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@entidad", entidadKey);
                        EntEstados objEstado;
                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            objEstado = new EntEstados
                            {
                                Key = Convert.ToInt32(dr["Key"]),
                                Description = dr["Description"].ToString()
                            };

                            estados.Add(objEstado);
                        }
                    }
                    return estados;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Ent_Persona GetInformacionUsuario(string dni)
        {
            try
            {
                Ent_Persona persona = new Ent_Persona();
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {

                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("USP_ObtenerInformacionUsuario", cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@dni", dni);
                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            persona.NombreCompleto = dr["NombresCliente"].ToString();
                            persona.ApellidoCompleto = dr["ApellidosCliente"].ToString();
                            persona.Bas_Direccion = dr["Bas_Direccion"].ToString();
                            persona.TipoDocumento = dr["Doc_Tip_Descripcion"].ToString();
                            persona.Bas_Telefono = dr["Bas_Telefono"].ToString();
                            persona.Bas_Celular = dr["Bas_Celular"].ToString();
                            persona.Bas_Correo = dr["Bas_Correo"].ToString();
                            persona.Departamento = dr["Dep_Descripcion"].ToString();
                            persona.Provincia = dr["Prv_Descripcion"].ToString();
                        }
                        cn.Close();
                    }
                }
                return persona;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string InsertDataWebhooks(string request)
        {
            try
            {
                var response = string.Empty;
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("USP_InsertDataWebhooks", cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@request", request);
                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            response = dr["response"].ToString();
                        }
                        cn.Close();
                    }
                }
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string GetPagoServicio(string numero_pago_servicio)
        {
            try
            {
                var response = string.Empty;
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("USP_GetPagoProcesado_MercadoPago", cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@idPagoServicio", numero_pago_servicio);
                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            response = dr["response"].ToString();
                        }
                        cn.Close();
                    }
                }
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

    
 
