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

            var studentRole = new IdentityRole { Name = "STUDENT" };
            await identityRoleManager.CreateAsync(studentRole);

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

            //Create Competency
            var competencyObject = new Competency[] {
                new Competency { Description = "Able to code using Java/CSS/HTML", TitleDescription = "Able to code", Course = courseObject[1] },
                new Competency { Description = "Able to code using Java/CSS/HTML", TitleDescription = "Able to code", Course = courseObject[1] },
                new Competency { Description = "Able to code using Java/CSS/HTML", TitleDescription = "Able to code", Course = courseObject[1] }
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
            var adminUser = new ApplicationUser { UserName = "ADMIN@TEST.COM", FullName = "Mr Administrator", Email = "ADMIN@TEST.COM", IsEnabled = true };
            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = ph.HashPassword(adminUser, "P@ssw0rd"); //More complex password
            await userManager.CreateAsync(adminUser);
            await userManager.AddToRoleAsync(adminUser, "ADMIN");

            //Student
            var studentUser = new ApplicationUser[] {
                new ApplicationUser { UserName = "P1551581@TEST.COM", FullName = "Ong Boon Ping, Klein", Email = "P1551581@TEST.COM", /*Course=courseObject,*/ IsEnabled = true },
                new ApplicationUser { UserName = "P1530247@TEST.COM", FullName = "Amelia Goh Jia Pei", Email = "P1530247@TEST.COM", /*Course=courseObject,*/ IsEnabled = true },
                new ApplicationUser { UserName = "P1551635@TEST.COM", FullName = "Loo Jia Wei", Email = "P1551635@TEST.COM", /*Course=courseObject,*/ IsEnabled = true },
                new ApplicationUser { UserName = "P1551677@TEST.COM", FullName = "Tan Chia Wei", Email = "P1551677@TEST.COM", /*Course=courseObject,*/ IsEnabled = true },
                new ApplicationUser { UserName = "P1530078@TEST.COM", FullName = "Cho Qi Xiang", Email = "P1530078@TEST.COM", /*Course=courseObject,*/ IsEnabled = true },
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
                new ApplicationUser { UserName = "SUPERVISOR@TEST.COM", FullName = "Santhosh Kumar", Email = "SUPERVISOR@TEST.COM", IsEnabled = true },
                new ApplicationUser { UserName = "JOHNNYLIM@COMPANY.COM", FullName = "Johnny Lim", Email = "JOHNNYLIM@COMPANY.COM", IsEnabled = true }
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
                new ApplicationUser { UserName = "LO@TEST.COM", FullName = "Leong Fong Sow", Email = "LO@TEST.COM", IsEnabled = true }
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
                new Batch { BatchName = "AY1718S1", Description = "Academic Yeat 2017-2018 Semester 1", StartDate = DateTime.ParseExact("14/06/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/09/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[0] },
                new Batch { BatchName = "AY1718S2", Description = "Academic Yeat 2017-2018 Semester 2", StartDate = DateTime.ParseExact("28/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("28/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[0] },
                new Batch { BatchName = "AY1718S1", Description = "Academic Yeat 2017-2018 Semester 1", StartDate = DateTime.ParseExact("14/06/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/09/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[1] },
                new Batch { BatchName = "AY1718S2", Description = "Academic Yeat 2017-2018 Semester 2", StartDate = DateTime.ParseExact("28/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("28/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[1] },
                new Batch { BatchName = "AY1718S1", Description = "Academic Yeat 2017-2018 Semester 1", StartDate = DateTime.ParseExact("14/02/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/08/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[2] },
                new Batch { BatchName = "AY1718S2", Description = "Academic Yeat 2017-2018 Semester 2", StartDate = DateTime.ParseExact("28/09/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("28/02/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[2] },
                new Batch { BatchName = "AY1617S1", Description = "Academic Yeat 2016-2017 Semester 1", StartDate = DateTime.ParseExact("14/06/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/09/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[0] },
                new Batch { BatchName = "AY1617S2", Description = "Academic Yeat 2016-2017 Semester 2", StartDate = DateTime.ParseExact("28/10/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("28/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[0] },
                new Batch { BatchName = "AY1617S1", Description = "Academic Yeat 2016-2017 Semester 1", StartDate = DateTime.ParseExact("14/06/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/09/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[1] },
                new Batch { BatchName = "AY1617S2", Description = "Academic Yeat 2016-2017 Semester 2", StartDate = DateTime.ParseExact("28/10/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("28/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[1] },
                new Batch { BatchName = "AY1617S1", Description = "Academic Yeat 2016-2017 Semester 1", StartDate = DateTime.ParseExact("14/02/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/08/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[2] },
                new Batch { BatchName = "AY1617S2", Description = "Academic Yeat 2016-2017 Semester 2", StartDate = DateTime.ParseExact("28/09/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("28/02/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[2] },
                new Batch { BatchName = "AY1516S1", Description = "Academic Yeat 2015-2016 Semester 1", StartDate = DateTime.ParseExact("14/06/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/09/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[0] },
                new Batch { BatchName = "AY1516S2", Description = "Academic Yeat 2015-2016 Semester 2", StartDate = DateTime.ParseExact("28/10/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("28/01/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[0] },
                new Batch { BatchName = "AY1516S1", Description = "Academic Yeat 2015-2016 Semester 1", StartDate = DateTime.ParseExact("14/02/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("15/08/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[2] },
                new Batch { BatchName = "AY1516S2", Description = "Academic Yeat 2015-2016 Semester 2", StartDate = DateTime.ParseExact("28/09/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture), EndDate = DateTime.ParseExact("28/02/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture), Course = courseObject[2] },
            };
            foreach (Batch b in batchObject)
            {
                db.Batches.Add(b);
            }
            db.SaveChanges();


            //Create Company
            var companyObject = new Company[]
            {
                new Company { CompanyName = "Some Weird Company", CompanyAddress = "BLK999# 03-03 PUNGGOL STREET SINGAPORE 666999", ContactPersonName = "Company Owner Name", ContactPersonNumber = "91239240", ContactPersonEmail = "CEO@COMPANY.COM", ContactPersonFax = "85001000" },
                new Company { CompanyName = "Convertium pte ltd", CompanyAddress = "BLK999# 03-03 PUNGGOL STREET SINGAPORE 666999", ContactPersonName = "Johnny Lim", ContactPersonNumber = "98988899", ContactPersonEmail = "JOHNNYLIM@COMPANY.COM", ContactPersonFax = "85858855" },
                new Company { CompanyName = "Some Noob Company", CompanyAddress = "BLK213# 03-03 BISHAN STREET SINGAPORE 540213", ContactPersonName = "Lee Noob Siao", ContactPersonNumber = "99955522", ContactPersonEmail = "CEO@NOOBSIAO.COM", ContactPersonFax = "85252000" }
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
                new Project {ProjectName = "Online awesome website management system", Company = companyObject[0], Supervisor = supervisorUser[0] },
                new Project {ProjectName = "New awesome website management system", Company = companyObject[2], Supervisor = supervisorUser[0] },
                new Project {ProjectName = "Headless CMS website management system", Company = companyObject[1], Supervisor = supervisorUser[1] }
            };
            foreach (Project p in projectObject)
            {
                db.Projects.Add(p);
            }
            db.SaveChanges();

            //Create UserBatch
            var userBatchObject = new UserBatch[] {
            new UserBatch { Batch = batchObject[0], User = studentUser[0] },
            new UserBatch { Batch = batchObject[0], User = studentUser[1] },
            new UserBatch { Batch = batchObject[0], User = studentUser[2] },
            new UserBatch { Batch = batchObject[0], User = studentUser[3] },
            new UserBatch { Batch = batchObject[0], User = studentUser[4] },
            new UserBatch { Batch = batchObject[1], User = studentUser[5] },
            new UserBatch { Batch = batchObject[0], User = SLOUser[0] }
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
            new Internship_Record{ UserBatch = userBatchObject[1], Project = projectObject[0], LiaisonOfficer = LOUser[0] },
            new Internship_Record{ UserBatch = userBatchObject[2], Project = projectObject[1], LiaisonOfficer = LOUser[0] }
            };
            foreach (Internship_Record ir in internshipObject)
            {
                db.Internship_Records.Add(ir);
            }
            db.SaveChanges();

            var monthObject = new Month_Record { InternshipRecordId = internshipObject[0].InternshipRecordId };
            db.Month_Records.Add(monthObject);
            db.SaveChanges();

            var dayObject = new Day_Record { Month = monthObject, Date = DateTime.ParseExact("01/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), ArrivalTime = DateTime.Parse("9:00:00 AM"), DepartureTime = DateTime.Parse("6:00:00 PM"), WeekNo = 1, IsPresent = true };
            db.Day_Records.Add(dayObject);
            db.SaveChanges();

            var taskObject = new Task_Record { MonthRecord = monthObject, Date = DateTime.ParseExact("02/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), Description = "I enjoy working", WeekNo = 1 };
            db.Tasks.Add(taskObject);
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
