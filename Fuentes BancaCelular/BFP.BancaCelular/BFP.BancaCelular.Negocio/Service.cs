using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using BFP.BancaCelular.Negocio;
using BFP.BancaCelular.Entidad;
using System.IO;
using System.Configuration;
using System.Data;

using BFP.BancaCelular.Data;
using BFP.BancaCelular.Comun;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(Name = "ServiceBancaCelular", ConformsTo = WsiProfiles.BasicProfile1_1)]

public class Service : System.Web.Services.WebService
{
    public UsuarioCredencial usuariologin;
    string usuario = ConfigurationManager.AppSettings["UsuarioAdiministrador"].ToString();

    public Service()
    {

    }

    [WebMethod(Description = "Procesa las operaciones habilitadas en la Banca Celular", EnableSession = false)]
    [SoapDocumentMethod(Binding = "ServiceBancaCelular")]
    [SoapHeader("usuariologin")]
    public EResultadoMensajeMT OperacionBancaCelular(string strIdTransaccion, int intIdOperacion, string strNumeroTelefono,
                                                                               int intIdOperadora, string strIdTransaccionVerifica,
                                                                                                   string strParametrosOperacion)
    {
        EResultadoMensajeMT oResultadoMensajeMT = new EResultadoMensajeMT();
        ECatalogo<string, string> oTipoOperacion = null;
        try
        {

            //if (ConfigurationManager.AppSettings["DEMORECARGA"] != null) 
            //{
            //    if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["DEMORECARGA"].ToString()))
            //    {
            //        if (intIdOperacion == 7)
            //        {
            //            strNumeroTelefono = ConfigurationManager.AppSettings["DEMORECARGA"].ToString();
            //        }
            //    }
            //}
            
            if (ValidaUsuario())
            {
                oResultadoMensajeMT = NProcesaOperacion.ProcesaOperacion(strIdTransaccion, intIdOperacion, strNumeroTelefono, intIdOperadora, strIdTransaccionVerifica, strParametrosOperacion);
            }
            else
            {
                oTipoOperacion = new ECatalogo<string, string>();
                oTipoOperacion = CCUtil.ObtenerOperacion(intIdOperacion);

                oResultadoMensajeMT.CodRet = string.Empty;
                oResultadoMensajeMT.MensajeMT = CConstantes.MensajesError.ERROR_AUTENTIFICACION;
                oResultadoMensajeMT.Fecha = CCUtil.FormateaFecha(String.Format("{0:ddMMyyyy}", DateTime.Now));
                oResultadoMensajeMT.Hora = CCUtil.FormateaHora(String.Format("{0:HHmmss}", DateTime.Now));
                oResultadoMensajeMT.MensajeError = CConstantes.MensajesError.ERROR_AUTENTIFICACION;

                NLog.InsertarLogOperaciones(strIdTransaccion, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                                                string.Empty, CSerializacion.SerializarXML<EResultadoMensajeMT>(oResultadoMensajeMT),
                                                DateTime.Now, oResultadoMensajeMT.MensajeError, strIdTransaccionVerifica.Trim());

            }

            return oResultadoMensajeMT;
        }
        catch (Exception ex)
        {
            oTipoOperacion = new ECatalogo<string, string>();
            oTipoOperacion = CCUtil.ObtenerOperacion(intIdOperacion);

            EMensajeOperacion oMensajeOperacion = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.ERROR, string.Empty);
            oResultadoMensajeMT.CodRet = oMensajeOperacion.IdMensaje;
            oResultadoMensajeMT.MensajeMT = oMensajeOperacion.DescMensaje;
            oResultadoMensajeMT.Fecha = CCUtil.FormateaFecha(String.Format("{0:ddMMyyyy}", DateTime.Now));
            oResultadoMensajeMT.Hora = CCUtil.FormateaHora(String.Format("{0:HHmmss}", DateTime.Now));
            oResultadoMensajeMT.MensajeError = ex.Message;

            NLog.InsertarLogOperaciones(strIdTransaccion, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                string.Empty, CSerializacion.SerializarXML<EResultadoMensajeMT>(oResultadoMensajeMT),
                DateTime.Now, oResultadoMensajeMT.MensajeError, strIdTransaccionVerifica);


            //Inserta Error
            NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                    "Service.cs : OperacionBancaCelular", string.Empty, ex.Message,
                    DateTime.Now, CConstantes.CodigoAplicativo.ServicioWeb);

            return oResultadoMensajeMT;
        }
        finally
        {
            oResultadoMensajeMT = null;
            oTipoOperacion = null;
        }
    }

    /// <summary>
    /// Valida los credenciales del usuario
    /// </summary>
    /// <returns>Flag de validacion: True-Valido, False-Invalido</returns>
    /// 
    private bool ValidaUsuario()
    {
        bool autoriza = false;
        EncriptadorService oSeguridad = null;
        int codigoApp = CConstantes.CodigoAplicativo.ServicioWeb;
        ECatalogo<string, string> oCatalogoMensaje = null;

        try
        {           
            if (usuariologin != null)
            {
                oSeguridad = new EncriptadorService();
                string userNameYP = oSeguridad.desencripta(usuariologin.userName);
                string passwordYP = oSeguridad.desencripta(usuariologin.password);

                string userNameBFP = oSeguridad.desencripta(ConfigurationManager.AppSettings["userLogin"].ToString());
                string passwordBFP = oSeguridad.desencripta(ConfigurationManager.AppSettings["password"].ToString());

                if (userNameYP == userNameBFP && passwordYP == passwordBFP)
                    autoriza = true;
            }

            return autoriza;

        }
        catch (Exception ex)
        {            
            oCatalogoMensaje = new ECatalogo<string, string>();
            oCatalogoMensaje.Valor = string.Empty;
            oCatalogoMensaje.Nombre = CConstantes.MensajesError.ERROR_ACCESO_PROXY_ENCRIPTACION;

            NLog.Insertar(usuario, oCatalogoMensaje, CConstantes.EstadosLog.ERROR,
                "Service.cs : ValidaUsuario", CSerializacion.SerializarXML<UsuarioCredencial>(usuariologin),
                ex.Message, DateTime.Now, codigoApp);

            return false;
        }
        finally
        {
            oSeguridad = null;
            oCatalogoMensaje = null;
        }
    }
}

# region "SOAP Headers"

    public class UsuarioCredencial: SoapHeader
    {
        public string userName;
        public string password;
    }

# endregion

