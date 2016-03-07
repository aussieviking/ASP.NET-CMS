﻿using Microsoft.AspNet.Identity;
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
        public async Task<ActionResult> Index()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return View(users);
        }

        // GET: admin/user/create
        [Route("create")]
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var model = new UserViewModel();
            model.LoadUserRoles(await _roleRepository.GetAllRolesAsync());

            return View(model);
        }

        // POST: admin/user/edit/<username>
        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            var completed = await _userService.CreateAsync(model);
            if (!completed) return View(model);

            return RedirectToAction("index");
        }

        // GET: admin/user/edit/<username>
        [Route("edit/{username}")]
        [HttpGet]
        public async Task<ActionResult> Edit(string username)
        {
            var user = await _userService.GetUserByNameAsync(username);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        // POST: admin/user/edit/<username>
        [Route("edit/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserViewModel model)
        {
            var updated = await _userService.UpdateAsync(model);
            if (!updated) return View();

            return RedirectToAction("index");
        }

        // POST: admin/user/delete/<username>
        [Route("delete/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string username)
        {
            await _userService.DeleteAsync(username);

            return RedirectToAction("index");
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