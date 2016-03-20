using Microsoft.Owin.Security;
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
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IUserRepository _users;

        public AdminController() : this(new UserRepository()) { }

        public AdminController(IUserRepository userRepository)
        {
            _users = userRepository;
        }

        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            var user = await _users.GetLoginUserAsync(model.Username, model.Password);
            if(user == null)
            {
                ModelState.AddModelError(String.Empty, "The user with the supplied credentials does not exist");
            }

            var authmanager = HttpContext.GetOwinContext().Authentication;
            var userIdentity = await _users.CreateIdentityAsync(user);
            authmanager.SignIn(
                new AuthenticationProperties() { IsPersistent = model.RememberMe }, userIdentity);

            return RedirectToAction("index");
        }

        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            var authmanager = HttpContext.GetOwinContext().Authentication;
            authmanager.SignOut();

            return RedirectToAction("index");
        }
    }
}