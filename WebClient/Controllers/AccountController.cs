﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection.Metadata;
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

        private Account GetUserLogin()
        {
            var userJson = HttpContext.Session.GetString("User");
            if (userJson != null)
            {
                Account account = JsonConvert.DeserializeObject<Account>(userJson);
                return account;
            }
            return null;

        }

        [HttpGet]
		public async Task<IActionResult> Logout()
		{

			HttpContext.Session.Remove("User");

			// Optionally, you might want to sign out the user from authentication
			// await HttpContext.SignOutAsync(); // Uncomment if using ASP.NET Identity

			return RedirectToAction("Index", "Blog");

		}

		[HttpGet]
		public async Task<IActionResult> SignUp()
		{
			return View("Sign-Up");
		}

		[HttpPost]
		public async Task<IActionResult> SignUp([Bind("Email, Username, Password")] Account account, [Bind("ConfirmPassword")] string confirmPassword)
		{
            string url = "https://localhost:5100/api/Account/SignUp";
			ViewBag.email = "";
            ViewBag.username = "";
            ViewBag.password = "";
            ViewBag.confirmPassword = "";
            if (!confirmPassword.Equals(account.Password))
            {
                ViewBag.message = "Password and Confirm Password do not match.";
				ViewBag.email = account.Email;
				ViewBag.username = account.Username;
				ViewBag.password = account.Password;
				ViewBag.confirmPassword = confirmPassword;
                return View("Sign-Up");
            }

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.PostAsJsonAsync(url, account))
                {
                    if (res.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return RedirectToAction("Index", "Blog");
                    }
                    else
                    {
                        using (HttpContent content = res.Content)
                        {
                            ViewBag.message = await content.ReadAsStringAsync();
                            ViewBag.email = account.Email;
                            ViewBag.username = account.Username;
                            ViewBag.password = account.Password;
                            ViewBag.confirmPassword = confirmPassword;
                            return View("Sign-Up");
                        }
                    }
                }
            }
        }
		[HttpGet]
		public IActionResult Profile()
		{
            var userJson = HttpContext.Session.GetString("User");
            Account account = JsonConvert.DeserializeObject<Account>(userJson);

            if (account == null) return View("Login");

            ViewBag.account = account;
			return View("Profile");
		}

		[HttpPost]
        public async Task<IActionResult> Profile(int id, string fullname, IFormFile? image)
        {
            Account accountUpdate = new Account();
            accountUpdate.Id = id;
            if (image != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                // Store the relative path in the database
                
                accountUpdate.Image = "/uploads/" + uniqueFileName;
            }
            accountUpdate.FullName = fullname;
            string url = "https://localhost:5100/api/Account";
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.PutAsJsonAsync(url, accountUpdate))
                {
                    if (res.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = await res.Content.ReadAsStringAsync();

                        var json = JObject.Parse(result);

                        accountUpdate = json.ToObject<Account>();

                        ViewBag.account = accountUpdate;

                        return View("Profile");
                    } 
                    
                }
            }
           // ViewBag.account = accountUpdate;
            return View("Profile");
        }
    }
}

