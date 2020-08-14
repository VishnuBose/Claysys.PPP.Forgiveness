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
            return await sbaLoanForgiveness.DeleteSbaLoanForgiveness(slug, sbaNumber, "ppp_loan_forgiveness_requests"); // loanForgivenessUrl - to be passed
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
                        forgive_payroll = Convert.ToDouble(sbaForgivenessobj.forgive_payroll), //"1000.00",//Convert.ToString(sbaForgivenessobj.pppLoanAmount + 2000),
                        forgive_rent = Convert.ToDouble(sbaForgivenessobj.forgive_rent), //"1000.00",
                        forgive_utilities = Convert.ToDouble(sbaForgivenessobj.forgive_utilities), //"1000.00",
                        forgive_mortgage = Convert.ToDouble(sbaForgivenessobj.forgive_mortgage), //"1000.00",
                        address1 = sbaForgivenessobj.address1, //"stringss",
                        address2 = sbaForgivenessobj.address2, //"stringsss",
                        dba_name = sbaForgivenessobj.dba_name, //"stringssssss",
                        phone_number = sbaForgivenessobj.phone_number, //"1234567890",
                                                                       //assign_to_user = sbaForgivenessobj. //null,
                        forgive_fte_at_loan_application = sbaForgivenessobj.forgive_fte_at_loan_application, //10,
                                                                                                             //updated = sbaForgivenessobj. //"06/28/2020",
                        forgive_line_6_3508_or_line_5_3508ez = Convert.ToDouble(sbaForgivenessobj.forgive_line_6_3508_or_line_5_3508ez), //"3999.00",//Convert.ToString(sbaForgivenessobj.pppLoanAmount + 2000),//Convert.ToString(sbaForgivenessobj.pppLoanAmount),
                        forgive_payroll_cost_60_percent_requirement = Convert.ToDouble(sbaForgivenessobj.forgive_payroll_cost_60_percent_requirement), //"1666.66",//Convert.ToString((sbaForgivenessobj.pppLoanAmount + 2000) * 0.6),//Convert.ToString(sbaForgivenessobj.pppLoanAmount * 0.6),
                        forgive_amount = Convert.ToDouble(sbaForgivenessobj.forgive_amount), //"1666.66",//Convert.ToString((sbaForgivenessobj.pppLoanAmount)),//Convert.ToString(sbaForgivenessobj.pppLoanAmount * 0.6),
                        forgive_fte_at_forgiveness_application = Convert.ToInt32(sbaForgivenessobj.forgive_fte_at_forgiveness_application), //10,

                        forgive_covered_period_from = sbaForgivenessobj.forgive_covered_period_from, //"2020-07-06",
                        forgive_covered_period_to = sbaForgivenessobj.forgive_covered_period_to, //"2020-09-06",
                        forgive_alternate_covered_period_from = sbaForgivenessobj.forgive_alternate_covered_period_from, //"2020-07-06",
                        forgive_alternate_covered_period_to = sbaForgivenessobj.forgive_alternate_covered_period_to, //"2020-09-06",
                        forgive_2_million = sbaForgivenessobj.forgive_2_million, //false,
                        forgive_payroll_schedule = sbaForgivenessobj.forgive_payroll_schedule, //"weekly",
                        forgive_lender_decision = sbaForgivenessobj.forgive_lender_decision, //1,
                        primary_email = sbaForgivenessobj.primary_email, //"user@example.com",
                        primary_name = sbaForgivenessobj.primary_name, //"MOCKDATA",
                        ez_form = sbaForgivenessobj.ez_form, //false,

                        forgive_lender_confirmation = sbaForgivenessobj.forgive_lender_confirmation //true
                    },
                    //created = "06/28/2020",
                    //assigned_to_user = " "
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
                        forgive_payroll = Convert.ToDouble(sbaForgivenessobj.forgive_payroll), //"1000.00",//Convert.ToString(sbaForgivenessobj.pppLoanAmount + 2000),
                        forgive_rent = Convert.ToDouble(sbaForgivenessobj.forgive_rent), //"1000.00",
                        forgive_utilities = Convert.ToDouble(sbaForgivenessobj.forgive_utilities), //"1000.00",
                        forgive_mortgage = Convert.ToDouble(sbaForgivenessobj.forgive_mortgage), //"1000.00",
                        address1 = sbaForgivenessobj.address1, //"stringss",
                        address2 = sbaForgivenessobj.address2, //"stringsss",
                        dba_name = sbaForgivenessobj.dba_name, //"stringssssss",
                        phone_number = sbaForgivenessobj.phone_number, //"1234567890",
                                                                       //assign_to_user = sbaForgivenessobj. //null,
                        forgive_fte_at_loan_application = sbaForgivenessobj.forgive_fte_at_loan_application, //10,
                                                                                                             //updated = sbaForgivenessobj. //"06/28/2020",
                        forgive_line_6_3508_or_line_5_3508ez = Convert.ToDouble(sbaForgivenessobj.forgive_line_6_3508_or_line_5_3508ez), //"3999.00",//Convert.ToString(sbaForgivenessobj.pppLoanAmount + 2000),//Convert.ToString(sbaForgivenessobj.pppLoanAmount),
                        forgive_payroll_cost_60_percent_requirement = Convert.ToDouble(sbaForgivenessobj.forgive_payroll_cost_60_percent_requirement), //"1666.66",//Convert.ToString((sbaForgivenessobj.pppLoanAmount + 2000) * 0.6),//Convert.ToString(sbaForgivenessobj.pppLoanAmount * 0.6),
                        forgive_amount = Convert.ToDouble(sbaForgivenessobj.forgive_amount), //"1666.66",//Convert.ToString((sbaForgivenessobj.pppLoanAmount)),//Convert.ToString(sbaForgivenessobj.pppLoanAmount * 0.6),
                        forgive_fte_at_forgiveness_application = sbaForgivenessobj.forgive_fte_at_forgiveness_application, //10,

                        forgive_modified_total = Convert.ToDouble(sbaForgivenessobj.forgive_modified_total), //"3999.00",//Convert.ToString(sbaForgivenessobj.pppLoanAmount + 2000),
                        forgive_schedule_a_line_1 = Convert.ToDouble(sbaForgivenessobj.forgive_schedule_a_line_1), //"1.00",//Convert.ToString(sbaForgivenessobj.pppLoanAmount),
                        forgive_schedule_a_line_2 = Convert.ToDouble(sbaForgivenessobj.forgive_schedule_a_line_2), //"1.00",
                        forgive_schedule_a_line_3_checkbox = Convert.ToBoolean(sbaForgivenessobj.forgive_schedule_a_line_3_checkbox), //true,
                        forgive_schedule_a_line_3 = Convert.ToDouble(sbaForgivenessobj.forgive_schedule_a_line_3), //"1.00",
                        forgive_schedule_a_line_4 = Convert.ToDouble(sbaForgivenessobj.forgive_schedule_a_line_4), //"1.00",
                        forgive_schedule_a_line_5 = Convert.ToDouble(sbaForgivenessobj.forgive_schedule_a_line_5), //"1.00",
                        forgive_schedule_a_line_6 = Convert.ToDouble(sbaForgivenessobj.forgive_schedule_a_line_6), //"1.00",
                        forgive_schedule_a_line_7 = Convert.ToDouble(sbaForgivenessobj.forgive_schedule_a_line_7), //"1.00",
                        forgive_schedule_a_line_8 = Convert.ToDouble(sbaForgivenessobj.forgive_schedule_a_line_8), //"1.00",
                        forgive_schedule_a_line_9 = Convert.ToDouble(sbaForgivenessobj.forgive_schedule_a_line_9), //"1.00",
                        forgive_schedule_a_line_10 = Convert.ToDouble(sbaForgivenessobj.forgive_schedule_a_line_10), //"6.00",//Convert.ToString(sbaForgivenessobj.pppLoanAmount + 2000),
                        forgive_schedule_a_line_10_checkbox = Convert.ToBoolean(sbaForgivenessobj.forgive_schedule_a_line_10_checkbox), //true,
                        forgive_schedule_a_line_11 = Convert.ToDouble(sbaForgivenessobj.forgive_schedule_a_line_11), //"10",
                        forgive_schedule_a_line_12 = Convert.ToDouble(sbaForgivenessobj.forgive_schedule_a_line_12), //"10",
                        forgive_schedule_a_line_13 = Convert.ToDouble(sbaForgivenessobj.forgive_schedule_a_line_13), //"1",
                        no_reduction_in_employees = Convert.ToBoolean(sbaForgivenessobj.no_reduction_in_employees), //true,
                        no_reduction_in_employees_and_covid_impact = sbaForgivenessobj.no_reduction_in_employees_and_covid_impact, //true,

                        forgive_covered_period_from = sbaForgivenessobj.forgive_covered_period_from, //"2020-07-06",
                        forgive_covered_period_to = sbaForgivenessobj.forgive_covered_period_to, //"2020-09-06",
                        forgive_alternate_covered_period_from = sbaForgivenessobj.forgive_alternate_covered_period_from, //"2020-07-06",
                        forgive_alternate_covered_period_to = sbaForgivenessobj.forgive_alternate_covered_period_to, //"2020-09-06",
                        forgive_2_million = sbaForgivenessobj.forgive_2_million, //false,
                        forgive_payroll_schedule = sbaForgivenessobj.forgive_payroll_schedule, //"weekly",
                        forgive_lender_decision = sbaForgivenessobj.forgive_lender_decision, //1,
                        primary_email = sbaForgivenessobj.primary_email, //"user@example.com",
                        primary_name = sbaForgivenessobj.primary_name, //"MOCKDATA",
                        ez_form = sbaForgivenessobj.ez_form, //false,

                        forgive_lender_confirmation = sbaForgivenessobj.forgive_lender_confirmation //true
                    },
                    //created = "06/28/2020",
                    //assigned_to_user = " "
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

        public static async Task UploadForgivenessDocument(SbaLoanDocumentsController sbaLoanDocuments, string sbaNumber)
        {
            LoanDocumentResponse response = await UploadForgivenessDocument(sbaLoanDocuments.DocumentName, sbaLoanDocuments.documentType, sbaLoanDocuments.etranId, sbaLoanDocuments.rawDocument
          , "ppp_loan_documents", sbaLoanDocuments, sbaNumber);

            string serialized = JsonConvert.SerializeObject(response,
               new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.IsoDateFormat });

        }

        public static async Task<LoanDocumentResponse> UploadForgivenessDocument(string requestName, string requestDocument_type, string etran_loan, byte[] document, string apiMethod, SbaLoanDocumentsController sbaLoanDocuments, string sbaNumber)
        {

            LoanDocumentResponse loanDocument = await sbaLoanDocuments.UploadForgivenessDocument(requestName,
                    requestDocument_type, etran_loan, document, apiMethod, sbaNumber);
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

        //public static async Task<bool> deleteSbaLoanForgiveness(SbaLoanForgivenessController sbaLoanForgiveness)
        //{
        //    return await sbaLoanForgiveness.deleteSbaLoanForgiveness(slug, pppLoanDocumentTypes);
        //}
    }

}
