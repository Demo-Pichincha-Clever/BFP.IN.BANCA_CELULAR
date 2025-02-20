using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Configuration;

namespace BFP.BancaCelular.Data
{
    /// <summary>
    /// Clase base de la capa de acceso a datos.
    /// </summary>
    public abstract class ADBase
    {
        /// <summary>
        /// Obtiene el objeto EjecutorBdd.
        /// </summary>
        private static Database dbBancaCelular;

        public static Database GetDatabase()
        {
            dbBancaCelular = DatabaseFactory.CreateDatabase("dbBancaCelular");
            return dbBancaCelular;
        }
    }
}
