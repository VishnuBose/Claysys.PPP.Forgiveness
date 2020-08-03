using Claysys.PPP.Forgiveness.DAL;
using Claysys.PPP.Forgiveness.Model;
using Newtonsoft.Json;
using sbaCSharpClient;
using sbaCSharpClient.controller;
using sbaCSharpClient.domain;
using sbaCSharpClient.restclient;
using sbaCSharpClient.service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Claysys.PPP.Forgiveness
{
    public partial class PPPForgiveness : ServiceBase
    {
        Timer timer = new Timer();

        SbaLoanDocumentsController sbaLoanDocuments;

        SbaLoanForgivenessController sbaLoanForgiveness;

        SbaLoanForgivenessMessageController sbaLoanForgivenessMessageControllers;
        public PPPForgiveness()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();
            Utility.Utility.LogAction("Service started");
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["Interval"]);
            timer.Enabled = true;
            string baseUri = ConfigurationManager.AppSettings["baseUri"],
                  apiToken = ConfigurationManager.AppSettings["apiToken"],
                 vendorKey = ConfigurationManager.AppSettings["vendorKey"];

            sbaLoanDocuments = new SbaLoanDocumentsController(new SbaLoanDocumentService(new SbaRestApiClient(baseUri, apiToken, vendorKey)));
            sbaLoanForgiveness = new SbaLoanForgivenessController(new SbaLoanForgivenessService(new SbaRestApiClient(baseUri, apiToken, vendorKey)));
            sbaLoanForgivenessMessageControllers = new SbaLoanForgivenessMessageController(new SbaLoanForgivenessMessageService(new SbaRestApiClient(baseUri, apiToken, vendorKey)));

            ManageForgivenessData();
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                ManageForgivenessData();
            }
            catch (Exception ex)
            {
                Utility.Utility.LogAction("Failed to run service due to: " + ex.Message);
            }

        }

        private void ManageForgivenessData() {
            DataManagement dataManagementObj = new DataManagement();
            var forgivenessDetails = dataManagementObj.GetForgivenessDetails();
            forgivenessDetails.ForEach(async forgivenessItem =>
            {
                await Methods.InvokeSbaLoanForgiveness(sbaLoanForgiveness, forgivenessItem);
                //await submitLoanDocument(sbaLoanDocuments, forgivenessItem);
                // use case 1
            });
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
        }
    }
}
