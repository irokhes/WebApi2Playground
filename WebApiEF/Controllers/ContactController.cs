using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebApiEF.Models;

namespace WebApiEF.Controllers
{
    public class ContactController : ApiController
    {
        private readonly EFContext _dbContext = new EFContext();

        

        // GET api/contact
        public IEnumerable<Contact> Get()
        {
            return _dbContext.Contacts.AsEnumerable();
        }

        // GET api/contact/5
        public Contact Get(int id)
        {
            var contact =_dbContext.Contacts.Find(id);
            if (contact == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            return contact;
        }

        // POST api/contact
        public HttpResponseMessage Post(Contact contact)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Contacts.Add(contact);
                _dbContext.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, contact);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new {id = contact.Id}));
                return response;
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        // PUT api/contact/5
        public HttpResponseMessage Put(int id, Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            if (id != contact.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            _dbContext.Entry(contact).State = EntityState.Modified;
            
            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK); 
        }

        // DELETE api/contact/5
        public HttpResponseMessage Delete(int id)
        {
            Contact Contact = _dbContext.Contacts.Find(id);
            if (Contact == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            _dbContext.Contacts.Remove(Contact);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, Contact); 
        }
    }
}
