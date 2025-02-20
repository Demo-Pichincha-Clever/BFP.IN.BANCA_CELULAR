using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BFP.BancaCelular.Comun;
using BFP.BancaCelular.Entidad;

namespace BFP.BancaCelular.Data
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

        /// <summary>
        /// Inserta el log de operaciones
        /// </summary>
        /// <param name="log">Objeto de tipo log de operaciones</param>
        /// 
        public static void InsertarLogOperaciones(ELogOperaciones log)
        {
            object[] parametros = null;

            try
            {
                parametros = new object[]
                {
                    log.codigoTransaccion,
                    log.idOperacion,
                    log.descripcionOperacion,
                    log.tramaEntrada,
                    log.xmlRetorno,
                    log.fechaEnvio,
                    log.fechaRecepcion,
                    log.tipoEvento,
                    log.mensajeRetornoError,
                    log.codigoTransaccionVerifica
                };

                // grabar logproceso             
                GetDatabase().ExecuteNonQuery(CConstantes.StoredProcedures.PRO_BC_InsertarLogOperaciones, parametros);
            }
            catch (Exception)
            {
                parametros = null;
            }

        }
    }
}
