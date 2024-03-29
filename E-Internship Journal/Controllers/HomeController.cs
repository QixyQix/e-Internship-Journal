﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E_Internship_Journal.Models;

namespace E_Internship_Journal.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult ChooseRole(string selected)
        {
            var Configurations = Startup.Configuration;
            Configurations["_g"] = selected;
            if (selected == "SLO")
            {
                return View("~/Views/Home/SLO/Index.cshtml");
            }
            else if (selected == "LO")
            {
                return View("~/Views/Home/LO/Index.cshtml");
            }
            else
            {
                return View("~/Views/Home/Supervisor/Index.cshtml");
            }
        }

        public IActionResult ChangeRole()
        {
            Startup.Configuration["_g"] = "norole";
            return View("~/Views/Home/SLO/chooseRole.cshtml");
        }

        public IActionResult Index(string selected)
        {
            if (((User.IsInRole("SLO") && User.IsInRole("SUPERVISOR") && User.IsInRole("LO")) || (User.IsInRole("SLO") && User.IsInRole("LO")) || (User.IsInRole("SLO") && User.IsInRole("SUPERVISOR")) || (User.IsInRole("SUPERVISOR") && User.IsInRole("LO"))) && (Startup.Configuration["_g"] == "norole"))
            {
                //return ChangeRole(selected);
                if (selected == "SLO" || selected == "LO" || selected == "Supervisor")
                {
                    return ChooseRole(selected);
                }
                else
                {
                    return ChangeRole();
                }
            }
            else
            {
                if (Startup.Configuration["_g"] == "norole")
                {
                    if (User.IsInRole("SUPERVISOR"))
                    {
                        Startup.Configuration["_g"] = "SUPERVISOR";
                    }
                    else if (User.IsInRole("STUDENT"))
                    {
                        Startup.Configuration["_g"] = "STUDENT";
                    }
                    else if (User.IsInRole("SLO"))
                    {
                        Startup.Configuration["_g"] = "SLO";
                    }
                    else if (User.IsInRole("ADMIN"))
                    {
                        Startup.Configuration["_g"] = "ADMIN";
                    }
                    else
                    {
                        Startup.Configuration["_g"] = "LO";
                    }
                }
                    return Homepage();
            }
        }
        //[Authorize(Roles = "Admin")]
        public IActionResult Homepage()
        {
            if (Startup.Configuration["_g"] == "SUPERVISOR")
            {
                return View("~/Views/Home/Supervisor/Index.cshtml");
            }
            else if (Startup.Configuration["_g"] == "STUDENT")
            {
                return View("~/Views/Home/Student/Index.cshtml");
            }
            else if (Startup.Configuration["_g"] == "ADMIN")
            {
                return View("~/Views/Home/Admin/Index.cshtml");
            }
            else if (Startup.Configuration["_g"] == "SLO")
            {
                return View("~/Views/Home/SLO/Index.cshtml");
            }
            else
            {
                return View("~/Views/Home/LO/Index.cshtml");
            }
        }

        public IActionResult Personal()
        {
            if (Startup.Configuration["_g"] == "SUPERVISOR")
            {
                return View("~/Views/Home/Supervisor/personal.cshtml");
            }
            else if (Startup.Configuration["_g"] == "STUDENT")
            {
                return View("~/Views/Home/Student/personal.cshtml");
            }
            else if (Startup.Configuration["_g"] == "ADMIN")
            {
                return View("~/Views/Home/Admin/personal.cshtml");
            }
            else if (Startup.Configuration["_g"] == "SLO")
            {
                return View("~/Views/Home/SLO/personal.cshtml");
            }
            else
            {
                return View("~/Views/Home/LO/personal.cshtml");
            }
        }

        public IActionResult Guide()
        {
            if (Startup.Configuration["_g"] == "SUPERVISOR")
            {
                return View("~/Views/Home/Supervisor/guide.cshtml");
            }
            else if (Startup.Configuration["_g"] == "SLO")
            {
                return View("~/Views/Home/SLO/guide.cshtml");
            }
            else if (Startup.Configuration["_g"] == "LO")
            {
                return View("~/Views/Home/LO/guide.cshtml");
            }
            else
            {
                return View("~/Views/Home/Student/guide.cshtml");
            }
        }

        public IActionResult Grading()
        {
            if (Startup.Configuration["_g"] == "SUPERVISOR")
            {
                return View("~/Views/Home/Supervisor/grading.cshtml");
            }
            else if (Startup.Configuration["_g"] == "STUDENT")
            {
                return View("~/Views/Home/Student/grading.cshtml");
            }
            else if (Startup.Configuration["_g"] == "SLO")
            {
                return View("~/Views/Home/SLO/grading.cshtml");
            }
            else /*if(Startup.Configuration["_g"] == "STUDENT")*/
            {
                return View("~/Views/Home/LO/grading.cshtml");
            }
        }

        public IActionResult Review()
        {
            if (Startup.Configuration["_g"] == "SUPERVISOR")
            {
                return View("~/Views/Home/Supervisor/Review.cshtml");
            }
            else if (Startup.Configuration["_g"] == "STUDENT")
            {
                return View("~/Views/Home/Student/Review.cshtml");
            }
            else /*if(Startup.Configuration["_g"] == "STUDENT")*/
            {
                return View("~/Views/Home/LO/Review.cshtml");
            }
        }

        public IActionResult Reflection_History()
        {
            if (Startup.Configuration["_g"] == "SUPERVISOR")
            {
                return View("~/Views/Home/Supervisor/reflection_history.cshtml");
            }
            else if (Startup.Configuration["_g"] == "STUDENT")
            {
                return View("~/Views/Home/Student/reflection_history.cshtml");
            }
            else /*if(Startup.Configuration["_g"] == "STUDENT")*/
            {
                return View("~/Views/Home/LO/reflection_history.cshtml");
            }
        }

        public IActionResult Attendance_History()
        {
            if (Startup.Configuration["_g"] == "SUPERVISOR")
            {
                return View("~/Views/Home/Supervisor/attendance_history.cshtml");
            }
            else if (Startup.Configuration["_g"] == "STUDENT")
            {
                return View("~/Views/Home/Student/attendance_history.cshtml");
            }
            else /*if(Startup.Configuration["_g"] == "STUDENT")*/
            {
                return View("~/Views/Home/LO/attendance_history.cshtml");
            }
        }

        public IActionResult Competency_History()
        {
            if (Startup.Configuration["_g"] == "SUPERVISOR")
            {
                return View("~/Views/Home/Supervisor/competency_history.cshtml");
            }
            else if (Startup.Configuration["_g"] == "STUDENT")
            {
                return View("~/Views/Home/Student/competency_history.cshtml");
            }
            else if (Startup.Configuration["_g"] == "SLO")
            {
                return View("~/Views/Home/SLO/competency_history.cshtml");
            }
            else /*if(Startup.Configuration["_g"] == "STUDENT")*/
            {
                return View("~/Views/Home/LO/competency_history.cshtml");
            }
        }

        //student
        public IActionResult Attendance_Summary()
        {
            return View("~/Views/Home/Student/attendance_summary.cshtml");
        }
        public IActionResult Day_Task()
        {
            return View("~/Views/Home/Student/day_task.cshtml");
        }
        public IActionResult Daily_Task()
        {
            return View("~/Views/Home/Student/daily_task.cshtml");
        }
        public IActionResult Competency_Checklist()
        {
            return View("~/Views/Home/Student/competency_checklist.cshtml");
        }
        public IActionResult Monthly_Reflection()
        {
            return View("~/Views/Home/Student/monthly_reflection.cshtml");
        }
        public IActionResult View_Student()
        {
            return View("~/Views/Home/LO/view_student.cshtml");
        }

        public IActionResult Student_Journal()
        {
            if (Startup.Configuration["_g"] == "SUPERVISOR")
            {
                return View("~/Views/Home/Supervisor/student_journal.cshtml");
            }
            else
            {
                return View("~/Views/Home/LO/student_journal.cshtml");
            }
        }


        public IActionResult Mentoring_History()
        {
            if (Startup.Configuration["_g"] == "SUPERVISOR")
            {
                return View("~/Views/Home/Supervisor/mentoring_history.cshtml");
            }
            else if (Startup.Configuration["_g"] == "LO")
            {
                return View("~/Views/Home/LO/mentoring_history.cshtml");
            }
            else
            {
                return View("~/Views/Home/Student/mentoring_history.cshtml");
            }
        }

        public IActionResult Student_Info()
        {
            if (Startup.Configuration["_g"] == "SUPERVISOR")
            {
                return View("~/Views/Home/Supervisor/student_info.cshtml");
            }
            else
            {
                return View("~/Views/Home/LO/student_info.cshtml");
            }
        }

        public IActionResult Company_Checklist()
        {
            if (Startup.Configuration["_g"] == "SUPERVISOR")
            {
                return View("~/Views/Home/Supervisor/company_checklist.cshtml");
            }
            else
            {
                return View("~/Views/Home/LO/company_checklist.cshtml");
            }
        }

        public IActionResult Task_History()
        {
            if (Startup.Configuration["_g"] == "SUPERVISOR")
            {
                return View("~/Views/Home/Supervisor/task_history.cshtml");
            }
            else
            {
                return View("~/Views/Home/LO/task_history.cshtml");
            }
        }


        public IActionResult Internship_Survey()
        {
            if (Startup.Configuration["_g"] == "LO")
            {
                return View("~/Views/Home/LO/internship_survey.cshtml");
            }
            else
            {
                return View("~/Views/Home/Student/internship_survey.cshtml");
            }
        }

        //Supervisor

        //LO
        public IActionResult Touchpoint()
        {
            if (Startup.Configuration["_g"] == "LO")
            {
                return View("~/Views/Home/LO/touchpoint.cshtml");
            }
            else
            {
                return View("~/Views/Home/Student/touchpoint.cshtml");
            }
        }
        public IActionResult Final_Grading()
        {
            return View("~/Views/Home/LO/final_grading.cshtml");
        }

        //Student
        public IActionResult Mentoring()
        {
            return View("~/Views/Home/Student/mentoring.cshtml");
        }
        public IActionResult Lock_Day_Task()
        {
            return View("~/Views/Home/Student/lock_day_task.cshtml");
        }

        //SLO
        public IActionResult Edit_Competency()
        {
            return View("~/Views/Home/SLO/edit_competency.cshtml");
        }
        public IActionResult Manage_Competency()
        {
            return View("~/Views/Home/SLO/manage_competency.cshtml");
        }
        public IActionResult View_Competency()
        {
            return View("~/Views/Home/SLO/view_competency.cshtml");
        }
        public IActionResult Manage_Student()
        {
            return View("~/Views/Home/SLO/manage_student.cshtml");
        }
        public IActionResult Manage_Batch()
        {
            return View("~/Views/Home/SLO/manage_batch.cshtml");
        }
        public IActionResult Manage_Grade()
        {
            return View("~/Views/Home/SLO/manage_grade.cshtml");
        }
        public IActionResult Manage_Company()
        {
            return View("~/Views/Home/SLO/manage_company.cshtml");
        }
        public IActionResult Manage_Project()
        {
            return View("~/Views/Home/SLO/manage_project.cshtml");
        }
        public IActionResult Edit_Project()
        {
            return View("~/Views/Home/SLO/edit_project.cshtml");
        }
        public IActionResult Add_Project()
        {
            return View("~/Views/Home/SLO/add_project.cshtml");
        }
        public IActionResult Edit_Company()
        {
            return View("~/Views/Home/SLO/edit_company.cshtml");
        }
        public IActionResult Add_Company()
        {
            return View("~/Views/Home/SLO/add_company.cshtml");
        }
        public IActionResult Edit_Grade()
        {
            return View("~/Views/Home/SLO/edit_grade.cshtml");
        }
        public IActionResult Add_Student()
        {
            return View("~/Views/Home/SLO/add_student.cshtml");
        }
        public IActionResult Edit_Student()
        {
            return View("~/Views/Home/SLO/edit_student.cshtml");
        }

        //Admin
        [Authorize(Roles = "ADMIN")]
        public IActionResult ViewCompany()
        {
            return View("~/Views/Home/Admin/Company.cshtml");
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult CreateCompany()
        {
            return View("~/Views/Home/Admin/Add_Company.cshtml");
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult EditCompany()
        {
            return View("~/Views/Home/Admin/Edit_Company.cshtml");
        }

        [Authorize(Roles = "ADMIN")]
        public IActionResult ViewBatch()
        {
            return View("~/Views/Home/Admin/Batch.cshtml");
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult CreateBatch()
        {
            return View("~/Views/Home/Admin/Add_Batch.cshtml");
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult EditBatch()
        {
            return View("~/Views/Home/Admin/Edit_Batch.cshtml");
        }

        [Authorize(Roles = "ADMIN")]
        public IActionResult ViewProject()
        {
            return View("~/Views/Home/Admin/Project.cshtml");
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult CreateProject()
        {
            return View("~/Views/Home/Admin/Add_Project.cshtml");
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult EditProject()
        {
            return View("~/Views/Home/Admin/Edit_Project.cshtml");
        }
        //public IActionResult EditCompany()
        //{
        //    return View("~/Views/Home/LO/touchpoint.cshtml");
        //}

        [Authorize(Roles = "ADMIN")]
        public IActionResult ViewUser()
        {
            return View("~/Views/Home/Admin/User.cshtml");
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult CreateUser()
        {
            return View("~/Views/Home/Admin/Add_User.cshtml");
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult EditUser()
        {
            return View("~/Views/Home/Admin/Edit_User.cshtml");
        }

        [Authorize(Roles = "ADMIN")]
        public IActionResult ViewCourse()
        {
            return View("~/Views/Home/Admin/Course.cshtml");
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult CreateCourse()
        {
            return View("~/Views/Home/Admin/Add_Course.cshtml");
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult EditCourse()
        {
            return View("~/Views/Home/Admin/Edit_Course.cshtml");
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult GradingLock()
        {
            return View("~/Views/Home/Admin/Grading_Lock.cshtml");
        }
    }
}
