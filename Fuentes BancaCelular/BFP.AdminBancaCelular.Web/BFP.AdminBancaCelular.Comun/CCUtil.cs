using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web;
using System.IO;


using BFP.AdminBancaCelular.Entidad;

namespace BFP.AdminBancaCelular.Comun
{
    public class CCUtil
    {
        /// <summary>
        /// Formatea la fecha dia/mes/año
        /// </summary>
        /// <param name="strFecha"></param>
        /// <returns></returns>
        public static string FormateaFecha(string strFecha)
        {
            string datofecha = strFecha.PadLeft(8, '0').ToString();
            string caracterSeparador = "/";
            string dia = datofecha.Substring(0, 2).ToString();
            string mes = datofecha.Substring(2, 2).ToString();
            string anio = datofecha.Substring(4, 4).ToString();

            string fechaParser = dia + caracterSeparador + mes + caracterSeparador + anio;

            return fechaParser;
        }

        public static void ConfigurarUpperCase(Control objContenedor)
        {
            // para cada control contenido en la colección
            foreach (Control obj in objContenedor.Controls)
            {
                if (obj is System.Web.UI.Control)
                {
                    // llamada recursiva para sub-hijos
                    if (((System.Web.UI.Control)obj).HasControls())
                        ConfigurarUpperCase(obj);
                    if (obj is TextBox) ((TextBox)obj).Attributes.Add("onblur", "this.value = this.value.toUpperCase();");
                    if (obj is TextBox) ((TextBox)obj).Attributes.Add("oncontextmenu", "return false;");
                }
            }
        }

        public static void GenerarLogError(string strSalida, string strUbicacion)
        {
            try
            {
                string strRuta = string.Concat(System.Configuration.ConfigurationManager.AppSettings.Get("LOG"));
                string strArchivo = string.Concat("BANCACELULAR", DateTime.Now.Year.ToString().PadLeft(4, '0'), DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Day.ToString().PadLeft(2, '0'), ".txt");
                string strRutaArchivo = string.Concat(strRuta, strArchivo);

                if (System.IO.File.Exists(strRutaArchivo))
                {
                    using (System.IO.StreamWriter objStreamWriter = new System.IO.StreamWriter(strRutaArchivo, true, System.Text.Encoding.Unicode))
                    {
                        objStreamWriter.WriteLine(strUbicacion + ": " + strSalida);
                    }
                }
                else
                {
                    using (System.IO.FileStream fs = new System.IO.FileStream(string.Format("{0}\\{1}",
                                                                strRuta,
                                                                strArchivo),
                                                                System.IO.FileMode.Create))
                    {
                        //Archivo creado ;
                    }
                    using (System.IO.StreamWriter objStreamWriter = new System.IO.StreamWriter(strRutaArchivo, true, System.Text.Encoding.Unicode))
                    {
                        objStreamWriter.WriteLine(strUbicacion + ": " + strSalida);
                    }

                }
                strRutaArchivo = string.Empty;

            }
            catch (Exception ex)
            {

            }
        }
    }
}
