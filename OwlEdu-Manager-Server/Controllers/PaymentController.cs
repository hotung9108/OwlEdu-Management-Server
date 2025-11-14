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
                return Ok(payments);
            }

            var paymentsByString = _paymentService.GetByStringKeywordAsync(keyword, pageNumber, pageSize, "Id");
            var paymentsByNumeric = _paymentService.GetByNumericKeywordAsync(keyword, pageNumber, pageSize, "Id");
            var paymentsByDateTime = _paymentService.GetByDateTimeKeywordAsync(keyword, pageNumber, pageSize, "Id");

            await Task.WhenAll(paymentsByString, paymentsByNumeric, paymentsByDateTime);

            var res = paymentsByString.Result.Concat(paymentsByNumeric.Result).Concat(paymentsByDateTime.Result).DistinctBy(t => t.Id).Select(t => ModelMapUtils.MapBetweenClasses<Payment, PaymentDTO>(t)).ToList();

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

        [HttpPost]
        public async Task<IActionResult> AddPayment([FromBody] PaymentDTO paymentDTO)
        {
            if (paymentDTO == null)
            {
                return BadRequest(new { Message = "Invalid payment data." });
            }

            var payment = ModelMapUtils.MapBetweenClasses<PaymentDTO, Payment>(paymentDTO);
            
            var allPayments = await _paymentService.GetAllAsync(-1, -1, "Id");

            if (allPayments == null)
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

            paymentDTO.Id = id;
            var payment = ModelMapUtils.MapBetweenClasses<PaymentDTO, Payment>(paymentDTO);

            await _paymentService.UpdateAsync(payment);

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

            await _paymentService.DeleteAsync(existingPayment); 

            return NoContent();
        }
    }
}
