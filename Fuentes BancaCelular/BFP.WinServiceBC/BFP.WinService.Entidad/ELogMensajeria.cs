using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFP.WinService.Entidad
{
    public class ELogMensajeria
    {
        public int Ident_Mensajeria { get; set; }
        public string Ident_Mensaje { get; set; }
        public string Ident_Canal { get; set; }
        public int Ident_Tipo_Mensaje { get; set; }
        public string Nro_Celular_Envio { get; set; }
        public string Email_Envio { get; set; }
        public string Codigo_Respuesta_YP { get; set; }
        public DateTime Fech_Envio_Mensaje { get; set; }
        public DateTime? Fech_Respuesta_YP { get; set; }
        public bool Estado_Envio_Mensaje { get; set; }
        public string Cuerpo_Mensaje { get; set; }
        public int Tipo_Mensaje { get; set; } 
    }
}
