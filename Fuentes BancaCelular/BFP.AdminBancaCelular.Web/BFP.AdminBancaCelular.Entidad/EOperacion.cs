using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFP.AdminBancaCelular.Entidad
{
    public class EOperacion
    {
        /// <summary>
        /// Identificador de la operacion
        /// </summary>
        public int IdOperacion { get; set; }

        /// <summary>
        /// Descripcion de la operacion 
        /// </summary>
        public string DesOperacion { get; set; }

        /// <summary>
        /// Descripcion de los comandos permitidos 
        /// </summary>
        public string Comandos { get; set; }

        /// <summary>
        /// Omision del primer producto
        /// </summary>
        public bool OmisionParametro { get; set; }

        /// <summary>
        /// Fecha de creacion de la operacion
        /// </summary>
        public string FechaCreacion { get; set; }

        /// <summary>
        /// Programa que atendera la operacion
        /// </summary>
        public string ProgramaAS400 { get; set; }

        /// <summary>
        /// Indica si esta habilitado para el proceso batch
        /// </summary>
        public bool ProcesoBatch { get; set; }

        /// <summary>
        /// Indica si se encuentra habilitado para operar
        /// </summary>
        public bool Habilitado { get; set; }
    }
}
