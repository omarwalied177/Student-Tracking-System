using Microsoft.EntityFrameworkCore;
using StudentTrackingSystem.BLL.Interfaces;
using StudentTrackingSystem.DAL.Data.Contexts;
using StudentTrackingSystem.DAL.Models;

public class SubjectRepository : ISubjectRepository
{
    private readonly AppDbContext _context;

    public SubjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Subject>> GetAllAsync()
    {
        return await _context.Subjects.ToListAsync();
    }

    public async Task<Subject?> GetAsync(int id)
    {
        return await _context.Subjects
            .Include(s => s.Teatchers)
            .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new InvalidOperationException($"Subject with id {id} not found.");
   }

    public async Task AddAsync(Subject subject)
    {
        await _context.Subjects.AddAsync(subject);
        // لا تعمل SaveChanges هنا
    }

    public void Update(Subject subject)
    {
        _context.Subjects.Update(subject);
        // لا تعمل SaveChanges هنا
    }

    public void Delete(Subject subject)
    {
        _context.Subjects.Remove(subject);
        // لا تعمل SaveChanges هنا
    }
}
