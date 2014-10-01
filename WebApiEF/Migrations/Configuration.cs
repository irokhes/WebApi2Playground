using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApiEF.Models;

namespace WebApiEF.Migrations
{
    
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DropCreateDatabaseAlways<EFContext>
    {
        public Configuration()
        {
            
        }

        protected override void Seed(EFContext context)
        {
            context.Contacts.AddOrUpdate(
              p => p.Name,
              new Contact { Name = "João Sousa", Address = "Street x", City = "Porto", Country = "Portugal" },
              new Contact { Name = "Steve Jon", Address = "Street y", City = "Porto", Country = "Portugal" },
              new Contact { Name = "Peter", Address = "Street z", City = "Porto", Country = "Portugal" }
            );
        }
    } 
}