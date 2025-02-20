using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BFP.BancaCelular.Entidad
{
    /// <summary>
    /// Clase entidad que describe las propiedades comunes.
    /// </summary>
    [Serializable]
    public class EBase
    {
        /// <summary>
        /// Fecha creación del usuario.
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Usuario creador.
        /// </summary>
        public string UsuarioCreacion { get; set; }

        /// <summary>
        /// Fecha creación del usuario.
        /// </summary>
        private DateTime mFechaModificacion;

        public DateTime FechaModificacion { get; set; }

        /// <summary>
        /// Usuario actualizador.
        /// </summary>
        public string UsuarioModificacion { get; set; }

        /// <summary>
        /// Código de actualizacion.
        /// </summary>
        private string mTipoActualizacion;

        [XmlIgnore]
        public string TipoActualizacion
        {
            get { return mTipoActualizacion; }
            set { mTipoActualizacion = value; }
        }

        /// <summary>
        /// Código de operación.
        /// </summary>
        [XmlIgnore]
        public string IdOperacion { get; set; }


        /// <summary>
        /// Operacion.
        /// </summary>
        [XmlIgnore]
        public string Operacion { get; set; }

        /// <summary>
        /// Operacion.
        /// </summary>
        [XmlIgnore]
        public DateTime HoraInicial { get; set; }

    }
}
