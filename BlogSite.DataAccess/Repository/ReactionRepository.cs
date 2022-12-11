using BlogSite.DataAccess.Repository.IRepository;
using BlogSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSite.DataAccess.Repository
{
    public class ReactionRepository : Repository<Reaction>, IReactionRepository
    {
        private ApplicationDbContext _db;

        public ReactionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Reaction obj)
        {
            var objFromDb = _db.Reactions.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Action = obj.Action;
            }
        }
    }
}
