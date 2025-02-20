using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BFP.WinService.Comun;
using BFP.WinService.Entidad;
using BFP.WinService.Data;

namespace BFP.WinService.Negocio
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
    }
}
