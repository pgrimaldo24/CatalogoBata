﻿using System;
using CapaEntidad.Util;
using CapaEntidad.Persona;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad.Pedido;

namespace CapaDato.Persona
{
    public class Dat_Persona
    {       
       
        public string strBuscarPersona(string nroDocumento)
        {
            string strJson = "";
            try
            {
                SqlConnection cn = new SqlConnection(Ent_Conexion.conexion);
                cn.Open();
                SqlCommand oComando = new SqlCommand("USP_LEER_PERSONA_USUARIO_MVC", cn);
                oComando.CommandType = CommandType.StoredProcedure;

                SqlParameter oDocumento = oComando.Parameters.Add("@bas_documento", SqlDbType.VarChar);
                oDocumento.Direction = ParameterDirection.Input;
                oDocumento.Value = nroDocumento;

                SqlDataReader oReader = oComando.ExecuteReader(CommandBehavior.SingleResult);
                DataTable dataTable = new DataTable("row");
                dataTable.Load(oReader);

                strJson = JsonConvert.SerializeObject(dataTable, Newtonsoft.Json.Formatting.Indented);
                strJson = strJson.Replace(Environment.NewLine, "");
                //strJson = strJson.Replace(" ", "");
                cn.Close();
            }
            catch (Exception ex)
            {
                return strJson;
            }
            //return oLista;
            return strJson;
        }

        public Ent_Persona GET_INFO_PERSONA (string codigo)
        {
            string sqlquery = "USP_Leer_Persona_Usuario";
            Ent_Persona info = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@bas_id", codigo);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            info = (from DataRow dr in dt.Rows
                                    select new Ent_Persona()
                                    {
                                        Bas_Documento = Convert.ToString(dr["Bas_Documento"]),
                                        NombreCompleto = Convert.ToString(dr["NombreCompleto"]),
                                        Bas_Direccion = Convert.ToString(dr["Bas_Direccion"]),
                                        Bas_Telefono = Convert.ToString(dr["Bas_Telefono"]),
                                        Bas_Celular = Convert.ToString(dr["Bas_Celular"]),
                                        asesor = Convert.ToString(dr["asesor"]),
                                        Are_Descripcion = Convert.ToString(dr["Are_Descripcion"]),
                                        Ubicacion = Convert.ToString(dr["Ubicacion"]),
                                        Bas_Correo = Convert.ToString(dr["Bas_Correo"]),
                                        bas_agencia = Convert.ToString(dr["bas_agencia"]),
                                        bas_destino = Convert.ToString(dr["bas_destino"]),
                                        Bas_id = Convert.ToString(dr["bas_id"]),
                                        _commission = Convert.ToDecimal(dr["Con_Fig_PorcDesc"]),
                                        _taxRate = Convert.ToDecimal(dr["Con_Fig_Igv"]),
                                        _commission_POS_visaUnica = Convert.ToDecimal(dr["Con_Fig_PorcDescPos"]),
                                        _percepcion = Convert.ToDecimal(dr["Con_Fig_Percepcion"]),
                                        Bas_Per_Tip_Id = Convert.ToString(dr["Bas_Per_Tip_Id"]),
                                        _aplica_percepcion = Convert.ToBoolean(dr["aplica_percepcion"]),
                                        premio = Convert.ToString(dr["Premio"]),
                                        desp_cod = Convert.ToString(dr["desp_cod"]),
                                        desp_des = Convert.ToString(dr["desp_des"]),
                                        bas_agencia_direccion= Convert.ToString(dr["bas_agencia_direccion"]),
                                        bas_referencia = Convert.ToString(dr["bas_referencia"]),
                                        bas_distrito = Convert.ToString(dr["dis_descripcion"]),
                                        bas_tipo_dis = Convert.ToString(dr["bas_tipo_dis"]),

                                        lider_agencia = Convert.ToString(dr["lider_agencia"]),
                                        lider_agencia_direccion = Convert.ToString(dr["lider_agencia_direccion"]),
                                        lider_destino = Convert.ToString(dr["lider_destino"]),
                                        lider_direccion = Convert.ToString(dr["lider_direccion"]),
                                        lider_referencia = Convert.ToString(dr["lider_referencia"]),
                                        lider_distrito = Convert.ToString(dr["lider_distrito"]),
                                        bas_provincia = Convert.ToString(dr["bas_provincia"]),
                                        lider_provincia = Convert.ToString(dr["lider_provincia"]),
                                        lider_bas_tipo_dis = Convert.ToString(dr["lider_bas_tipo_dis"]),

                                    }).FirstOrDefault();
                        }
                    }
                    if (cn != null)
                        if (cn.State == ConnectionState.Open) cn.Close();
                }
            }
            catch (Exception ex)
            {
                info = null;
            }
            return info;
        }

        public List<Ent_Pago_NCredito> get_nota_credito(string custId, string strLiqId)
        {
            string sqlquery = "USP_Leer_Pago_Liq";
            List< Ent_Pago_NCredito> info = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@bas_id", custId);
                        cmd.Parameters.AddWithValue("@liq_id", strLiqId);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            info = new List<Ent_Pago_NCredito>();
                            info = (from DataRow dr in dt.Rows
                                    select new Ent_Pago_NCredito()
                                    {
                                        Consumido = Convert.ToBoolean(dr["checks"]),
                                        Activado = Convert.ToBoolean(dr["active"]),
                                        Ncredito = dr["ncredito"].ToString(),
                                        Importe = Convert.ToDecimal(dr["importe"]),
                                        Rhv_return_nro = dr["rhv_return_no"].ToString(),
                                        Fecha_documento = Convert.ToDateTime(dr["dtd_document_date"]),
                                    }).ToList();
                        }
                    }
                    if (cn != null)
                        if (cn.State == ConnectionState.Open) cn.Close();
                }
            }
            catch (Exception ex)
            {
                info = null;
            }
            return info;
        }

        public List<Ent_Persona> Listar_Usuario_tipo(Ent_Persona Ent)
        {
            List<Ent_Persona> Listar = new List<Ent_Persona>();
            string sqlquery = "[USP_Leer_Usuario_Tipo]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@usu_tip_id", DbType.String).Value = Ent.Usu_Tip_ID;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Persona>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Persona()
                                      {
                                          Codigo = Convert.ToInt32(fila["Bas_Id"]),
                                          Descripcion = fila["nombres"].ToString()
                                      }
                                    ).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Listar;
        }
    }
}
