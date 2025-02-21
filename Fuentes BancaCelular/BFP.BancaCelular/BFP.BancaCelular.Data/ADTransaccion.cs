using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using BFP.BancaCelular.Entidad;
using BFP.BancaCelular.Comun;

namespace BFP.BancaCelular.Data
{
    public static class ADTransaccion
    {
        private static ESalida<string> _oError = new ESalida<string>();

        /// <summary>
        /// Ejecuta la operacion solicitada
        /// </summary>
        /// <param name="strNombreTransaccion">Codigo de la transaccion en AS400</param>
        /// <param name="shrLongitudCabecera">Longitud permitida para el envio de la trama en AS400</param>
        /// <param name="strMensajeIn">Trama de la operacion</param>
        /// <param name="strNombreMensajeOut">Nombre de la etiqueta de salida en el XML</param>
        /// <param name="intPosicionInicialLecturaOut">Posicion inicial a leer de la data que retorna el AS400</param>
        /// <param name="oError">Variable de salidad que retorna el codigo y mensaje de error retornado desde AS400</param>
        /// <returns>Retorna dataset con los datos de retorno del AS400</returns>
        /// 
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
                //strResultado = objSixCommunication.SendMessage(strNombreTransaccion,
                //                                                    shrLongitudCabecera, strMensajeIn);
                shrLongitudCabecera = short.Parse((82 + strMensajeIn.Length).ToString());
                strResultado = objSixCommunication.SendMessage(strNombreTransaccion,
                                                    shrLongitudCabecera, strMensajeIn);
                _oError.Codigo = strResultado.Substring(17, 4);

                if (_oError.Codigo == CConstantes.CodigoMensajeRetorno.EXITO)
                {
                    DataLoader xml = new DataLoader();
                    dsSalida = xml.ObtenerData(strResultado, strNombreMensajeOut, strNombreTransaccion,
                                                                intPosicionInicialLecturaOut);

                    _oError.Codigo = CConstantes.CodigoMensajeRetorno.EXITO;
                    _oError.Mensaje = string.Empty;
                }
                else
                {
                    _oError.Mensaje = strResultado.Substring(36, 4061);
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

    }
}
