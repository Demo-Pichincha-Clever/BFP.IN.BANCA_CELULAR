using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BFP.WinService.Entidad
{
    /// <summary>
    /// Representa un catalogo key-value.
    /// </summary>
    /// <typeparam name="T">Tipo del parámetro</typeparam>
    /// <typeparam name="V">Valor del parámetro</typeparam>
    [Serializable]
    public class ECatalogo<T, V>
    {
        /// <summary>
        /// Construye una nueva instancia del objeto catalogo, con su clave y valor inicializado.
        /// </summary>
        /// <param name="nombre">Clave del catalogo</param>
        /// <param name="valor">Valor del catalogo</param>
        public ECatalogo(T nombre, V valor)
        {
            Nombre = nombre;
            Valor = valor;
        }

        /// <summary>
        /// Constructor sin parametros, requisito para la serializacion.
        /// </summary>
        public ECatalogo()
        {
        }

        /// <summary>
        /// Nombre del catalogo.
        /// </summary>        
        [XmlElement("nombre")]
        public T Nombre { get; set; }

        /// <summary>
        /// Conjunto de valores del catalogo.
        /// </summary>      
        [XmlElement("valor")]
        public V Valor { get; set; }
    }
}
