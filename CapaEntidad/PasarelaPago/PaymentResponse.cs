using System;
using System.Collections.Generic;

namespace CapaEntidad.PasarelaPago
{
    public class ResponsePasarelaPago
    {
        public ResponseMP response { get; set; }
    }

    public class ResponseMP
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public PaymentResponse Data { get; set; }
    }

    public class PaymentResponse
    {
        public int? id { get; set; }
        public DateTime? date_created { get; set; }
        public DateTime? date_approved { get; set; }
        public DateTime? date_last_updated { get; set; }
        public DateTime? money_release_date { get; set; }

        public int? issuer_id { get; set; }
        public string payment_method_id { get; set; }

        public string payment_type_id { get; set; }
        public string status { get; set; }
        public string status_detail { get; set; }
        public string currency_id { get; set; }
        public string description { get; set; }
        public int? taxes_amount { get; set; }
        public int? shipping_amount { get; set; }
        public int? collector_id { get; set; }
        public PayerResponse payer { get; set; }
        public IDictionary<string, object> metadata { get; set; }
        public AdditionalInfoPaymentResponseDto additional_info { get; set; }
        public OrderDto order { get; set; }
        public string external_reference { get; set; }
        public decimal? transaction_amount { get; set; }
        public decimal? transaction_amount_refunded { get; set; }
        public int? coupon_amount { get; set; }
        public TransactionDetailsDto transaction_details { get; set; }
        public FeeDetailsDto fee_details { get; set; }

        public string statement_descriptor { get; set; }
        public int? installments { get; set; }

        public CardDto card { get; set; }
        public string notification_url { get; set; }
        public string processing_mode { get; set; }

        public PointOfInteractionDto point_of_interaction { get; set; }

        public string response_api { get; set; }
    }

    public class PayerResponse
    {
        public int? id { get; set; }
        public string email { get; set; }
        public IdentificationDto identification { get; set; }
        public string type { get; set; }
    }

    public class IdentificationDto
    {
        public int? number { get; set; }
        public string type { get; set; }
    }

    public class AdditionalInfoPaymentResponseDto
    {
        public IList<AdditionalInfoDto> items { get; set; }
        public RegistrationPayer payer { get; set; }
        public ReceiverAddressDto shipments { get; set; }
    }
    public class RegistrationPayer
    {
        public DateTime? registration_date { get; set; }
    }

    public class TransactionDetailsDto
    {
        public decimal? net_received_amount { get; set; }
        public decimal? total_paid_amount { get; set; }
        public decimal? overpaid_amount { get; set; }
        public decimal? installment_amount { get; set; }
    }

    public class FeeDetailsDto
    {
        public string type { get; set; }
        public decimal? amount { get; set; }
        public string fee_payer { get; set; }
    }

    public class CardDto
    {
        public int? first_six_digits { get; set; }
        public int? last_four_digits { get; set; }
        public int? expiration_month { get; set; }
        public int? expiration_year { get; set; }
        public DateTime? date_created { get; set; }
        public DateTime? date_last_updated { get; set; }
        public CardHolderResponseDto cardholder { get; set; }
    }

    public class CardHolderResponseDto
    {
        public string name { get; set; }
        public IdentificationDto identification { get; set; }
    }

    public class PointOfInteractionDto
    {
        public string type { get; set; }
        public ApplicationDataDto application_data { get; set; }
        public TransactionDataDto transaction_data { get; set; }

    }

    public class ApplicationDataDto
    {
        public string name { get; set; }
        public string version { get; set; }
    }

    public class TransactionDataDto
    {
        public string qr_code_base64 { get; set; }
        public string qr_code { get; set; }
    }
}
