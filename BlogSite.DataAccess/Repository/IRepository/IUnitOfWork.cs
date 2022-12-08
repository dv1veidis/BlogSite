using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSite.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IBlogPostRepository BlogPost { get; }
        void Save();
    }
}
