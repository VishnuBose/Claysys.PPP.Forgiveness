using Claysys.PPP.Forgiveness.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Claysys.PPP.Forgiveness.Model;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;
using Claysys.PPP.Forgiveness.domain;
using System.Web.SessionState;

namespace Claysys.PPP.Forgiveness.DAL
{
    public class DataManagement
    {

        string _selectApplicationProcName = ConfigurationManager.AppSettings["ForgivenessData"];
        string _updateForgivenessStatus = ConfigurationManager.AppSettings["UpdateForgivenessStatusSP"];
        string _selectApplicationStatus = ConfigurationManager.AppSettings["ApplicationStatusData"];
        string CUConnectionString = ConfigurationManager.ConnectionStrings["CUConnectionString"].ConnectionString;
        string _getDocumentEZ = ConfigurationManager.AppSettings["SelectDocumentEZ"];
        string _getDocumentFullApp = ConfigurationManager.AppSettings["SelectDocumentFullApp"];
        string _getDocumentAdditional = ConfigurationManager.AppSettings["SelectDocumentAdditional"];
        string _getDocumentExtra = ConfigurationManager.AppSettings["SelectDocumentExtra"];

        string _getForgivessCuDetails = ConfigurationManager.AppSettings["GetForgivessCuDetails"];
        string _updateForgivessPaymentDetails = ConfigurationManager.AppSettings["UpdateForgivenessPayment"];
        string _selectDocumentCount = ConfigurationManager.AppSettings["SelectDocumentCount"];
        string _insertForgivenessSBADoc = ConfigurationManager.AppSettings["ForgivenessSBADoc"];


        public static Object lockObj =  new object();
        public string connectionString;
        public string cuName;

