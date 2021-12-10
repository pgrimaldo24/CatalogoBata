using System.Collections.Generic;

namespace CapaEntidad.PasarelaPago
{
    public class CardDto
    {
        public string TokenPublic { get; set; }
        public string numeroTarjeta { get; set; }
        public string codigoSeguridad { get; set; }
        public int? fechaExpiracionMes { get; set; }
        public int? fechaExpiracionAnio { get; set; }
        public string nombreCompletoTitular { get; set; }
        public string numeroDocumento { get; set; }
        public string tipoDocumento { get; set; }
        public string os_device { get; set; }
        public string system_version { get; set; }
        public int? ram { get; set; }
        public int? disk_space { get; set; }
        public string model { get; set; }
        public int? free_disk_space { get; set; }
        public bool? feature_flash { get; set; }
        public bool? can_make_phone_calls { get; set; }
        public bool? can_send_sms { get; set; }
        public bool? video_camera_available { get; set; }
        public int? cpu_count { get; set; }
        public bool? simulator { get; set; }
        public string device_languaje { get; set; }
        public string device_idiom { get; set; }
        public string platform { get; set; }
        public string device_name { get; set; }
        public int? device_family { get; set; }
        public bool? retina_display_capable { get; set; }
        public bool? feature_camera { get; set; }
        public string device_model { get; set; }
        public bool? feature_front_camera { get; set; }
        public string resolution { get; set; }
        public List<VendorId> vendor_ids { get; set; }
    }
}
