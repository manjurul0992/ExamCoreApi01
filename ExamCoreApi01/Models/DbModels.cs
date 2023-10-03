using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;

namespace ExamCoreApi01.Models
{
    public class Skill
    {
        public Skill()
        {
            this.CandidateSkills = new List<CandidateSkill>();
        }
        public int SkillId { get; set; }
        public string? SkillName { get; set; }
        public virtual ICollection<CandidateSkill> CandidateSkills { get; set; }
    }
    public class Candidate
    {
        public Candidate()
        {
            this.CandidateSkills = new List<CandidateSkill>();
        }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = default!;
        [Column(TypeName = "date")]
        public DateTime BirthDate { get; set; }
        public string PhoneNo { get; set; } = default!;
        public string Picture { get; set; } = default!;
        public bool Fresher { get; set; }
       
        public virtual ICollection<CandidateSkill> CandidateSkills { get; set; }

    }
    public class CandidateSkill
    {
        public int CandidateSkillId { get; set; }
        [ForeignKey("Candidate")]
        public int CandidateId { get; set; }
        [ForeignKey("Skill")]
        public int SkillId { get; set; }
        //if
        //public string? SkillName { get; set; }
       
        public virtual Candidate Candidate { get; set; }
        public virtual Skill Skill { get; set; }
    }
    public class CandidateDbContext : DbContext
    {
        public CandidateDbContext(DbContextOptions<CandidateDbContext> options) : base(options) { }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<CandidateSkill> CandidateSkills { get; set; }
        public virtual DbSet<TblUser> TblUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblUser>().HasData
            (
                new TblUser { ID = 1, FullName = "manjurul",Email="manjurulislam327@gmail.com",Password="123456" }
            );
            modelBuilder.Entity<Skill>().HasData
            (
                new Skill { SkillId = 1, SkillName = "C#" },
                new Skill { SkillId = 2, SkillName = "SQL" },
                new Skill { SkillId = 3, SkillName = "HTML" },
                new Skill { SkillId = 4, SkillName = "Java" },
                new Skill { SkillId = 5, SkillName = "PHP" }
            );

            base.OnModelCreating(modelBuilder);
        }
        internal object Find(int CandidateId)
        {
            throw new NotImplementedException();
        }
    }

    //For Auth
    public class UserModel
    {
        public int ID { get; set; }
        public string? FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string AccessToken { get; set; }

    }

    public partial class TblUser
    {
        public int ID { get; set; }

        public string? FullName { get; set; }

        public string Email { get; set; } = null!;

        public string? Password { get; set; }


    }

}
