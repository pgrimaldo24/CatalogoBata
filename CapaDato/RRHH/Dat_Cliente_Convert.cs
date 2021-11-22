using CapaEntidad.RRHH;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.RRHH
{
    public class Dat_Cliente_Convert
    {
        public List<Ent_Clientes_Convert> lista_clientes(Ent_Clientes_Convert obj)
        {
            string sqlquery = "USP_MVC_LEER_CLIENTES_CONVERT";
            List<Ent_Clientes_Convert> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@usu_id", obj.usu_id);
                        cmd.Parameters.AddWithValue("@usu_tip_out", obj.usu_tip_out);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            lista = new List<Ent_Clientes_Convert>();
                            lista = (from DataRow fila in dt.Rows
                                     select new Ent_Clientes_Convert()
                                     {
                                         id=Convert.ToDecimal(fila["id"]),
                                         nombres= fila["nombres"].ToString(),
                                         dni = fila["dni"].ToString(),
                                         usu_tip_id= fila["usu_tip_id"].ToString(),
                                         usu_tip_des= fila["usu_tip_des"].ToString(),
                                     }
                                   ).ToList();
                        }
                    }
                }
            }
            catch (Exception)
            {

                lista = new List<Ent_Clientes_Convert>();
            }
            return lista;
        }

        public List<Ent_Clientes_Lider_Asesor> lista_clientes_lider_asesor()
        {
            string sqlquery = "USP_MVC_LISTA_LIDER_ASESOR";
            List<Ent_Clientes_Lider_Asesor> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;                       
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            lista = new List<Ent_Clientes_Lider_Asesor>();

                            List<Ent_Clientes_Lider_Asesor> des_d = new List<Ent_Clientes_Lider_Asesor>();
                            Ent_Clientes_Lider_Asesor des = new Ent_Clientes_Lider_Asesor();
                            des.bas_id = "0";
                            des.nombres = "--Ninguno--";
                            des.bas_usu_tipid = "N";
                            des_d.Add(des);

                            lista = (from DataRow fila in dt.Rows
                                     select new Ent_Clientes_Lider_Asesor()
                                     {
                                         bas_usu_tipid = (fila["bas_usu_tipid"] is DBNull) ? string.Empty : (string)(fila["bas_usu_tipid"]) ,
                                         bas_are_id = (fila["bas_are_id"] is DBNull) ? string.Empty : (string)(fila["bas_are_id"]),
                                         bas_id = (fila["bas_id"] is DBNull) ? string.Empty : (string)(fila["bas_id"]),
                                         nombres = (fila["nombres"] is DBNull) ? string.Empty : (string)(fila["nombres"]),
                                         bas_aco_id = (fila["bas_aco_id"] is DBNull) ? string.Empty : (string)(fila["bas_aco_id"]),
                                         npromotor= (fila["npromotor"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["npromotor"]),
                                         documento = (fila["documento"] is DBNull) ? string.Empty : (string)(fila["documento"]),
                                         direccion = (fila["direccion"] is DBNull) ? string.Empty : (string)(fila["direccion"]),
                                         celular = (fila["celular"] is DBNull) ? string.Empty : (string)(fila["celular"]),
                                         correo = (fila["correo"] is DBNull) ? string.Empty : (string)(fila["correo"]),
                                     }
                                   ).ToList();
                            lista = des_d.Union(lista).ToList();
                        }
                    }
                }
            }
            catch (Exception exc)
            {

                lista = new List<Ent_Clientes_Lider_Asesor>();
            }
            return lista;
        }

        public string convert_usuarios(string _bas_id,string _bas_lider,string _tipo_convert,decimal _usu_id_act)
        {
            string valida = "";
            string sqlquery = "[USP_MVC_CONVERT_USUARIOS]";
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
                            cmd.Parameters.AddWithValue("@bas_id",_bas_id);
                            cmd.Parameters.AddWithValue("@bas_lider", _bas_lider);
                            cmd.Parameters.AddWithValue("@tipo_convert", _tipo_convert);
                            cmd.Parameters.AddWithValue("@usu_id_act", _usu_id_act);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception exc)
                    {
                        valida = exc.Message;
                    }
                    if (cn != null)
                        if (cn.State == ConnectionState.Open) cn.Close();
                }
            }
            catch (Exception exc)
            {
                valida = exc.Message;                
            }
            return valida;
        }

        public string Update_relaciona_asesor_lider(string _bas_id_asesor, string _bas_id_lider, string _usu_id_act)
        {
            string valida = "";
            string sqlquery = "[USP_MVC_Actualiza_RelLider_Asesor]";
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
                            cmd.Parameters.AddWithValue("@bas_id_asesor", _bas_id_asesor);
                            cmd.Parameters.AddWithValue("@bas_id_lider", _bas_id_lider);
                            cmd.Parameters.AddWithValue("@usu_id_act", _usu_id_act);                            
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception exc)
                    {
                        valida = exc.Message;
                    }
                    if (cn != null)
                        if (cn.State == ConnectionState.Open) cn.Close();
                }
            }
            catch (Exception exc)
            {
                valida = exc.Message;
            }
            return valida;
        }

        public string desasociar_relaciona_asesor_lider(string _bas_id_lider, string _bas_aco_id, string _usu_id_act)
        {
            string valida = "";
            string sqlquery = "[USP_MVC_DESASOCIAR_LIDER_ASESOR]";
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
                            cmd.Parameters.AddWithValue("@bas_idlider", _bas_id_lider);
                            cmd.Parameters.AddWithValue("@bas_aco_id", _bas_aco_id);
                            cmd.Parameters.AddWithValue("@usu_id_act", _usu_id_act);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception exc)
                    {
                        valida = exc.Message;
                    }
                    if (cn != null)
                        if (cn.State == ConnectionState.Open) cn.Close();
                }
            }
            catch (Exception exc)
            {
                valida = exc.Message;
            }
            return valida;
        }
    }
}
