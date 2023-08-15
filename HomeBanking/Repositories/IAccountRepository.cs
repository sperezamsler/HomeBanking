using HomeBanking.Models;
using System.Collections.Generic;

namespace HomeBanking.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();
        Account FindById(long id);
        void Save(Account account);
        IEnumerable<Account> GetAccountsByClient(long clientId);
        Account FindByNumber(string Number);



    }
}
