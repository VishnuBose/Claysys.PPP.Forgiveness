using Claysys.PPP.Forgiveness.DAL;
using Claysys.PPP.Forgiveness.Model;
using Newtonsoft.Json;
using Claysys.PPP.Forgiveness.Methods;
using Claysys.PPP.Forgiveness.controller;
using Claysys.PPP.Forgiveness.domain;
using Claysys.PPP.Forgiveness.restclient;
using Claysys.PPP.Forgiveness.service;
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
using RestSharp;
using Claysys.PPP.Forgiveness.Utility;
using System.Runtime.InteropServices;

namespace Claysys.PPP.Forgiveness
{
    public partial class PPPForgiveness : ServiceBase
    {
        Timer timer = new Timer();

        public static string pppLoanDocumentTypes,
           pppLoanForgivenessRequests,
           pppLoanDocuments,
           pppLoanForgivenessMessageReply,
           pppLoanForgivenessMessages,
           pppSlug, pppSbaNumber;

        public static SbaLoanDocumentsController sbaLoanDocuments;

        public static SbaLoanForgivenessController sbaLoanForgiveness;

        public static SbaLoanForgivenessMessageController sbaLoanForgivenessMessageControllers;

        public PPPForgiveness()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();
            Utility.Utility.LogAction("Service started");
            Init();
            //GetMessageFromSBAAsync();
            //ManageForgivenessData();
            //GetApplicationStatus();
            //ManageForgivenessDataForMDL();
            //uploadForgivenessDocument(sbaLoanForgiveness);

        }

        public void Init()
        {
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["Interval"]);
            timer.Enabled = true;
            string baseUri = ConfigurationManager.AppSettings["baseUri"],
                  apiToken = ConfigurationManager.AppSettings["apiTokenMDL"],
                 vendorKey = ConfigurationManager.AppSettings["vendorKeyMDL"];

