using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace BFP.AdminBancaCelular.Comun
{
    /// <summary>
    /// Clase común que implementa métodos de serialización XML.
    /// </summary>
    public static class CSerializacion
    {
        /// <summary>
        /// Deserializa una cadena XML en un objeto.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cadenaXML"></param>
        /// <returns>
        /// Un objeto con la deserialización de la cadena XML.
        /// </returns>
        public static T DeserializarXML<T>(string cadenaXML)
        {
            XmlSerializer serializador = new XmlSerializer(typeof(T));
            MemoryStream flujo = new MemoryStream(StringToUTF8ByteArray(cadenaXML));
            return (T)serializador.Deserialize(flujo);
        }

        /// <summary>
        /// Convierte una cadena a un arreglo de bytes UTF8.
        /// </summary>
        /// <param name="cadenaXML">Cadena XML a ser convertida en arreglo de bytes Unicode.</param>
        /// <returns>
        /// Arreglo de bytes convertido desde una cadena XML.
        /// </returns>
        private static byte[] StringToUTF8ByteArray(string cadenaXML)
        {
            return Encoding.UTF8.GetBytes(cadenaXML);
        }

        /// <summary>
        /// Serializa en formato XML un objeto.
        /// </summary>
        /// <param name="objeto">Objeto a ser serializado en formato XML.</param>
        /// <typeparam name="T">Tipo del objeto.</typeparam>
        /// <returns>
        /// Cadena XML con la serialización del objeto.
        /// </returns>
        public static string SerializarXML<T>(T objeto)
        {
            XmlSerializer serializador = new XmlSerializer(objeto.GetType());

            MemoryStream flujo = new MemoryStream();

            XmlWriterSettings configuracion = new XmlWriterSettings();
            configuracion.Indent = true;
            configuracion.Encoding = Encoding.UTF8;

            // configuracion.OmitXmlDeclaration = true;

            XmlWriter writer = XmlWriter.Create(flujo, configuracion);

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            // ns.Add(String.Empty, String.Empty);

            serializador.Serialize(writer, objeto, ns);

            return UTF8ByteArrayToString(flujo.ToArray());
        }

        /// <summary>
        /// Convierte un arreglo de bytes de valores Unicode (UTF-8) a una cadena.
        /// </summary>
        /// <param name="caracteres">Arreglo de bytes Unicode a ser convertido a cadena.</param>
        /// <returns>
        /// Cadena convertida desde un arreglo de bytes Unicode.
        /// </returns>
        private static string UTF8ByteArrayToString(byte[] caracteres)
        {
            return Encoding.UTF8.GetString(caracteres);
        }
    }
}
