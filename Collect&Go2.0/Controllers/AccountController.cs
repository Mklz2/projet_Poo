using Collect_Go2._0.Interfaces;
using Collect_Go2._0.Models;
using Collect_Go2._0.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace Collect_Go2._0.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserDAL _userDal;

        public AccountController(IUserDAL userDal)
        {
            _userDal = userDal;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await Client.EmailExistsAsync(model.Email, _userDal))
            {
                ModelState.AddModelError(string.Empty, "Cette adresse email est déjà utilisée.");
                return View(model);
            }

            Client client = new Client(0, model.Firstname, model.Lastname, model.Email, model.Password, model.Phone);

            try
            {
                await client.CreateAccountAsync(_userDal);
                return RedirectToAction("Login");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Impossible de créer le compte, veuillez réessayer.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            Models.User? user = await Models.User.LoginAsync(email, password, _userDal);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.UserType),
                    new Claim(ClaimTypes.GivenName, user.Firstname),
                    new Claim(ClaimTypes.Surname, user.Lastname)
                };

                if (user is Client client)
                {
                    claims.Add(new Claim(ClaimTypes.MobilePhone, client.Phone));
                }

                if (user is Employee employee && employee.Store != null)
                {
                    claims.Add(new Claim("StoreId", employee.Store.StoreId.ToString()));
                }

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                if (user is Employee)
                {
                    return RedirectToAction("Dashboard", "Employee");
                }

                return RedirectToAction("Index", "Products");
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
