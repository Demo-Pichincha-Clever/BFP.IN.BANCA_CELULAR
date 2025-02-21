using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BFP.WinService.Comun;
using BFP.WinService.Entidad;

namespace BFP.WinService.Data
{
    public class ADTablaMaestra : ADBase
    {
        /// <summary>
        /// Obtener operaciones
        /// </summary>
        /// <returns></returns>
        public static EItemsParametros[] ObtenerItemsParametrosOperacion(string codigoTablaMaestra)
        {
            DataSet dataSet;
            EItemsParametros[] itemsParametro = null;
            object[] parametros = null;

            try
            {
                parametros = new object[] { codigoTablaMaestra };

                dataSet = GetDatabase().ExecuteDataSet(CConstantes.StoredProcedures.PRO_BC_ObtenerParametroOperacion, parametros);
                StringBuilder sbuilder = new StringBuilder();

                bool leido = false;

                foreach (DataRow fila in dataSet.Tables[0].Rows)
                {
                    sbuilder.Append(fila[0].ToString());
                    leido = true;
                }

                if (leido)
                    itemsParametro = CSerializacion.DeserializarXML<EItemsParametros[]>(sbuilder.ToString());

                return itemsParametro;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                dataSet = null;
                itemsParametro = null;
                parametros = null;
            }
        }

        /// <summary>
        /// Obtener operaciones
        /// </summary>
        /// <returns></returns>
        public static EItemsParametros[] ObtenerItemsParametros(string codigoTablaMaestra)
        {
            DataSet dataSet;
            EItemsParametros[] itemsParametro = null;
            StringBuilder sbuilder;
            object[] parametros;

            try
            {
                parametros = new object[] { codigoTablaMaestra };

                dataSet = GetDatabase().ExecuteDataSet(CConstantes.StoredProcedures.PRO_BC_ObtenerItemsParametro, parametros);
                sbuilder = new StringBuilder();

                bool leido = false;

                foreach (DataRow fila in dataSet.Tables[0].Rows)
                {
                    sbuilder.Append(fila[0].ToString());
                    leido = true;
                }

                if (leido)
                    itemsParametro = CSerializacion.DeserializarXML<EItemsParametros[]>(sbuilder.ToString());

                return itemsParametro;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                dataSet = null;
                itemsParametro = null;
                sbuilder = null;
                parametros = null;
            }
        }
    }
}