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
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;

namespace Claysys.PPP.Forgiveness
{
    public partial class PPPForgiveness : ServiceBase
    {
        Timer updateStatusTimer = new Timer();
        Timer requestSubmissionTimer = new Timer();
        public static string pppLoanDocumentTypes,
           pppLoanForgivenessRequests,
           pppLoanDocuments,
           pppLoanForgivenessMessageReply,
           pppLoanForgivenessMessages,
           pppSlug, pppSbaNumber;

        public static SbaLoanDocumentsController sbaLoanDocuments;

        public static SbaLoanForgivenessController sbaLoanForgiveness;

        public static SbaLoanForgivenessMessageController sbaLoanForgivenessMessageControllers;

        public static DataManagement dataManagementObj = new DataManagement();


        public PPPForgiveness()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // System.Diagnostics.Debugger.Launch();
            Utility.Utility.LogAction("Service started");
            updateStatusTimer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            updateStatusTimer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["Interval"]);
            updateStatusTimer.Enabled = true;


            requestSubmissionTimer.Elapsed += new ElapsedEventHandler(OnElapsedTimeForDataSubmission);
            requestSubmissionTimer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["ApiSubmissionInterval"]);
            requestSubmissionTimer.Enabled = true;

            Init();
            //GetMessageFromSBAAsync();
            //ManageForgivenessData();
            //GetApplicationStatus();
            //ManageForgivenessDataForMDL();
            //uploadForgivenessDocument(sbaLoanForgiveness);

        }

        public void UploadExtraDocumentsToSubmitedApplication()
        {
            foreach (var documentDetail in dataManagementObj.GetExtraDocuments())
            {
                sbaLoanDocuments.DocumentName = documentDetail.fileName;
                sbaLoanDocuments.documentType = documentDetail.DocumentType;
                sbaLoanDocuments.etranId = documentDetail.SlugID;
                sbaLoanDocuments.rawDocument = documentDetail.fileContent;
                Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, documentDetail.SBALoanNo, documentDetail.SlugID);
            }
        }

        public void Init()
        {
            Utility.Utility.LogAction("Initializing timer and objects");
            SubmitCreditUnionApplications();
            //UpdateCreditUnionApplicationStatus();
        }

        private void UpdateCreditUnionApplicationStatus()
        {
            //List<CreditUnionData> cuDataCollection = dataManagementObj.GetCreditUnionDetails();
            //cuDataCollection.ForEach((cuData) =>
            //{
            //    Utility.Utility.LogAction("CU - " + cuData.CUName + " Connection open for submission.");
            //    string baseUri = ConfigurationManager.AppSettings["baseUri"],
            //    apiToken = cuData.Token,
            //    vendorKey = cuData.Vendorkey;
            //    dataManagementObj.connectionString = cuData.ConnectionString;
            //    dataManagementObj.cuName = cuData.CUName;
            //    sbaLoanDocuments = new SbaLoanDocumentsController(new SbaLoanDocumentService(new SbaRestApiClient(baseUri, apiToken, vendorKey)));
            //    sbaLoanForgiveness = new SbaLoanForgivenessController(new SbaLoanForgivenessService(new SbaRestApiClient(baseUri, apiToken, vendorKey)));
            //    sbaLoanForgivenessMessageControllers = new SbaLoanForgivenessMessageController(new SbaLoanForgivenessMessageService(new SbaRestApiClient(baseUri, apiToken, vendorKey)));



            //    GetApplicationStatus();
            //    ForgivessPaymentUpdate(cuData.ConnectionString);
            //    //GetMessageFromSBAAsync();
            //    //SBAGeneratedDocuments();
            //    //ValidateDisbursedLoan();
            //});
        }


        private void SubmitCreditUnionApplications()
        {
            List<CreditUnionData> cuDataCollection = dataManagementObj.GetCreditUnionDetails();
            cuDataCollection.ForEach((cuData) =>
            {
                Utility.Utility.LogAction("CU - " + cuData.CUName + " Connection open for submission.");
                string baseUri = ConfigurationManager.AppSettings["baseUri"],
                apiToken = cuData.Token,
                vendorKey = cuData.Vendorkey;
                dataManagementObj.connectionString = cuData.ConnectionString;
                dataManagementObj.cuName = cuData.CUName;
                sbaLoanDocuments = new SbaLoanDocumentsController(new SbaLoanDocumentService(new SbaRestApiClient(baseUri, apiToken, vendorKey)));
                sbaLoanForgiveness = new SbaLoanForgivenessController(new SbaLoanForgivenessService(new SbaRestApiClient(baseUri, apiToken, vendorKey)));
                sbaLoanForgivenessMessageControllers = new SbaLoanForgivenessMessageController(new SbaLoanForgivenessMessageService(new SbaRestApiClient(baseUri, apiToken, vendorKey)));

                Utility.Utility.LogAction("CU - " + cuData.CUName + " Connection open for updation.");
                GetApplicationStatus();
                ForgivessPaymentUpdate(cuData.ConnectionString);
                SBAGeneratedDocuments(); // Only for CACU
                GetMessageFromSBAAsync();

                ManageForgivenessData();
            });
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                Utility.Utility.LogAction("Method calling from on Update Status timer in ForgivenessService ");
               
            }
            catch (Exception ex)
            {
                Utility.Utility.LogAction("Failed to run service due to: " + ex.Message);
            }

        }


        private void OnElapsedTimeForDataSubmission(object source, ElapsedEventArgs e)
        {
            try
            {
                Utility.Utility.LogAction("Method calling from on Update Status timer in ForgivenessService ");
                SubmitCreditUnionApplications();
                //UpdateCreditUnionApplicationStatus();
            }
            catch (Exception ex)
            {
                Utility.Utility.LogAction("Failed to run service due to: " + ex.Message);
            }

        }

        public void GetMessageFromSBAAsync()
        {
            try
            {

                var forgivenessDetails = dataManagementObj.SelectApplicationStatus();
                forgivenessDetails.ForEach(forgivenessItem =>
                {
                    string sbanumber = Convert.ToString(forgivenessItem.SbaLoanNumber);
                //string sbanumber = "9863517110";
                //string sbanumber = "5668467202";
                Methods.Methods.getForgivenessMessagesBySbaNumber(sbaLoanForgivenessMessageControllers, 1, sbanumber, true);
                });
            }
            catch (Exception ex)
            {
                Utility.Utility.LogAction("Failed to run service due to: " + ex.Message);
            }
        }


        public void ForgivessPaymentUpdate(string conString)
        {
            try
            {
                var forgivenessDetails = dataManagementObj.SelectApplicationStatus();

                forgivenessDetails.ForEach(forgivenessItem =>
                {
                    string sbanumber = Convert.ToString(forgivenessItem.SbaLoanNumber);
                    Methods.Methods.GetForgivenessPaymentBySbaNumber(sbanumber, sbaLoanForgiveness, conString).Wait();
                });
            }
            catch (Exception ex)
            {
                Utility.Utility.LogAction("Failed to run service due to: " + ex.Message);
            }
        }
        public void GetApplicationStatus()
        {
            try
            {
                var forgivenessDetails = dataManagementObj.SelectApplicationStatus();
                forgivenessDetails.ForEach(forgivenessItem =>
                 {
                     string applicationStatus = forgivenessItem.ApplicationStatus;
                     string sbanumber = Convert.ToString(forgivenessItem.SbaLoanNumber);
                     SbaPPPLoanForgivenessStatusResponse sbaObj = Methods.Methods.GetForgivenessRequestBySbaNumber(sbanumber, sbaLoanForgiveness);
                     string status = sbaObj.Status;
                     string slugId = string.Empty;
                     if (sbaObj.results.Count > 0 && !String.IsNullOrEmpty(sbaObj.results[0].slug))
                     {
                         slugId = sbaObj.results[0].slug;
                     //UpdateFogivenessDocument(sbaObj.results[0]).Wait();
                 }
                     if (applicationStatus != status)
                     {
                         dataManagementObj.UpdateForgivenessDb(sbanumber, status, slugId);
                     }

                 });
            }
            catch (Exception ex)
            {
                Utility.Utility.LogAction("Failed to run service due to: " + ex.Message);
            }
        }

        public void SBAGeneratedDocuments()
        {

            try
            {
                var forgivenessDetails = dataManagementObj.SelectApplicationStatus();
                int count = 0;
                forgivenessDetails.ForEach(forgivenessItem =>
                {
                    string applicationStatus = forgivenessItem.ApplicationStatus;
                    string sbanumber = Convert.ToString(forgivenessItem.SbaLoanNumber);

                    if (applicationStatus == "Payment Confirmed")
                    {
                        SbaPPPLoanForgivenessStatusResponse sbaObj = Methods.Methods.GetForgivenessRequestBySbaNumber(sbanumber, sbaLoanForgiveness);
                        string status = sbaObj.Status;

                        var documents = sbaObj.results[0].etran_loan.documents;
                        documents.ForEach(documentItem =>
                        {
                            string name = documentItem.name;
                            string description = documentItem.document_type.description;
                            string docTypeName = documentItem.document_type.name;

                            if (description == "Document generated by the system when a payment is completed")
                            {
                                string url = documentItem.url;
                                string document = documentItem.document;
                                dataManagementObj.InsertForgivenessSBADoc(sbanumber, url, document);
                                count++;
                            }

                        });

                    }

                });
            }
            catch (Exception ex)
            {
                Utility.Utility.LogAction("Failed to run service due to: " + ex.Message);
            }
        }


        public void ValidateDisbursedLoan()
        {
            var forgivenessDetails = dataManagementObj.SelectApplicationStatus();
            forgivenessDetails.ForEach( forgivenessItem =>
            {
                string applicationStatus = forgivenessItem.ApplicationStatus;
                //string sbanumber = "4935797208";
                string sbanumber = Convert.ToString(forgivenessItem.SbaLoanNumber);


                if (sbanumber != null)
                {
                    //SbaPPPLoanForgivenessStatusResponse sbaObj = await Methods.Methods.GetForgivenessRequestBySbaNumber(sbanumber, sbaLoanForgiveness);

                    //if (sbaObj.results[0].etran_loan.forgive_eidl_amount != 0)
                    //{
                    //    double? resultLoanNumber = sbaObj.results[0].etran_loan.forgive_eidl_amount;
                    //}

                    SbaPPPDisbursedLoanForgivenessStatusResponse sbaObj1 =  Methods.Methods.GetDisbursedLoanBySbaNumber(sbanumber, sbaLoanForgiveness);

                    var eidResults = sbaObj1.results[0].eidl_details;

                    string Orgname = sbaObj1.results[0].organization_name;

                }

            });
        }

        public void GetPaymentDashboardData()
        {

        }

        public void ManageForgivenessData()
        {
            try
            {
                var forgivenessDetails = dataManagementObj.GetForgivenessDetails();

                forgivenessDetails.ForEach(forgivenessItem =>
                 {
                     string status = forgivenessItem.applicationStatus;
                 //string edlAmount = forgivenessItem.forgive_eidl_amount;

                 try
                     {
                         switch (status)
                         {
                             case "Awaiting":
                             case "Failed":
                             case "Error":
                                 UseCaseOne(forgivenessItem); // submite and upload loan documents
                             break;
                             case "Resubmit":
                                 UseCaseTwo(forgivenessItem);
                                 break;


                         }
                     }
                     catch (Exception ex)
                     {
                         Utility.Utility.LogAction("Exception " + pppSbaNumber + ":" + ex.Message);

                     }
                 #region Reference code



                 // use case 3

                 //await Methods.Methods.getAllForgivenessRequests(sbaLoanForgiveness);
                 //await Methods.Methods.getSbaLoanForgivenessBySlug(sbaLoanForgiveness,pppSlug);
                 //await Methods.Methods.getForgivenessRequestBySbaNumber(sbaLoanForgiveness,pppSbaNumber);
                 //await Methods.Methods.deleteSbaLoanForgiveness(sbaLoanForgiveness, pppSlug);

                 //use case 4

                 // await Methods.Methods.getDocumentTypes(sbaLoanForgiveness);

                 //use case 5

                 // await Methods.Methods.getForgivenessMessagesBySbaNumber(sbaLoanForgivenessMessageControllers,1,pppSbaNumber,true); 
                 #endregion
             });
            }
            catch (Exception ex)
            {
                Utility.Utility.LogAction("Failed to run service due to: " + ex.Message);
            }
        }

        private static void UseCaseOne(SbaForgiveness forgivenessItem)
        {

            var sbaForgivenessObj = Methods.Methods.InvokeSbaLoanForgiveness(sbaLoanForgiveness, forgivenessItem);
            if (!string.IsNullOrEmpty(sbaForgivenessObj.slug))
            {
                pppSlug = sbaForgivenessObj.slug;
                pppSbaNumber = sbaForgivenessObj.etran_loan.sba_number;

                if (sbaForgivenessObj.etran_loan.ez_form)
                {
                    var documentDetail = dataManagementObj.GetForgivenessDocumentsEZ(pppSbaNumber);
                    if (!string.IsNullOrEmpty(documentDetail.PayrollAName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollAName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollAFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.PayrollBName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollBName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollBFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.PayrollCName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollCName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollCFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.PayrollDName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollDName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollDFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonPayrollCName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonPayrollCName;
                        sbaLoanDocuments.documentType = "20";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonPayrollCFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonPayrollBName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonPayrollBName;
                        sbaLoanDocuments.documentType = "21";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonPayrollBFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.CertifySalaryName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.CertifySalaryName;
                        sbaLoanDocuments.documentType = "16";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.CertifySalaryFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.EmployeeJobName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.EmployeeJobName;
                        sbaLoanDocuments.documentType = "15";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.EmployeeJobFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.CompanyOpsName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.CompanyOpsName;
                        sbaLoanDocuments.documentType = "13";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.CompanyOpsFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonPayrollAName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonPayrollAName;
                        sbaLoanDocuments.documentType = "8";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonPayrollAFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                }
                else
                {

                    var documentDetail = dataManagementObj.GetForgivenessDocumentsFullApp(pppSbaNumber);
                    if (!string.IsNullOrEmpty(documentDetail.PayrollCompensationName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollCompensationName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollCompensationFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.PayrollTaxFormName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollTaxFormName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollTaxFormFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.PayrollPayementsName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollPayementsName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollPayementsFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.FTEDocumentationName1))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.FTEDocumentationName1;
                        sbaLoanDocuments.documentType = "22";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.FTEDocumentFile1;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.FTEDocumentationName2))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.FTEDocumentationName2;
                        sbaLoanDocuments.documentType = "22";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.FTEDocumentFile2;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.FTEDocumentationName3))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.FTEDocumentationName3;
                        sbaLoanDocuments.documentType = "22";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.FTEDocumentFile3;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonpayrollName1))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonpayrollName1;
                        sbaLoanDocuments.documentType = "8";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonpayrollFile1;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonpayrollName2))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonpayrollName2;
                        sbaLoanDocuments.documentType = "21";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonpayrollFile2;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonpayrollName3))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonpayrollName3;
                        sbaLoanDocuments.documentType = "20";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonpayrollFile3;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.AdditionalDocumentName1))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.AdditionalDocumentName1;
                        sbaLoanDocuments.documentType = "12";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.AdditionalDocumentFile1;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.AdditionalDocumentName2))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.AdditionalDocumentName2;
                        sbaLoanDocuments.documentType = "11";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.AdditionalDocumentFile2;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.AdditionalDocumentName3))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.AdditionalDocumentName3;
                        sbaLoanDocuments.documentType = "15";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.AdditionalDocumentFile3;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.AdditionalDocumentName4))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.AdditionalDocumentName4;
                        sbaLoanDocuments.documentType = "10";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.AdditionalDocumentFile4;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.CustomerSafteyFileName))
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.CustomerSafteyFileName;
                        sbaLoanDocuments.documentType = "13";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.CustomerSafteyFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                }

                foreach (var documentDetail in dataManagementObj.GetAdditionalDocuments(pppSbaNumber))
                {
                    sbaLoanDocuments.DocumentName = documentDetail.fileName;
                    sbaLoanDocuments.documentType = documentDetail.DocumentType;
                    sbaLoanDocuments.etranId = pppSlug;
                    sbaLoanDocuments.rawDocument = documentDetail.fileContent;
                    Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
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


        private static async Task UpdateFogivenessDocument(SbaPPPLoanForgiveness sbaForgivenessObj)
        {
            if (!string.IsNullOrEmpty(sbaForgivenessObj.slug))
            {
                pppSlug = sbaForgivenessObj.slug;
                pppSbaNumber = sbaForgivenessObj.etran_loan.sba_number;

                if (sbaForgivenessObj.etran_loan.ez_form)
                {
                    var documentDetail = dataManagementObj.GetForgivenessDocumentsEZ(pppSbaNumber);

                    if (!string.IsNullOrEmpty(documentDetail.PayrollAName) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.PayrollAName) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollAName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollAFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.PayrollBName) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.PayrollBName) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollBName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollBFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.PayrollCName) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.PayrollCName) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollCName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollCFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.PayrollDName) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.PayrollDName) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollDName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollDFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonPayrollCName) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.NonPayrollCName) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonPayrollCName;
                        sbaLoanDocuments.documentType = "20";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonPayrollCFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonPayrollBName) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.NonPayrollBName) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonPayrollBName;
                        sbaLoanDocuments.documentType = "21";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonPayrollBFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.CertifySalaryName) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.CertifySalaryName) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.CertifySalaryName;
                        sbaLoanDocuments.documentType = "16";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.CertifySalaryFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.EmployeeJobName) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.EmployeeJobName) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.EmployeeJobName;
                        sbaLoanDocuments.documentType = "15";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.EmployeeJobFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.CompanyOpsName) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.CompanyOpsName) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.CompanyOpsName;
                        sbaLoanDocuments.documentType = "13";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.CompanyOpsFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonPayrollAName) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.NonPayrollAName) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonPayrollAName;
                        sbaLoanDocuments.documentType = "8";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonPayrollAFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                }
                else
                {

                    var documentDetail = dataManagementObj.GetForgivenessDocumentsFullApp(pppSbaNumber);
                    if (!string.IsNullOrEmpty(documentDetail.PayrollCompensationName) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.PayrollCompensationName) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollCompensationName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollCompensationFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.PayrollTaxFormName) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.PayrollTaxFormName) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollTaxFormName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollTaxFormFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.PayrollPayementsName) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.PayrollPayementsName) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.PayrollPayementsName;
                        sbaLoanDocuments.documentType = "23";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.PayrollPayementsFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.FTEDocumentationName1) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.FTEDocumentationName1) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.FTEDocumentationName1;
                        sbaLoanDocuments.documentType = "22";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.FTEDocumentFile1;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.FTEDocumentationName2) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.FTEDocumentationName2) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.FTEDocumentationName2;
                        sbaLoanDocuments.documentType = "22";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.FTEDocumentFile2;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.FTEDocumentationName3) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.FTEDocumentationName3) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.FTEDocumentationName3;
                        sbaLoanDocuments.documentType = "22";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.FTEDocumentFile3;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonpayrollName1) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.NonpayrollName1) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonpayrollName1;
                        sbaLoanDocuments.documentType = "8";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonpayrollFile1;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonpayrollName2) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.NonpayrollName2) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonpayrollName2;
                        sbaLoanDocuments.documentType = "21";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonpayrollFile2;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.NonpayrollName3) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.NonpayrollName3) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.NonpayrollName3;
                        sbaLoanDocuments.documentType = "20";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.NonpayrollFile3;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.AdditionalDocumentName1) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.AdditionalDocumentName1) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.AdditionalDocumentName1;
                        sbaLoanDocuments.documentType = "12";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.AdditionalDocumentFile1;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.AdditionalDocumentName2) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.AdditionalDocumentName2) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.AdditionalDocumentName2;
                        sbaLoanDocuments.documentType = "11";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.AdditionalDocumentFile2;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.AdditionalDocumentName3) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.AdditionalDocumentName3) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.AdditionalDocumentName3;
                        sbaLoanDocuments.documentType = "15";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.AdditionalDocumentFile3;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.AdditionalDocumentName4) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.AdditionalDocumentName4) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.AdditionalDocumentName4;
                        sbaLoanDocuments.documentType = "10";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.AdditionalDocumentFile4;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                    if (!string.IsNullOrEmpty(documentDetail.CustomerSafteyFileName) && dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.CustomerSafteyFileName) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.CustomerSafteyFileName;
                        sbaLoanDocuments.documentType = "13";
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.CustomerSafteyFile;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
                }

                foreach (var documentDetail in dataManagementObj.GetAdditionalDocuments(pppSbaNumber))
                {
                    if (dataManagementObj.GetForgivenessDocumentsCount(pppSbaNumber, documentDetail.fileName) <= 0)
                    {
                        sbaLoanDocuments.DocumentName = documentDetail.fileName;
                        sbaLoanDocuments.documentType = documentDetail.DocumentType;
                        sbaLoanDocuments.etranId = pppSlug;
                        sbaLoanDocuments.rawDocument = documentDetail.fileContent;
                        Methods.Methods.UploadForgivenessDocument(sbaLoanDocuments, pppSbaNumber, pppSlug);
                    }
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

        private static void UseCaseTwo(SbaForgiveness forgivenessItem)
        {
            SbaPPPLoanForgivenessStatusResponse sbaObj = Methods.Methods.GetForgivenessRequestBySbaNumber(pppSbaNumber, sbaLoanForgiveness);
            if (sbaObj.Status == "Pending Validation")
            {
                bool deleteStatasObj = Methods.Methods.DeleteSbaLoanForgiveness(sbaLoanForgiveness, pppSlug, pppSbaNumber);
                if (deleteStatasObj == true)
                {
                     UseCaseOne(forgivenessItem);
                }
            }
            else
            {
                Utility.Utility.LogAction($"{sbaObj.Status}. So this applicataion request is not applicable for resubmission!");
            }

        }

        protected override void OnStop()
        {
            updateStatusTimer.Enabled = false;
            requestSubmissionTimer.Enabled = false;
        }
    }
}
