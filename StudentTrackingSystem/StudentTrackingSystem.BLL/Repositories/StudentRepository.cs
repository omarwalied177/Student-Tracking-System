using StudentTrackingSystem.BLL.Interfaces;
using StudentTrackingSystem.DAL.Data.Contexts;
using StudentTrackingSystem.Models;

namespace StudentTrackingSystem.BLL.Repositories
{
    public class StudentRepository : GenaricRepository<Student>, IStudentRepository
    {
        public StudentRepository(AppDbContext context) : base(context)
        {

        }

        // لو عايز تضيف حاجات خاصة بالطالب، أضفها هنا
    }
}
