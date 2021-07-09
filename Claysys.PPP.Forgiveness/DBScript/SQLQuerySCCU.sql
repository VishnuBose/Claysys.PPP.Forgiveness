
--STORED PROCEDURES


/****** Object:  StoredProcedure [dbo].[spi_ForgivenessDocument]    Script Date: 8/14/2020 4:26:54 AM ******/

/****** Object:  StoredProcedure [dbo].[spi_ForgivenessDocument]    Script Date: 9/4/2020 1:15:03 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--STORED PROCEDURES


/****** Object:  StoredProcedure [dbo].[spi_ForgivenessDocument]    Script Date: 8/14/2020 4:26:54 AM ******/
/****** Object:  StoredProcedure [dbo].[spi_ForgivenessDocument]    Script Date: 9/29/2020 3:09:04 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--STORED PROCEDURES


/****** Object:  StoredProcedure [dbo].[spi_ForgivenessDocument]    Script Date: 8/14/2020 4:26:54 AM ******/

CREATE PROCEDURE [dbo].[spi_ForgivenessDocument]
@SBANumber varchar(max),
@Slug varchar(max) = null,
@Name varchar(50) = null,
@CreatedAt varchar(50) = null,
@UpdatedAt varchar(50) = null,
@Document varchar(max) = null,
@DocumentType bigint = null,
@Url varchar(max) = null,
@EtranLoan varchar(max) = null
AS
BEGIN
   
    INSERT INTO ForgivenessDocument (
	SBANumber,
    Slug,
    Name,
	CreatedAt,
    UpdatedAt,
	[Document],
	DocumentType,
	Url,
	EtranLoan
	)
	VALUES(
	@SBANumber,
	@Slug,
	@Name,
	@CreatedAt,
	@UpdatedAt,
	@Document,
	@DocumentType,
	@Url,
	@EtranLoan
	)
END
GO


