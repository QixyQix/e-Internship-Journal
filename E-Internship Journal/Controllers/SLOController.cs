﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Internship_Journal.Controllers
{
    public class SLOController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult BatchInformation()
        {
            return View();
        }
        public IActionResult ViewBatchInfo()
        {
            return View();
        }
        public IActionResult EditBatchInfo()
        {
            return View();
        }
    }
}