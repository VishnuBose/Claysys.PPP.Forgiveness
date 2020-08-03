using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Claysys.PPP.Forgiveness.Model;
using Newtonsoft.Json;
using sbaCSharpClient.controller;
using sbaCSharpClient.domain;
using sbaCSharpClient.restclient;
using sbaCSharpClient.service;

namespace sbaCSharpClient
{
    public static class Methods
    {
        private static async Task getSbaLoanRequestStatus(SbaLoanForgivenessController sbaLoanForgiveness)
        {
            SbaPPPLoanDocumentTypeResponse loanDocumentType =
                await sbaLoanForgiveness.getSbaLoanRequestStatus(2, "1,",
                    "ppp_loan_document_types"); // loanForgivenessUrl - to be passed
            if (loanDocumentType != null)
            {
                var serialized = JsonConvert.SerializeObject(loanDocumentType,
                    new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });
                Console.WriteLine($"{Environment.NewLine}{serialized}{Environment.NewLine}");
                Console.WriteLine("------------------------------------------------------------------------");
            }
        }

        private static async Task getSbaLoanForgiveness(SbaLoanForgivenessController sbaLoanForgiveness)
        {
            SbaPPPLoanForgiveness loanDocumentType =
                await sbaLoanForgiveness.getSbaLoanForgiveness("2",
                    "ppp_loan_document_types"); // loanForgivenessUrl - to be passed
            if (loanDocumentType != null)
            {
                var serialized = JsonConvert.SerializeObject(loanDocumentType,
                    new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });
                Console.WriteLine($"{Environment.NewLine}{serialized}{Environment.NewLine}");
                Console.WriteLine("------------------------------------------------------------------------");
            }
        }

        private static async Task getSbaLoanForgivenessBySlug(SbaLoanForgivenessController sbaLoanForgiveness)
        {
            SbaPPPLoanForgiveness loanDocumentType =
                await sbaLoanForgiveness.getSbaLoanForgivenessBySlug("2",
                    "ppp_loan_document_types"); // loanForgivenessUrl - to be passed
            if (loanDocumentType != null)
            {
                var serialized = JsonConvert.SerializeObject(loanDocumentType,
                    new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });
                Console.WriteLine($"{Environment.NewLine}{serialized}{Environment.NewLine}");
                Console.WriteLine("------------------------------------------------------------------------");
            }
        }

        private static void deleteSbaLoanForgiveness(SbaLoanForgivenessController sbaLoanForgiveness)
        {
            sbaLoanForgiveness.deleteSbaLoanForgiveness("2", "ppp_loan_document_types"); // loanForgivenessUrl - to be passed
            Console.WriteLine("------------------------------------------------------------------------");
        }

        public static async Task InvokeSbaLoanForgiveness(SbaLoanForgivenessController sbaLoanForgiveness, SbaForgiveness sbaForgivenessobj)
        {
            Race race = new Race
            {
                race = "1"
            };

            Demographics demographics = new Demographics
            {
                name = "abc",
                position = "xyz",
                veteran_status = "1",
                gender = "M",
                ethnicity = "H",
                races = new List<Race>(1)
                {
                    race
                }
            };

            SbaPPPLoanForgiveness pppLoanForgiveness = new SbaPPPLoanForgiveness
            {
                borrower_name = sbaForgivenessobj.Entity_Name,
                etran_loan = new EtranLoan()
                {
                    demographics = new List<Demographics>(1)
                    {
                        demographics
                    },
                    bank_notional_amount = Convert.ToString(sbaForgivenessobj.pppLoanAmount),
                    sba_number = Convert.ToString(sbaForgivenessobj.sbaLoanNumber),
                    loan_number = Convert.ToString(sbaForgivenessobj.flLoanNumber),
                    entity_name = sbaForgivenessobj.Entity_Name,
                    ein = Convert.ToString(sbaForgivenessobj.einSsn),
                    funding_date = Convert.ToString(sbaForgivenessobj.fundingDate),
                    forgive_eidl_amount = "100.00",
                    forgive_eidl_application_number = 123456789,
                    forgive_payroll = "1000.00",//Convert.ToString(sbaForgivenessobj.pppLoanAmount + 2000),
                    forgive_rent = "1000.00",
                    forgive_utilities = "1000.00",
                    forgive_mortgage = "1000.00",
                    address1 = "stringss",
                    address2 = "stringsss",
                    dba_name = "stringssssss",
                    phone_number = "1234567890",
                    //assign_to_user = null,
                    forgive_fte_at_loan_application = 10,
                    //updated = "06/28/2020",
                    forgive_line_6_3508_or_line_5_3508ez = "3999.00",//Convert.ToString(sbaForgivenessobj.pppLoanAmount + 2000),//Convert.ToString(sbaForgivenessobj.pppLoanAmount),
                    forgive_modified_total = "3999.00",//Convert.ToString(sbaForgivenessobj.pppLoanAmount + 2000),
                    forgive_payroll_cost_60_percent_requirement = "1666.66",//Convert.ToString((sbaForgivenessobj.pppLoanAmount + 2000) * 0.6),//Convert.ToString(sbaForgivenessobj.pppLoanAmount * 0.6),
                    forgive_amount = "1666.66",//Convert.ToString((sbaForgivenessobj.pppLoanAmount)),//Convert.ToString(sbaForgivenessobj.pppLoanAmount * 0.6),
                    forgive_fte_at_forgiveness_application = 10,
                    forgive_schedule_a_line_1 = "1.00",//Convert.ToString(sbaForgivenessobj.pppLoanAmount),
                    forgive_schedule_a_line_2 = "1.00",
                    forgive_schedule_a_line_3_checkbox = true,
                    forgive_schedule_a_line_3 = "1.00",
                    forgive_schedule_a_line_4 = "1.00",
                    forgive_schedule_a_line_5 = "1.00",
                    forgive_schedule_a_line_6 = "1.00",
                    forgive_schedule_a_line_7 = "1.00",
                    forgive_schedule_a_line_8 = "1.00",
                    forgive_schedule_a_line_9 = "1.00",
                    forgive_schedule_a_line_10 = "6.00",//Convert.ToString(sbaForgivenessobj.pppLoanAmount + 2000),
                    forgive_schedule_a_line_10_checkbox = true,
                    forgive_schedule_a_line_11 = "10",
                    forgive_schedule_a_line_12 = "10",
                    forgive_schedule_a_line_13 = "1",

                    forgive_covered_period_from = "2020-07-06",
                    forgive_covered_period_to = "2020-09-06",
                    forgive_alternate_covered_period_from = "2020-07-06",
                    forgive_alternate_covered_period_to = "2020-09-06",
                    forgive_2_million = false,
                    forgive_payroll_schedule = "weekly",
                    forgive_lender_decision = 1,
                    primary_email = "user@example.com",
                    primary_name = "MOCKDATA",
                    ez_form = false,
                    no_reduction_in_employees = true,
                    no_reduction_in_employees_and_covid_impact = true,
                    forgive_lender_confirmation = true
                },
                //created = "06/28/2020",
                //assigned_to_user = " "
            };

            SbaPPPLoanForgiveness sbaPppLoanForgiveness =
                await sbaLoanForgiveness.execute(pppLoanForgiveness, "ppp_loan_forgiveness_requests");
            if (sbaPppLoanForgiveness != null)
            {
                var serialized = JsonConvert.SerializeObject(sbaPppLoanForgiveness,
                    new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });
                Console.WriteLine($"{Environment.NewLine}{serialized}{Environment.NewLine}");
                Console.WriteLine("------------------------------------------------------------------------");
            }
        }

        private static async Task submitLoanDocument(SbaLoanDocumentsController sbaLoanDocuments)
        {
            LoanDocument request = new LoanDocument
            {
                slug = "e4dd77f4-18a4-428a-a26b-c5e9ed19133a",
                name = "testUpload",
                created_at = Convert.ToDateTime("2020-06-28T19:38:52.009454Z"),
                updated_at = Convert.ToDateTime("2020-06-28T19:38:52.009482Z"),
                document =
                    "https://lenders-cooperative-sandbox-app.s3.us-gov-west-1.amazonaws.com/media/loan/e4dd77f4-18a4-428a-a26b-c5e9ed19133a/files/testUpload.docx?AWSAccessKeyId=AKIAQ2EGUESVW7SZX5UO&Signature=TZZBJ%2BGKnpvDBvcnvktCLPmkT10%3D&Expires=1593381648",
                url =
                    "https://lenders-cooperative-sandbox-app.s3.us-gov-west-1.amazonaws.com/media/loan/e4dd77f4-18a4-428a-a26b-c5e9ed19133a/files/testUpload.docx?AWSAccessKeyId=AKIAQ2EGUESVW7SZX5UO&Signature=TZZBJ%2BGKnpvDBvcnvktCLPmkT10%3D&Expires=1593381648",
                etran_loan = "a82c54eb-36f3-460b-ad1d-1c7a6cde4d07",
                document_type = new LoanDocumentType
                {
                    id = 1,
                    name = "Payroll",
                    description = "Payroll Documents"
                }
            };

            LoanDocument loanDocument = await sbaLoanDocuments.submitLoanDocument(request, "ppp_loan_documents");
            if (loanDocument != null)
            {
                var serialized = JsonConvert.SerializeObject(loanDocument,
                    new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });
                Console.WriteLine($"{Environment.NewLine}{serialized}{Environment.NewLine}");
                Console.WriteLine("------------------------------------------------------------------------");
            }
        }

        private static async Task getSbaLoanDocumentTypeById(SbaLoanDocumentsController sbaLoanDocuments)
        {
            LoanDocumentType loanDocumentType =
                await sbaLoanDocuments.getSbaLoanDocumentTypeById(2, "ppp_loan_document_types"); // loanForgivenessUrl - to be passed
            if (loanDocumentType != null)
            {
                var serialized = JsonConvert.SerializeObject(loanDocumentType,
                    new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });
                Console.WriteLine($"{Environment.NewLine}{serialized}{Environment.NewLine}");
                Console.WriteLine("------------------------------------------------------------------------");
            }
        }

        private static async Task getDocumentTypes(SbaLoanDocumentsController sbaLoanDocuments)
        {
            Dictionary<string, string> reqParams = new Dictionary<string, string>();
            reqParams.Add("name", "test");
            reqParams.Add("description", "test");
            reqParams.Add("page", "test");

            SbaPPPLoanDocumentTypeResponse documentTypes =
                await sbaLoanDocuments.getDocumentTypes(reqParams, "ppp_loan_document_types");
            if (documentTypes != null)
            {
                var serialized = JsonConvert.SerializeObject(documentTypes,
                    new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });
                Console.WriteLine($"{Environment.NewLine}{serialized}{Environment.NewLine}");
                Console.WriteLine("------------------------------------------------------------------------");
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
                Console.WriteLine($"{Environment.NewLine}{serialized}{Environment.NewLine}");
                Console.WriteLine("------------------------------------------------------------------------");
            }
        }

        private static async Task getSbaLoanMessages(SbaLoanForgivenessMessageController sbaLoanForgivenessMessageController)
        {
            SbaPPPLoanMessagesResponse sbaPppLoanMessagesResponse =
                await sbaLoanForgivenessMessageController.getSbaLoanMessages(1, "test", true,
                    "ppp_loan_forgiveness_messages");

            if (sbaPppLoanMessagesResponse != null)
            {
                var serialized = JsonConvert.SerializeObject(sbaPppLoanMessagesResponse,
                    new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });
                Console.WriteLine($"{Environment.NewLine}{serialized}{Environment.NewLine}");
                Console.WriteLine("------------------------------------------------------------------------");
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
                Console.WriteLine($"{Environment.NewLine}{serialized}{Environment.NewLine}");
                Console.WriteLine("------------------------------------------------------------------------");
            }
        }
    }
}
