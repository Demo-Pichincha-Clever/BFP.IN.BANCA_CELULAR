using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Web.Services;

using BFP.AdminBancaCelular.Entidad;
using BFP.AdminBancaCelular.Negocio;
using BFP.AdminBancaCelular.Comun;


namespace BFP.AdminBancaCelular.Web
{
    public partial class MantenimientoOperaciones : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usuario"] != null && Session["nombre"] != null)
            {
                if (!Page.IsPostBack)
                {
                    CargarValidacionesJS();
                    CargarGrillaOperaciones();
                    CargarGrillaParametros();
                    EstadoEdicionTexto(false);
                    EstadoBotones(false);
                    EstadoEdicionParametro(false);
                }
            }
            else
            {
                Response.Redirect("Inicio.aspx");
            }
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Request.ServerVariables["http_user_agent"].IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) != -1)
                Page.ClientTarget = "uplevel";
        }

        #region "/** Accion Gridview **/"

        protected void chkOperId_CheckedChanged(object sender, EventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)(sender as Control).Parent.Parent;
            int index = gvRow.RowIndex;
            List<EParametrosOperacion> lstParametrosxOperacion;

            try
            {
                if (((CheckBox)gvOperaciones.Rows[index].FindControl("chkOperId")).Checked)
                {
                    for (int i = 0; i < gvOperaciones.Rows.Count; i++)
                    {
                        if (index != i)
                            ((CheckBox)gvOperaciones.Rows[i].FindControl("chkOperId")).Checked = false;
                    }

                    ViewState["Nuevo"] = null;
                    ViewState["Reedicion"] = null;
                    ViewState["Edicion"] = true;

                    EstadoEdicionParametro(true);
                    EstadoEdicionTexto(true);
                    EstadoBotones(true);
                    EstadoLimpiaParametro(false);
                    EstadoBotones(true);
                    
                    string idOperacion = ((Label)gvOperaciones.Rows[index].FindControl("lblIdOperacion")).Text;
                    EOperacion datoOperacion = NOperacion.LeerOperacionXMLxIdOperacion(idOperacion);
                    
                    txtIDOperacion.Text = Convert.ToString(datoOperacion.IdOperacion);
                    txtIDOperacion.Enabled = false;
                    txtNombreOperacion.Text = datoOperacion.DesOperacion;

                    string[] comandos = datoOperacion.Comandos.Split('-');

                    if (comandos.Length != 0)
                    {
                        if (comandos.Length == 1)
                        {
                            txtComando1.Text = comandos[0].ToString().Trim();
                            txtComando2.Text = string.Empty;
                            txtComando3.Text = string.Empty;
                        }
                        else if (comandos.Length == 2)
                        {
                            txtComando1.Text = comandos[0].ToString().Trim();
                            txtComando2.Text = comandos[1].ToString().Trim();
                            txtComando3.Text = string.Empty;
                        }
                        else if (comandos.Length == 3)
                        {
                            txtComando1.Text = comandos[0].ToString().Trim();
                            txtComando2.Text = comandos[1].ToString().Trim();
                            txtComando3.Text = comandos[2].ToString().Trim();
                        }
                    }
                    
                    txtProgramaAS.Text = datoOperacion.ProgramaAS400;
                    chkProcesoBatch.Checked = Convert.ToBoolean(datoOperacion.ProcesoBatch);
                    chkHabilitado.Checked = Convert.ToBoolean(datoOperacion.Habilitado);
                    chkOmision.Checked = Convert.ToBoolean(datoOperacion.OmisionParametro);
                    
                    lstParametrosxOperacion = NOperacion.LeerParametrosxOperacion(Convert.ToInt32(idOperacion));

                    for (int i = 0; i < gvParametros.Rows.Count; i++)
                    {
                        int idParametro = Convert.ToInt32(((Label)gvParametros.Rows[i].FindControl("lblIdParametro")).Text);

                        foreach (EParametrosOperacion item in lstParametrosxOperacion)
                        {
                            if (idParametro == item.IdParametro)
                            {
                                ((CheckBox)gvParametros.Rows[i].FindControl("chkParId")).Checked = true;
                            }
                        }
                    }
                }
                else
                {
                    EstadoBotones(false);
                    EstadoLimpia();
                    EstadoEdicionParametro(false);
                    EstadoEdicionTexto(false);
                }
            }
            catch (Exception)
            {
                lstParametrosxOperacion = null;
            }
            finally
            {
                lstParametrosxOperacion = null;
                udpDatosI.Update();
                udpDatosD.Update();
                udpParametros.Update();
                udpBotonera.Update();
            }
        }

        protected void gvOperaciones_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOperaciones.PageIndex = e.NewPageIndex;
            CargarGrillaOperaciones();

            if (ViewState["Edicion"] != null)
            {
                if (Convert.ToBoolean(ViewState["Edicion"]))
                {
                    EstadoLimpia();
                    EstadoEdicionOperacion(true);
                    EstadoEdicionParametro(false);
                    EstadoEdicionTexto(false);
                    EstadoBotones(false);
                }
            }

            udpOperaciones.Update();
            udpDatosI.Update();
            udpDatosD.Update();
            udpBotonera.Update();
            udpParametros.Update();
        }

        protected void gvParametros_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvParametros.PageIndex = e.NewPageIndex;
            CargarGrillaParametros();
            udpParametros.Update();
        }

        #endregion

        #region "/** Accion botones **/"

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            EOperacion oDatosOperacion = null;
            List<EParametrosOperacion> lstParametrosOperacion = null;
            string script = string.Empty;
            bool existeIDOperacion = false;
            StringBuilder strComandos = null;
            int posicionRegistro = 0;

            try
            {
                if (ViewState["Nuevo"] != null)
                {
                    if (Convert.ToBoolean(ViewState["Nuevo"]))
                    {
                        if (ViewState["Reedicion"] == null)
                        {
                            existeIDOperacion = NOperacion.ExisteIDOperacion(Convert.ToInt32(txtIDOperacion.Text.Trim()));
                        }
                    }
                }

                if (!existeIDOperacion)
                {
                    oDatosOperacion = new EOperacion();
                    oDatosOperacion.IdOperacion = Convert.ToInt32(txtIDOperacion.Text.Trim());
                    oDatosOperacion.DesOperacion = txtNombreOperacion.Text;

                    strComandos = new StringBuilder();

                    if (!String.IsNullOrEmpty(txtComando1.Text.Trim()))
                    {
                        strComandos = strComandos.Append(txtComando1.Text);
                    }
                    if (!String.IsNullOrEmpty(txtComando2.Text.Trim()))
                    {
                        if (strComandos.Length == 0)
                            strComandos = strComandos.Append(txtComando2.Text);
                        else
                            strComandos.Append(" - " + txtComando2.Text);
                    }
                    if (!String.IsNullOrEmpty(txtComando3.Text.Trim()))
                    {
                        if (strComandos.Length == 0)
                            strComandos = strComandos.Append(txtComando3.Text);
                        else
                            strComandos = strComandos.Append(" - " + txtComando3.Text);
                    }

                    oDatosOperacion.Comandos = strComandos.ToString();
                    oDatosOperacion.ProgramaAS400 = txtProgramaAS.Text;


                    if (ViewState["Nuevo"] != null)
                    {
                        if (Convert.ToBoolean(ViewState["Nuevo"]))
                        {
                            oDatosOperacion.FechaCreacion = CCUtil.FormateaFecha(String.Format("{0:ddMMyyyy}", DateTime.Now));
                        }


                    }
                    else if (ViewState["Edicion"] != null)
                    {
                        if (Convert.ToBoolean(ViewState["Edicion"]))
                        {
                            foreach (GridViewRow item in gvOperaciones.Rows)
                            {
                                if (((CheckBox)item.FindControl("chkOperId")).Checked == true)
                                {
                                    oDatosOperacion.FechaCreacion = ((Label)item.FindControl("lblFechaCreacion")).Text;
                                    posicionRegistro = item.RowIndex;
                                    break;
                                }
                            }
                        }
                    }

                    oDatosOperacion.OmisionParametro = chkOmision.Checked;
                    oDatosOperacion.ProcesoBatch = chkProcesoBatch.Checked;
                    oDatosOperacion.Habilitado = chkHabilitado.Checked;

                    bool existeComando = NOperacion.ExisteComandosOperacion(oDatosOperacion.IdOperacion, txtComando1.Text, txtComando2.Text, txtComando3.Text);

                    if (!existeComando)
                    {
                        lstParametrosOperacion = new List<EParametrosOperacion>();

                        foreach (GridViewRow item in gvParametros.Rows)
                        {
                            EParametrosOperacion oParametroOperacion = new EParametrosOperacion();
                            oParametroOperacion.IdOperacion = Convert.ToInt32(txtIDOperacion.Text.Trim());
                            oParametroOperacion.IdParametro = Convert.ToInt32(((Label)item.FindControl("lblIdParametro")).Text);
                            oParametroOperacion.DescripcionParametro = ((Label)item.FindControl("lblDescripcion")).Text;
                            oParametroOperacion.TipoDato = ((Label)item.FindControl("lblTipoDato")).Text;
                            oParametroOperacion.Longitud = ((Label)item.FindControl("lblLongitud")).Text;
                            oParametroOperacion.Decimal = ((Label)item.FindControl("lblDecimal")).Text;
                            oParametroOperacion.Selecciona = ((CheckBox)item.FindControl("chkParId")).Checked;

                            lstParametrosOperacion.Add(oParametroOperacion);
                        }

                        NOperacion.GeneraXML(oDatosOperacion.IdOperacion, oDatosOperacion, lstParametrosOperacion);

                        if (ViewState["Nuevo"] != null)
                        {
                            if (Convert.ToBoolean(ViewState["Nuevo"]))
                            {
                                ViewState["Reedicion"] = true;

                                script = @"<script type='text/javascript'>           
                              alert('Se ha registrado la operación con éxito'); 
                              </script>";
                            }
                        }
                        else if (ViewState["Edicion"] != null)
                        {
                            if (Convert.ToBoolean(ViewState["Edicion"]))
                            {
                                    script = @"<script type='text/javascript'>           
                              alert('Se ha modificado la operación con éxito'); 
                              </script>";
                            }
                        }
                    }
                    else
                    {
                        script = @"<script type='text/javascript'>           
                          alert('Alguno de los comandos ingresados ya se encuentra registrado en otra operación'); 
                          </script>";
                    }
                }
                else
                {
                    script = @"<script type='text/javascript'>           
                          alert('El código de la operación ya se encuentra registrado'); 
                          </script>";
                }

                ScriptManager.RegisterStartupScript(this, typeof(Page), "alerta", script, false);

            }
            catch (Exception)
            {
                throw new Exception("Error al registrar cambios");
            }
            finally
            {
                oDatosOperacion = null;
                lstParametrosOperacion = null;
                strComandos = null;

                CargarGrillaOperaciones();
                udpDatosD.Update();
                udpDatosI.Update();
                udpOperaciones.Update();
                udpParametros.Update();
                udpBotonera.Update();
                
                if (ViewState["Edicion"] != null)
                {
                    if (Convert.ToBoolean(ViewState["Edicion"]))
                    {
                        ((CheckBox)gvOperaciones.Rows[posicionRegistro].FindControl("chkOperId")).Checked = true;
                    }
                }
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            CargarGrillaOperaciones();
            CargarGrillaParametros();

            EstadoLimpia();
            EstadoEdicionOperacion(true);
            EstadoEdicionTexto(false);
            EstadoEdicionParametro(false);
            EstadoBotones(false);

            udpDatosI.Update();
            udpDatosD.Update();
            udpOperaciones.Update();
            udpParametros.Update();
            udpBotonera.Update();
        }

        protected void imgNuevo_Click(object sender, ImageClickEventArgs e)
        {
            ViewState["Nuevo"] = true;
            ViewState["Edicion"] = null;
            ViewState["Reedicion"] = null;

            EstadoLimpia();
            EstadoEdicionTexto(true);
            EstadoBotones(true);
            EstadoEdicionParametro(true);

            txtIDOperacion.Enabled = false;
            int codigOperacion = NOperacion.ObtenerSiguienteOperacion();
            txtIDOperacion.Text = codigOperacion.ToString();

            udpDatosI.Update();
            udpDatosD.Update();
            udpBotonera.Update();
            udpParametros.Update();
            udpOperaciones.Update();
        }

        protected void imgEliminar_Click(object sender, ImageClickEventArgs e)
        {
            string script = string.Empty;

            for (int i = 0; i < gvOperaciones.Rows.Count; i++)
            {
                if (((CheckBox)gvOperaciones.Rows[i].FindControl("chkOperId")).Checked == true)
                {
                    int operacionID = Convert.ToInt32(((Label)gvOperaciones.Rows[i].FindControl("lblIdOperacion")).Text);

                    if (!chkHabilitado.Checked)
                    {
                        NOperacion.GeneraXML(operacionID, null, null);

                        CargarGrillaOperaciones();
                        CargarGrillaParametros();

                        EstadoLimpia();
                        EstadoEdicionOperacion(true);
                        EstadoEdicionParametro(false);
                        EstadoEdicionTexto(false);
                        EstadoBotones(false);

                        udpBotonera.Update();
                        udpDatosD.Update();
                        udpDatosI.Update();
                        udpOperaciones.Update();
                        udpParametros.Update();

                        script = @"<script type='text/javascript'>           
                                        alert('Operacion eliminada satisfactoriamente'); 
                                    </script>";
                    }
                    else
                    {
                        script = @"<script type='text/javascript'>           
                                  alert('No se puede eliminar la operación'); 
                                    </script>";
                    }

                    break;
                }
                else
                {
                    script = @"<script type='text/javascript'>           
                          alert('Debe seleccionar una operación'); 
                          </script>";
                }
            }
                    
            ScriptManager.RegisterStartupScript(this, typeof(Page), "alerta", script, false);
        }

        #endregion

        #region "/** Codigo Personalizado **/"

        private void CargarGrillaParametros()
        {
            List<EParametrosOperacion> lstParametros = new List<EParametrosOperacion>();
            try
            {
                //Lee el archivo XML
                lstParametros = NOperacion.LeerParametros();

                if (lstParametros != null)
                {
                    if (lstParametros.Count > 0)
                    {
                        gvParametros.Visible = true;
                        gvParametros.DataSource = lstParametros;
                        gvParametros.DataBind();
                    }
                    else
                    {
                        gvParametros.Visible = false;
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                lstParametros = null;
            }
        }

        private void CargarGrillaOperaciones()
        {
            List<EOperacion> lstOperaciones = new List<EOperacion>();

            try
            {
                //Lee el archivo XML
                lstOperaciones = NOperacion.LeerOperacionXML();

                if (lstOperaciones != null)
                {
                    if (lstOperaciones.Count > 0)
                    {
                        lblDescOperacion.Text = "Operaciones registradas para la Banca Celular";
                        gvOperaciones.Visible = true;
                        gvOperaciones.DataSource = lstOperaciones;
                        gvOperaciones.DataBind();
                    }
                    else
                    {
                        lblDescOperacion.Text = "No existen operaciones registradas";
                        gvOperaciones.Visible = false;
                    }
                }
            }
            catch (Exception)
            {
                lstOperaciones = null;
            }
            finally
            {
                lstOperaciones = null;
            }

        }

        private void EstadoEdicionTexto(bool estado)
        {
            txtIDOperacion.Enabled = estado;
            txtNombreOperacion.Enabled = estado;
            txtComando1.Enabled = estado;
            txtComando2.Enabled = estado;
            txtComando3.Enabled = estado;
            txtProgramaAS.Enabled = estado;
            chkOmision.Enabled = estado;
            chkHabilitado.Enabled = estado;
        }

        private void EstadoEdicionOperacion(bool estado)
        {
            CheckBox chkOperID;

            foreach (GridViewRow item in gvOperaciones.Rows)
            {
                chkOperID = (CheckBox)item.FindControl("chkOperId");
                chkOperID.Enabled = estado;
            }
        }

        private void EstadoEdicionParametro(bool estado)
        {
            CheckBox chkParID;
            foreach (GridViewRow item in gvParametros.Rows)
            {
                chkParID = (CheckBox)item.FindControl("chkParId");
                chkParID.Enabled = estado;
            }
        }

        private void EstadoLimpiaParametro(bool estado)
        {
            CheckBox chkParID;
            foreach (GridViewRow item in gvParametros.Rows)
            {
                chkParID = (CheckBox)item.FindControl("chkParId");
                chkParID.Checked = estado;
            }
        }

        private void EstadoLimpia()
        {
            txtIDOperacion.Text = string.Empty;
            txtNombreOperacion.Text = string.Empty;
            txtComando1.Text = string.Empty;
            txtComando2.Text = string.Empty;
            txtComando3.Text = string.Empty;
            txtProgramaAS.Text = string.Empty;
            //chkProcesoBatch.Checked = false;
            chkHabilitado.Checked = false;
            chkOmision.Checked = false;

            CheckBox chkOperID;
            foreach (GridViewRow item in gvOperaciones.Rows)
            {
                chkOperID = (CheckBox)item.FindControl("chkOperId");
                chkOperID.Checked = false;
            }

            CheckBox chkParID;
            foreach (GridViewRow item in gvParametros.Rows)
            {
                chkParID = (CheckBox)item.FindControl("chkParId");
                chkParID.Checked = false;
            }
        }

        private void EstadoBotones(bool estado)
        {
            btnGuardar.Enabled = estado;
            btnCancelar.Enabled = estado;
        }

        protected void CargarValidacionesJS()
        {
            CCUtil.ConfigurarUpperCase(this);
            this.txtNombreOperacion.Attributes.Add("onkeypress", "AceptaSoloLetras()");
            this.txtComando1.Attributes.Add("onkeypress", "AceptaSoloLetras()");
            this.txtComando2.Attributes.Add("onkeypress", "AceptaSoloLetras()");
            this.txtComando3.Attributes.Add("onkeypress", "AceptaSoloLetras()");
            this.txtIDOperacion.Attributes.Add("onkeypress", "AceptaSoloNroEntero()");
            this.txtProgramaAS.Attributes.Add("onkeypress", "AceptaSoloNroEntero()");
        }

        #endregion

    }   



}
