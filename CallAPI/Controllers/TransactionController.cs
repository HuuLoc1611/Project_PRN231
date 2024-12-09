using CallAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CallAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        ProjectPrn231Context context = new ProjectPrn231Context();

        [HttpGet]
        public ActionResult Get() {
			var data = context.Transactions.Include(x=> x.Account).Select(x=> new
            {
                Id = x.Id,
				Date = x.Date,
                Price = x.Price,
                Account = new Account
                {
                    FullName = x.Account.FullName,
                    Username = x.Account.Username
                }

			}).ToList();
			return Ok(data);
        }

        [HttpGet("{id}")]
        public IActionResult GetTransactionByAccountId(int id)
        {
            var data = context.Transactions.FirstOrDefault(x => x.AccountId == id);
            if (data == null) return NotFound();

            return Ok(data);
        }

        [HttpPost]
        public ActionResult PostTransaction([FromBody]Transaction transaction) {
            if (!ModelState.IsValid) return BadRequest();
            
            context.Transactions.Add(transaction);
            context.SaveChanges();
            return Ok();
        }
    }
}
