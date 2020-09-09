using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Claysys.PPP.Forgiveness.Model;
using Newtonsoft.Json;
using Claysys.PPP.Forgiveness.controller;
using Claysys.PPP.Forgiveness.domain;
using Claysys.PPP.Forgiveness.restclient;
using Claysys.PPP.Forgiveness.service;

namespace Claysys.PPP.Forgiveness.Methods
{
    public static class Methods
    {
        public static string pppLoanDocuments;

        private static async Task getSbaLoanRequestStatus(SbaLoanForgivenessController sbaLoanForgiveness)
        {
            SbaPPPLoanDocumentTypeResponse loanDocumentType =
                await sbaLoanForgiveness.getSbaLoanRequestStatus(2, "1,",
                    "ppp_loan_document_types"); // loanForgivenessUrl - to be passed
            if (loanDocumentType != null)
            {
                var serialized = JsonConvert.SerializeObject(loanDocumentType,
                    new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });
                Utility.Utility.LogAction($"{Environment.NewLine}{serialized}{Environment.NewLine}");
            }
        }

        public static async Task<SbaPPPLoanForgivenessStatusResponse> getAllForgivenessRequests(SbaLoanForgivenessController sbaLoanForgiveness)
        {
            SbaPPPLoanForgivenessStatusResponse allForgivenessRequests =
                await sbaLoanForgiveness.getAllForgivenessRequests("ppp_loan_forgiveness_requests");
            return allForgivenessRequests;
        }

        public static async Task<SbaPPPLoanForgiveness> getSbaLoanForgivenessBySlug(SbaLoanForgivenessController sbaLoanForgiveness, string slug)
        {
            SbaPPPLoanForgiveness sbaLoanForgivenessBySlug =
                await sbaLoanForgiveness.getSbaLoanForgivenessBySlug(slug, "ppp_loan_forgiveness_requests");
            return sbaLoanForgivenessBySlug;
        }

        public static async Task<SbaPPPLoanForgivenessStatusResponse> GetForgivenessRequestBySbaNumber(string sbaNumber, SbaLoanForgivenessController sbaLoanForgiveness)
        {
            SbaPPPLoanForgivenessStatusResponse loanForgivenessStatusResponse =
                await sbaLoanForgiveness.GetForgivenessRequestBysbaNumber(sbaNumber, "ppp_loan_forgiveness_requests");
            return loanForgivenessStatusResponse;
        }

        public static async Task<SbaPPPLoanDocumentTypeResponse> GetDocumentTypes(SbaLoanForgivenessController sbaLoanForgiveness)
        {
            SbaPPPLoanDocumentTypeResponse loanDocumentType =
                await sbaLoanForgiveness.GetDocumenttypes("ppp_loan_document_types");
            return loanDocumentType;
        }

        public static async Task<SbaPPPLoanMessagesResponse> getForgivenessMessagesBySbaNumber(SbaLoanForgivenessMessageController sbaLoanForgivenessMessageControllers, int page, String sbaNumber, bool isComplete)
        {
            SbaPPPLoanMessagesResponse sbaPppLoanMessagesResponse =
                await sbaLoanForgivenessMessageControllers.getForgivenessMessagesBySbaNumber(page, sbaNumber, isComplete,
                    "ppp_loan_forgiveness_messages");
            return sbaPppLoanMessagesResponse;
        }


        public static async Task<bool> DeleteSbaLoanForgiveness(SbaLoanForgivenessController sbaLoanForgiveness, string slug, string sbaNumber)
        {
            return await sbaLoanForgiveness.DeleteSbaLoanForgiveness(slug, sbaNumber, "ppp_loan_forgiveness_requests"); 
        }

