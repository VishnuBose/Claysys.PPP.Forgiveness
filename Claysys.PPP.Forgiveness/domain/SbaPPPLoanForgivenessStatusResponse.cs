using System;
using System.Collections.Generic;

namespace Claysys.PPP.Forgiveness.domain
{
    public class SbaPPPLoanForgivenessStatusResponse
    {

        public int count { get; set; }

        public string next { get; set; }

        public string previous { get; set; }

        public List<SbaPPPLoanForgiveness> results { get; set; }

        public DateTime created { get; set; }

        public string assigned_to_user { get; set; }
        public string Status { get; set; }
    }

    public class SbaPPPDisbursedLoanForgivenessStatusResponse
    {
        public int count { get; set; }

        public string next { get; set; }

        public string previous { get; set; }

        public List<SbaPPPDisbursedLoanForgiveness> results { get; set; }

    }
}
