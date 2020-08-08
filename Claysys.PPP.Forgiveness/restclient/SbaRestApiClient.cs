using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Claysys.PPP.Forgiveness.Utility;
using Newtonsoft.Json;
using RestSharp;
using Claysys.PPP.Forgiveness.controller;
using Claysys.PPP.Forgiveness.domain;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Claysys.PPP.Forgiveness.restclient
{
    public class SbaRestApiClient
    {
        private readonly string baseUri;

        private readonly string apiToken;

        private readonly string vendorKey;

        private const string VENDOR_KEY = "Vendor-Key";

        public SbaRestApiClient(string BaseUri, string ApiToken, string VendorKey)
        {
            baseUri = BaseUri;
            apiToken = ApiToken;
            vendorKey = VendorKey;
        }

        public async Task<SbaPPPLoanForgiveness> invokeSbaLoanForgiveness(SbaPPPLoanForgiveness request, string loanForgivenessUrl)

        {
            string _updateProcName = ConfigurationManager.AppSettings["TestDataSPStatus"];
            string connectionString = ConfigurationManager.ConnectionStrings["condata"].ConnectionString;
            try
            {

                var serialized = JsonConvert.SerializeObject(request, new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });

                RestClient restClient = new RestClient($"{baseUri}/{loanForgivenessUrl}/");
                restClient.Timeout = -1;
                RestRequest restRequest = new RestRequest(Method.POST);
                restRequest.AddHeader("Authorization", apiToken);
                restRequest.AddHeader(VENDOR_KEY, vendorKey);
                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddParameter("application/json", serialized, ParameterType.RequestBody);
                IRestResponse response = await restClient.ExecuteAsync(restRequest);
                double sbaNumber = Convert.ToDouble(request.etran_loan.sba_number);
                using (SqlConnection _sqlCon = new SqlConnection(connectionString))
                {
                    SbaPPPLoanForgiveness sbaPPPLoanForgiveness = null;
                    try
                    {
                        _sqlCon.Open();
                        SqlCommand sql_cmnd = new SqlCommand(_updateProcName, _sqlCon);

                        if (response.IsSuccessful)
                        {
                            var jObject = JObject.Parse(response.Content);
                            string status = jObject["etran_loan"]["status"].ToString();
                            string slug = jObject["slug"].ToString();
                            string error = " ";
                            updateForgivenessDb(sql_cmnd, sbaNumber, status, error, slug);
                            Utility.Utility.LogAction("Invoke SBA Loan Requert Procedure executed successfully");
                            sbaPPPLoanForgiveness = JsonConvert.DeserializeObject<SbaPPPLoanForgiveness>(response.Content);
                        }
                        else
                        {
                            string status = "Failed";
                            string slug = " ";
                            updateForgivenessDb(sql_cmnd, sbaNumber, status, response.Content, slug);
                            Utility.Utility.LogAction($"Did not receive success code for SBA Number:{sbaNumber}. please investigate. \nresponse code: {response.StatusCode}.\n response:{response.Content}");
                            return new SbaPPPLoanForgiveness();
                        }

                    }

                    catch (Exception ex)
                    {
                        Utility.Utility.LogAction("invokeSbaLoanForgiveness() failed with error " + ex.Message);
                    }

                    finally
                    {
                        _sqlCon.Close();
                    }

                    return sbaPPPLoanForgiveness;
                }

            }
            catch (Exception exception)
            {
                Utility.Utility.LogAction($"invokeSbaLoanForgiveness() failed with error : {exception.Message}");
                return null;
            }
        }

        public static void updateForgivenessDb(SqlCommand sql_cmnd, double sbaNumber, string status, string error, string slug)
        {
            sql_cmnd.Parameters.AddWithValue("@status", status);
            sql_cmnd.Parameters.AddWithValue("@error", error);
            sql_cmnd.Parameters.AddWithValue("@sbaLoanNumber", sbaNumber);
            sql_cmnd.Parameters.AddWithValue("@slug", slug);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            Utility.Utility.LogAction($"Update Stored Procedure Successfully Worked for Sba Number : {sbaNumber} ");
            int r = sql_cmnd.ExecuteNonQuery();
        }

        public async Task<LoanDocumentResponse> uploadForgivenessDocument(string requestName, string requestDocument_type, string etran_loan, string document, string loanDocumentsUrl, string sbaNumber)
        {
            string _updateProcName = ConfigurationManager.AppSettings["TestDataSPStatus"];
            string connectionString = ConfigurationManager.ConnectionStrings["condata"].ConnectionString;
            double sbanumber = Convert.ToDouble(sbaNumber);
            try
            {
                RestClient restClient = new RestClient($"{baseUri}/{loanDocumentsUrl}/");
                restClient.Timeout = -1;
                RestRequest restRequest = new RestRequest(Method.POST);
                restRequest.AddHeader("Authorization", apiToken);
                restRequest.AddHeader(VENDOR_KEY, vendorKey);
                restRequest.AddParameter("name", requestName);
                restRequest.AddParameter("document_type", requestDocument_type);
                restRequest.AddParameter("etran_loan", etran_loan);
                restRequest.AddFile("document", document);

                IRestResponse response = await restClient.ExecuteAsync(restRequest);
                using (SqlConnection _sqlCon = new SqlConnection(connectionString))
                {
                    LoanDocumentResponse loanDocument = null;
                    try
                    {
                        _sqlCon.Open();
                        SqlCommand sql_cmnd = new SqlCommand(_updateProcName, _sqlCon);

                        if (response.IsSuccessful)
                        {
                            //var jObject = JObject.Parse(response.Content);
                            //string status = jObject["etran_loan"]["status"].ToString();
                            //string slug = etran_loan;//jObject["slug"].ToString();
                            //string error = " ";
                            //updateForgivenessDb(sql_cmnd, sbanumber, status, error, slug); need to do
                            Utility.Utility.LogAction("Upload loan document Procedure executed successfully");
                            loanDocument = JsonConvert.DeserializeObject<LoanDocumentResponse>(response.Content);

                        }
                        else
                        {
                            //string status = "Upload Loan Document Failed";
                            //string slug = " ";
                            //updateForgivenessDb(sql_cmnd, sbanumber, status, response.Content, slug);
                            Utility.Utility.LogAction($"Did not receive success code uploadForgivenessDocument() {loanDocumentsUrl}. please investigate. \nresponse code: {response.StatusCode}.\n response:{response.Content}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Utility.Utility.LogAction(" uploadForgivenessDocument sql connection failed with error " + ex.Message);
                    }
                    finally
                    {
                        _sqlCon.Close();
                    }
                    return loanDocument;
                }
            }
            catch (Exception exception)
            {
                Utility.Utility.LogAction($"{Environment.NewLine}{exception.Message}{Environment.NewLine}");
                return null;
            }
        }

        public async Task<LoanDocument> invokeSbaLoanDocument(LoanDocument request, string loanDocumentsUrl)
        {
            try
            {
                var serialized = JsonConvert.SerializeObject(request, new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });

                RestClient restClient = new RestClient($"{baseUri}/{loanDocumentsUrl}/");
                restClient.Timeout = -1;
                RestRequest restRequest = new RestRequest(Method.POST);
                restRequest.AddHeader("Authorization", apiToken);
                restRequest.AddHeader(VENDOR_KEY, vendorKey);
                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddParameter("application/json", serialized, ParameterType.RequestBody);
                IRestResponse response = await restClient.ExecuteAsync(restRequest);


                if (response.IsSuccessful)
                {
                    LoanDocument loanDocument = JsonConvert.DeserializeObject<LoanDocument>(response.Content);
                    return loanDocument;
                }
                else
                {
                    Utility.Utility.LogAction($"Did not receive success code. please investigate. \nresponse code: {response.StatusCode}.\n response:{response.Content}");
                    return new LoanDocument();
                }

            }
            catch (Exception exception)
            {
                Utility.Utility.LogAction($"{Environment.NewLine}{exception.Message}{Environment.NewLine}");
                return null;
            }
        }

        public async Task<SbaPPPLoanDocumentTypeResponse> getSbaLoanForgiveness(int pageNumber, string sbaNumber, string loanForgivenessUrl)
        {
            try
            {
                if (pageNumber <= 0)
                {
                    throw new Exception("Incorrect input data. please investigate");
                }

                string baseUrl = !string.IsNullOrEmpty(sbaNumber)
                    ? $"{baseUri}/{loanForgivenessUrl}?pageNumber={pageNumber}&sbaNumber={sbaNumber}"
                    : $"{baseUri}/{loanForgivenessUrl}?pageNumber={pageNumber}";

                RestClient restClient = new RestClient(baseUrl);
                restClient.Timeout = -1;
                RestRequest restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("Authorization", apiToken);
                restRequest.AddHeader(VENDOR_KEY, vendorKey);
                restRequest.AddHeader("Content-Type", "application/json");
                IRestResponse restResponse = await restClient.ExecuteAsync(restRequest);

                if (restResponse.IsSuccessful)
                {
                    SbaPPPLoanDocumentTypeResponse loanDocumentTypeResponse = JsonConvert.DeserializeObject<SbaPPPLoanDocumentTypeResponse>(restResponse.Content);
                    return loanDocumentTypeResponse;
                }
                else
                {
                    Utility.Utility.LogAction($"Did not receive success code for Sba Number : {sbaNumber}. please investigate. received response code: {restResponse.StatusCode}");
                    return new SbaPPPLoanDocumentTypeResponse();
                }
            }
            catch (Exception exception)
            {
                Utility.Utility.LogAction($"{Environment.NewLine}{exception.Message}{Environment.NewLine}");
                return null;
            }
        }

        public async Task<SbaPPPLoanForgivenessStatusResponse> getAllForgivenessRequests(string ppp_loan_forgiveness_requests)
        {
            try
            {
                RestClient restClient = new RestClient($"{baseUri}/{ppp_loan_forgiveness_requests}/");
                restClient.Timeout = -1;
                RestRequest restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("Authorization", apiToken);
                restRequest.AddHeader(VENDOR_KEY, vendorKey);
                restRequest.AddHeader("Content-Type", "application/json");
                IRestResponse restResponse = await restClient.ExecuteAsync(restRequest);

                if (restResponse.IsSuccessful)
                {
                    SbaPPPLoanForgivenessStatusResponse sbaLoanForgiveness = JsonConvert.DeserializeObject<SbaPPPLoanForgivenessStatusResponse>(restResponse.Content);
                    return sbaLoanForgiveness;
                }
                else
                {
                    Utility.Utility.LogAction($"Did not receive success code. please investigate. received response: {Environment.NewLine}StatusCode - {restResponse.StatusCode}{Environment.NewLine}Response - {restResponse.Content}");
                    return new SbaPPPLoanForgivenessStatusResponse();
                }

            }
            catch (Exception exception)
            {
                Utility.Utility.LogAction($"{Environment.NewLine}{exception.Message}{Environment.NewLine}");

                return null;
            }
        }


        public async Task<SbaPPPLoanForgiveness> getSbaLoanForgivenessBySlug(string slug, string loanForgivenessUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(slug))
                {
                    throw new Exception("Incorrect input data. please investigate");
                }

                RestClient restClient = new RestClient($"{baseUri}/{loanForgivenessUrl}/{slug}/");
                restClient.Timeout = -1;
                RestRequest restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("Authorization", apiToken);
                restRequest.AddHeader(VENDOR_KEY, vendorKey);
                restRequest.AddHeader("Content-Type", "application/json");
                IRestResponse restResponse = await restClient.ExecuteAsync(restRequest);

                if (restResponse.IsSuccessful)
                {
                    Console.WriteLine($"{Environment.NewLine}{restResponse.Content}{Environment.NewLine}");
                    Console.WriteLine("------------------------------------------------------------------------");

                    SbaPPPLoanForgiveness sbaLoanForgiveness = JsonConvert.DeserializeObject<SbaPPPLoanForgiveness>(restResponse.Content);
                    return sbaLoanForgiveness;
                }
                throw new Exception($"Did not receive success code. please investigate. received response: {Environment.NewLine}StatusCode - {restResponse.StatusCode}{Environment.NewLine}Response - {restResponse.Content}");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{Environment.NewLine}{exception.Message}{Environment.NewLine}");
                Console.WriteLine("------------------------------------------------------------------------");
                return null;
            }
        }

        public async Task<SbaPPPLoanForgivenessStatusResponse> getForgivenessRequestBysbaNumber(string sbaNumber, string ppp_loan_forgiveness_requests)
        {
            try
            {
                RestClient restClient = new RestClient($"{baseUri}/{ppp_loan_forgiveness_requests}/");
                restClient.Timeout = -1;
                RestRequest restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("Authorization", apiToken);
                restRequest.AddHeader(VENDOR_KEY, vendorKey);
                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddParameter("sba_number", sbaNumber);
                IRestResponse restResponse = await restClient.ExecuteAsync(restRequest);

                if (restResponse.IsSuccessful)
                {
                    var jObject = JObject.Parse(restResponse.Content);
                    string status = Convert.ToInt32(((Newtonsoft.Json.Linq.JValue)jObject["count"]).Value) <= 0 ? "Removed" : jObject["results"][0]["etran_loan"]["status"].ToString();

                    SbaPPPLoanForgivenessStatusResponse sbaLoanForgiveness = JsonConvert.DeserializeObject<SbaPPPLoanForgivenessStatusResponse>(restResponse.Content);
                    sbaLoanForgiveness.Status = status;
                    return sbaLoanForgiveness;

                }
                else
                {
                    Utility.Utility.LogAction($"Did not receive success code for the Sba Number: {sbaNumber} in getForgivenessRequestBysbaNumber(). please investigate. received response: {Environment.NewLine}StatusCode - {restResponse.StatusCode}{Environment.NewLine}Response - {restResponse.Content}");
                    return new SbaPPPLoanForgivenessStatusResponse();
                }

            }
            catch (Exception exception)
            {
                Utility.Utility.LogAction($"{Environment.NewLine}{exception.Message}{Environment.NewLine}");
                throw exception;
            }
        }

        public async Task<SbaPPPLoanDocumentTypeResponse> getSbaLoanForgiveness(string loanForgivenessUrl)
        {
            try
            {
                string baseUrl = $"{baseUri}/{loanForgivenessUrl}/";

                RestClient restClient = new RestClient(baseUrl);
                restClient.Timeout = -1;
                RestRequest restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("Authorization", apiToken);
                restRequest.AddHeader(VENDOR_KEY, vendorKey);
                restRequest.AddHeader("Content-Type", "application/json");
                IRestResponse restResponse = await restClient.ExecuteAsync(restRequest);

                if (restResponse.IsSuccessful)
                {
                    SbaPPPLoanDocumentTypeResponse loanDocumentTypeResponse = JsonConvert.DeserializeObject<SbaPPPLoanDocumentTypeResponse>(restResponse.Content);
                    return loanDocumentTypeResponse;
                }
                else
                {
                    Utility.Utility.LogAction($"Did not receive success code for getSbaLoanForgiveness(). please investigate. received response: {Environment.NewLine}StatusCode - {restResponse.StatusCode}{Environment.NewLine}Response - {restResponse.Content}");
                    return new SbaPPPLoanDocumentTypeResponse();
                }

            }
            catch (Exception exception)
            {
                Utility.Utility.LogAction($"{Environment.NewLine}{exception.Message}{Environment.NewLine}");
                return null;
            }
        }

        public async Task<SbaPPPLoanMessagesResponse> getForgivenessMessagesBySbaNumber(int page, string sbaNumber, bool isComplete, string loanForgivenessMessageUrl)
        {
            try
            {
                if (page <= 0)
                {
                    throw new Exception("Incorrect input data. please investigate");
                }

                string baseUrl = $"{baseUri}/{loanForgivenessMessageUrl}/";

                RestClient restClient = new RestClient(baseUrl);
                restClient.Timeout = -1;
                RestRequest restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("Authorization", apiToken);
                restRequest.AddHeader(VENDOR_KEY, vendorKey);
                restRequest.AddParameter("sba_number", sbaNumber);
                restRequest.AddParameter("page", page);
                restRequest.AddParameter("isComplete", isComplete);
                restRequest.AddHeader("Content-Type", "application/json");
                IRestResponse restResponse = await restClient.ExecuteAsync(restRequest);

                if (restResponse.IsSuccessful)
                {

                    SbaPPPLoanMessagesResponse loanMessagesResponse = JsonConvert.DeserializeObject<SbaPPPLoanMessagesResponse>(restResponse.Content);
                    return loanMessagesResponse;
                }
                throw new Exception($"Did not receive success code. please investigate. received response: {Environment.NewLine}StatusCode - {restResponse.StatusCode}{Environment.NewLine}Response - {restResponse.Content}");
            }
            catch (Exception exception)
            {
                Utility.Utility.LogAction($"{Environment.NewLine}{exception.Message}{Environment.NewLine}");

                return null;
            }
        }

        public async Task<bool> deleteSbaLoanForgiveness(string slug, string sbaNumber, string loanForgivenessUrl)
        {
            string _deleteProcName = ConfigurationManager.AppSettings["TestDataSPStatus"];
            string connectionString = ConfigurationManager.ConnectionStrings["condata"].ConnectionString;
            double sbanumber = Convert.ToDouble(sbaNumber);
            try
            {
                if (string.IsNullOrEmpty(slug))
                {
                    Utility.Utility.LogAction("Incorrect input data. please investigate");
                }

                RestClient restClient = new RestClient($"{baseUri}/{loanForgivenessUrl}/{slug}/");
                restClient.Timeout = -1;
                RestRequest restRequest = new RestRequest(Method.DELETE);
                restRequest.AddHeader("Authorization", apiToken);
                restRequest.AddHeader(VENDOR_KEY, vendorKey);
                restRequest.AddHeader("Content-Type", "application/json");
                IRestResponse restResponse = await restClient.ExecuteAsync(restRequest);

                using (SqlConnection _sqlCon = new SqlConnection(connectionString))
                {
                    SbaPPPLoanForgiveness sbaPPPLoanForgiveness = null;
                    try
                    {
                        _sqlCon.Open();
                        SqlCommand sql_cmnd = new SqlCommand(_deleteProcName, _sqlCon);

                        if (restResponse.IsSuccessful)
                        {
                            var jObject = JObject.Parse(restResponse.Content);
                            string status = jObject["etran_loan"]["status"].ToString();
                            string error = restResponse.Content;
                            updateForgivenessDb(sql_cmnd, sbanumber, status, error, slug);

                            sbaPPPLoanForgiveness = JsonConvert.DeserializeObject<SbaPPPLoanForgiveness>(restResponse.Content);
                            Utility.Utility.LogAction($"{Environment.NewLine}Delete was successful{Environment.NewLine}");

                        }
                        else
                        {
                            var jObject = JObject.Parse(restResponse.Content);
                            string status = "Deletion Failed";
                            string error = restResponse.Content;
                            updateForgivenessDb(sql_cmnd, sbanumber, status, error, slug);
                            Utility.Utility.LogAction($"Did not receive success code for SBA Number: {sbaNumber} Deletion. please investigate. received response: {Environment.NewLine}StatusCode - {restResponse.StatusCode}{Environment.NewLine}Response - {restResponse.Content}");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Utility.Utility.LogAction("Database connectivity in Delete SBA Loan Request failed with error " + ex.Message);
                    }

                    finally
                    {
                        _sqlCon.Close();

                    }
                    return true;
                }
            }

            catch (Exception exception)
            {
                Utility.Utility.LogAction($"{Environment.NewLine}{exception.Message}{Environment.NewLine}");
                return false;
            }
        }


        public async Task<SbaPPPLoanDocumentTypeResponse> getSbaLoanDocumentTypes(Dictionary<string, string> reqParams, string loanDocumentTypesUrl)
        {
            try
            {
                if (reqParams.Count <= 0)
                {
                    throw new Exception("Incorrect input data. please investigate");
                }
                var serialized = JsonConvert.SerializeObject(reqParams, new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });

                RestClient restClient = new RestClient($"{baseUri}/{loanDocumentTypesUrl}/");
                restClient.Timeout = -1;
                RestRequest restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("Authorization", apiToken);
                restRequest.AddHeader(VENDOR_KEY, vendorKey);
                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddParameter("application/json", serialized, ParameterType.RequestBody);
                IRestResponse restResponse = await restClient.ExecuteAsync(restRequest);

                if (restResponse.IsSuccessful)
                {
                    SbaPPPLoanDocumentTypeResponse documentTypeResponse = JsonConvert.DeserializeObject<SbaPPPLoanDocumentTypeResponse>(restResponse.Content);
                    return documentTypeResponse;
                }
                else
                {
                    Utility.Utility.LogAction($"Did not receive success code. please investigate. received response code: {restResponse.StatusCode}");
                    return new SbaPPPLoanDocumentTypeResponse();
                }

            }
            catch (Exception exception)
            {
                Utility.Utility.LogAction($"{Environment.NewLine}{exception.Message}{Environment.NewLine}");
                return null;
            }
        }

        public async Task<LoanDocumentType> getSbaLoanDocumentTypeById(int id, string loanForgivenessUrl)
        {
            try
            {
                if (id <= 0)
                {
                    Utility.Utility.LogAction("Incorrect input data. please investigate getSbaLoanDocumentTypeById in SbaRestApiClient");
                    throw new Exception("Incorrect input data. please investigate");
                }

                RestClient restClient = new RestClient($"{baseUri}/{loanForgivenessUrl}?id={id}");
                restClient.Timeout = -1;
                RestRequest restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("Authorization", apiToken);
                restRequest.AddHeader(VENDOR_KEY, vendorKey);
                restRequest.AddHeader("Content-Type", "application/json");
                IRestResponse restResponse = await restClient.ExecuteAsync(restRequest);

                if (restResponse.IsSuccessful)
                {
                    LoanDocumentType loanDocumentType = JsonConvert.DeserializeObject<LoanDocumentType>(restResponse.Content);
                    return loanDocumentType;
                }
                throw new Exception($"Did not receive success code. please investigate. received response code: {restResponse.StatusCode}");
            }
            catch (Exception exception)
            {
                Utility.Utility.LogAction($"{Environment.NewLine}{exception.Message}{Environment.NewLine}");
                return null;
            }
        }

        public async Task<MessageReply> updateSbaLoanForgivenessMessageReply(MessageReply request, string loanForgivenessMessageUrl)
        {
            try
            {
                var serialized = JsonConvert.SerializeObject(request, new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat });

                RestClient restClient = new RestClient($"{baseUri}/{loanForgivenessMessageUrl}/");
                restClient.Timeout = -1;
                RestRequest restRequest = new RestRequest(Method.POST);
                restRequest.AddHeader("Authorization", apiToken);
                restRequest.AddHeader(VENDOR_KEY, vendorKey);
                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddParameter("application/json", serialized, ParameterType.RequestBody);
                IRestResponse response = await restClient.ExecuteAsync(restRequest);

                if (response.IsSuccessful)
                {
                    MessageReply sbaLoanForgivenessMessageReply = JsonConvert.DeserializeObject<MessageReply>(response.Content);
                    return sbaLoanForgivenessMessageReply;
                }
                else
                {
                    Utility.Utility.LogAction($"Did not receive success code updateSbaLoanForgivenessMessageReply(). please investigate. received response code: {response.StatusCode}");
                    return new MessageReply();
                }

            }
            catch (Exception exception)
            {
                Utility.Utility.LogAction($"{Environment.NewLine}{exception.Message}{Environment.NewLine}");
                return null;
            }
        }

        public async Task<SbaPPPLoanMessagesResponse> getSbaLoanMessagesBySbaNumber(int page, string sbaNumber, bool isComplete, string loanForgivenessMessageUrl)
        {
            try
            {
                if (page <= 0)
                {
                    throw new Exception("Incorrect input data. please investigate");
                }

                string baseUrl = $"{baseUri}/{loanForgivenessMessageUrl}?page={page}&sbaNumber={sbaNumber}&isComplete={isComplete}";

                RestClient restClient = new RestClient(baseUrl);
                restClient.Timeout = -1;
                RestRequest restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("Authorization", apiToken);
                restRequest.AddHeader(VENDOR_KEY, vendorKey);
                restRequest.AddHeader("Content-Type", "application/json");
                IRestResponse restResponse = await restClient.ExecuteAsync(restRequest);

                if (restResponse.IsSuccessful)
                {
                    SbaPPPLoanMessagesResponse loanMessagesResponse = JsonConvert.DeserializeObject<SbaPPPLoanMessagesResponse>(restResponse.Content);
                    return loanMessagesResponse;
                }
                throw new Exception($"Did not receive success code. please investigate. received response code: {restResponse.StatusCode}");
            }
            catch (Exception exception)
            {
                Utility.Utility.LogAction($"{Environment.NewLine}{exception.Message}{Environment.NewLine}");
                return null;
            }
        }

        public async Task<SbaPPPLoanForgivenessMessage> getSbaLoanForgivenessMessagesBySlug(string slug, string loanForgivenessMessageUrl)
        {
            try
            {
                string baseUrl = $"{baseUri}/{loanForgivenessMessageUrl}?slug={slug}";

                RestClient restClient = new RestClient(baseUrl);
                restClient.Timeout = -1;
                RestRequest restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("Authorization", apiToken);
                restRequest.AddHeader(VENDOR_KEY, vendorKey);
                restRequest.AddHeader("Content-Type", "application/json");
                IRestResponse restResponse = await restClient.ExecuteAsync(restRequest);

                if (restResponse.IsSuccessful)
                {
                    SbaPPPLoanForgivenessMessage loanForgivenessMessage = JsonConvert.DeserializeObject<SbaPPPLoanForgivenessMessage>(restResponse.Content);
                    return loanForgivenessMessage;
                }
                else
                {
                    Utility.Utility.LogAction($"Did not receive success code. please investigate. received response code: {restResponse.StatusCode}");
                    return new SbaPPPLoanForgivenessMessage();
                }

            }
            catch (Exception exception)
            {
                Utility.Utility.LogAction($"{Environment.NewLine}{exception.Message}{Environment.NewLine}");
                return null;
            }
        }
    }
}
