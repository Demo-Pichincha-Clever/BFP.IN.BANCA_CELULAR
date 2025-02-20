using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test.BFP.ServiceBC;
using System.IO;
using System.Web;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBancaCelularClient serviceBC = new ServiceBancaCelularClient();
            UsuarioCredencial credencial = new UsuarioCredencial();
            string strGenero = string.Empty;
            //for (int i = 0; i < 5; i++)
            //{
            credencial.userName = "0D93C548B0DB6DBB";
            credencial.password = "78BC51938C6394E5";
            Random rd = new Random();
            //Numero  correlativo que  nos envia YP
            string strIdTransaccion =  rd.Next(0, 99999999).ToString().PadLeft(9, '0');  //"930000128"; 
            int intIdOperacion = 8; 
            //1	    Consulta de saldo	SALDO = SAL = S
            //2	    Consulta de Préstamo	CONSUL = CON  = C
            //3	    Consulta de Movimientos	MOVI = MOV = M
            //4	    Consulta Estado de cuenta TC	ESTADO = EST = E
            //5	    Pago de tarjeta de crédito 	PAGOTC = PAG = PA
            //6	    Pago de prestamos	PPREST = PPR = PP
            //7	    Recargas celulares	RECAR = REC = R
            //8	    Consulta de Ayuda	AYUDA = AYU = A
            //9	    Consulta de Verificación	VERIFICA
            //10    Transferencia entre cuentas TRANS = TRA = T
            //11    Consulta TC Efectivo Inmediato EFECTIV = EFI = EI
            //12	Consulta TC Consumo y efectivo DISPONI = DISP = D
            //13	Desafiliación canal BC DESAFIL = DES = DA

            //string strNumeroTelefono = "999745500";
            //string strNumeroTelefono = "989206861";
            string strNumeroTelefono = "954189547"; // Celular Afiliado
            int intIdOperadora = 1;
            string strIdTransaccionVerifica = "";
            string strParametrosOperacion = "AH01|10.1|SOL|CELMOVI"; // consulta estado cuenta
            strParametrosOperacion = "DES";
            //string strParametrosOperacion = "MASTER"; // consulta estado cuenta
            //Transferencia
            //string strParametrosOperacion = "AH02"; //Saldos
            //string strParametrosOperacion = "AH01"; //Movimientos
            //string strParametrosOperacion = ""; //Ayuda

            EResultadoMensajeMT oResultadoMT = serviceBC.OperacionBancaCelular(credencial, strIdTransaccion, intIdOperacion,
                strNumeroTelefono, intIdOperadora, strIdTransaccionVerifica, strParametrosOperacion);
            strGenero = oResultadoMT.MensajeMT;

            Console.WriteLine(oResultadoMT.CodRet);
            Console.WriteLine(oResultadoMT.MensajeMT);
            Console.WriteLine(oResultadoMT.Fecha);
            Console.WriteLine(oResultadoMT.Hora);
            //} 


            strGenero = "";
            //string cadena = "HolaññÑÁ";
            //string HResult_I, HResult_II;

            //string unicodeString = cadena.ToString();
            //Encoding ascii = Encoding.GetEncoding(437);
            //Encoding unicode = Encoding.Unicode;

            //byte[] unicodeBytes = unicode.GetBytes(unicodeString);
            //byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);


            //Stream s_II = new MemoryStream(asciiBytes);
            //Encoding encode_II = Encoding.GetEncoding("ISO-8859-1");
            //StreamReader readStream_II = new StreamReader(s_II, encode_II);
            //HResult_II = readStream_II.ReadLine();

        }
    }
}
