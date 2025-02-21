using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BFP.WinService.Comun
{
    public class CCConfiguracion
    {
        private int intServicioId;
        private int intStartId;
        private int intStopId;
        private bool booServicioBloqueado;
        private DateTime strServicioInicio;
        private DateTime strServicioFin;
        private int intServicioIntervaTiempo;
        private CCProcesos[] ObjProcesosNotificacion;
        public string Servicio { get { return "SERWIN"; } }
        public string Procesos { get { return "PROENV"; } }
        public int ServicioId { get { return intServicioId; } }
        public int StartId { get { return intStartId; } }
        public int StopId { get { return intStopId; } }
        public bool ServicioBloqueado { get { return booServicioBloqueado; } set { booServicioBloqueado = value; } }

        public CCConfiguracion(DataSet ObjData)
        {
            if (ObjData != null)
            {
                DataRow[] Objdatarows = ObjData.Tables[0].Select("Cod_Campo = 'PROENV' ");
                ObjProcesosNotificacion = new CCProcesos[Objdatarows.Length];
                int IndiceProcesos = -1;
                int Indicedetalle = -1;
                foreach (DataRow item in ObjData.Tables[0].Rows)
                {
                    intServicioId = int.Parse(item["Ident_Tabla"].ToString());
                    if (string.Compare(item["Cod_Campo"].ToString(), Servicio) == 0 && string.Compare(item["Descm_Campo"].ToString(), "OnStop Servicio") == 0)
                    { strServicioInicio = DateTime.Parse(item["Desc_Campo"].ToString()); intStopId = int.Parse(item["Ident_Campo"].ToString()); }
                    if (string.Compare(item["Cod_Campo"].ToString(), Servicio) == 0 && string.Compare(item["Descm_Campo"].ToString(), "OnStart Servicio") == 0)
                    { strServicioFin = DateTime.Parse(item["Desc_Campo"].ToString()); intStartId = int.Parse(item["Ident_Campo"].ToString()); }
                    if (string.Compare(item["Cod_Campo"].ToString(), Servicio) == 0 && string.Compare(item["Descm_Campo"].ToString(), "OnStart Interval Servicio") == 0)
                        intServicioIntervaTiempo = (int)(int.Parse(item["Cant_Campo"].ToString()) * decimal.Parse(item["Valor_Campo"].ToString()));

                    if (string.Compare(item["Cod_Campo"].ToString(), Procesos) == 0)
                    {
                        IndiceProcesos++;
                        ObjProcesosNotificacion[IndiceProcesos] = new CCProcesos();
                        ObjProcesosNotificacion[IndiceProcesos].CodigoTabla = int.Parse(item["Ident_Tabla"].ToString());
                        ObjProcesosNotificacion[IndiceProcesos].Descripcion = item["Descm_Campo"].ToString();
                        ObjProcesosNotificacion[IndiceProcesos].CodigoProceso = int.Parse(item["Ident_campo"].ToString());
                        ObjProcesosNotificacion[IndiceProcesos].EstadoEjecucion = "Pendiente";
                        ObjProcesosNotificacion[IndiceProcesos].Dias = int.Parse(item["Cant_Campo"].ToString());
                        ObjProcesosNotificacion[IndiceProcesos].Segundos = decimal.Parse(item["Valor_Campo"].ToString());
                        ObjProcesosNotificacion[IndiceProcesos].Descripcion = item["Descm_Campo"].ToString();
                        ObjProcesosNotificacion[IndiceProcesos].Estado = "Activo";
                        DataRow[] Objdatarowssub = ObjData.Tables[0].Select("Cod_Campo= '" + item["Ident_campo"].ToString() + "'");
                        ObjProcesosNotificacion[IndiceProcesos].Ejecutar = new CCEjecucion[Objdatarowssub.Length];
                        Indicedetalle = -1;
                        foreach (var rowsident in Objdatarowssub)
                        {
                            Indicedetalle++;
                            ObjProcesosNotificacion[IndiceProcesos].Ejecutar[Indicedetalle] = new CCEjecucion();
                            ObjProcesosNotificacion[IndiceProcesos].Ejecutar[Indicedetalle].CodigoTabla = int.Parse(rowsident["Ident_Campo"].ToString());
                            ObjProcesosNotificacion[IndiceProcesos].Ejecutar[Indicedetalle].Campo = rowsident["Descm_Campo"].ToString();
                            ObjProcesosNotificacion[IndiceProcesos].Ejecutar[Indicedetalle].Valor1 = rowsident["Desc_Campo"].ToString();
                            ObjProcesosNotificacion[IndiceProcesos].Ejecutar[Indicedetalle].Valor2 = rowsident["Cant_Campo"].ToString();
                            ObjProcesosNotificacion[IndiceProcesos].Ejecutar[Indicedetalle].Valor3 = rowsident["Valor_Campo"].ToString();
                            if (ObjProcesosNotificacion[IndiceProcesos].Ejecutar[Indicedetalle].Campo == "Fecha Programada")
                            {
                                ObjProcesosNotificacion[IndiceProcesos].FechaProgramada = DateTime.Parse(string.Format("{0} {1}", rowsident["Desc_Campo"].ToString(), "00:00:00"));
                            }
                            if (ObjProcesosNotificacion[IndiceProcesos].Ejecutar[Indicedetalle].Campo == "Hora Programada")
                            {
                                ObjProcesosNotificacion[IndiceProcesos].HoraProgramada = DateTime.Parse(rowsident["Desc_Campo"].ToString()).TimeOfDay;
                                ObjProcesosNotificacion[IndiceProcesos].HoraInicio = DateTime.MinValue.AddHours(double.Parse(rowsident["Cant_Campo"].ToString())).TimeOfDay;  //DateTime.Parse(rowsident["Desc_Campo"].ToString()).TimeOfDay;
                                ObjProcesosNotificacion[IndiceProcesos].HoraFin = DateTime.MinValue.AddHours(double.Parse(rowsident["Valor_Campo"].ToString())).TimeOfDay;
                            }
                            if (ObjProcesosNotificacion[IndiceProcesos].Ejecutar[Indicedetalle].Campo == "Habilitar/Desabilitar")
                            {
                                ObjProcesosNotificacion[IndiceProcesos].DiaBloqueados = rowsident["Desc_Campo"].ToString().Split('|');
                                ObjProcesosNotificacion[IndiceProcesos].HabilitarDia = rowsident["Cant_Campo"].ToString() == "1" ? true : false;

                            }
                        }
                    }
                }
                ServicioBloqueado = false;
            }

        }

        public DateTime ServicioInicio
        {
            get { return strServicioInicio; }
        }

        public DateTime ServicioFin
        {
            get { return strServicioFin; }
        }
        public int ServicioIntervaloTiempo
        {
            get { return intServicioIntervaTiempo; }
        }

        public CCProcesos[] ProcesosNotificacion
        {
            get { return ObjProcesosNotificacion; }
        }
    }

    public class CCProcesos
    {
        private string strDescripion;
        private int intCodigoProceso;
        private int intDias;
        private decimal intSegundos;
        private string strEstado;
        private CCEjecucion[] ObjEjecucion;
        private string strEstadoEjecucion;
        private DateTime strFechaProgramada;
        private TimeSpan strHoraProgramada;
        private TimeSpan tmsHoraFin;
        private TimeSpan tmsHoraInicio;
        private int intCodigoTabla;
        private string[] intDiaBloqueados;
        private bool booHabilitarDia;
        public int CodigoTabla
        {
            get { return intCodigoTabla; }
            set { intCodigoTabla = value; }
        }

        public string[] DiaBloqueados
        {
            get { return intDiaBloqueados; }
            set { intDiaBloqueados = value; }
        }

        public bool HabilitarDia
        {
            get { return booHabilitarDia; }
            set { booHabilitarDia = value; }
        }


        public int Dias
        {
            get { return intDias; }
            set { intDias = value; }

        }
        public decimal Segundos
        {
            get { return intSegundos; }
            set { intSegundos = value; }

        }
        public int CodigoProceso
        {
            get { return intCodigoProceso; }
            set { intCodigoProceso = value; }

        }


        public string Descripcion
        {
            get { return strDescripion; }
            set { strDescripion = value; }
        }

        public string Estado
        {
            get { return strEstado; }
            set { strEstado = value; }
        }

        public CCEjecucion[] Ejecutar
        {
            get { return ObjEjecucion; }
            set { ObjEjecucion = value; }
        }

        public string EstadoEjecucion
        {
            get { return strEstadoEjecucion; }
            set { strEstadoEjecucion = value; }
        }

        public DateTime FechaProgramada
        {
            get { return strFechaProgramada; }
            set { strFechaProgramada = value; }
        }

        public TimeSpan HoraProgramada
        {
            get { return strHoraProgramada; }
            set { strHoraProgramada = value; }
        }

        public TimeSpan HoraInicio
        {
            get { return tmsHoraInicio; }
            set { tmsHoraInicio = value; }
        }
        public TimeSpan HoraFin
        {
            get { return tmsHoraFin; }
            set { tmsHoraFin = value; }
        }
    }

    public class CCEjecucion
    {
        private int intCodigoTabla;
        private string strCampo;
        private string strValor1;
        private string strValor2;
        private string strValor3;

        public int CodigoTabla
        {
            get { return intCodigoTabla; }
            set { intCodigoTabla = value; }
        }
        public string Campo
        {
            get { return strCampo; }
            set { strCampo = value; }
        }
        public string Valor1
        {
            get { return strValor1; }
            set { strValor1 = value; }
        }

        public string Valor2
        {
            get { return strValor2; }
            set { strValor2 = value; }
        }

        public string Valor3
        {
            get { return strValor3; }
            set { strValor3 = value; }
        }


    }
}
