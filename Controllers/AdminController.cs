using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Huihuibao.Data;
using Huihuibao.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Huihuibao.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Admin
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Assign()
        {
            List<List<int>> listOfNextWeekAppplied = new List<List<int>>();
            List<List<int>> listOfNextWeekNotApplied = new List<List<int>>();

            DateTime today = DateTime.Today;
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
            DateTime nextMonday = today.AddDays(daysUntilMonday);
            List<DateTime> dates = new List<DateTime>();

            var timetable = await _context.TimeTables.Include(x => x.ApplicationUser).Where(x => x.DateTime <= nextMonday.AddDays(7) && x.DateTime >= nextMonday).ToListAsync();

            for (int i =0; i< 7; i++)
            {
                dates.Add(nextMonday.AddDays(i));
                var query1 = from item in timetable
                            where item.DateTime == nextMonday.AddDays(i) && item.Applied == true 
                            orderby item.ApplicationUser.UserName
                            select item.TimeTableId;
                var query2 = from item in timetable
                            where item.DateTime == nextMonday.AddDays(i) && item.Applied == false
                            orderby item.ApplicationUser.UserName
                            select item.TimeTableId;
                listOfNextWeekAppplied.Add(query1.ToList());
                listOfNextWeekNotApplied.Add(query2.ToList());
            }

            List<ApplicationUser> absent = new List<ApplicationUser>();

            foreach(var item in _userManager.Users)
            {
                if( !(timetable.Exists(x => x.ApplicationUser.Id == item.Id) || await _userManager.IsInRoleAsync(item, "Administrator")))
                {
                    absent.Add(item);
                }
            }

            ViewBag.Dates = dates;
            ViewData["Applied"] = listOfNextWeekAppplied;
            ViewData["NotApplied"] = listOfNextWeekNotApplied;
            ViewData["Absent"] = absent;

            return View(timetable);
        }

        [HttpPost]
        public IActionResult Assign(List<TimeTable> times)
        {
            foreach (var item in times)
            {
                _context.Update(item);
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Check()
        {
            List<List<int>> listOfNextWeekAppplied = new List<List<int>>();
            List<List<int>> listOfNextWeekNotApplied = new List<List<int>>();

            DateTime today = DateTime.Today;
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysUntilMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday) % 7;
            DateTime thisMonday = today.AddDays( - daysUntilMonday);
            List<DateTime> dates = new List<DateTime>();

            var timetable = await _context.TimeTables.Include(x => x.ApplicationUser).Where(x => x.DateTime <= thisMonday.AddDays(6) && x.DateTime >= thisMonday).ToListAsync();

            for (int i = 0; i < 7; i++)
            {
                dates.Add(thisMonday.AddDays(i));
                var query1 = from item in timetable
                             where item.DateTime == thisMonday.AddDays(i) && item.Applied == true
                             orderby item.ApplicationUser.UserName
                             select item.TimeTableId;
                var query2 = from item in timetable
                             where item.DateTime == thisMonday.AddDays(i) && item.Applied == false
                             orderby item.ApplicationUser.UserName
                             select item.TimeTableId;
                listOfNextWeekAppplied.Add(query1.ToList());
                listOfNextWeekNotApplied.Add(query2.ToList());
            }

            List<ApplicationUser> absent = new List<ApplicationUser>();

            foreach (var item in _userManager.Users)
            {
                if (!(timetable.Exists(x => x.ApplicationUser.Id == item.Id) || await _userManager.IsInRoleAsync(item, "Administrator")))
                {
                    absent.Add(item);
                }
            }

            ViewBag.Dates = dates;
            ViewData["Applied"] = listOfNextWeekAppplied;
            ViewData["NotApplied"] = listOfNextWeekNotApplied;
            ViewData["Absent"] = absent;

            return View(timetable);
        }

        public async Task<IActionResult> Manage()
        {
            
            return View(await _userManager.GetUsersInRoleAsync("Staff"));
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeTable = await _context.TimeTables
                .Include(t => t.ApplicationUser)
                .SingleOrDefaultAsync(m => m.TimeTableId == id);
            if (timeTable == null)
            {
                return NotFound();
            }

            return View(timeTable);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TimeTableId,DateTime,ApplicationUserId,Applied,Assigned")] TimeTable timeTable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(timeTable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", timeTable.ApplicationUserId);
            return View(timeTable);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeTable = await _context.TimeTables.SingleOrDefaultAsync(m => m.TimeTableId == id);
            if (timeTable == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", timeTable.ApplicationUserId);
            return View(timeTable);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TimeTableId,DateTime,ApplicationUserId,Applied,Assigned")] TimeTable timeTable)
        {
            if (id != timeTable.TimeTableId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timeTable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimeTableExists(timeTable.TimeTableId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", timeTable.ApplicationUserId);
            return View(timeTable);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var query = _context.TimeTables.Include(t => t.ApplicationUser).Where(t => t.ApplicationUser.Id == id);
            _context.RemoveRange(query);
            _context.SaveChanges();

            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Manage));
        }

        private bool TimeTableExists(int id)
        {
            return _context.TimeTables.Any(e => e.TimeTableId == id);
        }
    }
}
