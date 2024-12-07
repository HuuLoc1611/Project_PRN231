using Microsoft.AspNetCore.Mvc;
using WebClient.Services.VnPay;

namespace WebClient.Controllers
{
    public class OrderController : Controller
    {
        private readonly IVnPayService _vnPayService;
        public OrderController(IVnPayService vnPayService)
        {

            _vnPayService = vnPayService;
        }

    }
}
