using GD.Practical.Database;
using GD.Practical.Extension;
using GD.Practical.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GD.Practical.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly TimetableDbContext _context;

        [BindProperty]
        public Subject NewSubject { get; set; }

        public HomeController(ILogger<HomeController> logger, TimetableDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        #region School Config 

        public IActionResult Index()
        {
            _context.Subjects.RemoveRange(_context.Subjects);
            _context.SaveChanges();

            return View();
        }

        public IActionResult ConfigForm(SchoolConfig model)
        {
            _context.SchoolConfig.RemoveRange(_context.SchoolConfig);
            _context.SchoolConfig.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Subject");
        }

        #endregion School Config

        #region Subject Form

        public IActionResult Subject()
        {
            var config = _context.SchoolConfig.FirstOrDefault();

            if (config == null)
            {
                return RedirectToAction("Index");
            }

            @ViewData["LimitMessage"] = string.Format("Please ensure that {0} hours are assigned in order to finalize the timetable.", config.SubjectPerDay * config.WorkDays);
            @ViewData["LimitLeft"] = config.SubjectPerDay * config.WorkDays - _context.Subjects.Sum(s => s.TotalHours);

            return View(_context.Subjects.ToList());
        }

        public IActionResult AddSubjectForm()
        {
            var config = _context.SchoolConfig.FirstOrDefault();

            if (config == null)
            {
                return RedirectToAction("Index");
            }

            if (config.WorkDays * config.SubjectPerDay < _context.Subjects.Sum(s => s.TotalHours) + NewSubject.TotalHours)
            {
                @TempData["OverLimit"] = "You have enter hours that overlimit lecture slots";
                return RedirectToAction("Subject");
            }

            Subject? subjectExist = _context.Subjects.FirstOrDefault(s => s.Name == NewSubject.Name);

            if (subjectExist != null)
            {
                subjectExist.TotalHours += NewSubject.TotalHours;
            }
            else
            {
                _context.Subjects.Add(NewSubject);
            }

            _context.SaveChanges();

            return RedirectToAction("Subject");
        }

        #endregion Subject Form

        #region Time Table

        public IActionResult TimeTable()
        {
            SchoolConfig? config = _context.SchoolConfig.FirstOrDefault();

            if (config == null)
            {
                return RedirectToAction("Index");
            }

            List<Subject> subjects = _context.Subjects.ToList();

            var slots = new List<Subject>();

            foreach (var subject in subjects)
            {
                slots.AddRange(Enumerable.Range(0, subject.TotalHours).Select(s => subject).ToList());
            }

            var lectures = slots.Randomize()
                                .Select((value, index) => new { value, index })
                                .GroupBy(x => x.index / config.WorkDays)
                                .Select(g => g.Select(x => x.value).ToList())
                                .ToList();

            return View(lectures);
        }

        public IActionResult GenerateTimetable()
        {
            return RedirectToAction("TimeTable");
        }

        #endregion Time Table

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
