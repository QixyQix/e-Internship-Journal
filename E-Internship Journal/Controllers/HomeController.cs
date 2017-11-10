﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace E_Internship_Journal.Controllers
{
    public class HomeController : Controller
    {
        //[Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var test = User.IsInRole("SUPERVISOR");
            if (User.IsInRole("SUPERVISOR"))
            {
                return View("~/Views/Home/Supervisor/Index.cshtml");
            }
            else if (User.IsInRole("STUDENT"))
            {
                return View("~/Views/Home/Student/Index.cshtml");
            }
            else if (User.IsInRole("LO"))
            {
                return View("~/Views/Home/LO/Index.cshtml");
            }
            else if (User.IsInRole("SLO"))
            {
                return View("~/Views/Home/SLO/Index.cshtml");
            }
            else
            {
                return View("~/Views/Home/Admin/Index.cshtml");
            }
        }
        
        public IActionResult Grading()
        {
            var test = User.IsInRole("SUPERVISOR");
            if (User.IsInRole("SUPERVISOR"))
            {
                return View("~/Views/Home/Supervisor/grading.cshtml");
            }
            else /*if(User.IsInRole("STUDENT"))*/
            {
                return View("~/Views/Home/Student/grading.cshtml");
            }
        }
            public IActionResult Attendance_Summary()
        {
            var test = User.IsInRole("SUPERVISOR");
            if (User.IsInRole("SUPERVISOR"))
            {
                return View("~/Views/Home/Supervisor/attendance_summary.cshtml");
            }
            else /*if(User.IsInRole("STUDENT"))*/
            {
                return View("~/Views/Home/Student/attendance_summary.cshtml");
            }
        }
        public IActionResult Day_Task()
        {
            var test = User.IsInRole("SUPERVISOR");
            if (User.IsInRole("SUPERVISOR"))
            {
                return View("~/Views/Home/Supervisor/day_task.cshtml");
            }
            else /*if(User.IsInRole("STUDENT"))*/
            {
                return View("~/Views/Home/Student/day_task.cshtml");
            }
        }
        public IActionResult Daily_Task()
        {
            var test = User.IsInRole("SUPERVISOR");
            if (User.IsInRole("SUPERVISOR"))
            {
                return View("~/Views/Home/Supervisor/daily_task.cshtml");
            }
            else /*if(User.IsInRole("STUDENT"))*/
            {
                return View("~/Views/Home/Student/daily_task.cshtml");
            }
        }
        public IActionResult Competency_Checklist()
        {
            var test = User.IsInRole("SUPERVISOR");
            if (User.IsInRole("SUPERVISOR"))
            {
                return View("~/Views/Home/Supervisor/competency_checklist.cshtml");
            }
            else /*if(User.IsInRole("STUDENT"))*/
            {
                return View("~/Views/Home/Student/competency_checklist.cshtml");
            }
        }
        public IActionResult Monthly_Reflection()
        {
            var test = User.IsInRole("SUPERVISOR");
            if (User.IsInRole("SUPERVISOR"))
            {
                return View("~/Views/Home/Supervisor/monthly_reflection.cshtml");
            }
            else /*if(User.IsInRole("STUDENT"))*/
            {
                return View("~/Views/Home/Student/monthly_reflection.cshtml");
            }
        }
        public IActionResult Guide()
        {
            var test = User.IsInRole("SUPERVISOR");
            if (User.IsInRole("SUPERVISOR"))
            {
                return View("~/Views/Home/Supervisor/guide.cshtml");
            }
            else /*if(User.IsInRole("STUDENT"))*/
            {
                return View("~/Views/Home/Student/guide.cshtml");
            }
        }
        public IActionResult Review()
        {
            return View("~/Views/Home/Supervisor/Review.cshtml");
        }
        public IActionResult Personal()
        {
            var test = User.IsInRole("SUPERVISOR");
            if (User.IsInRole("SUPERVISOR"))
            {
                return View("~/Views/Home/Supervisor/personal.cshtml");
            }
            else /*if(User.IsInRole("STUDENT"))*/
            {
                return View("~/Views/Home/Student/personal.cshtml");
            }
        }
        public IActionResult Edit_Competency()
        {
            var test = User.IsInRole("SUPERVISOR");
            if (User.IsInRole("SUPERVISOR"))
            {
                return View("~/Views/Home/Supervisor/edit_competency.cshtml");
            }
            else if (User.IsInRole("STUDENT"))
            {
                return View("~/Views/Home/Student/edit_competency.cshtml");
            }else /*if(User.IsInRole("STUDENT"))*/
            {
                return View("~/Views/Home/SLO/edit_competency.cshtml");
            }
        }
        public IActionResult View_Student()
        {
            var test = User.IsInRole("SUPERVISOR");
            if (User.IsInRole("SUPERVISOR"))
            {
                return View("~/Views/Home/Supervisor/view_student.cshtml");
            }
            else if (User.IsInRole("STUDENT"))
            {
                return View("~/Views/Home/Student/view_student.cshtml");
            }
            else if (User.IsInRole("STUDENT"))
            {
                return View("~/Views/Home/SLO/view_student.cshtml");
            }
            else /*if(User.IsInRole("STUDENT"))*/
            {
                return View("~/Views/Home/SLO/view_student.cshtml");
            }
        }
        
        public IActionResult Reflection_History()
        {
            if (User.IsInRole("SUPERVISOR"))
            {
                return View("~/Views/Home/Supervisor/reflection_history.cshtml");
            }
            else if (User.IsInRole("STUDENT"))
            {
                return View("~/Views/Home/Student/reflection_history.cshtml");
            }
            else if (User.IsInRole("SLO"))
            {
                return View("~/Views/Home/SLO/reflection_history.cshtml");
            }
            else /*if(User.IsInRole("STUDENT"))*/
            {
                return View("~/Views/Home/LO/reflection_history.cshtml");
            }
        }
        public IActionResult Attendance_History()
        {
            if (User.IsInRole("SUPERVISOR"))
            {
                return View("~/Views/Home/Supervisor/attendance_history.cshtml");
            }
            else if (User.IsInRole("STUDENT"))
            {
                return View("~/Views/Home/Student/attendance_history.cshtml");
            }
            else if (User.IsInRole("SLO"))
            {
                return View("~/Views/Home/SLO/attendance_history.cshtml");
            }
            else /*if(User.IsInRole("STUDENT"))*/
            {
                return View("~/Views/Home/LO/attendance_history.cshtml");
            }
        }
        public IActionResult Competency_History()
        {
            if (User.IsInRole("SUPERVISOR"))
            {
                return View("~/Views/Home/Supervisor/competency_history.cshtml");
            }
            else if (User.IsInRole("STUDENT"))
            {
                return View("~/Views/Home/Student/competency_history.cshtml");
            }
            else if (User.IsInRole("SLO"))
            {
                return View("~/Views/Home/SLO/competency_history.cshtml");
            }
            else /*if(User.IsInRole("STUDENT"))*/
            {
                return View("~/Views/Home/LO/competency_history.cshtml");
            }
        }


        //Student

        //Supervisor
        public IActionResult Student_Journal()
        {
            return View("~/Views/Home/Supervisor/student_journal.cshtml");
        }
        public IActionResult Student_Info()
        {
            return View("~/Views/Home/Supervisor/student_info.cshtml");
        }
        public IActionResult Company_Checklist()
        {
            return View("~/Views/Home/Supervisor/company_checklist.cshtml");
        }
        public IActionResult Task_History()
        {
            return View("~/Views/Home/Supervisor/task_history.cshtml");
        }

        //LO
        public IActionResult Touchpoint()
        {
            return View("~/Views/Home/LO/touchpoint.cshtml");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }


        //Admin
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
        public IActionResult ViewBatch()
        {
            return View("~/Views/Home/Admin/Batch.cshtml");
        }

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
        public IActionResult EditCompany()
        {
            return View("~/Views/Home/LO/touchpoint.cshtml");
        }

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


    }
}
