using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapaPresentacion.Models.Crystal
{
    public class ReporteDevolucion
    {
        #region < ATRIBUTOS >

        /// <summary>
        /// Atributos, información del coordinador
        /// </summary>
        public String RHN_COORDINATOR;
        public String bdv_email;
        public String completeNameCustomer;
        public String BDV_DOCUMENT_NO;
        public String BDV_ADDRESS;
        public String BDV_PHONE;
        public String ubicationCustomer;

        /// <summary>
        /// Atributos, informacion persona a quien se le facturo
        /// </summary>
        public String facturadoDestinatario;
        public String facturadoUbicacion;
        public String facturadoTelefono;


        /// <summary>
        /// Atributos, Informacion de la bodega donde se realiza la devolucion
        /// </summary>
        public String wavDescription;
        public String wavAddress;
        public String wavPhone;
        public String wavUbication;


        /// <summary>
        /// Atributos, informacion de la cabecera de la devolucion
        /// </summary>
        public String RHV_CO;
        public String RHV_RETURN_NO;
        public DateTime RHD_DATE;
        public String RHV_TRANSACTION;
        public String RHN_PERSON;
        public String RHN_EMPLOYEE;

        /// <summary>
        /// Atributos, del detalle de la devolucion
        /// </summary>
        public String RDV_INVOICE;
        public String RDV_ARTICLE;
        public String nomArticle;
        public String descriptionArticle;
        public String RDV_SIZE;
        public Decimal RDN_QTY;
        public Decimal RDN_SELLPRICE;
        public Decimal RDN_DISSCOUNT_LIN;
        public Decimal RDN_COMMISSION;
        public Decimal RDN_HANDLING;
        public Decimal RDN_DISSCOUNT_GEN;
        public Decimal RDN_TAXES;
        public String mcpConcepto;
        public DateTime FecEmisionFactura;
        public string RHV_RHN_NUM_NOTA_CRED;

        public Decimal SUBTOTAL;
        public Decimal IGV;
        public Decimal TOTAL;
        //agregando percepcion
        public Decimal percepcion;
        public Decimal porc_percepcion;

        public string vdireccionfiscal;

        #endregion


        #region < CONSTRUCTORES >

        public ReporteDevolucion(String RHN_COORDINATOR, String bdv_email, String completeNameCustomer,
            String BDV_DOCUMENT_NO, String BDV_ADDRESS, String BDV_PHONE, String ubicationCustomer,
            String facturadoDestinatario, String facturadoUbicacion, String facturadoTelefono,
            String wavDescription, String wavAddress, String wavPhone, String wavUbication,
            String RHV_CO,
            String RHV_RETURN_NO, DateTime RHD_DATE, String RHV_TRANSACTION, String RHN_PERSON, String RHN_EMPLOYEE,
            String RDV_INVOICE, String RDV_ARTICLE, String nomArticle, String descriptionArticle,
            String RDV_SIZE, Decimal RDN_QTY, Decimal RDN_SELLPRICE,
            Decimal RDN_DISSCOUNT_LIN, Decimal RDN_COMMISSION, Decimal RDN_HANDLING, Decimal RDN_DISSCOUNT_GEN,
            Decimal RDN_TAXES, string mcpConcepto, DateTime FecEmisionFactura, string varNotaCredito, Decimal varSubTotal,
            Decimal varIGV, Decimal varTotal, Decimal varpercepcion, Decimal varporcpercepcion, string direccionfiscal)
        {
            //
            // TODO: Agregar aquí la lógica del constructor
            //
            /// <summary>
            /// Atributos, información del coordinador
            /// </summary>
            _RHN_COORDINATOR = RHN_COORDINATOR;
            _bdv_email = bdv_email;
            _completeNameCustomer = completeNameCustomer;
            _BDV_DOCUMENT_NO = BDV_DOCUMENT_NO;
            _BDV_ADDRESS = BDV_ADDRESS;
            _BDV_PHONE = BDV_PHONE;
            _ubicationCustomer = ubicationCustomer;

            /// <summary>
            /// Atributos, informacion persona a quien se le facturo
            /// </summary>
            _facturadoDestinatario = facturadoDestinatario;
            _facturadoUbicacion = facturadoUbicacion;
            _facturadoTelefono = facturadoTelefono;

            /// <summary>
            /// Atributos, Informacion de la bodega donde se realiza la devolucion
            /// </summary>
            _wavDescription = wavDescription;
            _wavAddress = wavAddress;
            _wavPhone = wavPhone;
            _wavUbication = wavUbication;

            /// <summary>
            /// Atributos, informacion de la cabecera de la devolucion
            /// </summary>
            _RHV_CO = RHV_CO;
            _RHV_RETURN_NO = RHV_RETURN_NO;
            _RHD_DATE = RHD_DATE;
            _RHV_TRANSACTION = RHV_TRANSACTION;
            _RHN_PERSON = RHN_PERSON;
            _RHN_EMPLOYEE = RHN_EMPLOYEE;

            /// <summary>
            /// Atributos, del detalle de la devolucion
            /// </summary>
            _RDV_INVOICE = RDV_INVOICE;
            _RDV_ARTICLE = RDV_ARTICLE;
            _RDV_SIZE = RDV_SIZE;
            _nomArticle = nomArticle;
            _descriptionArticle = descriptionArticle;
            _RDN_QTY = RDN_QTY;
            _RDN_SELLPRICE = RDN_SELLPRICE;
            _RDN_DISSCOUNT_LIN = RDN_DISSCOUNT_LIN;
            _RDN_COMMISSION = RDN_COMMISSION;
            _RDN_HANDLING = RDN_HANDLING;
            _RDN_DISSCOUNT_GEN = RDN_DISSCOUNT_GEN;
            _RDN_TAXES = RDN_TAXES;
            _mcpConcepto = mcpConcepto;
            _FecEmisionFactura = FecEmisionFactura;
            _RHV_RHN_NUM_NOTA_CRED = varNotaCredito;
            _SUBTOTAL = varSubTotal;
            _IGV = varIGV;
            _TOTAL = varTotal;

            //ahora igualmamos a la percepcion
            _Percepcion = varpercepcion;
            _Porc_Percepcion = varporcpercepcion;

            _direccionfiscal = direccionfiscal;
        }

        #endregion

        #region < SETTER's AND GETTER's>

        /// <summary>
        /// Atributos, información del coordinador
        /// </summary>
        /// 
        public string _direccionfiscal
        {
            get { return vdireccionfiscal; }
            set { vdireccionfiscal = value; }
        }
        public Decimal _SUBTOTAL
        {
            get { return SUBTOTAL; }
            set { SUBTOTAL = value; }
        }

        public Decimal _IGV
        {
            get { return IGV; }
            set { IGV = value; }
        }

        public Decimal _TOTAL
        {
            get { return TOTAL; }
            set { TOTAL = value; }
        }
        public Decimal _Percepcion
        {
            get { return percepcion; }
            set { percepcion = value; }
        }
        public Decimal _Porc_Percepcion
        {
            get { return porc_percepcion; }
            set { porc_percepcion = value; }
        }

        public String _RHN_COORDINATOR
        {
            get { return RHN_COORDINATOR; }
            set { RHN_COORDINATOR = value; }
        }
        public String _bdv_email
        {
            get { return bdv_email; }
            set { bdv_email = value; }
        }
        public String _completeNameCustomer
        {
            get { return completeNameCustomer; }
            set { completeNameCustomer = value; }
        }
        public String _BDV_DOCUMENT_NO
        {
            get { return BDV_DOCUMENT_NO; }
            set { BDV_DOCUMENT_NO = value; }
        }
        public String _BDV_ADDRESS
        {
            get { return BDV_ADDRESS; }
            set { BDV_ADDRESS = value; }
        }
        public String _BDV_PHONE
        {
            get { return BDV_PHONE; }
            set { BDV_PHONE = value; }
        }
        public String _ubicationCustomer
        {
            get { return ubicationCustomer; }
            set { ubicationCustomer = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public String _facturadoDestinatario
        {
            get { return facturadoDestinatario; }
            set { facturadoDestinatario = value; }
        }
        public String _facturadoUbicacion
        {
            get { return facturadoUbicacion; }
            set { facturadoUbicacion = value; }
        }
        public String _facturadoTelefono
        {
            get { return facturadoTelefono; }
            set { facturadoTelefono = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public String _wavDescription
        {
            get { return wavDescription; }
            set { wavDescription = value; }
        }
        public String _wavAddress
        {
            get { return wavAddress; }
            set { wavAddress = value; }
        }
        public String _wavPhone
        {
            get { return wavPhone; }
            set { wavPhone = value; }
        }
        public String _wavUbication
        {
            get { return wavUbication; }
            set { wavUbication = value; }
        }

        /// <summary>
        /// Atributos, informacion de la cabecera de la devolucion
        /// </summary>
        public String _RHV_CO
        {
            get { return RHV_CO; }
            set { RHV_CO = value; }
        }
        public String _RHV_RETURN_NO
        {
            get { return RHV_RETURN_NO; }
            set { RHV_RETURN_NO = value; }
        }
        public DateTime _RHD_DATE
        {
            get { return RHD_DATE; }
            set { RHD_DATE = value; }
        }
        public String _RHV_TRANSACTION
        {
            get { return RHV_TRANSACTION; }
            set { RHV_TRANSACTION = value; }
        }
        public String _RHN_PERSON
        {
            get { return RHN_PERSON; }
            set { RHN_PERSON = value; }
        }
        public String _RHN_EMPLOYEE
        {
            get { return RHN_EMPLOYEE; }
            set { RHN_EMPLOYEE = value; }
        }


        /// <summary>
        /// Atributos, del detalle de la devolucion
        /// </summary>
        public String _RDV_INVOICE
        {
            get { return RDV_INVOICE; }
            set { RDV_INVOICE = value; }
        }
        public String _RDV_ARTICLE
        {
            get { return RDV_ARTICLE; }
            set { RDV_ARTICLE = value; }
        }
        public String _RDV_SIZE
        {
            get { return RDV_SIZE; }
            set { RDV_SIZE = value; }
        }
        public String _nomArticle
        {
            get { return nomArticle; }
            set { nomArticle = value; }
        }
        public String _descriptionArticle
        {
            get { return descriptionArticle; }
            set { descriptionArticle = value; }
        }
        public Decimal _RDN_QTY
        {
            get { return RDN_QTY; }
            set { RDN_QTY = value; }
        }
        public Decimal _RDN_SELLPRICE
        {
            get { return RDN_SELLPRICE; }
            set { RDN_SELLPRICE = value; }
        }
        public Decimal _RDN_DISSCOUNT_LIN
        {
            get { return RDN_DISSCOUNT_LIN; }
            set { RDN_DISSCOUNT_LIN = value; }
        }
        public Decimal _RDN_COMMISSION
        {
            get { return RDN_COMMISSION; }
            set { RDN_COMMISSION = value; }
        }
        public Decimal _RDN_HANDLING
        {
            get { return RDN_HANDLING; }
            set { RDN_HANDLING = value; }
        }
        public Decimal _RDN_DISSCOUNT_GEN
        {
            get { return RDN_DISSCOUNT_GEN; }
            set { RDN_DISSCOUNT_GEN = value; }
        }
        public Decimal _RDN_TAXES
        {
            get { return RDN_TAXES; }
            set { RDN_TAXES = value; }
        }
        public DateTime _FecEmisionFactura
        {
            get { return FecEmisionFactura; }
            set { FecEmisionFactura = value; }
        }
        public String _mcpConcepto
        {
            get { return mcpConcepto; }
            set { mcpConcepto = value; }
        }
        public String _RHV_RHN_NUM_NOTA_CRED
        {
            get { return RHV_RHN_NUM_NOTA_CRED; }
            set { RHV_RHN_NUM_NOTA_CRED = value; }
        }


        #endregion
    }
}