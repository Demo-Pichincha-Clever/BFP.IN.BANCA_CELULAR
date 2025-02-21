using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;

using BFP.AdminBancaCelular.Entidad;
using BFP.AdminBancaCelular.Negocio;

namespace BFP.AdminBancaCelular.Web
{
    public partial class AdminBancaCelular : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Context.Request.Browser.Adapters.Clear();

            if (!Page.IsPostBack)
            {
                List<EParametrosOperacion> lstParametros = new List<EParametrosOperacion>();
                string usuario = Request.ServerVariables["AUTH_USER"].ToString().ToUpper();

                usuario = usuario.Replace("FINANCIERO\\", "");

                EItemsParametros oItemsParametros = NOperacion.ValidarAcceso(usuario);

                if (oItemsParametros != null)
                {
                    Session["usuario"] = oItemsParametros.ValItemTabla;
                    Session["nombre"] = oItemsParametros.DescLargaItemTabla;
                    Label lblUsuario = (Label)Page.Master.FindControl("lblUsuario");
                    lblUsuario.Text = "Usuario: " + oItemsParametros.DescLargaItemTabla;
                }
                else
                {
                    Session["usuario"] = "";
                    Session["nombre"] = "";
                    Response.Redirect("~/SinAcceso.aspx", true);
                }
            }
        }

        protected void ScriptManager1_AsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e)
        {
            if (e.Exception.Data["ExtraInfo"] != null)
            {
                ScriptManager1.AsyncPostBackErrorMessage =
                    e.Exception.Message +
                    e.Exception.Data["ExtraInfo"].ToString();
            }
            else
            {
                ScriptManager1.AsyncPostBackErrorMessage =
                    "Error no especificado.";
            }

        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Request.ServerVariables["http_user_agent"].IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) != -1)
                Page.ClientTarget = "uplevel";
        }

    }
}
