using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BFP.BancaCelular.Comun;
using BFP.BancaCelular.Entidad;
using BFP.BancaCelular.Data;

namespace BFP.BancaCelular.Negocio
{
    public static class NGeneral
    {
        public static bool VerificarBloqueoTemporalPorDesafiliacion(string strNumeroCelular , int  intOperacionId,  out string strError)
        {
            return ADGeneral.VerificarBloqueoTemporalPorDesafiliacion(strNumeroCelular, intOperacionId, out strError);
        }

        public static void RegistrarIntetosOperacionDia(string strNumeroCelular, int intOperacionId)
        {
            ADGeneral.RegistrarIntetosOperacionDia(strNumeroCelular, intOperacionId);
        }
    }
}
