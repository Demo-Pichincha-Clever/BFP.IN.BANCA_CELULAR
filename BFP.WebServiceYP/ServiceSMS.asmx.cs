using System;
using BFP.WebServiceYP.SIXP2;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Configuration;
using System.Web.Services;

namespace BFP.WebServiceYP
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]


    public class ServiceSMS : System.Web.Services.WebService
    {
        private Service objSixCommunication = null;
        private System.Data.DataSet _Cabecera;
        public string _strTrama = string.Empty;

        private string EjecutarTransaccion(string strNombreTransaccion, short shrLongitud, string strTrama, out string _strError)
        {
            string str3;
            Service service = new Service
            {
                Url = ConfigurationManager.AppSettings.Get("URLWS")
            };
            this.objSixCommunication = service;
            string message = string.Empty;
            string str2 = string.Empty;
            try
            {
                str2 = this.objSixCommunication.SendMessage(strNombreTransaccion, shrLongitud, strTrama);
                _strError = str2.Substring(0x11, 4);
                if (_strError.Trim().Length < 4)
                {
                    _strError = str2.Substring(0x1a, 4);
                }
                str3 = str2;
            }
            catch (Exception exception)
            {
                _strError = "6666";
                message = exception.Message;
                str3 = str2;
            }
            finally
            {
                this.objSixCommunication = null;
                str2 = null;
            }
            return str3;
        }

        [WebMethod(Description = "Consume el servicio SMS de YP", EnableSession = false)]
        public string EnviaSMS(string serial, string pin, string mobile, string message,
            string codigoCorto)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            ServicioYP.metroLineSoap ObjetoServicio = new ServicioYP.metroLineSoapClient();

            string codigoRespuesta = string.Empty;
            codigoRespuesta = ObjetoServicio.sendShortMessage(serial, pin, mobile, message, codigoCorto);

            return codigoRespuesta;
        }

        [WebMethod(Description = "servicio SMS de YP", EnableSession = false)]
        public string EnviaSMS_Hist(string serial, string pin, string mobile, string message, string codigoCorto, string strMensajeTrama)
        {
            metroLine line = new metroLine();
            string str2 = string.Empty;
            string str3 = string.Empty;
            str2 = this.EjecutarTransaccion(this.NombreTransaccion().ToString(), this.LongitudCabecera(), strMensajeTrama.ToString(), out str3);
            return line.sendShortMessage(serial, pin, mobile, message, codigoCorto);
        }

        public short LongitudCabecera()
        {
            return 0x1da;
        }

        public string NombreMensajeOut()
        {
            return "OutDat";
        }

        public string NombreTransaccion()
        {
            return "7079";
        }

        private System.Data.DataSet Cabecera
        {
            get
            {
                return this._Cabecera;
            }
            set
            {
                this._Cabecera = value;
            }
        }

    }
}
