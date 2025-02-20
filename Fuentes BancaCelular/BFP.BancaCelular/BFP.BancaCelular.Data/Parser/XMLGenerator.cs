using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Configuration;


namespace BFP.BancaCelular.Data
{
    public class XmlGenerator
    {
        private DataTable _dtMensajes;
        private DataTable _dtCampos;
        XmlDataDocument xmlDom = new XmlDataDocument();

        public XmlGenerator()
        {
        }

        public XmlDataDocument ObtenerXml(string strNombreTransaccion, string strNombreMensaje)
        {
            XmlNode xmlNodoCabecera;
            XmlNode xmlNodoDetalle;

            xmlNodoCabecera = xmlDom.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            xmlDom.AppendChild(xmlNodoCabecera);

            xmlNodoDetalle = xmlDom.CreateElement("", "BFP" + strNombreTransaccion, "");
            xmlDom.AppendChild(xmlNodoDetalle);

            _dtMensajes = ObtenerMensajesPorNombre(strNombreMensaje, strNombreTransaccion);

            foreach (DataRow drMensaje in _dtMensajes.Rows)
            {
                _dtCampos = ObtenerCampos((int)drMensaje["intMensajeID"]);

                XmlNode xmlNodoMensaje;
                xmlNodoMensaje = xmlDom.CreateElement("", drMensaje["strNombre"].ToString(), "");

                xmlNodoDetalle.AppendChild(xmlNodoMensaje);

                foreach (DataRow drCampo in _dtCampos.Rows)
                {
                    if (string.Compare((string)drCampo["strNombrePadre"], (string)drCampo["strNombre"]) == 0)
                    {
                        XmlElement xmlNodoPadre;
                        xmlNodoPadre = xmlDom.CreateElement("", drCampo["strNombre"].ToString(), "");

                        xmlNodoMensaje.AppendChild(xmlNodoPadre);
                        AgregarNodo(ref xmlNodoPadre, _dtCampos);
                    }

                }
            }

            xmlDom.Save(string.Format("{0}BFP{1}{2}.xml", System.Configuration.ConfigurationManager.AppSettings["XMLPrograma"], strNombreTransaccion, strNombreMensaje));

            return xmlDom;
        }


        void AgregarNodo(ref XmlElement xmlNode, DataTable dtCampos)
        {
            foreach (DataRow drCampo in dtCampos.Rows)
            {
                if (string.Compare((string)drCampo["strNombrePadre"], xmlNode.Name) == 0 && string.Compare((string)drCampo["strNombrePadre"], (string)drCampo["strNombre"]) != 0)
                {
                    XmlElement xmlNodoHijo;

                    xmlNodoHijo = xmlDom.CreateElement("", drCampo["strNombre"].ToString(), "");
                    if ((bool)drCampo["blnEsHoja"])
                    {
                        xmlNodoHijo.SetAttribute("Type", drCampo["strTipo"].ToString());
                        xmlNodoHijo.SetAttribute("Length", drCampo["intLongitud"].ToString());
                        xmlNodoHijo.SetAttribute("Default", drCampo["strDefaultValue"].ToString());
                        xmlNodoHijo.SetAttribute("Decimal", drCampo["intDecimales"].ToString());
                    }
                    else
                    {
                        xmlNodoHijo.SetAttribute("Length", drCampo["intLongitud"].ToString());
                        xmlNodoHijo.SetAttribute("Size", drCampo["intTamano"].ToString());
                    }

                    xmlNode.AppendChild(xmlNodoHijo);
                    AgregarNodo(ref xmlNodoHijo, dtCampos);
                }
            }
        }

        /// <summary>
        /// Obtiene el nombre del mensaje de cada transaccion
        /// </summary>
        /// <param name="strNombreMensaje"></param>
        /// <param name="strNombreTransaccion"></param>
        /// <returns></returns>
        public static DataTable ObtenerMensajesPorNombre(string strNombreMensaje, string strNombreTransaccion)
        {

            ADParser objParser = new ADParser();
            DataTable table = new DataTable();
            table.Load(objParser.ObtenerMensajePorNombre(strNombreMensaje, strNombreTransaccion));
            return table;
        }

        /// <summary>
        /// Obtiene los campos permitidos del mensaje
        /// </summary>
        /// <param name="intMensajeID"></param>
        /// <returns></returns>
        public static DataTable ObtenerCampos(int intMensajeID)
        {
            ADParser objParser = new ADParser();
            DataTable dtCampos = new DataTable();
            using (IDataReader reader = objParser.ObtenerCamposPorMensaje(intMensajeID))
            {
                dtCampos.Load(reader);
            }

            return dtCampos;
        }
    }
}
