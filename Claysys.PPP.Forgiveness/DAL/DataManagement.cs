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
        string connectionString = ConfigurationManager.ConnectionStrings["condata"].ConnectionString;
        public List<SbaForgiveness> GetForgivenessDetails()
        {


            List<SbaForgiveness> SbaForgivenessList = new List<SbaForgiveness>();
            using (SqlConnection _sqlCon = new SqlConnection(connectionString))
            {
                //if (Utility.IsEventLogged) Utility.LogAction("Calling " + _selectTemplateProcName + " for fetching doc ids");
                try
                {
                    _sqlCon.Open();

                    SqlCommand sql_cmnd = new SqlCommand(_selectProcName, _sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;

                    //sql_cmnd.Parameters.AddWithValue("@Status", SqlDbType.VarChar).Value = selectStatus;

                    SqlDataReader reader = sql_cmnd.ExecuteReader();

                    while (reader.Read())
                    {

                        SbaForgivenessList.Add(fillData(reader));

                    }

                }
                //if (Utility.IsEventLogged) Utility.LogAction("Procedure executed successfully");


                catch (Exception ex)
                {
                    Utility.Utility.LogAction("Fetching doc ids failed with error " + ex.Message);

                }

                finally
                {
                    _sqlCon.Close();
                }
                //string jsonResult = JsonConvert.SerializeObject(SbaForgivenessList);
                //string path = @"C:\pppLoan.json";
                //using (var result = new StreamWriter(path, true))
                //{
                //    result.WriteLine(jsonResult.ToString());
                //    result.Close();

                //}


            }
            return SbaForgivenessList;
        }
        public  SbaForgiveness fillData(SqlDataReader reader)
        {
            SbaForgiveness sbaForgivenessObj = new SbaForgiveness();
            sbaForgivenessObj.sbaLoanNumber = Convert.ToDouble(reader["SBA_Loan_Number"].ToString());
            sbaForgivenessObj.flLoanNumber = Convert.ToDouble(reader["FI_Loan_Number"].ToString());
            sbaForgivenessObj.fundingDate = Convert.ToDateTime(reader["Funding_Date"]).Year + "-" + Convert.ToDateTime(reader["Funding_Date"]).Month + "-" + Convert.ToDateTime(reader["Funding_Date"]).Day;
                //Convert.ToString(DateTime.ParseExact(reader["Funding_Date"].ToString(), "YYYY-MM-DD", CultureInfo.InvariantCulture));//reader["Funding_Date"].ToString();//Convert.ToDateTime(reader["Funding_Date"].ToString());
            sbaForgivenessObj.Entity_Name = Convert.ToString(reader["Entity_Name"]);
            sbaForgivenessObj.pppLoanAmount = Convert.ToInt32(reader["PPP_Loan_Amount"].ToString());
            sbaForgivenessObj.einSsn = Convert.ToInt32(reader["EIN_SSN"].ToString());
            return sbaForgivenessObj;
        }
    }

}
