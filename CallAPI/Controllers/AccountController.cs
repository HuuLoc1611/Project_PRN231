using CallAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CallAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		ProjectPrn231Context context = new ProjectPrn231Context();

		[HttpGet("Login")]
		public IActionResult Login(Account account)
		{
			Account accountLogin = new Account();

			accountLogin = context.Accounts.FirstOrDefault(x => x.Username == account.Username && x.Password == account.Password);
			return Ok(accountLogin);
		}
	}
}
