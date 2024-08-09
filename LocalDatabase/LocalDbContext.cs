using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common;
namespace LocalDatabase
{
    public class LocalDbContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public string DbPath { get; }
        public LocalDbContext()
        {
            var dirPath = Assembly.GetExecutingAssembly().Location;
            dirPath = System.IO.Path.GetDirectoryName(dirPath);
            dirPath = Directory.GetParent(dirPath).FullName;
            string pathDatabase = System.IO.Path.Combine(dirPath, "OGLocalDb.sqlite");

            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = pathDatabase;// System.IO.Path.Join(path, "blogging.db");
            //this.Database.SetInitializer<LocalDbContext>(new CreateDatabaseIfNotExists<LocalDbContext>());
            try
            {
                this.Database.CreateIfNotExists();
                Database.Initialize(false);
            }
            catch (Exception ex)
            {
                var t = ex;
            }
        }
    }

}
