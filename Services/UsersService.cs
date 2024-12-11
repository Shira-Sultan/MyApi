using MyApi.Models;
using MyApi.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MyApi.Services;


namespace MyApi.Services
{
    public class UsersService : IUsersService
    {
        List<Users> arrayUsers;
        private string fileName = "users.json";
        public UsersService()
        {
            fileName = Path.Combine("data", "users.json");

            using (var jsonFile = File.OpenText(fileName))
            {
                #pragma warning disable CS8601 // Possible null reference assignment.
                arrayUsers = JsonSerializer.Deserialize<List<Users>>(jsonFile.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                #pragma warning restore CS8601 // Possible null reference assignment.
            }
        }

        private void SaveToFile()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string jsonData = JsonSerializer.Serialize(arrayUsers, options);

            // שיניתי את הפונקציה כך שתשמור את הנתונים עם קידוד UTF-8
            File.WriteAllText(fileName, jsonData, System.Text.Encoding.UTF8);
        }

        public List<Users> GetAll() => arrayUsers;

        public Users Get(int id) => arrayUsers.FirstOrDefault(u => u.Id == id);

        public int Add(Users user)
        {
            if (arrayUsers.Count == 0)
                user.Id = 1;
            else
                user.Id = arrayUsers.Max(u => u.Id) + 1;

            arrayUsers.Add(user);
            SaveToFile();
            return user.Id;
        }

        public void Update(Users user)
        {
            var index = arrayUsers.FindIndex(u => u.Id == user.Id);
            if (index == -1)
                return;
            arrayUsers[index] = user;
            SaveToFile();
        }

        public void Delete(int id)
        {
            var user = Get(id);
            if (user is null)
                return;
            arrayUsers.Remove(user);

            SaveToFile();
        }

        public int Count { get => arrayUsers.Count(); }

        public int ExistUser(string name, string password)
        {
            Users existUser = arrayUsers.FirstOrDefault(u => u.Name.Equals(name) && u.password.Equals(password));
            if (existUser != null)
                return existUser.Id;
            return -1;
        }
    }
}

public static class UsersUtils
{
    public static void AddUser(this IServiceCollection services)
    {
        services.AddSingleton<IUsersService, UsersService>();
    }
}