/****** Object:  StoredProcedure [dbo].[spi_ForgivessPaymentDetails]    Script Date: 9/29/2020 3:10:29 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spi_ForgivessPaymentDetails] (
	 @BorrowerName VARCHAR(50) null
	,@SBANumber VARCHAR(50)
	,@DisbursementDate VARCHAR(50) null
	,@LoanAmount VARCHAR(50) null
	,@FinalForgiveAmount VARCHAR(50) null
	,@SBADicisionDate VARCHAR(50) null
	,@CalculatedInterest VARCHAR(50) null
	,@EIDLAdvanceReductionAmount VARCHAR(50) null
	,@Payment VARCHAR(50) null
	,@PaymentDate VARCHAR(50) null
	,@PaymentBatch VARCHAR(50) null 
	,@PaymentStatus VARCHAR(50) null
	,@SBADecision VARCHAR(50) null
	)
AS
BEGIN
	IF (
			(
				SELECT count(SBANumber)
				FROM ForgivenessPayment
				WHERE SBANumber = @SBANumber
				) = 0
			)
	BEGIN
		INSERT INTO ForgivenessPayment (
			[BorrowerName]
			,[SBANumber]
			,[DisbursementDate]
			,[LoanAmount]
			,[FinalForgiveAmount]
			,[SBADicisionDate]
			,[CalculatedInterest]
			,[EIDLAdvanceReductionAmount]
			,[Payment]
			,[PaymentDate]
			,[PaymentBatch]
			,[PaymentStatus]
			,[SBADecision]
			,[CreatedDate]
			,[ModifiedDate]
			)
		VALUES (
			@BorrowerName
			,@SBANumber
			,@DisbursementDate
			,@LoanAmount
			,@FinalForgiveAmount
			,@SBADicisionDate
			,@CalculatedInterest
			,@EIDLAdvanceReductionAmount
			,@Payment
			,@PaymentDate
			,@PaymentBatch
			,@PaymentStatus
			,@SBADecision
			,GETDATE()
			,NULL
			)
	END
	ELSE
	BEGIN
		UPDATE ForgivenessPayment
		SET [BorrowerName] = @BorrowerName
			,[SBANumber] = @SBANumber
			,[DisbursementDate] = @DisbursementDate
			,[LoanAmount] = @LoanAmount
			,[FinalForgiveAmount] = @FinalForgiveAmount
			,[SBADicisionDate] = @SBADicisionDate
			,[CalculatedInterest] = @CalculatedInterest
			,[EIDLAdvanceReductionAmount] = @EIDLAdvanceReductionAmount
			,[Payment] = @Payment
			,[PaymentDate] = @PaymentDate
			,[PaymentBatch] = @PaymentBatch
			,[PaymentStatus] = @PaymentStatus
			,[SBADecision] = @SBADecision
			,[ModifiedDate] = GETDATE()
		WHERE SBANumber = @SBANumber
	END
END
GO




/****** Object:  StoredProcedure [dbo].[spi_ForgivenessMessage]    Script Date: 9/4/2020 12:17:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[spi_ForgivenessMessage] 
@SBANumber bigint,
@Subject varchar(max) = null,
@Ticket varchar(50) = null,
@Messages varchar(max) = null,
@IsComplete BIT = NULL

AS
BEGIN
		 IF (
				(SELECT count(SBALoanNo)
				FROM ForgivenessApplication
				WHERE SBALoanNo = @SBANumber
				) = 0
	      )
		BEGIN
		   INSERT INTO [dbo].[ForgivenessMessage]
				   ([SbaNumber]
				   ,[Subject]
				   ,[Ticket]
				   ,[Messages]
				   ,[IsComplete])
			 VALUES
				   (@SBANumber,
					@Subject,
					@Ticket,
					@Messages,
					@IsComplete)

		 END
		 ELSE
	     BEGIN
			UPDATE [dbo].[ForgivenessMessage]
			SET [SbaNumber] = @SBANumber
				,[Subject] = @Subject
				,[Ticket] = @Ticket
				,[Messages] = @Messages
				,[IsComplete] =  @IsComplete
			WHERE SbaNumber = @SBANumber
         END

END


/****** Object:  StoredProcedure [dbo].[Sps_ForgivenessApplicationSelect]    Script Date: 9/3/2020 12:55:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[Sps_ForgivenessApplicationSelect]
AS
BEGIN
	(
	SELECT TOP 25 SA.SBALoanNumber
			,SA.LenderLoanNumber AS LenderPPPLoanNumber
			,SA.PPPLoandisbursementDate
			,SA.BusinessLegalName AS EntityName
			,SA.PPPLoanAmount
			,SA.TINNumber
			,PLF.SBAStatus
			,SA.EIDLAdvanceAmount
			,SA.EIDLApplicationNumber
			,NULL AS PayrollCosts
			,NULL AS CFLine3
			,NULL AS CFLine4
			,NULL AS CFLine2
			,SA.BusinessAddress AS BusinessAddress1
			,SA.BusinessAddress AS BusinessAddress2
			,SA.DBAName
			,SA.BusinessPhone
			,SA.EmployeesAtApplicationTime
			,NULL AS CFLine5
			,NULL AS CFLine7
			,SA.ForgivenessAmount AS CFLine8
			,SA.EmployeesAtForgivenessTime
			,NULL AS ForgiveModifiedTotal
			,NULL AS ForgiveScheduleALine1
			,NULL AS ForgiveScheduleALine2
			,NULL AS ForgiveScheduleALine3
			,NULL AS ForgiveScheduleALine3Chk
			,NULL AS ForgiveScheduleALine4
			,NULL AS ForgiveScheduleALine5
			,NULL AS ForgiveScheduleALine6
			,NULL AS ForgiveScheduleALine7
			,NULL AS ForgiveScheduleALine8
			,NULL AS ForgiveScheduleALine9
			,NULL AS ForgiveScheduleALine10
			,NULL AS ForgiveScheduleALine10Chk
			,NULL AS ForgiveScheduleALine11
			,NULL AS ForgiveScheduleALine12
			,NULL AS ForgiveScheduleALine13
			,NULL AS NoReductionInEmployees
			,NULL AS NoreductionInEmployeesAndCovidImpact
			,NULL AS CoveredPeriodFrom
			,NULL AS CoveredPeriodTo
			,NULL AS AltCoveredPeriodFrom
			,NULL AS AltCoveredPeriodTo
			,NULL AS ExcessPPPLoans
			,NULL AS PayrollSchedule
			,CASE 
				WHEN PLF.LenderDecision = 'APPROVED IN FULL'
					THEN 0
				WHEN PLF.LenderDecision = 'APPROVED IN PART'
					THEN 1
				WHEN PLF.LenderDecision = 'DENIED'
					THEN 2
				WHEN PLF.LenderDecision = 'DENIED WITHOUT PREJUDICE DUE TO PENDING SBA REVIEW'
					THEN 3
				END AS ForgiveLenderDecision
			,SA.Email
			,SA.PrimaryContact
			,'SmallApp' AS IsEzform
			,'true' AS forgive_lender_confirmation
			,CASE 
				WHEN (
						SA.PrincipleName IS NULL
						OR SA.PrincipleName = ''
						)
					THEN CASE 
							WHEN NP.BusinessOrPerson = 'Person'
								THEN (NP.FNamePrincipal + ' ' + NP.LNamePrincipal)
							ELSE NP.BusinessNamePrincipal
							END
				ELSE SA.PrincipleName
				END AS DemoName
			,CASE 
				WHEN (
						SA.Position IS NULL
						OR SA.Position = ''
						)
					THEN CASE 
							WHEN NP.BusinessOrPerson = 'Person'
								THEN NP.TitlePrincipal
							ELSE NP.JobTitle
							END
				ELSE SA.Position
				END AS DemoPosition
			,UPPER(SA.Gender) AS Demogender
			,UPPER(SA.Ethinicity) AS DemoEthinicity
			,UPPER(SA.Race) AS DemoRace
			,UPPER(SA.Veteran) AS DemoVeteran
		FROM [dbo].[LF_EZAppDetails] EZ
		INNER JOIN PPP_Loan_Forgiveness PLF ON EZ.LoanApplicationNumber = PLF.LoanApplicationNumber
		INNER JOIN NextGenPPP NP ON NP.LoanApplicationNumber = PLF.LoanApplicationNumber
		INNER JOIN LF_SmallAppDetails SA ON SA.LoanApplicationNumber = PLF.LoanApplicationNumber
		WHERE PLF.STATUS = 'DocuSign Complete'
			AND (
				PLF.LenderDecision = 'APPROVED IN FULL'
				OR PLF.LenderDecision = 'APPROVED IN PART'
				OR PLF.LenderDecision = 'DENIED'
				OR PLF.LenderDecision = 'DENIED WITHOUT PREJUDICE DUE TO PENDING SBA REVIEW'
				)
			AND EZ.IsEZInitiated = 'SmallApp'
			AND (SBAStatus is null OR SBAStatus = '' OR SBAStatus = 'Awaiting')
	)
	UNION ALL
	(
			SELECT TOP 25 LF.SBAPPPLoanNumber AS SBALoanNumber
				,LF.LenderPPPLoanNumber AS LenderPPPLoanNumber
				,LF.LoanDisbursementDate AS PPPLoandisbursementDate
				,LF.LoanForgivenessCalculation AS EntityName
				,LF.PPPLoanAmount
				,LF.TINID AS TINNumber
				,PLF.SBAStatus
				,LF.EIDLAdvanceAmount
				,LF.EIDLApplicationNumber AS EIDLApplicationNumber
				,LF.PayrollCosts
				,LF.BusinessRentorLeasePayments AS CFLine3
				,LF.BusinessUtilityPayments AS CFLine4
				,LF.BusinessMortgageInterestPayments AS CFLine2
				,LF.BusinessAddress AS BusinessAddress1
				,LF.BusinessAddress AS BusinessAddress2
				,LF.BorrowerDBA AS DBAName
				,LF.BusinessPhone AS BusinessPhone
				,LF.NoOfEmployeesInitial AS EmployeesAtApplicationTime
				,LF.LoanExpenses AS CFLine5
				,LF.PayrollCost75Requirement AS CFLine7
				,LF.ForgivenessAmount AS CFLine8
				,LF.NoOfEmployeesFinal AS EmployeesAtForgivenessTime
				,LF.ModifiedTotal AS ForgiveModifiedTotal
				,SA.CashCompensation AS ForgiveScheduleALine1
				,SA.AverageFTE AS ForgiveScheduleALine2
				,SA.HourlyWageReduction AS ForgiveScheduleALine3
				,SA.HourlyWageReduction_Check AS ForgiveScheduleALine3Chk
				,SA.CashCompensation_Table2 AS ForgiveScheduleALine4
				,SA.AverageFTE_Table2 AS ForgiveScheduleALine5
				,SA.AmountEmployeeHealthInsurance AS ForgiveScheduleALine6
				,SA.AmountEmployeeRetirementPlans AS ForgiveScheduleALine7
				,SA.AmountLocalTaxesAssessed AS ForgiveScheduleALine8
				,SA.IndividualAmountPaid AS ForgiveScheduleALine9
				,SA.PayrollCosts AS ForgiveScheduleALine10
				,SA.FTERedSafeHarb2 AS ForgiveScheduleALine10Chk
				,SA.AverageFTE_ChosenPeriod AS ForgiveScheduleALine11
				,SA.TotalAverageFTE AS ForgiveScheduleALine12
				,SA.FTEReductionQuotient AS ForgiveScheduleALine13
				,SA.FTE_Check AS NoReductionInEmployees
				,SA.FTERedSafeHarb1 AS NoreductionInEmployeesAndCovidImpact
				,LF.CoveredPeriodFrom AS CoveredPeriodFrom
				,LF.CoveredPeriodTo AS CoveredPeriodTo
				,LF.AlternativePayrollCoveredPeriodFrom AS AltCoveredPeriodFrom
				,LF.AlternativePayrollCoveredPeriodTo AS AltCoveredPeriodTo
				,LF.PPPLoansinExcess AS ExcessPPPLoans
				,LF.PayrollSchedule AS PayrollSchedule
				,CASE 
					WHEN PLF.LenderDecision = 'APPROVED IN FULL'
						THEN 0
					WHEN PLF.LenderDecision = 'APPROVED IN PART'
						THEN 1
					WHEN PLF.LenderDecision = 'DENIED'
						THEN 2
					WHEN PLF.LenderDecision = 'DENIED WITHOUT PREJUDICE DUE TO PENDING SBA REVIEW'
						THEN 3
					END AS ForgiveLenderDecision
				,LF.EmailAddress AS Email
				,LF.PrimaryContact
				,'false' AS IsEzform
				,'true' AS forgive_lender_confirmation
				,CASE 
					WHEN (
							DG.PrincipalName IS NULL
							OR DG.PrincipalName = ''
							)
						THEN CASE 
								WHEN NP.BusinessOrPerson = 'Person'
									THEN (NP.FNamePrincipal + ' ' + NP.LNamePrincipal)
								ELSE NP.BusinessNamePrincipal
								END
					ELSE DG.PrincipalName
					END AS DemoName
				,CASE 
					WHEN (
							DG.Position IS NULL
							OR DG.Position = ''
							)
						THEN CASE 
								WHEN NP.BusinessOrPerson = 'Person'
									THEN NP.TitlePrincipal
								ELSE NP.JobTitle
								END
					ELSE DG.Position
					END AS DemoPosition
				,UPPER(DG.Gender) AS DemoGender
				,UPPER(DG.Ethnicity) AS DemoEthinicity
				,UPPER(DG.Race) AS DemoRace
				,UPPER(DG.Veteran) AS DemoVeteran
			FROM [dbo].[LoanForgiveness] LF
			INNER JOIN [LF_PPPBorrowerDemographicInformation] DG ON LF.LoanApplicationNumber = DG.LoanApplicationNumber
			INNER JOIN [ScheduleA] SA ON LF.LoanApplicationNumber = SA.LoanApplicationNumber
			INNER JOIN PPP_Loan_Forgiveness PLF ON PLF.LoanApplicationNumber = LF.LoanApplicationNumber
			INNER JOIN LF_EZAppDetails EZ ON PLF.LoanApplicationNumber = EZ.LoanApplicationNumber
			INNER JOIN NextGenPPP NP ON NP.LoanApplicationNumber = LF.LoanApplicationNumber
			WHERE PLF.STATUS = 'DocuSign Complete'
				AND (
					PLF.LenderDecision = 'APPROVED IN FULL'
					OR PLF.LenderDecision = 'APPROVED IN PART'
					OR PLF.LenderDecision = 'DENIED'
					OR PLF.LenderDecision = 'DENIED WITHOUT PREJUDICE DUE TO PENDING SBA REVIEW'
					)
				AND EZ.IsEZInitiated = 'FullApp'
			    AND (SBAStatus is null OR SBAStatus = '' OR SBAStatus = 'Awaiting')
			)
	
	UNION ALL
	
	(
		SELECT TOP 25 EZ.SBALoanNumber
			,EZ.LenderLoanNumber AS LenderPPPLoanNumber
			,PPPLoandisbursementDate
			,EZ.BusinessLegalName AS EntityName
			,PPPLoanAmount
			,TINNumber
			,PLF.SBAStatus
			,EIDLAdvanceAmount
			,EIDLApplicationNumber
			,PayrollCosts
			,CFLine3
			,CFLine4
			,CFLine2
			,BusinessAddress AS BusinessAddress1
			,BusinessAddress AS BusinessAddress2
			,EZ.DBAName
			,BusinessPhone
			,EmployeesAtApplicationTime
			,CFLine5
			,CFLine7
			,CFLine8
			,EmployeesAtForgivenessTime
			,NULL AS ForgiveModifiedTotal
			,NULL AS ForgiveScheduleALine1
			,NULL AS ForgiveScheduleALine2
			,NULL AS ForgiveScheduleALine3
			,NULL AS ForgiveScheduleALine3Chk
			,NULL AS ForgiveScheduleALine4
			,NULL AS ForgiveScheduleALine5
			,NULL AS ForgiveScheduleALine6
			,NULL AS ForgiveScheduleALine7
			,NULL AS ForgiveScheduleALine8
			,NULL AS ForgiveScheduleALine9
			,NULL AS ForgiveScheduleALine10
			,NULL AS ForgiveScheduleALine10Chk
			,NULL AS ForgiveScheduleALine11
			,NULL AS ForgiveScheduleALine12
			,NULL AS ForgiveScheduleALine13
			,NULL AS NoReductionInEmployees
			,NULL AS NoreductionInEmployeesAndCovidImpact
			,CoveredPeriodFrom
			,CoveredPeriodTo
			,AltCoveredPeriodFrom
			,AltCoveredPeriodTo
			,ExcessPPPLoans
			,PayrollSchedule
			,CASE 
				WHEN PLF.LenderDecision = 'APPROVED IN FULL'
					THEN 0
				WHEN PLF.LenderDecision = 'APPROVED IN PART'
					THEN 1
				WHEN PLF.LenderDecision = 'DENIED'
					THEN 2
				WHEN PLF.LenderDecision = 'DENIED WITHOUT PREJUDICE DUE TO PENDING SBA REVIEW'
					THEN 3
				END AS ForgiveLenderDecision
			,EZ.Email
			,PrimaryContact
			,'true' AS IsEzform
			,'true' AS forgive_lender_confirmation
			,CASE 
				WHEN (
						PrincipleName IS NULL
						OR PrincipleName = ''
						)
					THEN CASE 
							WHEN NP.BusinessOrPerson = 'Person'
								THEN (NP.FNamePrincipal + ' ' + NP.LNamePrincipal)
							ELSE NP.BusinessNamePrincipal
							END
				ELSE PrincipleName
				END AS DemoName
			,CASE 
				WHEN (
						Position IS NULL
						OR Position = ''
						)
					THEN CASE 
							WHEN NP.BusinessOrPerson = 'Person'
								THEN NP.TitlePrincipal
							ELSE NP.JobTitle
							END
				ELSE Position
				END AS DemoPosition
			,UPPER(Gender) AS Demogender
			,UPPER(Ethinicity) AS DemoEthinicity
			,UPPER(Race) AS DemoRace
			,UPPER(Veteran) AS DemoVeteran
		FROM [dbo].[LF_EZAppDetails] EZ
		INNER JOIN PPP_Loan_Forgiveness PLF ON EZ.LoanApplicationNumber = PLF.LoanApplicationNumber
		INNER JOIN NextGenPPP NP ON NP.LoanApplicationNumber = PLF.LoanApplicationNumber
		WHERE PLF.STATUS = 'DocuSign Complete'
			AND (
				PLF.LenderDecision = 'APPROVED IN FULL'
				OR PLF.LenderDecision = 'APPROVED IN PART'
				OR PLF.LenderDecision = 'DENIED'
				OR PLF.LenderDecision = 'DENIED WITHOUT PREJUDICE DUE TO PENDING SBA REVIEW'
				)
			AND EZ.IsEZInitiated = 'EZApp'
			AND (SBAStatus is null OR SBAStatus = '' OR SBAStatus = 'Awaiting')
			--AND PLF.SBALoanNumber in (
			--'25136073-07',
			--'99104372-01',
			--'55133773-03',
			--'61236673-06'
			--)
		)
		
END





GO


/****** Object:  StoredProcedure [dbo].[Sps_ForgivenessGetAdditionalDocuments]    Script Date: 9/3/2020 3:31:50 AM ******/


		/****** Object:  StoredProcedure [dbo].[Sps_ForgivenessGetAdditionalDocuments]    Script Date: 4/28/2021 6:01:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Sps_ForgivenessGetAdditionalDocuments] (@sbaLoanNumber NVARCHAR(50))
AS
BEGIN
	DECLARE @loanApplicationNumber NVARCHAR(50)

	IF (
			(
				SELECT count(*)
				FROM PPP_Loan_Forgiveness
				WHERE SBALoanNumber = @sbaLoanNumber
				) = 0
			)
	BEGIN
		SET @loanApplicationNumber = (
				SELECT TOP 1 LoanApplicationNumber
				FROM PPP_Loan_Forgiveness
				WHERE SBALoanNumber = (substring(@sbaLoanNumber, 1, 8) + '-' + substring(@sbaLoanNumber, 9, 2))
				)
	END
	ELSE
	BEGIN
		SET @loanApplicationNumber = (
				SELECT TOP 1 LoanApplicationNumber
				FROM PPP_Loan_Forgiveness
				WHERE SBALoanNumber = @sbaLoanNumber
				)
	END (
				SELECT [FileContent]
					,[FileName]
					,CASE 
						WHEN [FileType] = 'Denial Justification'
							THEN 3
						WHEN [FileType] = 'Loan Application Supporting Documents for Self-Emp…Individuals, Independent Contractors and Partners'
							THEN 4
						WHEN [FileType] = 'Faith-Based Addendum to 2483'
							THEN 5
						WHEN [FileType] = 'Addendum B to 2483'
							THEN 6
						WHEN [FileType] = 'Addendum A to 2483'
							THEN 7
						WHEN [FileType] = 'Forgiveness Supporting Docs (Mortgage Interest Payments)'
							THEN 8
						WHEN [FileType] = 'Miscellaneous'
							THEN 9
						WHEN [FileType] = 'PPP Schedule A Worksheet - FTE Reduction Safe Harbor 2'
							THEN 10
						WHEN [FileType] = 'PPP Schedule A Worksheet - Table 2'
							THEN 11
						WHEN [FileType] = 'PPP Schedule A Worksheet - Table 1'
							THEN 12
						WHEN [FileType] = '3508 AND 3508-EZ Supporting Docs (Public Health Operating Restrictions)'
							THEN 13
						WHEN [FileType] = '3508-EZ Supporting Docs (FTE Certification)'
							THEN 14
						WHEN [FileType] = '3508 AND 3508-EZ Supporting Docs (Job Offer, Refusal, etc. Certification)'
							THEN 15
						WHEN [FileType] = '3508-EZ Supporting Docs (Salary AND Wage Certification)'
							THEN 16
						WHEN [FileType] = 'PPP Schedule A Worksheet'
							THEN 17
						WHEN [FileType] = 'PPP Schedule A'
							THEN 18
						WHEN [FileType] = 'PPP Borrower Demographic Information Form'
							THEN 19
						WHEN [FileType] = 'Forgiveness Supporting Docs (Utility Payments)'
							THEN 20
						WHEN [FileType] = 'Forgiveness Supporting Docs (Rent/Lease Payments)'
							THEN 21
						WHEN [FileType] = 'Forgiveness Supporting Docs (FTE)'
							THEN 22
						WHEN [FileType] = 'Forgiveness Supporting Docs (Payroll)'
							THEN 23
						WHEN [FileType] = 'Transcript of Account'
							THEN 24
						WHEN [FileType] = 'Borrower Note'
							THEN 25
						WHEN [FileType] = 'SBA Form 3508EZ'
							THEN 26
						WHEN [FileType] = 'SBA Form 3508'
							THEN 27
						WHEN [FileType] = 'SBA Form 2483'
							THEN 30
						WHEN [FileType] = 'SBA Form 2484'
							THEN 31
						WHEN [FileType] = 'Loan Application Supporting Docs (Payroll)'
							THEN 1
						WHEN [FileType] = 'Payroll Schedule A Line4'
							THEN 23 -- need clarification
						WHEN [FileType] = 'Payroll Schedule A Line9'
							THEN 23 -- need clarification
						WHEN [FileType] = 'SBA Form 3508S'
							THEN 35
						END AS DocumentType
				FROM [dbo].[MBLAdditionalDocs]
				WHERE LoanApplicationNumber = @loanApplicationNumber
				)
		
		UNION ALL
		
		(
			SELECT DD.SignedDocument AS [FileContent]
				,CASE 
					WHEN EZ.IsEZInitiated = 'FullApp'
						THEN 'SBA Form 3508.pdf'
					ELSE 'SBA Form 3508 EZ.pdf'
					END AS [FileName]
				,CASE 
					WHEN EZ.IsEZInitiated = 'FullApp'
						THEN 27
					ELSE 26
					END AS DocumentType
			FROM PPP_Loan_Forgiveness PLF
			INNER JOIN DocSignDocuments DD ON PLF.DocuSignID = DD.DocID
			INNER JOIN [LF_EZAppDetails] EZ ON Ez.LoanApplicationNumber = PLF.LoanApplicationNumber
			WHERE PLF.LoanApplicationNumber = @loanApplicationNumber
			)
END



GO


/****** Object:  StoredProcedure [dbo].[sps_ForgivenessGetAPIDetails]    Script Date: 9/3/2020 3:32:07 AM ******/



