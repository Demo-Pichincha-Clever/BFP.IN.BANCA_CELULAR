using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFP.BancaCelular.Entidad
{
    [Serializable]
    public class EMensajeOperacion
    {
        /// <summary>
        /// Identificador del mensaje
        /// </summary>
        public string IdMensaje { get; set; }

        /// <summary>
        /// Nombre del mensaje
        /// </summary>
        public string NombreMensaje { get; set; }

        /// <summary>   
        /// Descripcion del mensaje
        /// </summary>
        public string DescMensaje { get; set; }
    }
}
