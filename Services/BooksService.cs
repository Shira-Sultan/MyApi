using MyApi.Models;
using MyApi.Interfaces;
using System.Text.Json;
using MyApi.Services;


namespace MyApi.Services
{
    public class BooksService : IBooksService
    {
        List<Books> arrayBooks;

        private string fileName = "Books.json";
        public BooksService()
        {
            fileName = Path.Combine("data", "Books.json");

            using (var jsonFile = File.OpenText(fileName))
            {
                arrayBooks = JsonSerializer.Deserialize<List<Books>>(jsonFile.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }

        private void SaveToFile()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string jsonData = JsonSerializer.Serialize(arrayBooks, options);

            // שיניתי את הפונקציה כך שתשמור את הנתונים עם קידוד UTF-8
            File.WriteAllText(fileName, jsonData, System.Text.Encoding.UTF8);
        }

        public List<Books> GetAll(int userId)
        {
            return arrayBooks.FindAll(b => b.userId == userId);
        }

        public Books Get(int id) => arrayBooks.FirstOrDefault(p => p.Id == id);

        public int Add(Books newBook, int userId)
        {
            newBook.userId = userId;
            if (arrayBooks.Count == 0)
                newBook.Id = 1;
            else
                newBook.Id = arrayBooks.Max(b => b.Id) + 1;

            arrayBooks.Add(newBook);
            SaveToFile();
            return newBook.Id;
        }

        public bool Update(int id, Books book, int userId)
        {
            if (id != book.Id)
                return false;

            var existingBook = Get(id);
            if (existingBook == null)
                return false;

            book.userId = userId;

            var index = arrayBooks.IndexOf(existingBook);
            if (index == -1)
                return false;

            arrayBooks[index] = book;
            SaveToFile();

            return true;
        }

        public bool Delete(int id)
        {
            var existingBook = Get(id);
            if (existingBook == null)
                return false;

            var index = arrayBooks.IndexOf(existingBook);
            if (index == -1)
                return false;

            arrayBooks.RemoveAt(index);
            SaveToFile();
            return true;
        }

        public void DeleteAllBooks(int userId)
        {
            // arrayBooks.ForEach(book =>
            // {
            //     if (book.userId == userId)
            //         Delete(book.Id);
            // });

            arrayBooks.RemoveAll(book => book.userId == userId);
            SaveToFile();
        }

        public int Count { get => arrayBooks.Count(); }
    }
}

public static class BooksUtils
{
    public static void AddBook(this IServiceCollection services)
    {
        services.AddSingleton<IBooksService, BooksService>();
    }
}
