using System;
using System.Collections.Generic;

namespace CapaEntidad.PasarelaPago
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class IdentificationRoot
    {
        public string type { get; set; }
        public string number { get; set; }
    }

    public class PhoneRoot
    {
        public object area_code { get; set; }
        public object number { get; set; }
        public object extension { get; set; }
    }

    public class PayerRoot
    {
        public object type { get; set; }
        public string id { get; set; }
        public object operator_id { get; set; }
        public string email { get; set; }
        public IdentificationRoot identification { get; set; }
        public PhoneRoot phone { get; set; }
        public object first_name { get; set; }
        public object last_name { get; set; }
        public object entity_type { get; set; }
        public object address { get; set; }
    }

    public class MetadataRoot
    {
    }

    public class ItemRoot
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public object picture_url { get; set; }
        public object category_id { get; set; }
        public string quantity { get; set; }
        public string unit_price { get; set; }
    }

    public class AddressRoot
    {
        public string zip_code { get; set; }
        public string street_name { get; set; }
        public object street_number { get; set; }
    }

    public class ReceiverAddressRoot
    {
        public string zip_code { get; set; }
        public string state_name { get; set; }
        public string city_name { get; set; }
        public string street_name { get; set; }
        public object street_number { get; set; }
    }

    public class ShipmentsRoot
    {
        public ReceiverAddressRoot receiver_address { get; set; }
    }

    public class AdditionalInfoRoot
    {
        public List<ItemRoot> items { get; set; }
        public PayerRoot payer { get; set; }
        public ShipmentsRoot shipments { get; set; }
        public object available_balance { get; set; }
        public object nsu_processadora { get; set; }
        public object authentication_code { get; set; }
    }

    public class OrderRoot
    {
    }

    public class TransactionDetailsRoot
    {
        public object payment_method_reference_id { get; set; }
        public int? net_received_amount { get; set; }
        public double? total_paid_amount { get; set; }
        public int? overpaid_amount { get; set; }
        public object external_resource_url { get; set; }
        public double? installment_amount { get; set; }
        public object financial_institution { get; set; }
        public object payable_deferral_period { get; set; }
        public object acquirer_reference { get; set; }
    }

    public class CardholderRoot
    {
        public string name { get; set; }
        public IdentificationRoot identification { get; set; }
    }

    public class CardRoot
    {
        public object id { get; set; }
        public string first_six_digits { get; set; }
        public string last_four_digits { get; set; }
        public int? expiration_month { get; set; }
        public int? expiration_year { get; set; }
        public DateTime? date_created { get; set; }
        public DateTime? date_last_updated { get; set; }
        public CardholderRoot cardholder { get; set; }
    }

    public class BusinessInfoRoot
    {
        public string unit { get; set; }
        public string sub_unit { get; set; }
    }

    public class PointOfInteractionRoot
    {
        public string type { get; set; }
        public BusinessInfoRoot business_info { get; set; }
    }

    public class DataRoot
    {
        public long id { get; set; }
        public DateTime? date_created { get; set; }
        public object date_approved { get; set; }
        public DateTime? date_last_updated { get; set; }
        public object date_of_expiration { get; set; }
        public object money_release_date { get; set; }
        public string operation_type { get; set; }
        public string issuer_id { get; set; }
        public string payment_method_id { get; set; }
        public string payment_type_id { get; set; }
        public string status { get; set; }
        public string status_detail { get; set; }
        public string currency_id { get; set; }
        public object description { get; set; }
        public bool? live_mode { get; set; }
        public object sponsor_id { get; set; }
        public string authorization_code { get; set; }
        public object money_release_schema { get; set; }
        public int? taxes_amount { get; set; }
        public object counter_currency { get; set; }
        public object brand_id { get; set; }
        public int? shipping_amount { get; set; }
        public object pos_id { get; set; }
        public object store_id { get; set; }
        public object integrator_id { get; set; }
        public object platform_id { get; set; }
        public object corporation_id { get; set; }
        public int? collector_id { get; set; }
        public Payer payer { get; set; }
        public object marketplace_owner { get; set; }
        public Metadata metadata { get; set; }
        public AdditionalInfo additional_info { get; set; }
        public Order order { get; set; }
        public string external_reference { get; set; }
        public double? transaction_amount { get; set; }
        public int? transaction_amount_refunded { get; set; }
        public int? coupon_amount { get; set; }
        public object differential_pricing_id { get; set; }
        public object deduction_schema { get; set; }
        public int? installments { get; set; }
        public TransactionDetails transaction_details { get; set; }
        public List<object> fee_details { get; set; }
        public List<object> charges_details { get; set; }
        public bool? captured { get; set; }
        public bool? binary_mode { get; set; }
        public object call_for_authorize_id { get; set; }
        public string statement_descriptor { get; set; }
        public Card card { get; set; }
        public string notification_url { get; set; }
        public List<object> refunds { get; set; }
        public string processing_mode { get; set; }
        public object merchant_account_id { get; set; }
        public object merchant_number { get; set; }
        public PointOfInteraction point_of_interaction { get; set; }
    }

    public class ResponseRoot
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public DataRoot Data { get; set; }
    }

    public class Root
    {
        public ResponseRoot response { get; set; }
    }


}
