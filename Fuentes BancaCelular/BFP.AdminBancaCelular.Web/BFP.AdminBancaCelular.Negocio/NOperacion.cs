using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Xml;
using System.IO;

using BFP.AdminBancaCelular.Entidad;
using BFP.AdminBancaCelular.Data;
using BFP.AdminBancaCelular.Comun;


namespace BFP.AdminBancaCelular.Negocio
{
    public static class NOperacion
    {
        /// <summary>
        /// Lee las operaciones del archivo XML
        /// </summary>
        /// <returns>Objeto lista de tipo EOperacion</returns>
        /// 
        public static List<EOperacion> LeerOperacionXML()
        {
            string fileXML = ConfigurationManager.AppSettings["PathXML"] + ConfigurationManager.AppSettings["ArchivoXML"];
            DataSet ds_TablaOperacion = new DataSet();
            List<EOperacion> lstOperacion = new List<EOperacion>();

            try
            {
                if (File.Exists(fileXML))
                {
                    ds_TablaOperacion.ReadXml(fileXML);

                    if (ds_TablaOperacion != null)
                    {
                        if (ds_TablaOperacion.Tables[0].Rows.Count > 0)
                        {
                            lstOperacion = (from p in ds_TablaOperacion.Tables[0].AsEnumerable()
                                            orderby Convert.ToInt32(p.Field<string>("IdOperacion").Trim())
                                            select new EOperacion
                                            {
                                                IdOperacion = Convert.ToInt32(p.Field<string>("IdOperacion")),
                                                DesOperacion = p.Field<string>("DesOperacion"),
                                                Comandos = p.Field<string>("Comandos"),
                                                OmisionParametro = Convert.ToBoolean(Convert.ToInt32(p.Field<string>("OmiteParametros"))),
                                                FechaCreacion = p.Field<string>("FechaCreacion"),
                                                ProgramaAS400 = p.Field<string>("ProgramaAS400"),
                                                ProcesoBatch = Convert.ToBoolean(Convert.ToInt32(p.Field<string>("ProcesoBatch"))),
                                                Habilitado = Convert.ToBoolean(Convert.ToInt32(p.Field<string>("Habilitado")))
                                            }).ToList();
                        }
                    }
                }

                return lstOperacion;

            }
            catch (Exception)
            {
                return lstOperacion;
            }
            finally
            {
                lstOperacion = null;
                ds_TablaOperacion = null;
            }
        }

        /// <summary>
        /// Obtiene los parametros de la tabla maestra
        /// </summary>
        /// <returns>Objeto lista de tipo EParametrosOperacion</returns>
        /// 
        public static List<EParametrosOperacion> LeerParametros()
        {
            EItemsParametros[] arrayItemsParametros;
            List<EParametrosOperacion> lstItemsParametros = new List<EParametrosOperacion>();

            try
            {
                arrayItemsParametros = ADTablaMaestra.ObtenerItemsParametrosOperacion(CConstantes.TablaMaestras.PARAMETROS_OPERACION);

                lstItemsParametros = new List<EParametrosOperacion>();

                foreach (var item in arrayItemsParametros)
                {
                    EParametrosOperacion oParametro = new EParametrosOperacion();
                    oParametro.IdParametro = item.IdItemTabla;
                    oParametro.DescripcionParametro = item.DescLargaItemTabla;

                    string[] tipodatoFlotante = item.LongitudParametro.Split(',');
                    if (tipodatoFlotante.Count() > 1)
                    {
                        oParametro.Longitud = tipodatoFlotante[0].Trim();
                        oParametro.Decimal = tipodatoFlotante[1].Trim();
                    }
                    else
                    {
                        oParametro.Longitud = item.LongitudParametro;
                        oParametro.Decimal = "0";
                    }

                    oParametro.TipoDato = item.TipoDatoParametro;

                    lstItemsParametros.Add(oParametro);
                }

                return lstItemsParametros;

            }
            catch (Exception)
            {
                return lstItemsParametros;
            }
            finally
            {
                arrayItemsParametros = null;
                lstItemsParametros = null;
            }
        }

