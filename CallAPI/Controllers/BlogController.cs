using CallAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CallAPI.Controllers
{
	public class BlogController : Controller
	{

		ProjectPrn231Context context = new ProjectPrn231Context();


		//get all blog va filter

		[HttpGet("GetListBlog")]
		public IActionResult GetListBlog(string? searchName, DateTime? dateFrom, DateTime? dateTo, string? sort)
		{
			var blogs = context.Blogs.Include(x => x.Creator).Where(x=> x.Status == true).
				OrderByDescending(x => x.Id).Select(x => new
				{
					id = x.Id,
					Title = x.Title,
					Image = x.Image,
					CreatedDate = x.CreatedDate,
					Content = x.Content,
					
					Creator = new Account
					{
						FullName = x.Creator.FullName,
					},

                    AverageRating = x.Ratings.Any() ? x.Ratings.Average(r => r.Quality) : 0
                }).ToList();
			
			
			List<CommentBlog> commentBlogs = context.CommentBlogs.ToList();

			//filter by title
			if (!string.IsNullOrEmpty(searchName))
			{
				blogs = blogs.Where(x => x.Title.ToLower().Contains(searchName)).ToList();
			}

			//filter by date
			if (dateFrom.HasValue)
			{
				blogs = blogs.Where(x => x.CreatedDate >= dateFrom.Value).ToList();
			}

			// Lọc theo ngày kết thúc
			if (dateTo.HasValue)
			{
				blogs = blogs.Where(x => x.CreatedDate <= dateTo.Value).ToList();
			}

			if (!string.IsNullOrEmpty(sort))
			{
				if (sort.Equals("asc"))
				{
					blogs = blogs.OrderBy(x => x.AverageRating).ToList();
				}else
				{
                    blogs = blogs.OrderByDescending(x => x.AverageRating).ToList();
                }
			}

			var response = new
			{
				blogs = blogs, // Your filtered blog list
				commentCount = commentBlogs.Count, // Comment count
                sort = sort
            };

			return Ok(response);
		}


		//xem list blog cua user do
		[HttpGet("GetListBlogByUserId/{userId}")]
		public IActionResult GetListBlogByUserId(int userId)
		{
			var blogs = context.Blogs.Include(x => x.Creator).Where(x => x.CreatorId == userId && x.Status == true).
				OrderByDescending(x => x.Id).Select(x => new
				{
					id = x.Id,
					Title = x.Title,
					Image = x.Image,
					CreatedDate = x.CreatedDate,
					Content = x.Content,
					Creator = new Account
					{
						FullName = x.Creator.FullName,
					}
				}).ToList();
			return Ok(blogs);
		}

		//blog detail
		[HttpGet("GetBlogById/{id}")]
		public IActionResult GetBlogById(int id)
		{
			var blog = context.Blogs.Include(x => x.Creator).Select(x => new
			{
				Id = id,
				Title = x.Title,
				CreatedDate = x.CreatedDate,
				Image = x.Image,
				Creator = new Account
				{
					Id = x.Id,
					FullName = x.Creator.FullName
				}
			}).FirstOrDefault(x => x.Id == id);

			var commentList = context.CommentBlogs.Where(x => x.BlogId == id).ToList();

			var totalComment = commentList.Count;

			var response = new
			{
				blog = blog,
				commentList = commentList,
				totalComment = totalComment
			};
			if (blog != null)
			{
				return Ok(response);
			}
			else
			{
				return NotFound();
			}
		}


		//duyet blog xem co duoc up khong
		[HttpPut("UpdateStatusBlog/{bId}/{request}")]
		public IActionResult UpdateStatusBlog(int bId, int request)
		{
			Blog blogUpdate = new Blog();

			blogUpdate = context.Blogs.FirstOrDefault(x => x.Id == bId);
			if (blogUpdate != null)
			{
				if(request == 1)
				{
					blogUpdate.Status = true;
				} else
				{
					blogUpdate.Status = null;
				}
			}
			context.Blogs.Update(blogUpdate);
			context.SaveChanges();
			return Ok("Update OK");
		}


		//them blog
		[HttpPost("AddBlog")]
		public IActionResult CreateBlog([FromBody] Blog blog)
		{
			context.Blogs.Add(blog);
			context.SaveChanges();
			return Ok("Add Ok");
		}

		//update blog
        [HttpPut("UpdateBlog")]
        public IActionResult UpdateBlog([FromBody] Blog blog)
        {
            var blogUpdate = context.Blogs.FirstOrDefault(x => x.Id == blog.Id);
            if (blogUpdate != null)
            {
                blogUpdate.Title = blog.Title;
                blogUpdate.Image = blog.Image; // Ensure the image is updated correctly
                blogUpdate.IsComment = blog.IsComment; // Ensure IsComment is set correctly
                blogUpdate.Content = blog.Content;

                context.Blogs.Update(blogUpdate);
                context.SaveChanges();
                return Ok("Update OK");
            }
            return NotFound("Blog not found");
        }


        //xoa blog
        [HttpDelete("DeleteBlog")]
		public IActionResult DeleteBlog(int blogId)
		{
			Blog blog = new Blog();
			var commentBlog = context.CommentBlogs.Where(x => x.BlogId == blogId).ToList();

			context.CommentBlogs.RemoveRange(commentBlog);

			blog = context.Blogs.FirstOrDefault(x => x.Id == blogId);
			context.Blogs.Remove(blog);
			context.SaveChanges();
			return Ok("Delete OK");
		}

		//xem list blog can duoc duyet
		[HttpGet("GetListBlogRequest")]
		public IActionResult GetListBlogRequest()
		{
			var blogs = context.Blogs.Include(x => x.Creator).Where(x=> x.Status == false).
				OrderByDescending(x => x.Id).Select(x => new
				{
					id = x.Id,
					Title = x.Title,
					Image = x.Image,
					CreatedDate = x.CreatedDate,
					Content = x.Content,
					Creator = new Account
					{
						FullName = x.Creator.FullName,
					}
				}).ToList();

			return Ok(blogs);
		}
	}
}
