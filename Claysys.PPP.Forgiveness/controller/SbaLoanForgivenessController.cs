using System;
using System.Threading.Tasks;
using Claysys.PPP.Forgiveness.domain;
using Claysys.PPP.Forgiveness.service;

namespace Claysys.PPP.Forgiveness.controller
{
    public class SbaLoanForgivenessController
    {
        private readonly SbaLoanForgivenessService sbaLoanForgivenessService;

        public SbaLoanForgivenessController(SbaLoanForgivenessService sbaLoanForgivenessService)
        {
            this.sbaLoanForgivenessService = sbaLoanForgivenessService;
        }

        public Task<SbaPPPLoanForgiveness> execute(SbaPPPLoanForgiveness request, string loanForgivenessUrl)
        {
            Utility.Utility.LogAction($"Submit Request Received: {request}");
            Task<SbaPPPLoanForgiveness> response = sbaLoanForgivenessService.execute(request, loanForgivenessUrl);
            return response;
        }

        public Task<SbaPPPLoanDocumentTypeResponse> getSbaLoanRequestStatus(int page, string sbaNumber, string loanForgivenessUrl)
        {
            Utility.Utility.LogAction("Get Request Received.");
            Task<SbaPPPLoanDocumentTypeResponse> response = sbaLoanForgivenessService.getLoanStatus(page, sbaNumber, loanForgivenessUrl);
            return response;
        }

        public Task<SbaPPPLoanForgivenessStatusResponse> getAllForgivenessRequests(string ppp_loan_forgiveness_requests)
        {
            Utility.Utility.LogAction("Get Request Received.");
            Task<SbaPPPLoanForgivenessStatusResponse> response = sbaLoanForgivenessService.getAllForgivenessRequests(ppp_loan_forgiveness_requests);
            return response;
        }

        public Task<SbaPPPLoanForgiveness> getSbaLoanForgivenessBySlug(string slug, string loanForgivenessUrl)
        {
            Utility.Utility.LogAction("Get Request Received.");
            Task<SbaPPPLoanForgiveness> response = sbaLoanForgivenessService.getSbaLoanForgivenessBySlug(slug, loanForgivenessUrl);
            return response;
        }

        public Task<SbaPPPLoanForgivenessStatusResponse> getForgivenessRequestBysbaNumber(string sbaNumber, string ppp_loan_forgiveness_requests)
        {
            Utility.Utility.LogAction("Get Request Received.");
            Task<SbaPPPLoanForgivenessStatusResponse> response = sbaLoanForgivenessService.getForgivenessRequestBysbaNumber(sbaNumber, ppp_loan_forgiveness_requests);
            return response;
        }

        public Task<SbaPPPLoanDocumentTypeResponse> getDocumenttypes(string loanForgivenessUrl)
        {
            Utility.Utility.LogAction("Get Request Received.");
            Task<SbaPPPLoanDocumentTypeResponse> response = sbaLoanForgivenessService.getDocumenttypes(loanForgivenessUrl);
            return response;
        }

        public async Task<bool> deleteSbaLoanForgiveness(string slug,string sbaNumber, string loanForgivenessUrl)
        {
            Utility.Utility.LogAction("Delete Request Received.");
            return await sbaLoanForgivenessService.deleteSbaLoanForgiveness(slug,sbaNumber, loanForgivenessUrl);
        }


    }
}
