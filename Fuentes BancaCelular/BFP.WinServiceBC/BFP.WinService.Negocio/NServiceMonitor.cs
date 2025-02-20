using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.IO;

using BFP.WinService.Entidad;
using BFP.WinService.Comun;
using BFP.WinService.Data;

namespace BFP.WinService.Negocio
{
    public class NServiceMonitor
    {
        /// <summary>
        /// Realiza todo el procesamiento de envio de mensajeria
        /// </summary>
        /// 
        public static List<ELogMensajeria> ProcesaEnvioMensajeria()
        {
            List<ELogMensajeria> lstLogMensajeria = null;
            List<EMensajeria> lstMensajeriaAfiliacion = null;
            List<EMensajeria> lstMensajeriaDesafiliacion = null;
            List<EMensajesEnviados> lstMensajes_Enviados = null;
            ECatalogo<string, string> oTipoOperacion = new ECatalogo<string, string>();
            ELogMensajeria logMensajeriaSMS = null;
            ELogMensajeria logMensajeriaEmail = null;
            EMensajesEnviados oMensajeEnviados = null;

            int codigoApp = CConstantes.CodigoAplicativo.ServicioWinMensajeria;
            string usuario = ConfigurationManager.AppSettings["UsuarioAdiministrador"].ToString();

            try
            {
                lstLogMensajeria = new List<ELogMensajeria>();
                lstMensajeriaAfiliacion = new List<EMensajeria>();
                lstMensajeriaDesafiliacion = new List<EMensajeria>();

                lstMensajeriaAfiliacion = EjecutarConsulta(CConstantes.TipoEventoAfiliacion.AFILIACION);
                lstMensajeriaDesafiliacion = EjecutarConsulta(CConstantes.TipoEventoAfiliacion.DESAFILIACION);

                if (lstMensajeriaAfiliacion != null)
                {
                    if (lstMensajeriaAfiliacion.Count > 0)
                    {
                        lstMensajes_Enviados = new List<EMensajesEnviados>();

                        //Afiliaciones
                        foreach (EMensajeria item in lstMensajeriaAfiliacion)
                        {
                            bool envioEmail = false;
                            bool envioSMS = false;
                            logMensajeriaSMS = new ELogMensajeria();
                            logMensajeriaEmail = new ELogMensajeria();

                            if (!item.EnvioSMS)
                            {
                                logMensajeriaSMS = EnvioSMS(item, CConstantes.TipoEventoAfiliacion.AFILIACION, ref envioSMS);

                                if (logMensajeriaSMS != null)
                                    lstLogMensajeria.Add(logMensajeriaSMS);
                            }
                            else
                                envioSMS = true; // Mensaje ya enviado


                            if (!item.EnvioEmail)
                            {
                                logMensajeriaEmail = EnvioEmail(item, CConstantes.TipoEventoAfiliacion.AFILIACION, ref envioEmail);

                                if (logMensajeriaEmail != null)
                                    lstLogMensajeria.Add(logMensajeriaEmail);
                            }
                            else
                                envioEmail = true; // Mensaje ya enviado



                            oMensajeEnviados = new EMensajesEnviados();
                            oMensajeEnviados.NroCelular = item.NroCelular;
                            oMensajeEnviados.EnviaSMS = envioSMS;
                            oMensajeEnviados.EnviaCorreo = envioEmail;

                            lstMensajes_Enviados.Add(oMensajeEnviados);
                        }

                        if (lstMensajes_Enviados.Count > 0)
                            ActualizaMensajesEnviados(lstMensajes_Enviados, CConstantes.TipoEventoAfiliacion.AFILIACION);
                    }
                }

                if (lstMensajeriaDesafiliacion != null)
                {
                    if (lstMensajeriaDesafiliacion.Count > 0)
                    {
                        lstMensajes_Enviados = new List<EMensajesEnviados>();

                        //Desafiliaciones
                        foreach (EMensajeria item in lstMensajeriaDesafiliacion)
                        {
                            bool envioEmail = false;
                            bool envioSMS = false;
                            logMensajeriaSMS = new ELogMensajeria();
                            logMensajeriaEmail = new ELogMensajeria();

                            if (!item.EnvioSMS)
                            {
                                logMensajeriaSMS = EnvioSMS(item, CConstantes.TipoEventoAfiliacion.DESAFILIACION, ref envioSMS);

                                if (logMensajeriaSMS != null)
                                    lstLogMensajeria.Add(logMensajeriaSMS);
                            }
                            else
                                envioSMS = true; // Mensaje ya enviado


                            if (!item.EnvioEmail)
                            {
                                logMensajeriaEmail = EnvioEmail(item, CConstantes.TipoEventoAfiliacion.DESAFILIACION, ref envioEmail);

                                if (logMensajeriaEmail != null)
                                    lstLogMensajeria.Add(logMensajeriaEmail);
                            }
                            else
                                envioEmail = true; // Mensaje ya enviado


                            oMensajeEnviados = new EMensajesEnviados();
                            oMensajeEnviados.NroCelular = item.NroCelular;
                            oMensajeEnviados.EnviaSMS = envioSMS;
                            oMensajeEnviados.EnviaCorreo = envioEmail;

                            lstMensajes_Enviados.Add(oMensajeEnviados);
                        }

                        if (lstMensajes_Enviados.Count > 0)
                            ActualizaMensajesEnviados(lstMensajes_Enviados, CConstantes.TipoEventoAfiliacion.DESAFILIACION);
                    }
                }

                //Solo registra el log de mensajes SMS, los de email son registrados en el mismo store.
                if (lstLogMensajeria != null)
                {
                    if (lstLogMensajeria.Count > 0)
                        RegistrarLogMensajeria(lstLogMensajeria);
                }

                return lstLogMensajeria;
            }
            catch (Exception ex)
            {
                //Tipo de error
                oTipoOperacion.Valor = CConstantes.CodigoMensajeRetorno.ERROR;
                oTipoOperacion.Nombre = CConstantes.MensajesError.ErrorProcesamientoMensajeria;

                //Inserta Error
                NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                        "NServiceMonitor.cs : ProcesaEnvioMensajeria", string.Empty, ex.Message,
                        DateTime.Now, codigoApp);

                return lstLogMensajeria;
            }
            finally
            {
                lstMensajeriaAfiliacion = null;
                lstMensajeriaDesafiliacion = null;
                lstLogMensajeria = null;
                lstMensajes_Enviados = null;
                oTipoOperacion = null;
                logMensajeriaSMS = null;
                logMensajeriaEmail = null;
                oMensajeEnviados = null;
            }
        }

