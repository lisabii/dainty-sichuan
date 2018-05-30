using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Huihuibao.Data;
using Huihuibao.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Huihuibao.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public StaffController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Check()
        {
            var userId = _userManager.GetUserId(User);
            var applicationDbContext = _context.TimeTables.Include(t => t.ApplicationUser).Where(t => t.ApplicationUserId == userId);
            DateTime today = DateTime.Today;
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
            DateTime nextMonday = today.AddDays(daysUntilMonday);
            DateTime nextSunday = today.AddDays(daysUntilMonday + 6);

            var DBlist = await applicationDbContext.ToListAsync();
            var TTList = new List<TimeTable>();
            for(int i = 0; i< 7; i++)
            {
                var date = nextMonday.AddDays(i);
                var itemInDb = DBlist.SingleOrDefault(t => t.DateTime.Equals(date));
                if (itemInDb == null)
                {
                    TTList.Add(new TimeTable() { ApplicationUserId = userId, DateTime = date });
                }
                else
                {
                    TTList.Add(itemInDb);
                }
            }

            return View(TTList);
        }

        [HttpPost]
        public async Task<IActionResult> Check(List<TimeTable> times)
        {
            var userId = _userManager.GetUserId(User);
            foreach (var item in times)
            {
                _context.Update(item);
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}