        /// <summary>
        /// Obtiene los parametros seleccionados para la operacion segun id de la operación
        /// </summary>
        /// <param name="idOperacion"></param>
        /// <returns></returns>
        public static List<EParametrosOperacion> LeerParametrosxOperacion(int idOperacion)
        {
            string fileXML = ConfigurationManager.AppSettings["PathXML"] + ConfigurationManager.AppSettings["ArchivoXML"];
            DataSet ds_TablaParametros = new DataSet();
            List<EParametrosOperacion> lstParametrosOperacion = new List<EParametrosOperacion>();

            try
            {
                if (File.Exists(fileXML))
                {
                    ds_TablaParametros.ReadXml(fileXML);

                    DataSet ds_ParametrosxOperacion = GenerateDataSet(ds_TablaParametros);

                    if (ds_ParametrosxOperacion != null)
                    {
                        if (ds_ParametrosxOperacion.Tables[0].Rows.Count > 0)
                        {
                            lstParametrosOperacion = (from p in ds_ParametrosxOperacion.Tables[0].AsEnumerable()
                                                      where (p.Field<bool>("Selecciona") == true && p.Field<int>("IdOperacion") == idOperacion)
                                                      orderby p.Field<int>("IdParametro")
                                                      select new EParametrosOperacion
                                                      {
                                                          IdOperacion = p.Field<int>("IdOperacion"),
                                                          IdParametro = p.Field<int>("IdParametro"),
                                                          Selecciona = p.Field<bool>("Selecciona")
                                                      }).ToList();
                        }
                    }
                }

                return lstParametrosOperacion;
            }
            catch (Exception)
            {
                return lstParametrosOperacion;
            }
            finally
            {
                lstParametrosOperacion = null;
                ds_TablaParametros = null;
            }
        }

        /// <summary>
        /// Genera un dataset unico a partir de todos los dataset de los parametros
        /// </summary>
        /// <param name="dsParametros">DataSet de parametros</param>
        /// <returns>Retorna dataset unico de parametros</returns>
        /// 
        private static DataSet GenerateDataSet(DataSet dsParametros)
        {
            DataSet dsParametrosOperacion = new DataSet();
            DataTable objDataTable = new DataTable();

            DataColumn cIdOperacion;
            DataColumn cIdParametro;
            DataColumn cDescripcionParametro;
            DataColumn cTipoDato;
            DataColumn cLongitud;
            DataColumn cDecimal;
            DataColumn cSelecciona;

            cIdOperacion = new DataColumn("IdOperacion", Type.GetType("System.Int32"));
            cIdParametro = new DataColumn("IdParametro", Type.GetType("System.Int32"));
            cTipoDato = new DataColumn("TipoDato", Type.GetType("System.String"));
            cLongitud = new DataColumn("Longitud", Type.GetType("System.String"));
            cDecimal = new DataColumn("Decimal", Type.GetType("System.String"));
            cSelecciona = new DataColumn("Selecciona", Type.GetType("System.Boolean"));

            objDataTable.Columns.Add(cIdOperacion);
            objDataTable.Columns.Add(cIdParametro);
            objDataTable.Columns.Add(cTipoDato);
            objDataTable.Columns.Add(cLongitud);
            objDataTable.Columns.Add(cDecimal);
            objDataTable.Columns.Add(cSelecciona);

            for (int i = 1; i <= dsParametros.Tables.Count; i++)
            {
                if (dsParametros.Tables["Parametro" + i] != null)
                {
                    for (int j = 0; j < dsParametros.Tables["Parametro" + i].Rows.Count; j++)
                    {
                        DataRow row = objDataTable.NewRow();
                        row["IdOperacion"] = Convert.ToInt32(dsParametros.Tables["Parametro" + i].Rows[j]["IdOperacion"].ToString());
                        row["IdParametro"] = Convert.ToInt32(dsParametros.Tables["Parametro" + i].Rows[j]["IdParametro"].ToString());
                        row["TipoDato"] = dsParametros.Tables["Parametro" + i].Rows[j]["TipoDato"].ToString();
                        row["Longitud"] = dsParametros.Tables["Parametro" + i].Rows[j]["Longitud"].ToString();
                        row["Decimal"] = dsParametros.Tables["Parametro" + i].Rows[j]["Decimal"].ToString();
                        row["Selecciona"] = Convert.ToBoolean(Convert.ToInt32(dsParametros.Tables["Parametro" + i].Rows[j]["Selecciona"].ToString()));

                        objDataTable.Rows.Add(row);
                    }
                }
                else
                    break;
            }

            dsParametrosOperacion.Tables.Add(objDataTable);

            return dsParametrosOperacion;
        }

