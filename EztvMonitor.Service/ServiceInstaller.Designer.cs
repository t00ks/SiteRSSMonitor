namespace EztvMonitor.Service
{
    partial class ServiceInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.eztvServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.eztvServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.eztvEventLogInstaller = new System.Diagnostics.EventLogInstaller();
            // 
            // eztvServiceInstaller
            // 
            this.eztvServiceInstaller.Description = "Tooks-Net Eztv Monitor Service";
            this.eztvServiceInstaller.DisplayName = "EztvMonitorService";
            this.eztvServiceInstaller.ServiceName = "EztvMonitorService";
            // 
            // eztvServiceProcessInstaller
            // 
            this.eztvServiceProcessInstaller.Password = null;
            this.eztvServiceProcessInstaller.Username = null;
            // 
            // eztvEventLogInstaller
            // 
            this.eztvEventLogInstaller.CategoryCount = 0;
            this.eztvEventLogInstaller.CategoryResourceFile = null;
            this.eztvEventLogInstaller.Log = "EZTV";
            this.eztvEventLogInstaller.MessageResourceFile = null;
            this.eztvEventLogInstaller.ParameterResourceFile = null;
            this.eztvEventLogInstaller.Source = "EztvMonitorSvc";
            // 
            // ServiceInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.eztvServiceInstaller,
            this.eztvServiceProcessInstaller,
            this.eztvEventLogInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceInstaller eztvServiceInstaller;
        private System.ServiceProcess.ServiceProcessInstaller eztvServiceProcessInstaller;
        private System.Diagnostics.EventLogInstaller eztvEventLogInstaller;

    }
}