using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentTrackingSystem.BLL.Interfaces;
using StudentTrackingSystem.DAL.Data.Contexts;
using StudentTrackingSystem.DAL.Models;
using System.Threading.Tasks;

namespace StudentTrackingSystem.Controllers
{
    [Authorize]
    public class SubjectController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _appDb;

        public SubjectController(IUnitOfWork unitOfWork, AppDbContext appDb)
        {
            _unitOfWork = unitOfWork;
            _appDb = appDb;
        }

        public async Task<IActionResult> Index()
        {
            var subjects = await _appDb.Subjects
                .Include(s => s.Grade)
                .ToListAsync();

            // Get grades for the filter dropdown
            var grades = await _unitOfWork.GradeRepository.GetAllAsync();
            ViewBag.Grades = grades.ToList();

            return View(subjects);
        }

        public async Task<IActionResult> Details(int id)
        {
            var subject = await _appDb.Subjects
                .Include(s => s.Grade)
                .Include(s => s.Teatchers)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var grades = await _unitOfWork.GradeRepository.GetAllAsync();
            ViewBag.Grades = new SelectList(grades, "Id", "Name");
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subject subject)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.SubjectRepository.AddAsync(subject);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }

            var grades = await _unitOfWork.GradeRepository.GetAllAsync();
            ViewBag.Grades = new SelectList(grades, "Id", "Name", subject.GradeId);
            return View(subject);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _unitOfWork.SubjectRepository.GetAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            var grades = await _unitOfWork.GradeRepository.GetAllAsync();
            ViewBag.Grades = new SelectList(grades, "Id", "Name", subject.GradeId);
            return View(subject);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Subject subject)
        {
            if (id != subject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.SubjectRepository.Update(subject);
                    await _unitOfWork.CompleteAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await SubjectExists(subject.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var grades = await _unitOfWork.GradeRepository.GetAllAsync();
            ViewBag.Grades = new SelectList(grades, "Id", "Name", subject.GradeId);
            return View(subject);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _appDb.Subjects
                .Include(s => s.Grade)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _unitOfWork.SubjectRepository.GetAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            _unitOfWork.SubjectRepository.Delete(subject);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SubjectExists(int id)
        {
            return await _unitOfWork.SubjectRepository.GetAsync(id) != null;
        }
    }
}