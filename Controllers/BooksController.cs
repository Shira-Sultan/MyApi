using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.Services;
using MyApi.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {

        private IBooksService BooksService;
        public BooksController(IBooksService BooksService)
        {
            this.BooksService = BooksService;
        }

        [HttpGet]
        [Authorize(Policy = "User")]
        public ActionResult<List<Books>> GetAll()
        {
            return BooksService.GetAll(int.Parse(User.FindFirst("id")?.Value!));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "User")]
        public ActionResult<Books> Get(int id)
        {
            var book = BooksService.Get(id);
            if (book == null)
            {
                return NotFound();
            }
            return book;
        }

        [HttpPost]
        [Authorize(Policy = "User")]
        //מוסיפה פריט חדש לרשימה
        public ActionResult Create(Books newBook)
        {
            var newId = BooksService.Add(newBook,int.Parse(User.FindFirst("id")?.Value!));

            return CreatedAtAction("Create",
                new { id = newId}, BooksService.Get(newId));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "User")]
        //מעדכנת ערך של פריט ברשימה
        public ActionResult Update(int id, Books newBook)
        {
            var result = BooksService.Update(id, newBook,int.Parse(User.FindFirst("id")?.Value!));
            if (!result)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "User")]
        //מוחקת פריט מהרשימה
        public ActionResult Delete(int id)
        {
            bool result = BooksService.Delete(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}

