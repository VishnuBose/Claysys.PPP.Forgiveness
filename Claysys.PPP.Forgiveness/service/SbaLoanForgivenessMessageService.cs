using System;
using System.Threading.Tasks;
using Claysys.PPP.Forgiveness.domain;
using Claysys.PPP.Forgiveness.restclient;

namespace Claysys.PPP.Forgiveness.service
{
    public class SbaLoanForgivenessMessageService
    {
        private readonly SbaRestApiClient sbaRestApiClient;

        public SbaLoanForgivenessMessageService(SbaRestApiClient sbaRestApiClient)
        {
            this.sbaRestApiClient = sbaRestApiClient;
        }

        public Task<MessageReply> updateSbaLoanMessageReply(MessageReply request, string loanForgivenessMessageUrl)
        {
            Utility.Utility.LogAction("Processing Update LoanForgiveness Message Reply");
            return sbaRestApiClient.updateSbaLoanForgivenessMessageReply(request, loanForgivenessMessageUrl);
        }

        public Task<SbaPPPLoanMessagesResponse> getForgivenessMessagesBySbaNumber(int page, String sbaNumber, bool isComplete, string loanForgivenessMessageUrl)
        {
            Console.WriteLine("Retreiving Forgiveness Request Messages by SBA Number");
            return sbaRestApiClient.getForgivenessMessagesBySbaNumber(page, sbaNumber, isComplete, loanForgivenessMessageUrl);
        }

        public Task<SbaPPPLoanForgivenessMessage> getLoanMessagesBySlug(string slug, string loanForgivenessMessageUrl)
        {
            Utility.Utility.LogAction("Retreiving LoanForgiveness Message");
            return sbaRestApiClient.getSbaLoanForgivenessMessagesBySlug(slug, loanForgivenessMessageUrl);
        }
    }
}
