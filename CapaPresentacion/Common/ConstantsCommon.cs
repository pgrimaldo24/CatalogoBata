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
    }
}