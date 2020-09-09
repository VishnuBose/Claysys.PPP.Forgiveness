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
using Claysys.PPP.Forgiveness.DAL;

namespace Claysys.PPP.Forgiveness.restclient
{
    public class SbaRestApiClient
    {
        private readonly string baseUri;

        private readonly string apiToken;

        private readonly string vendorKey;

        private const string VENDOR_KEY = "Vendor-Key";

        string forgivenessUpdation = ConfigurationManager.AppSettings["ForgivenessApplicationUpdation"];

        string _messageProcName = ConfigurationManager.AppSettings["ForgivenessMessage"];

        string documentProcName = ConfigurationManager.AppSettings["ForgivenessDocument"];

        public SbaRestApiClient(string BaseUri, string ApiToken, string VendorKey)
        {
            baseUri = BaseUri;
            apiToken = ApiToken;
            vendorKey = VendorKey;
        }

        public async Task<SbaPPPLoanForgiveness> InvokeSbaLoanForgiveness(SbaPPPLoanForgiveness request, string loanForgivenessUrl)

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
            using (SqlConnection _sqlCon = new SqlConnection(Claysys.PPP.Forgiveness.PPPForgiveness.dataManagementObj.connectionString))
            {
                SbaPPPLoanForgiveness sbaPPPLoanForgiveness = null;
                _sqlCon.Open();
                SqlCommand sql_cmnd = new SqlCommand(forgivenessUpdation, _sqlCon);

                if (response.IsSuccessful)
                {
                    var jObject = JObject.Parse(response.Content);
                    string status = jObject["etran_loan"]["status"].ToString();
                    string slug = jObject["slug"].ToString();
                    string error = " ";
                    Claysys.PPP.Forgiveness.PPPForgiveness.dataManagementObj.UpdateForgivenessDb(sql_cmnd, sbaNumber, status, error, slug);
                    Utility.Utility.LogAction($"{sbaNumber}: Invoke SBA Loan Requert Procedure executed successfully");
                    sbaPPPLoanForgiveness = JsonConvert.DeserializeObject<SbaPPPLoanForgiveness>(response.Content);
                }
                else
                {
                    string status = "Failed";
                    string slug = " ";
                    Claysys.PPP.Forgiveness.PPPForgiveness.dataManagementObj.UpdateForgivenessDb(sql_cmnd, sbaNumber, status, Convert.ToString(response.Content), slug);
                    Utility.Utility.LogAction($"{sbaNumber}: Did not receive success code for SBA Number. please investigate. \nresponse code: {response.StatusCode}.\n response:{response.Content}");
                    return new SbaPPPLoanForgiveness();
                }

                _sqlCon.Close();
                return sbaPPPLoanForgiveness;
            }
        }




        public async Task<LoanDocumentResponse> UploadForgivenessDocument(string requestName, string requestDocument_type, string etran_loan, byte[] document, string loanDocumentsUrl, string sbaNumber, string slugId="")
        {

            double sbanumber = Convert.ToDouble(sbaNumber);

            RestClient restClient = new RestClient($"{baseUri}/{loanDocumentsUrl}/");
            restClient.Timeout = -1;
            RestRequest restRequest = new RestRequest(Method.POST);
            restRequest.AddHeader("Authorization", apiToken);
            restRequest.AddHeader(VENDOR_KEY, vendorKey);
            restRequest.AddParameter("name", requestName);
            restRequest.AddParameter("document_type", requestDocument_type);
            restRequest.AddParameter("etran_loan", etran_loan);
            restRequest.AddFile("document", document, requestName);

            IRestResponse response = await restClient.ExecuteAsync(restRequest);
            using (SqlConnection _sqlCon = new SqlConnection(Claysys.PPP.Forgiveness.PPPForgiveness.dataManagementObj.connectionString))
            {
                LoanDocumentResponse loanDocument = null;


                _sqlCon.Open();


                if (response.IsSuccessful)
                {
                    SqlCommand sql_cmnd = new SqlCommand(documentProcName, _sqlCon);
                    var jObject = JObject.Parse(response.Content);
                    string slug = jObject["slug"]?.ToString();
                    string name = jObject["name"]?.ToString();
                    string createdAt = jObject["created_at"]?.ToString();
                    string updatedAt = jObject["updated_at"]?.ToString();
                    string doc = jObject["document"]?.ToString();
                    int documentType = Convert.ToInt32(jObject["document_type"]?.ToString());
                    string url = jObject["url"]?.ToString();
                    string etranLoan = jObject["etran_loan"]?.ToString();

                    Claysys.PPP.Forgiveness.PPPForgiveness.dataManagementObj.ForgivenessDocument(sql_cmnd, sbaNumber, slug, name, createdAt, updatedAt, doc, documentType, url, etranLoan);
                    Utility.Utility.LogAction($"{sbaNumber} : Upload loan document Procedure executed successfully. Document : {doc} && Document Type : {documentType}");
                    loanDocument = JsonConvert.DeserializeObject<LoanDocumentResponse>(response.Content);

                }
                else
                {
                    SqlCommand sql_cmnd = new SqlCommand(forgivenessUpdation, _sqlCon);
                    string status = "Upload Loan Document Failed";
                    string slug = slugId;
                    Claysys.PPP.Forgiveness.PPPForgiveness.dataManagementObj.UpdateForgivenessDb(sql_cmnd, sbanumber, status, response.Content, slug);

                    Utility.Utility.LogAction($"{sbanumber} : Did not receive success code uploadForgivenessDocument() {loanDocumentsUrl}.Document : {document} && Document Type : {requestDocument_type} please investigate. \nresponse code: {response.StatusCode}.\n response:{response.Content}");
                }

                _sqlCon.Close();

                return loanDocument;
            }

        }

