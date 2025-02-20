using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Timers;

using BFP.WinService.Negocio;
using BFP.WinService.Entidad;
using BFP.WinService.Comun;

namespace BFP.WinServiceBC
{
    public partial class WinServiceBC : ServiceBase
    {
        private System.Timers.Timer timer = null;
        private CCConfiguracion ConfiguracionServicio = null;
        public WinServiceBC()
        {
            InitializeComponent();
            double interval = 0;
            ConfiguracionServicio = NServiceMonitor.ObtenerConfiguracion();

            if (ConfiguracionServicio != null)
            {
                interval = Convert.ToDouble(ConfiguracionServicio.ServicioIntervaloTiempo);
                timer = new System.Timers.Timer(interval);
                //File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "obtuvo configuracion de la BD " + " \r\n");
            }
            else
            {
                string serviceInterval = ConfigurationManager.AppSettings["IntervaloTiempo"];
                ConfiguracionServicio = NServiceMonitor.DefaultConfig();
                File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "toma info por defecto " + " \r\n");
                try
                {
                    if (ConfiguracionServicio == null)
                    {
                        //File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "es null " + " \r\n");
                        interval = Convert.ToDouble(serviceInterval);
                    }
                    else { interval = Convert.ToDouble(ConfiguracionServicio.ServicioIntervaloTiempo); }
                }
                catch (Exception ex) { File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "excepcion " + ex.Message + " \r\n"); }
                timer = new System.Timers.Timer(interval);
            }
            timer.Elapsed += new ElapsedEventHandler(this.ServiceTimer_Tick);
            string[] ListaCel = ConfigurationManager.AppSettings["CelTest"].ToString().Split('|');
            string resultado;
            if (ListaCel.Length > 0)
            { resultado = ""; } //ObjMensaje.NroCelular = ListaCel[objrandow.Next(0, ListaCel.Length)]; //item["ODNroCel"].ToString().Trim();
            else
            { resultado = ""; }   // ObjMensaje.NroCelular = item["ODNroCel"].ToString().Trim();
            //ProcesarEnvio();
        }

        protected override void OnStart(string[] args)
        {
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
            File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "Se Inicio el servicio " + " \r\n");
            if (ConfiguracionServicio != null)
                NServiceMonitor.ActualizarDatosConfiguracion(ConfiguracionServicio.ServicioId, ConfiguracionServicio.StartId, ConfiguracionServicio.Servicio, DateTime.Now.ToShortDateString());
        }

        private void ServiceTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Enabled = true;
            this.timer.Start();
            File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + " Se Ejecuto ProcesarEnvio()" + " \r\n");
            ProcesarEnvio();
        }


        private void ProcesarEnvio()
        {
            int diastranscurridos = 0;
            int horastrancurridos = 0;
            bool indicadorActualiza = false;
            //decimal DateTime.Now.Ticks = DateTime.Now.Ticks;
            try
            {
                File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), string.Concat("[", DateTime.Now.ToString(), "]", "Petición  ", ConfiguracionServicio.ServicioBloqueado.ToString(), " \r\n"));
                if (ConfiguracionServicio.ServicioBloqueado == false)
                {
                    ConfiguracionServicio.ServicioBloqueado = true;
                    File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), string.Concat("[", DateTime.Now.ToString(), "]", "Ingresara al ForEach ", ConfiguracionServicio.ServicioBloqueado.ToString(), " \r\n"));
                    foreach (var item in ConfiguracionServicio.ProcesosNotificacion)
                    {
                        decimal diaprogramado00 = item.FechaProgramada.Ticks;
                        decimal diaprogramado23 = item.FechaProgramada.Add(new TimeSpan(23, 59, 59)).Ticks;
                        //DateTime.Now.Ticks = DateTime.Now.Ticks;

                        if (item.Estado == "Activo" && item.EstadoEjecucion == "Pendiente")
                        {
                            //File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), string.Concat(DateTime.Now.ToString(), "Inicio proceso Activo y pendiente ", item.Descripcion, "  ", timer.Interval.ToString()) + " \r\n");
                            int x = (int)System.Globalization.CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(DateTime.Now);
                            if (item.DiaBloqueados.Contains(x.ToString()) == item.HabilitarDia)
                            {
                                //File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "Dia habilitado " + x.ToString() + "-" + item.HabilitarDia + " \r\n");
                                if (DateTime.Now.Ticks >= diaprogramado00 && DateTime.Now.Ticks <= diaprogramado23)
                                {
                                    //File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "cumple condicion  del dia " + string.Concat(DateTime.Now.Ticks.ToString(), "-", diaprogramado00.ToString(), "-", DateTime.Now.Ticks.ToString(), "-", diaprogramado23.ToString()) + " \r\n");
                                    if (item.HoraInicio.Ticks <= DateTime.Now.TimeOfDay.Ticks && item.HoraFin.Ticks >= DateTime.Now.TimeOfDay.Ticks)
                                    {
                                        //File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "cumple condicion de rango de hora" + string.Concat(item.HoraInicio.Ticks.ToString(), "-", DateTime.Now.TimeOfDay.Ticks.ToString(), "-", item.HoraFin.Ticks.ToString(), "-", DateTime.Now.TimeOfDay.Ticks.ToString()) + " \r\n");
                                        if (item.HoraProgramada.Ticks <= DateTime.Now.TimeOfDay.Ticks)
                                        {
                                            File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "cumple todas las condicion " + string.Concat(item.HoraProgramada.Ticks.ToString(), "--", DateTime.Now.TimeOfDay.Ticks.ToString()) + " \r\n");
                                            if (string.Compare(item.Descripcion, "Envio SMS Afiliacion/Desafiliacion") == 0)
                                            {
                                                File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "*******Inicio proceso " + item.Descripcion + " \r\n");
                                                item.EstadoEjecucion = "En Proceso";
                                                //ConfiguracionServicio.ServicioBloqueado = true;
                                                EnvioMensajesEmailAfiliacionDesafiliacion();
                                                item.HoraProgramada = item.HoraProgramada.Add(new TimeSpan(0, 0, (int)((item.Segundos / 1000) / 60)));
                                                NServiceMonitor.ActualizarDatosConfiguracion(item.CodigoTabla, item.Ejecutar[1].CodigoTabla, item.CodigoProceso.ToString(), item.HoraProgramada.ToString());
                                                item.EstadoEjecucion = "Pendiente";
                                                File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "*******Fin proceso " + item.Descripcion + " \r\n");
                                                //ConfiguracionServicio.ServicioBloqueado = false;
                                                indicadorActualiza = true;
                                            }
                                            if (string.Compare(item.Descripcion, "Envio SMS Notificacion") == 0)
                                            {
                                                //ConfiguracionServicio.ServicioBloqueado = true;
                                                File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "*******Inicio proceso " + item.Descripcion + " \r\n");
                                                item.EstadoEjecucion = "En Proceso";
                                                string strFechaEnvio = string.Concat(item.FechaProgramada.Year.ToString().PadLeft(4, '0'), item.FechaProgramada.Month.ToString().PadLeft(2, '0'), item.FechaProgramada.Day.ToString().PadLeft(2, '0'));
                                                string strHora = item.HoraProgramada.ToString();
                                                NServiceMonitor.ProcesarPreConsultaNotificaciones(strFechaEnvio, strHora);
                                                item.HoraProgramada = item.HoraProgramada.Add(new TimeSpan(0, 0, (int)((item.Segundos / 1000) / 60)));
                                                NServiceMonitor.ActualizarDatosConfiguracion(item.CodigoTabla, item.Ejecutar[1].CodigoTabla, item.CodigoProceso.ToString(), item.HoraProgramada.ToString());
                                                item.EstadoEjecucion = "Pendiente";
                                                File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "******Fin proceso " + item.Descripcion + " \r\n");
                                                //ConfiguracionServicio.ServicioBloqueado = false;
                                                indicadorActualiza = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), string.Concat("[", DateTime.Now.ToString(), "]", " No cumple Rango de hora: ", item.HoraInicio.ToString(), " entre ", item.HoraFin.ToString(), "Evalua: ", DateTime.Now.TimeOfDay.ToString(), " \r\n"));
                                        if (DateTime.Now.TimeOfDay.Ticks > item.HoraFin.Ticks)
                                        {
                                            NServiceMonitor.ActualizarDatosConfiguracion(item.CodigoTabla, item.Ejecutar[0].CodigoTabla, item.CodigoProceso.ToString(), item.FechaProgramada.AddDays(1).ToShortDateString());
                                            NServiceMonitor.ActualizarDatosConfiguracion(item.CodigoTabla, item.Ejecutar[1].CodigoTabla, item.CodigoProceso.ToString(), item.HoraInicio.ToString());
                                            File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), string.Concat("[", DateTime.Now.ToString(), "]", "Se actualiza la Fecha y hora : ", item.FechaProgramada.AddDays(1).ToShortDateString(), " hora Inicio ", item.HoraInicio.ToString(), " \r\n"));
                                            indicadorActualiza = true;
                                        }
                                    }
                                }
                                else
                                {
                                    File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), string.Concat("[", DateTime.Now.ToString(), "]", "fuera de rango de fecha ", item.FechaProgramada.ToString(), DateTime.Now.ToShortDateString().ToString(), " \r\n"));
                                    if (diaprogramado00 < DateTime.Now.Ticks && diaprogramado23 < DateTime.Now.Ticks)
                                    {
                                        NServiceMonitor.ActualizarDatosConfiguracion(item.CodigoTabla, item.Ejecutar[0].CodigoTabla, item.CodigoProceso.ToString(), DateTime.Now.ToShortDateString());
                                        NServiceMonitor.ActualizarDatosConfiguracion(item.CodigoTabla, item.Ejecutar[1].CodigoTabla, item.CodigoProceso.ToString(), item.HoraInicio.ToString());
                                        File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), string.Concat("[", DateTime.Now.ToString(), "]", "Se actualizo Fecha ", DateTime.Now.ToShortDateString(), item.HoraFin.ToString(), " \r\n"));
                                        indicadorActualiza = true;
                                    }
                                }
                            }
                            else
                            {
                                File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), string.Concat("[", DateTime.Now.ToString(), "] ", item.Descripcion.ToString(), " No cumple la condicion de dia de la semama", x.ToString(), " - ", DateTime.Now.DayOfWeek.ToString(), " - ", DateTime.Now, " \r\n"));
                                if (item.FechaProgramada.Ticks < DateTime.Now.Ticks) 
                                {
                                    NServiceMonitor.ActualizarDatosConfiguracion(item.CodigoTabla, item.Ejecutar[0].CodigoTabla, item.CodigoProceso.ToString(), DateTime.Now.AddDays(1).ToShortDateString());
                                    NServiceMonitor.ActualizarDatosConfiguracion(item.CodigoTabla, item.Ejecutar[1].CodigoTabla, item.CodigoProceso.ToString(), item.HoraInicio.ToString());
                                    indicadorActualiza = true;
                                    File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), string.Concat("[", DateTime.Now.ToString(), "] ", item.Descripcion.ToString(), " No cumple dia de semana Se actualiza Fecha ", DateTime.Now.AddDays(1).ToShortDateString(), item.HoraInicio.ToString(), " \r\n"));
                                }
                            }
                        }
                        else
                        {
                            File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), string.Concat("[", DateTime.Now.ToString(), "]", "Dentro Foreach no cumple estado ", item.EstadoEjecucion.ToString(), " \r\n"));
                        }
                    }
                    File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "******Salio del ForEach." + " \r\n");
                    if (indicadorActualiza)
                    {
                        File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "Actualiza Objeto ConfiguracionServicio - " + DateTime.Now.ToString() + " \r\n");
                        ConfiguracionServicio = NServiceMonitor.ObtenerConfiguracion();
                        indicadorActualiza = false;
                    }

                    ConfiguracionServicio.ServicioBloqueado = false;
                }
                else
                {
                    File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "Servicio Bloqueado  " + " \r\n");
                }


            }
            catch (Exception ex)
            {
                File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + " - " + ex.Message + " \r\n");
                ConfiguracionServicio.ServicioBloqueado = false;
                indicadorActualiza = false;
                EventLog.WriteEntry("Error Banca Celular! " + ex.Message,
                        System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private void EnvioMensajesEmailAfiliacionDesafiliacion()
        {
            List<ELogMensajeria> lstLogMensajeria = null;

            lstLogMensajeria = NServiceMonitor.ProcesaEnvioMensajeria();

            if (lstLogMensajeria.Count > 0)
            {
                File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + "LogWinServiceBancaCelular.txt", "Inició la consulta - " + DateTime.Now.ToString() + " \r\n");

                int envioEmail_Exito = (from c in lstLogMensajeria
                                        where c.Ident_Tipo_Mensaje == CConstantes.TipoMensaje.EMAIL
                                        && c.Estado_Envio_Mensaje == true
                                        select c).Count();

                if (envioEmail_Exito > 0)
                    File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + "LogWinServiceBancaCelular.txt", "Mensajes email enviados " + envioEmail_Exito.ToString() + " \r\n");

                int envioEmail_SinExito = (from c in lstLogMensajeria
                                           where c.Ident_Tipo_Mensaje == CConstantes.TipoMensaje.EMAIL
                                           && c.Estado_Envio_Mensaje == false
                                           select c).Count();

                if (envioEmail_SinExito > 0)
                    File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + "LogWinServiceBancaCelular.txt", "Mensajes email no enviados " + envioEmail_SinExito.ToString() + " \r\n");

                int envioSMS_Exito = (from c in lstLogMensajeria
                                      where c.Ident_Tipo_Mensaje == CConstantes.TipoMensaje.SMS
                                      && c.Estado_Envio_Mensaje == true
                                      && c.Nro_Celular_Envio != string.Empty
                                      select c).Count();

                if (envioSMS_Exito > 0)
                    File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + "LogWinServiceBancaCelular.txt", "Mensajes SMS enviados " + envioSMS_Exito.ToString() + " \r\n");

                int envioSMS_SinExito = (from c in lstLogMensajeria
                                         where c.Ident_Tipo_Mensaje == CConstantes.TipoMensaje.SMS
                                         && c.Estado_Envio_Mensaje == false
                                         && c.Nro_Celular_Envio != string.Empty
                                         select c).Count();

                if (envioSMS_SinExito > 0)
                    File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + "LogWinServiceBancaCelular.txt", "Mensajes SMS no enviados " + envioSMS_SinExito.ToString() + " \r\n");


                File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + "LogWinServiceBancaCelular.txt", "Finalizó la consulta - " + DateTime.Now.ToString() + " \r\n");
            }

            lstLogMensajeria = null;
        }

        protected override void OnStop()
        {
            timer.AutoReset = false;
            timer.Enabled = false;
            File.AppendAllText(ConfigurationManager.AppSettings["LOG"] + string.Concat("LogWinServiceBC", DateTime.Now.ToShortDateString().ToString().Replace("/", "").Replace("-", ""), ".txt"), DateTime.Now.ToString() + "Se detuvo el servicio ");
            if (ConfiguracionServicio != null)
                NServiceMonitor.ActualizarDatosConfiguracion(ConfiguracionServicio.ServicioId, ConfiguracionServicio.StopId, ConfiguracionServicio.Servicio, DateTime.Now.ToShortDateString());
        }
    }
}

