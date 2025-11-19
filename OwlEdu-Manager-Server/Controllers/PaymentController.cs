using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.DTOs;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;
using OwlEdu_Manager_Server.Utils;

namespace OwlEdu_Manager_Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments([FromQuery] string keyword = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (keyword == "")
            {
                var payments = await _paymentService.GetAllAsync(pageNumber, pageSize, "Id");
                return Ok(payments.Select(t => ModelMapUtils.MapBetweenClasses<Payment, PaymentDTO>(t)).ToList());
            }
            var keywords = keyword.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            IEnumerable<Payment> finalResult = null;

            foreach (var key in keywords)
            {
                // Tìm theo từng loại
                var byStr = await _paymentService.GetByStringKeywordAsync(key, -1, pageSize, "Id");
                var byNum = await _paymentService.GetByNumericKeywordAsync(key, -1, pageSize, "Id");
                var byTime = await _paymentService.GetByDateTimeKeywordAsync(key, -1, pageSize, "Id");

                // Ghép kết quả của TỪ khóa hiện tại
                var unionForCurrentKeyword = byStr
                    .Concat(byNum)
                    .Concat(byTime)
                    .DistinctBy(t => t.Id);

                // Lần đầu → gán luôn
                if (finalResult == null)
                    finalResult = unionForCurrentKeyword;
                else
                    // Giao nhau để chỉ giữ các item match “mọi keyword”
                    finalResult = finalResult.Intersect(unionForCurrentKeyword);
            }

            // Map sang DTO
            var res = finalResult.Select(t => ModelMapUtils.MapBetweenClasses<Payment, PaymentDTO>(t)).ToList();
            if (pageNumber != -1)
            {
                res = finalResult.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(t => ModelMapUtils.MapBetweenClasses<Payment, PaymentDTO>(t)).ToList();
            }

            if (res == null) res = new List<PaymentDTO>();

            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(string id)
        {
            var existingPayment = await _paymentService.GetByIdAsync(id);
            if (existingPayment == null)
            {
                return BadRequest(new {Message = "Payment not found."});
            }

            var res = ModelMapUtils.MapBetweenClasses<Payment, PaymentDTO>(existingPayment);

            return Ok(res);
        }

        [HttpGet("enrollment/{enrollmentId}")]
        public async Task<IActionResult> GetPaymentByEnrollmentId(string enrollmentId)
        {
            var existingPayment = await _paymentService.GetPayementByEnrollmentId(enrollmentId);
            if (existingPayment == null)
            {
                return BadRequest(new { Message = "Payment not found." });
            }

            var res = ModelMapUtils.MapBetweenClasses<Payment, PaymentDTO>(existingPayment);

            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> AddPayment([FromBody] PaymentDTO paymentDTO)
        {
            if (paymentDTO == null)
            {
                return BadRequest(new { Message = "Invalid payment data." });
            }

            var payment = ModelMapUtils.MapBetweenClasses<PaymentDTO, Payment>(paymentDTO);
            
            var allPayments = await _paymentService.GetAllAsync(-1, -1, "Id");

            if (allPayments.Count() == 0)
            {
                payment.Id = "HD" + DateTime.UtcNow.ToString("ddMMyyyy") + "0000";
            }
            else
            {
                var last = allPayments.Last();

                var lastIdNumber = int.Parse(last.Id.Substring(10));

                var newIdNumber = lastIdNumber + 1;

                string newId = "HD" + DateTime.UtcNow.ToString("ddMMyyyy");

                for (int i = 0; i < 4 - newIdNumber.ToString().Length; i++) newId += "0";

                payment.Id = newId + newIdNumber.ToString();
            }

            await _paymentService.AddAsync(payment);

            return CreatedAtAction(nameof(GetPaymentById), payment.Id, ModelMapUtils.MapBetweenClasses<Payment, PaymentDTO>(payment));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(string id, [FromBody] PaymentDTO paymentDTO)
        {
            if (id == null || paymentDTO == null)
            {
                return BadRequest(new { Message = "Invalid payment data." });
            }

            var existingPayment = await _paymentService.GetByIdAsync(id);
            if (existingPayment == null)
            {
                return BadRequest(new { Message = "Payment not found." });
            }

            existingPayment.PaymentDate = paymentDTO.PaymentDate;
            existingPayment.FeeCollectorId = paymentDTO.FeeCollectorId;
            existingPayment.PayerId = paymentDTO.PayerId;
            existingPayment.Method = paymentDTO.Method;
            existingPayment.Status = paymentDTO.Status;

            await _paymentService.UpdateAsync(existingPayment);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(string id)
        {
            var existingPayment = await _paymentService.GetByIdAsync(id);
            if (existingPayment == null)
            {
                return BadRequest(new { Message = "Payment not found." });
            }

            await _paymentService.DeleteAsync(id); 

            return NoContent();
        }
    }
}
