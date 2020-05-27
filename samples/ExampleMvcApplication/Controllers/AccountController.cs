using System.Threading.Tasks;
using ExampleMvcApplication.Models;
using IntelligentPlant.IndustrialAppStore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ExampleMvcApplication.Controllers {
    public class AccountController : Controller {

        [HttpGet]
        public IActionResult Login(string redirectUrl = "/") {
            var viewModel = new LoginViewModel() {
                IsPersistent = false,
                RedirectUrl = redirectUrl
            };
            return View(viewModel);
        }


        [HttpPost]
        [ActionName(nameof(Login))]
        [ValidateAntiForgeryToken]
        public IActionResult LoginPost(LoginViewModel viewModel) {
            return Challenge(new AuthenticationProperties() {
                IsPersistent = viewModel.IsPersistent,
                RedirectUri = string.IsNullOrWhiteSpace(viewModel.RedirectUrl) 
                    ? "/" 
                    : viewModel.RedirectUrl
            }, IndustrialAppStoreAuthenticationDefaults.AuthenticationScheme);
        }


        [HttpGet]
        public async Task<IActionResult> Logout() {
            await Request.HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
