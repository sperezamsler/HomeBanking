using HomeBanking.Models.DTO;
using HomeBanking.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using HomeBanking.Repositories;

namespace HomeBanking.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        ICardsRepository _cardRepository;

        public CardsController(ICardsRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        [HttpPost]
        public CardDTO Post(string cardHolder, long clientID, Card card)
        {
            Random rnd = new Random();
            string newCardNumber = String.Empty;
            Card cardAux;
            int newCardExpirationYears = 0;

            try
            {
                if (String.IsNullOrEmpty(card.Type) ||
                    String.IsNullOrEmpty(card.Color) ||
                    !Card.IsCardType(card.Type) ||
                    !Card.IsCardColor(card.Color) ||
                    String.IsNullOrEmpty(cardHolder) ||
                    clientID <= 0)
                {
                    return null;
                }

                if (card.Type.ToUpper() == "DEBIT")
                {
                    newCardExpirationYears = 5;
                }
                else if (card.Type.ToUpper() == "CREDIT")
                {
                    newCardExpirationYears = 4;
                }
                do
                {
                    newCardNumber = $"{rnd.Next(1111, 9999)}-{rnd.Next(1111, 9999)}-{rnd.Next(1111, 9999)}-{rnd.Next(1111, 9999)}";
                    cardAux = _cardRepository.FindByNumber(newCardNumber);
                }
                while (cardAux != null);


                Card newCard = new Card()
                {
                    CardHolder = cardHolder,
                    Type = card.Type,
                    Color = card.Color,
                    Number = newCardNumber,
                    Cvv = rnd.Next(111, 999),
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(newCardExpirationYears),
                    ClientId = clientID,
                };

                _cardRepository.Save(newCard);

                CardDTO newCardDTO = new CardDTO()
                {
                    Id = newCard.Id,
                    CardHolder = newCard.CardHolder,
                    Color = newCard.Color,
                    Cvv = newCard.Cvv,
                    FromDate = newCard.FromDate,
                    ThruDate = newCard.ThruDate,
                    Number = newCard.Number,
                    Type = newCard.Type,
                };

                return newCardDTO;
            }
            catch
            {
                return null;
            }
        }

    }
}
