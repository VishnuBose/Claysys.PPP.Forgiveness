using System.Collections.Generic;

namespace Claysys.PPP.Forgiveness.domain
{
	public class SbaPPPLoanDocumentTypeResponse
    {

        public int count{ get; set;}

        public string next{ get; set;}

        public string previous{ get; set;}

        public List<LoanDocumentType> results{ get; set;}

    }
}