        private static void ActualizaMensajesEnviados(List<EMensajesEnviados> lstMensajes_Enviados, string tipoEvento)
        {
            string tramaEnvio = string.Empty;
            string trama = string.Empty;
            StringBuilder tramaMensajes = new StringBuilder();
            ESalida<string> respuesta = new ESalida<string>();
            ECatalogo<string, string> oTipoOperacion = new ECatalogo<string, string>();

            int codigoApp = CConstantes.CodigoAplicativo.ServicioWinMensajeria;
            string usuario = ConfigurationManager.AppSettings["UsuarioAdiministrador"].ToString();
            string programaTransaccion = CConstantes.Transacciones.Programa_Actualiza_Mensajes();

            try
            {
                foreach (EMensajesEnviados item in lstMensajes_Enviados)
                {
                    trama = item.NroCelular.PadRight(15, ' ').Substring(0, 15) +
                        Convert.ToInt32(item.EnviaSMS).ToString().PadLeft(1, '0') +
                        Convert.ToInt32(item.EnviaCorreo).ToString().PadLeft(1, '0');

                    tramaMensajes.Append(trama);
                }

                tramaEnvio = tipoEvento + tramaMensajes.ToString();

                // Ejecutar la actualizacion
                int long_transaccion = CConstantes.Transacciones.LONG_TRANS_ACTUALIZA_MENSAJERIA;

                ADServiceMonitor.EjecutarActualizacion(programaTransaccion, (short)long_transaccion, tramaEnvio, out respuesta);

                if (!respuesta.Codigo.Equals(CConstantes.CodigoMensajeRetorno.EXITO))
                {
                    oTipoOperacion.Valor = programaTransaccion;

                    if (tipoEvento == CConstantes.TipoEventoAfiliacion.AFILIACION)
                        oTipoOperacion.Nombre = CConstantes.Transacciones.ActualizaAfiliacion;
                    else if (tipoEvento == CConstantes.TipoEventoAfiliacion.DESAFILIACION)
                        oTipoOperacion.Nombre = CConstantes.Transacciones.ActualizaDesafiliacion;

                    //Inserta Error
                    NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                            "NServiceMonitor.cs : ActualizaMensajesEnviados", tramaEnvio, respuesta.Mensaje,
                            DateTime.Now, codigoApp);
                }
            }
            catch (Exception ex)
            {
                oTipoOperacion.Valor = programaTransaccion;

                if (tipoEvento == CConstantes.TipoEventoAfiliacion.AFILIACION)
                    oTipoOperacion.Nombre = CConstantes.Transacciones.ActualizaAfiliacion;
                else if (tipoEvento == CConstantes.TipoEventoAfiliacion.DESAFILIACION)
                    oTipoOperacion.Nombre = CConstantes.Transacciones.ActualizaDesafiliacion;

                //Inserta Error
                NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                        "NServiceMonitor.cs : ActualizaMensajesEnviados", tramaEnvio, ex.Message,
                        DateTime.Now, codigoApp);

