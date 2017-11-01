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
            return View();
        }
        public IActionResult Index1()
        {
            return View();
        }
        public IActionResult Daily_Task()
        {
            return View();
        }
        public IActionResult Add_Task()
        {
            return View();
        }
        public IActionResult Edit_Task()
        {
            return View();
        }
        public IActionResult Key_Attendance()
        {
            return View();
        }
        public IActionResult Attendance()
        {
            return View();
        }
        public IActionResult Monthly_Reflection()
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
