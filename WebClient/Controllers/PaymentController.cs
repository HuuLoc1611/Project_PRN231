using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using WebClient.Models.VnPay;
using WebClient.Services.VnPay;

namespace WebClient.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        public PaymentController(IVnPayService vnPayService)
        {

            _vnPayService = vnPayService;
        }

        [HttpGet]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }

        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {

            //    var response = _vnPayService.PaymentExecute(Request.Query);

            return RedirectToAction("login", "account");

        }

  


    }
}
