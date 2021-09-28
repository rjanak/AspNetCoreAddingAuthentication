using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WishList.Data;
using WishList.Models;

namespace WishList.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ItemController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager )
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var usrmgrResult = _userManager.GetUserAsync(HttpContext.User);
            var userId = usrmgrResult.Result.Id;

            var model = _context.Items
                .Where( i => i.User.Id == userId )
                .ToList();

            return View("Index", model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public IActionResult Create(Models.Item item)
        {
            var usrmgrResult = _userManager.GetUserAsync(HttpContext.User);
            var userId = usrmgrResult.Result.Id;
            var loggedInUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            item.User = loggedInUser;
            _context.Items.Add(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var item = _context.Items.FirstOrDefault(e => e.Id == id);

            var usrmgrResult = _userManager.GetUserAsync(HttpContext.User);
            var userId = usrmgrResult.Result.Id;
            var loggedInUser = _context.Users.FirstOrDefault(u => u.Id == userId);

            if(! item.User.Id.Equals(loggedInUser.Id))
            {
                return Unauthorized();
            }
            _context.Items.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
