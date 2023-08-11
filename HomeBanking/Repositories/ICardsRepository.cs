using HomeBanking.Models;
using System.Collections.Generic;

namespace HomeBanking.Repositories
{
    public interface ICardsRepository
    {
        void Save(Card card);
        Card FindByNumber(string number);

    }
}