        /// <summary>
        /// Lee del XML los datos de las operaciones segun tipo de operacion
        /// </summary>
        /// <param name="idOperacion">Identificador de la operación</param>
        /// <returns>Retorna objeto de tipo EOperacion</returns>
        /// 
        public static EOperacion LeerOperacionXMLxIdOperacion(string idOperacion)
        {
            List<EOperacion> olstOperaciones = LeerOperacionXML();

            EOperacion datosOperacion = (from c in olstOperaciones
                                         where c.IdOperacion == Convert.ToInt32(idOperacion)
                                         select new EOperacion
                                         {
                                             IdOperacion = c.IdOperacion,
                                             DesOperacion = c.DesOperacion,
                                             Comandos = c.Comandos,
                                             ProgramaAS400 = c.ProgramaAS400,
                                             ProcesoBatch = c.ProcesoBatch,
                                             Habilitado = c.Habilitado,
                                             OmisionParametro = c.OmisionParametro
                                         }).First();

            return datosOperacion;
        }

        /// <summary>
        /// Genera el XML con todas las operaciones
        /// </summary>
        /// <param name="oDatoOperacion">Objeto con los datos de la operacion</param>
        /// <param name="lstParametrosOperacion">Lista de objetos con los parametros seleccionados para la operacion</param>
        /// 
        public static void GeneraXML(int idOperacion, EOperacion oDatoOperacion, List<EParametrosOperacion> lstParametrosOperacion)
        {
            DataSet ds_TablaParametros;
            DataTable dt_Operacion;
            DataTable dt_Parametros;
            List<EOperacion> lstDatosOperacion;
            List<EParametrosOperacion> lstDatosParametros;
            List<EParametrosOperacion> lstParametros;
            XmlDocument xmlDoc;
            int nroParametro = 1;

            try
            {
                string rutaFileXML = ConfigurationManager.AppSettings["PathXML"] + ConfigurationManager.AppSettings["ArchivoXML"];
                ds_TablaParametros = new DataSet();

                if (File.Exists(rutaFileXML))
                {
                    FileInfo file = new FileInfo(rutaFileXML);
                    if (file.Length > 0)
                    {
                        ds_TablaParametros.ReadXml(rutaFileXML);

                        if (ds_TablaParametros.Tables.Count > 0)
                        {
                            dt_Operacion = ds_TablaParametros.Tables[0];
                            dt_Parametros = GenerateDataSet(ds_TablaParametros).Tables[0];

                            //Extraemos las operaciones, excepto la que se modificará
                            lstDatosOperacion = (from o in dt_Operacion.AsEnumerable()
                                                 where Convert.ToInt32(o.Field<string>("IdOperacion")) != idOperacion
                                                 orderby Convert.ToInt32(o.Field<string>("IdOperacion"))
                                                 select new EOperacion
                                                 {
                                                     IdOperacion = Convert.ToInt32(o.Field<string>("IdOperacion")),
                                                     DesOperacion = o.Field<string>("DesOperacion"),
                                                     Comandos = o.Field<string>("Comandos"),
                                                     OmisionParametro = Convert.ToBoolean(Convert.ToInt32(o.Field<string>("OmiteParametros"))),
                                                     ProgramaAS400 = o.Field<string>("ProgramaAS400"),
                                                     ProcesoBatch = Convert.ToBoolean(Convert.ToInt32(o.Field<string>("ProcesoBatch"))),
                                                     Habilitado = Convert.ToBoolean(Convert.ToInt32(o.Field<string>("Habilitado"))),
                                                     FechaCreacion = o.Field<string>("FechaCreacion"),
                                                 }).ToList();

                            if (oDatoOperacion != null)
                            {
                                lstDatosOperacion.Add(oDatoOperacion);
                            }

                            //Extraemos los parametros, excepto las que pertenecen a la operacion a modificar
                            lstDatosParametros = (from p in dt_Parametros.AsEnumerable()
                                                  where (p.Field<int>("IdOperacion") != idOperacion)
                                                  orderby p.Field<int>("IdOperacion"), p.Field<int>("IdParametro")
                                                  select new EParametrosOperacion
                                                  {
                                                      IdOperacion = p.Field<int>("IdOperacion"),
                                                      IdParametro = p.Field<int>("IdParametro"),
                                                      TipoDato = p.Field<string>("TipoDato"),
                                                      Longitud = p.Field<string>("Longitud"),
                                                      Decimal = p.Field<string>("Decimal"),
                                                      Selecciona = p.Field<bool>("Selecciona")
                                                  }).ToList();

                            if (lstParametrosOperacion != null)
                            {
                                lstParametros = lstDatosParametros.Union(lstParametrosOperacion).ToList();
                            }
                            else
                            {
                                lstParametros = lstDatosParametros;
                            }

                            // Creando el XML 
                            xmlDoc = new XmlDocument();
                            XmlNode xmlNode = xmlDoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                            xmlDoc.AppendChild(xmlNode);
                            XmlComment xmlcomentario = xmlDoc.CreateComment("Operaciones Banca Celular");
                            xmlDoc.AppendChild(xmlcomentario);

                            XmlNode xmlNodeCabeceraOperacion = xmlDoc.CreateElement("EDatosOperacion");
                            xmlDoc.AppendChild(xmlNodeCabeceraOperacion);

                            foreach (EOperacion itemOperacion in lstDatosOperacion)
                            {
                                nroParametro = 1;

                                XmlNode xmlNodeOperacion = xmlDoc.CreateElement("Operacion");
                                xmlNodeCabeceraOperacion.AppendChild(xmlNodeOperacion);

                                XmlElement elem_idOperacion = xmlDoc.CreateElement("IdOperacion");
                                XmlElement elem_desOperacion = xmlDoc.CreateElement("DesOperacion");
                                XmlElement elem_comandos = xmlDoc.CreateElement("Comandos");
                                XmlElement elem_omiteParametro = xmlDoc.CreateElement("OmiteParametros");
                                XmlElement elem_fechaCreacion = xmlDoc.CreateElement("FechaCreacion");
                                XmlElement elem_programaAS400 = xmlDoc.CreateElement("ProgramaAS400");
                                XmlElement elem_procesoBatch = xmlDoc.CreateElement("ProcesoBatch");
                                XmlElement elem_habilitado = xmlDoc.CreateElement("Habilitado");

                                XmlText text_idOperacion = xmlDoc.CreateTextNode(itemOperacion.IdOperacion.ToString());
                                XmlText text_desOperacion = xmlDoc.CreateTextNode(itemOperacion.DesOperacion);
                                XmlText text_comandos = xmlDoc.CreateTextNode(itemOperacion.Comandos);
                                XmlText text_omiteParametro = xmlDoc.CreateTextNode(Convert.ToInt32(itemOperacion.OmisionParametro).ToString());
                                XmlText text_fechaCreacion = xmlDoc.CreateTextNode(itemOperacion.FechaCreacion);
                                XmlText text_programaAS400 = xmlDoc.CreateTextNode(itemOperacion.ProgramaAS400);
                                XmlText text_procesoBatch = xmlDoc.CreateTextNode(Convert.ToInt32(itemOperacion.ProcesoBatch).ToString());
                                XmlText text_habilitado = xmlDoc.CreateTextNode(Convert.ToInt32(itemOperacion.Habilitado).ToString());

                                elem_idOperacion.AppendChild(text_idOperacion);
                                elem_desOperacion.AppendChild(text_desOperacion);
                                elem_comandos.AppendChild(text_comandos);
                                elem_omiteParametro.AppendChild(text_omiteParametro);
                                elem_fechaCreacion.AppendChild(text_fechaCreacion);
                                elem_programaAS400.AppendChild(text_programaAS400);
                                elem_procesoBatch.AppendChild(text_procesoBatch);
                                elem_habilitado.AppendChild(text_habilitado);

                                xmlNodeOperacion.AppendChild(elem_idOperacion);
                                xmlNodeOperacion.AppendChild(elem_desOperacion);
                                xmlNodeOperacion.AppendChild(elem_comandos);
                                xmlNodeOperacion.AppendChild(elem_omiteParametro);
                                xmlNodeOperacion.AppendChild(elem_fechaCreacion);
                                xmlNodeOperacion.AppendChild(elem_programaAS400);
                                xmlNodeOperacion.AppendChild(elem_procesoBatch);
                                xmlNodeOperacion.AppendChild(elem_habilitado);

                                XmlNode xmlParametros = xmlDoc.CreateElement("Parametros");
                                xmlNodeOperacion.AppendChild(xmlParametros);

                                foreach (EParametrosOperacion itemParametro in lstParametros)
                                {
                                    if (itemOperacion.IdOperacion == itemParametro.IdOperacion)
                                    {
                                        XmlNode xmlNodeParametro = xmlDoc.CreateElement("Parametro" + nroParametro);

                                        XmlAttribute atributo_IdOperacion = xmlDoc.CreateAttribute("IdOperacion");
                                        XmlAttribute atributo_IdParametro = xmlDoc.CreateAttribute("IdParametro");
                                        XmlAttribute atributo_TipoDato = xmlDoc.CreateAttribute("TipoDato");
                                        XmlAttribute atributo_Longitud = xmlDoc.CreateAttribute("Longitud");
                                        XmlAttribute atributo_Decimal = xmlDoc.CreateAttribute("Decimal");
                                        XmlAttribute atributo_Selecciona = xmlDoc.CreateAttribute("Selecciona");

                                        atributo_IdOperacion.InnerText = itemParametro.IdOperacion.ToString();
                                        atributo_IdParametro.InnerText = itemParametro.IdParametro.ToString();
                                        atributo_TipoDato.InnerText = itemParametro.TipoDato;
                                        atributo_Longitud.InnerText = itemParametro.Longitud;
                                        atributo_Decimal.InnerText = itemParametro.Decimal;
                                        atributo_Selecciona.InnerText = Convert.ToInt32(itemParametro.Selecciona).ToString();

                                        xmlNodeParametro.Attributes.Append(atributo_IdOperacion);
                                        xmlNodeParametro.Attributes.Append(atributo_IdParametro);
                                        xmlNodeParametro.Attributes.Append(atributo_TipoDato);
                                        xmlNodeParametro.Attributes.Append(atributo_Longitud);
                                        xmlNodeParametro.Attributes.Append(atributo_Decimal);
                                        xmlNodeParametro.Attributes.Append(atributo_Selecciona);

                                        xmlParametros.AppendChild(xmlNodeParametro);

                                        nroParametro++;
                                    }
                                }

                            }

                            xmlDoc.Save(rutaFileXML);
                        }
                        else
                            CrearPrimeraOperacion(oDatoOperacion, lstParametrosOperacion);
                    }
                    else
                        CrearPrimeraOperacion(oDatoOperacion, lstParametrosOperacion);
                }
                else
                    CrearPrimeraOperacion(oDatoOperacion, lstParametrosOperacion);
            }
            catch (Exception)
            {
                throw new Exception("Error al general el XML");
            }
            finally
            {
                ds_TablaParametros = null;
                dt_Operacion = null;
                dt_Parametros = null;
                lstDatosOperacion = null;
                lstDatosParametros = null;
                lstParametros = null;
                xmlDoc = null;
            }
        }

