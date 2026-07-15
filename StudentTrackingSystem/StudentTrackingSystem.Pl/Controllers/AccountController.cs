//using StudentTrackingSystem.DAL.Models;
//using StudentTrackingSystem.PL.DTOs;
//using Microsoft.AspNetCore.Authentication;
////using Microsoft.AspNetCore.Authentication.Google;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
//using StudentTrackingSystem.DAL.Data.Contexts;
//using Microsoft.Extensions.Logging;

//namespace StudentTrackingSystem.PL.Controllers
//{
//    public class AccountController : Controller
//    {
//        private readonly UserManager<AppUser> _userManager;
//        private readonly SignInManager<AppUser> _signInManager;


//        private readonly AppDbContext appDb;
//        private readonly EmailService _emailService;
//        private readonly ILogger<AccountController> _logger;
//        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext appDb, EmailService emailService, ILogger<AccountController> logger)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//            this.appDb = appDb;
//            _emailService = emailService;
//            _logger = logger;


//        }

//        #region SignUp
//        [HttpGet]
//        public IActionResult SignUp() => View();

//        [HttpPost]
//        public async Task<IActionResult> SignUp(SignUpDTOs model)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = await _userManager.FindByNameAsync(model.UserName) ?? await _userManager.FindByEmailAsync(model.Email);

//                if (user is null)
//                {
//                    // Check if this email exists in the Teachers table
//                    var isTeacherEmail = appDb.Teatchers.Any(t => t.Email == model.Email);

//                    if (isTeacherEmail)
//                    {
//                        user = new AppUser
//                        {
//                            UserName = model.UserName,
//                            FirstName = model.FirstName,
//                            LastName = model.LastName,
//                            Email = model.Email,
//                            IsAgree = model.IsAgree
//                        };
//                        var result = await _userManager.CreateAsync(user, model.Password);
//                        await _userManager.AddToRoleAsync(user, "Teatcher");

//                        if (result.Succeeded)
//                        {
//                            return RedirectToAction("SignIn");
//                        }

//                        foreach (var error in result.Errors)
//                            ModelState.AddModelError("", error.Description);
//                    }
//                    else
//                    {
//                        ModelState.AddModelError("", "User already exists.");
//                    }

//                    // Check if this email exists in the Students table
//                    var isStudentEmail = appDb.Students.Any(S => S.EmailAddress == model.Email);

//                    if (isStudentEmail)
//                    {
//                        user = new AppUser
//                        {
//                            UserName = model.UserName,
//                            FirstName = model.FirstName,
//                            LastName = model.LastName,
//                            Email = model.Email,
//                            IsAgree = model.IsAgree
//                        };
//                        var result = await _userManager.CreateAsync(user, model.Password);
//                        await _userManager.AddToRoleAsync(user, "Student");
//                        if (result.Succeeded)
//                        {
//                            return RedirectToAction("SignIn");
//                        }

//                        foreach (var error in result.Errors)
//                            ModelState.AddModelError("", error.Description);
//                    }
//                    else
//                    {
//                        ModelState.AddModelError("", "User already exists.");
//                    }
//                }
//            }
//            return View();
//        }



//        #endregion

//        #region SignIn
//        [HttpGet]
//        public IActionResult SignIn() => View();

//        [HttpPost]
//        public async Task<IActionResult> SignIn(SignInDTO model)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = await _userManager.FindByEmailAsync(model.Email);

//                if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password))
//                {
//                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMy, false);
//                    if (result.Succeeded)
//                    {
//                        return RedirectToAction("Index", "Home");
//                    }
//                }

//                ModelState.AddModelError("", "Invalid login attempt.");
//            }

//            return View(model);
//        }
//        #endregion

//        #region SignOut
//        public new async Task<IActionResult> SignOut()
//        {
//            await _signInManager.SignOutAsync();
//            return RedirectToAction("SignIn");
//        }

//        #endregion

//        #region ForgetPassword
//        [HttpGet]
//        public IActionResult ForgetPassword() => View();

//        [HttpPost]
//        public async Task<IActionResult> SendResetPasswordUrl(ForgetPasswordDTO model)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = await _userManager.FindByEmailAsync(model.Email);
//                if (user is not null)
//                {
//                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
//                    var url = Url.Action("ResetPassword", "Account", new { email = model.Email, token }, Request.Scheme);

//                    // هنا بنبعت الإيميل
//                    await _emailService.SendEmailAsync(
//                        model.Email,
//                        "Reset Your Password",
//                        $"<p>Hello {user.FirstName},</p><p>Click the link below to reset your password:</p><p><a href='{url}'>Reset Password</a></p>"
//                    );

//                    return View("CheckYourInbox");
//                }
//            }

//            ModelState.AddModelError("", "Invalid Reset Password !!");
//            return View("ForgetPassword", model);
//        }

//        #endregion

//        #region CheckYourInbox
//        [HttpGet]
//        public IActionResult CheckYourInbox() => View();
//        #endregion

