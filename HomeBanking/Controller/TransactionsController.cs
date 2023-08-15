using HomeBanking.Models.DTO;
using HomeBanking.Models;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace HomeBanking.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;

        public TransactionsController(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        public IActionResult Post([FromBody] TransferDTO transferDTO)
        {
            Account fromAccount = null;
            Account toAccount = null;

            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : String.Empty;
                if (email == null)
                {
                    return Forbid("Have no authorizarion");
                }

                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                {
                    return Forbid("Client do not exists");
                }

                if (String.IsNullOrEmpty(transferDTO.FromAccountNumber) ||
                    String.IsNullOrEmpty(transferDTO.ToAccountNumber))
                {
                    return Forbid("From or To Account not given");
                }

                if (transferDTO.FromAccountNumber == transferDTO.ToAccountNumber)
                {
                    return Forbid("Can not transfer to the same account");
                }

                if (transferDTO.Amount <= 0 || String.IsNullOrEmpty(transferDTO.Description))
                {
                    return Forbid("Amount or description not given or valid");
                }

                fromAccount = _accountRepository.FindByNumber(transferDTO.FromAccountNumber);
                toAccount = _accountRepository.FindByNumber(transferDTO.ToAccountNumber);

                if (fromAccount == null || toAccount == null)
                {
                    return Forbid("From or To Account does not exist");
                }

                if (transferDTO.Amount > fromAccount.Balance)
                {
                    return Forbid("insufficient funds");
                }

                //generate debit in FromAccount
                Transaction debit = new Transaction()
                {
                    Type = TransactionType.DEBIT.ToString(),
                    Amount = transferDTO.Amount * -1, //multiply by -1 bc it must be negative
                    Description = $"{transferDTO.Description} to Account : {toAccount.Number}",
                    AccountId = fromAccount.Id,
                    Date = DateTime.Now,
                };
                //lo guardo en el repo
                _transactionRepository.Save(debit);

                //generate credit in ToAccount
                Transaction credit = new Transaction()
                {
                    Type = TransactionType.CREDIT.ToString(),
                    Amount = transferDTO.Amount,
                    Description = $"{transferDTO.Description} from Account: {fromAccount.Number} / {client.LastName}, {client.FirstName}",
                    AccountId = toAccount.Id,
                    Date = DateTime.Now,
                };
                _transactionRepository.Save(credit);

                fromAccount.Balance += debit.Amount;
                _accountRepository.Save(fromAccount);

                toAccount.Balance += credit.Amount;
                _accountRepository.Save(toAccount);

                return Created("", debit);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<Transaction> transactions = _transactionRepository.GetAllTransactions();
            if (transactions == null)
            {
                return Forbid("Could not access to transactions");
            }

            List<TransactionDTO> transactionsDTO = new List<TransactionDTO>();

            foreach (Transaction transaction in transactions)
            {
                transactionsDTO.Add(new TransactionDTO()
                {
                    Type = transaction.Type,
                    Amount = transaction.Amount,
                    Description = transaction.Description,
                    Date = transaction.Date,
                });
            }

            return Ok(transactionsDTO);
        }
    }
}
