using CallAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CallAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        ProjectPrn231Context context = new ProjectPrn231Context();

        //Lay rating cho blog
        [HttpGet("{blogId}")]
        public IActionResult GetRatingByBlog(int blogId)
        {
            var rating = context.Ratings.Where(x => x.BlogId == blogId).ToList();

            if (rating != null) return Ok(rating);

			else return NotFound();

		}

        //Rating lan dau cho blog
        [HttpPost]
        public IActionResult AddRating([FromBody]Rating rating)
        {
            if (ModelState.IsValid)
            {
                context.Ratings.Add(rating);
                context.SaveChanges();
                return Ok();           
            }
			else return BadRequest();
		}

        //Rating cho blog
        [HttpPut]
        public IActionResult UpdateRating([FromBody]Rating rating)
        {
            var checkRating = context.Ratings.FirstOrDefault(x=> x.AccountId == rating.AccountId && x.BlogId == rating.BlogId);

            if (ModelState.IsValid && checkRating != null)
            {
                checkRating.Quality = rating.Quality;
                context.Ratings.Update(checkRating);
                context.SaveChanges();
                return Ok();
            }
			else return BadRequest();

		}
    }
}
