using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BFP.BancaCelular.Comun;
using BFP.BancaCelular.Entidad;
namespace BFP.BancaCelular.Data
{
    public class ADGeneral : ADBase
    {
        public static Boolean VerificarBloqueoTemporalPorDesafiliacion(string strNumeroCelular, int intOperacionId, out string strError ) 
        {
            DataSet dataSet = null;
            object[] parametros;
            bool booProcesar = true;
            string _error = "0000";
            try
            {
                parametros = new object[] { strNumeroCelular, intOperacionId };
                dataSet = GetDatabase().ExecuteDataSet(CConstantes.StoredProcedures.PRO_BC_VerificaBloqueoTemporal, parametros);
                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0) 
                {
                    _error = dataSet.Tables[0].Rows[0]["strError"].ToString();
                    booProcesar = false;
                    if (string.Compare(_error, "0000") == 0) booProcesar = true;
                }
                
                strError = _error;
                return booProcesar;
            }
            catch (Exception)
            {
                strError = "0000";
                return true;
            }
            finally
            {
                dataSet = null;
                parametros = null;
            }
        }

        public static void RegistrarIntetosOperacionDia(string strNumeroCelular, int intOperacionId)
        {
            object[] parametros;
            string stError = string.Empty;
            try
            {
                parametros = new object[] { strNumeroCelular, intOperacionId };
                GetDatabase().ExecuteNonQuery(CConstantes.StoredProcedures.PRO_BC_RegistraIntentosOperacion, parametros);
                
            }
            catch (Exception ex)
            {
                stError = ex.Message;
            }
            finally
            {
                parametros = null;
            }
        }
    }
}
