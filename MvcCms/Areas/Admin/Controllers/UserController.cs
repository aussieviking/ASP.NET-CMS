using Microsoft.AspNet.Identity;
using MvcCms.Areas.Admin.Services;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("admin")]
    [RoutePrefix("user")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly UserService _userService;

        public UserController()
        {
            _userRepository = new UserRepository();
            _roleRepository = new RoleRepository();
            _userService = new UserService(ModelState, _userRepository, _roleRepository);
        }

        // GET: admin/user
        [Route("")]
        public ActionResult Index()
        {
            using (var manager = new CmsUserManager())
            {
                var users = manager.Users.ToList();
                return View(users);
            }
        }

        // GET: admin/user/create
        [Route("create")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: admin/user/edit/<username>
        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            if (await _userService.Create(model)) return RedirectToAction("index");

            return View(model);
        }

        // GET: admin/user/edit/<username>
        [Route("edit/{username}")]
        [HttpGet]
        public ActionResult Edit(string username)
        {
            using (var userStore = new CmsUserStore())
            using (var userManager = new CmsUserManager(userStore))
            {
                var user = userStore.FindByNameAsync(username).Result;
                if (user == null) return HttpNotFound();

                var userModel = new UserViewModel
                {
                    UserName = user.UserName,
                    DisplayName = user.DisplayName,
                    Email = user.Email
                };

                return View(userModel);
            }
        }

        // POST: admin/user/edit/<username>
        [Route("edit/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            using (var userStore = new CmsUserStore())
            using (var userManager = new CmsUserManager(userStore))
            {
                var user = userStore.FindByNameAsync(model.UserName).Result;
                if (user == null) return HttpNotFound();

                if (!String.IsNullOrWhiteSpace(model.NewPassword))
                {
                    if (String.IsNullOrWhiteSpace(model.CurrentPassword))
                    {
                        ModelState.AddModelError(String.Empty, "The current password must be supplied");
                        return View(model);
                    }

                    var passwordVerify = userManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, model.CurrentPassword);
                    if (passwordVerify != PasswordVerificationResult.Success)
                    {
                        ModelState.AddModelError(String.Empty, "The current password does not match our records");
                        return View(model);
                    }

                    var newHashedPassword = userManager.PasswordHasher.HashPassword(model.NewPassword);

                    user.PasswordHash = newHashedPassword;
                }

                user.Email = model.Email;
                user.DisplayName = model.DisplayName;

                var updateResult = userManager.UpdateAsync(user).Result;
                if (!updateResult.Succeeded)
                {
                    ModelState.AddModelError(String.Empty, "An error occurred updating the user details");
                    return View(model);
                }

                return RedirectToAction("index");
            }
        }

        // POST: admin/user/delete/<username>
        [Route("delete/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string username)
        {
            if (!ModelState.IsValid) return View();

            using (var userStore = new CmsUserStore())
            using (var userManager = new CmsUserManager(userStore))
            {
                var user = userStore.FindByNameAsync(username).Result;
                if (user == null) return HttpNotFound();

                var deleteResult = userManager.DeleteAsync(user).Result;

                if (!deleteResult.Succeeded)
                {
                    ModelState.AddModelError(String.Empty, "An error occurred deleting the user");
                    return View();
                }

                return RedirectToAction("index");
            }
        }

        private bool _isDisposed;
        protected override void Dispose(bool disposing)
        {
            if (_isDisposed != true)
            {
                _userRepository.Dispose();
                _roleRepository.Dispose();
            }

            _isDisposed = true;

            base.Dispose(disposing);
        }
    }
}