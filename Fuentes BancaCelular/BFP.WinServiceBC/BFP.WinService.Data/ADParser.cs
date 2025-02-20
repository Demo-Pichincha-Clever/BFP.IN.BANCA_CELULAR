using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BFP.WinService.Comun;
using System.Data.SqlClient;

namespace BFP.WinService.Data
{
    public partial class ADParser : ADBase
    {
        public IDataReader ObtenerMensajePorNombre(string strNombreMensaje, string strNombreTransaccion)
        {
            object[] parametros = new object[] { strNombreMensaje, strNombreTransaccion };
            IDataReader reader = null;

            try
            {
                reader = GetDatabase().ExecuteReader(CConstantes.StoredProcedures.PRO_BC_ObtenerMensajesNombre, parametros);

                return reader;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                parametros = null;
                reader = null;
            }
        }

        public IDataReader ObtenerCamposPorMensaje(int intMensajeID)
        {
            object[] parametros = new object[] { intMensajeID };
            IDataReader reader = null;

            try
            {
                reader = GetDatabase().ExecuteReader(CConstantes.StoredProcedures.PRO_BC_ObtenerCamposMensaje, parametros);

                return reader;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                parametros = null;
                reader = null;
            }
        }
    }
}
