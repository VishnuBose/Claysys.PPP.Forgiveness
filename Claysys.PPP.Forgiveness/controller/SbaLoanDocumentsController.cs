using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Claysys.PPP.Forgiveness.domain;
using Claysys.PPP.Forgiveness.service;

namespace Claysys.PPP.Forgiveness.controller
{
    public class SbaLoanDocumentsController
    {
        private readonly SbaLoanDocumentService sbaLoanDocumentService;
        public string DocumentName;
        public string documentType;
        public string etranId;
        public byte[] rawDocument;






        public SbaLoanDocumentsController(SbaLoanDocumentService sbaLoanDocumentService)
        {
            this.sbaLoanDocumentService = sbaLoanDocumentService;
        }

        public Task<SbaPPPLoanDocumentTypeResponse> getDocumentTypes(Dictionary<string, string> reqParams, string loanDocumentTypesUrl)
        {
            Utility.Utility.LogAction("Get Loan Docment types");
            Task<SbaPPPLoanDocumentTypeResponse> documentTypes = sbaLoanDocumentService.getDocumentTypes(reqParams, loanDocumentTypesUrl);
            return documentTypes;
        }

        public Task<LoanDocument> submitLoanDocument(LoanDocument request, string loanDocumentsUrl)
        {
            Utility.Utility.LogAction("Submit Loan Document.");
            Task<LoanDocument> document = sbaLoanDocumentService.submitLoanDocument(request, loanDocumentsUrl);
            return document;
        }
        
        public Task<LoanDocumentType> getSbaLoanDocumentTypeById(int id, string loanDocumentsUrl)
        {
            Utility.Utility.LogAction("Submit Loan Document.");
            Task<LoanDocumentType> document = sbaLoanDocumentService.getSbaLoanDocumentTypeById(id, loanDocumentsUrl);
            return document;
        }

        public LoanDocumentResponse UploadForgivenessDocument(string requestName, string requestDocument_type, string etran_loan, byte[] document, string loanDocumentsUrl, string sbaNumber, string slugId)
        {
            Utility.Utility.LogAction($"{sbaNumber} : Loan Document in SbaLoanDocumentsController.");
            LoanDocumentResponse loanDocument = sbaLoanDocumentService.UploadForgivenessDocument(requestName, requestDocument_type, etran_loan, document, loanDocumentsUrl, sbaNumber, slugId);
            return loanDocument;
        }
    }
}
