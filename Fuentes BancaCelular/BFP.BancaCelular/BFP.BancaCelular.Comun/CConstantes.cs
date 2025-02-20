using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFP.BancaCelular.Comun
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
            public const string PRO_BC_ObtenerMensajesNombre = "PROC_BC_Obtener_Mensajes_por_Nombre";
            public const string PRO_BC_ObtenerCamposMensaje = "PROC_BC_Obtener_Campos_por_Mensaje";
            public const string PRO_BC_InsertarLog = "PRO_BC_InsertarLog";
            public const string PRO_BC_ObtenerMensajeOperacionxCodigo = "PROC_BC_Obtener_Mensaje_Operacion_por_codigo";
            public const string PRO_BC_ObtenerItemsParametro = "PRO_BC_ObtenerItemsParametros";
            public const string PRO_BC_RegistraLogMensajeria = "PRO_BC_RegistrarLogMensajeria";
            public const string PRO_BC_InsertarLogOperaciones = "PRO_BC_InsertarLogOperaciones";
            public const string PRO_BC_VerificaBloqueoTemporal = "proc_BC_bloqueoPorDesafiliacion_NumeroIntentos";
            public const string PRO_BC_RegistraIntentosOperacion = "proc_BC_RegistrarIntentosOperacion";
        }

        /// <summary>
        /// Clase que contiene los identificadores de las tablas maestras
        /// </summary>
        public static class TablaMaestras
        {
            public const string PARAMETROS_OPERACION = "PAROPE";
        }

        /// <summary>
        /// Clase que contiene las constantes de las transacciones
        /// </summary>
        public static class Transacciones
        {
            public const int PosicionInicialCorte = 36;
            public static string Programa_Resolutor() { return "0100"; }
            public static string NombreMensajeOut = "OutDat";
            public const int LONGITUD_OPERACION = 150;
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
            public const int ServicioWeb = 1;
        }

        public static class CodigoMensajeRetorno
        {
            public const string ERROR = "0014";
            public const string ERROR_AUTENTIFICACION = "0015";
            public const string ERROR_SINTAXIS_OMISION = "0011";
            public const string EXITO = "0000";
            public const string ERROR_OPERACION_DESHABILITADA = "0027";
            public const string CLIENTE_DESAFILIADO = "0001";
            public const string ERROR_GENERACION_SINTAXIS_INCORRECTA = "6664";
        }

        /// <summary>
        /// Mensajes
        /// </summary>
        public static class MensajesError
        {
            public const string ERROR_AUTENTIFICACION = "Error en la autentificación";
            public const string GENERACION_TRAMA_PARAMETROS = "Error al generar la trama de parametros";
            public const string ERROR_PROCESAR_OPERACION = "Error al procesar la operacion";
            public const string NO_EXISTE_INFORMACION = "No se encontro informacion a retornar";
            public const string ERROR_CODIGO_RETORNO = "Error en la operacion. Codigo de retorno: ";
            public const string OMISION_PRIMER_PARAMETRO = "Error en la sintaxis, se esta omitiendo el primer parametro";
            public const string ERROR_TRX_CONSULTA = "Error al realizar la consulta";
            public const string ERROR_ACCESO_PROXY_ENCRIPTACION = "Error al acceder al servicio de encriptación";
            public const string OPERACION_NO_REGISTRADA = "La operacion no figura registrada en el XML";
            public const string OPERACION_NO_HABILITADA = "La operacion no esta habilitada";
            public const string GENERACION_TRAMA_BASE = "La Generacion de alguna trama es incorrecta";
        }
    }
}
