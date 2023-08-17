using HomeBanking.Models.DTO;
using HomeBanking.Models;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;

namespace HomeBanking.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;

        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository, ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var loans = _loanRepository.GetAllLoans();

                if (loans == null)
                {
                    return Forbid("Could not get avaible loans");
                }

                List<LoanDTO> loanDTOs = new List<LoanDTO>();

                foreach (var loan in loans)
                {
                    LoanDTO loanDTO = new LoanDTO()
                    {
                        Id = loan.Id,
                        Name = loan.Name,
                        MaxAmount = loan.MaxAmount,
                        Payments = loan.Payments,
                    };
                    loanDTOs.Add(loanDTO);
                }
                return Ok(loanDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] LoanApplicationDTO loanApplicationDTO)
        {
            try
            {
                if (loanApplicationDTO.Amount < 1 || String.IsNullOrEmpty(loanApplicationDTO.ToAccountNumber) || String.IsNullOrEmpty(loanApplicationDTO.Payments))
                    return StatusCode(403, "Datos invalidos");

                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                    return Forbid();

                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                    return Forbid();

                var loan = _loanRepository.FindById(loanApplicationDTO.LoanId);

                if (loan == null ||
                    loanApplicationDTO.Amount >= loan.MaxAmount ||
                    (loanApplicationDTO.Payments == null || loanApplicationDTO.Payments == string.Empty) ||
                    !loan.Payments.Split(',').Contains(loanApplicationDTO.Payments))
                    return StatusCode(403, "Datos invalidos");

                var account = _accountRepository.FindByNumber(loanApplicationDTO.ToAccountNumber);
                if (account == null || account.ClientId != client.Id)
                    return Forbid();

                Account updatedAccount = account;
                updatedAccount.Balance = account.Balance + loanApplicationDTO.Amount;

                ClientLoan clientloan = new ClientLoan
                {
                    ClientId = client.Id,
                    Amount = loanApplicationDTO.Amount + loanApplicationDTO.Amount * 0.2,
                    Payments = loanApplicationDTO.Payments,
                    LoanId = loanApplicationDTO.LoanId,
                };
                _clientLoanRepository.Save(clientloan);

                Transaction transaction = new Transaction
                {
                    AccountId = account.Id,
                    Amount = loanApplicationDTO.Amount,
                    Date = DateTime.Now,
                    Description = loan.Name + " loan approved",
                    Type = TransactionType.CREDIT.ToString(),
                };
                _transactionRepository.Save(transaction);
                _accountRepository.Save(account);

                return Ok(clientloan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