CREATE PROCEDURE [dbo].[sps_ForgivenessGetAPIDetails]
  
AS
BEGIN
   SELECT CUName
      ,ConnectionString
      ,Vendorkey
      ,Token
      ,Active
  FROM [dbo].[ForgivenessAPITable] where Active = 1
END
GO

/****** Object:  StoredProcedure [dbo].[Sps_ForgivenessGetDocumentsEZ]    Script Date: 9/3/2020 3:32:20 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Sps_ForgivenessGetDocumentsEZ]    Script Date: 4/28/2021 7:03:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Sps_ForgivenessGetDocumentsEZ] (@sbaLoanNumber NVARCHAR(50))
AS
BEGIN
	DECLARE @loanApplicationNumber NVARCHAR(50)

	IF (
			(
				SELECT count(*)
				FROM PPP_Loan_Forgiveness
				WHERE SBALoanNumber = @sbaLoanNumber
				) = 0
			)
	BEGIN
		SET @loanApplicationNumber = (
				SELECT TOP 1 LoanApplicationNumber
				FROM PPP_Loan_Forgiveness
				WHERE SBALoanNumber = (substring(@sbaLoanNumber, 1, 8) + '-' + substring(@sbaLoanNumber, 9, 2))
				)
	END
	ELSE
	BEGIN
		SET @loanApplicationNumber = (
				SELECT TOP 1 LoanApplicationNumber
				FROM PPP_Loan_Forgiveness
				WHERE SBALoanNumber = @sbaLoanNumber
				)
	END

	SELECT TOP (1000) [ID]
		,[LoanApplicationNumber]
		,[PayrollAName]
		,[PayrollAFile]
		,[PayrollBName]
		,[PayrollBFile]
		,[PayrollCName]
		,[PayrollCFile]
		,[PayrollDName]
		,[PayrollDFile]
		,[NonPayrollAName]
		,[NonPayrollAFile]
		,[NonPayrollBName]
		,[NonPayrollBFile]
		,[NonPayrollCName]
		,[NonPayrollCFile]
		,[CertifySalaryName]
		,[CertifySalaryFile]
		,[EmployeeJobName]
		,[EmployeeJobFile]
		,[EmployeeNosName]
		,[EmployeeNosFile]
		,[CompanyOpsName]
		,[CompanyOpsFile]
		,[SupportAllDocsName]
		,[SupportAllDocsFile]
	FROM [dbo].[LF_EZDocs]
	WHERE LoanApplicationNumber = @loanApplicationNumber
END


/****** Object:  StoredProcedure [dbo].[Sps_ForgivenessGetDocumentsFullApp]    Script Date: 9/3/2020 3:32:34 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Sps_ForgivenessGetDocumentsFullApp]    Script Date: 4/28/2021 7:04:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Sps_ForgivenessGetDocumentsFullApp] (@sbaLoanNumber NVARCHAR(50))
AS
BEGIN
	DECLARE @loanApplicationNumber NVARCHAR(50)

	IF (
			(
				SELECT count(*)
				FROM PPP_Loan_Forgiveness
				WHERE SBALoanNumber = @sbaLoanNumber
				) = 0
			)
	BEGIN
		SET @loanApplicationNumber = (
				SELECT TOP 1 LoanApplicationNumber
				FROM PPP_Loan_Forgiveness
				WHERE SBALoanNumber = (substring(@sbaLoanNumber, 1, 8) + '-' + substring(@sbaLoanNumber, 9, 2))
				)
	END
	ELSE
	BEGIN
		SET @loanApplicationNumber = (
				SELECT TOP 1 LoanApplicationNumber
				FROM PPP_Loan_Forgiveness
				WHERE SBALoanNumber = @sbaLoanNumber
				)
	END

	SELECT [PayrollCompensationName]
		,[PayrollCompensationFile]
		,[PayrollTaxFormName]
		,[PayrollTaxFormFile]
		,[PayrollPayementsName]
		,[PayrollPayementsFile]
		,[FTEDocumentationName1]
		,[FTEDocumentFile1]
		,[FTEDocumentationName2]
		,[FTEDocumentFile2]
		,[FTEDocumentationName3]
		,[FTEDocumentFile3]
		,[NonpayrollName1]
		,[NonpayrollFile1]
		,[NonpayrollName2]
		,[NonpayrollFile2]
		,[NonpayrollName3]
		,[NonpayrollFile3]
		,[AdditionalDocumentName1]
		,[AdditionalDocumentFile1]
		,[AdditionalDocumentName2]
		,[AdditionalDocumentFile2]
		,[AdditionalDocumentName3]
		,[AdditionalDocumentFile3]
		,[AdditionalDocumentName4]
		,[AdditionalDocumentFile4]
		,[CustomerSafteyFileName]
		,[CustomerSafteyFile]
	FROM [dbo].[LF_ForgivenessDocuments]
	WHERE LoanApplicationNumber = @loanApplicationNumber
END


/****** Object:  StoredProcedure [dbo].[sps_ForgivenessSelectStatus]    Script Date: 9/3/2020 3:32:47 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/****** Object:  StoredProcedure [dbo].[sps_ForgivenessSelectStatus]    Script Date: 8/14/2020 4:24:00 AM ******/


