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
            Utility.Utility.LogAction("Initializing timer and objects");

            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["Interval"]);
            timer.Enabled = true;
            string baseUri = ConfigurationManager.AppSettings["baseUri"],
                  apiToken = ConfigurationManager.AppSettings["apiToken"],
                 vendorKey = ConfigurationManager.AppSettings["vendorKey"];

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
                GetApplicationStatus();
                //ManageForgivenessData();
                // GetMessageFromSBAAsync();
                ManageForgivenessData();
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
                 string applicationStatus = forgivenessItem.ApplicationStatus;
                 string sbanumber = Convert.ToString(forgivenessItem.SbaLoanNumber);
                 //string error = forgivenessItem.Error;
                 //string slug = forgivenessItem.Slug;

                 SbaPPPLoanForgivenessStatusResponse sbaObj = await Methods.Methods.GetForgivenessRequestBySbaNumber(sbanumber, sbaLoanForgiveness);
                 string status = sbaObj.Status;
                 if (applicationStatus != status)
                 {
                     dataManagementObj.UpdateForgivenessDb(sbanumber, status);
                 }

             });
        }

        public void ManageForgivenessData()
        {

            DataManagement dataManagementObj = new DataManagement();
            var forgivenessDetails = dataManagementObj.GetForgivenessDetails();


            forgivenessDetails.ForEach(forgivenessItem =>
             {
                 string status = forgivenessItem.applicationStatus;
                 pppSbaNumber = Convert.ToString(forgivenessItem.sbaLoanNumber);

                 try
                 {
                     switch (status)
                     {
                         case "Awaiting":
                             UseCaseOne(forgivenessItem).Wait(); // submite and upload loan documents
                             break;
                         case "Resubmit":
                             UseCaseTwo(forgivenessItem).Wait();
                             break;
                     }
                 }
                 catch (Exception ex)
                 {
                     Utility.Utility.LogAction("Exception " + pppSbaNumber + ":" + ex.Message);

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

        private static async Task UseCaseOne(SbaForgiveness forgivenessItem)
        {

            var sbaForgivenessObj = await Methods.Methods.InvokeSbaLoanForgiveness(sbaLoanForgiveness, forgivenessItem);
            if (!string.IsNullOrEmpty(sbaForgivenessObj.slug))
            {
                pppSlug = sbaForgivenessObj.slug;
                pppSbaNumber = sbaForgivenessObj.etran_loan.sba_number;

                DataManagement dataManagementObj = new DataManagement();
                if (sbaForgivenessObj.etran_loan.ez_form)
                {
                    var documentDetail = dataManagementObj.GetForgivenessDocumentsEZ(pppSbaNumber);
                    if (!string.IsNullOrEmpty(documentDetail.PayrollAName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollAName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollAFile;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.PayrollBName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollBName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollBFile;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.PayrollCName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollCName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollCFile;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.PayrollDName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollDName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollDFile;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonPayrollCName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonPayrollCName;
                        sbaLoanDocuments.documentType = "20";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonPayrollCFile;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonPayrollBName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonPayrollBName;
                        sbaLoanDocuments.documentType = "21";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonPayrollBFile;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.CertifySalaryName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.CertifySalaryName;
                        sbaLoanDocuments.documentType = "16";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.CertifySalaryFile;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.EmployeeJobName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.EmployeeJobName;
                        sbaLoanDocuments.documentType = "15";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.EmployeeJobFile;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.CompanyOpsName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.CompanyOpsName;
                        sbaLoanDocuments.documentType = "13";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.CompanyOpsFile;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonPayrollAName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonPayrollAName;
                        sbaLoanDocuments.documentType = "8";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonPayrollAFile;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                }
                else
                {
                    // the docuemnt type need to be corrected with sreenivas !important
                    var documentDetail = dataManagementObj.GetForgivenessDocumentsFullApp(pppSbaNumber);
                    if (!string.IsNullOrEmpty(documentDetail.PayrollCompensationName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollCompensationName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollCompensationFile;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.PayrollTaxFormName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollTaxFormName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollTaxFormFile;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.PayrollPayementsName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollPayementsName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollPayementsFile;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.FTEDocumentationName1))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.FTEDocumentationName1;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.FTEDocumentFile1;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.FTEDocumentationName2))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.FTEDocumentationName2;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.FTEDocumentFile2;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.FTEDocumentationName3))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.FTEDocumentationName3;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.FTEDocumentFile3;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonpayrollName1))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonpayrollName1;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonpayrollFile1;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonpayrollName2))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonpayrollName2;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonpayrollFile2;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonpayrollName3))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonpayrollName3;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonpayrollFile3;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.AdditionalDocumentName1))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.AdditionalDocumentName1;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.AdditionalDocumentFile1;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.AdditionalDocumentName2))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.AdditionalDocumentName2;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.AdditionalDocumentFile2;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.AdditionalDocumentName3))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.AdditionalDocumentName3;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.AdditionalDocumentFile3;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.AdditionalDocumentName4))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.AdditionalDocumentName4;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.AdditionalDocumentFile4;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.CustomerSafteyFileName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.CustomerSafteyFileName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.CustomerSafteyFile;
                        await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                    }
                }

                foreach (var documentDetail in dataManagementObj.GetAdditionalDocuments(pppSbaNumber))
                {
                    sbaLoanDocuments.DocumentName = documentDetail.fileName;
                    sbaLoanDocuments.documentType = documentDetail.DocumentType;
                    sbaLoanDocuments.etranId = pppSlug;
                    sbaLoanDocuments.rawDocument = documentDetail.fileContent;
                    await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);
                }

                #region For reference
                //var documentObj = await Methods.Methods.GetDocumentTypes(sbaLoanForgiveness);

                //sbaLoanDocuments.DocumentName = "payroll.docx";
                //sbaLoanDocuments.documentType = "23";
                //sbaLoanDocuments.etranId = sbaForgivenessObj.slug;
                //sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                //await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);

                //sbaLoanDocuments.DocumentName = "payroll.docx";
                //sbaLoanDocuments.documentType = "23";
                //sbaLoanDocuments.etranId = sbaForgivenessObj.slug;
                //sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                //await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);

                //sbaLoanDocuments.DocumentName = "payroll.docx";
                //sbaLoanDocuments.documentType = "23";
                //sbaLoanDocuments.etranId = sbaForgivenessObj.slug;
                //sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                //await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);

                //sbaLoanDocuments.DocumentName = "payroll.docx";
                //sbaLoanDocuments.documentType = "21";
                //sbaLoanDocuments.etranId = sbaForgivenessObj.slug;
                //sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                //await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);

                //sbaLoanDocuments.DocumentName = "payroll.docx";
                //sbaLoanDocuments.documentType = "22";
                //sbaLoanDocuments.etranId = sbaForgivenessObj.slug;
                //sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                //await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);

                //sbaLoanDocuments.DocumentName = "payroll.docx";
                //sbaLoanDocuments.documentType = "8";
                //sbaLoanDocuments.etranId = sbaForgivenessObj.slug;
                //sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                //await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);

                //sbaLoanDocuments.DocumentName = "payroll.docx";
                //sbaLoanDocuments.documentType = "20";
                //sbaLoanDocuments.etranId = sbaForgivenessObj.slug;
                //sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                //await Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber);

                //sbaLoanDocuments.DocumentName = "payroll.docx";
                //sbaLoanDocuments.documentType = "15";
                //sbaLoanDocuments.etranId = "64bf0f5b-c088-4efe-99a7-bd65b73920ae";
                //sbaLoanDocuments.rawDocument = @"C:\Users\Vishnu\Desktop\payroll.docx";
                //await Methods.Methods.uploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber); 
                #endregion
            }
        }

        private static async Task UseCaseTwo(SbaForgiveness forgivenessItem)
        {
            SbaPPPLoanForgivenessStatusResponse sbaObj = await Methods.Methods.GetForgivenessRequestBySbaNumber(pppSbaNumber, sbaLoanForgiveness);
            if (sbaObj.Status == "Pending Validation")
            {

                // if (deleteStatasObj.status == "Deleted")
                //{

                bool deleteStatasObj = await Methods.Methods.DeleteSbaLoanForgiveness(sbaLoanForgiveness, pppSlug, pppSbaNumber);
                if (deleteStatasObj == true)
                {
                    await UseCaseOne(forgivenessItem);
                }

                //  }
                //else {
                //     Utility.Utility.LogAction($"{pppSbaNumber} :  {sbaObj.Status}. Deletion faiiled!");
                //  }
            }
            else
            {
                Utility.Utility.LogAction($"{sbaObj.Status}. So this applicataion request is not applicable for resubmission!");
            }

        }


        protected override void OnStop()
        {
            timer.Enabled = false;
        }
    }
}
