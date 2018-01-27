using E_Internship_Journal.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Data
{
    public static class DataSeeder
    {
        public static async void SeedData(ApplicationDbContext db)
        {
            //if db is not exist ,it will create database .but ,do nothing .
            db.Database.EnsureCreated();

            //RoleStore needs using Microsoft.AspNet.Identity.EntityFramework;
            var identityRoleStore = new RoleStore<IdentityRole>(db);
            var identityRoleManager = new RoleManager<IdentityRole>(identityRoleStore, null, null, null, null, null);

            //declaration of variable i for arrays
            //var i = 0;

            //Defining user "roles" inside the AspNetRoles table.
            var adminRole = new IdentityRole { Name = "ADMIN" };
            await identityRoleManager.CreateAsync(adminRole);

            var studentRole = new IdentityRole { Name = "STUDENT" };            await identityRoleManager.CreateAsync(studentRole);

            var supervisorRole = new IdentityRole { Name = "SUPERVISOR" };
            await identityRoleManager.CreateAsync(supervisorRole);

            var liaisonOfficerRole = new IdentityRole { Name = "LO" };
            await identityRoleManager.CreateAsync(liaisonOfficerRole);

            var seniorLiaisonOfficerRole = new IdentityRole { Name = "SLO" };
            await identityRoleManager.CreateAsync(seniorLiaisonOfficerRole);

            db.SaveChanges();

            //Create Courses
            var courseObject = new Course[] {
                new Course {CourseCode = "DIT", CourseName = "Diploma in Information Technology" },
                new Course {CourseCode = "DBIT", CourseName = "Diploma in Business Information Technology" },
                new Course {CourseCode = "EEE", CourseName = "Diploma in Electrical and Electronic Engineering" },
            };
            foreach (Course c in courseObject)
            {
                db.Courses.Add(c);
            }
            db.SaveChanges();

            var competencyTitle = new CompetencyTitle
            {
                TitleCompetency = "Domain Immersion",
                Course = courseObject[0],
                ViewBy = 1
            };
            db.CompetencyTitles.Add(competencyTitle);
            db.SaveChanges();
            //Create Competency
            //var competencyObject = new Competency[] {
            //    new Competency { Description = "Describe and explain respectively domain knowledge Understand existing business operations Identify roles and responsibilities of company staff members Understand a project/system business requirements", TitleDescription = "Domain Immersation", Course = courseObject[0] },
            //    new Competency { Description = "Participate and contribute to the review, analysis and verification of business and software requirements Assist project team for project requirements gathering and analysis Assist in gathering of user requirements, which can be any activity such as design of questionnaire, logistics planning Identify and document an application problem or symptoms from users(when there is reported software bug) Propose solutions to an application problem with designated supervisors or mentors", TitleDescription = "Project / System Analysis & Design", Course = courseObject[0] },
            //    new Competency { Description = " Prepare technical documentation and reports on requirements and analysis Report the workflow of a business requirement, in the form of UML(Sequence, State etc. diagrams) or any in company standard documentation format Report or present a problem resolution/design to the users Document constraints or possible impacts in the application, during any code or functionality changes from bugfix or change request Perform an impact analysis (i.e. what could happen to the users, like loss of what data) of the risks involved in the reported application problem/error", TitleDescription = "Project/System Analysis & Design", Course = courseObject[0] },
            //    new Competency { Description = "Discuss with the student on the internship requirements. (This is recommended to be carried out at the start of the internship)", TitleDescription = "Project/System Analysis & Design", Course = courseObject[0] },
            //    new Competency { Description = "Guide the student to meet requirements and learning objectives. This includes guiding the student on the tasks, setting performance expectations and encouraging student to ask questions to clarify job resposibilities.", TitleDescription = "Project/System Analysis & Design", Course = courseObject[0] },
            //    new Competency { Description = "Provide periodic feedback to student on his/her performance", TitleDescription = "Project/System Analysis & Design", Course = courseObject[0] },
            //    new Competency { Description = "Meet and discuss with the SP Liaison Officer on student's progress and performance during company visits.", TitleDescription = "Project/System Analysis & Design", Course = courseObject[0] },
            //    new Competency { Description = "Provide final assessment on student's performance", TitleDescription = "Project/System Analysis & Design", Course = courseObject[0] },
            //    new Competency { Description = "Provide student an overview of the company structure, nature of business and work requirements.", TitleDescription = "Mentorship", Course = courseObject[0] },
            //    new Competency { Description = "Share about job propects, career paths and trends within the industry. This can be done through sharing of work experience, relevant resources, networking contacts, as appropriate.", TitleDescription = "Mentorship", Course = courseObject[0] },
            //};
            //foreach (Competency c in competencyObject)
            //{
            //    db.Competencies.Add(c);
            //}
            var competencyObject = new Competency[] {
                new Competency { Description = "Able to code using Java/CSS/HTML", CompetencyTitleId = competencyTitle.CompetencyTitleId , ViewBy = 1},
                new Competency { Description = "Able to code using Java/CSS/HTML",CompetencyTitleId = competencyTitle.CompetencyTitleId , ViewBy = 2},
                new Competency { Description = "Able to code using Java/CSS/HTML", CompetencyTitleId = competencyTitle.CompetencyTitleId, ViewBy = 3 }
            };
            foreach (Competency c in competencyObject)
            {
                db.Competencies.Add(c);
            }
            db.SaveChanges();

            //CREATE USERS
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);

            //Admin
            var adminUser = new ApplicationUser[] {
                new ApplicationUser { UserName = "ADMIN@TEST.COM", FullName = "Mr Administrator", Email = "ADMIN@TEST.COM", IsEnabled = true }
            };
            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            //adminUser.PasswordHash = ph.HashPassword(adminUser, "P@ssw0rd"); //More complex password
            //await userManager.CreateAsync(adminUser);
            //await userManager.AddToRoleAsync(adminUser, "ADMIN");
            for (int i = 0; i < adminUser.Length; i++)
            {
                adminUser[i].PasswordHash = ph.HashPassword(adminUser[i], "P@ssw0rd"); //More complex password
                await userManager.CreateAsync(adminUser[i]);
                await userManager.AddToRoleAsync(adminUser[i], "ADMIN");
            }

            //Student
            var studentUser = new ApplicationUser[] {
                new ApplicationUser { UserName = "P1551581@TEST.COM", FullName = "Ong Boon Ping, Klein", Email = "P1551581@TEST.COM", /*Course=courseObject,*/ IsEnabled = true },
                new ApplicationUser { UserName = "P1530247@TEST.COM", FullName = "Amelia Goh Jia Pei", Email = "P1530247@TEST.COM", /*Course=courseObject,*/ IsEnabled = true },
                new ApplicationUser { UserName = "P1551635@TEST.COM", FullName = "Loo Jia Wei", Email = "P1551635@TEST.COM", /*Course=courseObject,*/ IsEnabled = true },
                new ApplicationUser { UserName = "P1551677@TEST.COM", FullName = "Tan Chia Wei", Email = "P1551677@TEST.COM", /*Course=courseObject,*/ IsEnabled = true },
                new ApplicationUser { UserName = "P1530078@TEST.COM", FullName = "Cho Qi Xiang", Email = "P1530078@TEST.COM", /*Course=courseObject,*/ IsEnabled = true },
                new ApplicationUser { UserName = "P1555555@TEST.COM", FullName = "Sim Zi Quan", Email = "P1555555@TEST.COM", /*Course=courseObject,*/ IsEnabled = true },
                new ApplicationUser { UserName = "P1566666@TEST.COM", FullName = "Kish Choy", Email = "P1566666@TEST.COM", /*Course=courseObject,*/ IsEnabled = true },
                new ApplicationUser { UserName = "P1577777@TEST.COM", FullName = "Mark Lee Guo Jing", Email = "P1577777@TEST.COM", /*Course=courseObject,*/ IsEnabled = true },
                new ApplicationUser { UserName = "STUDENT@TEST.COM", FullName = "James Bond", Email = "STUDENT@TEST.COM", /*Course=courseObject,*/ IsEnabled = true },
            };
            ph = new PasswordHasher<ApplicationUser>();
            for (int i = 0; i < studentUser.Length; i++)
            {
                studentUser[i].PasswordHash = ph.HashPassword(studentUser[i], "P@ssw0rd"); //More complex password
                await userManager.CreateAsync(studentUser[i]);
                await userManager.AddToRoleAsync(studentUser[i], "STUDENT");
            }

            //Supervisor
            var supervisorUser = new ApplicationUser[] {

                new ApplicationUser { UserName = "JOHNNYLIM@COMPANY.COM", FullName = "Johnny Lim", Email = "JOHNNYLIM@COMPANY.COM", IsEnabled = true },
                new ApplicationUser { UserName = "GOHCK@COMPANY.COM", FullName = "Goh Ching Keong", Email = "SUPERVISOR@COMPANY.COM", IsEnabled = true },
                new ApplicationUser { UserName = "TIMOTHYTCM@COMPANY.COM", FullName = "Timothy Tan Chee Meng", Email = "TIMOTHYTCM@COMPANY.COM", IsEnabled = true },
                new ApplicationUser { UserName = "JOWENDLC@COMPANY.COM", FullName = "Jowen De La Cruz", Email = "JOWENDLC@COMPANY.COM", IsEnabled = true },
                new ApplicationUser { UserName = "JUNENDANIEL@COMPANY.COM", FullName = "Daniel Tan Jun En", Email = "JUNENDANIEL@COMPANY.COM", IsEnabled = true },
                new ApplicationUser { UserName = "JESSCHOO@COMPANY.COM", FullName = "Jess Choo Li Ying", Email = "JESSCHOO@COMPANY.COM", IsEnabled = true },
                new ApplicationUser { UserName = "TECKMING@COMPANY.COM", FullName = "Micheal Lee Teck Ming", Email = "TECKMING@COMPANY.COM", IsEnabled = true },
                new ApplicationUser { UserName = "DESENG@COMPANY.COM", FullName = "Lee De Seng", Email = "DESENG@COMPANY.COM", IsEnabled = true },
                new ApplicationUser { UserName = "SUSANPX@COMPANY.COM", FullName = "Susan Yee Pei Xuan", Email = "SUSANPX@COMPANY.COM", IsEnabled = true },
                new ApplicationUser { UserName = "YINGYING@COMPANY.COM", FullName = "Luo Ying Ying", Email = "YINGYING@COMPANY.COM", IsEnabled = true },
                new ApplicationUser { UserName = "SAMSOM@COMPANY.COM", FullName = "Samson Reuben", Email = "SAMSON@COMPANY.COM", IsEnabled = true },
                new ApplicationUser { UserName = "SUPERVISOR@TEST.COM", FullName = "Santhosh Kumar", Email = "SUPERVISOR@TEST.COM", IsEnabled = true }
            };
            ph = new PasswordHasher<ApplicationUser>();
            for (int i = 0; i < supervisorUser.Length; i++)
            {
                supervisorUser[i].PasswordHash = ph.HashPassword(supervisorUser[i], "P@ssw0rd"); //More complex password
                await userManager.CreateAsync(supervisorUser[i]);
                await userManager.AddToRoleAsync(supervisorUser[i], "SUPERVISOR");
            }

            //LO
            var LOUser = new ApplicationUser[] {
                new ApplicationUser { UserName = "LO@TEST.COM", FullName = "Leong Fong Sow", Email = "LO@TEST.COM", IsEnabled = true },
                new ApplicationUser { UserName = "LJK@TEST.COM", FullName = "Low Jin Kiat", Email = "LJK@TEST.COM", IsEnabled = true },
                new ApplicationUser { UserName = "LDY@TEST.COM", FullName = "Lim De Yang", Email = "LDY@TEST.COM", IsEnabled = true },
                new ApplicationUser { UserName = "THS@TEST.COM", FullName = "Tan Hu Shien", Email = "THS@TEST.COM", IsEnabled = true },
                new ApplicationUser { UserName = "LZ@TEST.COM", FullName = "Lin Zhao", Email = "LZ@TEST.COM", IsEnabled = true },
                new ApplicationUser { UserName = "RP@TEST.COM", FullName = "Ronnie Peh", Email = "RP@TEST.COM", IsEnabled = true },
                new ApplicationUser { UserName = "JPX@TEST.COM", FullName = "Ji Peng Xiang", Email = "JPX@TEST.COM", IsEnabled = true },
                new ApplicationUser { UserName = "CK@TEST.COM", FullName = "Chanon Kulchol", Email = "CK@TEST.COM", IsEnabled = true }
            };
            ph = new PasswordHasher<ApplicationUser>();
            for (int i = 0; i < LOUser.Length; i++)
            {
                LOUser[i].PasswordHash = ph.HashPassword(LOUser[i], "P@ssw0rd"); //More complex password
                await userManager.CreateAsync(LOUser[i]);
                await userManager.AddToRoleAsync(LOUser[i], "LO");
            }

            //SLO
            var SLOUser = new ApplicationUser[] {
                new ApplicationUser { UserName = "SLO@TEST.COM", FullName = "Teo Seok Ling", Email = "SLO@TEST.COM", /*Course = courseObject, */ IsEnabled = true }
            };
            ph = new PasswordHasher<ApplicationUser>();
            for (int i = 0; i < SLOUser.Length; i++)
            {
                SLOUser[i].PasswordHash = ph.HashPassword(SLOUser[i], "P@ssw0rd"); //More complex password
                await userManager.CreateAsync(SLOUser[i]);
                await userManager.AddToRoleAsync(SLOUser[i], "SLO");
            }

            db.SaveChanges();

            //Create Batch
            var batchObject = new Batch[]
            {
                new Batch { BatchName = "AY1718S1", Description = "Academic Year 2017-2018 Semester 1", StartDate = DateTime.ParseExact("14/06/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/09/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[0] },
                new Batch { BatchName = "AY1718S2", Description = "Academic Year 2017-2018 Semester 2", StartDate = DateTime.ParseExact("30/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("30/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[0] },
                new Batch { BatchName = "AY1718S1", Description = "Academic Year 2017-2018 Semester 1", StartDate = DateTime.ParseExact("14/06/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/09/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[1] },
                new Batch { BatchName = "AY1718S2", Description = "Academic Year 2017-2018 Semester 2", StartDate = DateTime.ParseExact("30/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("28/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[1] },
                new Batch { BatchName = "AY1718S1", Description = "Academic Year 2017-2018 Semester 1", StartDate = DateTime.ParseExact("14/02/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/08/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[2] },
                new Batch { BatchName = "AY1718S2", Description = "Academic Year 2017-2018 Semester 2", StartDate = DateTime.ParseExact("28/09/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("28/02/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[2] },
                new Batch { BatchName = "AY1617S1", Description = "Academic Year 2016-2017 Semester 1", StartDate = DateTime.ParseExact("14/06/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/09/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[0] },
                new Batch { BatchName = "AY1617S2", Description = "Academic Year 2016-2017 Semester 2", StartDate = DateTime.ParseExact("28/10/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("28/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[0] },
                new Batch { BatchName = "AY1617S1", Description = "Academic Year 2016-2017 Semester 1", StartDate = DateTime.ParseExact("14/06/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/09/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[1] },
                new Batch { BatchName = "AY1617S2", Description = "Academic Year 2016-2017 Semester 2", StartDate = DateTime.ParseExact("28/10/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("28/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[1] },
                new Batch { BatchName = "AY1617S1", Description = "Academic Year 2016-2017 Semester 1", StartDate = DateTime.ParseExact("14/02/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/08/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[2] },
                new Batch { BatchName = "AY1617S2", Description = "Academic Year 2016-2017 Semester 2", StartDate = DateTime.ParseExact("28/09/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("28/02/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[2] },
                new Batch { BatchName = "AY1516S1", Description = "Academic Year 2015-2016 Semester 1", StartDate = DateTime.ParseExact("14/06/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/09/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[0] },
                new Batch { BatchName = "AY1516S2", Description = "Academic Year 2015-2016 Semester 2", StartDate = DateTime.ParseExact("28/10/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("28/01/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[0] },
                new Batch { BatchName = "AY1516S1", Description = "Academic Year 2015-2016 Semester 1", StartDate = DateTime.ParseExact("14/02/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/08/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[2] },
                new Batch { BatchName = "AY1516S2", Description = "Academic Year 2015-2016 Semester 2", StartDate = DateTime.ParseExact("28/09/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("28/02/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[2] },
            };
            foreach (Batch b in batchObject)
            {
                db.Batches.Add(b);
            }
            db.SaveChanges();


            //Create Company
            var companyObject = new Company[]
            {
                new Company { CompanyName = "KPMG", CompanyAddress = "BLK559 #03-03 Raffles City SINGAPORE 666559", ContactPersonName = "Goh Ching Keong", ContactPersonNumber = "91239240", ContactPersonEmail = "gohck@kpmg.com", ContactPersonFax = "85001000" },
                new Company { CompanyName = "RapidCloud Pte Ltd", CompanyAddress = "BLK90 #04-06 Redhill Drive SINGAPORE 687090", ContactPersonName = "Jowen De La Cruz", ContactPersonNumber = "87545098", ContactPersonEmail = "jowen@rapidcloud.com", ContactPersonFax = "67594059" },
                new Company { CompanyName = "Webspark Pte Ltd", CompanyAddress = "BLK410 #02-19 19 Kallang Ave SINGAPORE 339410", ContactPersonName = "Daniel Tan Jun En", ContactPersonNumber = "94372138", ContactPersonEmail = "danieltje@webspark.com", ContactPersonFax = "67654334" },
                new Company { CompanyName = "IT Solution Pte Ltd", CompanyAddress = "BLK900 #07-07 51 Goldhill Plaza SINGAPORE 308900", ContactPersonName = "Jess Choo Li Ying", ContactPersonNumber = "87435948", ContactPersonEmail = "jesschoo@itsolution.com", ContactPersonFax = "66903275" },
                new Company { CompanyName = "Google", CompanyAddress = "BLK371 #03-71 70 Pasir Panjang Road SINGAPORE 117371", ContactPersonName = "Micheal Lee Teck Ming", ContactPersonNumber = "93342289", ContactPersonEmail = "teckming88@google.com", ContactPersonFax = "63429098" },
                new Company { CompanyName = "Paypal", CompanyAddress = " Suntec Tower Five #09-01 5 Temasek Blvd SINGAPORE 038985", ContactPersonName = "Lee De Seng", ContactPersonNumber = "87009865", ContactPersonEmail = "leedeseng@paypal.com", ContactPersonFax = "62469864" },
                new Company { CompanyName = "PSA Corporation Ltd", CompanyAddress = " BLK55 #01-28 7B Keppel Road SINGAPORE 089055", ContactPersonName = "Susan Yee Pei Xuan", ContactPersonNumber = "97876855", ContactPersonEmail = "susanypx@psacorp.sg", ContactPersonFax = "62459800" },
                new Company { CompanyName = "Apixel IT Support", CompanyAddress = " BLK983 #39-01 10 Marina Boulevard SINGAPORE 018983", ContactPersonName = "Luo Ying Ying", ContactPersonNumber = "89710761", ContactPersonEmail = "yyluo@apixel.com", ContactPersonFax = "64399089" },
                new Company { CompanyName = "EFusion Technology Pte Ltd", CompanyAddress = " BLK321A #01-96 Beach Rd SINGAPORE 199557", ContactPersonName = "Samson Reuben", ContactPersonNumber = "96437865", ContactPersonEmail = "samsonreuben@efusion.com", ContactPersonFax = "64890392" },
                new Company { CompanyName = "Tinkerbox Studios Pte Ltd", CompanyAddress = " BLK113B #01-65 Jalan Besar SINGAPORE 208833", ContactPersonName = "Timothy Tan Chee Meng", ContactPersonNumber = "84889032", ContactPersonEmail = "timothytan@tinkerbox.sg", ContactPersonFax = "68732131" }
            };
            foreach (Company c in companyObject)
            {
                db.Companies.Add(c);
            }
            db.SaveChanges();

            ////Create Company
            //var companyObject = new Company { CompanyName = "Some Weird Company", CompanyAddress = "BLK999# 03-03 PUNGGOL STREET SINGAPORE 666666", ContactPersonName = "Company Owner Name", ContactPersonNumber = "91239240", ContactPersonEmail = "CEO@COMPANY.COM", ContactPersonFax = "85001000" };
            //db.Companies.Add(companyObject);
            //db.SaveChanges();

            //Create Project
            var projectObject = new Project[]
            {
                new Project {ProjectName = "Working Towards Smart Nation", Company = companyObject[0], Supervisor = supervisorUser[0]},
                new Project {ProjectName = "Database management system", Company = companyObject[2], Supervisor = supervisorUser[0]},
                new Project {ProjectName = "Headless CMS website management system", Company = companyObject[1], Supervisor = supervisorUser[1]},
                new Project {ProjectName = "Creating new SEO", Company = companyObject[3], Supervisor = supervisorUser[1]},
                new Project {ProjectName = "New Government website design", Company = companyObject[4], Supervisor = supervisorUser[0] },
                new Project {ProjectName = "Maintaining new startup website", Company = companyObject[5], Supervisor = supervisorUser[0] },
                new Project {ProjectName = "Design a brand new website for webspark", Company = companyObject[6], Supervisor = supervisorUser[1]},
                new Project {ProjectName = "Maintaining SP spade", Company = companyObject[7], Supervisor = supervisorUser[1] },
                new Project {ProjectName = "Website management system", Company = companyObject[8], Supervisor = supervisorUser[1] },
                new Project {ProjectName = "Debug for rapicloud website", Company = companyObject[9], Supervisor = supervisorUser[1] }
            };
            foreach (Project p in projectObject)
            {
                db.Projects.Add(p);
            }
            db.SaveChanges();

            //Create UserBatch
            var userBatchObject = new UserBatch[] {
                new UserBatch { Batch = batchObject[0], User = SLOUser[0] }, //Batch:AY1718S1 Student:P1551581@TEST.COM
                new UserBatch { Batch = batchObject[0], User = studentUser[1] },
                new UserBatch { Batch = batchObject[0], User = studentUser[2] },
                new UserBatch { Batch = batchObject[0], User = studentUser[3] },
                new UserBatch { Batch = batchObject[0], User = studentUser[4] },

                new UserBatch { Batch = batchObject[1], User = SLOUser[0] },
                new UserBatch { Batch = batchObject[1], User = studentUser[0] },
                new UserBatch { Batch = batchObject[1], User = studentUser[5] },
                new UserBatch { Batch = batchObject[1], User = studentUser[6] },
                //new UserBatch { Batch = batchObject[1], User = studentUser[6] },
                //new UserBatch { Batch = batchObject[1], User = studentUser[7] },
                //new UserBatch { Batch = batchObject[1], User = studentUser[8] },
                //new UserBatch { Batch = batchObject[1], User = studentUser[9] },
                //new UserBatch { Batch = batchObject[1], User = studentUser[10] },
                //new UserBatch { Batch = batchObject[1], User = studentUser[11] },
                //new UserBatch { Batch = batchObject[1], User = studentUser[12] },
                  };
            foreach (UserBatch ub in userBatchObject)
            {
                db.UserBatches.Add(ub);
            };
            db.SaveChanges();

            //var userSLOUserBatchObject = new UserBatch[] {
            //new UserBatch { Batch = batchObject[0], User = SLOUser }
            //};
            //db.UserBatches.Add(userSLOUserBatchObject);

            //db.SaveChanges();

            //var userBatchObject2 = new UserBatch { Batch = batchObject, User = studentUser2 };
            //db.UserBatches.Add(userBatchObject2);

            //db.SaveChanges();

            //Create Internship Journal
            var internshipObject = new Internship_Record[]
            {
                new Internship_Record { UserBatch = userBatchObject[6], Project = projectObject[0], LiaisonOfficer = LOUser[4] },
                new Internship_Record { UserBatch = userBatchObject[7], Project = projectObject[0], LiaisonOfficer = LOUser[4] },  //Batch:AY1718S1 Student:P1551581@TEST.COM Project:Working Towards Smart Nation LO:Leong Fong Sow
                new Internship_Record { UserBatch = userBatchObject[8], Project = projectObject[7], LiaisonOfficer = LOUser[4] }
            };
            foreach (Internship_Record ir in internshipObject)
            {
                db.Internship_Records.Add(ir);
            }
            db.SaveChanges();

            var monthObject = new Month_Record[] {
                new Month_Record { InternshipRecordId = internshipObject[0].InternshipRecordId, SoftSkillsCompetencyDoneWell = "I did well in my time management manage to finish up all the task give to me on time and i did my task very carefully, did a few time of checking and troubleshoot before hand in my task to my supervisor.", SoftSkillsCompetencyImprove = "I feel that i should improve in my communication skills with my colleague so i can interact and get to know more about them, also that i will not feel awkward when having meeting and can give out opinion.", TechnicalCompetencyApplied = "i have applied things that i had learn in the school. Got to apply some simple knowledge on Design experience when creating the website do a simple survey question and post it online to let the public answer.", TechnicalCompetencyDoneWell = "I did well on how quickly i can think of an idea and suggest out by giving some idea and get accepted. As also because i have the idea i gave is something similar to what i have did in one of my assignment in school", TechnicalCompetencyImprove="I want to improve on how i can be more confident because i did hesitate not to give my idea as i thought it was bad, was very happy that got accepted." },
                new Month_Record { InternshipRecordId = internshipObject[1].InternshipRecordId, SoftSkillsCompetencyDoneWell = "I try to finish up all the task given to me on time and i am cautious with my work, check at least twice and troubleshoot before I hand in my task to my supervisor.", SoftSkillsCompetencyImprove = "I feel that i am not good at communication skills, whenever i talk to my colleague i get nervous, i can barely present myself when we are having a meeting.", TechnicalCompetencyApplied = "i have applied things that i had learn in the school. Got to apply some simple knowledge on Design experience when creating the website do a simple survey question and post it online to let the public answer.", TechnicalCompetencyDoneWell = "i manage to excel when it comes to quick thinking and creativity. i can think of an idea and suggest out by giving some idea and get accepted. As also because i have the idea i gave is something similar to what i have did in one of my assignment in school", TechnicalCompetencyImprove="I want to improve on being more confident, as i want to convince people to accept my idea instead of being rejected because i was not convincing enough." },
                new Month_Record { InternshipRecordId = internshipObject[2].InternshipRecordId, SoftSkillsCompetencyDoneWell = "Given the fact that i am a cautious person, i would always check to double confirm, I often give my opinion on solution to help solving an issue", SoftSkillsCompetencyImprove = "I would need to improve on by over confidence attitude, whenever i see my colleague talking, i often give my own opinion without being asked for, i can be rather rude in such ways.", TechnicalCompetencyApplied = "i have applied things i learn school. Got to apply some basic skills on programming experience when hosting a website i am able to follow up with the project.", TechnicalCompetencyDoneWell = "i manage to excel when it comes to quick thinking and creativity. i can think of an idea and suggest out by giving some idea and get accepted. As also because i have the idea i gave is something similar to what i have did in one of my assignment in school", TechnicalCompetencyImprove="I want to learn more so that I am able to improve my programming skills and with it I can help others whenever they encounter a problem." },
            };
            foreach (Month_Record mr in monthObject)
            {
                db.Month_Records.Add(mr);
            }
            db.SaveChanges();

            var dayObject = new Day_Record[] {                //student1
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("30/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 1, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("31/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 1, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("01/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 1, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("02/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 1, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("03/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 1, IsPresent = true },

                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("06/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 2, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("07/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 2, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("08/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 2, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("09/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 2, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("10/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 2, IsPresent = true },

                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("13/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 3, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("14/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 3, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("15/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 3, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("16/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 3, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("17/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 3, IsPresent = true },

                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("20/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 4, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("21/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 4, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("22/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 4, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("23/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 4, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("24/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 4, IsPresent = true },

                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("27/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 5, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("28/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 5, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("29/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 5, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("30/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 5, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("01/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 5, IsPresent = true },

                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("04/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 6, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("05/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 6, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("06/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 6, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("07/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 6, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("08/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 6, IsPresent = true },

                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("11/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 7, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("12/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 7, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("13/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 7, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("14/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 7, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("15/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 7, IsPresent = true },

                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("19/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 8, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("20/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 8, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("21/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 8, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("22/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 8, IsPresent = true },

                //new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("25/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 8, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("26/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 9, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("27/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 9, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("28/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 9, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("29/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 9, IsPresent = true },

                //new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("01/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 8, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("02/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 10, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("03/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 10, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("04/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 10, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("05/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 10, IsPresent = true },

                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("08/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 11, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 11, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("10/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 11, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("11/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 11, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("12/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 11, IsPresent = true },

                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("15/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 12, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("16/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 12, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("17/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 12, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("18/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 12, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("19/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 12, IsPresent = true },

                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("22/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 13, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("23/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 13, IsPresent = true },
                new Day_Record { Month = monthObject[0], Date = DateTime.ParseExact("24/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 13, IsPresent = true },


                //student2
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("30/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 1, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("31/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 1, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("01/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 1, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("02/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 1, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("03/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 1, IsPresent = true },

                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("06/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 2, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("07/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 2, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("08/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 2, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("09/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 2, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("10/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 2, IsPresent = true },

                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("13/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 3, IsPresent = true },
                //sick mc 2 days 14 - 15/11/2017
                //new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("14/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 3, IsPresent = true },
                //new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("15/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 3, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("16/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 3, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("17/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 3, IsPresent = true },

                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("20/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 4, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("21/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 4, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("22/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 4, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("23/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 4, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("24/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 4, IsPresent = true },

                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("27/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 5, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("28/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 5, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("29/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 5, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("30/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 5, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("01/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 5, IsPresent = true },

                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("04/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 6, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("05/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 6, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("06/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 6, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("07/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 6, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("08/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 6, IsPresent = true },

                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("11/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 7, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("12/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 7, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("13/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 7, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("14/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 7, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("15/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 7, IsPresent = true },

                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("19/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 8, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("20/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 8, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("21/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 8, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("22/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 8, IsPresent = true },

                //new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("25/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 8, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("26/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 9, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("27/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 9, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("28/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 9, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("29/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 9, IsPresent = true },

                //new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("01/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 8, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("02/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 10, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("03/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 10, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("04/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 10, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("05/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 10, IsPresent = true },

                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("08/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 11, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 11, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("10/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 11, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("11/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 11, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("12/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 11, IsPresent = true },

                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("15/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 12, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("16/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 12, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("17/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 12, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("18/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 12, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("19/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 12, IsPresent = true },

                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("22/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 13, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("23/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 13, IsPresent = true },
                new Day_Record { Month = monthObject[1], Date = DateTime.ParseExact("24/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 13, IsPresent = true },
                };
            foreach (Day_Record dr in dayObject)
            {
                db.Day_Records.Add(dr);
            }
            db.SaveChanges();

            var taskObject = new Task_Record[] {
                //student 1
                //week1
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("30/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Download and try to run source code", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("30/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Explore application (Adding an assignment)", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("30/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meet with Terry for source code setup and partial explanation (Error with setup/code for adding java/python assignment)", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("30/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on Hangfire documentation about background processing", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("31/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Setup and try running code with visual studio 2015", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("31/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on Hangfire documentation about background processing cont.", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("31/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Further read up on Hangfire documentation about background methods", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("31/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Understanding on terms about setup/code error (add java/python assignment)", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("31/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Search up on error/Talk to Mr Low about said error. ", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("31/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Try fixing error by setting working directory (Result: failed) (Conclusion: Seek for assistance or use another computer)", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("01/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Setup spare computer for testing", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("01/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on usage of Processes", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("01/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Figure out error (Error adding java/python assignment) (Solved) (Cause: VS Solution not in the same hard drive partition)", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("01/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Discussion with colleagues about code (Code Briefing)", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("01/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Bug Fix Hangfire (Try different versions Hangfire / Strong Key Signing, use package) ", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("02/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Explore and test code and web application for further understanding", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("02/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Bug: During login if admin no letter (e.g. p/s/a) entered is in caps error will be thrown to user.", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("02/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Bug Fixed Hangfire Dashboard Click into Jobs etc (Cause: Hangfire Database error) (Cause: Hangfire Database error) ", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("03/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Figure out errors; Not being able to add python or java due to python not being able to find the upload file in the file path (on local)", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("03/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meeting with Mr Low and Mr Karl Kwan, about changes to be made: Allow lecturer to be able to view assignment as if he was a student and allow Admin/Lecturer to approve a new registration.", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("03/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Prepare for demo; add students to actual database for Mr Karl Kwan for his lesson starting week 1 of the next term. Contact him if no response from him before dateline.", WeekNo = 1 },

                 //week2
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("06/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "add students to actual database for Mr Karl Kwan for his lesson starting week 1 of the next term. Contact him if no response from him before dateline.", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("06/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Begin adding lecturer’s student view. (added controller, views, viewmodels etc. code)", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("07/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add “ghost” student account creation on login as lecturer.", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("07/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Research and test to see which way of coding the feature is better. ", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("07/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Create a “ghost/temp” student account for the lecturer to view it", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("08/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Implement test assignment feature.", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("08/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Figure out error of 'System.ComponentModel.Win32Exception' in System.dll ('Access is denied') System.ComponentModel.Win32Exception", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("08/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "'System.InvalidOperationException' in System.dll ('Cannot process request because the process (20496) has exited.') System.InvalidOperationException", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("09/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Allow Lecturer to Test Assignment despite assignment being unavailable date wise.", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("09/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Despite max attempt still allow Lecturer to Test Assignment", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("09/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Possible Feature  – Delete submission.", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("09/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Bug Fix: Duplicate assignments when multiple classes shown", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("09/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Feature where student accounts must be approved before logging in", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("10/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add View & Functionality for admins and lecturers to approve or reject student accounts.", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("10/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Select all checkbox", WeekNo = 2 },

                 //week3
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("13/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Nav Bar link for admins and lecturers", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("13/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Alertify.js package and use it for user button click feedback", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("13/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Set Students created by lecturers/admins to be already approved", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("13/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix number of accounts rejected counter", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("14/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Code Testing/Bug Test", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("14/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Set Back to view Assignments link in PostSubmission to redirect to lecturer’s account if temp student is used", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("14/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up/Research on new feature, Check student answer file with a regex to see if they are using the right methods. (Eg. Assignment about for loop coding, using regex to check if for loop was used).", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("15/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Implement New Feature regex check", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("15/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Implement New Feature regex check", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("16/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Modify grade, sandbox, lecturer controller and student controller", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("16/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Study and test grade and sandbox classes and methods", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("17/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Hit code structure problem, grade and sandbox classes currently only return a decimal, need to think of a way to pass over the value of if regex check", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("17/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Tried using Session storage but HttpContext.Current.Session not defined/does not exist.", WeekNo = 3 },

                 //week4
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("20/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Implement Regex Check Feature; Fix Regex match method expressions", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("20/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Tried using Static Variable in Grade then retrieve in Student Controller, but value changes to false for some reason after a certain method section.", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("20/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Error Hit: Static regexCheck variable in grade class check to false before the check to save the entry into the database.", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("21/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Change return values for sandbox and grade to make Regex Check possible.", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("21/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Change all Boolean variables regexResult to Nullable Booleans", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("21/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Regex Check result into Student Post Submission View", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("21/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Modify Database schema for regex, store regex expressions in a separate regex table instead of storing in assignments.", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("22/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix errors brought up by database change.", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("22/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix ambiguous call between System Regex object and DB Modal Regex DB object", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("22/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix Errors of being unable to save assignment & fix sandbox return value methodology", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("22/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add description column to regular expression database table", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("23/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Bind Add assignment Drop down list to database table data", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("23/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Error Hit: cannot add java (Process.Start, cannot find file specified) and C# assignments.", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("23/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Regex Check throwing error, may be due to java path", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("23/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Bind Add assignment Drop down list to database table data", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("24/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Bug Fix for adding java and C# assignments, null return object for solution and regexCheck method throwing error", WeekNo = 4 },

                 //week5
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("27/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "C# assignment error grading (cause: file path given to regexcheck method in grading has “-cp” infront) (solution: add another regex check to trim file path string)", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("27/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meeting with Mr Karl Kwan to discuss about features/implementation and issues.", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("27/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Java assignment grading error to be continued, check ErrorLog/ErrorNotes", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("28/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix Java assignment grading error (cause: filepath passed to regexcheck method was not in a proper file path format) (solution: use regex and string replace methods to build a proper path)", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("28/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Find back code version without regex check feature, to be published on the production site.", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("28/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Find back old databases schema before regex check feature.", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("28/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on how to do database migration/update production schema.", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("29/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Figure out error caused by submission, Assignment Test Case must have a Description if not will throw error.", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("29/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on database versioning", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("30/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Move approve student link into Manage Class / Student in Lecturer Console.", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("30/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "And Move approve student link into Manage Student in Admin Console", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("30/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Code Testing for errors submitting assignments as Lecturer", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("01/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Approve Student sort by class function ", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("01/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Restructure controller classes", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("01/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Approve Student checkbox onclick still not working/being triggered.", WeekNo = 5 },

                 //week6
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("04/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix Approve Student Checkbox onclick", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("04/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add send Reject Email upon student registration rejection.", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("04/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Approve Student checkbox onclick still not working/being triggered.", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("05/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Back to Lecturer’s Management View to layout view so link persists for temp student", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("05/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add ‘use lowercase’ reminder to AdminNo in Student Registration", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("05/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add ‘Add Lecturer test submission always show 1.", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("05/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Reformat reject email code logic and message.", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("06/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meeting with Mr Low: Progress Update (Refer to Meeting 4)", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("06/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix Error with consolidating database schema and code schema", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("06/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix code side Database mapping issues", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("07/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Class Score Excel Generation: Create Controller function to generate excel document and Figure out how to trigger download excel from controller side", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("08/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Class Score Excel Generation: Redo Excel creation code logic", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("08/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Implement/Add EPPlus to generate advanced formatting excel documents", WeekNo = 6 },

                 //week7
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("11/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Refine/Fix Class Score Excel Generation code logic & excel formatting.", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("11/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Complete Class Score Excel Generation.", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("11/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Set up Assignment on Production for Mr Low class (Practical 2)", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("12/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Edit database schema for suspending student", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("12/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Account controller code to check if student is suspended", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("12/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Create view for suspended student account", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("12/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Explore use of DB version control and LiquiBase", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("13/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Create Suspend student controls for lecturer", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("13/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Edit reject student email message telling student to recreate account ", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("14/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Remove Student Contact No. (Comment out/set default value 0)", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("14/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix Approve Student load for admin console", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("14/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix Suspend Student load for admin console", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("14/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meeting with Mr Low – progress update (refer to Meeting 5 in onenote)", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("15/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Code/Application Testing: Test submission file formats", WeekNo = 7 },

                 //wee8
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("18/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Public Holiday Deepavali", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("19/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Code/Application Testing", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("19/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on web deploy onto IIS and updating of database.", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("19/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Possible Issue: Realise submitted solutions, testcases and student submissions are saved locally so need to copy over files from live site and port over to edited code solution.", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("20/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Backup live site database and code", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("20/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Merge and prep edited code solution for deploy", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("20/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Test making changes and bringing over solutions/testcases/submissions locally", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("20/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Hit Error running script generated from live server SQL DB. Unable to create db.", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("21/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Final Testing of code and backup of code and databases", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("21/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Deployment of code onto live site (SPadeServerV3)", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("21/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Deployment successful but hit error with some functions and permissions", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("21/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Added database user and permission access to new IIS site", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("21/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meeting with Mr Karl Kwan about uploading of solution files from dos to unix and unix to dos. (Deployed code was still not fully working then)", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("22/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meeting with Mr Karl Kwan (refer to meeting 6)", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("22/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix errors thrown by deployed code for permissions and functions", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("22/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Continue testing deployed code", WeekNo = 8 },

                 //week9
                 //new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("25/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Approve all student accounts in prod db (by default was set to 0 when created new approved column)", WeekNo = 9 },
                 //new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("25/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Test deployed code functions", WeekNo = 9 },
                 //new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("25/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Test Create assignment solution and testcase from Mr Karl Kwan. (Test if can receive file with linux line ending)", WeekNo = 9 },
                 //new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("25/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on creation of XML for testcase generation", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("25/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Public Holiday Christmas", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("26/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Auto approve student account if using ichat email. (As requested from Mr Low)", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("26/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on generating xml testcase", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("27/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Create Views for generating xml testcase", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("27/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add and remove testcase inputs in view", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("27/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Set up basic controller functionality", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("27/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on opening link as tab for lecturer to copy and paste xml", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("27/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Might change from post to controller function to javascript function to generate xml", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("28/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Complete View for generating xml testcase", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("28/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Complete View for xml testcase results", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("28/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Test controller logic to send over/handle inputs send over from form to controller", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("29/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Issue hit, each testcase should be able to accept multiple inputs.Need to revamp feature.", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("29/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Edit view allowing multiple input", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("29/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Error hit, after adding another row, add input logic not working because of dom navigation logic, which the default typed in code has extra text child nodes.", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("29/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Bug Fix error tbc, already tried cleaning dom element before navigating, so far not working.", WeekNo = 9 },

                 //week10
                 //new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("01/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Cont. Figure out work around for additional blank text child nodes in DOM causing adding new inputs to go to wrong table cell.", WeekNo = 10 },
                 //new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("01/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Archive and revert code, to prep to continue with other features.", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("02/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Edit database schema, to enable locking of student registration.", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("02/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Prevent students from selecting classes which are locked when registering for an account.", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("03/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix errors on class locking controls.", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("03/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Feature where lecturer can add a new class", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("03/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Test/Fix Class Name input in add new class (try putting in symbol values)", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("04/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix/Touch up class name validation", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("04/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Testing class name in add class", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("05/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add semester and acad year options (dropdown list select) for adding new class.", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("05/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Lec_Class association when lecturer create new class", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("05/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add acad year display to all views with class", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("05/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add acad year controls to update class controls", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("05/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Error with loop logic displaying classes in Manage Assignments, AcadID displaying on another class instead of its own.", WeekNo = 10 },

                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("08/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add acad year display to all views with class and edit all controller methods for to display the views ", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("08/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Above task completed for Lecturer’s controls, Admin not entirely completed yet.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("08/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Application testing.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix prod site student error after trying to confirm email. Debug and check if error was logged.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meeting with Mr Karl Kwan (refer to Meeting 8)", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Create classes and assign to lecturers as per Mr Kwan’s email request.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Push new code to live site. (Up till allow lecturer to create a new class)", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Lock all classes except Mr Kwan’s 8 new classes.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Edit controller to only display associated classes for each lecturer in Lock Class Registration page instead of displaying all classes.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Edit controller to display all students in classes in the module if the lecturer is a module coordinator, for approve/suspend student pages.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("10/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix code logic of if, module coordinator, display all students in classes associated to that module", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("10/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add missing links from admin’s manage students (same ones that are in ManageClassesAndStudents in lecturer controls)", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("10/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Push new code up to live site, has important updates needed before live use.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("10/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Debugging for Karl Kwan’s reported errors.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("10/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Create new account for Karl Kwan, his is supposedly corrupted.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("11/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Debugging for Karl Kwan’s reported errors cont.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("11/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Application testing with IE (Karl Kwan used Internet Explorer on his Windows machine)", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("11/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Check if any errors were logged in the server.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("11/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Had talk with Mr Low, update on progress thus far and what has been going on. Mr Low, mentioned about new feature to allow deletion of older accounts and data to clear space/free up the application.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("11/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Check on error log given by Karl Kwan.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("12/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix prod site errors. Updated/Added old solutions, testcases & submissions.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("12/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix Issues regarding Mr Hubertus and Ms Junie Tan’s access to accounts (Forgot password wasn’t working for them & Emails used were invalid)", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("12/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Debug issues regarding Mr Shek’s student being unable to submit his assignment (AssignmentTitle: Listquestion in class NVWPG2) (Generic error display) (Error tested on offline ver., Infinite loop error)", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("12/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Debug issues of Mr Kwan’s student being unable to submit his assignment (LeftPointingHalfTrigangle) (Generic error display – check in server for message) (Might be same as above error) (Get Submission from student)", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("12/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix errors of students being unable to sign up. (Cause: Student accounts were previously created for testing prob using bulk creation with emails no longer valid) (Solution: Backup & Delete all old DISM submissions & accounts under 1a01-03 & 1b21-22)", WeekNo = 11 },

                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("15/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Test Mr Karl Kwan’s assignment using student’s solution to check error (Test locally and on prod site server for error message) (Answer ends after one wrong input, student’s continues which creates an infinite loop)", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("15/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix error of students still being unable to create accounts (adminno already exists) (Solution: delete accounts in AspNetUsers)", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("15/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix error of auto approve student accounts using ichat email.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("15/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Identified that on prod site student programs which causes infinite loops will throw error and not complete the submission successfully. (Mr Kwan/Mr Shek ‘s student’s error)", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("16/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Handle issue for some of the users.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("16/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix prod site errors. Updated/Added old solutions, testcases & submissions.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("16/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Tested some assignments on prod site to see if working or not. Tested oldest assignment on prod site to check if grading was working again. (Success, not all tested)", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("16/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix student admin issues using SPade", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("16/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Debug infinite loop error", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("16/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Set up another website on server to test the infinite loop error.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("17/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Figure out/set up proper maintenance mode for prod server. Allowing only server admin to access website and disable access to the public. (Added IPAddress and domain restrictions with custom error page)", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("17/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix student admin issues", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("17/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Setup another website on server to test the infinite loop error cont.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("18/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Secondary Test server still throwing errors, cannot log in to accounts. Still cannot use to test errors.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("18/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Debug infinite loop error. Still unable to fix.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("18/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add notice to post submission page, if student gets 0 display notice to check if output/error messages are the same and no infinite loop etc.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("19/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Solve admin issues sent in by students/lecturers", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("19/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Adjust incorrect notification", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("19/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Debug infinite loop error (on local if try to view page without running will throw same error as on the server)", WeekNo = 12 },

                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("22/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Solve admin issues sent in by lecturers/students", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("22/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Successfully Deploy on live.", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("22/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Monitor web server.", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("23/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Help to resolve issue from the system", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("23/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Minor tweak, help a student to retrieve his account", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("23/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Continue monitoring web server", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("24/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Continue monitoring web server", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("24/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Write a documentation for reference in future.", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[0], Date = DateTime.ParseExact("24/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meet Mr Low for final assessment", WeekNo = 13 },

                 //student 2
                //week1
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("30/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Learn about the roles to play", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("30/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Explore application, download and try to run source code", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("30/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Met with previous project manager to handover for source code setup and partial explanation (Error with setup/code for adding java/python assignment)", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("30/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on Hangfire documentation about background processing", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("31/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Setup and try running code with visual studio 2015", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("31/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on Hangfire documentation about background processing cont.", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("31/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Further read up on Hangfire documentation about background methods", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("31/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Understanding on terms about setup/code error (add java/python assignment)", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("31/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Search up on error/Talk to Mr Low about said error. ", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("01/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Setup spare computer for testing", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("01/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on usage of Processes", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("01/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Figure out error (Error adding java/python assignment) (Solved) (Cause: VS Solution not in the same hard drive partition)", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("01/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Discussion with colleagues about code (Code Briefing)", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("01/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Bug Fix Hangfire (Try different versions Hangfire / Strong Key Signing, use package) ", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("02/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Explore and test code and web application for further understanding", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("02/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Bug: During login if admin no letter (e.g. p/s/a) entered is in caps error will be thrown to user.", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("02/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Bug Fixed Hangfire Dashboard Click into Jobs etc (Cause: Hangfire Database error) (Cause: Hangfire Database error) ", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("03/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Figure out errors; Not being able to add python or java due to python not being able to find the upload file in the file path (on local)", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("03/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meeting with Mr Low and Mr Karl Kwan, about changes to be made: Allow lecturer to be able to view assignment as if he was a student and allow Admin/Lecturer to approve a new registration.", WeekNo = 1 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("03/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Prepare for demo; add students to actual database for Mr Karl Kwan for his lesson starting week 1 of the next term. Contact him if no response from him before dateline.", WeekNo = 1 },

                 //week2
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("06/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "add students to actual database for Mr Karl Kwan for his lesson starting week 1 of the next term. Contact him if no response from him before dateline.", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("06/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Begin adding lecturer’s student view. (added controller, views, viewmodels etc. code)", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("07/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add “ghost” student account creation on login as lecturer.", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("07/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Research and test to see which way of coding the feature is better. ", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("07/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Create a “ghost/temp” student account for the lecturer to view it", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("08/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Implement test assignment feature.", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("08/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "'System.InvalidOperationException' in System.dll ('Cannot process request because the process (20496) has exited.') System.InvalidOperationException", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("09/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Allow Lecturer to Test Assignment despite assignment being unavailable date wise.", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("09/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Despite max attempt still allow Lecturer to Test Assignment", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("09/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Possible Feature  – Delete submission.", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("09/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Feature where student accounts must be approved before logging in", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("10/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add View & Functionality for admins and lecturers to approve or reject student accounts.", WeekNo = 2 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("10/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Select all checkbox", WeekNo = 2 },

                 //week3
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("13/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Nav Bar link for admins and lecturers", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("13/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Alertify.js package and use it for user button click feedback", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("13/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Set Students created by lecturers/admins to be already approved", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("13/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix number of accounts rejected counter", WeekNo = 3 },
                 //mc
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("16/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Modify grade, sandbox, lecturer controller and student controller", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("16/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Study and test grade and sandbox classes and methods", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("17/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Hit code structure problem, grade and sandbox classes currently only return a decimal, need to think of a way to pass over the value of if regex check", WeekNo = 3 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("17/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Tried using Session storage but HttpContext.Current.Session not defined/does not exist.", WeekNo = 3 },

                 //week4
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("20/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Implement Regex Check Feature; Fix Regex match method expressions", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("20/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Tried using Static Variable in Grade then retrieve in Student Controller, but value changes to false for some reason after a certain method section.", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("20/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Error Hit: Static regexCheck variable in grade class check to false before the check to save the entry into the database.", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("21/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Change return values for sandbox and grade to make Regex Check possible.", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("21/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Change all Boolean variables regexResult to Nullable Booleans", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("21/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Regex Check result into Student Post Submission View", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("21/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Modify Database schema for regex, store regex expressions in a separate regex table instead of storing in assignments.", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("22/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix errors brought up by database change.", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("22/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix ambiguous call between System Regex object and DB Modal Regex DB object", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("22/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix Errors of being unable to save assignment & fix sandbox return value methodology", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("22/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add description column to regular expression database table", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("23/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Bind Add assignment Drop down list to database table data", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("23/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Error Hit: cannot add java (Process.Start, cannot find file specified) and C# assignments.", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("23/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Regex Check throwing error, may be due to java path", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("23/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Bind Add assignment Drop down list to database table data", WeekNo = 4 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("24/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Bug Fix for adding java and C# assignments, null return object for solution and regexCheck method throwing error", WeekNo = 4 },

                 //week5
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("27/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "C# assignment error grading (cause: file path given to regexcheck method in grading has “-cp” infront) (solution: add another regex check to trim file path string)", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("27/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meeting with Mr Karl Kwan to discuss about features/implementation and issues.", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("27/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Java assignment grading error to be continued, check ErrorLog/ErrorNotes", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("28/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix Java assignment grading error (cause: filepath passed to regexcheck method was not in a proper file path format) (solution: use regex and string replace methods to build a proper path)", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("28/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Find back code version without regex check feature, to be published on the production site.", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("28/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Find back old databases schema before regex check feature.", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("28/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on how to do database migration/update production schema.", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("29/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Figure out error caused by submission, Assignment Test Case must have a Description if not will throw error.", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("29/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on database versioning", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("30/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Move approve student link into Manage Class / Student in Lecturer Console.", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("30/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "And Move approve student link into Manage Student in Admin Console", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("30/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Code Testing for errors submitting assignments as Lecturer", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("01/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Approve Student sort by class function ", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("01/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Restructure controller classes", WeekNo = 5 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("01/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Approve Student checkbox onclick still not working/being triggered.", WeekNo = 5 },

                 //week6
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("04/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix Approve Student Checkbox onclick", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("04/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add send Reject Email upon student registration rejection.", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("04/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Approve Student checkbox onclick still not working/being triggered.", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("05/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Back to Lecturer’s Management View to layout view so link persists for temp student", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("05/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add ‘use lowercase’ reminder to AdminNo in Student Registration", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("05/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add ‘Add Lecturer test submission always show 1.", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("05/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Reformat reject email code logic and message.", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("06/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meeting with Mr Low: Progress Update (Refer to Meeting 4)", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("06/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix Error with consolidating database schema and code schema", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("06/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix code side Database mapping issues", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("07/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Class Score Excel Generation: Create Controller function to generate excel document and Figure out how to trigger download excel from controller side", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("08/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Class Score Excel Generation: Redo Excel creation code logic", WeekNo = 6 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("08/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Implement/Add EPPlus to generate advanced formatting excel documents", WeekNo = 6 },

                 //week7
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("11/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Refine/Fix Class Score Excel Generation code logic & excel formatting.", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("11/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Complete Class Score Excel Generation.", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("11/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Set up Assignment on Production for Mr Low class (Practical 2)", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("12/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Edit database schema for suspending student", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("12/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Account controller code to check if student is suspended", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("12/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Create view for suspended student account", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("12/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Explore use of DB version control and LiquiBase", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("13/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Create Suspend student controls for lecturer", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("13/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Edit reject student email message telling student to recreate account ", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("14/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Remove Student Contact No. (Comment out/set default value 0)", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("14/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix Approve Student load for admin console", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("14/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix Suspend Student load for admin console", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("14/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meeting with Mr Low – progress update (refer to Meeting 5 in onenote)", WeekNo = 7 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("15/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Code/Application Testing: Test submission file formats", WeekNo = 7 },

                 //week8
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("18/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Public Holiday Deepavali", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("19/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Code/Application Testing", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("19/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on web deploy onto IIS and updating of database.", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("19/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Possible Issue: Realise submitted solutions, testcases and student submissions are saved locally so need to copy over files from live site and port over to edited code solution.", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("20/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Backup live site database and code", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("20/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Merge and prep edited code solution for deploy", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("20/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Test making changes and bringing over solutions/testcases/submissions locally", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("20/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Hit Error running script generated from live server SQL DB. Unable to create db.", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("21/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Final Testing of code and backup of code and databases", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("21/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Deployment of code onto live site (SPadeServerV3)", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("21/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Deployment successful but hit error with some functions and permissions", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("21/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Added database user and permission access to new IIS site", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("21/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meeting with Mr Karl Kwan about uploading of solution files from dos to unix and unix to dos. (Deployed code was still not fully working then)", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("22/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meeting with Mr Karl Kwan (refer to meeting 6)", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("22/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix errors thrown by deployed code for permissions and functions", WeekNo = 8 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("22/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Continue testing deployed code", WeekNo = 8 },

                 //week9
                 //new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("25/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Approve all student accounts in prod db (by default was set to 0 when created new approved column)", WeekNo = 9 },
                 //new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("25/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Test deployed code functions", WeekNo = 9 },
                 //new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("25/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Test Create assignment solution and testcase from Mr Karl Kwan. (Test if can receive file with linux line ending)", WeekNo = 9 },
                 //new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("25/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on creation of XML for testcase generation", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("25/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Public Holiday Christmas", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("26/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Auto approve student account if using ichat email. (As requested from Mr Low)", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("26/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on generating xml testcase", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("27/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Create Views for generating xml testcase", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("27/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add and remove testcase inputs in view", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("27/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Set up basic controller functionality", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("27/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Read up on opening link as tab for lecturer to copy and paste xml", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("27/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Might change from post to controller function to javascript function to generate xml", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("28/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Complete View for generating xml testcase", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("28/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Complete View for xml testcase results", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("28/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Test controller logic to send over/handle inputs send over from form to controller", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("29/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Issue hit, each testcase should be able to accept multiple inputs.Need to revamp feature.", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("29/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Edit view allowing multiple input", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("29/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Error hit, after adding another row, add input logic not working because of dom navigation logic, which the default typed in code has extra text child nodes.", WeekNo = 9 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("29/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Bug Fix error tbc, already tried cleaning dom element before navigating, so far not working.", WeekNo = 9 },

                 //week10
                 //new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("01/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Cont. Figure out work around for additional blank text child nodes in DOM causing adding new inputs to go to wrong table cell.", WeekNo = 10 },
                 //new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("01/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Archive and revert code, to prep to continue with other features.", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("02/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Edit database schema, to enable locking of student registration.", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("02/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Prevent students from selecting classes which are locked when registering for an account.", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("03/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix errors on class locking controls.", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("03/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Feature where lecturer can add a new class", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("03/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Test/Fix Class Name input in add new class (try putting in symbol values)", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("04/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix/Touch up class name validation", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("04/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Testing class name in add class", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("05/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add semester and acad year options (dropdown list select) for adding new class.", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("05/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add Lec_Class association when lecturer create new class", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("05/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add acad year display to all views with class", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("05/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add acad year controls to update class controls", WeekNo = 10 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("05/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Error with loop logic displaying classes in Manage Assignments, AcadID displaying on another class instead of its own.", WeekNo = 10 },

                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("08/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add acad year display to all views with class and edit all controller methods for to display the views ", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("08/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Above task completed for Lecturer’s controls, Admin not entirely completed yet.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("08/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Application testing.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix prod site student error after trying to confirm email. Debug and check if error was logged.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meeting with Mr Karl Kwan (refer to Meeting 8)", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Create classes and assign to lecturers as per Mr Kwan’s email request.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Push new code to live site. (Up till allow lecturer to create a new class)", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Lock all classes except Mr Kwan’s 8 new classes.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Edit controller to only display associated classes for each lecturer in Lock Class Registration page instead of displaying all classes.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("09/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Edit controller to display all students in classes in the module if the lecturer is a module coordinator, for approve/suspend student pages.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("10/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix code logic of if, module coordinator, display all students in classes associated to that module", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("10/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add missing links from admin’s manage students (same ones that are in ManageClassesAndStudents in lecturer controls)", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("10/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Push new code up to live site, has important updates needed before live use.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("10/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Debugging for Karl Kwan’s reported errors.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("10/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Create new account for Karl Kwan, his is supposedly corrupted.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("11/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Debugging for Karl Kwan’s reported errors cont.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("11/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Application testing with IE (Karl Kwan used Internet Explorer on his Windows machine)", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("11/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Check if any errors were logged in the server.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("11/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Had talk with Mr Low, update on progress thus far and what has been going on. Mr Low, mentioned about new feature to allow deletion of older accounts and data to clear space/free up the application.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("11/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Check on error log given by Karl Kwan.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("12/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix prod site errors. Updated/Added old solutions, testcases & submissions.", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("12/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix Issues regarding Mr Hubertus and Ms Junie Tan’s access to accounts (Forgot password wasn’t working for them & Emails used were invalid)", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("12/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Debug issues regarding Mr Shek’s student being unable to submit his assignment (AssignmentTitle: Listquestion in class NVWPG2) (Generic error display) (Error tested on offline ver., Infinite loop error)", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("12/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Debug issues of Mr Kwan’s student being unable to submit his assignment (LeftPointingHalfTrigangle) (Generic error display – check in server for message) (Might be same as above error) (Get Submission from student)", WeekNo = 11 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("12/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix errors of students being unable to sign up. (Cause: Student accounts were previously created for testing prob using bulk creation with emails no longer valid) (Solution: Backup & Delete all old DISM submissions & accounts under 1a01-03 & 1b21-22)", WeekNo = 11 },

                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("15/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Test Mr Karl Kwan’s assignment using student’s solution to check error (Test locally and on prod site server for error message) (Answer ends after one wrong input, student’s continues which creates an infinite loop)", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("15/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix error of students still being unable to create accounts (adminno already exists) (Solution: delete accounts in AspNetUsers)", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("15/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix error of auto approve student accounts using ichat email.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("15/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Identified that on prod site student programs which causes infinite loops will throw error and not complete the submission successfully. (Mr Kwan/Mr Shek ‘s student’s error)", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("16/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Handle issue for some of the users.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("16/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix prod site errors. Updated/Added old solutions, testcases & submissions.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("16/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Tested some assignments on prod site to see if working or not. Tested oldest assignment on prod site to check if grading was working again. (Success, not all tested)", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("16/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix student admin issues using SPade", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("16/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Debug infinite loop error", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("16/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Set up another website on server to test the infinite loop error.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("17/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Figure out/set up proper maintenance mode for prod server. Allowing only server admin to access website and disable access to the public. (Added IPAddress and domain restrictions with custom error page)", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("17/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Fix student admin issues", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("17/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Setup another website on server to test the infinite loop error cont.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("18/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Secondary Test server still throwing errors, cannot log in to accounts. Still cannot use to test errors.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("18/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Debug infinite loop error. Still unable to fix.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("18/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Add notice to post submission page, if student gets 0 display notice to check if output/error messages are the same and no infinite loop etc.", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("19/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Solve admin issues sent in by students/lecturers", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("19/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Adjust incorrect notification", WeekNo = 12 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("19/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Debug infinite loop error (on local if try to view page without running will throw same error as on the server)", WeekNo = 12 },

                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("22/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Solve admin issues sent in by lecturers/students", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("22/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Successfully Deploy on live.", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("22/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Monitor web server.", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("23/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Help to resolve issue from the system", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("23/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Minor tweak, help a student to retrieve his account", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("23/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Continue monitoring web server", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("24/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Continue monitoring web server", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("24/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Write a documentation for reference in future.", WeekNo = 13 },
                 new Task_Record { MonthRecord = monthObject[1], Date = DateTime.ParseExact("24/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "Meet Mr Low for final assessment", WeekNo = 13 },
            };
            foreach (Task_Record tr in taskObject)
            {
                db.Tasks.Add(tr);
            }
            db.SaveChanges();

            //Create Internship Journal 2
            //var internshipObject2 = new Internship_Record { UserBatch = userBatchObject2, Project = projectObject, LiaisonOfficer = LOUser };
            //db.Internship_Records.Add(internshipObject2);
            //db.SaveChanges();

            //var monthObject2 = new Month_Record { InternshipRecordId = internshipObject.InternshipRecordId };
            //db.Month_Records.Add(monthObject2);
            //db.SaveChanges();
            //var dayObject2 = new Day_Record { Month = monthObject, Date = DateTime.ParseExact("28/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 1, IsPresent = true };
            //db.Day_Records.Add(dayObject2);
            //db.SaveChanges();

            //var taskObject2 = new Task_Record { MonthRecord = monthObject, Date = DateTime.ParseExact("28/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "It is a great day to eat out", WeekNo = 1 };
            //db.Tasks.Add(taskObject2);
            //db.SaveChanges();
        }
    }
}



