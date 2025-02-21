using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFP.WinService.Entidad
{
    public class EMensajesEnviados
    {
        /// <summary>
        /// Numero del celular del cliente
        /// </summary>
        public string NroCelular { get; set; }

        /// <summary>   
        /// Tipo Mensajería: A-Afiliacion / D-Desafiliacion
        /// </summary>
        public string TipoEvento { get; set; }

        /// <summary>   
        /// Flag que indica que se envio el SMS
        /// </summary>
        public bool EnviaSMS { get; set; }

        /// <summary>   
        /// Flag que indica que se envio el correo electronico
        /// </summary>
        public bool EnviaCorreo { get; set; }
    }
}
