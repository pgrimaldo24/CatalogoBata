using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Pedido
{
    public  class Ent_Pedido_Flete
    {
        public string pedido { get; set; }
        public string fecha { get; set; }
        public Int32 pares { get; set; }
        public string estado { get; set; }
        public decimal ganancia { get; set; }
        public decimal subtotal { get; set; }
        public decimal pagoncsf { get; set; }
        public Decimal total { get; set; }
        public decimal percepcion { get; set; }
        public decimal tpagar { get; set; }

        public decimal bas_id { get; set; }
        public DateTime fec_ini { get; set; }
        public DateTime fec_fin { get; set; }

    }
}
