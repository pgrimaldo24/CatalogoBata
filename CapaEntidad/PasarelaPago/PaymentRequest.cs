using System;
using System.Collections.Generic;

namespace CapaEntidad.PasarelaPago
{
    public class PaymentRequest
    {
        public AdditionalInfoPaymentDto additional_info { get; set; } 
        public bool? binary_mode { get; set; } 
        public bool? capture { get; set; } 
        public string external_reference { get; set; }
        public int? installments { get; set; }  
        public MetadataDto metadata { get; set; }
        public string notification_url { get; set; } 
        public PayerRequestDto payer { get; set; } 
        public string payment_method_id { get; set; } 
        public string token { get; set; }
        public decimal? transaction_amount { get; set; }
    }
    public class AdditionalInfoPaymentDto
    {
        public List<AdditionalInfoDto> items { get; set; }
        public PayerMercadoPagoDto payer { get; set; }
        public ReceiverAddressDto shipments { get; set; }
    }

    public class MetadataDto
    { 
    }


    public class ReceiverAddressDto
    {
        public Receiver_AddressDto receiver_address { get; set; }
    }

    public class Receiver_AddressDto
    {
        public string zip_code { get; set; }
        public string state_name { get; set; }
        public string city_name { get; set; }
        public string street_name { get; set; }
        public string street_number { get; set; }
    }

    public class BardcodeDto
    {
        public string type { get; set; }
        public string content { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
    }

    public class OrderDto
    {
        public string type { get; set; }
        public int? id { get; set; }
    }

    public class PayerRequestDto
    {
        //public string entity_type { get; set; }
        //public string type { get; set; }
        //public string id { get; set; }

        public string email { get; set; }

        public PayerIdentificationDto identification { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }

    public class AdditionalInfoDto
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string picture_url { get; set; }
        public string category_id { get; set; }
        public int? quantity { get; set; }
        public decimal? unit_price { get; set; }
    }
    public class PayerIdentificationDto
    {
        public string type { get; set; }
        public string number { get; set; }
    }
    public class PayerMercadoPagoDto
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public PhoneDto phone { get; set; }
        public AddressDto address { get; set; }
    }

    public class PhoneDto
    {
        public string area_code { get; set; }
        public string number { get; set; }
    }
    public class AddressDto
    {
        public string zip_code { get; set; }
        public string street_name { get; set; }
        public string street_number { get; set; }
    }
}
