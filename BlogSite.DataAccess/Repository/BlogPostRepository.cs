using BlogSite.DataAccess.Repository.IRepository;
using BlogSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSite.DataAccess.Repository
{
    public class BlogPostRepository : Repository<BlogPost>, IBlogPostRepository
    {
        private ApplicationDbContext _db;

        public BlogPostRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(BlogPost obj)
        {
            var objFromDb = _db.BlogPosts.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.Text = obj.Text;
                objFromDb.CategoryId = obj.CategoryId;
            }
        }
    }
}
