using Microsoft.EntityFrameworkCore;
using StudentTrackingSystem.DAL.Data.Contexts;
using StudentTrackingSystem.DAL.Models;
using StudentTrackingSystem.BLL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentTrackingSystem.BLL.Repositories
{
    public class GradeRepository : IGradeRepository
    {
        private readonly AppDbContext _context;

        public GradeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Grade>> GetAllAsync()
        {
            return await _context.Grades.ToListAsync();
        }

        public async Task<Grade> GetAsync(int id)
        {
            return await _context.Grades
                .FirstOrDefaultAsync(g => g.Id == id)
                ?? throw new InvalidOperationException($"Grade with id {id} not found.");

        }

        public async Task AddAsync(Grade grade)
        {
            await _context.Grades.AddAsync(grade);
        }

        public void Update(Grade grade)
        {
            _context.Grades.Update(grade);
        }

        public void Delete(Grade grade)
        {
            _context.Grades.Remove(grade);
        }
    }
}