CREATE PROCEDURE [dbo].[sps_ForgivenessSelectStatus]
AS
BEGIN
	SET NOCOUNT ON

	SELECT SBALoanNo as SBALoanNo,
	[Status] as Status
	FROM ForgivenessApplication
	--WHERE SBAStatus IS NULL
	--	OR SBAStatus = ''
	--	OR SBAStatus = 'Awaiting'
END
GO

/****** Object:  StoredProcedure [dbo].[spu_ForgivenessUpdation]    Script Date: 9/3/2020 3:33:13 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/****** Object:  StoredProcedure [dbo].[spu_ForgivenessUpdation]    Script Date: 8/14/2020 4:24:29 AM ******/
/****** Object:  StoredProcedure [dbo].[spu_ForgivenessUpdation]    Script Date: 4/28/2021 6:11:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[spu_ForgivenessUpdation]    Script Date: 8/14/2020 4:24:29 AM ******/
ALTER PROCEDURE [dbo].[spu_ForgivenessUpdation] @sbaLoanNumber VARCHAR(20) = NULL
	,@status VARCHAR(20)
	,@error VARCHAR(max) = NULL
	,@slug VARCHAR(50) = NULL
AS
BEGIN
	IF (
			(
				SELECT count(SBALoanNo)
				FROM ForgivenessApplication
				WHERE SBALoanNo = @sbaLoanNumber
				) = 0
			)
	BEGIN
		INSERT INTO [dbo].[ForgivenessApplication] (
			[SBALoanNo]
			,[Status]
			,[SlugID]
			,[Error]
			,[CreatedDate]
			)
		VALUES (
			@sbaLoanNumber
			,@status
			,@slug
			,@error
			,GETDATE()
			)
	END
	ELSE
	BEGIN
		UPDATE ForgivenessApplication
		SET [SBALoanNo] = @sbaLoanNumber
			,[Status] = @status
			,[SlugID] = @slug
			,[Error] = @error
			,[ModifiedDate] = GETDATE()
		WHERE SBALoanNo = @sbaLoanNumber
	END

	IF (
			(
				SELECT count(*)
				FROM PPP_Loan_Forgiveness
				WHERE SBALoanNumber = @sbaLoanNumber
				) = 0
			)
	BEGIN
		UPDATE PPP_Loan_Forgiveness
		SET SBAStatus = @status
		WHERE SBALoanNumber = (substring(@sbaLoanNumber, 1, 8) + '-' + substring(@sbaLoanNumber, 9, 2))

		UPDATE PPP_Loan_Forgiveness
		SET SBAStatus = @status
			,STATUS = 'Forgiveness Sent to SBA'
		WHERE SBALoanNumber = (substring(@sbaLoanNumber, 1, 8) + '-' + substring(@sbaLoanNumber, 9, 2))
			AND @status = 'Pending Validation'
	END
	ELSE
	BEGIN
		UPDATE PPP_Loan_Forgiveness
		SET SBAStatus = @status
		WHERE SBALoanNumber = @sbaLoanNumber

		UPDATE PPP_Loan_Forgiveness
		SET SBAStatus = @status
			,STATUS = 'Forgiveness Sent to SBA'
		WHERE SBALoanNumber = @sbaLoanNumber
			AND @status = 'Pending Validation'
	END
			-- update tbl_SbaForgivenessMDC set [Application Status]=@status,Error=@error,slug=@slug where SBA_Loan_Number=@sbaLoanNumber OR slug=@slug;
