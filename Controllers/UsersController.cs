using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.Services;
using MyApi.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace MyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {

        private IUsersService UsersService;
        // readonly IBooksService ifUserDeleted;
        public UsersController(IUsersService UsersService)
        {
            this.UsersService = UsersService;
        }

        [HttpGet()]
        [Authorize(Policy = "Admin")]
        public ActionResult<List<Users>> GetAll() =>
            UsersService.GetAll();

        [HttpGet("{id}")]
        [Authorize(Policy = "User")]
        public ActionResult<Users> Get(int id)
        {
            var user = UsersService.Get(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        //מוסיף משתמש חדש
        public ActionResult Create(Users newUser)
        {
            UsersService.Add(newUser);
            return CreatedAtAction(nameof(Create), new { id = newUser.Id }, newUser);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "User")]
        //עדכון משתמש
        public ActionResult Update(int id, Users newUser)
        {
            if (id != newUser.Id)
                return BadRequest();

            var existingUser = UsersService.Get(id);
            if (existingUser is null)
                return NotFound();

            UsersService.Update(newUser);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        //מחיקת משתמש
        public ActionResult Delete(int id)
        {
            var user = UsersService.Get(id);
            if (user is null)
                return NotFound();

            UsersService.Delete(id);

            //ifUserDeleted.DeleteAllBooks(user.Id);        
            return Content(UsersService.Count.ToString());
        }

        [HttpPost]
        [Route("/login")]
        public ActionResult<objectToReturn> Login([FromBody] Users user)
        {

            int UserExistID = UsersService.ExistUser(user.Name, user.password);
            if (UserExistID == -1)
            {
                return Unauthorized();
            }
            Console.WriteLine(UserExistID);

            var claims = new List<Claim> { };
            if (user.password == "shira2396" && user.Name == "Shira")
                claims.Add(new Claim("type", "Admin"));
            else
                claims.Add(new Claim("type", "User"));

            claims.Add(new Claim("id", UserExistID.ToString()));

            var token = myBooksTokenService.GetToken(claims);
            return new OkObjectResult(new { id = UserExistID, token = myBooksTokenService.WriteToken(token) });

        }

        public class objectToReturn
        {
            public int Id { get; set; }
            public string token { get; set; }
        }

    }
}

