using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CapaPresentacion.AspNetForms
{
    public partial class reportArticlesReturned : System.Web.UI.Page
    {
        ReportDocument rd = null;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                string strReportName = System.Web.HttpContext.Current.Session["ReportName"].ToString();
                var rptSource = System.Web.HttpContext.Current.Session["rptSource"];
           

                rd = new ReportDocument();

                string strRptPath = Server.MapPath("~/") + "RptsCrystal//" + strReportName;

                rd.Load(strRptPath);

                // Setting report data source
                if (rptSource != null && rptSource.GetType().ToString() != "System.String")
                    rd.SetDataSource(rptSource);

           
                var download = System.Web.HttpContext.Current.Session["rptDownload"];

                if ((bool)download)
                {
                    DownloadPDF();
                }
                else
                {
                    crv.ReportSource = rd;
                    crv.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                    crv.HasToggleGroupTreeButton = false;
                    crv.Zoom(93);
                }

            }
            catch (Exception exc)
            {

            }
        }
        private void DownloadPDF()
        {
            try
            {
                ExportFormatType formatType = ExportFormatType.NoFormat;
                formatType = ExportFormatType.PortableDocFormat;
                rd.ExportToHttpResponse(formatType, Response, true, "Nota_Credito");
                //Response.End();
            }
            catch (Exception exc)
            {
            }
        }
        protected void Page_Unload(object sender, EventArgs e)
        {
            /// Reporte generalizado
            if ((rd != null) && rd.IsLoaded)
            {
                rd.Close();
                rd.Dispose();
            }
        }
    }
}