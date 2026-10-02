using Amazings_API.Models;
using Amazings_API.Persistence.Data;
using Amazings_API.Records;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace Amazings_API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	[Authorize]
	public class AccountsController : ControllerBase

	{
		private readonly ApplicationDbContext _dbContext;

		public AccountsController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpPost]
		public IActionResult Create([FromBody] CreateAccountRecord record)
		{
			var customerIdClaim = User.FindFirst("CustomerId");

			if (customerIdClaim == null)
			{
				return Unauthorized("Customer identity not found.");
			}

			var customerId = int.Parse(customerIdClaim.Value);

			var customer = _dbContext.Customers
				.FirstOrDefault(c => c.Id == customerId);

			if (customer == null)
			{
				return NotFound("Customer not found.");
			}

			var existingAccount = _dbContext.Accounts
				.FirstOrDefault(a => a.CustomerId == customerId);

			if (existingAccount != null)
			{
				return BadRequest("Customer already has an account.");
			}

			var account = new Account
			{
				CustomerId = customerId,
				AccountNumber = GenerateAccountNumber(),
				Balance = 0
			};

			_dbContext.Accounts.Add(account);
			_dbContext.SaveChanges();

			return Ok(account);
		}

		[HttpPost("{id}/deposit")]
		public IActionResult Deposit(
	int id,
	[FromBody] DepositRecord record)
		{
			var customerIdClaim = User.FindFirst("CustomerId");

			if (customerIdClaim == null)
			{
				return Unauthorized();
			}

			var customerId = int.Parse(customerIdClaim.Value);

			var account = _dbContext.Accounts
				.FirstOrDefault(a =>
					a.Id == id &&
					a.CustomerId == customerId);

			if (account == null)
			{
				return NotFound("Account not found.");
			}

			if (record.Amount <= 0)
			{
				return BadRequest("Deposit amount must be greater than zero.");
			}

			using var transaction = _dbContext.Database.BeginTransaction();

			try
			{
				account.Balance += record.Amount;

				var transactionRecord = new Transaction
				{
					ReceiverAccountId = account.Id,
					Amount = record.Amount,
					TransactionType = "Deposit",
					Direction = "Credit",
					Status = "Completed",
					Reference = GenerateTransactionReference(),
					CreatedAt = DateTime.UtcNow
				};


				_dbContext.Transactions.Add(transactionRecord);

				_dbContext.SaveChanges();

				transaction.Commit();

				return Ok(new
				{
					message = "Deposit successful.",
					accountId = account.Id,
					accountNumber = account.AccountNumber,
					newBalance = account.Balance,
					transactionId = transactionRecord.Id,
					reference = transactionRecord.Reference
				});

			}
			catch
			{
				transaction.Rollback();

				return StatusCode(500, "Deposit failed. No money was added.");
			}
		}

		[HttpPost("{id}/withdraw")]
		public IActionResult Withdraw(
	int id,
	[FromBody] WithdrawalRecord record)
		{
			var customerIdClaim = User.FindFirst("CustomerId");

			if (customerIdClaim == null)
			{
				return Unauthorized();
			}

			var customerId = int.Parse(customerIdClaim.Value);

			var account = _dbContext.Accounts
	.FirstOrDefault(a =>
		a.Id == id &&
		a.CustomerId == customerId);

			if (account == null)
			{
				return NotFound("Account not found.");
			}

			if (record.Amount <= 0)
			{
				return BadRequest("Withdrawal amount must be greater than zero.");
			}

			if (record.Amount > account.Balance)
			{
				return BadRequest("Insufficient funds.");
			}

			using var transaction = _dbContext.Database.BeginTransaction();

			try
			{
				account.Balance -= record.Amount;

				var transactionRecord = new Transaction
				{
					SenderAccountId = account.Id,
					Amount = record.Amount,
					TransactionType = "Withdrawal",
					Direction = "Debit",
					Status = "Completed",
					Reference = GenerateTransactionReference(),
					CreatedAt = DateTime.UtcNow
				};


				_dbContext.Transactions.Add(transactionRecord);

				_dbContext.SaveChanges();

				transaction.Commit();

				return Ok(new
				{
					message = "Withdrawal successful.",
					accountId = account.Id,
					accountNumber = account.AccountNumber,
					newBalance = account.Balance,
					transactionId = transactionRecord.Id,
					reference = transactionRecord.Reference
				});

			}
			catch
			{
				transaction.Rollback();

				return StatusCode(500, "Withdrawal failed. No money was removed.");
			}
		}

		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			var customerIdClaim = User.FindFirst("CustomerId");

			if (customerIdClaim == null)
			{
				return Unauthorized("Customer identity not found.");
			}

			var customerId = int.Parse(customerIdClaim.Value);

			var account = _dbContext.Accounts
				.FirstOrDefault(a =>
					a.Id == id &&
					a.CustomerId == customerId);

			if (account == null)
			{
				return NotFound("Account not found.");
			}

			return Ok(account);
		}

		[HttpGet("me")]
		public IActionResult GetMyAccount()
		{
			var customerIdClaim = User.FindFirst("CustomerId");

			if (customerIdClaim == null)
			{
				return Unauthorized("Customer identity not found.");
			}

			var customerId = int.Parse(customerIdClaim.Value);

			var account = _dbContext.Accounts
				.FirstOrDefault(a => a.CustomerId == customerId);

			if (account == null)
			{
				return NotFound("Account not found.");
			}

			return Ok(new
			{
				accountId = account.Id,
				accountNumber = account.AccountNumber,
				balance = account.Balance,
				customerId = account.CustomerId
			});
		}

		private string GenerateTransactionReference()
		{
			return $"TRX-{Guid.NewGuid().ToString("N")[..12].ToUpper()}";
		}

		private string GenerateAccountNumber()
		{
			var random = new Random();

			string accountNumber;

			do
			{
				accountNumber = random.Next(100000000, 999999999).ToString();
			}
			while (_dbContext.Accounts.Any(a => a.AccountNumber == accountNumber));

			return accountNumber;
		}
	}
}