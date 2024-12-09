using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebClient.Models;
using WebClient.Services.VnPay;

namespace WebClient.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IVnPayService _vnPayService;
        public TransactionController(IVnPayService vnPayService)
        {

            _vnPayService = vnPayService;
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

        //lay du lieu khi vua thanh toan xong tren vn pay
        [HttpGet]
        public async Task <IActionResult> CheckOut()
        {
            string url = "https://localhost:5100/api/";
            var response = _vnPayService.PaymentExecute(Request.Query);

            Transaction transaction = new Transaction
            {
                AccountId = GetUserLogin().Id,
                Date = DateTime.Now,
                Price = 10000.0

            };


            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.PostAsJsonAsync(url + "transaction", transaction))
                {
                    if (res.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        using (HttpResponseMessage res2 = await client.PutAsJsonAsync(url + "Account/UpdateMember" , GetUserLogin()))
                        {
                            if (res2.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                string result = await res2.Content.ReadAsStringAsync();

                                //UPDATE ACCOUNT LAY THONG TIN MOI NHAT CUA ACCONT 
                                var json = JObject.Parse(result);

                                HttpContext.Session.Remove("User");


                                string userJson = JsonConvert.SerializeObject(json);
                                HttpContext.Session.SetString("User", userJson);

                                return RedirectToAction("WriteBlog", "Blog");
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index", "Blog");
        }


        //Manage nhung nguoi da la member
        [HttpGet]
        public async Task<IActionResult> ManageTransaction()
        {
            if (GetUserLogin().Role != 2) return RedirectToAction("Index","Blog");
            return View("Manage-Transaction");
        }


    }
}
