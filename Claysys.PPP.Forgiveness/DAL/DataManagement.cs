using Claysys.PPP.Forgiveness.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Claysys.PPP.Forgiveness.Model;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;

namespace Claysys.PPP.Forgiveness.DAL
{
    public class DataManagement
    {
        string _selectProcName = ConfigurationManager.AppSettings["TestDataSP"];
        string _StatusProcName = ConfigurationManager.AppSettings["ApplicationStatus"];
        string _UpdateProcName = ConfigurationManager.AppSettings["TestDataSPStatus"];
        string connectionString = ConfigurationManager.ConnectionStrings["condata"].ConnectionString;

        string _selectProcNameMDC = ConfigurationManager.AppSettings["TestDataSPMDC"];
        string connectionStringMDC = ConfigurationManager.ConnectionStrings["condataMDC"].ConnectionString;

        public List<SbaForgiveness> GetForgivenessDetails()
        {

            List<SbaForgiveness> SbaForgivenessList = new List<SbaForgiveness>();
            using (SqlConnection _sqlCon = new SqlConnection(connectionStringMDC))
            {
                if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Calling " + _selectProcNameMDC + "In data management class");
                try
                {
                    _sqlCon.Open();

                    SqlCommand sql_cmnd = new SqlCommand(_selectProcNameMDC, _sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = sql_cmnd.ExecuteReader();

                    while (reader.Read())
                    {

                        SbaForgivenessList.Add(fillData(reader));

                    }
                    if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Procedure executed successfully");
                }
                catch (Exception ex)
                {
                    Utility.Utility.LogAction("Fetching doc ids failed with error " + ex.Message);

                }

                finally
                {
                    _sqlCon.Close();
                }

            }
            return SbaForgivenessList;
        }
        public SbaForgiveness fillData(SqlDataReader reader)
        {
            SbaForgiveness sbaForgivenessObj = new SbaForgiveness();
            sbaForgivenessObj.sbaLoanNumber = Convert.ToDouble(reader["SBA_Loan_Number"].ToString());
            sbaForgivenessObj.flLoanNumber = Convert.ToDouble(reader["FI_Loan_Number"].ToString());
            sbaForgivenessObj.fundingDate = Convert.ToDateTime(reader["Funding_Date"]).Year + "-" + Convert.ToDateTime(reader["Funding_Date"]).Month + "-" + Convert.ToDateTime(reader["Funding_Date"]).Day;
            //Convert.ToString(DateTime.ParseExact(reader["Funding_Date"].ToString(), "YYYY-MM-DD", CultureInfo.InvariantCulture));//reader["Funding_Date"].ToString();//Convert.ToDateTime(reader["Funding_Date"].ToString());
            sbaForgivenessObj.Entity_Name = Convert.ToString(reader["Entity_Name"]);
            sbaForgivenessObj.pppLoanAmount = Convert.ToInt32(reader["PPP_Loan_Amount"].ToString());
            sbaForgivenessObj.einSsn = Convert.ToInt32(reader["EIN_SSN"].ToString());
            sbaForgivenessObj.applicationStatus = reader["Application Status"].ToString();
            return sbaForgivenessObj;
        }

        public List<SbaForgiveness> SelectApplicationStatus()
        {
            List<SbaForgiveness> SbaForgivenessList = new List<SbaForgiveness>();
            using (SqlConnection _sqlCon = new SqlConnection(connectionStringMDC))
            {
                if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Calling " + _StatusProcName + "In data management class");
                try
                {
                    _sqlCon.Open();

                    SqlCommand sql_cmnd = new SqlCommand(_StatusProcName, _sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = sql_cmnd.ExecuteReader();

                    while (reader.Read())
                    {

                        SbaForgivenessList.Add(fillData(reader));

                    }
                    if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Procedure executed successfully");
                }
                catch (Exception ex)
                {
                    Utility.Utility.LogAction("Fetching doc ids failed with error " + ex.Message);

                }

                finally
                {
                    _sqlCon.Close();
                }

            }
            return SbaForgivenessList;
        }
        public void UpdateForgivenessDb(string sbaNumber, string status, string error, string slug)
        {
             using (SqlConnection _sqlCon = new SqlConnection(connectionStringMDC))
             {
                if (Utility.Utility.IsEventLogged) Utility.Utility.LogAction("Calling " + _UpdateProcName + "In data management class");
                try
                {
                    _sqlCon.Open();
                    SqlCommand sql_cmnd = new SqlCommand(_UpdateProcName, _sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    sql_cmnd.Parameters.AddWithValue("@status", status);
                    sql_cmnd.Parameters.AddWithValue("@error", error);
                    sql_cmnd.Parameters.AddWithValue("@sbaLoanNumber", sbaNumber);
                    sql_cmnd.Parameters.AddWithValue("@slug", slug);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    int r = sql_cmnd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {

                    Utility.Utility.LogAction("Failed updateForgivenessDb with error " + ex.Message);

                }
                finally
                {
                    _sqlCon.Close();
                }

            }  
        }

    }

}
