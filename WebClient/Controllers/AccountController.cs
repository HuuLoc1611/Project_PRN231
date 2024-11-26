using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebClient.Models;

namespace WebClient.Controllers
{
	public class AccountController : Controller
	{
		ProjectPrn231Context context = new ProjectPrn231Context();

		[HttpGet]
		public IActionResult Login()
		{
			return View("Login");
		}

		[HttpPost]
		public async Task<IActionResult> Login(string username, string password)
		{
			Account accountLogin = new Account();
			accountLogin = await context.Accounts.FirstOrDefaultAsync(x=> x.Username.Equals(username) && x.Password.Equals(password));
			if (accountLogin == null) 
			{
				ViewBag.error = "Username or pass word wrong!";
				ViewBag.username = username;
				ViewBag.password = password;
				return View("Login");
				
			} else
			{
                string userJson = JsonConvert.SerializeObject(accountLogin);
                HttpContext.Session.SetString("User", userJson);
                return RedirectToAction("Index","Blog");
			}

		}

		[HttpGet]
		public IActionResult GetCurrentUser()
		{
			string userJson = HttpContext.Session.GetString("User");
			if (userJson != null)
			{
				var user = JsonConvert.DeserializeObject<Account>(userJson);
				return Ok(user);
			}
			return Json(null); // Or return an appropriate error response
		}

		[HttpGet]
		public async Task<IActionResult> Logout()
		{

			HttpContext.Session.Remove("User");

			// Optionally, you might want to sign out the user from authentication
			// await HttpContext.SignOutAsync(); // Uncomment if using ASP.NET Identity

			return RedirectToAction("Index", "Blog");

		}
	}
}
