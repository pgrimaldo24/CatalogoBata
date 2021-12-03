namespace CapaEntidad.PasarelaPago
{
    public class CardTokenResponseDto
    {
        public ResponseToken response { get; set; } 
    }

    public class CardHolder
    {
        public IdentificationCardHolder identification { get; set; }
        public string name { get; set; }
    }

    public class IdentificationCardHolder
    {
        public string type { get; set; }
    }

    public class ResponseToken
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public RootDto Data { get; set; }
       
    }

    public class RootDto
    {
        public string id { get; set; }
        public string public_key { get; set; }
        public int expiration_month { get; set; }
        public int expiration_year { get; set; }
        public CardHolder cardholder { get; set; }
        public string status { get; set; }
        public string date_created { get; set; }
        public string date_last_updated { get; set; }
        public string date_due { get; set; }
        public bool luhn_validation { get; set; }
        public bool live_mode { get; set; }
        public bool require_esc { get; set; }
        public int security_code_length { get; set; }
    }
}
