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
        string _getForgivessCuDetails = ConfigurationManager.AppSettings["GetForgivessCuDetails"];

        public string connectionString;

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

                        SbaForgivenessList.Add(fillData(reader));

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
        public void UpdateForgivenessDb(SqlCommand sql_cmnd, double sbaNumber, string status, string error, string slug)
        {
            sql_cmnd.Parameters.AddWithValue("@status", status);
            sql_cmnd.Parameters.AddWithValue("@error", error);
            sql_cmnd.Parameters.AddWithValue("@sbaLoanNumber", Convert.ToString(sbaNumber));
            sql_cmnd.Parameters.AddWithValue("@slug", slug);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            Utility.Utility.LogAction($"Update Stored Procedure Successfully Worked for Sba Number : {sbaNumber} ");
            int r = sql_cmnd.ExecuteNonQuery();
        }

        public void ForgivenessMessage(SqlCommand sql_cmnd, string sbaNumber, string subject, string ticket, string message, bool isCompleted)
        {
            sql_cmnd.Parameters.AddWithValue("@SBANumber", Convert.ToInt32(sbaNumber));
            sql_cmnd.Parameters.AddWithValue("@Subject", subject);
            sql_cmnd.Parameters.AddWithValue("@Ticket", ticket);
            sql_cmnd.Parameters.AddWithValue("@Messages", message);
            sql_cmnd.Parameters.AddWithValue("@IsComplete", isCompleted);

            sql_cmnd.CommandType = CommandType.StoredProcedure;
            Utility.Utility.LogAction($"Update Stored Procedure Successfully Worked for Sba Number : {sbaNumber} ");
            int r = sql_cmnd.ExecuteNonQuery();
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



        public List<ForgiveAdditionalDocuments> GetAdditionalDocuments(string sbaLoanNum)
        {
            List<ForgiveAdditionalDocuments> document = new List<ForgiveAdditionalDocuments>();
            using (SqlConnection _sqlCon = new SqlConnection(connectionString))
            {
                if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Calling " + _selectApplicationProcName + "In data management class");
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

        public ForgivenessDocumentsEZ GetForgivenessDocumentsEZ(string sbaLoanNum)
        {
            ForgivenessDocumentsEZ document = new ForgivenessDocumentsEZ();
            using (SqlConnection _sqlCon = new SqlConnection(connectionString))
            {
                if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Calling " + _selectApplicationProcName + "In data management class");
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
            bool isEZForm = Convert.ToBoolean(reader["IsEzform"]);
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
            sbaForgivenessObj.sbaLoanNumber = Convert.ToDouble(reader["SBALoanNumber"].ToString().Replace("-", ""));
            sbaForgivenessObj.flLoanNumber = Convert.ToDouble(reader["LoanApplicationNumber"].ToString());
            sbaForgivenessObj.fundingDate = Convert.ToDateTime(reader["PPPLoandisbursementDate"]).Year + "-" + Convert.ToDateTime(reader["PPPLoandisbursementDate"]).Month + "-" + Convert.ToDateTime(reader["PPPLoandisbursementDate"]).Day;
            sbaForgivenessObj.Entity_Name = Convert.ToString(reader["PrimaryContact"]);
            sbaForgivenessObj.pppLoanAmount = Convert.ToInt32(reader["PPPLoanAmount"].ToString());
            sbaForgivenessObj.einSsn = Convert.ToInt32(reader["TINNumber"].ToString().Replace("-", ""));
            sbaForgivenessObj.applicationStatus = String.IsNullOrEmpty(Convert.ToString(reader["SBAStatus"])) ? "Awaiting" : Convert.ToString(reader["SBAStatus"]);
            sbaForgivenessObj.forgive_eidl_amount = Convert.ToString(reader["EIDLAdvanceAmount"]) == "" ? "0.00" : Convert.ToString(reader["EIDLAdvanceAmount"]);
            sbaForgivenessObj.forgive_eidl_application_number = Convert.ToInt32(Convert.ToString(reader["EIDLApplicationNumber"]) == string.Empty ? 0 : reader["EIDLApplicationNumber"]);
            sbaForgivenessObj.forgive_payroll = Convert.ToString(reader["PayrollCosts"]);
            sbaForgivenessObj.forgive_rent = Convert.ToString(reader["CFLine3"]);
            sbaForgivenessObj.forgive_utilities = Convert.ToString(reader["CFLine4"]);
            sbaForgivenessObj.forgive_mortgage = Convert.ToString(reader["CFLine2"]);
            sbaForgivenessObj.address1 = Convert.ToString(reader["BusinessAddress1"]);
            sbaForgivenessObj.address2 = Convert.ToString(reader["BusinessAddress2"]);
            sbaForgivenessObj.dba_name = Convert.ToString(reader["DBAName"]);
            sbaForgivenessObj.phone_number = Convert.ToString(reader["BusinessPhone"]);
            sbaForgivenessObj.forgive_fte_at_loan_application = Convert.ToInt32(Convert.ToString(reader["EmployeesAtApplicationTime"]) == "" ? 0 : reader["EmployeesAtApplicationTime"]);
            sbaForgivenessObj.forgive_line_6_3508_or_line_5_3508ez = Convert.ToString(reader["CFLine5"]);
            sbaForgivenessObj.forgive_payroll_cost_60_percent_requirement = Convert.ToString(reader["CFLine7"]) == "" ? "0.00" : Convert.ToString(Math.Round(Convert.ToDouble(reader["CFLine7"]), 2));
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
                sbaForgivenessObj.no_reduction_in_employees = Convert.ToBoolean(string.IsNullOrEmpty(Convert.ToString(reader["NoReductionInEmployees"])) ? false : reader["NoReductionInEmployees"]);
                sbaForgivenessObj.no_reduction_in_employees_and_covid_impact = Convert.ToBoolean(string.IsNullOrEmpty(Convert.ToString(reader["NoreductionInEmployeesAndCovidImpact"])) ? false : true);
            }
            sbaForgivenessObj.forgive_covered_period_from = Convert.ToDateTime(reader["CoveredPeriodFrom"]).Year + "-" + Convert.ToDateTime(reader["CoveredPeriodFrom"]).Month + "-" + Convert.ToDateTime(reader["CoveredPeriodFrom"]).Day;
            sbaForgivenessObj.forgive_covered_period_to = Convert.ToDateTime(reader["CoveredPeriodTo"]).Year + "-" + Convert.ToDateTime(reader["CoveredPeriodTo"]).Month + "-" + Convert.ToDateTime(reader["CoveredPeriodTo"]).Day;
            if (String.IsNullOrEmpty(Convert.ToString(reader["AltCoveredPeriodFrom"])))
            {
                sbaForgivenessObj.forgive_alternate_covered_period_from = Convert.ToDateTime(reader["CoveredPeriodFrom"]).Year + "-" + Convert.ToDateTime(reader["CoveredPeriodFrom"]).Month + "-" + Convert.ToDateTime(reader["CoveredPeriodFrom"]).Day;
            }
            else
            {
                sbaForgivenessObj.forgive_alternate_covered_period_from = Convert.ToDateTime(reader["AltCoveredPeriodFrom"]).Year + "-" + Convert.ToDateTime(reader["AltCoveredPeriodFrom"]).Month + "-" + Convert.ToDateTime(reader["AltCoveredPeriodFrom"]).Day;
            }


            if (String.IsNullOrEmpty(Convert.ToString(reader["AltCoveredPeriodTo"])))
            {
                sbaForgivenessObj.forgive_alternate_covered_period_to = Convert.ToDateTime(reader["CoveredPeriodTo"]).Year + "-" + Convert.ToDateTime(reader["CoveredPeriodTo"]).Month + "-" + Convert.ToDateTime(reader["CoveredPeriodTo"]).Day;
            }
            else
            {
                sbaForgivenessObj.forgive_alternate_covered_period_to = Convert.ToDateTime(reader["AltCoveredPeriodTo"]).Year + "-" + Convert.ToDateTime(reader["AltCoveredPeriodTo"]).Month + "-" + Convert.ToDateTime(reader["AltCoveredPeriodTo"]).Day;
            }
            sbaForgivenessObj.forgive_2_million = Convert.ToString(reader["ExcessPPPLoans"]).ToLower() == "true" ? true : false;
            sbaForgivenessObj.forgive_payroll_schedule = Convert.ToString(reader["PayrollSchedule"]); ;
            sbaForgivenessObj.forgive_lender_decision = Convert.ToInt32(reader["ForgiveLenderDecision"]);
            sbaForgivenessObj.primary_email = Convert.ToString(reader["Email"]);
            sbaForgivenessObj.primary_name = Convert.ToString(reader["PrimaryContact"]); ;
            sbaForgivenessObj.ez_form = isEZForm;
            sbaForgivenessObj.forgive_lender_confirmation = true;
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

        public void UpdateForgivenessDb(string sbaNumber, string status)
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

    }

}
