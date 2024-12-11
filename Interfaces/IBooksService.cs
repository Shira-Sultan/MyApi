using MyApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyApi.Interfaces
{
    public interface IBooksService
    {
        List<Books> GetAll(int userId);

        Books Get(int id);

        int Add(Books newBook, int userId);

        bool Delete(int id);

        void DeleteAllBooks(int userId);

        bool Update(int id, Books book, int userId);

        int Count { get;}
    }
}