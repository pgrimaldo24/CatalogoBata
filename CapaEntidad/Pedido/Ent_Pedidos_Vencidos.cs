using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Pedido
{
    public class Ent_Pedidos_Vencidos
    {
        public string pedido { get; set; }
        public string asesor { get; set; }
        public string lider { get; set; }
        public string promotor { get; set; }
        public string fechapedido { get; set; }
        public string fechaven { get; set; }
        public Int32 pares { get; set; }

        public string Bas_Id { get; set; }
        public DateTime FechaIni { get; set; }
        public DateTime FechaFin { get; set; }
        public string Bas_Aco_Id { get; set; }

    }
}
