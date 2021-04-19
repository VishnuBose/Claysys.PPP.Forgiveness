using System;
using System.Threading.Tasks;
using Claysys.PPP.Forgiveness.domain;
using Claysys.PPP.Forgiveness.service;

namespace Claysys.PPP.Forgiveness.controller
{
    public class SbaLoanForgivenessMessageController : IDisposable
    {
        public SbaLoanForgivenessMessageController()
        {

        }
        private readonly SbaLoanForgivenessMessageService sbaLoanForgivenessMessageService;

        public SbaLoanForgivenessMessageController(SbaLoanForgivenessMessageService sbaLoanForgivenessMessageService)
        {
            this.sbaLoanForgivenessMessageService = sbaLoanForgivenessMessageService;
        }

        public Task<MessageReply> updateSbaLoanMessageReply(MessageReply request, string loanForgivenessMessageUrl)
        {
            Utility.Utility.LogAction("Processing Update LoanForgiveness Message Reply");
            return sbaLoanForgivenessMessageService.updateSbaLoanMessageReply(request, loanForgivenessMessageUrl);
        }

        public SbaPPPLoanMessagesResponse getForgivenessMessagesBySbaNumber(int page, String sbaNumber, bool isComplete, string loanForgivenessMessageUrl)
        {
            Console.WriteLine("Retreiving LoanForgiveness Request Messages by SBA Number");
            return sbaLoanForgivenessMessageService.getForgivenessMessagesBySbaNumber(page, sbaNumber, isComplete, loanForgivenessMessageUrl);
        }

        public Task<SbaPPPLoanForgivenessMessage> getLoanMessagesBySlug(string slug, string loanForgivenessMessageUrl)
        {
            Utility.Utility.LogAction("Retreiving LoanForgiveness Message");
            return sbaLoanForgivenessMessageService.getLoanMessagesBySlug(slug, loanForgivenessMessageUrl);
        }

        public void Dispose()
        {
        }
    }
}
