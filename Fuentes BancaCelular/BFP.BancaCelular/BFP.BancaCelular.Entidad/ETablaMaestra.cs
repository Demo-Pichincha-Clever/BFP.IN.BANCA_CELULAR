using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BFP.BancaCelular.Entidad
{
    [Serializable]
    public class ETablaMaestra 
    {
        /// <summary>
        /// Identificador de la tabla maestra
        /// </summary>
        public string idTabla { get; set; }

        /// <summary>
        /// Descripcion de la tabla maestra
        /// </summary>
        public string descTabla { get; set; }

        /// <summary>
        /// Identificador del tipo de tabla maestra (T= Tabla | P= Parametro)
        /// </summary>
        public string tipoTabla { get; set; }

        /// <summary>
        /// Valor del registro de tipo Parametro
        /// </summary>
        public string valorTabla { get; set; }

        /// <summary>
        /// Indica si la tabla es editable o no. (true, false)
        /// </summary>
        public bool indModifica { get; set; }

        /// <summary>
        /// Indica si la tabla puede eliminarse logicamente. (true, false)
        /// </summary>
        public bool indElimina { get; set; }

        /// <summary>
        /// Identificador del usuario que realiza el registro
        /// </summary>
        public int idUsuarioCreacion { get; set; }

        /// <summary>
        /// Fecha de registro
        /// </summary>
        public DateTime fechaCreacion { get; set; }

        /// <summary>
        /// Identificador del usuario que realiza la modificacion
        /// </summary>
        public int? idUsuarioModificacion { get; set; }
    
        /// <summary>
        /// Modificacion del registro
        /// </summary>
        public DateTime? fechaModificacion { get; set; }

        /// <summary>
        /// Estado de la tabla maestra
        /// </summary>
        public bool indEstado { get; set; }
    }
}
