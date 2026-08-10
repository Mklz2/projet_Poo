using Collect_Go2._0.DAL;
using Collect_Go2._0.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace Collect_Go2._0.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                user.Role = "Client";
                user.StoreId = null;

                UserRepository repository = new UserRepository();

                bool result = repository.AddUser(user);
                Console.WriteLine($"Utilisateur ajouté : {result}");
                if (result)
                {
                    return RedirectToAction("Login");
                }
            }
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Login(string email, string password)
        //{
        //    UserRepository repository = new UserRepository();

        //    User user = repository.GetUserByEmailAndPassword(email, password);

        //    if (user != null)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    ViewBag.Error = "Email ou mot de passe incorrect.";

        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            UserRepository repository = new UserRepository();

            User user = repository.GetUserByEmailAndPassword(email, password);

            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email ou mot de passe incorrect.";

            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
