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
        public DbSet<Competency> Competencies { get; set; }
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

        public ApplicationDbContext()
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
            modelBuilder.Entity<ApplicationUser>()
     .Property(courses => courses.CourseId)
     .IsRequired(false);

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

            /* modelBuilder.Entity<UserBatch>()
                .Property(userBatches => userBatches.InternshipRecordId)
                .HasColumnName("InternshipRecordId")
                .HasColumnType("int")
                .IsRequired();*/
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
                .HasName("PrimaryKey_ProjectId");

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
                .Property(projects => projects.CompanyID)
                .HasColumnName("CompanyID")
                .HasColumnType("int")
                .IsRequired();

            modelBuilder.Entity<Project>()
                .HasOne(project => project.Supervisor)
                .WithMany()
                .HasForeignKey(project => project.SupervisorId)
                .HasPrincipalKey(applicationUserClass => applicationUserClass.Id)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();


            //----------- Defining Project Entity - End --------------

            //----------- Defining Competency Entity - Start --------------
            modelBuilder.Entity<Competency>()
                .HasKey(compenties => compenties.CompetencyId)
                .HasName("PrimaryKey_CompetencyId");

            modelBuilder.Entity<Competency>()
                .Property(compenties => compenties.CompetencyId)
                .HasColumnName("CompetencyId")
                .HasColumnType("int")
                .UseSqlServerIdentityColumn()
                .ValueGeneratedOnAdd()
                .IsRequired();
            modelBuilder.Entity<Competency>()
                .Property(compenties => compenties.TitleDescription)
                .HasColumnName("TitleDescription")
                .HasColumnType("VARCHAR(50)")
                .IsRequired();
            modelBuilder.Entity<Competency>()
                .Property(compenties => compenties.Description)
                .HasColumnName("Description")
                .HasColumnType("VARCHAR(MAX)")
                .IsRequired();
            modelBuilder.Entity<Competency>()
                .Property(compenties => compenties.CourseId)
                .HasColumnName("CourseId")
                .HasColumnType("int")
                .IsRequired();

            //----------- Defining Competency Entity - End --------------

            //----------- Defining Internship_Record Entity - Start --------------
            modelBuilder.Entity<Internship_Record>()
            .HasKey(Internship_Records => Internship_Records.InternshipRecordId)
            .HasName("PrimaryKey_InternshipRecordId");

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.InternshipRecordId)
            .HasColumnName("InternshipRecordId")
            .HasColumnType("int")
            .UseSqlServerIdentityColumn()
            .ValueGeneratedOnAdd()
            .IsRequired();

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.PresentationGrading)
            .HasColumnName("PresentationGrading")
            .HasColumnType("int")
            .IsRequired(false);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.ReflectionGrading)
            .HasColumnName("ReflectionGrading")
            .HasColumnType("int")
            .IsRequired(false);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.Comment)
            .HasColumnName("Comment")
            .HasColumnType("VARCHAR(MAX)")
            .HasDefaultValue("");

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.PosterUrl)
            .HasColumnName("Description")
            .HasColumnType("VARCHAR(MAX)")
            .HasDefaultValue("");

            //This is for Company Checklist when student first enter the company
            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.CompanyCheck1a)
            .HasColumnName("CompanyCheck1a")
            .HasColumnType("bit")
            .HasDefaultValue(0);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.CompanyCheck1b)
            .HasColumnName("CompanyCheck1b")
            .HasColumnType("bit")
            .HasDefaultValue(0);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.CompanyCheck2a)
            .HasColumnName("CompanyCheck2a")
            .HasColumnType("bit")
            .HasDefaultValue(0);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.CompanyCheck2b)
            .HasColumnName("CompanyCheck2b")
            .HasColumnType("bit")
            .HasDefaultValue(0);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.CompanyCheck2c)
            .HasColumnName("CompanyCheck2c")
            .HasColumnType("bit")
            .HasDefaultValue(0);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.CompanyCheck2d)
            .HasColumnName("CompanyCheck2d")
            .HasColumnType("bit")
            .HasDefaultValue(0);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.CompanyCheck2e)
            .HasColumnName("CompanyCheck2e")
            .HasColumnType("bit")
            .HasDefaultValue(0);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.CompanyCheck2f)
            .HasColumnName("CompanyCheck2f")
            .HasColumnType("bit")
            .HasDefaultValue(0);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.CompanyCheck3a)
            .HasColumnName("CompanyCheck3a")
            .HasColumnType("bit")
            .HasDefaultValue(0);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.CompanyCheck3b)
            .HasColumnName("CompanyCheck3b")
            .HasColumnType("bit")
            .HasDefaultValue(0);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.CompanyCheck3c)
            .HasColumnName("CompanyCheck3c")
            .HasColumnType("bit")
            .HasDefaultValue(0);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.FeedbackUseful)
            .HasColumnName("FeedbackUseful")
            .HasColumnType("bit")
            .HasDefaultValue(0); 

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.FeedbackImproved)
            .HasColumnName("FeedbackImproved")
            .HasColumnType("bit")
            .HasDefaultValue(0);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.FeedbackExperiences)
            .HasColumnName("FeedbackExperiences")
            .HasColumnType("bit")
            .HasDefaultValue(0);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.FeedbackRecommend)
            .HasColumnName("FeedbackRecommend")
            .HasColumnType("bit")
            .HasDefaultValue(0);

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.FeedbackEnjoy)
            .HasColumnName("FeedbackEnjoy")
            .HasColumnType("VARCHAR(100)")
            .HasDefaultValue("");

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.FeedbackLeastEnjoy)
            .HasColumnName("FeedbackLeastEnjoy")
            .HasColumnType("VARCHAR(100)")
            .HasDefaultValue("");

            /*  modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.LiaisonOfficeId)
            .HasColumnName("LiaisonOfficeId")
            .HasColumnType("int")
            .IsRequired();*/
            modelBuilder.Entity<Internship_Record>()
            .HasOne(userBatches => userBatches.LiaisonOfficer)
            .WithMany()
            .HasForeignKey(userBatches => userBatches.LiaisonOfficerId)
            .HasPrincipalKey(applicationUserClass => applicationUserClass.Id)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.UserBatchId)
            .HasColumnName("UserBatchId")
            .HasColumnType("int")
            .IsRequired();

            modelBuilder.Entity<Internship_Record>()
            .Property(Internship_Records => Internship_Records.ProjectId)
            .HasColumnName("ProjectId")
            .HasColumnType("int")
            .IsRequired();

            //----------- Defining Internship_Record Entity - End --------------


            //----------- Defining TouchPoint_Record Entity - Start --------------
            modelBuilder.Entity<TouchPoint_Record>()
            .HasKey(touchPoint_Records => touchPoint_Records.TouchPointId)
            .HasName("PrimaryKey_TouchPointId");

            modelBuilder.Entity<TouchPoint_Record>()
            .Property(touchPoint_Records => touchPoint_Records.TouchPointId)
            .HasColumnName("TouchPointId")
            .HasColumnType("int")
            .UseSqlServerIdentityColumn()
            .ValueGeneratedOnAdd()
            .IsRequired();
            modelBuilder.Entity<TouchPoint_Record>()
            .Property(touchPoint_Records => touchPoint_Records.Comments)
            .HasColumnName("Comments")
            .HasColumnType("VARCHAR(MAX)")
            .IsRequired();
            modelBuilder.Entity<TouchPoint_Record>()
            .Property(touchPoint_Records => touchPoint_Records.InternshipRecordId)
            .HasColumnName("InternshipRecordId")
            .HasColumnType("int")
            .IsRequired();

            //----------- Defining TouchPoint_Record Entity - End --------------

            //----------- Defining Month_Record Entity - Start --------------
            modelBuilder.Entity<Month_Record>()
            .HasKey(month_Records => month_Records.MonthId)
            .HasName("PrimaryKey_MonthId");

            modelBuilder.Entity<Month_Record>()
            .Property(month_Records => month_Records.MonthId)
            .HasColumnName("MonthId")
            .HasColumnType("int")
            .UseSqlServerIdentityColumn()
            .ValueGeneratedOnAdd()
            .IsRequired();

            modelBuilder.Entity<Month_Record>()
            .Property(month_Records => month_Records.Approved)
            .HasColumnName("Approved")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired(false);

            modelBuilder.Entity<Month_Record>()
            .Property(month_Records => month_Records.SoftSkillsCompetency)
            .HasColumnName("SoftSkillsCompetency")
            .HasColumnType("VARCHAR(MAX)")
            .IsRequired(false);

            modelBuilder.Entity<Month_Record>()
            .Property(month_Records => month_Records.TechnicalCompetency)
            .HasColumnName("TechnicalCompetency")
            .HasColumnType("VARCHAR(MAX)")
            .IsRequired(false);

            modelBuilder.Entity<Month_Record>()
            .Property(month_Records => month_Records.MentorSessionDateTimeStart)
            .HasColumnName("MentorSessionDateTimeStart")
            .IsRequired(false);

            modelBuilder.Entity<Month_Record>()
            .Property(month_Records => month_Records.MentorSessionDateTimeEnd)
            .HasColumnName("MentorSessionDateTimeEnd")
            .IsRequired(false);

            modelBuilder.Entity<Month_Record>()
            .Property(month_Records => month_Records.MentorSessionReflection)
            .HasColumnName("MentorSessionReflection")
            .HasColumnType("VARCHAR(MAX)")
            .IsRequired(false);

            modelBuilder.Entity<Month_Record>()
            .Property(month_Records => month_Records.CommunicationGrading)
            .HasColumnName("CommunicationGrading")
            .HasColumnType("int")
            .IsRequired(false);

            modelBuilder.Entity<Month_Record>()
            .Property(month_Records => month_Records.TechnicalGrading)
            .HasColumnName("TechnicalGrading")
            .HasColumnType("int")
            .IsRequired(false);

            modelBuilder.Entity<Month_Record>()
            .Property(month_Records => month_Records.IndependenceGrading)
            .HasColumnName("IndependenceGrading")
            .HasColumnType("int")
            .IsRequired(false);

            modelBuilder.Entity<Month_Record>()
            .Property(month_Records => month_Records.PerformanceGrading)
            .HasColumnName("PerformanceGrading")
            .HasColumnType("int")
            .IsRequired(false);

            modelBuilder.Entity<Month_Record>()
            .Property(month_Records => month_Records.OverallGrading)
            .HasColumnName("OverallGrading")
            .HasColumnType("int")
            .IsRequired(false);

            modelBuilder.Entity<Month_Record>()
            .Property(month_Records => month_Records.OverallFeedback)
            .HasColumnName("OverallFeedback")
            .HasColumnType("VARCHAR(MAX)")
            .IsRequired(false);

            modelBuilder.Entity<Month_Record>()
            .Property(month_Records => month_Records.InternshipRecordId)
            .HasColumnName("InternshipRecordId")
            .HasColumnType("int")
            .IsRequired();

            //----------- Defining Month_Record Entity - End --------------

            //----------- Defining Day_Record Entity - Start --------------
            modelBuilder.Entity<Day_Record>()
            .HasKey(day_Records => day_Records.DayId)
            .HasName("PrimaryKey_DayId");

            modelBuilder.Entity<Day_Record>()
            .Property(day_Records => day_Records.DayId)
            .HasColumnName("DayId")
            .HasColumnType("int")
            .UseSqlServerIdentityColumn()
            .ValueGeneratedOnAdd()
            .IsRequired();

            modelBuilder.Entity<Day_Record>()
            .Property(day_Records => day_Records.Date)
            .HasColumnName("Date")
            .IsRequired();

            modelBuilder.Entity<Day_Record>()
            .Property(day_Records => day_Records.MC)
            .HasColumnName("MC")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired(false);

            modelBuilder.Entity<Day_Record>()
            .Property(day_Records => day_Records.CompanyOff)
            .HasColumnName("CompanyOff")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired(false);

            modelBuilder.Entity<Day_Record>()
            .Property(day_Records => day_Records.ArrivalTime)
            .HasColumnName("ArrivalTime")
            .IsRequired();

            modelBuilder.Entity<Day_Record>()
            .Property(day_Records => day_Records.DepartureTime)
            .HasColumnName("DepartureTime")
            .IsRequired();

            modelBuilder.Entity<Day_Record>()
            .Property(day_Records => day_Records.WeekNo)
            .HasColumnName("WeekNo")
            .HasColumnType("int")
            .IsRequired();

            modelBuilder.Entity<Day_Record>()
            .Property(day_Records => day_Records.MonthRecordId)
            .HasColumnName("MonthRecordId")
            .HasColumnType("int")
            .IsRequired();


            //----------- Defining Day_Record Entity - End --------------

            //----------- Defining Competency_Checked Entity - Start --------------
            modelBuilder.Entity<Competency_Checked>()
            .HasKey(competency_Checkeds => competency_Checkeds.CompentencyCheckedId)
            .HasName("PrimaryKey_CompentencyCheckedId");

            modelBuilder.Entity<Competency_Checked>()
            .Property(competency_Checkeds => competency_Checkeds.CompentencyCheckedId)
            .HasColumnName("CompentencyCheckedId")
            .HasColumnType("int")
            .UseSqlServerIdentityColumn()
            .ValueGeneratedOnAdd()
            .IsRequired();


            modelBuilder.Entity<Competency_Checked>()
            .Property(competency_Checkeds => competency_Checkeds.MonthRecordId)
            .HasColumnName("MonthRecordId")
            .HasColumnType("int")
            .IsRequired();

            modelBuilder.Entity<Competency_Checked>()
            .Property(competency_Checkeds => competency_Checkeds.CompetencyId)
            .HasColumnName("CompetencyId")
            .HasColumnType("int")
            .IsRequired();


            //----------- Defining Competency_Checked Entity - End --------------

            //----------- Defining Task_Record Entity - Start --------------
            modelBuilder.Entity<Task_Record>()
            .HasKey(task_Records => task_Records.TaskRecordId)
            .HasName("PrimaryKey_TaskRecordId");

            modelBuilder.Entity<Task_Record>()
            .Property(task_Records => task_Records.TaskRecordId)
            .HasColumnName("TaskRecordId")
            .HasColumnType("int")
            .UseSqlServerIdentityColumn()
            .ValueGeneratedOnAdd()
            .IsRequired();


            modelBuilder.Entity<Task_Record>()
            .Property(task_Records => task_Records.Description)
            .HasColumnName("Description")
            .HasColumnType("VARCHAR(MAX)")
            .IsRequired();

            modelBuilder.Entity<Task_Record>()
            .Property(task_Records => task_Records.DayRecordId)
            .HasColumnName("DayRecordId")
            .HasColumnType("int")
            .IsRequired();


            //----------- Defining Task_Record Entity - End --------------

            //----------- Defining RegistrationPin Entity - Start --------------
            modelBuilder.Entity<RegistrationPin>()
            .HasKey(registrationPin => registrationPin.RegistrationPinId)
            .HasName("PrimaryKey_RegistrationPinId");

            modelBuilder.Entity<RegistrationPin>()
            .Property(registrationPin => registrationPin.RegistrationPinId)
            .HasColumnName("RegistrationPinId")
            .HasColumnType("VARCHAR(100)")
            .IsRequired();

            modelBuilder.Entity<RegistrationPin>()
                .HasIndex(registrationPin => registrationPin.RegistrationPinId)
                .IsUnique()
                .HasName("RegistrationPin_RegistrationPinId_UniqueConstraint");

            modelBuilder.Entity<RegistrationPin>()
                .Property(registrationPin => registrationPin.Valid)
                .HasColumnName("Vald")
                .HasColumnType("bit")
                .HasDefaultValue(0)
                .IsRequired();

            modelBuilder.Entity<RegistrationPin>()
            .HasOne(registration => registration.User)
            .WithMany()
            .HasForeignKey(registration => registration.UserId)
            .HasPrincipalKey(applicationUserClass => applicationUserClass.Id)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

            //----------- Defining RegistrationPin Entity - End --------------

            //----------------------------------------------------------------------------
            //Define Entity relationship
            //----------------------------------------------------------------------------

            //------ Relationship Brand-Course -------------------/
            modelBuilder.Entity<Batch>()
            .HasOne(BatchObject => BatchObject.Course)
            .WithMany(CourseObject => CourseObject.Batches)
            .HasForeignKey(BatchObject => BatchObject.CourseId);

            //------ Relationship Brand-Course End-------------------/

            //------ Relationship UserBatch-Internship_Record -------------------/
            modelBuilder.Entity<UserBatch>()
            .HasOne(InternshipObject => InternshipObject.InternshipRecord)
            .WithOne(UserBatchObject => UserBatchObject.UserBatch)
            .HasForeignKey<Internship_Record>(UserBatchObject => UserBatchObject.UserBatchId);
            //------ Relationship UserBatch-Internship_Record End -------------------/

            //------ Relationship Competency-Course -------------------/
            modelBuilder.Entity<Competency>()
            .HasOne(CompetencyObject => CompetencyObject.Course)
            .WithMany(CourseObject => CourseObject.Competencies)
            .HasForeignKey(CompetencyObject => CompetencyObject.CourseId);

            //------ Relationship Competency-Course End-------------------/

            modelBuilder.Entity<ApplicationUser>()
            .HasOne(ApplicationUserObject => ApplicationUserObject.Course)
            .WithMany(CourseObject => CourseObject.User)
            .HasForeignKey(ApplicationUserObject => ApplicationUserObject.CourseId)
            .IsRequired(false);

            //------ Relationship Project-Company -------------------/
            modelBuilder.Entity<Project>()
            .HasOne(ProjectObject => ProjectObject.Company)
            .WithMany(CompanyObject => CompanyObject.Projects)
            .HasForeignKey(ProjectObject => ProjectObject.CompanyID);

            //------ Relationship Project-Company End -------------------/

            /*modelBuilder.Entity<Internship_Record>()
            .HasOne(InternshipObject => InternshipObject.UserBatch)
            .WithOne(UserBatchObject => UserBatchObject.InternshipRecord)
            .HasForeignKey<UserBatch>(UserBatchObject => UserBatchObject.UserBatchId);*/

            //------ Relationship Internship_Record-Project -------------------/
            modelBuilder.Entity<Internship_Record>()
            .HasOne(InternshipObject => InternshipObject.Project)
            .WithMany(ProjectObject => ProjectObject.InternshipRecords)
            .HasForeignKey(InternshipObject => InternshipObject.ProjectId);
            //------ Relationship Internship_Record-Project End-------------------/

            //------ Relationship Internship_RecordTouchPoint -------------------/
            modelBuilder.Entity<TouchPoint_Record>()
            .HasOne(TouchPointObject => TouchPointObject.InternshipRecord)
            .WithMany(InternshipObject => InternshipObject.TouchPoints)
            .HasForeignKey(TouchPointObject => TouchPointObject.InternshipRecordId);
            //------ Relationship Internship_RecordTouchPoint End-------------------/

            //------ Relationship Internship_Record-MonthRecord -------------------/
            modelBuilder.Entity<Month_Record>()
            .HasOne(MonthObject => MonthObject.InternshipRecord)
            .WithMany(InternshipObject => InternshipObject.MonthRecords)
            .HasForeignKey(MonthObject => MonthObject.InternshipRecordId);
            //------ Relationship Internship_Record-MonthRecord End-------------------/

            //------ Relationship MonthRecordDayRecord -------------------/
            modelBuilder.Entity<Day_Record>()
            .HasOne(DayObject => DayObject.Month)
            .WithMany(MonthObject => MonthObject.DayRecords)
            .HasForeignKey(DayObject => DayObject.MonthRecordId);
            //------ Relationship MonthRecord-DayRecord End-------------------/

            //------ Relationship Competency-MonthRecord -------------------/
            modelBuilder.Entity<Competency_Checked>()
            .HasKey(u => new { u.MonthRecordId, u.CompetencyId })
            .HasName("PrimaryKey_MonthCompetency");
            modelBuilder.Entity<Competency_Checked>()
            .HasOne(MCRDefinition => MCRDefinition.MonthRecord)
            .WithMany(MCRDefinition => MCRDefinition.CompetencyCheckeds)
            .HasForeignKey(MCRDefinition => MCRDefinition.MonthRecordId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Competency_Checked>()
            .HasOne(MCRDefinition => MCRDefinition.Competency)
            .WithMany(MCRDefinition => MCRDefinition.CompetencyCheckeds)
            .HasForeignKey(MCRDefinition => MCRDefinition.CompetencyId)
            .OnDelete(DeleteBehavior.Restrict);
            //------ Relationship Competency-MonthRecord End-------------------/


            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