        public List<SbaForgiveness> GetForgivenessDetails()
        {

            List<SbaForgiveness> SbaForgivenessList = new List<SbaForgiveness>();
            using (SqlConnection _sqlCon = new SqlConnection(connectionString))
            {
                if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Calling " + _selectApplicationProcName + "In data management class");
                try
                {
                    _sqlCon.Open();

                    SqlCommand sql_cmnd = new SqlCommand(_selectApplicationProcName, _sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = sql_cmnd.ExecuteReader();

                    while (reader.Read())
                    {
                        try
                        {
                            SbaForgivenessList.Add(fillData(reader));
                        }
                        catch (Exception ex)
                        {
                            Utility.Utility.LogAction("Error while getting data from DB " + ex);
                        }

                    }
                    if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("GetForgivenessDetails Stored Procedure executed successfully");
                }
                catch (Exception ex)
                {
                    Utility.Utility.LogAction("GetForgivenessDetails Stored Procedure execution failed with error " + ex.Message);

                }

                finally
                {
                    _sqlCon.Close();
                }

            }
            return SbaForgivenessList;
        }


        public List<CreditUnionData> GetCreditUnionDetails()
        {
            List<CreditUnionData> sbaDataObj = new List<CreditUnionData>();
            using (SqlConnection _sqlCon = new SqlConnection(CUConnectionString))
            {
                try
                {
                    _sqlCon.Open();
                    SqlCommand sql_cmnd = new SqlCommand(_getForgivessCuDetails, _sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = sql_cmnd.ExecuteReader();
                    while (reader.Read())
                    {
                        sbaDataObj.Add(new CreditUnionData
                        {
                            CUName = Convert.ToString(reader["CUName"]),
                            Vendorkey = Convert.ToString(reader["Vendorkey"]),
                            ConnectionString = Convert.ToString(reader["ConnectionString"]),
                            Token = Convert.ToString(reader["Token"])
                        });
                    }
                }
                catch (Exception ex)
                {
                    Utility.Utility.LogAction("Stored Procedure execution failed with error " + ex.Message);
                }
                finally
                {
                    _sqlCon.Close();
                }
                return sbaDataObj;
            }
        }

        public void UpdateForgivenessPaymentDetails(SbaPPPLoanForgivenessStatusResponse sbsData, string conString)
        {
            using (SqlConnection _sqlCon = new SqlConnection(conString))
            {
                try
                {
                    _sqlCon.Open();
                    SqlCommand sql_cmnd = new SqlCommand(_updateForgivessPaymentDetails, _sqlCon);
                    sql_cmnd.Parameters.AddWithValue("@BorrowerName", sbsData.results[0].borrower_name == null ? "" : sbsData.results[0].borrower_name);
                    sql_cmnd.Parameters.AddWithValue("@SBANumber", sbsData.results[0].etran_loan.sba_number == null ? "" : sbsData.results[0].etran_loan.sba_number);
                    sql_cmnd.Parameters.AddWithValue("@DisbursementDate", sbsData.results[0].etran_loan.funding_date == null ? "" : sbsData.results[0].etran_loan.funding_date);
                    sql_cmnd.Parameters.AddWithValue("@LoanAmount", sbsData.results[0].etran_loan.bank_notional_amount == null ? "" : Convert.ToString(sbsData.results[0].etran_loan.bank_notional_amount));
                    sql_cmnd.Parameters.AddWithValue("@FinalForgiveAmount", sbsData.results[0].etran_loan.final_forgive_amount == null ? "" : sbsData.results[0].etran_loan.final_forgive_amount);
                    sql_cmnd.Parameters.AddWithValue("@SBADicisionDate", sbsData.results[0].etran_loan.approval_date == null ? "" : Convert.ToString(sbsData.results[0].etran_loan.approval_date));
                    sql_cmnd.Parameters.AddWithValue("@CalculatedInterest", sbsData.results[0].etran_loan.calculated_interest == null ? "" : Convert.ToString(sbsData.results[0].etran_loan.calculated_interest));
                    //sql_cmnd.Parameters.AddWithValue("@EIDLAdvanceReductionAmount", sbsData.results[0].etran_loan.forgive_eidl_amount == null ? "" : Convert.ToString(sbsData.results[0].etran_loan.forgive_eidl_amount));
                    sql_cmnd.Parameters.AddWithValue("@Payment", sbsData.results[0].etran_loan.final_forgive_amount_with_interest == null ? "" : Convert.ToString(sbsData.results[0].etran_loan.final_forgive_amount_with_interest));
                    sql_cmnd.Parameters.AddWithValue("@PaymentDate", sbsData.results[0].etran_loan.final_forgive_payment_date == null ? "" : Convert.ToString(sbsData.results[0].etran_loan.final_forgive_payment_date));
                    sql_cmnd.Parameters.AddWithValue("@PaymentBatch", sbsData.results[0].etran_loan.final_forgive_payment_batch == null ? "" : sbsData.results[0].etran_loan.final_forgive_payment_batch);
                    sql_cmnd.Parameters.AddWithValue("@PaymentStatus", sbsData.results[0].etran_loan.payment_status == null ? "" : sbsData.results[0].etran_loan.payment_status);
                    sql_cmnd.Parameters.AddWithValue("@SBADecision", sbsData.results[0].etran_loan.sba_decision == null ? "" : sbsData.results[0].etran_loan.sba_decision);


                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    int r = sql_cmnd.ExecuteNonQuery();
                    _sqlCon.Close();
                }
                catch (Exception ex)
                {
                    Utility.Utility.LogAction($"SBALoan : {sbsData.results[0].etran_loan.sba_number}, Update payment Details Failed, {ex}");
                }

            }
        }

        public void UpdateForgivenessDb(SqlCommand sql_cmnd, double sbaNumber, string status, string error, string slug)
        {
            sql_cmnd.Parameters.AddWithValue("@status", status);
            sql_cmnd.Parameters.AddWithValue("@error", error);
            sql_cmnd.Parameters.AddWithValue("@sbaLoanNumber", Convert.ToString(sbaNumber));
            sql_cmnd.Parameters.AddWithValue("@slug", slug);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            int r = sql_cmnd.ExecuteNonQuery();
            Utility.Utility.LogAction($"Update Stored Procedure Successfully Worked for Sba Number : {sbaNumber} ");
        }

        public void ForgivenessMessage(SqlCommand sql_cmnd, string sbaNumber, string subject, string ticket, string message, bool isCompleted)
        {
            sql_cmnd.Parameters.AddWithValue("@SBANumber", Convert.ToInt64(sbaNumber));
            sql_cmnd.Parameters.AddWithValue("@Subject", subject);
            sql_cmnd.Parameters.AddWithValue("@Ticket", ticket);
            sql_cmnd.Parameters.AddWithValue("@Messages", message);
            sql_cmnd.Parameters.AddWithValue("@IsComplete", isCompleted);

            sql_cmnd.CommandType = CommandType.StoredProcedure;
            int r = sql_cmnd.ExecuteNonQuery();
            Utility.Utility.LogAction($"Update Stored Procedure Successfully Worked for Sba Number : {sbaNumber} ");
        }

        public void ForgivenessDocument(SqlCommand sql_cmnd, string sbaNumber, string Slug, string Name, string CreatedAt, string UpdatedAt, string Document, int DocumentType, string Url, string EtranLoan)
        {
            sql_cmnd.Parameters.AddWithValue("@SBANumber", sbaNumber);
            sql_cmnd.Parameters.AddWithValue("@Slug", Slug);
            sql_cmnd.Parameters.AddWithValue("@Name", Name);
            sql_cmnd.Parameters.AddWithValue("@CreatedAt", CreatedAt);
            sql_cmnd.Parameters.AddWithValue("@UpdatedAt", UpdatedAt);
            sql_cmnd.Parameters.AddWithValue("@Document", Document);
            sql_cmnd.Parameters.AddWithValue("@DocumentType", DocumentType);
            sql_cmnd.Parameters.AddWithValue("@Url", Url);
            sql_cmnd.Parameters.AddWithValue("@EtranLoan", EtranLoan);

            sql_cmnd.CommandType = CommandType.StoredProcedure;
            Utility.Utility.LogAction($"Update Stored Procedure Successfully Worked for Sba Number : {sbaNumber}, Document : {Document} && Document Type : {DocumentType}");
            int r = sql_cmnd.ExecuteNonQuery();
        }


        public List<ForgiveAdditionalDocuments> GetExtraDocuments()
        {
            List<ForgiveAdditionalDocuments> document = new List<ForgiveAdditionalDocuments>();
            using (SqlConnection _sqlCon = new SqlConnection(connectionString))
            {
                if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Calling " + _getDocumentAdditional + "In data management class");
                try
                {
                    _sqlCon.Open();

                    SqlCommand sql_cmnd = new SqlCommand(_getDocumentExtra, _sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = sql_cmnd.ExecuteReader();

                    while (reader.Read())
                    {
                        var doc = new ForgiveAdditionalDocuments();
                        doc.fileName = Convert.ToString(reader["FileName"]);
                        if (!string.IsNullOrEmpty(doc.fileName))
                            doc.fileContent = (byte[])(reader["FileContent"]);
                        doc.DocumentType = Convert.ToString(reader["DocumentType"]);
                        doc.SBALoanNo = Convert.ToString(reader["SBALoanNo"]);
                        doc.SlugID = Convert.ToString(reader["SlugID"]);
                        document.Add(doc);
                    }
                }
                catch (Exception ex)
                {
                    Utility.Utility.LogAction(_selectApplicationProcName + " Stored Procedure execution failed with error " + ex.Message);
                }

                finally
                {
                    _sqlCon.Close();
                }

            }
            return document;
        }
        public List<ForgiveAdditionalDocuments> GetAdditionalDocuments(string sbaLoanNum)
        {
            List<ForgiveAdditionalDocuments> document = new List<ForgiveAdditionalDocuments>();
            using (SqlConnection _sqlCon = new SqlConnection(connectionString))
            {
                if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Calling " + _getDocumentAdditional + "In data management class");
                try
                {
                    _sqlCon.Open();

                    SqlCommand sql_cmnd = new SqlCommand(_getDocumentAdditional, _sqlCon);
                    sql_cmnd.Parameters.AddWithValue("@sbaLoanNumber", sbaLoanNum);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = sql_cmnd.ExecuteReader();

                    while (reader.Read())
                    {
                        var doc = new ForgiveAdditionalDocuments();
                        doc.fileName = Convert.ToString(reader["FileName"]);
                        if (!string.IsNullOrEmpty(doc.fileName))
                            doc.fileContent = (byte[])(reader["FileContent"]);
                        doc.DocumentType = Convert.ToString(reader["DocumentType"]);
                        document.Add(doc);
                    }
                    if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("GetForgivenessDetails Stored Procedure executed successfully");
                }
                catch (Exception ex)
                {
                    Utility.Utility.LogAction("GetForgivenessDetails Stored Procedure execution failed with error " + ex.Message);
                }

                finally
                {
                    _sqlCon.Close();
                }

            }
            return document;
        }

        public ForgivenessDocumentsFullApp GetForgivenessDocumentsFullApp(string sbaLoanNum)
        {
            ForgivenessDocumentsFullApp document = new ForgivenessDocumentsFullApp();
            using (SqlConnection _sqlCon = new SqlConnection(connectionString))
            {
                if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Calling " + _selectApplicationProcName + "In data management class");
                try
                {
                    _sqlCon.Open();

                    SqlCommand sql_cmnd = new SqlCommand(_getDocumentFullApp, _sqlCon);
                    sql_cmnd.Parameters.AddWithValue("@sbaLoanNumber", sbaLoanNum);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = sql_cmnd.ExecuteReader();

                    while (reader.Read())
                    {
                        document.PayrollCompensationName = Convert.ToString(reader["PayrollCompensationName"]);
                        if (!string.IsNullOrEmpty(document.PayrollCompensationName))
                            document.PayrollCompensationFile = (byte[])(reader["PayrollCompensationFile"]);
                        document.PayrollTaxFormName = Convert.ToString(reader["PayrollTaxFormName"]);
                        if (!string.IsNullOrEmpty(document.PayrollTaxFormName))
                            document.PayrollTaxFormFile = (byte[])(reader["PayrollTaxFormFile"]);
                        document.PayrollPayementsName = Convert.ToString(reader["PayrollPayementsName"]);
                        if (!string.IsNullOrEmpty(document.PayrollPayementsName))
                            document.PayrollPayementsFile = (byte[])(reader["PayrollPayementsFile"]);
                        document.FTEDocumentationName1 = Convert.ToString(reader["FTEDocumentationName1"]);
                        if (!string.IsNullOrEmpty(document.FTEDocumentationName1))
                            document.FTEDocumentFile1 = (byte[])(reader["FTEDocumentFile1"]);
                        document.FTEDocumentationName2 = Convert.ToString(reader["FTEDocumentationName2"]);
                        if (!string.IsNullOrEmpty(document.FTEDocumentationName2))
                            document.FTEDocumentFile2 = (byte[])(reader["FTEDocumentFile2"]);
                        document.FTEDocumentationName3 = Convert.ToString(reader["FTEDocumentationName3"]);
                        if (!string.IsNullOrEmpty(document.FTEDocumentationName3))
                            document.FTEDocumentFile3 = (byte[])(reader["FTEDocumentFile3"]);
                        document.NonpayrollName1 = Convert.ToString(reader["NonpayrollName1"]);
                        if (!string.IsNullOrEmpty(document.NonpayrollName1))
                            document.NonpayrollFile1 = (byte[])(reader["NonpayrollFile1"]);
                        document.NonpayrollName2 = Convert.ToString(reader["NonpayrollName2"]);
                        if (!string.IsNullOrEmpty(document.NonpayrollName2))
                            document.NonpayrollFile2 = (byte[])(reader["NonpayrollFile2"]);
                        document.NonpayrollName3 = Convert.ToString(reader["NonpayrollName3"]);
                        if (!string.IsNullOrEmpty(document.NonpayrollName3))
                            document.NonpayrollFile3 = (byte[])(reader["NonpayrollFile3"]);
                        document.AdditionalDocumentName1 = Convert.ToString(reader["AdditionalDocumentName1"]);
                        if (!string.IsNullOrEmpty(document.AdditionalDocumentName1))
                            document.AdditionalDocumentFile1 = (byte[])(reader["AdditionalDocumentFile1"]);
                        document.AdditionalDocumentName2 = Convert.ToString(reader["AdditionalDocumentName2"]);
                        if (!string.IsNullOrEmpty(document.AdditionalDocumentName2))
                            document.AdditionalDocumentFile2 = (byte[])(reader["AdditionalDocumentFile2"]);
                        document.AdditionalDocumentName3 = Convert.ToString(reader["AdditionalDocumentName3"]);
                        if (!string.IsNullOrEmpty(document.AdditionalDocumentName3))
                            document.AdditionalDocumentFile3 = (byte[])(reader["AdditionalDocumentFile3"]);
                        document.AdditionalDocumentName4 = Convert.ToString(reader["AdditionalDocumentName4"]);
                        if (!string.IsNullOrEmpty(document.AdditionalDocumentName4))
                            document.AdditionalDocumentFile4 = (byte[])(reader["AdditionalDocumentFile4"]);
                        document.CustomerSafteyFileName = Convert.ToString(reader["CustomerSafteyFileName"]);
                        if (!string.IsNullOrEmpty(document.CustomerSafteyFileName))
                            document.CustomerSafteyFile = (byte[])(reader["CustomerSafteyFile"]);
                    }
                    if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("GetForgivenessDetails Stored Procedure executed successfully");
                }
                catch (Exception ex)
                {
                    Utility.Utility.LogAction("GetForgivenessDetails Stored Procedure execution failed with error " + ex.Message);

                }

                finally
                {
                    _sqlCon.Close();
                }

            }
            return document;
        }


        public int GetForgivenessDocumentsCount(string sbaLoanNum, string fileName)
        {
            int count = 0;
            using (SqlConnection _sqlCon = new SqlConnection(connectionString))
            {
                if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Calling " + _selectDocumentCount + "In data management class");
                try
                {
                    _sqlCon.Open();

                    SqlCommand sql_cmnd = new SqlCommand(_selectDocumentCount, _sqlCon);
                    sql_cmnd.Parameters.AddWithValue("@sbaloanno", sbaLoanNum);
                    sql_cmnd.Parameters.AddWithValue("@filename", fileName);

                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = sql_cmnd.ExecuteReader();

                    while (reader.Read())
                    {
                        count = Convert.ToInt32(reader["Count"]);
                    }
                    if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("GetForgivenessDocumentsCount Stored Procedure executed successfully");
                }
                catch (Exception ex)
                {
                    Utility.Utility.LogAction("GetForgivenessDocumentsCount Stored Procedure execution failed with error " + ex.Message);
                }

                finally
                {
                    _sqlCon.Close();
                }
            }
            return count;
        }
        public ForgivenessDocumentsEZ GetForgivenessDocumentsEZ(string sbaLoanNum)
        {
            ForgivenessDocumentsEZ document = new ForgivenessDocumentsEZ();
            using (SqlConnection _sqlCon = new SqlConnection(connectionString))
            {
                if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Calling " + _getDocumentEZ + "In data management class");
                try
                {
                    _sqlCon.Open();

                    SqlCommand sql_cmnd = new SqlCommand(_getDocumentEZ, _sqlCon);
                    sql_cmnd.Parameters.AddWithValue("@sbaLoanNumber", sbaLoanNum);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = sql_cmnd.ExecuteReader();

                    while (reader.Read())
                    {
                        document.PayrollAName = Convert.ToString(reader["PayrollAName"]);
                        if (!string.IsNullOrEmpty(document.PayrollAName))
                            document.PayrollAFile = (byte[])reader["PayrollAFile"];
                        document.PayrollBName = Convert.ToString(reader["PayrollBName"]);
                        if (!string.IsNullOrEmpty(document.PayrollBName))
                            document.PayrollBFile = (byte[])(reader["PayrollBFile"]);
                        document.PayrollCName = Convert.ToString(reader["PayrollCName"]);
                        if (!string.IsNullOrEmpty(document.PayrollCName))
                            document.PayrollCFile = (byte[])(reader["PayrollCFile"]);
                        document.PayrollDName = Convert.ToString(reader["PayrollDName"]);
                        if (!string.IsNullOrEmpty(document.PayrollDName))
                            document.PayrollDFile = (byte[])(reader["PayrollDFile"]);
                        document.NonPayrollAName = Convert.ToString(reader["NonPayrollAName"]);
                        if (!string.IsNullOrEmpty(document.NonPayrollAName))
                            document.NonPayrollAFile = (byte[])(reader["NonPayrollAFile"]);
                        document.NonPayrollBName = Convert.ToString(reader["NonPayrollBName"]);
                        if (!string.IsNullOrEmpty(document.NonPayrollBName))
                            document.NonPayrollBFile = (byte[])(reader["NonPayrollBFile"]);
                        document.NonPayrollCName = Convert.ToString(reader["NonPayrollCName"]);
                        if (!string.IsNullOrEmpty(document.NonPayrollCName))
                            document.NonPayrollCFile = (byte[])(reader["NonPayrollCFile"]);
                        document.CertifySalaryName = Convert.ToString(reader["CertifySalaryName"]);
                        if (!string.IsNullOrEmpty(document.CertifySalaryName))
                            document.CertifySalaryFile = (byte[])(reader["CertifySalaryFile"]);
                        document.EmployeeJobName = Convert.ToString(reader["EmployeeJobName"]);
                        if (!string.IsNullOrEmpty(document.EmployeeJobName))
                            document.EmployeeJobFile = (byte[])(reader["EmployeeJobFile"]);
                        document.EmployeeNosName = Convert.ToString(reader["EmployeeNosName"]);
                        if (!string.IsNullOrEmpty(document.EmployeeNosName))
                            document.EmployeeNosFile = (byte[])(reader["EmployeeNosFile"]);
                        document.CompanyOpsName = Convert.ToString(reader["CompanyOpsName"]);
                        if (!string.IsNullOrEmpty(document.CompanyOpsName))
                            document.CompanyOpsFile = (byte[])(reader["CompanyOpsFile"]);
                        document.SupportAllDocsName = Convert.ToString(reader["SupportAllDocsName"]);
                        if (!string.IsNullOrEmpty(document.SupportAllDocsName))
                            document.SupportAllDocsFile = (byte[])(reader["SupportAllDocsFile"]);
                    }
                    if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("GetForgivenessDetails Stored Procedure executed successfully");
                }
                catch (Exception ex)
                {
                    Utility.Utility.LogAction("GetForgivenessDetails Stored Procedure execution failed with error " + ex.Message);

                }

                finally
                {
                    _sqlCon.Close();
                }

            }
            return document;
        }

        public SbaForgiveness fillData(SqlDataReader reader)
        {
            bool isSmallForm = Convert.ToString(reader["IsEzform"]) == "SmallApp" ? true : false;
            bool isEZForm;
            if (!isSmallForm)
                isEZForm = Convert.ToBoolean(reader["IsEzform"]);
            else
                isEZForm = true;
            long intValue;

            Race race = new Race
            {
                race = Convert.ToString(reader["DemoRace"])
            };
            Demographics demographics = new Demographics
            {
                name = Convert.ToString(reader["DemoName"]),
                position = Convert.ToString(reader["DemoPosition"]),
                veteran_status = Convert.ToString(reader["DemoVeteran"]),
                gender = Convert.ToString(reader["DemoGender"]),
                ethnicity = Convert.ToString(reader["DemoEthinicity"]),
                races = new List<Race>(1)
                {
                    race
                }
            };
            SbaForgiveness sbaForgivenessObj = new SbaForgiveness();
            sbaForgivenessObj.demographics = new List<Demographics>(1)
                    {
                        demographics
                    };
            sbaForgivenessObj.sbaLoanNumber = Convert.ToDouble(Convert.ToString(reader["SBALoanNumber"]).Replace("-", ""));
            sbaForgivenessObj.flLoanNumber = Convert.ToString(reader["LenderPPPLoanNumber"].ToString());
            sbaForgivenessObj.fundingDate = Convert.ToDateTime(reader["PPPLoandisbursementDate"]).Year + "-" + Convert.ToDateTime(reader["PPPLoandisbursementDate"]).Month + "-" + Convert.ToDateTime(reader["PPPLoandisbursementDate"]).Day;
            sbaForgivenessObj.Entity_Name = Convert.ToString(reader["EntityName"]);
            sbaForgivenessObj.pppLoanAmount = Convert.ToDouble(Convert.ToString(reader["PPPLoanAmount"]));
            sbaForgivenessObj.einSsn = Convert.ToString(reader["TINNumber"]).Replace("-", "");
            sbaForgivenessObj.applicationStatus = String.IsNullOrEmpty(Convert.ToString(reader["SBAStatus"])) ? "Awaiting" : Convert.ToString(reader["SBAStatus"]);
            //sbaForgivenessObj.forgive_eidl_amount = Convert.ToString(reader["EIDLAdvanceAmount"]) == "" ? "0.00" : Convert.ToString(reader["EIDLAdvanceAmount"]);
            //sbaForgivenessObj.forgive_eidl_application_number = Int64.TryParse(Convert.ToString(reader["EIDLApplicationNumber"]), out intValue) ? intValue : (long?)null;
            //sbaForgivenessObj.forgive_eidl_application_number = Convert.ToInt64(String.IsNullOrWhiteSpace(Convert.ToString(reader["EIDLApplicationNumber"])) ? 0 : reader["EIDLApplicationNumber"]);
            sbaForgivenessObj.forgive_payroll = string.IsNullOrEmpty(Convert.ToString(reader["PayrollCosts"]))?"0.00": Convert.ToString(reader["PayrollCosts"]);
            if (!isSmallForm)
            {
                
                sbaForgivenessObj.forgive_rent = Convert.ToString(reader["CFLine3"]);
                sbaForgivenessObj.forgive_utilities = Convert.ToString(reader["CFLine4"]);
                sbaForgivenessObj.forgive_mortgage = Convert.ToString(reader["CFLine2"]);
            }

            sbaForgivenessObj.address1 = Convert.ToString(reader["BusinessAddress1"]);
            sbaForgivenessObj.address2 = Convert.ToString(reader["BusinessAddress2"]);
            sbaForgivenessObj.dba_name = Convert.ToString(reader["DBAName"]);
            sbaForgivenessObj.phone_number = Convert.ToString(reader["BusinessPhone"]);
            sbaForgivenessObj.forgive_fte_at_loan_application = Convert.ToInt32(Convert.ToString(reader["EmployeesAtApplicationTime"]) == "" ? 0 : reader["EmployeesAtApplicationTime"]);

            if (!isSmallForm)
            {
                sbaForgivenessObj.forgive_line_6_3508_or_line_5_3508ez = Convert.ToString(reader["CFLine5"]); // Line 9 of new form
                sbaForgivenessObj.forgive_payroll_cost_60_percent_requirement = Convert.ToString(reader["CFLine7"]) == "" ? "0.00" : Convert.ToString(Math.Round(Convert.ToDouble(reader["CFLine7"]), 2));
            }

            sbaForgivenessObj.forgive_amount = Convert.ToString(reader["CFLine8"]);


            sbaForgivenessObj.forgive_fte_at_forgiveness_application = Convert.ToInt32(Convert.ToString(reader["EmployeesAtForgivenessTime"]) == "" ? 0 : reader["EmployeesAtForgivenessTime"]);


            if (!isEZForm)
            {
                sbaForgivenessObj.forgive_modified_total = Convert.ToString(reader["ForgiveModifiedTotal"]);
                sbaForgivenessObj.forgive_schedule_a_line_1 = Convert.ToString(reader["ForgiveScheduleALine1"]);
                sbaForgivenessObj.forgive_schedule_a_line_2 = Convert.ToString(reader["ForgiveScheduleALine2"]);
                sbaForgivenessObj.forgive_schedule_a_line_3_checkbox = Convert.ToBoolean(string.IsNullOrEmpty(Convert.ToString(reader["ForgiveScheduleALine3Chk"])) ? false : reader["ForgiveScheduleALine3Chk"]);
                sbaForgivenessObj.forgive_schedule_a_line_3 = Convert.ToString(reader["ForgiveScheduleALine3"]);
                sbaForgivenessObj.forgive_schedule_a_line_4 = Convert.ToString(reader["ForgiveScheduleALine4"]);
                sbaForgivenessObj.forgive_schedule_a_line_5 = Convert.ToString(reader["ForgiveScheduleALine5"]);
                sbaForgivenessObj.forgive_schedule_a_line_6 = Convert.ToString(reader["ForgiveScheduleALine6"]);
                sbaForgivenessObj.forgive_schedule_a_line_7 = Convert.ToString(reader["ForgiveScheduleALine7"]);
                sbaForgivenessObj.forgive_schedule_a_line_8 = Convert.ToString(reader["ForgiveScheduleALine8"]);
                sbaForgivenessObj.forgive_schedule_a_line_9 = Convert.ToString(reader["ForgiveScheduleALine9"]);
                sbaForgivenessObj.forgive_schedule_a_line_10 = Convert.ToString(reader["ForgiveScheduleALine10"]);
                sbaForgivenessObj.forgive_schedule_a_line_10_checkbox = Convert.ToBoolean(string.IsNullOrEmpty(Convert.ToString(reader["ForgiveScheduleALine10Chk"])) ? false : reader["ForgiveScheduleALine10Chk"]);
                sbaForgivenessObj.forgive_schedule_a_line_11 = Convert.ToString(reader["ForgiveScheduleALine11"]);
                sbaForgivenessObj.forgive_schedule_a_line_12 = Convert.ToString(reader["ForgiveScheduleALine12"]);
                sbaForgivenessObj.forgive_schedule_a_line_13 = Convert.ToString(reader["ForgiveScheduleALine13"]);
                
            }

            if (!isSmallForm && !isEZForm)
            {
                sbaForgivenessObj.no_reduction_in_employees_and_covid_impact = Convert.ToBoolean(string.IsNullOrEmpty(Convert.ToString(reader["NoreductionInEmployeesAndCovidImpact"])) ? false : reader["NoreductionInEmployeesAndCovidImpact"]);
                sbaForgivenessObj.no_reduction_in_employees = Convert.ToBoolean(string.IsNullOrEmpty(Convert.ToString(reader["NoReductionInEmployees"])) ? false : reader["NoReductionInEmployees"]);
            }

            if (!isSmallForm)
            {

                sbaForgivenessObj.forgive_covered_operations_expenditures = Convert.ToString(reader["forgive_covered_operations_expenditures"]) == "" ? "0" : Convert.ToString(reader["forgive_covered_operations_expenditures"]);  //needs to fill
                sbaForgivenessObj.forgive_covered_property_damage_costs = Convert.ToString(reader["forgive_covered_property_damage_costs"]) == "" ? "0" : Convert.ToString(reader["forgive_covered_property_damage_costs"]); //needs to fill
                sbaForgivenessObj.forgive_covered_supplier_costs = Convert.ToString(reader["forgive_covered_supplier_costs"]) == "" ? "0" : Convert.ToString(reader["forgive_covered_supplier_costs"]); //needs to fill
                sbaForgivenessObj.forgive_covered_protection_expenditures = Convert.ToString(reader["forgive_covered_protection_expenditures"]) == "" ? "0" : Convert.ToString(reader["forgive_covered_protection_expenditures"]);//needs to fill
                //sbaForgivenessObj.forgive_payroll_schedule = Convert.ToString(reader["PayrollSchedule"]);
            }
            sbaForgivenessObj.forgive_2_million = Convert.ToString(reader["ExcessPPPLoans"]).ToLower() == "true" ? true : false;
            sbaForgivenessObj.forgive_covered_period_from = Convert.ToDateTime(reader["CoveredPeriodFrom"]).Year + "-" + Convert.ToDateTime(reader["CoveredPeriodFrom"]).Month + "-" + Convert.ToDateTime(reader["CoveredPeriodFrom"]).Day;
            sbaForgivenessObj.forgive_covered_period_to = Convert.ToDateTime(reader["CoveredPeriodTo"]).Year + "-" + Convert.ToDateTime(reader["CoveredPeriodTo"]).Month + "-" + Convert.ToDateTime(reader["CoveredPeriodTo"]).Day;
            sbaForgivenessObj.forgive_lender_decision = Convert.ToInt32(reader["ForgiveLenderDecision"]);
            sbaForgivenessObj.primary_email = Convert.ToString(reader["Email"]);
            sbaForgivenessObj.primary_name = Convert.ToString(reader["PrimaryContact"]); ;
            sbaForgivenessObj.ez_form = isSmallForm ? false : isEZForm;
            sbaForgivenessObj.s_form = isSmallForm;
            sbaForgivenessObj.forgive_lender_confirmation = true;
            sbaForgivenessObj.naics_code = Convert.ToString(reader["NAICS"]); ; // needs to fill
            sbaForgivenessObj.ppp_loan_draw = Convert.ToInt32(string.IsNullOrEmpty(Convert.ToString(reader["PPPLoanDraw"]))?1: reader["PPPLoanDraw"]); //needs to fill 
         
            return sbaForgivenessObj;
        }

        public List<ForgivenessData> SelectApplicationStatus()
        {
            List<ForgivenessData> sbaForgiveness = new List<ForgivenessData>();
            using (SqlConnection _sqlCon = new SqlConnection(connectionString))
            {
                if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Calling " + _selectApplicationStatus + "In data management class");
                try
                {
                    _sqlCon.Open();

                    SqlCommand sql_cmnd = new SqlCommand(_selectApplicationStatus, _sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = sql_cmnd.ExecuteReader();

                    while (reader.Read())
                    {
                        sbaForgiveness.Add(FillStatusData(reader));

                    }
                    if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Procedure for Forgiveness loan status executed successfully.");
                }
                catch (Exception ex)
                {
                    Utility.Utility.LogAction("Procedure for Forgiveness loan status execution failed " + ex.Message);

                }

                finally
                {
                    _sqlCon.Close();
                }

            }
            return sbaForgiveness;
        }

        public static ForgivenessData FillStatusData(SqlDataReader reader)
        {
            ForgivenessData sbaForgivenessobj = new ForgivenessData();
            sbaForgivenessobj.SbaLoanNumber = Convert.ToString(reader["SBALoanNo"]).Replace("-", "");
            sbaForgivenessobj.ApplicationStatus = Convert.ToString(reader["Status"]);
            return sbaForgivenessobj;
        }

        public void UpdateForgivenessDb(string sbaNumber, string status, string slugID)
        {
            using (SqlConnection _sqlCon = new SqlConnection(connectionString))
            {
                if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Calling " + _updateForgivenessStatus + "In data management class");
                try
                {
                    _sqlCon.Open();
                    SqlCommand sql_cmnd = new SqlCommand(_updateForgivenessStatus, _sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    sql_cmnd.Parameters.AddWithValue("@status", status);
                    sql_cmnd.Parameters.AddWithValue("@sbaLoanNumber", sbaNumber);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    int r = sql_cmnd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {

                    Utility.Utility.LogAction("Failed UpdateForgivenessDb's stored procedure with error " + ex.Message);

                }
                finally
                {
                    _sqlCon.Close();
                }

            }
        }


        public void InsertForgivenessSBADoc(string sbaNumber, string url, string document)
        {
            using (SqlConnection _sqlCon = new SqlConnection(connectionString))
            {
                if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Calling " + _insertForgivenessSBADoc + "In data management class");
                try
                {
                    _sqlCon.Open();
                    SqlCommand sql_cmnd = new SqlCommand(_insertForgivenessSBADoc, _sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    sql_cmnd.Parameters.AddWithValue("@url", url);
                    sql_cmnd.Parameters.AddWithValue("@sbaloanno", sbaNumber);
                    sql_cmnd.Parameters.AddWithValue("@document", document);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    int r = sql_cmnd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {

                    Utility.Utility.LogAction("Failed Update ForgivenessSBADoc table with error " + ex.Message);

                }
                finally
                {
                    _sqlCon.Close();
                }

            }
        }

    }

}
