using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using E_Internship_Journal.Models;
/* Additional using statements besides the defaults (Start) */
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
/* Additional using statements besides the defaults (End) */

namespace E_Internship_Journal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<UserBatch> UserBatches { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Competency_Checked> Competency_Checkeds { get; set; }
        public DbSet<Competency> competencies { get; set; }
        public DbSet<Internship_Record> Internship_Records { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Month_Record> Month_Records { get; set; }
        public DbSet<Day_Record> Day_Records { get; set; }
        public DbSet<Task_Record> Tasks { get; set; }
        public DbSet<RegistrationPin> RegistrationPins { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Frequently used command in Command Prompt
            //To generate migration files:
            //>dnu restore
            //>dnx ef migrations add setup_db_file
            //Reference: http://www.bricelam.net/2014/09/14/migrations-on-k.html
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=E_Internship_Journal;Trusted_Connection=True;MultipleActiveResultSets=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //----------- Defining Course Entity - Start --------------
            //Make the CourseId a  Primary Key
            modelBuilder.Entity<Course>()
                .HasKey(courses => courses.CourseId)
                .HasName("PrimaryKey_CourseId");

            modelBuilder.Entity<Course>()
                .Property(courses => courses.CourseId)
                .HasColumnName("CourseId")
                .HasColumnType("int")
                .UseSqlServerIdentityColumn()
                .ValueGeneratedOnAdd()
                .IsRequired();

            modelBuilder.Entity<Course>()
                .Property(courses => courses.CourseCode)
                .HasColumnName("CourseCode")
                .HasColumnType("VARCHAR(10)")
                .IsRequired();

            modelBuilder.Entity<Course>()
                .Property(courses => courses.CourseName)
                .HasColumnName("CourseName")
                .HasColumnType("VARCHAR(100)")
                .IsRequired();


            modelBuilder.Entity<Course>()
                .HasIndex(courses => courses.CourseCode).IsUnique()
                .HasName("Course_CourseCode_UniqueConstraint");

            //----------- Defining Course Entity - End --------------


            //----------- Defining Batches Entity - Start --------------
            modelBuilder.Entity<Batch>()
                .HasKey(batches => batches.BatchId)
                .HasName("PrimaryKey_BatchId");

            modelBuilder.Entity<Batch>()
                .Property(batches => batches.BatchId)
                .HasColumnName("BatchId")
                .HasColumnType("int")
                .UseSqlServerIdentityColumn()
                .ValueGeneratedOnAdd()
                .IsRequired();

            modelBuilder.Entity<Batch>()
                .Property(batches => batches.BatchName)
                .HasColumnName("BatchName")
                .HasColumnType("VARCHAR(20)")
                .IsRequired();

            modelBuilder.Entity<Batch>()
                .Property(batches => batches.StartDate)
                .IsRequired();

            modelBuilder.Entity<Batch>()
                .Property(batches => batches.EndDate)
                .IsRequired();

            modelBuilder.Entity<Batch>()
                .Property(batches => batches.CourseId)
                .HasColumnName("CourseId")
                .HasColumnType("int")
                .IsRequired();

            modelBuilder.Entity<Batch>()
                .HasIndex(batches => batches.BatchName).IsUnique()
                .HasName("Batch_BatchName_UniqueConstraint");

            //----------- Defining Batches Entity - End --------------

            //----------- Defining UserBatches Entity - Start --------------

            modelBuilder.Entity<UserBatch>()
                .HasKey(userBatches => userBatches.UserBatchId)
                .HasName("PrimaryKey_UserBatchId");

            modelBuilder.Entity<UserBatch>()
                .Property(userBatches => userBatches.UserBatchId)
                .HasColumnName("UserBatchId")
                .HasColumnType("int")
                .UseSqlServerIdentityColumn()
                .ValueGeneratedOnAdd()
                .IsRequired();

            modelBuilder.Entity<UserBatch>()
                .Property(userBatches => userBatches.BatchId)
                .HasColumnName("BatchId")
                .HasColumnType("int")
                .IsRequired();

            modelBuilder.Entity<UserBatch>()
                .HasOne(userBatches => userBatches.User)
                .WithMany()
                .HasForeignKey(userBatches => userBatches.UserId)
                .HasPrincipalKey(applicationUserClass => applicationUserClass.Id)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<UserBatch>()
                .Property(userBatches => userBatches.InternshipRecordId)
                .HasColumnName("InternshipRecordId")
                .HasColumnType("int")
                .IsRequired();
            //----------- Defining UserBatches Entity - End --------------
            //----------- Defining Company Entity - Start --------------
            modelBuilder.Entity<Company>()
                .HasKey(companies => companies.CompanyId)
                .HasName("PrimaryKey_CompanyId");

            modelBuilder.Entity<Company>()
                .Property(companies => companies.CompanyId)
                .HasColumnName("CompanyId")
                .HasColumnType("int")
                .UseSqlServerIdentityColumn()
                .ValueGeneratedOnAdd()
                .IsRequired();
            modelBuilder.Entity<Company>()
                .Property(companies => companies.CompanyName)
                .HasColumnName("CompanyName")
                .HasColumnType("VARCHAR(100)")
                .IsRequired();
            modelBuilder.Entity<Company>()
                .Property(companies => companies.CompanyAddress)
                .HasColumnName("CompanyAddress")
                .HasColumnType("VARCHAR(MAX)")
                .IsRequired();
            //----------- Defining Company Entity - End --------------

            //----------- Defining Project Entity - Start --------------
            modelBuilder.Entity<Project>()
                .HasKey(projects => projects.ProjectId)
                .HasName("PrimaryKey_CompanyId");

            modelBuilder.Entity<Project>()
                .Property(projects => projects.ProjectId)
                .HasColumnName("ProjectId")
                .HasColumnType("int")
                .UseSqlServerIdentityColumn()
                .ValueGeneratedOnAdd()
                .IsRequired();
            modelBuilder.Entity<Project>()
                .Property(projects => projects.ProjectName)
                .HasColumnName("ProjectName")
                .HasColumnType("VARCHAR(100)")
                .IsRequired();
            modelBuilder.Entity<Project>()
                .Property(projects => projects.SupervisorId)
                .HasColumnName("SupervisorId")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Project>()
                .Property(projects => projects.CompanyID)
                .HasColumnName("CompanyID")
                .HasColumnType("int")
                .IsRequired();




            //----------- Defining Project Entity - End --------------




            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
