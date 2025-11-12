using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.DTOs;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;

namespace OwlEdu_Manager_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController :Controller
    {
        private readonly AccountService _accountService;
        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAccounts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var accounts = await _accountService.GetAllAsync(pageNumber, pageSize, "Id");
            return Ok(accounts);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(string id)
        {
            var account = await _accountService.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound(new { Message = "Account not found." });
            }
            return Ok(account);
        }
        [HttpPost]
        public async Task<IActionResult> AddAccount([FromBody] Account newAccount)
        {
            if (newAccount == null)
            {
                return BadRequest(new { Message = "Account data is required." });
            }

            await _accountService.AddAsync(newAccount);
            return CreatedAtAction(nameof(GetAccountById), new { id = newAccount.Id }, newAccount);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(string id, [FromBody] AccountDTO accountDTO)
        {
            if (accountDTO == null || id != accountDTO.Id)
            {
                return BadRequest(new { Message = "Invalid account data or mismatched ID." });
            }

            var existingAccount = await _accountService.GetByIdAsync(id);
            if (existingAccount == null)
            {
                return NotFound(new { Message = "Account not found." });
            }

            //Mapping
            var account = new Account 
            {
                
            };

            await _accountService.UpdateAsync(account);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var existingAccount = await _accountService.GetByIdAsync(id);
            if (existingAccount == null)
            {
                return NotFound(new { Message = "Account not found." });
            }
            await _accountService.DeleteAsync(id);
            return NoContent();
        }
        [HttpGet("search/string")]
        public async Task<IActionResult> SearchAccountsByString([FromQuery] string keyword, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { Message = "Keyword cannot be empty." });
            }

            var accounts = await _accountService.GetByStringKeywordAsync(keyword, pageNumber, pageSize);
            return Ok(accounts);
        }
        [HttpGet("search/number")]
        public async Task<IActionResult> SearchAccountsByNumber([FromQuery] string keyword, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { Message = "Keyword cannot be empty." });
            }

            var accounts = await _accountService.GetByNumericKeywordAsync(keyword, pageNumber, pageSize);
            return Ok(accounts);
        }

        [HttpGet("search/datetime")]
        public async Task<IActionResult> SearchAccountsByDateTime([FromQuery] string keyword, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { Message = "Keyword cannot be empty." });
            }

            var accounts = await _accountService.GetByDateTimeKeywordAsync(keyword, pageNumber, pageSize);
            return Ok(accounts);
        }
    }
}
