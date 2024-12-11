using MyApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyApi.Interfaces
{
    public interface IUsersService
    {
        List<Users> GetAll();

        Users Get(int id);

        int Add(Users user);

        void Delete(int id);

        void Update(Users user);

        int Count { get;}

        int ExistUser(string name, string password);
    }
}