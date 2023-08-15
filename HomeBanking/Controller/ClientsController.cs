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
    public class ClientsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private AccountsController _accountsController;
        private CardsController _cardsController;
        public ClientsController(IClientRepository clientRepository, AccountsController accountsController, CardsController cardsController)
        {
            _clientRepository = clientRepository;
            _accountsController = accountsController;
            _cardsController = cardsController;
        }

        [HttpGet]
        public IActionResult Get()

        {

            try

            {

                var clients = _clientRepository.GetAllClients();



                var clientsDTO = new List<ClientDTO>();



                foreach (Client client in clients)

                {

                    var newClientDTO = new ClientDTO

                    {

                        Id = client.Id,

                        Email = client.Email,

                        FirstName = client.FirstName,

                        LastName = client.LastName,

                        Accounts = client.Accounts.Select(ac => new AccountDTO

                        {

                            Id = ac.Id,

                            Balance = ac.Balance,

                            CreationDate = ac.CreationDate,

                            Number = ac.Number

                        }).ToList(),
                        Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                        {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                        }).ToList(),

                         Cards = client.Cards.Select(c => new CardDTO
                         {
                             Id = c.Id,
                             CardHolder = c.CardHolder,
                             Color = c.Color,
                             Cvv = c.Cvv,
                             FromDate = c.FromDate,
                             Number = c.Number,
                             ThruDate = c.ThruDate,
                             Type = c.Type
                         }).ToList()

                    };



                    clientsDTO.Add(newClientDTO);

                }





                return Ok(clientsDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }

        [HttpGet("current/accounts")]
        public IActionResult GetAccounts()
        {
            IEnumerable<AccountDTO> accounts;

            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : String.Empty;
                if (email == String.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                accounts = client.Accounts.Select(ac => new AccountDTO
                {
                    Id = ac.Id,
                    Balance = ac.Balance,
                    CreationDate = ac.CreationDate,
                    Number = ac.Number,
                }).ToList();


                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [HttpGet("{id}")]

        public IActionResult Get(long id)

        {

            try

            {

                var client = _clientRepository.FindById(id);

                if (client == null)

                {

                    return Forbid();

                }



                var clientDTO = new ClientDTO

                {

                    Id = client.Id,

                    Email = client.Email,

                    FirstName = client.FirstName,

                    LastName = client.LastName,

                    Accounts = client.Accounts.Select(ac => new AccountDTO

                    {

                        Id = ac.Id,

                        Balance = ac.Balance,

                        CreationDate = ac.CreationDate,

                        Number = ac.Number

                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                    Id = cl.Id,
                    LoanId = cl.LoanId,
                    Name = cl.Loan.Name,
                    Amount = cl.Amount,
                    Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                    Id = c.Id,
                    CardHolder = c.CardHolder,
                    Color = c.Color,
                    Cvv = c.Cvv,
                    FromDate = c.FromDate,
                    Number = c.Number,
                    ThruDate = c.ThruDate,
                    Type = c.Type
                    }).ToList()
                };



                return Ok(clientDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }

        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color,
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type
                    }).ToList()
                };

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")]
        public IActionResult CreateAccount()
        {
            // Obtener el cliente autenticado
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                // Verificar si el cliente ya tiene 3 cuentas registradas
                if (client.Accounts.Count >= 3)
            {
                return StatusCode(403, "Prohibido: El cliente ya tiene 3 cuentas registradas.");
            }

            // Generar el número de cuenta aleatorio
                var random = new Random();
                string accountNumber;
                //validamos si el numero de cuenta ya existe
                do
                {
                accountNumber = $"VIN-{random.Next(4, 99999999)}";
                }
                while (client.Accounts.Any(a => a.Number == accountNumber));
                
                // Crear la nueva cuenta
                var newAccount = new Account
            {
                Number = accountNumber,
                Balance = 0,
                CreationDate = DateTime.Now,
                ClientId = client.Id // Asociar la cuenta con el cliente
            };

            // Guardar la cuenta en el repositorio
            _accountsController.Post(client.Id);

            return StatusCode(201, "Cuenta creada exitosamente.");
        }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Client client)
        {
            try
            {
                //validamos datos antes
                if (String.IsNullOrEmpty(client.Email) || String.IsNullOrEmpty(client.Password) || String.IsNullOrEmpty(client.FirstName) || String.IsNullOrEmpty(client.LastName))
                    return StatusCode(403, "datos inválidos");

                //buscamos si ya existe el usuario
                Client user = _clientRepository.FindByEmail(client.Email);

                if (user != null)
                {
                    return StatusCode(403, "Email está en uso");
                }

                Client newClient = new Client
                {
                    Email = client.Email,
                    Password = client.Password,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                };

                _clientRepository.Save(newClient);
                AccountDTO newAccount = _accountsController.Post(newClient.Id);
                if (newAccount == null)
                {
                    return StatusCode(500, "No se creo la cuenta correctamente");
                }
                return Created("", newClient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("current/cards")]
        public IActionResult PostCard([FromBody] Card card)
        {
            string cardHolder = String.Empty;
            string newCardType = String.Empty;
            int cardsAmount = 0;

            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : String.Empty;
                if (email == String.Empty)
                {
                    return Forbid("Do not have authorization");
                }

                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                {
                    return Forbid("Not existing client");
                }

                //validate data
                if (String.IsNullOrEmpty(card.Type) ||
                    String.IsNullOrEmpty(card.Color) ||
                    !Card.IsCardType(card.Type) ||
                    !Card.IsCardColor(card.Color))
                {
                    return StatusCode(403, "Invalid Data");
                }

                foreach (Card cardAux in client.Cards)
                {
                    if (cardAux.Type == card.Type)
                    {
                        cardsAmount++;
                    }
                }

                if (cardsAmount >= 3)
                {
                    return Forbid($"Over max card type {card.Type} permitted");
                }

                cardHolder = $"{client.FirstName} {client.LastName}";

                CardDTO newCardDTO = _cardsController.Post(cardHolder, client.Id, card);
                if (newCardDTO == null)
                {
                    return StatusCode(403, "Card not created at cardsController");
                }

                return Created("", newCardDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

