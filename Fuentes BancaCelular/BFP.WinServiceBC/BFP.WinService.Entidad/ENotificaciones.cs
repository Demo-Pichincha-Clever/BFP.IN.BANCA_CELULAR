﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFP.WinService.Entidad
{
    public class ENotificaciones
    {
        /// <summary>
        /// Numero del celular del cliente
        /// </summary>
        public string NroCelular { get; set; }

        /// <summary>   
        /// Email del cliente
        /// </summary>
        public string Email { get; set; }

        /// <summary>   
        /// Canal de emision: HB-HomeBanking, VT-Ventanilla, PL-Plataforma, BT-Banca Telefonica ,BC Banca Celular
        /// /// </summary>
        public string Canal { get; set; }

        /// <summary>
        /// Indica si el mensaje fue enviado
        /// </summary>
        public bool EnvioSMS { get; set; }

        /// <summary>
        /// Indica si el mensaje fue enviado
        /// </summary>
        public bool EnvioEmail { get; set; }

        /// <summary>
        /// Indica si el mensaje fue enviado
        /// </summary>
        public string CodigoOperadora { get; set; }

        /// <summary>
        /// Mensaje a enviar
        /// </summary>
        public string Mensaje { get; set; }

        /// <summary>
        /// Codigo IBS Mensaje
        /// </summary>
        public string MensajeId { get; set; }
    }
}
