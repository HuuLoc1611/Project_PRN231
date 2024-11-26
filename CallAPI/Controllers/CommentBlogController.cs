using CallAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CallAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CommentBlogController : ControllerBase
	{
		ProjectPrn231Context context = new ProjectPrn231Context();

		[HttpGet("GetCommentListByBlod/{blogId}")]
		public IActionResult GetCommentListByBlod(int blogId)
		{
			var commentBlog = context.CommentBlogs.Where(x=> x.BlogId == blogId).OrderByDescending(x=> x.Id).ToList();
			return Ok(commentBlog);
		}

		[HttpPost("AddComment")]
		public IActionResult AddComment(CommentBlog comment)
		{
			context.CommentBlogs.Add(comment);
			context.SaveChanges();
			return Ok("Add ok");
		}

		[HttpPut("EditComment")]
		public IActionResult Edit(CommentBlog comment)
		{
			CommentBlog cmtBlog = new CommentBlog();
			cmtBlog = context.CommentBlogs.FirstOrDefault(b => b.Id == comment.Id);
			context.CommentBlogs.Update(cmtBlog);
			cmtBlog.Content = comment.Content;
			context.SaveChanges();
			return Ok("Update ok");
		}

		[HttpDelete]
		public IActionResult Delete(int id) 
		{
			CommentBlog cmtBlog = new CommentBlog();
			cmtBlog = context.CommentBlogs.FirstOrDefault(b => b.Id == id);
			context.CommentBlogs.Remove(cmtBlog);
			context.SaveChanges();
			return Ok("Delete ok");
		}

	}
}
