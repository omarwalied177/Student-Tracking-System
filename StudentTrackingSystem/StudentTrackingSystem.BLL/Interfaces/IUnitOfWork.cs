using StudentTrackingSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentTrackingSystem.BLL.Interfaces
{
    public interface IUnitOfWork
    {
        IStudentRepository StudentRepository { get; }
        ITeatcherRepository TeatcherRepository { get; }
        ISubjectRepository SubjectRepository { get; }
        IGradeRepository GradeRepository { get; }  // إضافة GradeRepository
        IParentRepository ParentRepository { get; }
         
        Task<int> CompleteAsync();
    }



}
