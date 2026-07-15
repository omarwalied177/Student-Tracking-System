using StudentTrackingSystem.BLL.Interfaces;
using StudentTrackingSystem.DAL.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudentTrackingSystem.DAL.Models;

namespace StudentTrackingSystem.BLL.Repositories
{
    public class TeatcherRepository : ITeatcherRepository
    {
        private readonly AppDbContext _context;

        public TeatcherRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Teatcher>> GetAllAsync()
        {
            return await _context.Teatchers.ToListAsync();
        }

        public async Task<Teatcher?> GetAsync(int id)
        {
            return await _context.Teatchers.FindAsync(id);
        }

        public async Task AddAsync(Teatcher entity)
        {
            await _context.Teatchers.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Update(Teatcher entity)
        {
            _context.Teatchers.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(Teatcher entity)
        {
            _context.Teatchers.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<Teatcher>> GetAllWithSubjectAsync()
        {
            return await _context.Teatchers
                                 .Include(t => t.Subject)
                                 .ToListAsync();
        }
        public async Task<Teatcher?> GetWithSubjectAsync(int id)
        {
            return await _context.Teatchers
                                 .Include(t => t.Subject)
                                 .FirstOrDefaultAsync(t => t.Id == id);
        }


    }
}
