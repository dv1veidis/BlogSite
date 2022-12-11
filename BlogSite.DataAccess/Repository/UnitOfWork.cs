using BlogSite.DataAccess.Repository.IRepository;
using BlogSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSite.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            BlogPost = new BlogPostRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            Reaction = new ReactionRepository(_db);
        }
        public ICategoryRepository Category { get; private set; }
        public IBlogPostRepository BlogPost { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IReactionRepository Reaction { get; private set; }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
