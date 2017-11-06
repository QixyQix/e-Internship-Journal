using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace E_Internship_Journal.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Add_Company()
        {
            return View();
        }
        public IActionResult Batch()
        {
            return View();
        }
        public IActionResult Add_Batch()
        {
            return View();
        }
        public IActionResult Company()
        {
            return View();
        }
        public IActionResult Edit_Batch()
        {
            return View();
        }
    }
}