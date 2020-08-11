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

        public Task<SbaPPPLoanForgiveness> Execute(SbaPPPLoanForgiveness request, string loanForgivenessUrl)
        {
            Utility.Utility.LogAction($"Submit Request Received in SBALoanForgivenessController : {request.etran_loan.sba_number}");
            Task<SbaPPPLoanForgiveness> response = sbaLoanForgivenessService.Execute(request, loanForgivenessUrl);
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

        public Task<SbaPPPLoanForgivenessStatusResponse> GetForgivenessRequestBysbaNumber(string sbaNumber, string ppp_loan_forgiveness_requests)
        {
            Utility.Utility.LogAction($"{sbaNumber} :Get Request Received in SbaLoanForgivenessController.");
            Task<SbaPPPLoanForgivenessStatusResponse> response = sbaLoanForgivenessService.GetForgivenessRequestBysbaNumber(sbaNumber, ppp_loan_forgiveness_requests);
            return response;
        }

        public Task<SbaPPPLoanDocumentTypeResponse> GetDocumenttypes(string loanForgivenessUrl)
        {
            Utility.Utility.LogAction("Get Request Received in SbaLoanForgivenessController.");
            Task<SbaPPPLoanDocumentTypeResponse> response = sbaLoanForgivenessService.GetDocumenttypes(loanForgivenessUrl);
            return response;
        }

        public async Task<bool> DeleteSbaLoanForgiveness(string slug,string sbaNumber, string loanForgivenessUrl)
        {
            Utility.Utility.LogAction($"{sbaNumber} : Delete Request Received in SbaLoanForgivenessController.");
            return await sbaLoanForgivenessService.DeleteSbaLoanForgiveness(slug,sbaNumber, loanForgivenessUrl);
        }


    }
}
