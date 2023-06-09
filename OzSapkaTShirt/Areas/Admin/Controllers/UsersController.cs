using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OzSapkaTShirt.Data;
using OzSapkaTShirt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace OzSapkaTShirt.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationContext _context;
        public UsersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            return _userManager.Users != null ?
                        View(await _userManager.Users.Include(u => u.GenderType).Include(u => u.City).OrderBy(u => u.Name).ThenBy(u => u.SurName).ToListAsync()) :
                        Problem("Entity set 'ApplicationContext.Users'  is null.");
        }
        public IActionResult Login()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UserName,PassWord")] ApplicationUser user)
        {
            Microsoft.AspNetCore.Identity.SignInResult signInResult;

            if (ModelState["UserName"].ValidationState == ModelValidationState.Valid)
            {
                if (ModelState["Password"].ValidationState == ModelValidationState.Valid)
                {
                    signInResult = _signInManager.PasswordSignInAsync(user.UserName, user.PassWord, false, false).Result;
                    if (signInResult.Succeeded == true)
                    {
                        return Redirect("/admin/users/index");
                    }
                }
            }
            return View(user);
        }
        [Authorize(Roles = "Administrator")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ChangePassword(string oldPassword, string Password)
        {
            string userIdentity = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IdentityResult identityResult;
            ApplicationUser existingUser = _userManager.FindByIdAsync(userIdentity).Result;

            existingUser.PassWord = Password;
            existingUser.ConfirmPassWord = Password;
            existingUser.UserName = existingUser.UserName.Trim();
            identityResult = _userManager.ChangePasswordAsync(existingUser, oldPassword, Password).Result;
            if (identityResult.Succeeded == true)
            {
                return Redirect("/admin/users/index");
            }
            return View();
        }
        public async Task<IActionResult> Filtering()
        {
            var items = _userManager.Users;
            ViewData["Items"] = items;
            
            return View(items);
            //return _userManager.Users != null ?
            //            View(await _userManager.Users.Include(u => u.GenderType).Include(u => u.City).OrderBy(u => u.Name).ThenBy(u => u.SurName).ToListAsync()) :
            //            Problem("Entity set 'ApplicationContext.Users'  is null.");
        }
        public List<Order> filterUser(string id)
        {
            //kullanıcıya ait tüm siparişleri listeler
            var filterUser = _context.Orders.Where(o => o.UserId == id).Include(o => o.User).Include(o => o.OrderProducts).ToList();
            return filterUser;
        }
    }
}
