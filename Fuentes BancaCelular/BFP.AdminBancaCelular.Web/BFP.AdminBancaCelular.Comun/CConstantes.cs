using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFP.AdminBancaCelular.Comun
{
    /// <summary>
    /// Contiene las constantes del sistema.
    /// </summary> 
    public static class CConstantes
    {
        /// <summary>
        /// Clase que contiene los nombres de los stores procedures
        /// </summary>
        public static class StoredProcedures
        {
            public const string PRO_BC_ObtenerParametroOperacion = "PRO_BC_ObtenerParametrosOperacion";
            public const string PRO_BC_InsertarLog = "PRO_BC_InsertarLog";
            public const string PRO_BC_ObtenerItemsParametro = "PRO_BC_ObtenerItemsParametros";
        }

        /// <summary>
        /// Clase que contiene los identificadores de las tablas maestras
        /// </summary>
        public static class TablaMaestras
        {
            public const string PARAMETROS_OPERACION = "PAROPE";
            public const string TABLA_USUARIOS = "USERBC";
        }

        /// <summary>
        /// Clase que contiene las constantes de los estados del log
        /// </summary>
        public static class EstadosLog
        {
            public const string INFORMACION = "I";
            public const string ADVERTENCIA = "A";
            public const string ERROR = "E";
        }

        /// <summary>
        /// Clase que contiene las constantes de los estados del log
        /// </summary>
        public static class CodigoAplicativo
        {
            public const int AdministradorWeb = 3;
        }

        public static class CodigoRetorno
        {
            public const string EXITO = "0000";
            public const string ERROR = "9999";
        }
    }
}
