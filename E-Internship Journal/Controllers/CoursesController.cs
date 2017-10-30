using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace E_Internship_Journal.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        // GET: /<controller>/
        public IActionResult Create()
        {
            return View();
        }
        // GET: /Products/Update
        public IActionResult Update()
        {
            return View();
        }
    }
}