using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BFP.BancaCelular.Entidad
{
    [Serializable]
    public class EResultadoMensajeMT
    {
        /// <summary>
        /// Código de retorno
        /// </summary>
        public string CodRet { get; set; }

        /// <summary>
        /// Mensaje de retorno - MT
        /// </summary>
        public string MensajeMT { get; set; }

        /// <summary>
        /// Fecha de la devolución del mensaje MT
        /// </summary>
        public string Fecha { get; set; }


        /// <summary>
        /// Hora de la devolución del mensaje MT
        /// </summary>
        public string Hora { get; set; }

        /// <summary>
        /// Fecha de la devolución del mensaje MT
        /// </summary>
        [XmlIgnore]
        public string CodigoError { get; set; }


        /// <summary>
        /// Hora de la devolución del mensaje MT
        /// </summary>
        [XmlIgnore]
        public string MensajeError { get; set; }
    }
}
