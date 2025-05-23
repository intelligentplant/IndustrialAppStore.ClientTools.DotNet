﻿using ExampleMvcApplication.Models;

using IntelligentPlant.IndustrialAppStore.Authentication;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ExampleMvcApplication.Controllers {
    public class AccountController : Controller {

        private readonly IndustrialAppStoreAuthenticationOptions _authenticationOptions;


        public AccountController(IOptions<IndustrialAppStoreAuthenticationOptions> authenticationOptions) {
            _authenticationOptions = authenticationOptions.Value;
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = "/") {
            if (_authenticationOptions.UseExternalAuthentication) {
                return LocalRedirect(returnUrl);
            }

            var viewModel = new LoginViewModel() {
                IsPersistent = false,
                RedirectUrl = returnUrl
            };
            return View(viewModel);
        }


        [HttpPost]
        [ActionName(nameof(Login))]
        [ValidateAntiForgeryToken]
        public IActionResult LoginPost(LoginViewModel viewModel) {
            if (_authenticationOptions.UseExternalAuthentication) {
                return RedirectToAction("Index", "Home");
            }

            return Challenge(new AuthenticationProperties() {
                IsPersistent = viewModel.IsPersistent,
                RedirectUri = string.IsNullOrWhiteSpace(viewModel.RedirectUrl) 
                    ? "/" 
                    : viewModel.RedirectUrl
            }, IndustrialAppStoreAuthenticationDefaults.AuthenticationScheme);
        }


        [HttpGet]
        public async Task<IActionResult> Logout() {
            if (_authenticationOptions.UseExternalAuthentication) {
                return RedirectToAction("Index", "Home");
            }

            await Request.HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
