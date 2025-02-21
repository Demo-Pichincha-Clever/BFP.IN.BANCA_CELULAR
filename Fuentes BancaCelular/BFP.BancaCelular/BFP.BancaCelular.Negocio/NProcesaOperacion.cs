using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Web;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using System.Threading;

using BFP.BancaCelular.Entidad;
using BFP.BancaCelular.Comun;
using BFP.BancaCelular.Data;

namespace BFP.BancaCelular.Negocio
{
    /// <summary>
    /// Manejador de la lógica de negocio de operaciones
    /// </summary>
    /// 
    public static class NProcesaOperacion
    {


        /// <summary>
        /// Procesa la operacion y retorna el mensaje MT
        /// </summary>
        /// <param name="idTransaccion">Codigo de transaccion, generador por YP</param>
        /// <param name="idOperacion">Codigo de la operacion a procesar</param>
        /// <param name="numeroTelefono">Numero de telefono del cliente</param>
        /// <param name="idOperadora">Operadora que pertenece el numero del cliente</param>
        /// <param name="idTransaccionVerifica">Opcional, para efectos de verificar si la operacion se proceso ante caidas de timeout</param>
        /// <param name="strParametrosOperacion">Conjunto de parametros requeridos para la operacion. Estos estan separados por el caracter "|"</param>
        /// <returns>CodigoMensaje, Mensaje MT, Fecha proceso, Hora proceso</returns>
        /// 
        public static EResultadoMensajeMT ProcesaOperacion(string idTransaccion, int idOperacion, 
                                                            string numeroTelefono, int idOperadora, 
                                                            string idTransaccionVerifica, string strParametrosOperacion)
        
