using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.PasarelaPago
{
    public class WebhooksResponseDto
    {
        public long id { get; set; }
        public bool live_mode { get; set; }
        public string type { get; set; }
        public DateTime date_created { get; set; }
        public long application_id { get; set; }
        public int user_id { get; set; }
        public int version { get; set; }
        public string api_version { get; set; }
        public string action { get; set; }
        public DataDto data { get; set; }
    }

    public class DataDto
    {
        public string id { get; set; }
    }
}
