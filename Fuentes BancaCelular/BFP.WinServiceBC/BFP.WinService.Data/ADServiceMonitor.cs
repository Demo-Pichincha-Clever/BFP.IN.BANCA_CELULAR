using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Configuration;

using BFP.WinService.Entidad;
using BFP.WinService.Comun;

namespace BFP.WinService.Data
{
    public class ADServiceMonitor : ADBase
    {
        private static ESalida<string> _oError = new ESalida<string>();

        /// <summary>
        /// Consulta las afiliaciones a la banca celular desde el IBS
        /// Consulta las desafiliaciones a la banca celular desde el IBS
        /// Realiza la actualización del campo "EnvioMensajeAfiliacion" y "EnvioMensajeDesafiliacion" 
        /// de las tabla de afiliacion y desafiliacion en el IBS
        /// </summary>
        /// <returns></returns>
        public static DataSet EjecutarTransaccion(string strNombreTransaccion,
                                    short shrLongitudCabecera, string strMensajeIn,
                                    string strNombreMensajeOut, int intPosicionInicialLecturaOut,
                                    out ESalida<string> oError)
        {
            Service objSixCommunication = new Service();

            DataSet dsSalida = null;
            string strResultado = string.Empty;

            try
            {
                strResultado = objSixCommunication.SendMessage(strNombreTransaccion,
                                                                    shrLongitudCabecera, strMensajeIn);


                _oError.Codigo = strResultado.Substring(17, 4);

                if (_oError.Codigo == CConstantes.CodigoMensajeRetorno.EXITO)
                {
                    if (strResultado.Substring(36, strResultado.Length - 36).Trim().Length > 0)
                    {
                        DataLoader xml = new DataLoader();
                        dsSalida = xml.ObtenerData(strResultado, strNombreMensajeOut, strNombreTransaccion,
                                                                    intPosicionInicialLecturaOut);
              
                    }

                    _oError.Codigo = CConstantes.CodigoMensajeRetorno.EXITO;
                    _oError.Mensaje = string.Empty;
                                        
                }
                else
                {
                    _oError.Mensaje = strResultado.Substring(36, strResultado.Length - 36);
                }

                oError = _oError;

                return dsSalida;
            }
            catch (Exception ex)
            {
                dsSalida = null;
                _oError.Codigo = CConstantes.CodigoMensajeRetorno.ERROR;
                _oError.Mensaje = ex.Message;
                oError = _oError;

                return dsSalida;
            }
            finally
            {
                objSixCommunication = null;
                dsSalida = null;
            }
        }



        public static void RegistrarLogMensajeria(List<ELogMensajeria> lstLogMensajeria)
        {
            object[] parametros;

            try
            {
                foreach (ELogMensajeria item in lstLogMensajeria)
                {
                    if (!String.IsNullOrEmpty(item.Nro_Celular_Envio))
                    {
                        parametros = new object[] { item.Ident_Mensaje, item.Ident_Canal, item.Ident_Tipo_Mensaje, 
                    item.Nro_Celular_Envio, item.Email_Envio, item.Codigo_Respuesta_YP, 
                    item.Fech_Envio_Mensaje, item.Fech_Respuesta_YP, item.Estado_Envio_Mensaje, item.Cuerpo_Mensaje,item.Tipo_Mensaje };

                        // grabar            
                        GetDatabase().ExecuteNonQuery(CConstantes.StoredProcedures.PRO_BC_RegistraLogMensajeria, parametros);
                    }
                }
            }
            catch (Exception ex)
            {
                parametros = null;
            }
            finally
            {
                parametros = null;
            }
        }
         
        public static void EnviarEmail(EMensajeria oMensajeria, string tipoEvento)
        {
            object[] parametros;

            try
            {
                parametros = new object[] { oMensajeria.Email, tipoEvento};

                // Ejecutar envio            
                GetDatabase().ExecuteNonQuery(CConstantes.StoredProcedures.PRO_BC_Envia_Correo, parametros);

            }
            catch (Exception)
            {
                parametros = null;
            }
            finally
            {
                parametros = null;
            }
        }


        //public static int ValidarEnvioEmail(int codigoEmail)
        //{
        //    object[] parametros;
        //    int estadoEnvio = -1;

        //    try
        //    {
        //        parametros = new object[] { codigoEmail, estadoEnvio };

        //        // Ejecutar envio            
        //        estadoEnvio = Convert.ToInt32(GetDatabase().ExecuteScalar(CConstantes.StoredProcedures.PRO_BC_Valida_Envio_Email, parametros));

        //        return estadoEnvio;
        //    }
        //    catch (Exception)
        //    {
        //        parametros = null;

        //        return estadoEnvio;
        //    }
        //    finally
        //    {
        //        parametros = null;
        //    }
        //}

        /// <summary>
        /// Realiza la actualizacion sobre los mensajes pendientes por SMS y Email de afiliaciones o desafiliaciones
        /// </summary>
        /// <param name="programaTransaccion">Nombre del programa que realizará el procedimiento de actualizacion</param>
        /// <param name="shrLongitudCabecera">Longitud de la cabecera que se espera de la trama</param>
        /// <param name="strMensajeIn">Trama a enviar al AS400</param>
        /// <param name="oError">Resultado de la invocacion al programa</param>
        /// 
        public static void EjecutarActualizacion(string strNombreTransaccion, short shrLongitudCabecera, 
            string strMensajeIn, out ESalida<string> oError)
        {
            Service objSixCommunication = new Service();

            string strResultado = string.Empty;

            try
            {
                strResultado = objSixCommunication.SendMessage(strNombreTransaccion,
                                                                    shrLongitudCabecera, strMensajeIn);
                _oError.Codigo = strResultado.Substring(17, 4);

                if (_oError.Codigo == CConstantes.CodigoMensajeRetorno.EXITO)
                {
                    _oError.Codigo = CConstantes.CodigoMensajeRetorno.EXITO;
                    _oError.Mensaje = string.Empty;
                }
                else
                {
                    _oError.Mensaje = strResultado.Substring(36, 4061);
                }

                oError = _oError;

            }
            catch (Exception ex)
            {
                _oError.Codigo = CConstantes.CodigoMensajeRetorno.ERROR;
                _oError.Mensaje = ex.Message;
                oError = _oError;
            }
            finally
            {
                objSixCommunication = null;
            }
        }

        public static DataSet ObtenerConfiguracionServicio()
        {
            object[] parametros;

            try
            {
                parametros = new object[] { "CONNOT" };
                
                return GetDatabase().ExecuteDataSet(CConstantes.StoredProcedures.PRO_BC_TablaConfiguracion, parametros);

            }
            catch (Exception ex)
            {
                File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + ex.Message + " \r\n");
                return null;
            }
            finally
            {
                parametros = null;
            }
        }

        public static bool ActualizarDatosConfiguracion(int intTablaId, int intCampoId ,string strCodigoCampo, string strValor) 
        {
            object[] parametros;
            try
            {
                parametros = new object[] { intTablaId, intCampoId, strCodigoCampo, strValor };

                GetDatabase().ExecuteNonQuery(CConstantes.StoredProcedures.PRO_BC_ActualizarTablaConfiguracion, parametros);
                return true;
            }
            catch (Exception ex)
            {
                File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + ex.Message + " \r\n");
                return false;
            }
            finally
            {
                parametros = null;
            }
        }

    }
}
