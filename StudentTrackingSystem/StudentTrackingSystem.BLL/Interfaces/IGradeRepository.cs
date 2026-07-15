using StudentTrackingSystem.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentTrackingSystem.BLL.Interfaces
{
    public interface IGradeRepository
    {
        Task<IEnumerable<Grade>> GetAllAsync();
        Task<Grade> GetAsync(int id);
        Task AddAsync(Grade grade);
        void Update(Grade grade);
        void Delete(Grade grade);
    }
}
