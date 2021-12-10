namespace CapaPresentacion.Common
{
    public class ConstantsCommon
    {
        public struct EndPointCatalogoPago
        {
            public const string EndPointCatalogoPagoAuth = "EndPointCatalogoPagoAuth";
            public const string EndPointCatalogoPagoProccessPayment = "EndPointCatalogoPagoProccessPayment";
            public const string EndPointCatalogoPagoProccessCardToken = "EndPointCatalogoPagoCardToken";
        }

        public struct TokenMercadoPago
        {
            public const string PUBLIC_KEY_MERCADO_PAGO = "PUBLIC_KEY_MERCADO_PAGO";
        }
        public struct TypeRequest
        {
            public const string POST = "POST";
            public const string PUT = "PUT";
            public const string GET = "GET";
            public const string DELETE = "DELETE";
        }

        public struct Credentials_CatalogoPago
        {
            public const string User = "user_catalogo";
            public const string Password = "password_catalogo";
        }

        public struct CodeType
        {
            public const string Error = "-1"; 
        }

        public struct Resources
        {
            public const string ErrorPostInvoque = "Error de comunicación con el método del PostInvoque.";
            public const string ErrorTarjetaInvalida = "La tarjeta es inválida, verifica tus datos.";
            public const string ErrorSetCardToken = "Error de comunicación con el método SetCardToken.";
            public const string AREA_CODE_PE = "+51";
        }

        public struct Method
        {
            public const string PROCCESS_PAYMENT = "ProccessPayment";
            public const string CARDTOKEN = "CardToken";
        }
    }
}