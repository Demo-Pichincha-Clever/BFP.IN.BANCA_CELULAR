using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BFP.BancaCelular.Entidad
{
    [Serializable]
    public class EDatosOperacion
    {
        /// <summary>
        /// Código transaccion de la operacion, generado por YP
        /// </summary>
        public string IdTransaccion { get; set; } 

        /// <summary>
        /// Identificador de la operacion de la banca celular
        /// </summary>
        public int IdOperacion { get; set; }

        /// <summary>
        /// Nombre de la operacion de la banca celular
        /// </summary>
        public string NombreOperacion { get; set; }

        /// <summary>
        /// Numero del telefono afiliado
        /// </summary>
        public string NumeroTelefono { get; set; }

        /// <summary>
        /// Identificador de la operadora 1:Claro / 2:Movistar
        /// </summary>
        public int IdOperadora { get; set; }

        /// <summary>
        /// Codigo transaccion de verificacion, enviado por YP
        /// </summary>
        /// 
        public string IdTransaccionVerifica { get; set; }

        /// <summary>
        /// Programa que atiende la operacion
        /// </summary>
        public string ProgramaAS400 { get; set; }

        /// <summary>
        /// Estado que indica si operacion se encuentra habilitada o no.
        /// </summary>
        public bool Habilitado { get; set; }

    }
}
