using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using BFP.BancaCelular.Entidad;
using BFP.BancaCelular.Comun;

namespace BFP.BancaCelular.Data
{
    public class ADMensajeOperacion : ADBase
    {
        /// <summary>
        /// Obtener operaciones
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
                return mensajeOperacion;
            }
            finally
            {
                dataSet = null;
                mensajeOperacion = null;
                sbuilder = null;
                parametros = null;
            }

        }
    }
}