//        #region ResetPassword
//        [HttpGet]
//        public IActionResult ResetPassword(string email, string token)
//        {
//            TempData["email"] = email;
//            TempData["token"] = token;
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
//        {
//            if (ModelState.IsValid)
//            {
//                var email = TempData["email"] as string;
//                var token = TempData["token"] as string;

//                if (email != null && token != null)
//                {
//                    var user = await _userManager.FindByEmailAsync(email);
//                    if (user != null)
//                    {
//                        var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
//                        if (result.Succeeded)
//                            return RedirectToAction("SignIn");

//                        foreach (var error in result.Errors)
//                            ModelState.AddModelError("", error.Description);
//                    }
//                }
//                else
//                {
//                    return BadRequest("Invalid operation !!");
//                }
//            }

//            return View();
//        }
//        #endregion

//        //#region Google Login
//        //public IActionResult GoogleLogin()
//        //{
//        //    var prop = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
//        //    return Challenge(prop, GoogleDefaults.AuthenticationScheme);
//        //}

//        //public async Task<IActionResult> GoogleResponse()
//        //{
//        //    var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
//        //    return RedirectToAction("Index", "Home");
//        //}
//        //#endregion

//        public IActionResult AccessDenied() => View();
//    }
//}


using StudentTrackingSystem.DAL.Models;
using StudentTrackingSystem.PL.DTOs;
using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using StudentTrackingSystem.DAL.Data.Contexts;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace StudentTrackingSystem.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext appDb;
        private readonly EmailService _emailService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext appDb, EmailService emailService, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.appDb = appDb;
            _emailService = emailService;
            _logger = logger;
        }

        #region SignUp
        [HttpGet]
        public IActionResult SignUp() => View();

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDTOs model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName) ?? await _userManager.FindByEmailAsync(model.Email);

                if (user is null)
                {
                    // Check if this email exists in the Teachers table
                    var isTeacherEmail = appDb.Teatchers.Any(t => t.Email == model.Email);

                    if (isTeacherEmail)
                    {
                        user = new AppUser
                        {
                            UserName = model.UserName,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            IsAgree = model.IsAgree
                        };
                        var result = await _userManager.CreateAsync(user, model.Password);
                        await _userManager.AddToRoleAsync(user, "Teatcher");

                        if (result.Succeeded)
                        {
                            return RedirectToAction("SignIn");
                        }

                        foreach (var error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                    else
                    {
                        // Check if this email exists in the Students table
                        var isStudentEmail = appDb.Students.Any(S => S.EmailAddress == model.Email);

                        if (isStudentEmail)
                        {
                            user = new AppUser
                            {
                                UserName = model.UserName,
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                Email = model.Email,
                                IsAgree = model.IsAgree
                            };
                            var result = await _userManager.CreateAsync(user, model.Password);
                            await _userManager.AddToRoleAsync(user, "Student");
                            if (result.Succeeded)
                            {
                                return RedirectToAction("SignIn");
                            }

                            foreach (var error in result.Errors)
                                ModelState.AddModelError("", error.Description);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Email not found in our records.");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User already exists.");
                }
            }
            return View();
        }
        #endregion

        #region SignIn
        [HttpGet]
        public IActionResult SignIn() => View();

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMy, false);
                    if (result.Succeeded)
                    {
                        // Redirect based on user role
                        var roles = await _userManager.GetRolesAsync(user);

                        if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("AdminDashboard", "Dashboard");
                        }
                        else if (roles.Contains("Teatcher"))
                        {
                            return RedirectToAction("TeacherDashboard", "Dashboard");
                        }
                        else if (roles.Contains("Student"))
                        {
                            return RedirectToAction("StudentDashboard", "Dashboard");
                        }
                        else
                        {
                            // Default fallback if no specific role is found
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }

                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View(model);
        }
        #endregion

        #region SignOut
        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn");
        }
        #endregion

        #region ForgetPassword
        [HttpGet]
        public IActionResult ForgetPassword() => View();

        [HttpPost]
        public async Task<IActionResult> SendResetPasswordUrl(ForgetPasswordDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var url = Url.Action("ResetPassword", "Account", new { email = model.Email, token }, Request.Scheme);

                    // هنا بنبعت الإيميل
                    await _emailService.SendEmailAsync(
                        model.Email,
                        "Reset Your Password",
                        $"<p>Hello {user.FirstName},</p><p>Click the link below to reset your password:</p><p><a href='{url}'>Reset Password</a></p>"
                    );

                    return View("CheckYourInbox");
                }
            }

            ModelState.AddModelError("", "Invalid Reset Password !!");
            return View("ForgetPassword", model);
        }
        #endregion

        #region CheckYourInbox
        [HttpGet]
        public IActionResult CheckYourInbox() => View();
        #endregion

        #region ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            if (ModelState.IsValid)
            {
                var email = TempData["email"] as string;
                var token = TempData["token"] as string;

                if (email != null && token != null)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user != null)
                    {
                        var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                        if (result.Succeeded)
                            return RedirectToAction("SignIn");

                        foreach (var error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }
                else
                {
                    return BadRequest("Invalid operation !!");
                }
            }

            return View();
        }
        #endregion

        public IActionResult AccessDenied() => View();
    }
}