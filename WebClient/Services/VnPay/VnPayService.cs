using WebClient.Library;
using WebClient.Models.VnPay;

namespace WebClient.Services.VnPay
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            // Lấy thời gian hiện tại theo múi giờ cấu hình
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);

            // Tạo mã giao dịch duy nhất
            var transactionRef = $"TXN{DateTime.UtcNow.Ticks}";

            // Tạo thư viện VNPay
            var pay = new VnPayLibrary();

            // Thêm dữ liệu yêu cầu
            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString()); // Đơn vị VNĐ * 100
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", _configuration["Vnpay:PaymentBackReturnUrl"]);
            pay.AddRequestData("vnp_TxnRef", transactionRef);

            // Tạo URL thanh toán
            var paymentUrl = pay.CreateRequestUrl(
                _configuration["Vnpay:BaseUrl"],
                _configuration["Vnpay:HashSecret"]
            );

            return paymentUrl;
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
            return response;
        }
    }
}
