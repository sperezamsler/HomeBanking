using HomeBanking.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HomeBanking.Repositories
{
    public class CardsRepository : RepositoryBase<Card>, ICardsRepository
    {
        public CardsRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Card FindByNumber(string number)
        {
            return FindByCondition(card => card.Number == number).FirstOrDefault();
        }

        public void Save(Card card)

        {

            Create(card);

            SaveChanges();

        }
    }
}
