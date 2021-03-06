using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineAdmission.APP.ViewModels.Administrative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdministrationsController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AdministrationsController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        
        public IActionResult UserList()
        {

            if (TempData["success"] !=null)
            {
                ViewBag.success = TempData["success"].ToString();
            }
            if(TempData["error"] !=null)
            {
                ViewBag.error = TempData["error"].ToString();
            }
            if(TempData["warning"] !=null)
            {
                ViewBag.warning = TempData["warning"].ToString();
            }
            var users = _userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
   
            var appUser = await _userManager.FindByIdAsync(id);

            return View(appUser);
        }

        public async Task<IActionResult> DeleteConfirmed(string id, IdentityUser user)
        {
            if (id!=user.Id)
            {
                return null;
            }
            
            IdentityUser identityUser = await _userManager.FindByIdAsync(id);
            var isRoleAssingned =await _userManager.GetRolesAsync(identityUser);
            if (isRoleAssingned.Count()>=1)
            {
                TempData["warning"] = user.UserName+" is in a role.";
            }
            else
            {
                var deleted = await _userManager.DeleteAsync(identityUser);
                if (deleted.Succeeded)
                {
                    TempData["success"] = user.UserName + " is Deleted.";
                }
                else
                {
                    TempData["error"] = "Delete failed.";
                }
            }
            
            return RedirectToAction("UserList");
        }


        [HttpGet]
        public IActionResult RoleList()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole() 
        {
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleVM model)
        {
            if (ModelState.IsValid)
            {
                // We just need to specify a unique role name to create a new role
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                // Saves the role in the underlying AspNetRoles table
                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            //Find the role by RoleID
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={id} cannot be found";
                return View("NotFound");
            }
            var model = new EditRoleVM
            {
                Id = role.Id,
                RoleName = role.Name
            };
            //Retrive all the users
            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleVM model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;

                // Update the Role using UpdateAsync
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("roleList");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddRemoveUserFromRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleVM>();

            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleVM
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddRemoveUserFromRole(List<UserRoleVM> model, string roleIdString)
        {
            var role = await _roleManager.FindByIdAsync(roleIdString);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleIdString} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleIdString });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleIdString });
        }

    }
}