        public static async Task<SbaPPPLoanForgiveness> InvokeSbaLoanForgiveness(SbaLoanForgivenessController sbaLoanForgiveness, SbaForgiveness sbaForgivenessobj)
        {
            SbaPPPLoanForgiveness pppLoanForgiveness;

            if (sbaForgivenessobj.ez_form)
            {
                pppLoanForgiveness = new SbaPPPLoanForgiveness
                {
                    borrower_name = sbaForgivenessobj.Entity_Name,
                    etran_loan = new EtranLoan()
                    {
                        demographics = sbaForgivenessobj.demographics,
                        bank_notional_amount = Convert.ToDouble(sbaForgivenessobj.pppLoanAmount),
                        sba_number = Convert.ToString(sbaForgivenessobj.sbaLoanNumber),
                        loan_number = Convert.ToString(sbaForgivenessobj.flLoanNumber),
                        entity_name = sbaForgivenessobj.Entity_Name,
                        ein = Convert.ToString(sbaForgivenessobj.einSsn),
                        funding_date = Convert.ToString(sbaForgivenessobj.fundingDate),
                        forgive_eidl_amount = Convert.ToDouble(sbaForgivenessobj.forgive_eidl_amount),
                        forgive_eidl_application_number = sbaForgivenessobj.forgive_eidl_application_number,
                        forgive_payroll = Convert.ToDouble(sbaForgivenessobj.forgive_payroll), 
                        forgive_rent = Convert.ToDouble(sbaForgivenessobj.forgive_rent),
                        forgive_utilities = Convert.ToDouble(sbaForgivenessobj.forgive_utilities), 
                        forgive_mortgage = Convert.ToDouble(sbaForgivenessobj.forgive_mortgage), 
                        address1 = sbaForgivenessobj.address1, 
                        address2 = sbaForgivenessobj.address2, 
                        dba_name = sbaForgivenessobj.dba_name, 
                        phone_number = sbaForgivenessobj.phone_number,                                               
                        forgive_fte_at_loan_application = sbaForgivenessobj.forgive_fte_at_loan_application,                                                                                      
                        forgive_line_6_3508_or_line_5_3508ez = Convert.ToDouble(sbaForgivenessobj.forgive_line_6_3508_or_line_5_3508ez), 
                        forgive_payroll_cost_60_percent_requirement = Convert.ToDouble(sbaForgivenessobj.forgive_payroll_cost_60_percent_requirement), 
                        forgive_amount = Convert.ToDouble(sbaForgivenessobj.forgive_amount), 
                        forgive_fte_at_forgiveness_application = Convert.ToInt32(sbaForgivenessobj.forgive_fte_at_forgiveness_application), 
                        forgive_covered_period_from = sbaForgivenessobj.forgive_covered_period_from, 
                        forgive_covered_period_to = sbaForgivenessobj.forgive_covered_period_to, 
                        forgive_alternate_covered_period_from = sbaForgivenessobj.forgive_alternate_covered_period_from, 
                        forgive_alternate_covered_period_to = sbaForgivenessobj.forgive_alternate_covered_period_to, 
                        forgive_2_million = sbaForgivenessobj.forgive_2_million, 
                        forgive_payroll_schedule = sbaForgivenessobj.forgive_payroll_schedule, 
                        forgive_lender_decision = sbaForgivenessobj.forgive_lender_decision, 
                        primary_email = sbaForgivenessobj.primary_email, 
                        primary_name = sbaForgivenessobj.primary_name, 
                        ez_form = sbaForgivenessobj.ez_form, 

                        forgive_lender_confirmation = sbaForgivenessobj.forgive_lender_confirmation 
                    },
                    
                };
            }
            else
            {
                pppLoanForgiveness = new SbaPPPLoanForgiveness
                {
                    borrower_name = sbaForgivenessobj.Entity_Name,
                    etran_loan = new EtranLoan()
                    {
                        demographics = sbaForgivenessobj.demographics,
                        bank_notional_amount = Convert.ToDouble(sbaForgivenessobj.pppLoanAmount),
                        sba_number = Convert.ToString(sbaForgivenessobj.sbaLoanNumber),
                        loan_number = Convert.ToString(sbaForgivenessobj.flLoanNumber),
                        entity_name = sbaForgivenessobj.Entity_Name,
                        ein = Convert.ToString(sbaForgivenessobj.einSsn),
                        funding_date = Convert.ToString(sbaForgivenessobj.fundingDate),
                        forgive_eidl_amount = Convert.ToDouble(sbaForgivenessobj.forgive_eidl_amount),
                        forgive_eidl_application_number = sbaForgivenessobj.forgive_eidl_application_number,
                        forgive_payroll = Convert.ToDouble(sbaForgivenessobj.forgive_payroll), 
                        forgive_rent = Convert.ToDouble(sbaForgivenessobj.forgive_rent), 
                        forgive_utilities = Convert.ToDouble(sbaForgivenessobj.forgive_utilities), 
                        address1 = sbaForgivenessobj.address1, 
                        address2 = sbaForgivenessobj.address2, 
                        dba_name = sbaForgivenessobj.dba_name, 
                        phone_number = sbaForgivenessobj.phone_number, 
                        forgive_mortgage = Convert.ToDouble(sbaForgivenessobj.forgive_mortgage),

                        forgive_fte_at_loan_application = sbaForgivenessobj.forgive_fte_at_loan_application, 
                                                                                                             
                        forgive_line_6_3508_or_line_5_3508ez = Convert.ToDouble(sbaForgivenessobj.forgive_line_6_3508_or_line_5_3508ez), 
                        forgive_payroll_cost_60_percent_requirement = Convert.ToDouble(sbaForgivenessobj.forgive_payroll_cost_60_percent_requirement), 
                        forgive_amount = Convert.ToDouble(sbaForgivenessobj.forgive_amount), 
                        forgive_fte_at_forgiveness_application = sbaForgivenessobj.forgive_fte_at_forgiveness_application, 

                        forgive_modified_total = Convert.ToDouble(string.IsNullOrEmpty(sbaForgivenessobj.forgive_modified_total) ? "0.00" : sbaForgivenessobj.forgive_modified_total), 
                        forgive_schedule_a_line_1 = Convert.ToDouble(string.IsNullOrEmpty(sbaForgivenessobj.forgive_schedule_a_line_1) ? "0.00" : sbaForgivenessobj.forgive_schedule_a_line_1), 
                        forgive_schedule_a_line_2 = Convert.ToDouble(string.IsNullOrEmpty(sbaForgivenessobj.forgive_schedule_a_line_2) ? "0.00" : sbaForgivenessobj.forgive_schedule_a_line_2),
                        forgive_schedule_a_line_3_checkbox = Convert.ToBoolean(sbaForgivenessobj.forgive_schedule_a_line_3_checkbox), 
                        forgive_schedule_a_line_3 = Convert.ToDouble(string.IsNullOrEmpty(sbaForgivenessobj.forgive_schedule_a_line_3) ? "0.00" : sbaForgivenessobj.forgive_schedule_a_line_3), 
                        forgive_schedule_a_line_4 = Convert.ToDouble(string.IsNullOrEmpty(sbaForgivenessobj.forgive_schedule_a_line_4) ? "0.00" : sbaForgivenessobj.forgive_schedule_a_line_4), 
                        forgive_schedule_a_line_5 = Convert.ToDouble(string.IsNullOrEmpty(sbaForgivenessobj.forgive_schedule_a_line_5) ? "0.00" : sbaForgivenessobj.forgive_schedule_a_line_5), 
                        forgive_schedule_a_line_6 = Convert.ToDouble(string.IsNullOrEmpty(sbaForgivenessobj.forgive_schedule_a_line_6) ? "0.00" : sbaForgivenessobj.forgive_schedule_a_line_6), 
                        forgive_schedule_a_line_7 = Convert.ToDouble(string.IsNullOrEmpty(sbaForgivenessobj.forgive_schedule_a_line_7) ? "0.00" : sbaForgivenessobj.forgive_schedule_a_line_7), 
                        forgive_schedule_a_line_8 = Convert.ToDouble(string.IsNullOrEmpty(sbaForgivenessobj.forgive_schedule_a_line_8) ? "0.00" : sbaForgivenessobj.forgive_schedule_a_line_8), 
                        forgive_schedule_a_line_9 = Convert.ToDouble(string.IsNullOrEmpty(sbaForgivenessobj.forgive_schedule_a_line_9) ? "0.00" : sbaForgivenessobj.forgive_schedule_a_line_9), 
                        forgive_schedule_a_line_10 = Convert.ToDouble(string.IsNullOrEmpty(sbaForgivenessobj.forgive_schedule_a_line_10) ? "0.00" : sbaForgivenessobj.forgive_schedule_a_line_10), 
                        forgive_schedule_a_line_10_checkbox = Convert.ToBoolean(sbaForgivenessobj.forgive_schedule_a_line_10_checkbox), 
                        forgive_schedule_a_line_11 = Convert.ToDouble(string.IsNullOrEmpty(sbaForgivenessobj.forgive_schedule_a_line_11) ? "0.00" : sbaForgivenessobj.forgive_schedule_a_line_11), 
                        forgive_schedule_a_line_12 = Convert.ToDouble(string.IsNullOrEmpty(sbaForgivenessobj.forgive_schedule_a_line_12) ? "0.00" : sbaForgivenessobj.forgive_schedule_a_line_12), 
                        forgive_schedule_a_line_13 = Convert.ToDouble(string.IsNullOrEmpty(sbaForgivenessobj.forgive_schedule_a_line_13) ? "0.00" : sbaForgivenessobj.forgive_schedule_a_line_13), 
                        no_reduction_in_employees = Convert.ToBoolean(sbaForgivenessobj.no_reduction_in_employees), 
                        no_reduction_in_employees_and_covid_impact = sbaForgivenessobj.no_reduction_in_employees_and_covid_impact, 

                        forgive_covered_period_from = sbaForgivenessobj.forgive_covered_period_from, 
                        forgive_covered_period_to = sbaForgivenessobj.forgive_covered_period_to, 
                        forgive_alternate_covered_period_from = sbaForgivenessobj.forgive_alternate_covered_period_from, 
                        forgive_alternate_covered_period_to = sbaForgivenessobj.forgive_alternate_covered_period_to, 
                        forgive_2_million = sbaForgivenessobj.forgive_2_million, 
                        forgive_payroll_schedule = sbaForgivenessobj.forgive_payroll_schedule, 
                        forgive_lender_decision = sbaForgivenessobj.forgive_lender_decision, 
                        primary_email = sbaForgivenessobj.primary_email, 
                        primary_name = sbaForgivenessobj.primary_name, 
                        ez_form = sbaForgivenessobj.ez_form, 

                        forgive_lender_confirmation = sbaForgivenessobj.forgive_lender_confirmation 
                    },
                    
                };
            }
            SbaPPPLoanForgiveness sbaPppLoanForgiveness =
                await sbaLoanForgiveness.Execute(pppLoanForgiveness, "ppp_loan_forgiveness_requests");
            if (sbaPppLoanForgiveness != null)
            {
                var serialized = JsonConvert.SerializeObject(sbaPppLoanForgiveness,
                    new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });
                Utility.Utility.LogAction($"{Environment.NewLine}{serialized}{Environment.NewLine}");
                return sbaPppLoanForgiveness;
            }
            else
            {
                Utility.Utility.LogAction($"{pppLoanForgiveness.etran_loan.sba_number}Invoke method in Method.cs failed to return SbaPPPLoanForgiveness object with values.");
            }
            return new SbaPPPLoanForgiveness();
        }

        public static async Task UploadForgivenessDocument(SbaLoanDocumentsController sbaLoanDocuments, string sbaNumber, string slugId)
        {
            LoanDocumentResponse response = await UploadForgivenessDocument(sbaLoanDocuments.DocumentName, sbaLoanDocuments.documentType, sbaLoanDocuments.etranId, sbaLoanDocuments.rawDocument
          , "ppp_loan_documents", sbaLoanDocuments, sbaNumber, slugId);

            string serialized = JsonConvert.SerializeObject(response,
               new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.IsoDateFormat });

        }

        public static async Task<LoanDocumentResponse> UploadForgivenessDocument(string requestName, string requestDocument_type, string etran_loan, byte[] document, string apiMethod, SbaLoanDocumentsController sbaLoanDocuments, string sbaNumber,string slugId)
        {

            LoanDocumentResponse loanDocument = await sbaLoanDocuments.UploadForgivenessDocument(requestName,
                    requestDocument_type, etran_loan, document, apiMethod, sbaNumber, slugId);
            return loanDocument;
        }


        public static async Task submitLoanDocument(SbaLoanDocumentsController sbaLoanDocuments)
        {
            LoanDocument request = new LoanDocument
            {
                slug = "5777a53a-350b-4d1c-b169-e29e5e2001c2",
                name = "sample.pdf",
                created_at = Convert.ToDateTime("2020-08-04T09:07:04.508730Z"),
                updated_at = Convert.ToDateTime("2020-08-04T09:07:04.508757Z"),
                document =
                    "https://lenders-cooperative-sandbox-app.s3.us-gov-west-1.amazonaws.com/media/loan/5777a53a-350b-4d1c-b169-e29e5e2001c2/files/sample.pdf?AWSAccessKeyId=AKIAQ2EGUESVZTJ6OS4N&Signature=8g38qCB9N63ppoJiTao3Z2J1VHI%3D&Expires=1596532039",
                url =
                    "https://sandbox.forgiveness.sba.gov/loans/doc_download_file/5777a53a-350b-4d1c-b169-e29e5e2001c2",
                etran_loan = "4312ef27-f54e-49e1-8606-c3b761944420",
                document_type = new LoanDocumentType
                {
                    id = 1,
                    name = "Loan Application Supporting Docs (Payroll)",
                    description = "Payroll Documents"
                }
            };

            LoanDocument loanDocument = await sbaLoanDocuments.submitLoanDocument(request, "ppp_loan_documents");
            if (loanDocument != null)
            {
                var serialized = JsonConvert.SerializeObject(loanDocument,
                    new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });
                Utility.Utility.LogAction($"{Environment.NewLine}{serialized}{Environment.NewLine}");
            }
        }

        public static async Task getSbaLoanDocumentTypeById(SbaLoanDocumentsController sbaLoanDocuments)
        {
            LoanDocumentType loanDocumentType =
                await sbaLoanDocuments.getSbaLoanDocumentTypeById(2, "ppp_loan_document_types"); // loanForgivenessUrl - to be passed
            if (loanDocumentType != null)
            {
                var serialized = JsonConvert.SerializeObject(loanDocumentType,
                    new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });
                Utility.Utility.LogAction($"{Environment.NewLine}{serialized}{Environment.NewLine}");
            }
        }


        private static async Task updateSbaLoanMessageReply(SbaLoanForgivenessMessageController sbaLoanForgivenessMessageController)
        {
            MessageReply message = new MessageReply();

            MessageReply sbaLoanMessageReply =
                await sbaLoanForgivenessMessageController.updateSbaLoanMessageReply(message,
                    "ppp_loan_forgiveness_message_reply");

            if (sbaLoanMessageReply != null)
            {
                var serialized = JsonConvert.SerializeObject(sbaLoanMessageReply,
                    new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });
                Utility.Utility.LogAction($"{Environment.NewLine}{serialized}{Environment.NewLine}");
            }
        }


        private static async Task getLoanMessagesBySlug(SbaLoanForgivenessMessageController sbaLoanForgivenessMessageController)
        {
            SbaPPPLoanForgivenessMessage loanMessagesBySlug =
                await sbaLoanForgivenessMessageController.getLoanMessagesBySlug("test",
                    "ppp_loan_forgiveness_messages");

            if (loanMessagesBySlug != null)
            {
                var serialized = JsonConvert.SerializeObject(loanMessagesBySlug,
                    new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });
                Utility.Utility.LogAction($"{Environment.NewLine}{serialized}{Environment.NewLine}");
            }
        }

        
    }

}
