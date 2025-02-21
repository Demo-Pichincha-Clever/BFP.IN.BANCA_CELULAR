using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TestServiceProd.BFP.ServiceBancaCelular;

namespace TestServiceProd
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBancaCelularClient serviceBC = new ServiceBancaCelularClient();
            UsuarioCredencial credencial = new UsuarioCredencial();

            credencial.userName = "0D93C548B0DB6DBB";
            credencial.password = "78BC51938C6394E5";

            string strIdTransaccion = "000000005";
            int intIdOperacion = 9;
            string strNumeroTelefono = "954189547";
            int intIdOperadora = 1;
            string strIdTransaccionVerifica = string.Empty;
            string strParametrosOperacion = "";


            EResultadoMensajeMT oResultadoMT = serviceBC.OperacionBancaCelular(credencial, strIdTransaccion, intIdOperacion,
                strNumeroTelefono, intIdOperadora, strIdTransaccionVerifica, strParametrosOperacion);


            Console.WriteLine(oResultadoMT.CodRet);
            Console.WriteLine(oResultadoMT.MensajeMT);
            Console.WriteLine(oResultadoMT.Fecha);
            Console.WriteLine(oResultadoMT.Hora);
        }
    }
}
