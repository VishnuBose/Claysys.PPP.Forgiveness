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

        public Task<SbaPPPLoanForgiveness> execute(SbaPPPLoanForgiveness request, string loanForgivenessUrl)
        {
            Utility.Utility.LogAction("Processing Sba Loan Forgiveness request");
            return sbaRestApiClient.invokeSbaLoanForgiveness(request, loanForgivenessUrl);
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

        public Task<SbaPPPLoanForgivenessStatusResponse> getForgivenessRequestBysbaNumber(string sbaNumber, string ppp_loan_forgiveness_requests)
        {
            Utility.Utility.LogAction("Retreiving Sba Loan Forgiveness request using sbaNumber");
            return sbaRestApiClient.getForgivenessRequestBysbaNumber(sbaNumber, ppp_loan_forgiveness_requests);
        }

        public Task<SbaPPPLoanDocumentTypeResponse> getDocumenttypes(string loanForgivenessUrl)
        {
            Utility.Utility.LogAction("Retreiving document types");
            return sbaRestApiClient.getSbaLoanForgiveness(loanForgivenessUrl);
        }

        public async Task<bool> deleteSbaLoanForgiveness(string slug,string sbaNumber, string loanForgivenessUrl)
        {
            Utility.Utility.LogAction("Deleting Sba Loan Forgiveness request");
            return await sbaRestApiClient.deleteSbaLoanForgiveness(slug,sbaNumber, loanForgivenessUrl);
        }

        public Task<LoanDocumentResponse> UploadForgivenessDocument(string requestName, string requestDocument_type, string etran_loan, string document, string loanDocumentsUrl,string sbaNumber)
        {
            Utility.Utility.LogAction("Retreiving Sba Loan Forgiveness request");
            return sbaRestApiClient.uploadForgivenessDocument( requestName,  requestDocument_type,  etran_loan,  document,  loanDocumentsUrl,sbaNumber);
        }
    }
}
