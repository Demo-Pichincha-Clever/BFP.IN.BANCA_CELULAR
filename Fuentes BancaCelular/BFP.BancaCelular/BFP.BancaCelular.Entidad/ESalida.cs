using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BFP.BancaCelular.Entidad
{
    /// <summary>
    /// Clase entidad que describe las propiedades de salida para la mensajería de seguridad.
    /// </summary>
    /// <typeparam name="T">Tipo de dato del código.</typeparam>
    [Serializable]
    public class ESalida<T>
    {
        /// <summary>
        /// Código.
        /// </summary>
        [XmlElement("codigo")]
        public T Codigo {get; set;}

        /// <summary>
        /// Descripción del mensaje.
        /// </summary>
        [XmlElement("descripcion")]
        public string Mensaje {get; set;}
    }
}
