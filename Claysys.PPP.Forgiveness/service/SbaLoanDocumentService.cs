﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Claysys.PPP.Forgiveness.domain;
using Claysys.PPP.Forgiveness.controller;
using Claysys.PPP.Forgiveness.restclient;

namespace Claysys.PPP.Forgiveness.service
{
    public class SbaLoanDocumentService
    {
        private readonly SbaRestApiClient sbaRestApiClient;

        public SbaLoanDocumentService(SbaRestApiClient sbaRestApiClient)
        {
            this.sbaRestApiClient = sbaRestApiClient;
        }

        public Task<LoanDocument> submitLoanDocument(LoanDocument request, string loanDocumentsUrl)
        {
            Utility.Utility.LogAction("Processing Loan Document Submission.");
            return sbaRestApiClient.invokeSbaLoanDocument(request, loanDocumentsUrl);
        }

        public Task<SbaPPPLoanDocumentTypeResponse> getDocumentTypes(Dictionary<string, string> reqParams, string loanDocumentTypesUrl)
        {
            Utility.Utility.LogAction("Retreiving Sba Loan Document Types");
            return sbaRestApiClient.getSbaLoanDocumentTypes(reqParams, loanDocumentTypesUrl);
        }
        
        public Task<LoanDocumentType> getSbaLoanDocumentTypeById(int id, string loanDocumentTypesUrl)
        {
            Utility.Utility.LogAction("Retreiving Sba Loan Document Types");
            return sbaRestApiClient.getSbaLoanDocumentTypeById(id, loanDocumentTypesUrl);
        }

        public Task<LoanDocumentResponse> uploadForgivenessDocument(string requestName, string requestDocument_type, string etran_loan, string document, string loanDocumentsUrl,string sbaNumber)
        {
            Utility.Utility.LogAction("Processing Loan Document Submission.");
            return sbaRestApiClient.uploadForgivenessDocument(requestName, requestDocument_type, etran_loan, document, loanDocumentsUrl, sbaNumber);
        }
    }
}
