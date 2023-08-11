using HomeBanking.Models.DTO;
using HomeBanking.Models;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Net;

namespace HomeBanking.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountRepository _accountRepository;

        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }


        [HttpGet]
        public IActionResult Get()

        {

            try

            {

                var accounts = _accountRepository.GetAllAccounts();



                var accountDTO = new List<AccountDTO>();



                foreach (Account account in accounts)

                {

                    var newAccountDTO = new AccountDTO

                    {

                        Id = account.Id,

                        Number = account.Number,

                        CreationDate = account.CreationDate,

                        Balance = account.Balance,

                        Transactions = account.Transactions.Select(tr => new TransactionDTO

                        {

                            Id = tr.Id,

                            Type = tr.Type,

                            Amount = tr.Amount,

                            Description = tr.Description,

                            Date = tr.Date,

                        }).ToList()

                    };



                    accountDTO.Add(newAccountDTO);

                }





                return Ok(accountDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }
        // Crear la nueva cuenta
        [HttpPost]
        public AccountDTO Post(long clientId)
        {
            Random rnd = new Random();
            Account account;
            string newAccountNumber;

            try
            {
                do
                {
                    newAccountNumber = "VIN-" + rnd.Next(1, 99999999);
                    account = _accountRepository.FindByNumber(newAccountNumber);
                }
                while (account != null);


                Account newAccount = new Account
                {
                    Number = newAccountNumber,
                    CreationDate = DateTime.Now,
                    Balance = 0.0,
                    ClientId = clientId
                };

                _accountRepository.Save(newAccount);

                AccountDTO accountDTO = new AccountDTO
                {
                    Id = newAccount.Id,
                    Number = newAccount.Number,
                    CreationDate = newAccount.CreationDate,
                    Balance = newAccount.Balance,
                };
                return accountDTO;

            }
            catch
            {
                return null;
            }
        }
        [HttpGet("{id}")]

        public IActionResult Get(long id)

        {

            try

            {

                var account = _accountRepository.FindById(id);

                if (account == null)

                {

                    return Forbid();

                }



                var accountDTO = new AccountDTO

                {

                    Id = account.Id,

                    Number = account.Number,

                    CreationDate = account.CreationDate,

                    Balance = account.Balance,

                    Transactions = account.Transactions.Select(tr => new TransactionDTO

                    {

                        Id = tr.Id,

                        Type = tr.Type,

                        Amount = tr.Amount,

                        Description = tr.Description,

                        Date = tr.Date,

                    }).ToList()

                };



                return Ok(accountDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }


    }
}