END


/****** Object:  StoredProcedure [dbo].[Spu_ForgivessStatusUpdation]    Script Date: 9/3/2020 3:33:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




/****** Object:  StoredProcedure [dbo].[Spu_ForgivessStatusUpdation]    Script Date: 8/14/2020 4:25:16 AM ******/

/****** Object:  StoredProcedure [dbo].[Spu_ForgivessStatusUpdation]    Script Date: 4/28/2021 6:39:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Spu_ForgivessStatusUpdation]    Script Date: 8/14/2020 4:25:16 AM ******/
ALTER PROCEDURE [dbo].[Spu_ForgivessStatusUpdation] (
	@status NVARCHAR(50)
	,@sbaLoanNumber NVARCHAR(50)
	)
AS
BEGIN
	DECLARE @sbanumberWithOutHiphen NVARCHAR

	IF (
			(
				SELECT count(*)
				FROM PPP_Loan_Forgiveness
				WHERE SBALoanNumber = @sbaLoanNumber
				) = 0
			)
	BEGIN
		UPDATE PPP_Loan_Forgiveness
		SET SBAStatus = @status
		WHERE SBALoanNumber = (substring(@sbaLoanNumber, 1, 8) + '-' + substring(@sbaLoanNumber, 9, 2))
	END
	ELSE
	BEGIN
		UPDATE PPP_Loan_Forgiveness
		SET SBAStatus = @status
		WHERE SBALoanNumber = @sbaLoanNumber
	END

	UPDATE ForgivenessApplication
	SET [SBALoanNo] = @sbaLoanNumber
		,[Status] = @status
		,[ModifiedDate] = GETDATE()
	WHERE SBALoanNo = @sbaLoanNumber
