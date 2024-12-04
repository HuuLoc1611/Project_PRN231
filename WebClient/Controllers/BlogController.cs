using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


//using Newtonsoft.Json;
using WebClient.Models;
using System;
using static System.Net.WebRequestMethods;

namespace WebClient.Controllers
{
	public class BlogController : Controller
	{
		private readonly ProjectPrn231Context context = new ProjectPrn231Context();

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

		//Blog list - trang index
        [HttpGet]
		public async Task<IActionResult> Index()
		{
			List<Blog> blogs = await context.Blogs.Include(x => x.Creator).OrderByDescending(x=> x.Id).ToListAsync();

			//var blogsJson = JsonConvert.SerializeObject(blogs);

			List<Tag> tags = await context.Tags.ToListAsync();

			List<CommentBlog> commentBlogs = await context.CommentBlogs.ToListAsync();

			ViewBag.ListBlog = context.Blogs.Include(x=> x.Creator).Where(x=> x.Status == true).OrderByDescending(x => x.Id).ToList();
			ViewBag.account = GetUserLogin();
			ViewBag.CommentCount = commentBlogs.Count;

			return View("Index");
		}


		public IActionResult BlogDetail(int? id)
		{
			if (id == null) return NotFound(); // Handle not found case

			// Get the logged-in user
			Account accountLogin = GetUserLogin();

			ViewBag.account = accountLogin;

			// Fetch the blog with comments asynchronously
			var blog =  context.Blogs
									 .Include(x => x.Creator)
									 .FirstOrDefault(x => x.Id == id);
			ViewBag.blog = blog;


			// Fetch total comments count and list asynchronously
			var totalComment =  context.CommentBlogs
											 .Count(x => x.BlogId == id);

			//total comment
			ViewBag.totalComment = totalComment;

			//list comment
			var commentList = context.CommentBlogs.Include(x => x.Account).Where(x => x.BlogId == id).ToList();

			ViewBag.commentList = commentList;

			//rating
			if (accountLogin != null)
			{
				var rating = context.Ratings.FirstOrDefault(x => x.AccountId == accountLogin.Id && x.BlogId == blog.Id);
				ViewBag.rating = rating;
			}

			
			
			return View("Blog-Detail");
		}


		[HttpGet]
		public IActionResult WriteBlog()
		{
			// chua login thi login
			if (GetUserLogin()==null)
			{
				return RedirectToAction("Login", "Account");
			}
			 return View("Write-Blog");
		}

        [HttpPost]
        public async Task<IActionResult> CreateBlog(string title, string content, IFormFile image, bool allowComments = false)
        {
			Blog blog = new Blog();
			//gan can gia tri cho blog
			blog.Title = title;
			blog.CreatedDate = DateTime.Now;
			blog.Content = content;
			blog.CreatorId = GetUserLogin().Id;

			//cho phep blog do duoc comment hay khong
			blog.IsComment = allowComments;

			//status false la chua duoc duyet
			blog.Status = false;

            // Handle image upload
            if (image != null && image.Length > 0)
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
                blog.Image = "/uploads/" + uniqueFileName;
            }

			string url = "https://localhost:5100/AddBlog";

			using (HttpClient client = new HttpClient())
			{
				using (HttpResponseMessage res = await client.PostAsJsonAsync(url, blog))
				{
					using (HttpContent content2 = res.Content)
					{
						string data = await content2.ReadAsStringAsync();
					}
				}
			}

			return RedirectToAction("Index");
        }

		[HttpGet]
        //Xem cac blog can duoc duyet
        public async Task<IActionResult> MonitorBlog()
		{	
			if (GetUserLogin() == null)
			{         
                return RedirectToAction("Index", "Blog");
            }
            if (GetUserLogin().Role != 2)
            {
                return RedirectToAction("Index", "Blog");
            }

            List<Blog> blogs = new List<Blog>();
			string url2 = "https://localhost:5100/GetListBlogRequest";
			using (HttpClient client = new HttpClient())
			{
				using (HttpResponseMessage res = await client.GetAsync(url2))
				{
					using (HttpContent content = res.Content)
					{
						string data = await content.ReadAsStringAsync();
						blogs = JsonConvert.DeserializeObject<List<Blog>>(data);
					}
				}
			}
			ViewBag.listBlogRequest = blogs;
			return View("Monitor-Blog");
		}

