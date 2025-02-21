using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFP.BancaCelular.Entidad
{

    [Serializable]
    public class ELogOperaciones
    {
    
        public string codigoTransaccion { get; set; }
        public string idOperacion { get; set; }
        public string descripcionOperacion { get; set; }
        public string tramaEntrada { get; set; }
        public string xmlRetorno { get; set; }
        public DateTime fechaRecepcion { get; set; }
        public DateTime fechaEnvio { get; set; }
        public string tipoEvento { get; set; }
        public string mensajeRetornoError { get; set; }
        public string codigoTransaccionVerifica { get; set; }

    }
}