namespace CapaEntidad.PasarelaPago
{
    public class CardToken
    {
        public string token_public { get; set; }
        public string card_number { get; set; }
        public string security_code { get; set; }
        public int expiration_month { get; set; }
        public int expiration_year { get; set; }
        public CardHolderDto cardholder { get; set; }
    }

    public class CardHolderDto
    {
        public string name { get; set; }
        public IndetificationDto identification { get; set; }
    }

    public class IndetificationDto
    {
        public string number { get; set; }
        public string type { get; set; }
    }
}
