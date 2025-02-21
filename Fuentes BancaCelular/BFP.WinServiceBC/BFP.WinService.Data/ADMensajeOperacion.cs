using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml.Serialization;

using BFP.WinService.Entidad;
using BFP.WinService.Comun;

namespace BFP.WinService.Data
{
    public class ADMensajeOperacion : ADBase
    {
        /// <summary>
        /// Obtener mensaje de las operaciones
        /// </summary>
        /// <returns></returns>
        public static EMensajeOperacion ObtenerMensajeOperacion(string codigoMensaje, string nombreTransaccion)
        {
            DataSet dataSet;
            EMensajeOperacion mensajeOperacion = null;
            StringBuilder sbuilder;
            object[] parametros;

            try
            {
                parametros = new object[] { codigoMensaje, nombreTransaccion };

                dataSet = GetDatabase().ExecuteDataSet(CConstantes.StoredProcedures.PRO_BC_ObtenerMensajeOperacionxCodigo, parametros);
                sbuilder = new StringBuilder();

                bool leido = false;

                foreach (DataRow fila in dataSet.Tables[0].Rows)
                {
                    sbuilder.Append(fila[0].ToString());
                    leido = true;
                }

                if (leido)
                {
                    mensajeOperacion = CSerializacion.DeserializarXML<EMensajeOperacion>(sbuilder.ToString());
                }

                return mensajeOperacion;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                dataSet = null;
                mensajeOperacion = null;
                sbuilder = null;
                parametros = null;
            }

        }

        /// <summary>
        /// Obtener mensaje de error de YP
        /// </summary>
        /// <returns></returns>
        public static string ObtenerMensajeError_ServicioSMS(string codigoMensaje)
        {
            object[] parametros;
            string descripcionMensaje = string.Empty;

            try
            {
                parametros = new object[] { codigoMensaje };
                descripcionMensaje = Convert.ToString(GetDatabase().ExecuteScalar(CConstantes.StoredProcedures.PRO_BC_ObtenerMensajeError_ServicioSMS, parametros));

                return descripcionMensaje;
            }
            catch (Exception)
            {
                return string.Empty;
            }
            finally
            {
                parametros = null;
            }
        }
    }
}
