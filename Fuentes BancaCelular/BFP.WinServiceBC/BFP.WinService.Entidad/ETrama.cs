using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BFP.WinService.Entidad
{

    /// <summary>
    /// Entidad de trama
    /// </summary>
    [Serializable]
    public class ETrama : EBase
    {
        /// <summary>
        /// Entrada
        /// </summary>
        public string Entrada { get; set; }

        /// <summary>
        /// Salida
        /// </summary>
        public string Salida { get; set; }
    }
}

