using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Configuration;
using System.IO;

namespace BFP.WinService.Data
{
    public class DataLoader
    {
        int _intPosicion = 0;

        public DataSet ObtenerData(string strTrama, string strNombreMensaje, string strNombreTransaccion, int intPosicionInicial)
        {
            _intPosicion = intPosicionInicial;
            DataSet dsResult = new DataSet(strNombreTransaccion);
            XmlDataDocument xmlDom = LeerXML(strNombreTransaccion, strNombreMensaje);
            XmlNode xmlNode = xmlDom.GetElementsByTagName("OData")[0];
            GenerateDatatable(ref dsResult, xmlNode, "OData");
            FillDataTable(ref dsResult, xmlNode, "OData", strTrama);

            return dsResult;
        }

        private XmlDataDocument LeerXML(string strNombreTransaccion, string strNombreMensaje)
        {
            XmlDataDocument document = new XmlDataDocument();
            if (File.Exists(ConfigurationManager.AppSettings.Get("XMLPATH") + "BFP" + strNombreTransaccion + strNombreMensaje + ".xml"))
            {
                document.Load(ConfigurationManager.AppSettings.Get("XMLPATH") + "BFP" + strNombreTransaccion + strNombreMensaje + ".xml");
                return document;
            }

            XmlGenerator generator = new XmlGenerator();
            return generator.ObtenerXml(strNombreTransaccion, strNombreMensaje);
        }

        void GenerateDatatable(ref DataSet dsPadre, XmlNode xmlNode, string strName)
        {
            DataTable dtOut = new DataTable(strName);
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if (node.HasChildNodes)
                {
                    DataColumn dtCol = new DataColumn(node.Name + "ID");
                    dtOut.Columns.Add(dtCol);
                    GenerateDatatable(ref dsPadre, node, node.Name);
                }
                else
                {
                    DataColumn dtCol = new DataColumn(node.Name, System.Type.GetType(node.Attributes["Type"].Value.Trim()));
                    dtOut.Columns.Add(dtCol);
                }
            }

            dsPadre.Tables.Add(dtOut);
        }

        void FillDataTable(ref DataSet dsPadre, XmlNode xmlNode, string strName, string strTrama)
        {
            DataTable dtOut = dsPadre.Tables[strName];
            DataRow drData = dtOut.NewRow();

            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if (node.HasChildNodes)
                {
                    drData[node.Name + "ID"] = "__ID";
                    for (int i = 0; i < int.Parse(node.Attributes["Size"].Value); i++)
                    {
                        if (strTrama.Substring(_intPosicion, int.Parse(node.Attributes["Length"].Value)).Trim().Length == 0)
                        {
                            int intNewPos = int.Parse(node.Attributes["Size"].Value) - i;
                            int intLongitud = int.Parse(node.Attributes["Length"].Value);
                            _intPosicion = _intPosicion + intNewPos * intLongitud;
                            break;
                        }

                        FillDataTable(ref dsPadre, node, node.Name, strTrama);
                    }
                }
                else
                {
                    switch (dtOut.Columns[node.Name].DataType.Name)
                    {
                        case "Double":
                            string strEntero = strTrama.Substring(_intPosicion, int.Parse(node.Attributes["Length"].Value) - int.Parse(node.Attributes["Decimal"].Value));
                            string strDecimal = strTrama.Substring(_intPosicion + strEntero.Length, int.Parse(node.Attributes["Decimal"].Value));

                            if (strEntero.Trim().Length == 0)
                            {
                                strEntero = "0";
                            }

                            if (strDecimal.Trim().StartsWith("-"))
                            {
                                strEntero = "-" + strEntero;
                                strDecimal = strDecimal.Substring(1);
                                strDecimal = strDecimal.Trim().PadLeft(int.Parse(node.Attributes["Decimal"].Value), '0');

                                if (strDecimal.Trim().Length == 0)
                                {
                                    strDecimal = "0";
                                    strDecimal = strDecimal.Trim().PadLeft(int.Parse(node.Attributes["Decimal"].Value), '0');
                                }

                            }
                            else
                            {
                                strDecimal = strDecimal.Trim().PadLeft(int.Parse(node.Attributes["Decimal"].Value), '0');
                            }


                            if (strEntero.Trim().Length == 0 && strDecimal.Trim().Length == 0)
                            {
                                drData[node.Name] = "0.0";
                            }
                            else
                            {
                                drData[node.Name] = strEntero.Trim() + "." + strDecimal.Trim();
                            }

                            break;
                        case "DateTime":
                            if (int.Parse(node.Attributes["Length"].Value) == 8)
                            {
                                string strDia = strTrama.Substring(_intPosicion, 2);
                                string strMes = strTrama.Substring(_intPosicion + 2, 2);
                                string strAnio = strTrama.Substring(_intPosicion + 4, 4);

                                drData[node.Name] = DateTime.Parse(strDia + "/" + strMes + "/" + strAnio);
                            }
                            else
                            {
                                throw new Exception("La fecha tiene un tamaño inválido");
                            }

                            break;

                        default:
                            if (strTrama.Length >= int.Parse(node.Attributes["Length"].Value))
                            {
                                drData[node.Name] = strTrama.Substring(_intPosicion, int.Parse(node.Attributes["Length"].Value));
                            }
                            else
                            {
                                drData[node.Name] = strTrama;
                            }
                            break;
                    }

                    _intPosicion = _intPosicion + int.Parse(node.Attributes["Length"].Value);
                }
            }

            dtOut.Rows.Add(drData);
        }

    }
}