END



/****** Object:  StoredProcedure [dbo].[sps_ForgivenessDocumentCount]    Script Date: 4/28/2021 6:46:13 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[sps_ForgivenessDocumentCount]
(
	@sbaloanno nvarchar(50),
	@filename nvarchar(50)
)
AS
BEGIN
    select count(*) as [Count] from ForgivenessDocument where SBANumber =  @sbaloanno and Name =  RTRIM(LTRIM(@filename))
END
GO



 /****** Object:  Table [dbo].[ForgivenessApplication]    Script Date: 9/3/2020 3:35:05 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ForgivenessApplication](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SBALoanNo] [nvarchar](50) NULL,
	[Status] [nvarchar](50) NULL,
	[SlugID] [nvarchar](50) NULL,
	[Error] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


/****** Object:  Table [dbo].[ForgivenessAPITable]    Script Date: 9/3/2020 3:35:33 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ForgivenessAPITable](
	[CUName] [nvarchar](50) NULL,
	[ConnectionString] [nvarchar](max) NULL,
	[Vendorkey] [nvarchar](max) NULL,
	[Token] [nvarchar](max) NULL,
	[Active] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


/****** Object:  Table [dbo].[ForgivenessDocument]    Script Date: 9/3/2020 3:35:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ForgivenessDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SBANumber] [varchar](max) NOT NULL,
	[Slug] [varchar](max) NULL,
	[Name] [varchar](50) NULL,
	[CreatedAt] [varchar](50) NULL,
	[UpdatedAt] [varchar](50) NULL,
	[Document] [varchar](max) NULL,
	[DocumentType] [int] NULL,
	[Url] [varchar](max) NULL,
	[EtranLoan] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


/****** Object:  Table [dbo].[ForgivenessMessage]    Script Date: 9/3/2020 3:36:07 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ForgivenessMessage](
	[SbaNumber] [bigint] NOT NULL,
	[Subject] [varchar](max) NOT NULL,
	[Ticket] [varchar](50) NULL,
	[Messages] [varchar](max) NULL,
	[IsComplete] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ForgivenessPayment]    Script Date: 9/29/2020 2:35:58 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ForgivenessPayment](
	[BorrowerName] [nvarchar](50) NULL,
	[SBANumber] [nvarchar](50) NOT NULL,
	[DisbursementDate] [nvarchar](50) NULL,
	[LoanAmount] [nvarchar](50) NULL,
	[FinalForgiveAmount] [nvarchar](50) NULL,
	[SBADicisionDate] [nvarchar](50) NULL,
	[CalculatedInterest] [nvarchar](50) NULL,
	[EIDLAdvanceReductionAmount] [nvarchar](50) NULL,
	[Payment] [nvarchar](50) NULL,
	[PaymentDate] [date] NULL,
	[PaymentBatch] [nvarchar](50) NULL,
	[PaymentStatus] [nvarchar](50) NULL,
	[SBADecision] [nvarchar](50) NULL,
	[CreatedDate] [date] NULL,
	[ModifiedDate] [date] NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL
) ON [PRIMARY]
GO