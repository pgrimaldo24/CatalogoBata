﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Facturacion
{
    public class Ent_Movimientos_Ventas
    {
        public string Mcv_Description { get; set; }
        public int? Anno { get; set; }
        public int? Can_Week_No { get; set; }
        public Decimal Ventas { get; set; }
        public Decimal Podv { get; set; }
        public Decimal Pventas { get; set; }
        public Decimal Pventasneto { get; set; }
        public Decimal Pmargen { get; set; }
        public Decimal Pmargenpor { get; set; }

        //campo adicionales de buscqueda
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string IdTipoArticulo { get; set; }
    }
    public class Ent_Movimientos_Ventas_Chart
    {
        public string label { get; set; }
        public string[] backgroundColor { get; set; }
        public string borderWidth { get; set; }
        public decimal[] data { get; set; }
    }
    public class Ent_Movimientos_Ventas_Chart_Data
    {
        public string[] labels { get; set; }
        public string[] labelsTooltip { get; set; }
        public List<Ent_Movimientos_Ventas_Chart> datasets { get; set; }
    }

    public class Ent_Comisiones
    {
        public int AreaId { get; set; }
        public string Asesor { get; set; }
        public string Lider { get; set; }
        public string LiderDni { get; set; }
        public Decimal? TotPares { get; set; }
        public Decimal? TotVenta { get; set; }
        public Decimal? PorComision { get; set; }
        public Decimal? Comision { get; set; }
        public Decimal? Bonosnuevas { get; set; }
        public Decimal? SubTotalSinIGV { get; set; }
        //campos adicionales de busqueda
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
    public class Ent_Resumen_Ventas
    {
        public int? Anno { get; set; }
        public int? Semana { get; set; }
        public int? TotalTickets { get; set; }
        public int? Pares { get; set; }
        public Decimal? TotalIgv { get; set; }
        public Decimal? PrecioPromedio { get; set; }
        public Decimal? NParesTicket { get; set; }
        public int? Anno1 { get; set; }
        public int? Semana1 { get; set; }
        public int? TotalTickets1 { get; set; }
        public int? Pares1 { get; set; }
        public Decimal? TotalIgv1 { get; set; }
        public Decimal? PrecioPromedio1 { get; set; }
        public Decimal? NParesTicket1 { get; set; }
        //Campos condicionales de busqueda
        public int Codigo { get; set; }
        public int Descripcion { get; set; }
    }

    public class Ent_Lider_Ventas
    {
        public string asesora { get; set; }
        public string lider { get; set; }
        public string departamento { get; set; }
        public string provincia { get; set; }
        public string distrito { get; set; }
        public Decimal Enero { get; set; }
        public Decimal Febrero { get; set; }
        public Decimal Marzo { get; set; }
        public Decimal Abril { get; set; }
        public Decimal Mayo { get; set; }
        public Decimal Junio { get; set; }
        public Decimal Julio { get; set; }
        public Decimal Agosto { get; set; }
        public Decimal Septiembre { get; set; }
        public Decimal Octubre { get; set; }
        public Decimal Noviembre { get; set; }
        public Decimal Diciembre { get; set; }
        public Decimal Grand_Total {get; set;}
        //campos adicionales de busqueda
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class Ent_Ventas_Tallas
    {
        public string Concepto { get; set; }
        public string Articulo { get; set; }
        public Decimal TotalParesVenta { get; set; }
        public Decimal TotalParesStock { get; set; }
        //Detalle
        public List<Ent_Ventas_Talla_Detalle> _ListarDetalle_Venta { get; set; }
        public List<Ent_Ventas_Talla_Detalle> _ListarDetalle_Stock { get; set; }


        //campos adicionales de busqueda
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
    }
    public class Ent_Ventas_Talla_Detalle
    {
        public string Talla { get; set; }
        public int Pares_Stock { get; set; }
    }

    public class Ent_Salida_Almacen
    {
        public int IdDespacho { get; set; }
        public string Desp_Nrodoc { get; set; }
        public string Desp_Descripcion { get; set; }
        public string Desp_Tipo_Descripcion { get; set; }
        public string Desp_Tipo { get; set; }
        public Decimal? TotalParesEnviado { get; set; }
        public string Estado { get; set; }
        public string Desp_FechaCre { get; set; }
        //campos adicionales de busqueda
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Tipo { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }

        public Ent_Edit_Salida_Almacen_Cabecera _Cabecera { get; set; }
        public List<Ent_Edit_Salida_Almacen_Detalle> _Detalle { get; set; }
    }
    public class Ent_Edit_Salida_Almacen_Cabecera
    {
        public string Desp_NroDoc { get; set; }
        public Decimal? Desp_id { get; set; }
        public string Estado { get; set; }
        public string Desp_FechaCre { get; set; }
        public string Desp_Tipo_Des { get; set; }
        public string Desp_Tipo { get; set; }
        public string Desp_Descripcion { get; set; }
        public string IdEstado { get; set; }
        public string Atendido { get; set; }
        //Cabecera
        public int NroPedidos { get; set; }
        public int NroEnviados { get; set; }
        public int NroPremio { get; set; }
        public int PremioEnviados { get; set; }
        public int CatalogEnviados { get; set; }
        public int CatalogPedidos { get; set; }
        public Decimal MontoTotal { get; set; }
        public bool chkAtender { get; set; }
        public bool chkEstSalida { get; set; }
        public string strDataDetalle { get; set; }
        public Decimal UsuarioCrea { get; set; }
    }

    public class Ent_Edit_Salida_Almacen_Detalle
    {
        //Detalle
        public string Desp_NroDoc { get; set; }
        public string Desp_Descripcion { get; set; }
        public string Estado { get; set; }
        public string Desp_FechaCre { get; set; }
        public string Asesor { get; set; }
        public string NombreLider { get; set; }
        public string Promotor { get; set; }
        public string Rotulo { get; set; }
        public string Rotulo_Courier { get; set; }
        public string Agencia { get; set; }
        public string Destino { get; set; }
        public string Pedido { get; set; }
        public Decimal? TotalPremio { get; set; }
        public Decimal? TotalPremioEnviado { get; set; }
        public Decimal? TotalCatalogo { get; set; }
        public Decimal? TotalCatalogEnviado { get; set; }
        public Decimal? TotalPares { get; set; }
        public Decimal? TotalParesEnviado { get; set; }
        public Decimal? Total_Cantidad { get; set; }
        public Decimal? Total_Cantidad_Envio { get; set; }
        public Decimal? TotalVenta { get; set; }
        public string CobroFlete { get; set; }
        public string Courier { get; set; }
        public string Observacion { get; set; }
        public string Detalle { get; set; }
        public string McaCourier { get; set; }
        public string McaFlete { get; set; }
        public Decimal? Enviado { get; set; }
        public Decimal? Desp_IdDetalle { get; set; }
        public Decimal? Desp_id { get; set; }
        public Decimal? TotalParesEnviadoEdit { get; set; }
        public Decimal? TotalCatalogEnviadoEdit { get; set; }
        public Decimal? TotalPremioEnviadoEdit { get; set; }
        public string IdEstado { get; set; }
        public string Atendido { get; set; }
        public Decimal? IdLider { get; set; }
        public string Lid_Prom { get; set; }
        public string Desp_Tipo_Des { get; set; }
        public string Desp_Tipo { get; set; }
        public string Delivery { get; set; }
        public string Dni_Promotor { get; set; }
    }

    public class Ent_Ventas_Semanales
    {
        public string AQ { get; set; }
        public int Anio { get; set; }
        public string Mes { get; set; }
        public string Dia { get; set; }
        public DateTime Fecha { get; set; }
        public Decimal Total { get; set; }
        //campos adicionales de busqueda
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class Ent_Ventas_Lider
    {
        public string Bas_Id { get; set; }
        public string Are_Id { get; set; }
        public string Asesor { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int IdMes { get; set; }
        public string Mes { get; set; }
        //Campos Adicionales
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string[] RorwsTh1 { get; set; }
        public string[] RorwsTh2 { get; set; }
        public string[] RorwsTh3 { get; set; }
        public object Data { get; set; }
        public List<Ent_Ventas_Lider_Col> _List_Ent_Ventas_Lider_Col { get; set; }
    }

    public class Ent_Ventas_Lider_Col
    {
        public string sName { get; set; }
        public string mData { get; set; }
        public string sClass { get; set; }

        public string cssColor { get; set; }
        public string fName{ get; set; }
    }

    public class Ent_Campaña_Fecha
    {
        public int Anio { get; set; }
        public int CamFec_Num { get; set; }
        public string CamFec_Ini { get; set; }
        public string CamFec_Fin { get; set; }
        public string CamFec_Nom { get; set; }
    }

    public class Ent_Consulta_Premios
    {
        public string Asesor { get; set; }
        public string Lider { get; set; }
        public string Promotor { get; set; }
        public string Documento { get; set; }
        public Decimal? Total { get; set; }
        public Decimal? Limite { get; set; }
        public Decimal? Saldo { get; set; }
        public string Descripcion { get; set; }
        public string Liqprem { get; set; }
        public string Liqpremiori { get; set; }
        public string Xentrega { get; set; }
        //Campos Agregados
        public DateTime FechaIni { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Valida { get; set; }
        public string Are_Id { get; set; }
        //
        public string Bas_Are_Id { get; set; }
        public string Bas_Id { get; set; }
        public string Nombres { get; set; }
        public string Bas_Aco_Id { get; set; }
        public string Bas_Usu_TipId { get; set; }
        public string Mes { get; set; }
        public int intMes { get; set; }
    }

    public class Ent_Comision_Lider
    {
        public string AreaId { get; set; }
        public string Asesor { get; set; }
        public string Lider { get; set; }
        public string LiderDni { get; set; }
        public Decimal? TotalPares { get; set; }
        public Decimal? TotalVenta { get; set; }
        public Decimal? PorcentajeComision { get; set; }
        public Decimal? Comision { get; set; }
        public Decimal? BonosNuevas { get; set; }
        public Decimal? SubTotalSinIGV { get; set; }
        public Decimal? CostoT { get; set; }
        public Decimal? Margen { get; set; }
        //Campos de busqueda
        public DateTime FechaIni { get; set; }
        public DateTime FechaFin { get; set; }
        public string Bas_Id { get; set; }
        public string Bas_Aco_Id { get; set; }
    }

    public class Ent_Ventas_Anual
    {
        public int IdAnio { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string MesNombre { get; set; }
        public Decimal Total { get; set; }
        public string Opcion { get; set; }
    }
    public class Ent_Ventas_Anual_Chart_Data
    {
        public string[] labels { get; set; }
        public string[] labelsTooltip { get; set; }
        public List<Ent_Ventas_Anual_Chart> datasets { get; set; }
    }
    public class Ent_Ventas_Anual_Chart
    {
        public string label { get; set; }
        public string[] backgroundColor { get; set; }
        public string borderWidth { get; set; }
        public decimal[] data { get; set; }
    }

    public class Ent_Ventas_Status
    {
        public int Asesor { get; set; }
        public int Lider { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActivacion { get; set; }
        public string Telefono { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
        public string NroDocumento { get; set; }
        public Decimal TotalVenta { get; set; }
        public int Anio { get; set; }
        public int MesNro { get; set; }
        //campos adicionales
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        //lista de Meses
        public int IdMes { get; set; }
        public string Mes { get; set; }
        //Armamos cabecera del vista
        public string[] RorwsTh1 { get; set; }
        public string[] RorwsTh2 { get; set; }
        public object Data { get; set; }
        public List<Ent_Ventas_Status_Col> _List_Ent_Ventas_Status_Col { get; set; }
        //
        public bool isOkUpdate { get; set; }
        public int Estado { get; set; }
    }
    public class Ent_Ventas_Status_Col
    {
        public string sName { get; set; }
        public string mData { get; set; }
        public string sClass { get; set; }

        public string cssColor { get; set; }
        public string fName { get; set; }
    }

    public class Ent_Ventas_PorZona
    {
        public string Asesor { get; set; }
        public string Directora { get; set; }
        public string Promotor { get; set; }
        public string DniPromotor { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
        public string Categoria { get; set; }
        public string SubCategoria { get; set; }
        public int? Pares { get; set; }
        public Decimal? Soles { get; set; }
        public Decimal? Costo { get; set; }
        public Decimal? Margen { get; set; }
        //Campos de busqueda
        public DateTime FechaIni { get; set; }
        public DateTime FechaFin { get; set; }
        public string Bas_Aco_Id { get; set; }
        public string Bas_Id { get; set; }
        public string CodDep { get; set; }
        public string CodPrv { get; set; }
        public string Linea { get; set; }
        public string CodCat { get; set; }
    }

    public class Ent_Ventas_Devolucion
    {
        public string Clientes { get; set; }
        public string DniRuc { get; set; }
        public int Salida { get; set; }
        public int Devolucion { get; set; }
        public decimal pventasneto { get; set; }
        public decimal pnotasneto { get; set; }
        //Campos de busqueda
        public string Bas_Id { get; set; }
        public DateTime FechaIni { get; set; }
        public DateTime FechaFin { get; set; }
        public string Bas_Aco_Id { get; set; }
    }

    public class Ent_Ventas_CategoriaMarca
    {        
        public DateTime FechaIni { get; set; }
        public DateTime FechaFin { get; set; }
        public string Bas_Id { get; set; }
        public string Bas_Aco_Id { get; set; }
        public string Mar_Id { get; set; }
        public List<Ent_Ventas_CategoriaMarca_List> _List_Principal { get; set; }
        public List<Ent_Ventas_CategoriaMarca_CharBar> _List_CharBar { get; set; }
        public List<Ent_Ventas_CategoriaMarca_Secundario> _List_Secundario { get; set; }
        public List<Ent_Ventas_CategoriaMarca_CharPie> _List_CharPie { get; set; }
        public List<Ent_Ventas_CategoriaMarca_Totales> _List_Total { get; set; }
    }

    public class Ent_Ventas_CategoriaMarca_List
    {
        public string Asesor { get; set; }
        public string NombreLider { get; set; }
        public string Promotora { get; set; }
        public string Categoria { get; set; }
        public string Marca { get; set; }
        public Decimal? Monto { get; set; }
        public Decimal? Cantidad { get; set; }
    }

    public class Ent_Ventas_CategoriaMarca_CharBar
    {
        public string NombreLider { get; set; }
        public Decimal Monto { get; set; }
        public Decimal cantidad { get; set; }
        public Decimal porc { get; set; }
        public Decimal Cantidadporc { get; set; }
    }
    public class Ent_Ventas_CategoriaMarca_Chart_Data
    {
        public string[] labels { get; set; }
        public string[] labelsTooltip { get; set; }
        public List<Ent_Ventas_CategoriaMarca_Chart> datasets { get; set; }
    }
    public class Ent_Ventas_CategoriaMarca_Chart
    {
        public string label { get; set; }
        public string[] backgroundColor { get; set; }
        public string borderWidth { get; set; }
        public decimal[] data { get; set; }
    }
    public class Ent_Ventas_CategoriaMarca_Secundario
    {
        public string NombreLider { get; set; }
        public string Categoria { get; set; }
        public Decimal? Monto { get; set; }
        public Decimal? Cantidad { get; set; }
        public Decimal? Prc { get; set; }
        public Decimal? CantidadPrc { get; set; }
    }

    public class Ent_Ventas_CategoriaMarca_CharPie
    {
        public string Categoria { get; set; }
        public string Marca { get; set; }
        public Decimal Monto { get; set; }
        public Decimal Cantidad { get; set; }
        public Decimal Cantidad2 { get; set; }
        public Decimal Prc { get; set; }
        public Decimal CantidadPrc { get; set; }
    }

    public class Ent_Ventas_CategoriaMarca_Totales
    {
        public Decimal TotalCantidad { get; set; }
        public Decimal TotalMonto { get; set; }
    }
}
