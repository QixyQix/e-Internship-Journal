using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace E_Internship_Journal.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var test = User.IsInRole("SUPERVISOR");
            if (User.IsInRole("SUPERVISOR"))
            {
                return View("~/Views/Home/Supervisor/Index.cshtml");
            }
            else
            {
                return View("~/Views/Home/Student/Index.cshtml");
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
        //public IActionResult Manage_Task()
        //{
        //    var test = User.IsInRole("SUPERVISOR");
        //    if (User.IsInRole("SUPERVISOR"))
        //    {
        //        return View("~/Views/Home/Supervisor/edit_task2.cshtml");
        //    }
        //    else /*if(User.IsInRole("STUDENT"))*/
        //    {
        //        return View("~/Views/Home/Student/edit_task2.cshtml");
        //    }
        //}
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
        public IActionResult Add_Task()
        {
            return View();
        }
        public IActionResult Edit_Task()
        {
            return View();
        }
        //public IActionResult Edit_Task2()
        //{
        //    return View();
        //}
        public IActionResult Key_Attendance()
        {
            return View();
        }
        public IActionResult Attendance()
        {
            return View();
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
    }
}