        private static void CrearPrimeraOperacion(EOperacion oDatoOperacion, List<EParametrosOperacion> lstParametrosOperacion)
        {
            string rutaFileXML = ConfigurationManager.AppSettings["PathXML"] + ConfigurationManager.AppSettings["ArchivoXML"];
            int nroParametro = 1;
            XmlDocument xmlDoc;

            // Creando el XML 
            xmlDoc = new XmlDocument();
            XmlNode xmlNode = xmlDoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            xmlDoc.AppendChild(xmlNode);
            XmlComment xmlcomentario = xmlDoc.CreateComment("Operaciones Banca Celular");
            xmlDoc.AppendChild(xmlcomentario);

            XmlNode xmlNodeCabeceraOperacion = xmlDoc.CreateElement("EDatosOperacion");
            xmlDoc.AppendChild(xmlNodeCabeceraOperacion);


            XmlNode xmlNodeOperacion = xmlDoc.CreateElement("Operacion");
            xmlNodeCabeceraOperacion.AppendChild(xmlNodeOperacion);

            XmlElement elem_idOperacion = xmlDoc.CreateElement("IdOperacion");
            XmlElement elem_desOperacion = xmlDoc.CreateElement("DesOperacion");
            XmlElement elem_comandos = xmlDoc.CreateElement("Comandos");
            XmlElement elem_OmiteParametros = xmlDoc.CreateElement("OmiteParametros");
            XmlElement elem_fechaCreacion = xmlDoc.CreateElement("FechaCreacion");
            XmlElement elem_programaAS400 = xmlDoc.CreateElement("ProgramaAS400");
            XmlElement elem_procesoBatch = xmlDoc.CreateElement("ProcesoBatch");
            XmlElement elem_habilitado = xmlDoc.CreateElement("Habilitado");

            XmlText text_idOperacion = xmlDoc.CreateTextNode(oDatoOperacion.IdOperacion.ToString());
            XmlText text_desOperacion = xmlDoc.CreateTextNode(oDatoOperacion.DesOperacion);
            XmlText text_comandos = xmlDoc.CreateTextNode(oDatoOperacion.Comandos);
            XmlText text_OmiteParametros = xmlDoc.CreateTextNode(Convert.ToInt32(oDatoOperacion.OmisionParametro).ToString());
            XmlText text_fechaCreacion = xmlDoc.CreateTextNode(oDatoOperacion.FechaCreacion);
            XmlText text_programaAS400 = xmlDoc.CreateTextNode(oDatoOperacion.ProgramaAS400);
            XmlText text_procesoBatch = xmlDoc.CreateTextNode(Convert.ToInt32(oDatoOperacion.ProcesoBatch).ToString());
            XmlText text_habilitado = xmlDoc.CreateTextNode(Convert.ToInt32(oDatoOperacion.Habilitado).ToString());

            elem_idOperacion.AppendChild(text_idOperacion);
            elem_desOperacion.AppendChild(text_desOperacion);
            elem_comandos.AppendChild(text_comandos);
            elem_OmiteParametros.AppendChild(text_OmiteParametros);
            elem_fechaCreacion.AppendChild(text_fechaCreacion);
            elem_programaAS400.AppendChild(text_programaAS400);
            elem_procesoBatch.AppendChild(text_procesoBatch);
            elem_habilitado.AppendChild(text_habilitado);

            xmlNodeOperacion.AppendChild(elem_idOperacion);
            xmlNodeOperacion.AppendChild(elem_desOperacion);
            xmlNodeOperacion.AppendChild(elem_comandos);
            xmlNodeOperacion.AppendChild(elem_OmiteParametros);
            xmlNodeOperacion.AppendChild(elem_fechaCreacion);
            xmlNodeOperacion.AppendChild(elem_programaAS400);
            xmlNodeOperacion.AppendChild(elem_procesoBatch);
            xmlNodeOperacion.AppendChild(elem_habilitado);

            XmlNode xmlParametros = xmlDoc.CreateElement("Parametros");
            xmlNodeOperacion.AppendChild(xmlParametros);

            foreach (EParametrosOperacion itemParametro in lstParametrosOperacion)
            {
                XmlNode xmlNodeParametro = xmlDoc.CreateElement("Parametro" + nroParametro);

                XmlAttribute atributo_IdOperacion = xmlDoc.CreateAttribute("IdOperacion");
                XmlAttribute atributo_IdParametro = xmlDoc.CreateAttribute("IdParametro");
                XmlAttribute atributo_TipoDato = xmlDoc.CreateAttribute("TipoDato");
                XmlAttribute atributo_Longitud = xmlDoc.CreateAttribute("Longitud");
                XmlAttribute atributo_Decimal = xmlDoc.CreateAttribute("Decimal");
                XmlAttribute atributo_Selecciona = xmlDoc.CreateAttribute("Selecciona");

                atributo_IdOperacion.InnerText = itemParametro.IdOperacion.ToString();
                atributo_IdParametro.InnerText = itemParametro.IdParametro.ToString();
                atributo_TipoDato.InnerText = itemParametro.TipoDato;
                atributo_Longitud.InnerText = itemParametro.Longitud;
                atributo_Decimal.InnerText = itemParametro.Decimal;
                atributo_Selecciona.InnerText = Convert.ToInt32(itemParametro.Selecciona).ToString();

                xmlNodeParametro.Attributes.Append(atributo_IdOperacion);
                xmlNodeParametro.Attributes.Append(atributo_IdParametro);
                xmlNodeParametro.Attributes.Append(atributo_TipoDato);
                xmlNodeParametro.Attributes.Append(atributo_Longitud);
                xmlNodeParametro.Attributes.Append(atributo_Decimal);
                xmlNodeParametro.Attributes.Append(atributo_Selecciona);

                xmlParametros.AppendChild(xmlNodeParametro);

                nroParametro++;
            }

            xmlDoc.Save(rutaFileXML);
        }

