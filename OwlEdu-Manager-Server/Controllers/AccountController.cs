using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;

namespace OwlEdu_Manager_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController :ControllerBase
    {
        private readonly AccountService _accountService;
        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }
        // GET: api/Account
        [HttpGet]
        public async Task<IActionResult> GetAllAccounts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var accounts = await _accountService.GetAllAsync(pageNumber, pageSize);
            return Ok(accounts);
        }
        // GET: api/Account/{id}
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
        // POST: api/Account
        [HttpPost]
        public async Task<IActionResult> AddAccount([FromBody] Account account)
        {
            if (account == null)
            {
                return BadRequest(new { Message = "Invalid account data." });
            }

            await _accountService.AddAsync(account);
            return CreatedAtAction(nameof(GetAccountById), new { id = account.Id }, account);
        }
        // PUT: api/Account/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(string id, [FromBody] Account account)
        {
            if (account == null || id != account.Id)
            {
                return BadRequest(new { Message = "Invalid account data or mismatched ID." });
            }

            var existingAccount = await _accountService.GetByIdAsync(id);
            if (existingAccount == null)
            {
                return NotFound(new { Message = "Account not found." });
            }

            await _accountService.UpdateAsync(account);
            return NoContent();
        }
        // DELETE: api/Account/{id}
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
        // GET: api/Account/search?keyword=example&pageNumber=1&pageSize=10
        //[HttpGet("search")]
        //public async Task<IActionResult> SearchAccounts([FromQuery] string keyword, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        //{
        //    if (string.IsNullOrWhiteSpace(keyword))
        //    {
        //        return BadRequest(new { Message = "Keyword cannot be empty." });
        //    }

        //    var accounts = await _accountService.GetByKeywordAsync(keyword, pageNumber, pageSize);
        //    return Ok(accounts);
        //}
    }
}
