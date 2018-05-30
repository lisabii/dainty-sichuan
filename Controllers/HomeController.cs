using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Huihuibao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Huihuibao.Data;

namespace Huihuibao.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ApplicationDbContext _context;
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;

        //public HomeController(ApplicationDbContext context,
        //    UserManager<ApplicationUser> userManager,
        //    SignInManager<ApplicationUser> signInManager)
        //{
        //    _context = context;
        //    _userManager = userManager;
        //    _signInManager = signInManager;
        //}

        public IActionResult Index()
        {
            if (User.IsInRole("Administrator"))
            {
                return RedirectToAction(nameof(AdminController.Index), "Admin");
            }
            else if (User.IsInRole("Staff"))
            {
                return RedirectToAction(nameof(StaffController.Index), "Staff");
            }
            return View();
        }

        [Authorize( Roles ="Administrator")]
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
