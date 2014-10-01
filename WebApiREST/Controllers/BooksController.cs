using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using Microsoft.Ajax.Utilities;
using WebApiREST.DTOs;
using WebApiREST.Models;

namespace WebApiREST.Controllers
{
    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        private EFContext db = new EFContext();

        private static readonly Expression<Func<Book, BookDto>> AsBookDto = x => new BookDto()
        {
            Author = x.Author.Name,
            Genre = x.Genre,
            Title = x.Title
        };

        // GET: api/Books
        [Route("")]
        public IQueryable<BookDto> GetBooks()
        {
            return db.Books.Include(b => b.Author).Select(AsBookDto);
        }

        [Route("{id:int}")]
        // GET: api/Books/5
        [ResponseType(typeof(BookDto))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            BookDto book = await db.Books.Include(x => x.Author).Where(x => x.AuthorId == id).Select(AsBookDto).FirstOrDefaultAsync();
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [Route("{id:int}/details")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> GetBookDetails(int id)
        {
            var book = await db.Books.Include(x => x.Author).Where(x => x.BookId == id).Select(
                b =>
                     new BookDetailDto
                              {
                                  Title = b.Title,
                                  Genre = b.Genre,
                                  PublishDate = b.PublishDate,
                                  Price = b.Price,
                                  Description = b.Description,
                                  Author = b.Author.Name
                              }).FirstOrDefaultAsync();
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);

        }

        [Route("Genre")]
        public IQueryable<BookDto> GetBooksByGenre(string genre)
        {
            return db.Books.Include(x => x.Author).Where(x => x.Genre == genre).Select(AsBookDto);

        }

        [Route("~api/authors/{authorId}/books")]
        public IQueryable<BookDto> GetBooksByAuthor(int authorId)
        {
            return db.Books.Include(x => x.Author).Where(x => x.AuthorId == authorId).Select(AsBookDto);
        }

        [Route("date/{pubdate:datetime:regex(\\d{4}-\\d{2}-\\d{2})}")]
        [Route("date/{*pubdate:datetime:regex(\\d{4}/\\d{2}/\\d{2})}")]  // new
        public IQueryable<BookDto> GetBooks(DateTime pubdate)
        {
            return db.Books.Include(x => x.Author).Where(x => x.PublishDate == pubdate).Select(AsBookDto);
        }

        // PUT: api/Books/
        public async Task<IHttpActionResult> PutBook(int id, Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.BookId)
            {
                return BadRequest();
            }

            db.Entry(book).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Books
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> PostBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Books.Add(book);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = book.BookId }, book);
        }

        // DELETE: api/Books/5
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> DeleteBook(int id)
        {
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            db.Books.Remove(book);
            await db.SaveChangesAsync();

            return Ok(book);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.BookId == id) > 0;
        }
    }
}