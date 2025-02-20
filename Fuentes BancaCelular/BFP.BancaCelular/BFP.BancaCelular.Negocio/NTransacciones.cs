using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;

using BFP.BancaCelular.Entidad;
using BFP.BancaCelular.Data;
using BFP.BancaCelular.Comun;


namespace BFP.BancaCelular.Negocio
{
    public static class NTransacciones
    {
        /// <summary>
        /// Realiza toda la logica de procesamiento para las operaciones de la banca celular.
        /// </summary>
        /// <param name="oTrama">Trama de datos ingresados por el cliente</param>
        /// <param name="oDatosOperacion">Objeto con los datos fijos de la operacion </param>
        /// <returns>Retorna el mensaje a MT a devolver a YP</returns>
        /// 
        public static EResultadoMensajeMT EjecutaTransaccionOperacion(ETrama oTrama, EDatosOperacion oDatosOperacion)
        {
            EMensajeOperacion oMensajeOperacion, oMensaje;
            EResultadoMensajeMT oResultado = new EResultadoMensajeMT();
            ESalida<string> respuesta = new ESalida<string>();
            ECatalogo<string, string> oTipoOperacion;
            int indMaxMT = -1;
            string usuario = ConfigurationManager.AppSettings["UsuarioAdiministrador"].ToString();
            int codigoApp = CConstantes.CodigoAplicativo.ServicioWeb;

            try
            {
                int longitud = CConstantes.Transacciones.LONGITUD_OPERACION;

                DataSet dsResultado = ADTransaccion.EjecutarTransaccion(oDatosOperacion.ProgramaAS400,
                                        ((short)longitud), oTrama.Entrada, CConstantes.Transacciones.NombreMensajeOut,
                                        CConstantes.Transacciones.PosicionInicialCorte, out respuesta);

                oResultado.CodigoError = respuesta.Codigo;

                if (respuesta.Codigo.Equals(CConstantes.CodigoMensajeRetorno.EXITO))
                {
                    if (dsResultado != null)
                    {
                        if (dsResultado.Tables[0].Rows.Count > 0)
                        {
                            oTipoOperacion = CCUtil.ObtenerOperacion(oDatosOperacion.IdOperacion);

                            oResultado.CodRet = respuesta.Codigo;

                            if (dsResultado.Tables["OData"].Rows[0]["ODMensRet"].ToString().Trim().IndexOf("@") > indMaxMT)
                                oResultado.MensajeMT = dsResultado.Tables["OData"].Rows[0]["ODMensRet"].ToString().Trim().Replace("@", "^").ToString();
                            else
                                oResultado.MensajeMT = dsResultado.Tables["OData"].Rows[0]["ODMensRet"].ToString().Trim();

                            if (String.IsNullOrEmpty(oResultado.MensajeMT))
                            {
                                oMensajeOperacion = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.ERROR, string.Empty);
                                oResultado.MensajeMT = oMensajeOperacion.DescMensaje;
                                oResultado.MensajeError = CConstantes.MensajesError.NO_EXISTE_INFORMACION;
                            }

                            oResultado.Fecha = CCUtil.FormateaFecha(dsResultado.Tables["OData"].Rows[0]["ODFecha"].ToString().Trim());
                            oResultado.Hora = CCUtil.FormateaHora(dsResultado.Tables["OData"].Rows[0]["ODHora"].ToString().Trim());

                            NLog.InsertarLogOperaciones(oDatosOperacion.IdTransaccion.Trim(), oTipoOperacion, CConstantes.EstadosLog.INFORMACION,
                                            oTrama.Entrada, CSerializacion.SerializarXML<EResultadoMensajeMT>(oResultado),
                                            oTrama.HoraInicial, oResultado.MensajeError, oDatosOperacion.IdTransaccionVerifica.Trim());

                        }
                        else
                        {
                            oTipoOperacion = CCUtil.ObtenerOperacion(oDatosOperacion.IdOperacion);

                            oMensajeOperacion = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.ERROR, string.Empty);
                            oResultado.CodRet = oMensajeOperacion.IdMensaje;
                            oResultado.MensajeMT = oMensajeOperacion.DescMensaje;
                            oResultado.Fecha = CCUtil.FormateaFecha(String.Format("{0:ddMMyyyy}", DateTime.Now));
                            oResultado.Hora = CCUtil.FormateaHora(String.Format("{0:HHmmss}", DateTime.Now));
                            oResultado.MensajeError = CConstantes.MensajesError.NO_EXISTE_INFORMACION;

                            NLog.InsertarLogOperaciones(oDatosOperacion.IdTransaccion.Trim(), oTipoOperacion, CConstantes.EstadosLog.ADVERTENCIA,
                                         oTrama.Entrada, CSerializacion.SerializarXML<EResultadoMensajeMT>(oResultado),
                                         oTrama.HoraInicial, oResultado.MensajeError, oDatosOperacion.IdTransaccionVerifica.Trim());

                        }
                    }
                    else
                    {
                        oTipoOperacion = CCUtil.ObtenerOperacion(oDatosOperacion.IdOperacion);

                        oMensajeOperacion = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.ERROR, string.Empty);
                        oResultado.CodRet = oMensajeOperacion.IdMensaje;
                        oResultado.MensajeMT = oMensajeOperacion.DescMensaje;
                        oResultado.Fecha = CCUtil.FormateaFecha(String.Format("{0:ddMMyyyy}", DateTime.Now));
                        oResultado.Hora = CCUtil.FormateaHora(String.Format("{0:HHmmss}", DateTime.Now));
                        oResultado.MensajeError = CConstantes.MensajesError.NO_EXISTE_INFORMACION;

                        NLog.InsertarLogOperaciones(oDatosOperacion.IdTransaccion.Trim(), oTipoOperacion, CConstantes.EstadosLog.ADVERTENCIA,
                            oTrama.Entrada, CSerializacion.SerializarXML<EResultadoMensajeMT>(oResultado),
                            oTrama.HoraInicial, oResultado.MensajeError, oDatosOperacion.IdTransaccionVerifica.Trim());

                    }
                }
                else
                {
                    oTipoOperacion = CCUtil.ObtenerOperacion(oDatosOperacion.IdOperacion);
                    //if (oTipoOperacion.Valor.ToString().Length > 4)
                        oMensajeOperacion = ADMensajeOperacion.ObtenerMensajeOperacion(oResultado.CodigoError.Trim(), string.Empty);
                    //else
                    //    oMensajeOperacion = ADMensajeOperacion.ObtenerMensajeOperacion(oResultado.CodigoError.Trim(), oTipoOperacion.Valor.ToString().PadLeft(4, '0'));

                    if (oMensajeOperacion != null)
                    {
                        oResultado.CodRet = oMensajeOperacion.IdMensaje;
                        oResultado.MensajeMT = oMensajeOperacion.DescMensaje;
                        oResultado.MensajeError = oMensajeOperacion.DescMensaje;
                    }
                    else
                    {
                        oMensaje = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.ERROR, string.Empty);
                        oResultado.CodRet = oMensaje.IdMensaje;
                        oResultado.MensajeMT = oMensaje.DescMensaje;
                        oResultado.MensajeError = CConstantes.MensajesError.ERROR_CODIGO_RETORNO + oResultado.CodigoError;
                    }

