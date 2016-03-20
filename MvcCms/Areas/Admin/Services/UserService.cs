using MvcCms.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

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

        public async Task<UserViewModel> GetUserByNameAsync(string username)
        {
            var user = await _userRepository.GetUserByNameAsync(username);
            if (user == null) return null;

            var viewModel = new UserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                DisplayName = user.DisplayName
            };

            var userRoles = await _userRepository.GetRolesForUserAsync(user);

            viewModel.SelectedRole = userRoles.Count() > 1
                ? userRoles.FirstOrDefault()
                : userRoles.SingleOrDefault();

            viewModel.LoadUserRoles(await _roleRepository.GetAllRolesAsync());

            return viewModel;
        }

        public async Task<bool> CreateAsync(UserViewModel model)
        {
            if (!_modelState.IsValid) return false;

            if (String.IsNullOrWhiteSpace(model.NewPassword))
            {
                _modelState.AddModelError(String.Empty, "Please specify a password");
                return false;
            }

            var existingUser = await _userRepository.GetUserByNameAsync(model.UserName);
            if (existingUser != null)
            {
                _modelState.AddModelError(String.Empty, "The user already exists!");
                return false;
            }

            var newUser = new CmsUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.UserName
            };

            await _userRepository.CreateAsync(newUser, model.NewPassword);

            await _userRepository.AddUserToRoleAsync(newUser, model.SelectedRole);

            return true;
        }

        public async Task<bool> UpdateAsync(UserViewModel model)
        {
            if (!_modelState.IsValid) return false;
            
            var user = await _userRepository.GetUserByNameAsync(model.UserName);
            if (user == null) return false;

            if (!String.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (String.IsNullOrWhiteSpace(model.CurrentPassword))
                {
                    _modelState.AddModelError(String.Empty, "The current password must be supplied");
                    return false;
                }

                var passwordVerify = _userRepository.VerifyHashedPassword(user.PasswordHash, model.CurrentPassword);
                if (passwordVerify != PasswordVerificationResult.Success)
                {
                    _modelState.AddModelError(String.Empty, "The current password does not match our records");
                    return false;
                }

                var newHashedPassword = _userRepository.HashPassword(model.NewPassword);
                user.PasswordHash = newHashedPassword;
            }

            user.Email = model.Email;
            user.DisplayName = model.DisplayName;

            await _userRepository.UpdateAsync(user);

            var roles = await _userRepository.GetRolesForUserAsync(user);
            await _userRepository.RemoveUserFromRolesAsync(user, roles.ToArray());
            await _userRepository.AddUserToRoleAsync(user, model.SelectedRole);

            return true;
        }

        public async Task DeleteAsync(string username)
        {
            var user = await _userRepository.GetUserByNameAsync(username);
            if (user == null) return;

            await _userRepository.DeleteAsync(user);
        }
    }
}