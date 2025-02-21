using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BFP.BancaCelular.Comun;
using BFP.BancaCelular.Entidad;
using BFP.BancaCelular.Data;

namespace BFP.BancaCelular.Negocio
{
    public static class NLog
    {
        /// <summary>
        /// Insertar log de eventos
        /// </summary>
        /// <param name="log"></param>
        public static void Insertar(string idUsuario, ECatalogo<string, string> objOpe,
                                    string TipoEvento, string Origen,
                                    string Entrada, string Salida, DateTime HoraInicial, int codigoApp)
        {
            ELog log = new ELog();

            if (idUsuario != null)
            {
                log.Usuario = idUsuario;
            }
            else
            {
                log.Usuario = string.Empty;
            }

            if (objOpe != null)
            {
                log.IdOperacion = objOpe.Valor;
                log.DescripcionOperacion = objOpe.Nombre;
            }
            else
            {
                log.IdOperacion = string.Empty;
                log.DescripcionOperacion = string.Empty;
            }

            log.HoraInicial = HoraInicial;
            log.TipoEvento = TipoEvento;
            log.Origen = Origen;
            log.Entrada = Entrada;
            log.Salida = Salida;
            log.HoraFinal = DateTime.Now;
            log.CodigoApp = codigoApp;

            ADLog.Insertar(log);
        }



        public static void InsertarLogOperaciones(string idTransaccion, ECatalogo<string, string> operacion,
                                    string tipoEvento, string tramaEntrada, string xmlRetorno, 
                                    DateTime HoraRecepcion, string mensajeError, string codigoTransaccionVerifica)
        {

            ELogOperaciones log = new ELogOperaciones();

            log.codigoTransaccion  = idTransaccion;

            if (operacion != null)
            {
                log.idOperacion = operacion.Valor;
                log.descripcionOperacion = operacion.Nombre;
            }
            else
            {
                log.idOperacion = string.Empty;
                log.descripcionOperacion = string.Empty;
            }

            log.fechaRecepcion = HoraRecepcion;
            log.tipoEvento = tipoEvento;
            log.tramaEntrada = tramaEntrada;
            log.xmlRetorno = xmlRetorno;
            log.fechaEnvio = DateTime.Now;
            log.mensajeRetornoError = mensajeError;
            log.codigoTransaccionVerifica = codigoTransaccionVerifica;

            ADLog.InsertarLogOperaciones(log);
        }
    }
}
