using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Internship_Journal.Controllers
{
    public class LOController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Student_Info()
        {
            return View();
        }
        public IActionResult AttendanceLO()
        {
            return View();
        }
        public IActionResult Daily_Task()
        {
            return View();
        }
        public IActionResult MonthlyReflectionLO()
        {
            return View();
        }
        public IActionResult GradingLO()
        {
            return View();
        }
        public IActionResult CompanyChecklistLO()
        {
            return View();
        }
        public IActionResult CompetencyChecklistLO()
        {
            return View();
        }
        public IActionResult Update_ParticularLO()
        {
            return View();
        }
    }
}