        public async Task<LoanDocument> invokeSbaLoanDocument(LoanDocument request, string loanDocumentsUrl)
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



        public async Task<SbaPPPLoanDocumentTypeResponse> getSbaLoanForgiveness(int pageNumber, string sbaNumber, string loanForgivenessUrl)
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
                Utility.Utility.LogAction($"{sbaNumber} : Did not receive success code for Sba Number. please investigate. received response code: {restResponse.StatusCode}");
                return new SbaPPPLoanDocumentTypeResponse();
            }

        }

        public async Task<SbaPPPLoanForgivenessStatusResponse> getAllForgivenessRequests(string ppp_loan_forgiveness_requests)
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


        public async Task<SbaPPPLoanForgiveness> getSbaLoanForgivenessBySlug(string slug, string loanForgivenessUrl)
        {
            if (string.IsNullOrEmpty(slug))
            {
                Utility.Utility.LogAction("Incorrect input data in getSbaLoanForgivenessBySlug. please investigate");
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
                Utility.Utility.LogAction($"{Environment.NewLine}{restResponse.Content}{Environment.NewLine}");
                SbaPPPLoanForgiveness sbaLoanForgiveness = JsonConvert.DeserializeObject<SbaPPPLoanForgiveness>(restResponse.Content);
                return sbaLoanForgiveness;
            }
            else
            {
                Utility.Utility.LogAction($"Did not receive success code. please investigate. received response: {Environment.NewLine}StatusCode - {restResponse.StatusCode}{Environment.NewLine}Response - {restResponse.Content}");
                return new SbaPPPLoanForgiveness();
            }
        }

        public async Task<SbaPPPLoanForgivenessStatusResponse> GetForgivenessRequestBysbaNumber(string sbaNumber, string ppp_loan_forgiveness_requests)
        {
            RestClient restClient = new RestClient($"{baseUri}/{ppp_loan_forgiveness_requests}/");
            restClient.Timeout = -1;
            RestRequest restRequest = new RestRequest(Method.GET);
            restRequest.AddHeader("Authorization", apiToken);
            restRequest.AddHeader(VENDOR_KEY, vendorKey);
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddParameter("sba_number", sbaNumber);
            IRestResponse restResponse = restClient.Execute(restRequest);

            if (restResponse.IsSuccessful)
            {
                var jObject = JObject.Parse(restResponse.Content);
                string status = Convert.ToInt32(((Newtonsoft.Json.Linq.JValue)jObject["count"]).Value) <= 0 ? "" : jObject["results"][0]["etran_loan"]["status"].ToString();

                SbaPPPLoanForgivenessStatusResponse sbaLoanForgiveness = JsonConvert.DeserializeObject<SbaPPPLoanForgivenessStatusResponse>(restResponse.Content);
                sbaLoanForgiveness.Status = string.IsNullOrEmpty(status) ? "Error" : status;
                return sbaLoanForgiveness;

            }
            else
            {
                Utility.Utility.LogAction($"{sbaNumber} : Did not receive success code for the Sba Number: {sbaNumber} in getForgivenessRequestBysbaNumber(). please investigate. received response: {Environment.NewLine}StatusCode - {restResponse.StatusCode}{Environment.NewLine}Response - {restResponse.Content}");
                return new SbaPPPLoanForgivenessStatusResponse();
            }

        }

        public async Task<SbaPPPLoanDocumentTypeResponse> GetSbaLoanForgiveness(string loanForgivenessUrl)
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


            if (page <= 0)
            {
                Utility.Utility.LogAction(("Incorrect input data. please investigate"));
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

            using (SqlConnection _sqlCon = new SqlConnection(Claysys.PPP.Forgiveness.PPPForgiveness.dataManagementObj.connectionString))
            {
                SbaPPPLoanMessagesResponse loanMessagesResponse = null;

                _sqlCon.Open();
              

                if (restResponse.IsSuccessful)
                {
                    SqlCommand sql_cmnd = new SqlCommand(_messageProcName, _sqlCon);
                    var jObject = JObject.Parse(restResponse.Content);
                    string subject = jObject["slug"]?.ToString();
                    string ticket = jObject["name"]?.ToString();
                    string message = jObject["created_at"]?.ToString();
                    Boolean isCompleted = Convert.ToBoolean(jObject["updated_at"]);

                    Claysys.PPP.Forgiveness.PPPForgiveness.dataManagementObj.ForgivenessMessage(sql_cmnd, sbaNumber, subject, ticket, message, isCompleted);
                    Utility.Utility.LogAction("Forgiveness Message Procedure executed successfully");

                    loanMessagesResponse = JsonConvert.DeserializeObject<SbaPPPLoanMessagesResponse>(restResponse.Content);

                }
                else
                {
                    SqlCommand sql_cmnd = new SqlCommand(forgivenessUpdation, _sqlCon);
                    string status = "Upload Loan Document Failed";
                    string slug = " ";
                    Claysys.PPP.Forgiveness.PPPForgiveness.dataManagementObj.UpdateForgivenessDb(sql_cmnd, Convert.ToDouble(sbaNumber), status, restResponse.Content, slug);

                    Utility.Utility.LogAction($"{sbaNumber}Did not receive success code for getForgivenessMessagesBySbaNumber function. please investigate. received response: {Environment.NewLine}StatusCode - {restResponse.StatusCode}{Environment.NewLine}Response - {restResponse.Content}");
                    return new SbaPPPLoanMessagesResponse();
                }

                _sqlCon.Close();
                return loanMessagesResponse;
            }

        }


        public async Task<bool> DeleteSbaLoanForgiveness(string slug, string sbaNumber, string loanForgivenessUrl)
        {
           
            double sbanumber = Convert.ToDouble(sbaNumber);

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

            using (SqlConnection _sqlCon = new SqlConnection(Claysys.PPP.Forgiveness.PPPForgiveness.dataManagementObj.connectionString))
            {
                SbaPPPLoanForgiveness sbaPPPLoanForgiveness = null;

                _sqlCon.Open();
                SqlCommand sql_cmnd = new SqlCommand(forgivenessUpdation, _sqlCon);

                if (restResponse.IsSuccessful)
                {
                    var jObject = JObject.Parse(restResponse.Content);
                    string status = jObject["etran_loan"]["status"].ToString();
                    string error = restResponse.Content;
                    Claysys.PPP.Forgiveness.PPPForgiveness.dataManagementObj.UpdateForgivenessDb(sql_cmnd, sbanumber, status, error, slug);

                    sbaPPPLoanForgiveness = JsonConvert.DeserializeObject<SbaPPPLoanForgiveness>(restResponse.Content);
                    Utility.Utility.LogAction($"{Environment.NewLine}Delete was successful{Environment.NewLine}");

                }
                else
                {
                    var jObject = JObject.Parse(restResponse.Content);
                    string status = "Deletion Failed";
                    string error = restResponse.Content;
                    Claysys.PPP.Forgiveness.PPPForgiveness.dataManagementObj.UpdateForgivenessDb(sql_cmnd, sbanumber, status, error, slug);
                    Utility.Utility.LogAction($"Did not receive success code for SBA Number: {sbaNumber} Deletion. please investigate. received response: {Environment.NewLine}StatusCode - {restResponse.StatusCode}{Environment.NewLine}Response - {restResponse.Content}");
                    return false;
                }

                _sqlCon.Close();
                return true;
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
