using MvcCms.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Models;
using System.Threading.Tasks;

namespace MvcCms.Areas.Admin.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ModelStateDictionary _modelState;

        public UserService(ModelStateDictionary modelState, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _modelState = modelState;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<bool> Create(UserViewModel model)
        {
            if (!_modelState.IsValid) return false;

            if (String.IsNullOrEmpty(model.NewPassword))
            {
                _modelState.AddModelError(String.Empty, "Please specify a password");
                return false;
            }

            var newUser = new CmsUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.UserName
            };

            await _userRepository.CreateAsync(newUser, model.NewPassword);

            return true;
        }
    }
}