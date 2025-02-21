using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFP.WinService.Comun
{
    public class CConstantes
    {
        /// <summary>
        /// Clase que contiene los nombres de los stores procedures
        /// </summary>
        public static class StoredProcedures
        {
            public const string PRO_BC_ObtenerParametroOperacion = "PRO_BC_ObtenerParametrosOperacion";
            public const string PRO_BC_ObtenerCamposMensaje = "PROC_BC_Obtener_Campos_por_Mensaje";
            public const string PRO_BC_ObtenerMensajesNombre = "PROC_BC_Obtener_Mensajes_por_Nombre";
            public const string PRO_BC_InsertarLog = "PRO_BC_InsertarLog";
            public const string PRO_BC_ObtenerMensajeOperacionxCodigo = "PROC_BC_Obtener_Mensaje_Operacion_por_codigo";
            public const string PRO_BC_ObtenerItemsParametro = "PRO_BC_ObtenerItemsParametros";
            public const string PRO_BC_RegistraLogMensajeria = "PRO_BC_RegistrarLogMensajeria";
            public const string PRO_BC_Envia_Correo = "PRO_BC_Envia_Correo";
            public const string PRO_BC_ObtenerMensajeError_ServicioSMS = "PRO_BC_ObtenerMensajeError_ServicioSMS";
            public const string PRO_BC_ValidaNroMensajes = "PRO_BC_ObtenerMensajeError_ServicioSMS";
            public const string PRO_BC_TablaConfiguracion = "proc_BC_Obtener_TablaConfiguracion";
            public const string PRO_BC_ActualizarTablaConfiguracion = "proc_BC_Actualizar_TablaConfiguracion";
            

        }

        /// <summary>
        /// Clase que contiene los identificadores de las tablas maestras
        /// </summary>
        public static class TablaMaestras
        {
            public const string CODIGO_CORTO = "CODCOR";
            public const string SERIAL_SERVICIO_YP = "SERIAL";
            public const string PIN_SERVICIO_YP = "PIN_YP";
            public const string DISCADO_DIRECTO_INTERNACIONAL = "51";
        }

        /// <summary>
        /// Clase que contiene las constantes de las transacciones
        /// </summary>
        public static class Transacciones
        {
            public const int LONG_TRANS_CONSULTA_MENSAJERIA = 83;
            public const int LONG_TRANS_ACTUALIZA_MENSAJERIA = 848;
            public const int PosicionInicialCorte = 36;
            public static string Programa_Consulta_Mensajes_Pendientes() { return "0106"; }
            public static string Programa_Actualiza_Mensajes() { return "0112"; }
            public static string NombreMensajeOut = "OutDat";
            public static string Programa_Consulta_Notificaciones() { return "0107"; }

            public static string ConsultaAfiliacion = "Consulta de Afiliaciones";
            public static string ConsultaDesafiliacion = "Consulta de Desafiliaciones";
            public static string ActualizaAfiliacion = "Actualiza estado de SMS y Email de afiliaciones";
            public static string ActualizaDesafiliacion = "Actualiza estado de SMS y Email de desaafiliaciones";
        }

        /// <summary>
        /// Clase que contiene las constantes de los estados del log
        /// </summary>
        public static class CodigoAplicativo
        {
            public const int ServicioWinMensajeria = 2;
        }

        public static class CodigoMensajeRetorno
        {
            public const string EXITO = "0000";
            public const string SMS_AFILIACION = "0060";
            public const string SMS_DESAFILIACION = "0061";
            public const string ERROR = "9999";
        }

        /// <summary>
        /// Mensajes
        /// </summary>
        public static class MensajesError
        {
            public const string INFORMACION = "No existe informacion a procesar";
            public const string INFORMACION_AFILIACION = "No existe informacion de afiliaciones a procesar";
            public const string INFORMACION_DESAFILIACION = "No existe informacion de desafiliaciones a procesar";
            public const string ERROR_TRX_CONSULTA = "Error al realizar la consulta";
            public const string ERROR_TRX_CONSULTA_AFILIACION = "Error al realizar la consulta de afiliaciones";
            public const string ERROR_TRX_CONSULTA_DESAFILIACION = "Error al realizar la consulta de desafiliaciones";
            public const string ERROR_FORMATO_EMAIL = "Formato de correo invalido";

            public static string Error_MensajeSMS_YP = "Respuesta error del servicio SMS de YP";
            public static string ErrorEnvioSMS = "Error en el envio SMS";
            public static string ErrorEnvioEmail = "Error en el envio de email";
            public static string ErrorProcesamientoMensajeria = "Error en el procesamiento de la mensajeria";
        }

        /// <summary>
        /// Mensajes
        /// </summary>
        public static class TipoEventoAfiliacion
        {
            public const string AFILIACION = "A";
            public const string DESAFILIACION = "D";
        }

        /// <summary>
        /// Mensajes
        /// </summary>
        public static class TipoMensaje
        {
            public const int SMS = 1;
            public const int EMAIL = 2;
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
    }
}
