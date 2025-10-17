namespace B2BMessagesMonitoringService
{
    partial class ProjectInstaller
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
            this.B2BMonitoringServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.B2BMonitoringServiceIntaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // B2BMonitoringServiceProcessInstaller
            // 
            this.B2BMonitoringServiceProcessInstaller.Password = null;
            this.B2BMonitoringServiceProcessInstaller.Username = null;
            // 
            // B2BMonitoringServiceIntaller
            // 
            this.B2BMonitoringServiceIntaller.Description = "Monitors B2B messages communicated with SAP";
            this.B2BMonitoringServiceIntaller.DisplayName = "B2B Messages Monitoring Service";
            this.B2BMonitoringServiceIntaller.ServiceName = "B2B Messages Monitoring Service";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.B2BMonitoringServiceProcessInstaller,
            this.B2BMonitoringServiceIntaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller B2BMonitoringServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller B2BMonitoringServiceIntaller;
    }
}