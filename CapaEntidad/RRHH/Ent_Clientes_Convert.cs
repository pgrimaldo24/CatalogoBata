using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.RRHH
{
    public class Ent_Clientes_Convert
    {
        public Decimal id { get; set; }
        public string nombres { get; set; }
        public string dni { get; set; }
        public string usu_tip_id { get; set; }
        public string usu_tip_des { get; set; }

        /*parametros de busqueda*/
        public Decimal usu_id { get; set; }
        public string usu_tip_out { get; set; }
    }

    public class Ent_Clientes_Lider_Asesor
    {
        public string bas_usu_tipid { get; set; }
        public string bas_are_id { get; set; }
        public string bas_id{ get; set; }
        public string nombres { get; set; }
        public string bas_aco_id { get; set; }
        public Decimal? npromotor { get; set; }

        public string documento { get; set; }
        public string direccion { get; set; }
        public string celular { get; set; }
        public string correo { get; set; }

    }
}
