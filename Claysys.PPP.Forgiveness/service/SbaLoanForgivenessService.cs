using System;
using System.Threading.Tasks;
using Claysys.PPP.Forgiveness.controller;
using Claysys.PPP.Forgiveness.domain;
using Claysys.PPP.Forgiveness.restclient;

namespace Claysys.PPP.Forgiveness.service
{
    public class SbaLoanForgivenessService
    {
        private readonly SbaRestApiClient sbaRestApiClient;

        public SbaLoanForgivenessService(SbaRestApiClient sbaRestApiClient)
        {
            this.sbaRestApiClient = sbaRestApiClient;
        }

        public SbaPPPLoanForgiveness Execute(SbaPPPLoanForgiveness request, string loanForgivenessUrl)
        {
            Utility.Utility.LogAction($"{request.etran_loan.sba_number} : Processing Sba Loan Forgiveness request in SBALoanForgivenessService");
            return sbaRestApiClient.InvokeSbaLoanForgiveness(request, loanForgivenessUrl);
        }

        public Task<SbaPPPLoanDocumentTypeResponse> getLoanStatus(int page, string sbaNumber, string loanForgivenessUrl)
        {
            Utility.Utility.LogAction("Retreiving Sba Loan Forgiveness request");
            return sbaRestApiClient.getSbaLoanForgiveness(page, sbaNumber, loanForgivenessUrl);
        }

        public Task<SbaPPPLoanForgivenessStatusResponse> getAllForgivenessRequests(string ppp_loan_forgiveness_requests)
        {
            Utility.Utility.LogAction("Retreiving all Forgiveness request");
            return sbaRestApiClient.getAllForgivenessRequests(ppp_loan_forgiveness_requests);
        }

        public Task<SbaPPPLoanForgiveness> getSbaLoanForgivenessBySlug(string slug, string loanForgivenessUrl)
        {
            Utility.Utility.LogAction("Retreiving Sba Loan Forgiveness request using slug");
            return sbaRestApiClient.getSbaLoanForgivenessBySlug(slug, loanForgivenessUrl);
        }

        public SbaPPPLoanForgivenessStatusResponse GetForgivenessRequestBysbaNumber(string sbaNumber, string ppp_loan_forgiveness_requests)
        {
            Utility.Utility.LogAction($"{sbaNumber} : Retreiving Sba Loan Forgiveness request using sbaNumber in SbaLoannForgivenessService.");
            return sbaRestApiClient.GetForgivenessRequestBysbaNumber(sbaNumber, ppp_loan_forgiveness_requests);
        }

        public Task<SbaPPPLoanForgivenessStatusResponse> GetForgivenessPaymentBysbaNumber(string sbaNumber, string ppp_loan_forgiveness_requests,string conString)
        {
            Utility.Utility.LogAction($"{sbaNumber} : Retreiving Sba Loan Forgiveness request using sbaNumber in SbaLoannForgivenessService.");
            return sbaRestApiClient.GetForgivenessPaymentBysbaNumber(sbaNumber, ppp_loan_forgiveness_requests, conString);
        }

        public SbaPPPDisbursedLoanForgivenessStatusResponse GetDisbursedLoanBySbaNumber(string sbaNumber, string ppp_loan_validations)
        {
            Utility.Utility.LogAction($"{sbaNumber} : Retreiving Sba Loan Forgiveness request using sbaNumber in SbaLoannForgivenessService.");
            return sbaRestApiClient.GetDisbursedLoanBySbaNumber(sbaNumber, ppp_loan_validations);
        }

        public Task<SbaPPPLoanDocumentTypeResponse> GetDocumenttypes(string loanForgivenessUrl)
        {
            Utility.Utility.LogAction("Retreiving document types in SbaLoanForgivenessService.");
            return sbaRestApiClient.GetSbaLoanForgiveness(loanForgivenessUrl);
        }

        public bool DeleteSbaLoanForgiveness(string slug, string sbaNumber, string loanForgivenessUrl)
        {
            Utility.Utility.LogAction($"{sbaNumber} : Deleting Sba Loan Forgiveness request in SbaLoanForgivenessService.");
            return sbaRestApiClient.DeleteSbaLoanForgiveness(slug, sbaNumber, loanForgivenessUrl);
        }

        public LoanDocumentResponse UploadForgivenessDocument(string requestName, string requestDocument_type, string etran_loan, byte[] document, string loanDocumentsUrl, string sbaNumber)
        {
            Utility.Utility.LogAction("Retreiving Sba Loan Forgiveness request");
            return sbaRestApiClient.UploadForgivenessDocument(requestName, requestDocument_type, etran_loan, document, loanDocumentsUrl, sbaNumber);
        }
    }
}
