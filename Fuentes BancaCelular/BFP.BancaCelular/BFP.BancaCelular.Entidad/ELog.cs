using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFP.BancaCelular.Entidad
{
    public class ELog
    {

        public ELog()
        {
            FechaHora = DateTime.Now;
        }

        /// <summary>
        /// Usuario
        /// </summary>
        public string Usuario { get; set; }

        /// Fehca Actual
        /// </summary>
        public DateTime FechaHora { get; set; }

        public DateTime Fecha
        {
            get
            {
                return FechaHora;
            }
        }

        /// <summary>
        /// Hora inicial
        /// </summary>
        public DateTime HoraInicial { get; set; }

        /// <summary>
        /// Hora final
        /// </summary>
        public DateTime HoraFinal { get; set; }

        /// <summary>
        /// Mensaje
        /// </summary>
        public string IdOperacion { get; set; }

        /// <summary>
        /// Descripcion Mensaje
        /// </summary>
        public string DescripcionOperacion { get; set; }

        /// <summary>
        /// Tipo Evento
        /// </summary>
        public string TipoEvento { get; set; }

        /// <summary>
        /// Lugar desde donde se inserta el log.
        /// </summary>
        public string Origen { get; set; }

        /// <summary>
        /// Datos de entrada.
        /// </summary>
        public string Entrada { get; set; }

        /// <summary>
        /// Datos de salida.
        /// </summary>
        public string Salida { get; set; }

        /// <summary>
        /// Codigo del aplicativo: 1-BancaCelular / 2-Servicio Win / 3-Afiliacion HB
        /// </summary>
        public int CodigoApp { get; set; }

    }
}
