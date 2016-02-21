using Microsoft.AspNet.Identity.EntityFramework;
using MvcCms.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MvcCms.Data
{
    public class CmsContext : IdentityDbContext<CmsUser>
    {
        public CmsContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>()
                .HasKey(e => e.Id)
                .Property(e => e.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Post>()
                .HasRequired(e => e.Author);
        }
    }
}