        /// <summary>
        /// 
        /// </summary>
        public static int ObtenerSiguienteOperacion()
        {
            List<EOperacion> olstOperacion = LeerOperacionXML();
            int nroOperacion = 0;

            if (olstOperacion != null)
            {
                if (olstOperacion.Count > 0)
                {
                    nroOperacion = (from c in olstOperacion
                                    orderby c.IdOperacion descending
                                    select c.IdOperacion).First();
                }
            }

            return (nroOperacion + 1);

        }

        /// <summary>
        /// Valida si existe registrado algun comando de la operacion
        /// </summary>
        /// <param name="idOperacion">Codigo de la operacion</param>
        /// <param name="comando1">Comando 1</param>
        /// <param name="comando2">Comando 2</param>
        /// <param name="comando3">Comando 3</param>
        /// <returns></returns>
        public static bool ExisteComandosOperacion(int idOperacion, string comando1, string comando2, string comando3)
        {
            DataSet ds_TablaParametros;
            bool existeComando = false;

            string rutaFileXML = ConfigurationManager.AppSettings["PathXML"] + ConfigurationManager.AppSettings["ArchivoXML"];
            ds_TablaParametros = new DataSet();

            if (File.Exists(rutaFileXML))
            {
                FileInfo file = new FileInfo(rutaFileXML);
                if (file.Length > 0)
                {
                    ds_TablaParametros.ReadXml(rutaFileXML);

                    if (ds_TablaParametros.Tables.Count > 0)
                    {
                        if (ds_TablaParametros.Tables[0].Rows.Count > 0)
                        {
                            //Extraemos las operaciones, excepto la que se modificará
                            var comandos = (from o in ds_TablaParametros.Tables[0].AsEnumerable()
                                            where o.Field<string>("IdOperacion") != idOperacion.ToString()
                                            select new EOperacion
                                            {
                                                Comandos = o.Field<string>("Comandos"),
                                            }).ToList();


                            if (comandos.Count > 0)
                            {
                                foreach (var item in comandos)
                                {
                                    if (!existeComando)
                                    {
                                        string[] array = item.Comandos.Split('-');

                                        for (int i = 0; i < array.Length; i++)
                                        {
                                            if (!String.IsNullOrEmpty(array[i].Trim()))
                                            {
                                                if ((array[i].Trim() == comando1.Trim()) ||
                                                    (array[i].Trim() == comando2.Trim()) ||
                                                    (array[i].Trim() == comando3.Trim()))
                                                {
                                                    existeComando = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return existeComando;
        }

        /// <summary>
        /// Verifica que no haya duplicidad de operaciones
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool ExisteIDOperacion(int idOperacion)
        {
            DataSet ds_TablaParametros;
            bool existeIDOperacion = false;

            string rutaFileXML = ConfigurationManager.AppSettings["PathXML"] + ConfigurationManager.AppSettings["ArchivoXML"];
            ds_TablaParametros = new DataSet();

            if (File.Exists(rutaFileXML))
            {
                FileInfo file = new FileInfo(rutaFileXML);
                if (file.Length > 0)
                {
                    ds_TablaParametros.ReadXml(rutaFileXML);

                    if (ds_TablaParametros.Tables.Count > 0)
                    {
                        if (ds_TablaParametros.Tables[0].Rows.Count > 0)
                        {
                            List<EOperacion> lstDatosOperacion = (from o in ds_TablaParametros.Tables[0].AsEnumerable()
                                                                  where Convert.ToInt32(o.Field<string>("IdOperacion")) == idOperacion
                                                                  orderby Convert.ToInt32(o.Field<string>("IdOperacion"))
                                                                  select new EOperacion
                                                                  {
                                                                      IdOperacion = Convert.ToInt32(o.Field<string>("IdOperacion")),
                                                                  }).ToList();

                            if (lstDatosOperacion.Count > 0)
                            {
                                existeIDOperacion = true;
                            }
                        }
                    }
                }
            }

            return existeIDOperacion;
        }

        /// <summary>
        /// Valida el acceso del usuario al administrador
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        /// <summary>

        public static EItemsParametros ValidarAcceso(string usuario)
        {
            EItemsParametros[] arrayItemsParametros;
            EItemsParametros oItemsParametros = null;

            try
            {
                arrayItemsParametros = ADTablaMaestra.ObtenerItemsParametros(CConstantes.TablaMaestras.TABLA_USUARIOS);

                foreach (var item in arrayItemsParametros)
                {
                    if (item.ValItemTabla.Trim().ToUpper() == usuario.Trim().ToUpper())
                    {
                        oItemsParametros = new EItemsParametros();
                        oItemsParametros.IdTabla = item.IdTabla;
                        oItemsParametros.IdItemTabla = item.IdItemTabla;
                        oItemsParametros.ValItemTabla = item.ValItemTabla;
                        oItemsParametros.DescLargaItemTabla = item.DescLargaItemTabla;
                        break;
                    }
                }

                return oItemsParametros;

            }
            catch (Exception)
            {
                return oItemsParametros;
            }
            finally
            {
                arrayItemsParametros = null;
                oItemsParametros = null;
            }
        }

    }
}
