using StudentTrackingSystem.BLL.Interfaces;
using StudentTrackingSystem.DAL.Data.Contexts;
using StudentTrackingSystem.DAL.Models;
using StudentTrackingSystem.Models;

namespace StudentTrackingSystem.BLL.Repositories
{
    public class ParentRepository : GenaricRepository<Parent>, IParentRepository
    {
        public ParentRepository(AppDbContext context) : base(context)
        {

        }

        // لو عايز تضيف حاجات خاصة بالطالب، أضفها هنا
    }
}
