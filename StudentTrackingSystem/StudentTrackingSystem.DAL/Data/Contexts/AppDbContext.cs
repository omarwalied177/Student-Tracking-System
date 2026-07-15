using StudentTrackingSystem.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentTrackingSystem.DAL.Data.Models;
using StudentTrackingSystem.Models;
using Microsoft.AspNetCore.Identity; // تأكد إن ده موجود علشان يلاقي Student

namespace StudentTrackingSystem.DAL.Data.Contexts
{
    //public class AppDbContext : DbContext
    //{
    //    public DbSet<Grade> Grades { get; set; }

    //    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    //    {
    //    }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        base.OnModelCreating(modelBuilder);
    //        // Additional model configuration if needed  
    //    }
    //}
    public class AppDbContext : /*DbContext*/ IdentityDbContext<AppUser> // لو بتستخدم Identity
    {
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Subject> Subjects { get; set; } // ✅ ضيف ده
        public DbSet<Teatcher> Teatchers { get; set; } // لو بتستخدمه
        public DbSet<Student> Students { get; set; } // لو بتستخدمه
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<StudentAssignment> StudentAssignments { get; set; }
        
        // Quiz-related DbSets
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<StudentQuizSubmission> StudentQuizSubmissions { get; set; }
        public DbSet<StudentQuizAnswer> StudentQuizAnswers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // أي إعدادات إضافي

            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Grade)
                .WithMany()
                .HasForeignKey(s => s.GradeId)
                .OnDelete(DeleteBehavior.Cascade);  // يحدد الحذف التلقائي

            // Configure Assignment relationships
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Grade)
                .WithMany()
                .HasForeignKey(a => a.GradeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Subject)
                .WithMany()
                .HasForeignKey(a => a.SubjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Teacher)
                .WithMany()
                .HasForeignKey(a => a.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Teatcher>()
       .HasOne(t => t.Subject)
       .WithMany(s => s.Teatchers)
       .HasForeignKey(t => t.SubjectId)
       .OnDelete(DeleteBehavior.Restrict);

            // Quiz-related configurations
            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Grade)
                .WithMany()
                .HasForeignKey(q => q.GradeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Subject)
                .WithMany()
                .HasForeignKey(q => q.SubjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Teacher)
                .WithMany()
                .HasForeignKey(q => q.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<QuizQuestion>()
                .HasOne(q => q.Quiz)
                .WithMany(q => q.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentQuizSubmission>()
                .HasOne(s => s.Quiz)
                .WithMany()
                .HasForeignKey(s => s.QuizId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentQuizSubmission>()
                .HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentQuizAnswer>()
                .HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentQuizAnswer>()
                .HasOne(a => a.Submission)
                .WithMany(s => s.Answers)
                .HasForeignKey(a => a.SubmissionId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        public void DataSeed()
        {
            SeedUsers();
            SeedRoles();
            SaveChanges();
        }

        private void SeedRoles()
        {
            var adminRole = Roles.Find(Consts.Constants.AdminRoleId);
            var teatcherRoleId = Roles.Find(Consts.Constants.TeatcherRoleId);
            var studentRoleId = Roles.Find(Consts.Constants.StudentRoleId);

            if (adminRole == null)
            {
                Roles.Add(new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                    Id = Consts.Constants.AdminRoleId,
                });
            }

            if (teatcherRoleId == null)
            {
                Roles.Add(new IdentityRole
                {
                    Name = "Teatcher",
                    NormalizedName = "Teatcher".ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                    Id = Consts.Constants.TeatcherRoleId,
                });
            }
            
            if (studentRoleId == null)
            {
                Roles.Add(new IdentityRole
                {
                    Name = "Student",
                    NormalizedName = "Student".ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                    Id = Consts.Constants.StudentRoleId,
                });
            }
        }


        private void SeedUsers()
        {
            var userId = Guid.NewGuid().ToString("D"); 
            var user = Users.FirstOrDefault(u => u.Email == "admin@admin.eg");

            if (user != null)
                return;

            user = new AppUser
            {
                Id = userId,
                Email = "admin@admin.eg",
                UserName = "admin@admin.eg",
                NormalizedUserName = "admin@admin.eg".ToUpper(),
                NormalizedEmail = "admin@admin.eg".ToUpper(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                PhoneNumber = "01000000000",
                TwoFactorEnabled = false,
                PasswordHash = null,
                FirstName= "Ali",
                LastName= "Mohamed",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                IsAgree = true,
            };

            var hasher = new PasswordHasher<IdentityUser>();
            user.PasswordHash = hasher.HashPassword(user, "P@ssw0rd");

            Users.Add(user);


            this.UserRoles.Add(new IdentityUserRole<string>
            {
                RoleId = Consts.Constants.AdminRoleId,
                UserId = user.Id
            });
        }
    }
}