		[HttpPost]
		//Accept hoac reject blog
		public async Task<IActionResult> RequestBlog(int bId, int request)
		{

			string url = $"https://localhost:5100/UpdateStatusBlog/{bId}/{request}";  // Use interpolated string to insert bId and request

			using (HttpClient client = new HttpClient())
			{
				// Create an empty PUT request since we only need URL parameters
				using (HttpResponseMessage res = await client.PutAsync(url, null))
				{
					if (res.IsSuccessStatusCode)
					{
						// Optional: Handle response if needed
						string data = await res.Content.ReadAsStringAsync();
						// Process the data if required
					}	
				}
			}


			return RedirectToAction("MonitorBlog", "Blog");
		}

		[HttpGet]
		//xem cac blog minh da dang
		public async Task<IActionResult> MyBlog()
		{
			if(GetUserLogin() == null)
			{
				return RedirectToAction("Login", "Account");
			}
			List<Blog> blogs = new List<Blog>();
			string url2 = "https://localhost:5100/GetListBlogByUserId";
			using (HttpClient client = new HttpClient())
			{
				using (HttpResponseMessage res = await client.GetAsync(url2 + "/" +GetUserLogin().Id))
				{
					using (HttpContent content = res.Content)
					{
						string data = await content.ReadAsStringAsync();
						blogs = JsonConvert.DeserializeObject<List<Blog>>(data);
					}
				}
			}
			ViewBag.blogs = blogs;
			return View("My-Blog");
		}

		[HttpGet]
		//xoa blog
		public async Task<IActionResult> DeleteBlog(int? id)
		{
			string url = "https://localhost:5100/DeleteBlog?blogId=";
            using (HttpClient client = new HttpClient())
            {
                // Make sure to construct the URL correctly with the query parameter
                using (HttpResponseMessage res = await client.DeleteAsync(url + id)) // cmId is appended to the URL
                {
                    if (res.IsSuccessStatusCode)
                    {
                        // Optional: Handle successful response
                        string data = await res.Content.ReadAsStringAsync();
                        // Process the data if necessary
                    } 
                }
            }
			return RedirectToAction("MyBlog", "Blog");
        }

		//View blog de update
		[HttpGet]
		public async Task<IActionResult> ViewBlog(int? id)
		{
            if (GetUserLogin() == null)
            {
                return RedirectToAction("Login", "Account");
            }
			if(GetUserLogin().Role != context.Blogs.Include(x => x.Creator).FirstOrDefault(x => x.Id == id).CreatorId)
			{
                return RedirectToAction("Index", "Blog");
            }
            Blog blog = new Blog();
            List<CommentBlog> commentList = new List<CommentBlog>();
            int totalComment = 0;
            Account accountLogin = GetUserLogin();
            // Construct the URL
            string url = "https://localhost:5100/GetBlogById/" + id;

            // Call the external API to get the blog details
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(url))
                {
                    if (res.IsSuccessStatusCode) // Check if the request was successful
                    {
                        string result = await res.Content.ReadAsStringAsync();

                        var json = JObject.Parse(result);

                        // Deserialize the response to extract blog and comment data
                        blog = json["blog"].ToObject<Blog>();
                        commentList = json["commentList"].ToObject<List<CommentBlog>>();
                        totalComment = (int)json["totalComment"];
                    }
                }
            }

			ViewBag.blog = context.Blogs.Include(x => x.Creator).FirstOrDefault(x => x.Id == id);
            return View("Update-Blog");

        }

        [HttpPost]
		//Update blog
        public async Task<IActionResult> UpdateBlog(int blogId, string? title, string? content, IFormFile? newImage, bool? isComment)
        {
            Blog blog = new Blog
            {
                Id = blogId,
                Title = title,
                Content = content,
                IsComment = isComment ?? false // Set default value
            };

            // Handle image upload
            if (newImage != null && newImage.Length > 0)
            {
                // Save the new image
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(newImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await newImage.CopyToAsync(fileStream);
                }

                // Store the relative path in the database
                blog.Image = "/uploads/" + uniqueFileName;
            }
            else
            {
                // Retain the existing image path if no new image is uploaded
                var currentBlog = await context.Blogs.FindAsync(blogId);
                blog.Image = currentBlog?.Image; // Ensure to fetch the current image path
            }

            string url = "https://localhost:5100/UpdateBlog";

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.PutAsJsonAsync(url, blog))
                {
                    if (!res.IsSuccessStatusCode)
                    {
                        // Handle the error response here (log or display message)
                    }
                }
            }

            return RedirectToAction("ViewBlog", "Blog", new { id = blogId });
        }

    }
}