        {
            EResultadoMensajeMT oResultadoMensajeMT = new EResultadoMensajeMT();
            EDatosOperacion oDatosOperacion = new EDatosOperacion();
            string strErrorGeneradoBloqueo = "0000";
            ETrama oTramaEnvio;
            bool errorSintaxis = false;
            ECatalogo<string, string> oTipoOperacion = new ECatalogo<string,string>();
            string usuario = ConfigurationManager.AppSettings["UsuarioAdiministrador"].ToString();
            

            try
            {
               

                oDatosOperacion.IdTransaccion = idTransaccion;
                oDatosOperacion.IdOperacion = idOperacion;
                oDatosOperacion.NumeroTelefono = numeroTelefono;
                oDatosOperacion.IdOperadora = idOperadora;
                oDatosOperacion.IdTransaccionVerifica = String.IsNullOrEmpty(idTransaccionVerifica) ? "0" : idTransaccionVerifica.ToString();
                ObtenerDatosXML(ref oDatosOperacion, idOperacion);
                //Verifica Bloqueo Temporal
                bool booIndicadorBloqueo = NGeneral.VerificarBloqueoTemporalPorDesafiliacion(numeroTelefono, idOperacion, out strErrorGeneradoBloqueo);
                if (!booIndicadorBloqueo) oDatosOperacion.Habilitado = false;
                if (oDatosOperacion.Habilitado)
                {
                    //Genera la trama de los datos fijos de la operacion
                    string tramaBaseOperacion = GeneraTramaBaseOperacion(oDatosOperacion);
                    if (tramaBaseOperacion != null)
                    {
                        string parametrosOperacion = String.IsNullOrEmpty(strParametrosOperacion) ? string.Empty : strParametrosOperacion;

                        //Lee el archivo XML por operacion
                        List<EParametrosOperacion> lstParametrosOperacion = LeerParametrosOperacionXML(idOperacion, parametrosOperacion, ref errorSintaxis);

                        if (lstParametrosOperacion.Count > 0)
                        {
                            if (!errorSintaxis)
                            {
                                //Genera la trama de los parametros de la operacion
                                string tramaParametrosOperacion = GeneraTramaParametrosOperacion(lstParametrosOperacion);

                                if (!String.IsNullOrEmpty(tramaParametrosOperacion))
                                {
                                    oTramaEnvio = new ETrama();
                                    oTramaEnvio.Entrada = String.Concat(tramaBaseOperacion, tramaParametrosOperacion);
                                    oTramaEnvio.HoraInicial = DateTime.Now;


                                    //Ejecuta la operacion
                                    oResultadoMensajeMT = NTransacciones.EjecutaTransaccionOperacion(oTramaEnvio, oDatosOperacion);
                                    if (oResultadoMensajeMT.CodigoError == "0000")
                                        NGeneral.RegistrarIntetosOperacionDia(numeroTelefono, idOperacion);
                                    if (oResultadoMensajeMT == null)
                                    {
                                        oTipoOperacion.Valor = idOperacion.ToString().PadLeft(4, '0');
                                        oTipoOperacion.Nombre = oDatosOperacion.NombreOperacion;

                                        EMensajeOperacion oMensajeOperacion = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.ERROR, string.Empty);
                                        oResultadoMensajeMT.CodRet = oMensajeOperacion.IdMensaje;
                                        oResultadoMensajeMT.MensajeMT = oMensajeOperacion.DescMensaje;
                                        oResultadoMensajeMT.Fecha = CCUtil.FormateaFecha(String.Format("{0:ddMMyyyy}", DateTime.Now));
                                        oResultadoMensajeMT.Hora = CCUtil.FormateaHora(String.Format("{0:HHmmss}", DateTime.Now));
                                        oResultadoMensajeMT.MensajeError = CConstantes.MensajesError.ERROR_PROCESAR_OPERACION;

                                        NLog.InsertarLogOperaciones(oDatosOperacion.IdTransaccion.Trim(), oTipoOperacion, CConstantes.EstadosLog.ERROR,
                                                   oTramaEnvio.Entrada, CSerializacion.SerializarXML<EResultadoMensajeMT>(oResultadoMensajeMT),
                                                   oTramaEnvio.HoraInicial, oResultadoMensajeMT.MensajeError, oDatosOperacion.IdTransaccionVerifica.Trim());

                                    }
                                }
                                else
                                {
                                    oTipoOperacion.Valor = idOperacion.ToString().PadLeft(4, '0');
                                    oTipoOperacion.Nombre = oDatosOperacion.NombreOperacion;

                                    EMensajeOperacion oMensajeOperacion = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.ERROR, string.Empty);
                                    oResultadoMensajeMT.CodRet = oMensajeOperacion.IdMensaje;
                                    oResultadoMensajeMT.MensajeMT = oMensajeOperacion.DescMensaje;
                                    oResultadoMensajeMT.Fecha = CCUtil.FormateaFecha(String.Format("{0:ddMMyyyy}", DateTime.Now));
                                    oResultadoMensajeMT.Hora = CCUtil.FormateaHora(String.Format("{0:HHmmss}", DateTime.Now));
                                    oResultadoMensajeMT.MensajeError = CConstantes.MensajesError.GENERACION_TRAMA_PARAMETROS;

                                    NLog.InsertarLogOperaciones(oDatosOperacion.IdTransaccion.Trim(), oTipoOperacion, CConstantes.EstadosLog.ERROR,
                                                tramaBaseOperacion, CSerializacion.SerializarXML<EResultadoMensajeMT>(oResultadoMensajeMT),
                                                DateTime.Now, oResultadoMensajeMT.MensajeError, oDatosOperacion.IdTransaccionVerifica.Trim());

                                }
                            }
                            else
                            {
                                oTipoOperacion.Valor = idOperacion.ToString().PadLeft(4, '0');
                                oTipoOperacion.Nombre = oDatosOperacion.NombreOperacion;

                                EMensajeOperacion oMensajeOperacion = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.ERROR_SINTAXIS_OMISION, string.Empty);

                                oResultadoMensajeMT.CodRet = oMensajeOperacion.IdMensaje;
                                oResultadoMensajeMT.MensajeMT = oMensajeOperacion.DescMensaje;
                                oResultadoMensajeMT.Fecha = CCUtil.FormateaFecha(String.Format("{0:ddMMyyyy}", DateTime.Now));
                                oResultadoMensajeMT.Hora = CCUtil.FormateaHora(String.Format("{0:HHmmss}", DateTime.Now));
                                oResultadoMensajeMT.MensajeError = CConstantes.MensajesError.OMISION_PRIMER_PARAMETRO;

                                NLog.InsertarLogOperaciones(oDatosOperacion.IdTransaccion.Trim(), oTipoOperacion, CConstantes.EstadosLog.ERROR,
                                                tramaBaseOperacion, CSerializacion.SerializarXML<EResultadoMensajeMT>(oResultadoMensajeMT),
                                                DateTime.Now, oResultadoMensajeMT.MensajeError, oDatosOperacion.IdTransaccionVerifica.Trim());

                            }
                        }
                        else
                        {
                            oTipoOperacion.Valor = idOperacion.ToString().PadLeft(4, '0');
                            oTipoOperacion.Nombre = oDatosOperacion.NombreOperacion;

                            EMensajeOperacion oMensajeOperacion = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.ERROR, string.Empty);
                            oResultadoMensajeMT.CodRet = oMensajeOperacion.IdMensaje;
                            oResultadoMensajeMT.MensajeMT = oMensajeOperacion.DescMensaje;
                            oResultadoMensajeMT.Fecha = CCUtil.FormateaFecha(String.Format("{0:ddMMyyyy}", DateTime.Now));
                            oResultadoMensajeMT.Hora = CCUtil.FormateaHora(String.Format("{0:HHmmss}", DateTime.Now));
                            oResultadoMensajeMT.MensajeError = CConstantes.MensajesError.OPERACION_NO_REGISTRADA;

                            NLog.InsertarLogOperaciones(oDatosOperacion.IdTransaccion.Trim(), oTipoOperacion, CConstantes.EstadosLog.ERROR,
                                         strParametrosOperacion, CSerializacion.SerializarXML<EResultadoMensajeMT>(oResultadoMensajeMT),
                                         DateTime.Now, oResultadoMensajeMT.MensajeError, oDatosOperacion.IdTransaccionVerifica.Trim());

                            //Inserta Error
                            NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                                    "NProcesaOperacion.cs : LeerParametrosOperacionXML", strParametrosOperacion,
                                    oResultadoMensajeMT.MensajeError, DateTime.Now, CConstantes.CodigoAplicativo.ServicioWeb);
                        }
                    }
                    else 
                    {
                        oTipoOperacion.Valor = idOperacion.ToString().PadLeft(4, '0');
                        oTipoOperacion.Nombre = oDatosOperacion.NombreOperacion;

                        EMensajeOperacion oMensajeOperacion = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.ERROR_GENERACION_SINTAXIS_INCORRECTA, string.Empty);
                        oResultadoMensajeMT.CodRet = oMensajeOperacion.IdMensaje;
                        oResultadoMensajeMT.MensajeMT = oMensajeOperacion.DescMensaje;
                        oResultadoMensajeMT.Fecha = CCUtil.FormateaFecha(String.Format("{0:ddMMyyyy}", DateTime.Now));
                        oResultadoMensajeMT.Hora = CCUtil.FormateaHora(String.Format("{0:HHmmss}", DateTime.Now));
                        oResultadoMensajeMT.MensajeError = CConstantes.MensajesError.GENERACION_TRAMA_BASE;

                        NLog.InsertarLogOperaciones(oDatosOperacion.IdTransaccion.Trim(), oTipoOperacion, CConstantes.EstadosLog.ERROR,
                                     strParametrosOperacion, CSerializacion.SerializarXML<EResultadoMensajeMT>(oResultadoMensajeMT),
                                     DateTime.Now, oResultadoMensajeMT.MensajeError, oDatosOperacion.IdTransaccionVerifica.Trim());

                        //Inserta Error
                        NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                                "NProcesaOperacion.cs : GeneraTramaBaseOperacion", strParametrosOperacion,
                                oResultadoMensajeMT.MensajeError, DateTime.Now, CConstantes.CodigoAplicativo.ServicioWeb);
                    }

                }
                else
                {
                    oTipoOperacion.Valor = idOperacion.ToString().PadLeft(4, '0');
                    oTipoOperacion.Nombre = oDatosOperacion.NombreOperacion;
                    string strCodigoError = CConstantes.CodigoMensajeRetorno.ERROR_OPERACION_DESHABILITADA;
                    string strMensajeError = CConstantes.MensajesError.OPERACION_NO_HABILITADA;
                    if (!booIndicadorBloqueo) { strCodigoError = strErrorGeneradoBloqueo; strMensajeError = "Bloqueo por Desafiliacion o Intentos"; }
                    EMensajeOperacion oMensajeOperacion = ADMensajeOperacion.ObtenerMensajeOperacion(strCodigoError, string.Empty);
                    oResultadoMensajeMT.CodRet = oMensajeOperacion.IdMensaje;
                    oResultadoMensajeMT.MensajeMT = oMensajeOperacion.DescMensaje;
                    oResultadoMensajeMT.Fecha = CCUtil.FormateaFecha(String.Format("{0:ddMMyyyy}", DateTime.Now));
                    oResultadoMensajeMT.Hora = CCUtil.FormateaHora(String.Format("{0:HHmmss}", DateTime.Now));
                    oResultadoMensajeMT.MensajeError = strMensajeError;

                    NLog.InsertarLogOperaciones(oDatosOperacion.IdTransaccion.Trim(), oTipoOperacion, CConstantes.EstadosLog.ADVERTENCIA,
                        string.Empty, CSerializacion.SerializarXML<EResultadoMensajeMT>(oResultadoMensajeMT),
                        DateTime.Now, oResultadoMensajeMT.MensajeError, oDatosOperacion.IdTransaccionVerifica.Trim());

                    //Inserta Error
                    NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                            "NProcesaOperacion.cs : ProcesaOperacion", strParametrosOperacion,
                            oResultadoMensajeMT.MensajeError, DateTime.Now, CConstantes.CodigoAplicativo.ServicioWeb);

                }

                return oResultadoMensajeMT;

            }
            catch (Exception ex)
            {
                oTipoOperacion.Valor = idOperacion.ToString().PadLeft(4, '0');
                oTipoOperacion.Nombre = oDatosOperacion.NombreOperacion;

                EMensajeOperacion oMensajeOperacion = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.ERROR, string.Empty);
                oResultadoMensajeMT.CodRet = oMensajeOperacion.IdMensaje;
                oResultadoMensajeMT.MensajeMT = oMensajeOperacion.DescMensaje;
                oResultadoMensajeMT.Fecha = CCUtil.FormateaFecha(String.Format("{0:ddMMyyyy}", DateTime.Now));
                oResultadoMensajeMT.Hora = CCUtil.FormateaHora(String.Format("{0:HHmmss}", DateTime.Now));
                oResultadoMensajeMT.MensajeError = CConstantes.MensajesError.ERROR_PROCESAR_OPERACION;

                NLog.InsertarLogOperaciones(oDatosOperacion.IdTransaccion.Trim(), oTipoOperacion, CConstantes.EstadosLog.ERROR,
                                  numeroTelefono, CSerializacion.SerializarXML<EResultadoMensajeMT>(oResultadoMensajeMT),
                                  DateTime.Now, oResultadoMensajeMT.MensajeError, oDatosOperacion.IdTransaccionVerifica.Trim());


                //Inserta Error
                NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                        "NProcesaOperacion.cs : ProcesaOperacion", string.Empty, ex.Message,
                        DateTime.Now, CConstantes.CodigoAplicativo.ServicioWeb);

                return oResultadoMensajeMT;
            }
            finally
            {
                oTipoOperacion = null;
                oResultadoMensajeMT = null;
                oDatosOperacion = null;
                oTramaEnvio = null;
            }


        }

        private static void ObtenerDatosXML(ref EDatosOperacion oDatosOperacion, int idOperacion)
        {
            string fileXML = ConfigurationManager.AppSettings["XMLOperaciones"] + ConfigurationManager.AppSettings["ArchivoXML"];
            DataSet ds_TablaParametros = new DataSet();

            try
            {
                if (File.Exists(fileXML))
                {
                    ds_TablaParametros.ReadXml(fileXML);

                    if (ds_TablaParametros != null)
                    {
                        if (ds_TablaParametros.Tables[0].Rows.Count > 0)
                        {
                            List<EDatosOperacion> lstDatosOperacion = (from p in ds_TablaParametros.Tables[0].AsEnumerable()
                                                                       where p.Field<string>("IdOperacion").Trim() == idOperacion.ToString()
                                                                       select new EDatosOperacion
                                                                       {
                                                                           Habilitado = Convert.ToBoolean(Convert.ToInt32(p.Field<string>("Habilitado"))),
                                                                           ProgramaAS400 = p.Field<string>("ProgramaAS400"),
                                                                           NombreOperacion = p.Field<string>("DesOperacion")
                                                                       }).ToList();

                            if (lstDatosOperacion.Count > 0)
                            {
                                foreach (EDatosOperacion item in lstDatosOperacion)
                                {
                                    oDatosOperacion.Habilitado = item.Habilitado;
                                    oDatosOperacion.ProgramaAS400 = item.ProgramaAS400;
                                    oDatosOperacion.NombreOperacion = item.NombreOperacion;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                ds_TablaParametros = null;
            }
            finally
            {
                ds_TablaParametros = null;
            }
        }

        /// <summary>
        /// Genera la trama de los datos fijos de la operacion
        /// </summary>
        /// <param name="oDatosOperacion">Objeto con los datos de la operacion</param>
        /// <returns>Trama concatenada de los datos ingresados</returns>
        private static string GeneraTramaBaseOperacion(EDatosOperacion oDatosOperacion)
        {
            StringBuilder strTramaBase = new StringBuilder();
                     
            try
            {
                strTramaBase.Append(oDatosOperacion.IdTransaccion.Substring(0, 9));
                strTramaBase.Append(oDatosOperacion.IdOperacion.ToString().PadLeft(3, '0').Substring(0, 3));
                strTramaBase.Append(oDatosOperacion.NumeroTelefono.PadRight(15, ' ').Substring(0, 15));
                strTramaBase.Append(oDatosOperacion.IdOperadora.ToString().Substring(0, 1));
                strTramaBase.Append(oDatosOperacion.IdTransaccionVerifica.PadLeft(9, '0').Substring(0, 9));

                return strTramaBase.ToString();

            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                strTramaBase = null;
            }
        }


        /// <summary>
        /// Obtener programa por operacion
        /// </summary>
        /// <returns>Codigo de Transaccion</returns>
        private static string ObtenerProgramaAS400(int idOperacion)
        {
            string fileXML = ConfigurationManager.AppSettings["XMLOperaciones"] + ConfigurationManager.AppSettings["ArchivoXML"];
            List<EParametrosOperacion> lstParametros = new List<EParametrosOperacion>();
            DataSet ds_TablaParametros = new DataSet();
            string programaAS400 = string.Empty;

            try
            {
                if (File.Exists(fileXML))
                {
                    ds_TablaParametros.ReadXml(fileXML);

                    if (ds_TablaParametros != null)
                    {
                        if (ds_TablaParametros.Tables[0].Rows.Count > 0)
                        {
                            List<string> lista = (from p in ds_TablaParametros.Tables[0].AsEnumerable()
                                               where p.Field<string>("IdOperacion").Trim() == idOperacion.ToString()
                                                  select p.Field<string>("ProgramaAS400")).ToList();

                            if (lista.Count > 0)
                            {
                                programaAS400 = lista[0].ToString();
                            }
                        }
                    }
                }

                return programaAS400;
            }
            catch(Exception)
            {
                return programaAS400;
            }
            finally
            {
                ds_TablaParametros = null;
                lstParametros = null;
            }
        }


        /// <summary>
        /// Retorna los parametros requeridos por la operacion a procesar
        /// </summary>
        /// <param name="idOperacion">Codigo de la operacion</param>
        /// <param name="strParametrosOperacion">Conjunto de parametros</param>
        /// <param name="errorSintaxis">Error de sinxtasis, ante omision del primer producto</param>
        /// <returns>Retorna la estructura de parametros requeridos en una lista de objetos</returns>
        /// 
        private static List<EParametrosOperacion> LeerParametrosOperacionXML(int idOperacion, string strParametrosOperacion, ref bool errorSintaxis)
        {
            string fileXML = ConfigurationManager.AppSettings["XMLOperaciones"] + ConfigurationManager.AppSettings["ArchivoXML"];
            List<EParametrosOperacion> lstParametros = new List<EParametrosOperacion>();
            DataSet ds_TablaParametros = new DataSet();
            string usuario = ConfigurationManager.AppSettings["UsuarioAdiministrador"].ToString();
            string[] parametros = null;
            string primerParametro = string.Empty;
            bool omisionParametros = false;

            try
            {
                if (File.Exists(fileXML))
                {
                    ds_TablaParametros.ReadXml(fileXML);

                    if (!String.IsNullOrEmpty(strParametrosOperacion))
                        parametros = strParametrosOperacion.Split('|');

                    int indParametro = 0;

                    //Recopila en un dataset todos los parametros del archivo xml
                    DataSet ds_Parametros = GenerateDataSetParametros(ds_TablaParametros);

                    if (ds_TablaParametros != null)
                    {
                        //Se obtiene el atributo de omision de la operacion
                        if (ds_TablaParametros.Tables[0].Rows.Count > 0)
                        {
                            List<int> lista = (from p in ds_TablaParametros.Tables[0].AsEnumerable()
                                               where p.Field<string>("IdOperacion").Trim() == idOperacion.ToString().Trim()
                                               select Convert.ToInt32(p.Field<string>("OmiteParametros"))).ToList();

                            if (lista.Count > 0)
                                omisionParametros = Convert.ToBoolean(lista[0]);
                        }

                        //Se filtra los parametros segun el id de la operacion
                        if (ds_Parametros.Tables[0].Rows.Count > 0)
                        {
                            lstParametros = (from p in ds_Parametros.Tables[0].AsEnumerable()
                                             where p.Field<int>("IdOperacion") == idOperacion 
                                             orderby p.Field<int>("IdParametro")
                                             select new EParametrosOperacion
                                             {
                                                 IdOperacion = p.Field<int>("IdOperacion"),
                                                 IdParametro = p.Field<int>("IdParametro"),
                                                 TipoDato = p.Field<string>("TipoDato"),
                                                 Longitud = p.Field<string>("Longitud"),
                                                 Decimal = p.Field<string>("Decimal"),
                                                 Selecciona = p.Field<bool>("Selecciona"),
                                                 ValorParametro = string.Empty
                                             }).ToList();


                            // Se asigna valores a los parametros
                            if (parametros != null)
                            {
                                foreach (EParametrosOperacion item in lstParametros)
                                {
                                    if (item.Selecciona)
                                    {
                                        item.ValorParametro = parametros[indParametro].Trim();
                                        indParametro++;
                                    }
                                }

                                primerParametro = (from c in lstParametros
                                                   where c.IdOperacion == idOperacion
                                                   orderby c.IdParametro
                                                   select c.ValorParametro).First();
                            }


                            if (omisionParametros == false && String.IsNullOrEmpty(primerParametro.Trim()))
                                errorSintaxis = true;
                               
                        }
                    }
                }

                return lstParametros;
            }
            catch (Exception ex)
            {
                return lstParametros;
            }
            finally
            {
                lstParametros = null;
                ds_TablaParametros = null;
                parametros = null;
            }
        }

        /// <summary>
        /// Genera un dataset unico a partir de todos los dataset de los parametros
        /// </summary>
        /// <param name="dsParametros">DataSet de parametros</param>
        /// <returns>Retorna dataset unico de parametros</returns>
        /// 
        private static DataSet GenerateDataSetParametros(DataSet dsParametros)
        {
            DataSet dsParametrosOperacion = new DataSet();
            DataTable objDataTable = new DataTable();

            DataColumn cIdOperacion;
            DataColumn cIdParametro;
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
        /// Genera la trama de los parametros que conforman la operacion
        /// </summary>
        /// <param name="lstParametrosOperacion">Lista de objetos de parametros requeridos para la operacion</param>
        /// <param name="strParametrosOperacion">Parametros ingresados por el cliente, separados por el caracter "|"</param>
        /// <returns>Retorna la trama generada de parametros </returns>
        /// 
        private static string GeneraTramaParametrosOperacion(List<EParametrosOperacion> lstParametrosOperacion)
        {
            StringBuilder strTrama = new StringBuilder();

            try
            {
                    foreach (EParametrosOperacion oParametroOperacion in lstParametrosOperacion)
                    {
                        string trama = ElaboraTrama(oParametroOperacion);
                        if (!String.IsNullOrEmpty(trama))
                            strTrama.Append(trama);
                        else
                        {
                            strTrama = null;
                            break;
                        }
                    }

                    if (strTrama  == null)
                        return null;
                    else 
                        return strTrama.ToString();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                lstParametrosOperacion = null;
                strTrama = null;
            }
        }


        /// <summary>
        /// Elabora la trama segun tipo de dato y longitud
        /// </summary>
        /// <param name="valorParametro">Valor del parametro, ingresado por el cliente</param>
        /// <param name="itemParametro">Objeto con la estructura de tipo de datos de los parametros</param>
        /// <returns>Trama concatenada segun tipo y longitud</returns>
        /// 
        private static string ElaboraTrama(EParametrosOperacion oParametroOperacion)
        {
            string strTrama = string.Empty;

            try
            {
                string valorParametro = oParametroOperacion.ValorParametro.Trim().ToUpper();
                int valorLongitud = Convert.ToInt32(oParametroOperacion.Longitud.Trim());
                int valorDecimal = Convert.ToInt32(oParametroOperacion.Decimal.Trim());
                int Longitudtotal = valorLongitud + valorDecimal;
                string tipoDato = oParametroOperacion.TipoDato.Trim();

                switch (tipoDato)
                {
                    case "System.String":
                        strTrama = valorParametro.Trim().PadRight(valorLongitud, ' ').Substring(0, valorLongitud);
                        break;

                    case "System.Double":

                        if (String.IsNullOrEmpty(valorParametro))
                        {
                            strTrama = Convert.ToString(System.Math.Round(Convert.ToDouble("0"), valorDecimal) * 100).PadLeft(Longitudtotal, '0');
                        }
                        else 
                        {
                            int indexComa = valorParametro.IndexOf(",");
                            if (indexComa >= 0)
                                valorParametro = valorParametro.Replace(",", ".");
                            strTrama = Convert.ToString(System.Math.Round(Convert.ToDouble(valorParametro), valorDecimal) * 100).PadLeft(Longitudtotal, '0');
                        }


                        //string[] datosParametro;
                        //int indexPunto = valorParametro.IndexOf(".");
                        //int indexComa = valorParametro.IndexOf(",");

                        //char[] separador = new char[] { ',', '.' };
                        //datosParametro = valorParametro.Split(separador);

                        //if ((indexPunto >= 0 && indexComa < 0) || (indexPunto < 0 && indexComa >= 0))
                        //{
                        //    if (datosParametro.Length > 0)
                        //    {
                        //        strTrama = datosParametro[0].Trim().PadLeft(valorLongitud, '0').Substring(0, valorLongitud);
                        //        strTrama = strTrama + datosParametro[1].Trim().PadRight(valorDecimal, '0').Substring(0, valorDecimal);
                        //    }
                        //}
                        //else
                        //{
                        //    if (datosParametro.Length > 0)
                        //    {
                        //        strTrama = datosParametro[0].Trim().PadLeft(valorLongitud, '0').Substring(0, valorLongitud);
                        //        strTrama = strTrama + string.Empty.PadLeft(valorDecimal, '0').Substring(0, valorDecimal);
                        //    }

                        //    //strTrama = valorParametro.PadLeft(valorLongitud + valorDecimal, '0').Substring(0, valorLongitud + valorDecimal);
                        //}

                        break;

                    case "System.Entero":
                        strTrama = valorParametro.PadRight(valorLongitud, '0').Substring(0, valorLongitud);
                        break;
                }


                return strTrama;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                strTrama = null;
            }
        }
    }
}
