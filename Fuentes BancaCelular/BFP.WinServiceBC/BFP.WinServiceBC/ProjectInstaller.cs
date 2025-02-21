using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;


namespace BFP.WinServiceBC
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;

        public ProjectInstaller()
        {
            InitializeComponent();

            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "ServicioBancaCelular";

            serviceInstaller.DisplayName = "BFP.ServiceBancaCelular";
            serviceInstaller.ServiceName = "BFP.ServiceBancaCelular";

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
            
        }
    }
}