                    oResultado.Fecha = CCUtil.FormateaFecha(String.Format("{0:ddMMyyyy}", DateTime.Now));
                    oResultado.Hora = CCUtil.FormateaHora(String.Format("{0:HHmmss}", DateTime.Now));

                    NLog.InsertarLogOperaciones(oDatosOperacion.IdTransaccion.Trim(), oTipoOperacion, CConstantes.EstadosLog.ADVERTENCIA,
                                         oTrama.Entrada, CSerializacion.SerializarXML<EResultadoMensajeMT>(oResultado),
                                         oTrama.HoraInicial, oResultado.MensajeError, oDatosOperacion.IdTransaccionVerifica.Trim());

                }

                return oResultado;
            }
            catch (Exception ex)
            {
                oTipoOperacion = CCUtil.ObtenerOperacion(oDatosOperacion.IdOperacion);

                oMensajeOperacion = ADMensajeOperacion.ObtenerMensajeOperacion(CConstantes.CodigoMensajeRetorno.ERROR, string.Empty);
                oResultado.CodRet = oMensajeOperacion.IdMensaje;
                oResultado.MensajeMT = oMensajeOperacion.DescMensaje;
                oResultado.Fecha = CCUtil.FormateaFecha(String.Format("{0:ddMMyyyy}", DateTime.Now));
                oResultado.Hora = CCUtil.FormateaHora(String.Format("{0:HHmmss}", DateTime.Now));
                oResultado.MensajeError = CConstantes.MensajesError.ERROR_PROCESAR_OPERACION;

                NLog.InsertarLogOperaciones(oDatosOperacion.IdTransaccion.Trim(), oTipoOperacion, CConstantes.EstadosLog.ERROR,
                                         oTrama.Entrada, CSerializacion.SerializarXML<EResultadoMensajeMT>(oResultado),
                                         oTrama.HoraInicial, oResultado.MensajeError, oDatosOperacion.IdTransaccionVerifica.Trim());


                //Inserta Error
                NLog.Insertar(usuario, oTipoOperacion, CConstantes.EstadosLog.ERROR,
                        "NTransacciones.cs : EjecutaTransaccionOperacion", string.Empty, ex.Message,
                        DateTime.Now, codigoApp);

                return oResultado;

            }
            finally
            {
                oResultado = null;
                respuesta = null;
                oTipoOperacion = null;
                oMensajeOperacion = null;
                oMensaje = null;
            }
        }
    }
}

