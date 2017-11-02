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
        public static async void SeedData(ApplicationDbContext db) {
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
            var competencyObject = new Competency { Description = "Able to code using Java/CSS/HTML", TitleDescription="Able to code", Course = courseObject};
            db.Competencies.Add(competencyObject);
            db.SaveChanges();

            //CREATE USERS
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);
            //Admin
            var adminUser = new ApplicationUser { UserName = "ADMIN@TEST.COM", FullName = "Mr Administrator", Email = "ADMIN@TEST.COM", IsEnabled = true};
            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = ph.HashPassword(adminUser, "P@ssw0rd"); //More complex password
            await userManager.CreateAsync(adminUser);
            await userManager.AddToRoleAsync(adminUser, "ADMIN");
            //Student
            var studentUser = new ApplicationUser { UserName = "STUDENT@TEST.COM", FullName = "James Bond", Email = "STUDENT@TEST.COM", Course=courseObject, IsEnabled = true };
            ph = new PasswordHasher<ApplicationUser>();
            studentUser.PasswordHash = ph.HashPassword(studentUser, "P@ssw0rd"); //More complex password
            await userManager.CreateAsync(studentUser);
            await userManager.AddToRoleAsync(studentUser, "STUDENT");
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
            var SLOUser = new ApplicationUser { UserName = "SLO@TEST.COM", FullName = "Teo Seok Ling", Email = "SLO@TEST.COM", Course = courseObject, IsEnabled = true };
            ph = new PasswordHasher<ApplicationUser>();
            SLOUser.PasswordHash = ph.HashPassword(SLOUser, "P@ssw0rd"); //More complex password
            await userManager.CreateAsync(SLOUser);
            await userManager.AddToRoleAsync(SLOUser, "SLO");

            db.SaveChanges();

            //Create Batch
            var batchObject = new Batch { BatchName = "AY1718S2", Description = "Academic Yeat 2017-2018 Semester 2",
                StartDate = DateTime.ParseExact("30/10/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                EndDate = DateTime.ParseExact("30/12/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                Course = courseObject
            };
            db.Batches.Add(batchObject);

            db.SaveChanges();

            //Create Company
            var companyObject = new Company { CompanyName = "Some Weird Company", CompanyAddress = "BLK999# 03-03 HELL STREET SINGAPORE 666666" };
            db.Companies.Add(companyObject);
            db.SaveChanges();

            //Create Project
            var projectObject = new Project { ProjectName = "Online awesome website management system", Company=companyObject, Supervisor=supervisorUser};
            db.Projects.Add(projectObject);
            db.SaveChanges();

            //Create UserBatch
            var userBatchObject = new UserBatch { Batch = batchObject, User = studentUser };
            db.UserBatches.Add(userBatchObject);
            db.SaveChanges();

            //Create Internship Journal
            var internshipObject = new Internship_Record { UserBatch = userBatchObject, Project = projectObject, LiaisonOfficer = LOUser };
            db.Internship_Records.Add(internshipObject);
            db.SaveChanges();


            //Save changes
            db.SaveChanges();
        }
    }
}
