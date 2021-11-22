using CapaEntidad.Logistica;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Logistica
{
    public class Dat_Gestion_Stock
    {
        public Ent_Gestion_Stock get_gestion(DataTable dt)
        {
            string sqlquery = "[USP_MVC_GESTION_STOCK]";
            Ent_Gestion_Stock obj = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@tmp",dt);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            DataTable dt_medida = new DataTable("Medida");
                            DataTable dt_detalle = new DataTable("Detalle");
                            dt_medida = ds.Tables[0];
                            dt_detalle = ds.Tables[1];

                            List<Ent_Gestion_Stock_Medida> list_medida = new List<Ent_Gestion_Stock_Medida>();
                            list_medida = (from DataRow fila in dt_medida.Rows
                                           select new Ent_Gestion_Stock_Medida()
                                           {
                                               cod_rgmed=fila["cod_rgmed"].ToString(),
                                               reg_med = fila["reg_med"].ToString(),
                                               talla = fila["talla"].ToString(),
                                           }
                                         ).ToList();

                            List<Ent_Gestion_Stock_Detalle> list_detalle = new List<Ent_Gestion_Stock_Detalle>();
                            list_detalle = (from DataRow fila in dt_detalle.Rows
                                           select new Ent_Gestion_Stock_Detalle()
                                           {
                                               item =Convert.ToInt32(fila["item"]),
                                               almacen = fila["almacen"].ToString(),
                                               articulo = fila["articulo"].ToString(),
                                               talla = fila["talla"].ToString(),
                                               stock = Convert.ToInt32(fila["stock"]),
                                               regmed = fila["regmed"].ToString(),
                                               med_per = fila["med_per"].ToString(),
                                               foto= fila["foto"].ToString(),
                                           }
                                         ).ToList();

                            obj = new Ent_Gestion_Stock();
                            obj.gestion_medida = list_medida;
                            obj.gestion_detalle = list_detalle;

                        }
                    }
                }
            }
            catch (Exception exc)
            {

                throw exc;
            }
            return obj;
        }
    }
}
