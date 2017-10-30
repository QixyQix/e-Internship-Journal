using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using E_Internship_Journal.Data;

namespace E_Internship_Journal.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20171030064949_a")]
    partial class a
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("E_Internship_Journal.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<int?>("CourseId");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Batch", b =>
                {
                    b.Property<int>("BatchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("BatchId")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BatchName")
                        .IsRequired()
                        .HasColumnName("BatchName")
                        .HasColumnType("VARCHAR(20)");

                    b.Property<int>("CourseId")
                        .HasColumnName("CourseId")
                        .HasColumnType("int");

                    b.Property<string>("Description");

                    b.Property<DateTime>("EndDate");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("BatchId")
                        .HasName("PrimaryKey_BatchId");

                    b.HasIndex("BatchName")
                        .IsUnique()
                        .HasName("Batch_BatchName_UniqueConstraint");

                    b.HasIndex("CourseId");

                    b.ToTable("Batches");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Company", b =>
                {
                    b.Property<int>("CompanyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CompanyId")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CompanyAddress")
                        .IsRequired()
                        .HasColumnName("CompanyAddress")
                        .HasColumnType("VARCHAR(MAX)");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnName("CompanyName")
                        .HasColumnType("VARCHAR(100)");

                    b.HasKey("CompanyId")
                        .HasName("PrimaryKey_CompanyId");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Competency", b =>
                {
                    b.Property<int>("CompetencyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CompetencyId")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CourseId")
                        .HasColumnName("CourseId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnName("Description")
                        .HasColumnType("VARCHAR(MAX)");

                    b.Property<string>("TitleDescriotion")
                        .IsRequired()
                        .HasColumnName("TitleDescriotion")
                        .HasColumnType("VARCHAR(50)");

                    b.HasKey("CompetencyId")
                        .HasName("PrimaryKey_CompetencyId");

                    b.HasIndex("CourseId");

                    b.ToTable("competencies");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Competency_Checked", b =>
                {
                    b.Property<int>("MonthRecordId")
                        .HasColumnName("MonthRecordId")
                        .HasColumnType("int");

                    b.Property<int>("CompetencyId")
                        .HasColumnName("CompetencyId")
                        .HasColumnType("int");

                    b.Property<int>("CompentencyCheckedId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CompentencyCheckedId")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("MonthRecordId", "CompetencyId")
                        .HasName("PrimaryKey_MonthCompetency");

                    b.HasAlternateKey("CompentencyCheckedId")
                        .HasName("PrimaryKey_CompentencyCheckedId");

                    b.HasIndex("CompetencyId");

                    b.ToTable("Competency_Checkeds");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Course", b =>
                {
                    b.Property<int>("CourseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CourseId")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CourseCode")
                        .IsRequired()
                        .HasColumnName("CourseCode")
                        .HasColumnType("VARCHAR(10)");

                    b.Property<string>("CourseName")
                        .IsRequired()
                        .HasColumnName("CourseName")
                        .HasColumnType("VARCHAR(100)");

                    b.HasKey("CourseId")
                        .HasName("PrimaryKey_CourseId");

                    b.HasIndex("CourseCode")
                        .IsUnique()
                        .HasName("Course_CourseCode_UniqueConstraint");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Day_Record", b =>
                {
                    b.Property<int>("DayId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("DayId")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ArrivalTime")
                        .HasColumnName("ArrivalTime");

                    b.Property<bool?>("CompanyOff")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CompanyOff")
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime>("Date")
                        .HasColumnName("Date");

                    b.Property<DateTime>("DepartureTime")
                        .HasColumnName("DepartureTime");

                    b.Property<bool?>("MC")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MC")
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<int>("MonthRecordId")
                        .HasColumnName("MonthRecordId")
                        .HasColumnType("int");

                    b.Property<int>("WeekNo")
                        .HasColumnName("WeekNo")
                        .HasColumnType("int");

                    b.HasKey("DayId")
                        .HasName("PrimaryKey_DayId");

                    b.HasIndex("MonthRecordId");

                    b.ToTable("Day_Records");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Internship_Record", b =>
                {
                    b.Property<int>("InternshipRecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("InternshipRecordId")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnName("Comment")
                        .HasColumnType("VARCHAR(MAX)");

                    b.Property<bool>("CompanyCheck1a")
                        .HasColumnName("CompanyCheck1a")
                        .HasColumnType("bit");

                    b.Property<bool>("CompanyCheck1b")
                        .HasColumnName("CompanyCheck1b")
                        .HasColumnType("bit");

                    b.Property<bool>("CompanyCheck2a")
                        .HasColumnName("CompanyCheck2a")
                        .HasColumnType("bit");

                    b.Property<bool>("CompanyCheck2b")
                        .HasColumnName("CompanyCheck2b")
                        .HasColumnType("bit");

                    b.Property<bool>("CompanyCheck2c")
                        .HasColumnName("CompanyCheck2c")
                        .HasColumnType("bit");

                    b.Property<bool>("CompanyCheck2d")
                        .HasColumnName("CompanyCheck2d")
                        .HasColumnType("bit");

                    b.Property<bool>("CompanyCheck2e")
                        .HasColumnName("CompanyCheck2e")
                        .HasColumnType("bit");

                    b.Property<bool>("CompanyCheck2f")
                        .HasColumnName("CompanyCheck2f")
                        .HasColumnType("bit");

                    b.Property<bool>("CompanyCheck3a")
                        .HasColumnName("CompanyCheck3a")
                        .HasColumnType("bit");

                    b.Property<bool>("CompanyCheck3b")
                        .HasColumnName("CompanyCheck3b")
                        .HasColumnType("bit");

                    b.Property<bool>("CompanyCheck3c")
                        .HasColumnName("CompanyCheck3c")
                        .HasColumnType("bit");

                    b.Property<string>("FeedbackEnjoy")
                        .HasColumnName("FeedbackEnjoy")
                        .HasColumnType("VARCHAR(100)");

                    b.Property<bool>("FeedbackExperiences")
                        .HasColumnName("FeedbackExperiences")
                        .HasColumnType("bit");

                    b.Property<bool>("FeedbackImproved")
                        .HasColumnName("FeedbackImproved")
                        .HasColumnType("bit");

                    b.Property<string>("FeedbackLeastEnjoy")
                        .HasColumnName("VARCHAR(100)")
                        .HasColumnType("bit");

                    b.Property<bool>("FeedbackRecommend")
                        .HasColumnName("FeedbackRecommend")
                        .HasColumnType("bit");

                    b.Property<bool>("FeedbackUseful")
                        .HasColumnName("FeedbackUseful")
                        .HasColumnType("bit");

                    b.Property<string>("LiaisonOfficerId")
                        .IsRequired();

                    b.Property<string>("PosterUrl")
                        .HasColumnName("Description")
                        .HasColumnType("VARCHAR(MAX)");

                    b.Property<int>("PresentationGrading")
                        .HasColumnName("PresentationGrading")
                        .HasColumnType("int");

                    b.Property<int>("ProjectId")
                        .HasColumnName("ProjectId")
                        .HasColumnType("int");

                    b.Property<int>("ReflectionGrading")
                        .HasColumnName("ReflectionGrading")
                        .HasColumnType("int");

                    b.Property<int>("UserBatchId")
                        .HasColumnName("UserBatchId")
                        .HasColumnType("int");

                    b.HasKey("InternshipRecordId")
                        .HasName("PrimaryKey_InternshipRecordId");

                    b.HasIndex("LiaisonOfficerId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserBatchId")
                        .IsUnique();

                    b.ToTable("Internship_Records");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Month_Record", b =>
                {
                    b.Property<int>("MonthId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MonthId")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("Approved")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Approved")
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<int?>("CommunicationGrading")
                        .HasColumnName("CommunicationGrading")
                        .HasColumnType("int");

                    b.Property<int?>("IndependenceGrading")
                        .HasColumnName("IndependenceGrading")
                        .HasColumnType("int");

                    b.Property<int>("InternshipRecordId")
                        .HasColumnName("InternshipRecordId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("MentorSessionDateTimeEnd")
                        .HasColumnName("MentorSessionDateTimeEnd");

                    b.Property<DateTime?>("MentorSessionDateTimeStart")
                        .HasColumnName("MentorSessionDateTimeStart");

                    b.Property<string>("MentorSessionReflection")
                        .HasColumnName("MentorSessionReflection")
                        .HasColumnType("VARCHAR(MAX)");

                    b.Property<string>("OverallFeedback")
                        .HasColumnName("OverallFeedback")
                        .HasColumnType("VARCHAR(MAX)");

                    b.Property<int?>("OverallGrading")
                        .HasColumnName("OverallGrading")
                        .HasColumnType("int");

                    b.Property<int?>("PerformanceGrading")
                        .HasColumnName("PerformanceGrading")
                        .HasColumnType("int");

                    b.Property<string>("SoftSkillsCompetency")
                        .HasColumnName("SoftSkillsCompetency")
                        .HasColumnType("VARCHAR(MAX)");

                    b.Property<string>("TechnicalCompetency")
                        .HasColumnName("TechnicalCompetency")
                        .HasColumnType("VARCHAR(MAX)");

                    b.Property<int?>("TechnicalGrading")
                        .HasColumnName("TechnicalGrading")
                        .HasColumnType("int");

                    b.HasKey("MonthId")
                        .HasName("PrimaryKey_MonthId");

                    b.HasIndex("InternshipRecordId");

                    b.ToTable("Month_Records");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ProjectId")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CompanyID")
                        .HasColumnName("CompanyID")
                        .HasColumnType("int");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasColumnName("ProjectName")
                        .HasColumnType("VARCHAR(100)");

                    b.Property<string>("SupervisorId")
                        .IsRequired();

                    b.HasKey("ProjectId")
                        .HasName("PrimaryKey_ProjectId");

                    b.HasIndex("CompanyID");

                    b.HasIndex("SupervisorId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.RegistrationPin", b =>
                {
                    b.Property<int>("RegistrationPinId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RegistrationPinId")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Pin");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("RegistrationPinId")
                        .HasName("PrimaryKey_RegistrationPinId");

                    b.HasIndex("UserId");

                    b.ToTable("RegistrationPins");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Task_Record", b =>
                {
                    b.Property<int>("TaskRecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("TaskRecordId")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DayRecordId")
                        .HasColumnName("DayRecordId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnName("Description")
                        .HasColumnType("VARCHAR(MAX)");

                    b.HasKey("TaskRecordId")
                        .HasName("PrimaryKey_TaskRecordId");

                    b.HasIndex("DayRecordId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.TouchPoint_Record", b =>
                {
                    b.Property<int>("TouchPointId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("TouchPointId")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comments")
                        .IsRequired()
                        .HasColumnName("Comments")
                        .HasColumnType("VARCHAR(MAX)");

                    b.Property<int>("InternshipRecordId")
                        .HasColumnName("InternshipRecordId")
                        .HasColumnType("int");

                    b.HasKey("TouchPointId")
                        .HasName("PrimaryKey_TouchPointId");

                    b.HasIndex("InternshipRecordId");

                    b.ToTable("TouchPoint_Record");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.UserBatch", b =>
                {
                    b.Property<int>("UserBatchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("UserBatchId")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BatchId")
                        .HasColumnName("BatchId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("UserBatchId")
                        .HasName("PrimaryKey_UserBatchId");

                    b.HasIndex("BatchId");

                    b.HasIndex("UserId");

                    b.ToTable("UserBatches");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.ApplicationUser", b =>
                {
                    b.HasOne("E_Internship_Journal.Models.Course", "Course")
                        .WithMany("User")
                        .HasForeignKey("CourseId");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Batch", b =>
                {
                    b.HasOne("E_Internship_Journal.Models.Course", "Course")
                        .WithMany("Batches")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Competency", b =>
                {
                    b.HasOne("E_Internship_Journal.Models.Course", "Course")
                        .WithMany("Competencies")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Competency_Checked", b =>
                {
                    b.HasOne("E_Internship_Journal.Models.Competency", "Competency")
                        .WithMany("CompetencyCheckeds")
                        .HasForeignKey("CompetencyId");

                    b.HasOne("E_Internship_Journal.Models.Month_Record", "MonthRecord")
                        .WithMany("CompetencyCheckeds")
                        .HasForeignKey("MonthRecordId");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Day_Record", b =>
                {
                    b.HasOne("E_Internship_Journal.Models.Month_Record", "Month")
                        .WithMany("DayRecords")
                        .HasForeignKey("MonthRecordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Internship_Record", b =>
                {
                    b.HasOne("E_Internship_Journal.Models.ApplicationUser", "LiaisonOfficer")
                        .WithMany()
                        .HasForeignKey("LiaisonOfficerId");

                    b.HasOne("E_Internship_Journal.Models.Project", "Project")
                        .WithMany("InternshipRecords")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("E_Internship_Journal.Models.UserBatch", "UserBatch")
                        .WithOne("InternshipRecord")
                        .HasForeignKey("E_Internship_Journal.Models.Internship_Record", "UserBatchId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Month_Record", b =>
                {
                    b.HasOne("E_Internship_Journal.Models.Internship_Record", "InternshipRecord")
                        .WithMany("MonthRecords")
                        .HasForeignKey("InternshipRecordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Project", b =>
                {
                    b.HasOne("E_Internship_Journal.Models.Company", "Company")
                        .WithMany("Projects")
                        .HasForeignKey("CompanyID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("E_Internship_Journal.Models.ApplicationUser", "Supervisor")
                        .WithMany()
                        .HasForeignKey("SupervisorId");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.RegistrationPin", b =>
                {
                    b.HasOne("E_Internship_Journal.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("E_Internship_Journal.Models.Task_Record", b =>
                {
                    b.HasOne("E_Internship_Journal.Models.Day_Record", "DayRecord")
                        .WithMany("Tasks")
                        .HasForeignKey("DayRecordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("E_Internship_Journal.Models.TouchPoint_Record", b =>
                {
                    b.HasOne("E_Internship_Journal.Models.Internship_Record", "InternshipRecord")
                        .WithMany("TouchPoints")
                        .HasForeignKey("InternshipRecordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("E_Internship_Journal.Models.UserBatch", b =>
                {
                    b.HasOne("E_Internship_Journal.Models.Batch", "Batch")
                        .WithMany("UserBatches")
                        .HasForeignKey("BatchId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("E_Internship_Journal.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("E_Internship_Journal.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("E_Internship_Journal.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("E_Internship_Journal.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
