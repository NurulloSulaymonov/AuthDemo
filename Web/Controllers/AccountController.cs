using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Web.Models;

namespace Web.Controllers;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        return View(new LoginDto() { ReturnUrl = returnUrl });
    }
    
    [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {

            if (ModelState.IsValid == false) return View(model);
            
            if (model.Username == "alijon" && model.Password == "1234")
            {
                //fill claims 
            var cliams = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Alijon"),
                new Claim(ClaimTypes.Email, "alijon@gmail.com"),
               // new Claim(ClaimTypes.Role, "Admin")
            };

       
            //create identity 
            var userIdentity = new ClaimsIdentity(cliams, "UserIdentity");
            // create principal
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity[]{userIdentity});
            await  HttpContext.SignInAsync(userPrincipal, new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddDays(1),
                IsPersistent = true,
            });
            
            if (string.IsNullOrEmpty(model.ReturnUrl))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(model.ReturnUrl);
            }
            
        }
        
        return View(model);
    }
        
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterDto());
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid) return View(model);
    
            var user = new IdentityUser() { UserName = model.Username };
            var result = await _userManager.CreateAsync(user, model.Password);
            if(result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
        
        public IActionResult AccessDenied()
        {
            return View();
        }
        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index","Home");
        }


}