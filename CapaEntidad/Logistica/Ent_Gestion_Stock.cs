using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Logistica
{

    public class Ent_Gestion_Stock
    {
        public List<Ent_Gestion_Stock_Detalle> gestion_detalle { get; set; }
        public List<Ent_Gestion_Stock_Medida> gestion_medida { get; set; }
    }

    public class Ent_Gestion_Stock_Detalle
    {
        public Int32 item { get; set; }
        public string almacen { get; set; }
        public string articulo { get; set; }
        public string talla { get; set; }
        public Int32 stock { get; set; }
        public string regmed { get; set; }
        public string med_per { get; set; }
        public string foto { get; set; }
    }
    public class Ent_Gestion_Stock_Medida
    {
        public string cod_rgmed { get; set; }
        public string reg_med { get; set; }
        public string talla { get; set; }
    }
}
