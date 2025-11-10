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
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _accountService.GetAllAsync();
            return Ok(accounts);
        }

        // GET: api/Account/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(string id)
        {
            var account = await _accountService.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        // POST: api/Account
        [HttpPost]
        public async Task<IActionResult> AddAccount([FromBody] Account account)
        {
            await _accountService.AddAsync(account);
            return CreatedAtAction(nameof(GetAccountById), new { id = account.Id }, account);
        }

        // PUT: api/Account/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(string id, [FromBody] Account account)
        {
            if (id != account.Id)
            {
                return BadRequest();
            }
            await _accountService.UpdateAsync(account);
            return NoContent();
        }

        // DELETE: api/Account/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            await _accountService.DeleteAsync(id);
            return NoContent();
        }
    }
}