            sbaLoanDocuments = new SbaLoanDocumentsController(new SbaLoanDocumentService(new SbaRestApiClient(baseUri, apiToken, vendorKey)));
            sbaLoanForgiveness = new SbaLoanForgivenessController(new SbaLoanForgivenessService(new SbaRestApiClient(baseUri, apiToken, vendorKey)));
            sbaLoanForgivenessMessageControllers = new SbaLoanForgivenessMessageController(new SbaLoanForgivenessMessageService(new SbaRestApiClient(baseUri, apiToken, vendorKey)));
            //GetApplicationStatus();
            ManageForgivenessData();
        }


        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                Utility.Utility.LogAction("Method calling from on elapsed time in ForgivenessService ");
                //GetApplicationStatus();
                //ManageForgivenessData();
                // GetMessageFromSBAAsync();
                //ManageForgivenessDataForMDL();
                // uploadForgivenessDocument(sbaLoanForgiveness);
            }
            catch (Exception ex)
            {
                Utility.Utility.LogAction("Failed to run service due to: " + ex.Message);
            }

        }

        private async Task GetMessageFromSBAAsync()
        {
            await Methods.Methods.getForgivenessMessagesBySbaNumber(sbaLoanForgivenessMessageControllers, 1, "2922100006", true);
        }

        public void GetApplicationStatus()
        {
            DataManagement dataManagementObj = new DataManagement();
            var forgivenessDetails = dataManagementObj.SelectApplicationStatus();
            forgivenessDetails.ForEach(async forgivenessItem =>
             {
                 string applicationStatus = forgivenessItem.applicationStatus;
                 string sbanumber = Convert.ToString(forgivenessItem.sbaLoanNumber);
                 string error = forgivenessItem.error;
                 string slug = forgivenessItem.slug;

                 SbaPPPLoanForgivenessStatusResponse sbaObj = await Methods.Methods.GetForgivenessRequestBySbaNumber(sbanumber, sbaLoanForgiveness);
                 string status = sbaObj.Status;
                 if (applicationStatus != status)
                 {
                   dataManagementObj.UpdateForgivenessDb(sbanumber, status, error, slug);
                 }

             });
        }

        public void ManageForgivenessData()
        {

            DataManagement dataManagementObj = new DataManagement();
            var forgivenessDetails = dataManagementObj.GetForgivenessDetails();
            forgivenessDetails.ForEach(async forgivenessItem =>
             {
                 string status = forgivenessItem.applicationStatus;
                 pppSbaNumber = Convert.ToString(forgivenessItem.sbaLoanNumber);

                 try
                 {
                     switch (status)
                     {
                         case "Awaiting":
                             await useCaseOne(forgivenessItem); // submite and upload loan documents
                             break;
                         case "Resubmit":
                             SbaPPPLoanForgivenessStatusResponse sbaObj = await Methods.Methods.GetForgivenessRequestBySbaNumber(pppSbaNumber, sbaLoanForgiveness);
                             if (sbaObj.Status == "Pending Validation")
                             {

                                 // if (deleteStatasObj.status == "Deleted")
                                 //{

                                 var deleteStatasObj = await Methods.Methods.deleteSbaLoanForgiveness(sbaLoanForgiveness, pppSlug, pppSbaNumber);

                                 await useCaseOne(forgivenessItem);
                                 //  }
                                 //else {
                                 //     Utility.Utility.LogAction($"{pppSbaNumber} :  {sbaObj.Status}. Deletion faiiled!");
                                 //  }
                             }
                             else
                             {
                                 Utility.Utility.LogAction($"{sbaObj.Status}. So this applicataion request is not applicable for resubmission!");
                             }

                             break;

                     }
                 }
                 catch (Exception ex)
                 {
                     Utility.Utility.LogAction("Exception "+pppSbaNumber+":" + ex.Message);

                 }



                 // use case 3

                 //await Methods.Methods.getAllForgivenessRequests(sbaLoanForgiveness);
                 //await Methods.Methods.getSbaLoanForgivenessBySlug(sbaLoanForgiveness,pppSlug);
                 //await Methods.Methods.getForgivenessRequestBySbaNumber(sbaLoanForgiveness,pppSbaNumber);
                 //await Methods.Methods.deleteSbaLoanForgiveness(sbaLoanForgiveness, pppSlug);

                 //use case 4

                 // await Methods.Methods.getDocumentTypes(sbaLoanForgiveness);

                 //use case 5

                 // await Methods.Methods.getForgivenessMessagesBySbaNumber(sbaLoanForgivenessMessageControllers,1,pppSbaNumber,true);
             });
        }

        private static async Task useCaseOne(SbaForgiveness forgivenessItem)
        {
            var sbaForgivenessObj = await Methods.Methods.InvokeSbaLoanForgiveness(sbaLoanForgiveness, forgivenessItem);
            if (!string.IsNullOrEmpty(sbaForgivenessObj.slug))
            {
                pppSlug = sbaForgivenessObj.slug;
                pppSbaNumber = sbaForgivenessObj.etran_loan.sba_number;
                //var documentObj = await Methods.Methods.getDocumentTypes(sbaLoanForgiveness);

                sbaLoanDocuments.DocumentName = "payroll.docx";
                sbaLoanDocuments.documentType = "23";
                sbaLoanDocuments.etranId = sbaForgivenessObj.slug;
                sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                await Methods.Methods.uploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);

                sbaLoanDocuments.DocumentName = "payroll.docx";
                sbaLoanDocuments.documentType = "23";
                sbaLoanDocuments.etranId = sbaForgivenessObj.slug;
                sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                await Methods.Methods.uploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);

                sbaLoanDocuments.DocumentName = "payroll.docx";
                sbaLoanDocuments.documentType = "23";
                sbaLoanDocuments.etranId = sbaForgivenessObj.slug;
                sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                await Methods.Methods.uploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);

                sbaLoanDocuments.DocumentName = "payroll.docx";
                sbaLoanDocuments.documentType = "21";
                sbaLoanDocuments.etranId = sbaForgivenessObj.slug;
                sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                await Methods.Methods.uploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);

                sbaLoanDocuments.DocumentName = "payroll.docx";
                sbaLoanDocuments.documentType = "22";
                sbaLoanDocuments.etranId = sbaForgivenessObj.slug;
                sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                await Methods.Methods.uploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);

                sbaLoanDocuments.DocumentName = "payroll.docx";
                sbaLoanDocuments.documentType = "8";
                sbaLoanDocuments.etranId = sbaForgivenessObj.slug;
                sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                await Methods.Methods.uploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);

                sbaLoanDocuments.DocumentName = "payroll.docx";
                sbaLoanDocuments.documentType = "20";
                sbaLoanDocuments.etranId = sbaForgivenessObj.slug;
                sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                await Methods.Methods.uploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);

                //sbaLoanDocuments.DocumentName = "payroll.docx";
                //sbaLoanDocuments.documentType = "15";
                //sbaLoanDocuments.etranId = "64bf0f5b-c088-4efe-99a7-bd65b73920ae";
                //sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                //await Methods.Methods.uploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
            }
        }




        protected override void OnStop()
        {
            timer.Enabled = false;
        }
    }
}
