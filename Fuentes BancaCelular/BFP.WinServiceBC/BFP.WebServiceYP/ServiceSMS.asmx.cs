using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;

namespace BFP.WebServiceYP
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]

    public class ServiceSMS : System.Web.Services.WebService
    {

        [WebMethod(Description = "Consume el servicio SMS de YP", EnableSession = false)]
        public string EnviaSMS(string serial, string pin, string mobile, string message, 
            string codigoCorto)
        {
            metroLine serviceSMS = new metroLine();
            string codigoRespuesta = string.Empty;
            codigoRespuesta = serviceSMS.sendShortMessage(serial, pin, mobile, message, codigoCorto);

            return codigoRespuesta;
        }
        

    }
}