                tramaMensajes = null;
            }
            finally
            {
                respuesta = null;
                tramaMensajes = null;
                oTipoOperacion = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="p"></param>
        private static ELogMensajeria EnvioEmail(EMensajeria oMensajeria, string tipoEvento, ref bool enviado)
        {
            int codigoApp = CConstantes.CodigoAplicativo.ServicioWinMensajeria;
            string descError = string.Empty;
            DateTime fechaEnvio;
            EMensajeOperacion EMensaje = new EMensajeOperacion();
            ELogMensajeria logMensajeria = new ELogMensajeria();
            string usuario = ConfigurationManager.AppSettings["UsuarioAdiministrador"].ToString();
            ECatalogo<string, string> oTipoOperacion = new ECatalogo<string, string>();

            try
            {
                bool formatoValido = ValidarFormatoEmail(oMensajeria.Email);

                if (formatoValido)
                {
                    //Enviando E-mail
                    ADServiceMonitor.EnviarEmail(oMensajeria, tipoEvento);
                    fechaEnvio = DateTime.Now;
                    enviado = true;

                    if (tipoEvento == CConstantes.TipoEventoAfiliacion.AFILIACION)
                        EMensaje = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.SMS_AFILIACION, string.Empty);
                    else if (tipoEvento == CConstantes.TipoEventoAfiliacion.DESAFILIACION)
                        EMensaje = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.SMS_DESAFILIACION, string.Empty);

                    logMensajeria = new ELogMensajeria();
                    logMensajeria.Ident_Mensaje = EMensaje.IdMensaje;
                    logMensajeria.Ident_Tipo_Mensaje = CConstantes.TipoMensaje.EMAIL;
                    logMensajeria.Ident_Canal = oMensajeria.Canal.Trim();
                    logMensajeria.Nro_Celular_Envio = oMensajeria.NroCelular.Trim();
                    logMensajeria.Email_Envio = oMensajeria.Email.Trim();
                    logMensajeria.Fech_Envio_Mensaje = DateTime.Now;
                    logMensajeria.Fech_Respuesta_YP = null;
                    logMensajeria.Codigo_Respuesta_YP = null;
                    logMensajeria.Estado_Envio_Mensaje = enviado;
                }
                else
                {
                    enviado = false;
                    descError = CConstantes.MensajesError.ERROR_FORMATO_EMAIL;

                    //Tipo de error
                    oTipoOperacion.Valor = CConstantes.CodigoMensajeRetorno.ERROR;
                    oTipoOperacion.Nombre = CConstantes.MensajesError.ErrorEnvioEmail;

                    //Inserta Error
                    NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                            "NServiceMonitor.cs : EnvioEmail", oMensajeria.Email, descError,
                            DateTime.Now, codigoApp);
                }

                return logMensajeria;
            }
            catch (Exception ex)
            {
                //Tipo de error
                oTipoOperacion.Valor = CConstantes.CodigoMensajeRetorno.ERROR;
                oTipoOperacion.Nombre = CConstantes.MensajesError.ErrorEnvioEmail;

                //Inserta Error
                NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                        "NServiceMonitor.cs : EnvioEmail", oMensajeria.Email, ex.Message,
                        DateTime.Now, codigoApp);

                return logMensajeria = null;
            }
            finally
            {
                logMensajeria = null;
                EMensaje = null;
                oTipoOperacion = null;
            }
        }

        //Valida el formato del email
        private static bool ValidarFormatoEmail(string email)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(email))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Genera el envio de SMS
        /// </summary>
        /// <param name="mensajeria">Objeto que tiene los datos de envio</param>
        /// <param name="tipoEvento">Indica si se trata de una afiliacion o desafiliacion</param>
        /// <returns>Log del envio SMS</returns>
        /// 
        private static ELogMensajeria EnvioSMS(EMensajeria oMensajeria, string tipoEvento, ref bool estadoEnvio)
        {
            ServiceSMS serviceSMS = new ServiceSMS();
            EItemsParametros[] serial_YP = null;
            EItemsParametros[] pin_YP = null;
            EItemsParametros[] codigoCorto = null;
            EMensajeOperacion EMensaje = null;
            ELogMensajeria logMensajeria = null;
            ECatalogo<string, string> oTipoOperacion = new ECatalogo<string, string>();

            string codigoRespuestaYP = string.Empty;
            int codigoApp = CConstantes.CodigoAplicativo.ServicioWinMensajeria;
            string usuario = ConfigurationManager.AppSettings["UsuarioAdiministrador"].ToString();

            try
            {
                if (tipoEvento == CConstantes.TipoEventoAfiliacion.AFILIACION)
                    EMensaje = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.SMS_AFILIACION, string.Empty);
                else if (tipoEvento == CConstantes.TipoEventoAfiliacion.DESAFILIACION)
                    EMensaje = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.SMS_DESAFILIACION, string.Empty);

                serial_YP = ADTablaMaestra.ObtenerItemsParametros(CConstantes.TablaMaestras.SERIAL_SERVICIO_YP);
                pin_YP = ADTablaMaestra.ObtenerItemsParametros(CConstantes.TablaMaestras.PIN_SERVICIO_YP);
                codigoCorto = ADTablaMaestra.ObtenerItemsParametros(CConstantes.TablaMaestras.CODIGO_CORTO);

                //Obteniendo el codigo corto de la operadora
                string codigoCortoOperadora = (from c in codigoCorto
                                               where c.IdItemTabla == Convert.ToInt32(oMensajeria.CodigoOperadora)
                                               && c.IdTabla == CConstantes.TablaMaestras.CODIGO_CORTO.Trim()
                                               select c.ValItemTabla).First();

                logMensajeria = new ELogMensajeria();
                logMensajeria.Ident_Mensaje = EMensaje.IdMensaje;
                logMensajeria.Ident_Tipo_Mensaje = CConstantes.TipoMensaje.SMS;
                logMensajeria.Ident_Canal = oMensajeria.Canal.Trim();
                logMensajeria.Nro_Celular_Envio = oMensajeria.NroCelular.Trim();
                logMensajeria.Email_Envio = null;
                logMensajeria.Fech_Envio_Mensaje = DateTime.Now;

                //Enviando mensaje SMS
                codigoRespuestaYP = serviceSMS.EnviaSMS(serial_YP[0].ValItemTabla.Trim(), pin_YP[0].ValItemTabla.Trim(), CConstantes.TablaMaestras.DISCADO_DIRECTO_INTERNACIONAL + oMensajeria.NroCelular.Trim(),
                    EMensaje.DescMensaje.Trim(), codigoCortoOperadora.Trim());
                //Fin Envio mensaje SMS

                logMensajeria.Fech_Respuesta_YP = DateTime.Now;
                logMensajeria.Codigo_Respuesta_YP = codigoRespuestaYP.Trim();

                if (logMensajeria.Codigo_Respuesta_YP.Length.CompareTo(16) == 0)
                    logMensajeria.Estado_Envio_Mensaje = true;
                else
                {
                    logMensajeria.Estado_Envio_Mensaje = false;
                    string descError = ADMensajeOperacion.ObtenerMensajeError_ServicioSMS(codigoRespuestaYP);

                    //Tipo de error
                    oTipoOperacion.Valor = CConstantes.CodigoMensajeRetorno.ERROR;
                    oTipoOperacion.Nombre = CConstantes.MensajesError.Error_MensajeSMS_YP;

                    //Inserta Error
                    NLog.Insertar(usuario, null, CConstantes.EstadosLog.ERROR,
                            "NServiceMonitor.cs : EnvioSMS", codigoRespuestaYP, descError,
                            DateTime.Now, codigoApp);
                }

                estadoEnvio = logMensajeria.Estado_Envio_Mensaje;

                return logMensajeria;
            }
            catch (Exception ex)
            {
                //Tipo de error
                oTipoOperacion.Valor = CConstantes.CodigoMensajeRetorno.ERROR;
                oTipoOperacion.Nombre = CConstantes.MensajesError.ErrorEnvioSMS;

                //Inserta Error
                NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                        "NServiceMonitor.cs : EnvioSMS", oMensajeria.NroCelular, ex.Message,
                        DateTime.Now, codigoApp);

                return logMensajeria = null;
            }
            finally
            {
                serviceSMS = null;
                serial_YP = null;
                pin_YP = null;
                codigoCorto = null;
                EMensaje = null;
                logMensajeria = null;
                oTipoOperacion = null;
            }
        }

        /// <summary>
        /// Registra los mensajes enviados SMS y Correo Electronico
        /// </summary>
        /// <param name="lstLogMensajeria"></param>
        public static void RegistrarLogMensajeria(List<ELogMensajeria> lstLogMensajeria)
        {
            ADServiceMonitor.RegistrarLogMensajeria(lstLogMensajeria);
        }

        /// <summary>
        /// Retorna un objeto de tipo lista que contiene los celulares y email 
        /// de los clientes afiliados por IBS, con el fin de enviarles el mensaje de bienvenida 
        /// por SMS y correo electronico
        /// </summary>
        /// <param name="oTrama">Trama requerida para ejecutar la consulta de afiliaciones o desafiliaciones</param>
        /// <param name="tipoEvento">Indica el tipo de consulta: afiliación o desafiliación</param>
        /// <returns>Retorna la lista de correos y celulares pendientes de envio SMS y correo respectivamente</returns>
        /// 
        public static List<EMensajeria> EjecutarConsulta(string tipoEvento)
        {
            EMensajeria oResultado = null;
            List<EMensajeria> lstResultado = new List<EMensajeria>(); ;
            ESalida<string> respuesta = new ESalida<string>();
            int codigoApp = CConstantes.CodigoAplicativo.ServicioWinMensajeria;
            string mensajeError = string.Empty;
            string usuario = ConfigurationManager.AppSettings["UsuarioAdiministrador"].ToString();
            ECatalogo<string, string> oTipoOperacion = new ECatalogo<string, string>();

            string programaTransaccion = CConstantes.Transacciones.Programa_Consulta_Mensajes_Pendientes();

            try
            {
                int long_transaccion = CConstantes.Transacciones.LONG_TRANS_CONSULTA_MENSAJERIA;

                DataSet dsResultado = ADServiceMonitor.EjecutarTransaccion(programaTransaccion,
                                        ((short)long_transaccion), tipoEvento, CConstantes.Transacciones.NombreMensajeOut,
                                        CConstantes.Transacciones.PosicionInicialCorte, out respuesta);

                if (respuesta.Codigo.Equals(CConstantes.CodigoMensajeRetorno.EXITO))
                {
                    if (dsResultado != null)
                    {
                        if (dsResultado.Tables["ODatosAD"].Rows.Count > 0)
                        {
                            foreach (DataRow dr in dsResultado.Tables["ODatosAD"].Rows)
                            {
                                oResultado = new EMensajeria();
                                oResultado.NroCelular = dr["ODBcfNce"].ToString().Trim();
                                oResultado.Email = dr["ODBcfEml"].ToString().Trim();
                                oResultado.Canal = dr["ODCanal"].ToString().Trim();
                                oResultado.EnvioSMS = Convert.ToBoolean(Convert.ToInt32(dr["ODEnvSms"].ToString().Trim()));
                                oResultado.EnvioEmail = Convert.ToBoolean(Convert.ToInt32(dr["ODEnvEml"].ToString().Trim()));
                                oResultado.CodigoOperadora = dr["ODOperad"].ToString().Trim();
                                lstResultado.Add(oResultado);
                            }
                        }
                        //else
                        //{
                        //    oTipoOperacion.Valor = programaTransaccion;

                        //    if (tipoEvento == CConstantes.TipoEventoAfiliacion.AFILIACION)
                        //    {
                        //        oTipoOperacion.Nombre = CConstantes.Transacciones.ConsultaAfiliacion;
                        //        mensajeError = CConstantes.MensajesError.INFORMACION_AFILIACION;
                        //    }
                        //    else if (tipoEvento == CConstantes.TipoEventoAfiliacion.DESAFILIACION)
                        //    {
                        //        oTipoOperacion.Nombre = CConstantes.Transacciones.ConsultaDesafiliacion;
                        //        mensajeError = CConstantes.MensajesError.INFORMACION_DESAFILIACION;
                        //    }

                        //    NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.INFORMACION,
                        //            "NServiceMonitor.cs : EjecutarConsulta", tipoEvento, mensajeError,
                        //            DateTime.Now, codigoApp);

                        //}
                    }
                    //else
                    //{
                    //    oTipoOperacion.Valor = programaTransaccion;

                    //    if (tipoEvento == CConstantes.TipoEventoAfiliacion.AFILIACION)
                    //    {
                    //        oTipoOperacion.Nombre = CConstantes.Transacciones.ConsultaAfiliacion;
                    //        mensajeError = CConstantes.MensajesError.INFORMACION_AFILIACION;
                    //    }
                    //    else if (tipoEvento == CConstantes.TipoEventoAfiliacion.DESAFILIACION)
                    //    {
                    //        oTipoOperacion.Nombre = CConstantes.Transacciones.ConsultaDesafiliacion;
                    //        mensajeError = CConstantes.MensajesError.INFORMACION_DESAFILIACION;
                    //    }

                    //    NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.INFORMACION,
                    //            "NServiceMonitor.cs : EjecutarConsulta", tipoEvento, mensajeError,
                    //            DateTime.Now, codigoApp);

                    //    //if (tipoEvento == CConstantes.TipoEventoAfiliacion.AFILIACION)
                    //    //{
                    //    //    oTipoOperacion.Nombre = CConstantes.Transacciones.ConsultaAfiliacion;
                    //    //    mensajeError = CConstantes.MensajesError.ERROR_TRX_CONSULTA_AFILIACION;
                    //    //}
                    //    //else if (tipoEvento == CConstantes.TipoEventoAfiliacion.DESAFILIACION)
                    //    //{
                    //    //    oTipoOperacion.Nombre = CConstantes.Transacciones.ConsultaDesafiliacion;
                    //    //    mensajeError = CConstantes.MensajesError.ERROR_TRX_CONSULTA_DESAFILIACION;
                    //    //}

                    //    ////Inserta Error
                    //    //NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                    //    //        "NServiceMonitor.cs : EjecutarConsulta", tipoEvento, mensajeError,
                    //    //        DateTime.Now, codigoApp);
                    //}
                }
                else
                {
                    oTipoOperacion.Valor = programaTransaccion;

                    if (tipoEvento == CConstantes.TipoEventoAfiliacion.AFILIACION)
                        oTipoOperacion.Nombre = CConstantes.Transacciones.ConsultaAfiliacion;
                    else if (tipoEvento == CConstantes.TipoEventoAfiliacion.DESAFILIACION)
                        oTipoOperacion.Nombre = CConstantes.Transacciones.ConsultaDesafiliacion;

                    if (respuesta.Mensaje.Trim().Length == 0)
                    {
                        if (tipoEvento == CConstantes.TipoEventoAfiliacion.AFILIACION)
                            respuesta.Mensaje = CConstantes.MensajesError.ERROR_TRX_CONSULTA_AFILIACION;
                        else if (tipoEvento == CConstantes.TipoEventoAfiliacion.DESAFILIACION)
                            respuesta.Mensaje = CConstantes.MensajesError.ERROR_TRX_CONSULTA_DESAFILIACION;
                    }

                    //Inserta Error
                    NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                            "NServiceMonitor.cs : EjecutarConsulta", tipoEvento, respuesta.Mensaje,
                            DateTime.Now, codigoApp);
                }

                return lstResultado;
            }

            catch (Exception ex)
            {
                oTipoOperacion.Valor = programaTransaccion;

                if (tipoEvento == CConstantes.TipoEventoAfiliacion.AFILIACION)
                    oTipoOperacion.Nombre = CConstantes.Transacciones.ConsultaAfiliacion;
                else if (tipoEvento == CConstantes.TipoEventoAfiliacion.DESAFILIACION)
                    oTipoOperacion.Nombre = CConstantes.Transacciones.ConsultaDesafiliacion;


                //Inserta Error
                NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                        "NServiceMonitor.cs : EjecutarConsulta", tipoEvento, ex.Message,
                        DateTime.Now, codigoApp);


                return lstResultado;
            }
            finally
            {
                lstResultado = null;
                oResultado = null;
                respuesta = null;
                oTipoOperacion = null;
            }
        }


        public static CCConfiguracion ObtenerConfiguracion()
        {
            CCConfiguracion Objconfiguracion = null;
            try
            {
                using (DataSet dsData = ADServiceMonitor.ObtenerConfiguracionServicio())
                {
                    if (dsData != null)
                    {
                        Objconfiguracion = new CCConfiguracion(dsData);
                        if (Objconfiguracion != null)
                            ActualizarDatosConfiguracion(Objconfiguracion.ServicioId, Objconfiguracion.StartId, Objconfiguracion.Servicio, DateTime.Now.ToShortDateString());
                    }

                }
                return Objconfiguracion;
            }
            catch (Exception ex)
            {
                Objconfiguracion = null;
                File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "excepcion NServiceMonitor" + ex.Message + " \r\n");
                return Objconfiguracion;
            }
            finally { Objconfiguracion = null; }
        }

        public static CCConfiguracion DefaultConfig()
        {
            CCConfiguracion Objconfiguracion = null;
            try
            {

                using (DataSet dsData = new DataSet())
                {
                    dsData.ReadXml(string.Concat(ConfigurationManager.AppSettings["XMLPATH"].ToString(), "DefaultConfig.xml"));
                    if (dsData != null)
                    {
                        Objconfiguracion = new CCConfiguracion(dsData);
                        if (Objconfiguracion != null)
                            ActualizarDatosConfiguracion(Objconfiguracion.ServicioId, Objconfiguracion.StartId, Objconfiguracion.Servicio, DateTime.Now.ToShortDateString());
                    }

                }
                return Objconfiguracion;
            }
            catch (Exception ex)
            {
                Objconfiguracion = null;
                return Objconfiguracion;
            }
            finally { Objconfiguracion = null; }
        }

        public static void ActualizarDatosConfiguracion(int intTablaId, int intCampoId, string strCodigoCampo, string strValor)
        {
            try
            {
                if (ADServiceMonitor.ActualizarDatosConfiguracion(intTablaId, intCampoId, strCodigoCampo, strValor))
                {
                    File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), string.Concat(DateTime.Now.ToString(), "Actualizo", intTablaId.ToString(), "--", intCampoId.ToString(), "--", strCodigoCampo.ToString(), "--", strValor.ToString(), " \r\n"));
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "excepcion ActualizarDatosConfiguracion - NServiceMonitor");
            }
            //finally { Objconfiguracion = null; }
        }

        //public static bool ProcesarEnvioNotificaciones()
        //{
        //    return true;
        //}

        public static bool ProcesarPreConsultaNotificaciones(string strFecha, string strHora)
        {
            int intNroRegistros = 0;
            int intResult = 0;
            int intNroTotalRegistros = 0;
            string strTramaCondulta = string.Empty;
            List<EMensajeria> lstResultado = new List<EMensajeria>();
            int codigoApp = CConstantes.CodigoAplicativo.ServicioWinMensajeria;
            string usuario = ConfigurationManager.AppSettings["UsuarioAdiministrador"].ToString();
            //ServiceSMS serviceSMS = null; // new ServiceSMS();
            ENotificaciones ObjMensaje = null; //new ENotificaciones();
            ESalida<string> respuesta = new ESalida<string>();
            ECatalogo<string, string> oTipoOperacion = new ECatalogo<string, string>();
            try
            {
                while (intNroRegistros <= intNroTotalRegistros)
                {
                    strTramaCondulta = string.Concat("001", strFecha, strHora, intNroRegistros.ToString().PadLeft(2, '0'));
                    int long_transaccion = CConstantes.Transacciones.LONG_TRANS_CONSULTA_MENSAJERIA + strTramaCondulta.Length;
                    string programaTransaccion = CConstantes.Transacciones.Programa_Consulta_Notificaciones();
                    using (DataSet dsResultado = ADServiceMonitor.EjecutarTransaccion(programaTransaccion,
                                            ((short)long_transaccion), strTramaCondulta, CConstantes.Transacciones.NombreMensajeOut,
                                            CConstantes.Transacciones.PosicionInicialCorte, out respuesta))
                    {
                        if (int.TryParse(respuesta.Codigo, out intResult))
                        {
                            if (dsResultado != null && dsResultado.Tables["ODprocess"].Rows.Count > 0)
                            {
                                intNroTotalRegistros = int.Parse(dsResultado.Tables["OData"].Rows[0]["ODtotreg"].ToString());
                                intNroRegistros = intNroRegistros + int.Parse(dsResultado.Tables["OData"].Rows[0]["ODnroope"].ToString());
                                if (!dsResultado.Tables["ODprocess"].Columns.Contains("ODRespuestaYP"))
                                    dsResultado.Tables["ODprocess"].Columns.Add("ODRespuestaYP");
                                Random objrandow = new Random();
                                string s = ConfigurationManager.AppSettings["CelTest"].ToString();
                                string[] ListaCel = ConfigurationManager.AppSettings["CelTest"].ToString().Split('|');
                                int intIndicadorEnviados = 0;
                                foreach (DataRow item in dsResultado.Tables["ODprocess"].Rows)
                                {
                                    if (item["ODNroCel"].ToString().Trim().Length == 9 && item["ODNroCel"].ToString() != string.Empty)
                                    {
                                        bool booEstado = false;
                                        //serviceSMS = new ServiceSMS();
                                        ObjMensaje = new ENotificaciones();
                                        ObjMensaje.MensajeId = item["ODNroId"].ToString();
                                        ObjMensaje.Canal = "BC";
                                        ObjMensaje.CodigoOperadora = item["ODOpeCel"].ToString();
                                        ObjMensaje.EnvioSMS = true;
                                        ObjMensaje.EnvioEmail = false;
                                        if (ListaCel.Length > 1)
                                            ObjMensaje.NroCelular = ListaCel[objrandow.Next(0, ListaCel.Length)]; //item["ODNroCel"].ToString().Trim();
                                        else
                                            ObjMensaje.NroCelular = item["ODNroCel"].ToString().Trim();

                                        ObjMensaje.Mensaje = item["ODTxtsms"].ToString().Trim();
                                        ELogMensajeria Obj = EnvioSMSNotificaciones(ObjMensaje, ref booEstado);
                                        if (booEstado)
                                        {
                                            intIndicadorEnviados++;
                                            item["ODRespuestaYP"] = Obj.Codigo_Respuesta_YP;
                                        }
                                    }
                                }

                                string strTramaActualizacion = string.Concat("002", strFecha, strHora, intIndicadorEnviados.ToString().PadLeft(2, '0'));
                                foreach (DataRow item in dsResultado.Tables["ODprocess"].Rows)
                                {
                                    if (item["ODRespuestaYP"].ToString() != string.Empty)
                                    {
                                        strTramaActualizacion = string.Concat(strTramaActualizacion, item["ODNroId"].ToString().PadLeft(10, '0'), item["ODRespuestaYP"].ToString().PadLeft(20, ' ')); //item["ODRespuestaYP"] = objrandow.Next(1, 99999999);
                                    }
                                }
                                if (intIndicadorEnviados > 0)
                                {
                                    strTramaCondulta = strTramaActualizacion;
                                    long_transaccion = CConstantes.Transacciones.LONG_TRANS_CONSULTA_MENSAJERIA + strTramaCondulta.Length;
                                    programaTransaccion = CConstantes.Transacciones.Programa_Consulta_Notificaciones();
                                    using (DataSet dsResultActualiza = ADServiceMonitor.EjecutarTransaccion(programaTransaccion,
                                                            ((short)long_transaccion), strTramaCondulta, CConstantes.Transacciones.NombreMensajeOut,
                                                            CConstantes.Transacciones.PosicionInicialCorte, out respuesta))
                                    {
                                        if (int.TryParse(respuesta.Codigo, out intResult))
                                        {
                                            if (dsResultActualiza != null)
                                            {
                                                //intNroTotalRegistros = int.Parse(dsResultado.Tables["OData"].Rows[0]["ODtotreg"].ToString());
                                                //intNroRegistros = intNroRegistros + int.Parse(dsResultado.Tables["OData"].Rows[0]["ODnroope"].ToString());
                                            }
                                        }
                                        else { break; }
                                    }
                                }

                            }
                            else { break; }
                        }
                        else
                        {
                            oTipoOperacion.Valor = CConstantes.CodigoMensajeRetorno.ERROR;
                            oTipoOperacion.Nombre = CConstantes.MensajesError.ErrorEnvioSMS;
                            //Inserta Error
                            NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                                    "NServiceMonitor.cs : EnvioSMSNotificaciones", strTramaCondulta, "Codigo Error Desconocido :" + respuesta,
                                    DateTime.Now, codigoApp);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Tipo de error
                oTipoOperacion.Valor = CConstantes.CodigoMensajeRetorno.ERROR;
                oTipoOperacion.Nombre = CConstantes.MensajesError.ErrorEnvioSMS;
                //Inserta Error
                NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                        "NServiceMonitor.cs : EnvioSMSNotificaciones", strTramaCondulta, ex.Message,
                        DateTime.Now, codigoApp);

            }


            return true;
        }


        /// <summary>
        /// Genera el envio de SMS
        /// </summary>
        /// <param name="mensajeria">Objeto que tiene los datos de envio</param>
        /// <param name="tipoEvento">Indica si se trata de una afiliacion o desafiliacion</param>
        /// <returns>Log del envio SMS</returns>
        /// 
        private static ELogMensajeria EnvioSMSNotificaciones(ENotificaciones oMensajeria, ref bool estadoEnvio)
        {
            ServiceSMS serviceSMS = new ServiceSMS();
            EItemsParametros[] serial_YP = null;
            EItemsParametros[] pin_YP = null;
            EItemsParametros[] codigoCorto = null;
            List<ELogMensajeria> ObjMensajesLog = null;
            //EMensajeOperacion EMensaje = null;
            ELogMensajeria logMensajeria = null;
            ECatalogo<string, string> oTipoOperacion = new ECatalogo<string, string>();

            string codigoRespuestaYP = string.Empty;
            int codigoApp = CConstantes.CodigoAplicativo.ServicioWinMensajeria;
            string usuario = ConfigurationManager.AppSettings["UsuarioAdiministrador"].ToString();

            try
            {

                serial_YP = ADTablaMaestra.ObtenerItemsParametros(CConstantes.TablaMaestras.SERIAL_SERVICIO_YP);
                pin_YP = ADTablaMaestra.ObtenerItemsParametros(CConstantes.TablaMaestras.PIN_SERVICIO_YP);
                codigoCorto = ADTablaMaestra.ObtenerItemsParametros(CConstantes.TablaMaestras.CODIGO_CORTO);

                string codigoCortoOperadora = (from c in codigoCorto
                                               where c.IdItemTabla == Convert.ToInt32(oMensajeria.CodigoOperadora)
                                               && c.IdTabla == CConstantes.TablaMaestras.CODIGO_CORTO.Trim()
                                               select c.ValItemTabla).First();

                logMensajeria = new ELogMensajeria();
                logMensajeria.Ident_Mensaje = "0065"; //oMensajeria.MensajeId;
                logMensajeria.Ident_Tipo_Mensaje = CConstantes.TipoMensaje.SMS;
                logMensajeria.Ident_Canal = oMensajeria.Canal.Trim();
                logMensajeria.Nro_Celular_Envio = oMensajeria.NroCelular.Trim();
                logMensajeria.Email_Envio = null;
                logMensajeria.Fech_Envio_Mensaje = DateTime.Now;
                logMensajeria.Cuerpo_Mensaje = oMensajeria.Mensaje.Trim();
                logMensajeria.Tipo_Mensaje = int.Parse(oMensajeria.MensajeId);

                //Enviando mensaje SMS
                codigoRespuestaYP = serviceSMS.EnviaSMS(serial_YP[0].ValItemTabla.Trim(), pin_YP[0].ValItemTabla.Trim(), CConstantes.TablaMaestras.DISCADO_DIRECTO_INTERNACIONAL + oMensajeria.NroCelular.Trim(),
                    oMensajeria.Mensaje.Trim(), codigoCortoOperadora.Trim());
                //Fin Envio mensaje SMS

                logMensajeria.Fech_Respuesta_YP = DateTime.Now;
                logMensajeria.Codigo_Respuesta_YP = codigoRespuestaYP.Trim();

                if (logMensajeria.Codigo_Respuesta_YP.Length.CompareTo(16) == 0)
                {
                    logMensajeria.Estado_Envio_Mensaje = true;
                    ObjMensajesLog = new List<ELogMensajeria>();
                    ObjMensajesLog.Add(logMensajeria);
                    ADServiceMonitor.RegistrarLogMensajeria(ObjMensajesLog);
                    ObjMensajesLog = null;
                }
                else
                {
                    logMensajeria.Estado_Envio_Mensaje = false;
                    string descError = ADMensajeOperacion.ObtenerMensajeError_ServicioSMS(codigoRespuestaYP);
                    //Tipo de error
                    oTipoOperacion.Valor = CConstantes.CodigoMensajeRetorno.ERROR;
                    oTipoOperacion.Nombre = CConstantes.MensajesError.Error_MensajeSMS_YP;
                    //Inserta Error
                    NLog.Insertar(usuario, null, CConstantes.EstadosLog.ERROR,
                            "NServiceMonitor.cs : EnvioSMS", codigoRespuestaYP, descError,
                            DateTime.Now, codigoApp);
                }
                estadoEnvio = logMensajeria.Estado_Envio_Mensaje;
                return logMensajeria;
            }
            catch (Exception ex)
            {
                //Tipo de error
                oTipoOperacion.Valor = CConstantes.CodigoMensajeRetorno.ERROR;
                oTipoOperacion.Nombre = CConstantes.MensajesError.ErrorEnvioSMS;

                //Inserta Error
                NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                        "NServiceMonitor.cs : EnvioSMS", oMensajeria.NroCelular, ex.Message,
                        DateTime.Now, codigoApp);

                return logMensajeria = null;
            }
            finally
            {
                serviceSMS = null;
                serial_YP = null;
                pin_YP = null;
                codigoCorto = null;
                //EMensaje = null;
                logMensajeria = null;
                oTipoOperacion = null;
            }
        }


    }
}
