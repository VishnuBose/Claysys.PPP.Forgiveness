using Claysys.PPP.Forgiveness.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claysys.PPP.Forgiveness.Model
{
    public class SbaForgiveness
    {
        public double sbaLoanNumber { get; set; }
        public double flLoanNumber { get; set; }
        public string fundingDate { get; set; }
        public string Entity_Name { get; set; }
        public float pppLoanAmount { get; set; }
        public int einSsn { get; set; }
        public string applicationStatus { get; set; }
        public string error { get; set; }
        public string slug { get; set; }

        public string forgive_eidl_amount { get; set; }

        public int forgive_eidl_application_number { get; set; }

        public string forgive_payroll { get; set; }

        public string forgive_rent { get; set; }

        public string forgive_utilities { get; set; }

        public string forgive_mortgage { get; set; }

        public string address1 { get; set; }

        public string address2 { get; set; }

        public string dba_name { get; set; }

        public string phone_number { get; set; }

        public int forgive_fte_at_loan_application { get; set; }

        public List<Demographics> demographics { get; set; }

        public List<LoanDocument> documents { get; set; }

        public BankStatus bank_status { get; set; }

        public string forgive_line_6_3508_or_line_5_3508ez { get; set; }

        public string forgive_modified_total { get; set; }

        public string forgive_payroll_cost_60_percent_requirement { get; set; }

        public string forgive_amount { get; set; }

        public int forgive_fte_at_forgiveness_application { get; set; }

        public string forgive_schedule_a_line_1 { get; set; }

        public string forgive_schedule_a_line_2 { get; set; }

        public bool forgive_schedule_a_line_3_checkbox { get; set; }

        public string forgive_schedule_a_line_3 { get; set; }

        public string forgive_schedule_a_line_4 { get; set; }

        public string forgive_schedule_a_line_5 { get; set; }

        public string forgive_schedule_a_line_6 { get; set; }

        public string forgive_schedule_a_line_7 { get; set; }

        public string forgive_schedule_a_line_8 { get; set; }

        public string forgive_schedule_a_line_9 { get; set; }

        public string forgive_schedule_a_line_10 { get; set; }

        public bool forgive_schedule_a_line_10_checkbox { get; set; }

        public string forgive_schedule_a_line_11 { get; set; }

        public string forgive_schedule_a_line_12 { get; set; }

        public string forgive_schedule_a_line_13 { get; set; }

        public string forgive_covered_period_from { get; set; }

        public string forgive_covered_period_to { get; set; }

        public string forgive_alternate_covered_period_from { get; set; }

        public string forgive_alternate_covered_period_to { get; set; }

        public bool forgive_2_million { get; set; }

        public string forgive_payroll_schedule { get; set; }
        public int forgive_lender_decision { get; internal set; }


        public string primary_email { get; internal set; }
        public string primary_name { get; internal set; }

        internal bool ez_form;
        internal bool no_reduction_in_employees;
        internal bool no_reduction_in_employees_and_covid_impact;
        public bool forgive_lender_confirmation;


    }


    public class ForgivenessDocumentsEZ
    {

        public string PayrollAName;
        public byte[] PayrollAFile;
        public string PayrollBName;
        public byte[] PayrollBFile;
        public string PayrollCName;
        public byte[] PayrollCFile;
        public string PayrollDName;
        public byte[] PayrollDFile;
        public string NonPayrollAName;
        public byte[] NonPayrollAFile;
        public string NonPayrollBName;
        public byte[] NonPayrollBFile;
        public string NonPayrollCName;
        public byte[] NonPayrollCFile;
        public string CertifySalaryName;
        public byte[] CertifySalaryFile;
        public string EmployeeJobName;
        public byte[] EmployeeJobFile;
        public string EmployeeNosName;
        public byte[] EmployeeNosFile;
        public string CompanyOpsName;
        public byte[] CompanyOpsFile;
        public string SupportAllDocsName;
        public byte[] SupportAllDocsFile;


    }


    public class ForgivenessDocumentsFullApp
    {
        public string PayrollCompensationName;
        public byte[] PayrollCompensationFile;
        public string PayrollTaxFormName;
        public byte[] PayrollTaxFormFile;
        public string PayrollPayementsName;
        public byte[] PayrollPayementsFile;
        public string FTEDocumentationName1;
        public byte[] FTEDocumentFile1;
        public string FTEDocumentationName2;
        public byte[] FTEDocumentFile2;
        public string FTEDocumentationName3;
        public byte[] FTEDocumentFile3;
        public string NonpayrollName1;
        public byte[] NonpayrollFile1;
        public string NonpayrollName2;
        public byte[] NonpayrollFile2;
        public string NonpayrollName3;
        public byte[] NonpayrollFile3;
        public string AdditionalDocumentName1;
        public byte[] AdditionalDocumentFile1;
        public string AdditionalDocumentName2;
        public byte[] AdditionalDocumentFile2;
        public string AdditionalDocumentName3;
        public byte[] AdditionalDocumentFile3;
        public string AdditionalDocumentName4;
        public byte[] AdditionalDocumentFile4;
        public string CustomerSafteyFileName;
        public byte[] CustomerSafteyFile;
    }


    public class ForgiveAdditionalDocuments
    {
        public string fileName;

        public byte[] fileContent;

        public string DocumentType;
    }
}
