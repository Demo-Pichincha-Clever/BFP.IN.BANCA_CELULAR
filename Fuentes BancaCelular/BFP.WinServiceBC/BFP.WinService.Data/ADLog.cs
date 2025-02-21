using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BFP.WinService.Comun;
using BFP.WinService.Entidad;

namespace BFP.WinService.Data
{
    /// <summary>
    /// Manejador de data para el log de eventos.
    /// </summary>
    public class ADLog : ADBase
    {
        /// <summary>
        /// Insertar log de eventos
        /// </summary>
        /// <param name="log">Entidad CLogEvento</param>       
        public static void Insertar(ELog log)
        {
            object[] parametros = null;

            try
            {
                parametros = new object[]
                {
                    log.Fecha,
                    log.HoraInicial,
                    log.HoraFinal,
                    log.Usuario,
                    log.IdOperacion,
                    log.DescripcionOperacion,
                    log.TipoEvento,
                    log.Origen,
                    log.Entrada,
                    log.Salida,
                    log.CodigoApp
                };

                // grabar logproceso             
                GetDatabase().ExecuteNonQuery(CConstantes.StoredProcedures.PRO_BC_InsertarLog, parametros);
            }
            catch (Exception)
            {
                parametros = null;
            }

        }
    }
}
