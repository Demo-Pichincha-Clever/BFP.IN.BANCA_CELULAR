using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BFP.AdminBancaCelular.Entidad
{
    [Serializable]
    public class EItemsParametros 
    {
        /// <summary>
        /// Identificador de la tabla maestra
        /// </summary>
        public string IdTabla { get; set; }

        /// <summary>
        /// Identificador del item de la tabla maestra
        /// </summary>
        [XmlElement("IdParametro")]
        public int IdItemTabla { get; set; }
        
        /// <summary>   
        /// Valor del item de la tabla maestra
        /// </summary>
        [XmlElement("ValorParametro")]
        public string ValItemTabla { get; set; }

        /// <summary>
        /// Descripcion larga del item de la tabla maestra
        /// </summary>
        [XmlElement("DescripcionParametro")]
        public string DescLargaItemTabla { get; set; }

        /// <summary>
        /// Descripcion corta del item de la tabla maestra
        /// </summary>
        [XmlIgnore]
        public string DescCortaItemTabla { get; set; }
        
        /// <summary>
        /// Indicador para modificar el item
        /// </summary>
        [XmlIgnore]
        public bool IndModifica { get; set; }

        /// <summary>
        /// Indicador para eliminar el item
        /// </summary>
        [XmlIgnore]
        public bool IndElimina { get; set; }

        /// <summary>
        /// Usuario que registra el item
        /// </summary>
        [XmlIgnore]
        public int IdUsuarioCreacion { get; set; }

        /// <summary>
        /// Fecha que registra el item
        /// </summary>
        [XmlIgnore]
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Item que actualiza el item
        /// </summary>
        [XmlIgnore]
        public int? IdUsuarioModificacion { get; set; }

        /// <summary>
        /// Fecha que actualiza el item
        /// </summary>
        [XmlIgnore]
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// Estado del item de la tabla maestra
        /// </summary
        [XmlIgnore]
        public bool IndEstado { get; set; }

        /// <summary>   
        /// Longitud del parametro
        /// </summary>
        public string LongitudParametro { get; set; }

        /// <summary>   
        /// Tipo de dato del parametro
        /// </summary>
        public string TipoDatoParametro { get; set; }
    }
}
