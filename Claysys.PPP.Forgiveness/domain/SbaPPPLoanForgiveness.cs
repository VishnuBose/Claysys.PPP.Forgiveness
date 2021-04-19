using System.Collections.Generic;

namespace Claysys.PPP.Forgiveness.domain
{
	public class SbaPPPLoanForgiveness
    {

        public int id{ get; set;}

        public string slug{ get; set;}

        public string borrower_name{ get; set;}

        public EtranLoan etran_loan{ get; set;}

    }

    public class SbaPPPDisbursedLoanForgiveness
    {
        public string organization_name { get; set; }

        public double? bank_notional_amount { get; set; }

        public string sba_number { get; set; }

        public string loan_number { get; set; }

        public string ein { get; set; }

        // format - yyyy-MM-dd
        public string funding_date { get; set; }

        public double? forgive_eidl_amount { get; set; }

        public string eidl_details { get; set; }

        //public IDictionary<string, string> eidl_details { get; set; }
        //public List<EidLDetails> eidl_details { get; set; }
    }
}
