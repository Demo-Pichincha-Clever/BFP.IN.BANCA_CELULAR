using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BFP.AdminBancaCelular.Entidad
{
    /// <summary>
    /// Clase que detalla los parametros de la operacion
    /// </summary>
    [Serializable]
    public class EParametrosOperacion
    {
        /// <summary>
        /// Identificador de la operacion
        /// </summary>
        public int IdOperacion { get; set; }

        /// <summary>
        /// Identificador del parametro
        /// </summary>
        public int IdParametro{ get; set; }

        /// <summary>   
        /// Nombre del parametro
        /// </summary>
        public string NombreParametro { get; set; }

        /// <summary>   
        /// Descripcion del parametro
        /// </summary>
        public string DescripcionParametro { get; set; }

        /// <summary>   
        /// Valor del parametro
        /// </summary>
        public string ValorParametro { get; set; }

        /// <summary>
        /// Descripcion del item de la tabla maestra
        /// </summary>
        public string TipoDato { get; set; }

        /// <summary>
        /// Longitud de cada parametro
        /// </summary>
        public string Longitud { get; set; }

        /// Longitud de datos decimales
        /// </summary>
        public string Decimal { get; set; }

        /// Parametro seleccionado para la operacion
        /// </summary>
        public bool Selecciona { get; set; }
    }
}
