using System;
using System.Data.Common;

namespace HomeBanking.Models
{
    public class Card
    {
        public long Id  { get; set; }
        public string CardHolder { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Number { get; set; }
        public int Cvv { get; set; }
        public DateTime FromDate{ get; set; }
        public DateTime ThruDate{ get; set; }
        public Client Client { get; set; }
        public long ClientId { get; set; }

        public static bool IsCardType(string cardType)
        {
            return CardType.TryParse(cardType, out CardType result);
        }

        public static bool IsCardColor(string cardColor)
        {
            return CardColor.TryParse(cardColor, out CardColor result);
        }
    }
}