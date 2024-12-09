using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebClient.Models;
using System.Net;

namespace WebClient.Controllers
{
	public class CommentBlogController : Controller
	{
		ProjectPrn231Context context = new ProjectPrn231Context();
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
		[HttpPost]
		//tao 1 comment moi
		public async Task<IActionResult> CommentBlog(int blogId, int? parentId, string content)
		{
			if (GetUserLogin()==null)
			{
				return RedirectToAction("Login", "Account");
			}
			CommentBlog comment = new CommentBlog();
			if (parentId == 0) // la comment gốc
			{
				comment.ParentId = 0;
				comment.CreatedDate = DateTime.Now;
				comment.AccountId = GetUserLogin().Id;
				comment.Content = content;
				comment.BlogId = blogId;
			}
			else // reply comment
			{
				comment.ParentId = parentId;
				comment.CreatedDate = DateTime.Now;
				comment.Content = content;
				comment.AccountId = GetUserLogin().Id;
				comment.BlogId = blogId;
			}
			string url = "https://localhost:5100/api/CommentBlog/AddComment";

			using (HttpClient client = new HttpClient())
			{
				using (HttpResponseMessage res = await client.PostAsJsonAsync(url, comment))
				{
					using (HttpContent content2 = res.Content)
					{
						string data = await content2.ReadAsStringAsync();
					}
				}
			}
			return RedirectToAction("BlogDetail", "Blog", new { id = blogId });
		}

		[HttpPost]
		//edit comment
		public async Task<IActionResult> EditComment(int cmId, string content, int blogId)
		{
			if (GetUserLogin() == null)
			{
				return RedirectToAction("Login", "Account");
			}

			string url = "https://localhost:5100/api/CommentBlog/EditComment"; // Ensure the URL matches your API

			// Create a new CommentBlog object
			CommentBlog comment = new CommentBlog
			{
				Id = cmId,
				Content = content // Assuming your CommentBlog has a BlogId property
			};

			using (HttpClient client = new HttpClient())
			{
				// Send the PUT request with the CommentBlog object
				using (HttpResponseMessage res = await client.PutAsJsonAsync(url, comment))
				{
					if (res.IsSuccessStatusCode)
					{
						// Optional: Handle response if needed
						string data = await res.Content.ReadAsStringAsync();
						// Process the data if required
					}
					else
					{
						// Optional: Handle unsuccessful response
						ModelState.AddModelError(string.Empty, "Error occurred while updating the comment.");
						return View(); // Or return an appropriate view or action
					}
				}
			}

			// Redirect to BlogDetail after successful edit
			return RedirectToAction("BlogDetail", "Blog", new { id = blogId });
		}

		[HttpPost]
		//xoa comment
		public async Task<IActionResult> DeleteComment(int cmId, int blogId)
		{
			string url = "https://localhost:5100/api/CommentBlog?id=";

			using (HttpClient client = new HttpClient())
			{
				// Make sure to construct the URL correctly with the query parameter
				using (HttpResponseMessage res = await client.DeleteAsync(url + cmId)) // cmId is appended to the URL
				{
					if (res.IsSuccessStatusCode)
					{
						// Optional: Handle successful response
						string data = await res.Content.ReadAsStringAsync();
						// Process the data if necessary
					}
					else
					{
						// Optional: Handle unsuccessful response
						ModelState.AddModelError(string.Empty, "Error occurred while deleting the comment.");
						return View(); // Or return an appropriate view or action
					}
				}
			}

			// Redirect to BlogDetail after successful deletion
			return RedirectToAction("BlogDetail", "Blog", new { id = blogId });
		}

		
	}

}
