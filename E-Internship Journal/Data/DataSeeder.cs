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
            var courseObject = new Course { CourseCode = "DIT", CourseName = "Diploma in Information Technology" };
            db.Courses.Add(courseObject);
            db.SaveChanges();

            //Create Competency
            var competencyObject = new Competency { Description = "Able to code using Java/CSS/HTML", TitleDescription = "Able to code", Course = courseObject };
            db.Competencies.Add(competencyObject);
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
            var studentUser = new ApplicationUser { UserName = "STUDENT@TEST.COM", FullName = "James Bond", Email = "STUDENT@TEST.COM", /*Course=courseObject,*/ IsEnabled = true };
            ph = new PasswordHasher<ApplicationUser>();
            studentUser.PasswordHash = ph.HashPassword(studentUser, "P@ssw0rd"); //More complex password
            await userManager.CreateAsync(studentUser);
            await userManager.AddToRoleAsync(studentUser, "STUDENT");

            var studentUser2 = new ApplicationUser { UserName = "TestStudent@TEST.COM", FullName = "For Testing", Email = "TestStudent@TEST.COM", /*Course=courseObject,*/ IsEnabled = true };
            ph = new PasswordHasher<ApplicationUser>();
            studentUser2.PasswordHash = ph.HashPassword(studentUser2, "P@ssw0rd"); //More complex password
            await userManager.CreateAsync(studentUser2);
            await userManager.AddToRoleAsync(studentUser2, "STUDENT");
            //Supervisor
            var supervisorUser = new ApplicationUser { UserName = "SUPERVISOR@TEST.COM", FullName = "Santhosh Kumar", Email = "SUPERVISOR@TEST.COM", IsEnabled = true };
            ph = new PasswordHasher<ApplicationUser>();
            supervisorUser.PasswordHash = ph.HashPassword(supervisorUser, "P@ssw0rd"); //More complex password
            await userManager.CreateAsync(supervisorUser);
            await userManager.AddToRoleAsync(supervisorUser, "SUPERVISOR");
            //LO
            var LOUser = new ApplicationUser { UserName = "LO@TEST.COM", FullName = "Leong Fong Sow", Email = "LO@TEST.COM", IsEnabled = true };
            ph = new PasswordHasher<ApplicationUser>();
            LOUser.PasswordHash = ph.HashPassword(LOUser, "P@ssw0rd"); //More complex password
            await userManager.CreateAsync(LOUser);
            await userManager.AddToRoleAsync(LOUser, "LO");
            //SLO
            var SLOUser = new ApplicationUser { UserName = "SLO@TEST.COM", FullName = "Teo Seok Ling", Email = "SLO@TEST.COM", /*Course = courseObject, */ IsEnabled = true };
            ph = new PasswordHasher<ApplicationUser>();
            SLOUser.PasswordHash = ph.HashPassword(SLOUser, "P@ssw0rd"); //More complex password
            await userManager.CreateAsync(SLOUser);
            await userManager.AddToRoleAsync(SLOUser, "SLO");

            db.SaveChanges();

            //Create Batch
            var batchObject = new Batch { BatchName = "AY1718S2", Description = "Academic Yeat 2017-2018 Semester 2",
                StartDate = DateTime.ParseExact("30/08/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                EndDate = DateTime.ParseExact("31/03/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                Course = courseObject
            };
            db.Batches.Add(batchObject);

            db.SaveChanges();

            //Create Company
            var companyObject = new Company { CompanyName = "Some Weird Company", CompanyAddress = "BLK999# 03-03 PUNGGOL STREET SINGAPORE 666666", ContactPersonName = "Company Owner Name", ContactPersonNumber = "91239240", ContactPersonEmail = "CEO@COMPANY.COM", ContactPersonFax = "85001000" };
            db.Companies.Add(companyObject);
            db.SaveChanges();

            //Create Project
            var projectObject = new Project { ProjectName = "Online awesome website management system", Company = companyObject, Supervisor = supervisorUser };
            db.Projects.Add(projectObject);
            db.SaveChanges();

            //Create UserBatch
            var userBatchObject = new UserBatch { Batch = batchObject, User = studentUser };
            db.UserBatches.Add(userBatchObject);

            db.SaveChanges();
            var userSLOUserBatchObject = new UserBatch { Batch = batchObject, User = SLOUser };
            db.UserBatches.Add(userSLOUserBatchObject);

            db.SaveChanges();

            var userBatchObject2 = new UserBatch { Batch = batchObject, User = studentUser2 };
            db.UserBatches.Add(userBatchObject2);

            db.SaveChanges();

            //Create Internship Journal
            var internshipObject = new Internship_Record { UserBatch = userBatchObject, Project = projectObject, LiaisonOfficer = LOUser };
            db.Internship_Records.Add(internshipObject);
            db.SaveChanges();

            var monthObject = new Month_Record { InternshipRecordId = internshipObject.InternshipRecordId };
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
