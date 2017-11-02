using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace E_Internship_Journal.Controllers
{
    public class SupervisorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Attendance()
        {
            return View();
        }
        public IActionResult View_Task()
        {
            return View();
        }
        public IActionResult Grading()
        {
            return View();
        }
        public IActionResult Monthly_Reflection()
        {
            return View();
        }
    }
}
