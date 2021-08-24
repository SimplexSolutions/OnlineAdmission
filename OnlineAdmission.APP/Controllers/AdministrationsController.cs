using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineAdmission.APP.ViewModels.Administrative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Controllers
{
    public class AdministrationsController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdministrationsController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
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


    }
}
