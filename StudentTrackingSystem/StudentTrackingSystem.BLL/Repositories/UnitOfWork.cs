using StudentTrackingSystem.BLL.Interfaces;
using StudentTrackingSystem.DAL.Data.Contexts;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
     

    public IStudentRepository StudentRepository { get; private set; }
    public ITeatcherRepository TeatcherRepository { get; private set; }
    public ISubjectRepository SubjectRepository { get; private set; }
    public IGradeRepository GradeRepository { get; private set; }  // إضافة GradeRepository
    public IParentRepository ParentRepository { get; private set; }

    public UnitOfWork(AppDbContext context, IStudentRepository studentRepository, ITeatcherRepository teatcherRepository, ISubjectRepository subjectRepository, IGradeRepository gradeRepository, IParentRepository parentRepository )
    {
        _context = context;
        StudentRepository = studentRepository;
        TeatcherRepository = teatcherRepository;
        SubjectRepository = subjectRepository;
        GradeRepository = gradeRepository;  // تهيئة GradeRepository
        ParentRepository = parentRepository;
        
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }
}

