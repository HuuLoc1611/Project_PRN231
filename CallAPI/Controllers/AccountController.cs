using CallAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

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

        [HttpPost("SignUp")]
        public IActionResult SignUp([FromBody] Account account)
        {
            var checkAccount = context.Accounts.FirstOrDefault(x => x.Email == account.Email || x.Username == account.Username);
            if (checkAccount != null)
            {
                return BadRequest("Username or email existed");
            }
            if (ModelState.IsValid)
            {
                account.Role = 1;
                account.IsMember = false;
                context.Accounts.Add(account);
                context.SaveChanges();
                return Ok();
            }
            else return BadRequest();
        }

        [HttpPut]
        public IActionResult UpdateProfile([FromBody] Account account)
        {
            var accountCheck = context.Accounts.FirstOrDefault(x => x.Id == account.Id);
            if (ModelState.IsValid && accountCheck != null)
            {
                accountCheck.FullName = account.FullName;
                accountCheck.Image = account.Image;

                context.Accounts.Update(accountCheck);
                context.SaveChanges();
                return Ok(accountCheck);
            }
            else return BadRequest();
        }

        [HttpGet("{Id}")]
        public IActionResult GetAccountById(int Id)
        {
            var accountCheck = context.Accounts.FirstOrDefault(x => x.Id == Id);
            if (accountCheck == null) return NotFound();

            return Ok(accountCheck);
        }
    }
}
