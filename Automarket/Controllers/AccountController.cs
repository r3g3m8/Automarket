using Automarket.Domain.ViewModels.Account;
using Automarket.Domain.ViewModels.Profile;
using Automarket.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Automarket.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private string code;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpGet]
        public async Task<IActionResult> Confirm(string name)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.SendConfirmationCode(name);
                if (response.StatusCode == Domain.Enum.StatusCode.Ok)
                {
                    code = response.Data;
                    return PartialView("Confirm", response.Data);
                    //
                }
                ModelState.AddModelError("", response.Description);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(RegisterViewModel model) 
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.Register(model);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(response.Data));
                    return RedirectToAction("Index", "Home");
                ModelState.AddModelError("", "Неверный код подтверждения");
            }
            return View(model);
            //return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ModelState.Remove("VerifyCode");
            ModelState.Remove("SendedCode");
            if (ModelState.IsValid)
            {
                var response = await _accountService.SendConfirmationCode(model.Name);              
                if (response.StatusCode == Domain.Enum.StatusCode.Ok)
                {
                    model.SendedCode = response.Data;
                    return View("Confirm", model);
                    //
                }
                ModelState.AddModelError("", response.Description);
                //return PartialView("Confirm", model);
            }
            return View(model);
    }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.Login(model);
                if (response.StatusCode == Domain.Enum.StatusCode.Ok)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(response.Data));

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Нет подключения к бд");
            }
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.ChangePassword(model);
                if (response.StatusCode == Domain.Enum.StatusCode.Ok)
                {
                    return Json(new { description = response.Description });
                }
            }
            var modelError = ModelState.Values.SelectMany(v => v.Errors);

            return StatusCode(StatusCodes.Status500InternalServerError, new { modelError.FirstOrDefault().ErrorMessage });
        }
    }
}
