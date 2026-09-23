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
	public class TransactionsController : ControllerBase

	{
		private readonly ApplicationDbContext _dbContext;

		public TransactionsController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		private string GenerateTransactionReference()
		{
			return $"TRX-{Guid.NewGuid().ToString("N")[..12].ToUpper()}";
		}

		[HttpPost("transfer")]
		public IActionResult Transfer([FromBody] TransferRecord record)
		{
			if (record.SenderAccountId == record.ReceiverAccountId)
			{
				return BadRequest("Sender and receiver accounts cannot be the same.");
			}

			if (record.Amount <= 0)
			{
				return BadRequest("Transfer amount must be greater than zero.");
			}

			// Get the CustomerId from the logged-in user's JWT
			var customerIdClaim = User.FindFirst("CustomerId");

			if (customerIdClaim == null)
			{
				return Unauthorized("Customer identity not found.");
			}

			var customerId = int.Parse(customerIdClaim.Value);

			// Make sure the logged-in customer owns the sender account
			var sender = _dbContext.Accounts
				.FirstOrDefault(a =>
					a.Id == record.SenderAccountId &&
					a.CustomerId == customerId);

			if (sender == null)
			{
				return NotFound("Sender account not found or does not belong to you.");
			}

			// The receiver can belong to another customer
			var receiver = _dbContext.Accounts
				.FirstOrDefault(a => a.Id == record.ReceiverAccountId);

			if (receiver == null)
			{
				return NotFound("Receiver account not found.");
			}

			if (sender.Balance < record.Amount)
			{
				return BadRequest("Insufficient funds.");
			}

			using var transaction = _dbContext.Database.BeginTransaction();

			try
			{
				sender.Balance -= record.Amount;
				receiver.Balance += record.Amount;

				var transactionRecord = new Transaction
				{
					SenderAccountId = sender.Id,
					ReceiverAccountId = receiver.Id,
					Amount = record.Amount,
					TransactionType = "Transfer",
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
					message = "Transfer successful.",
					reference = transactionRecord.Reference,
					senderAccount = sender.AccountNumber,
					receiverAccount = receiver.AccountNumber,
					amount = record.Amount,
					senderNewBalance = sender.Balance,
					receiverNewBalance = receiver.Balance,
					transactionId = transactionRecord.Id
				});

			}
			catch
			{
				transaction.Rollback();

				return StatusCode(500, "Transfer failed. No money was moved.");
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
				.FirstOrDefault(a => a.CustomerId == customerId);

			if (account == null)
			{
				return NotFound("Account not found.");
			}

			var transaction = _dbContext.Transactions
				.FirstOrDefault(t =>
					t.Id == id &&
					(t.SenderAccountId == account.Id ||
					 t.ReceiverAccountId == account.Id));

			if (transaction == null)
			{
				return NotFound("Transaction not found.");
			}

			return Ok(transaction);
		}


		[HttpGet("account/{accountId}")]
		public IActionResult GetAccountTransactions(int accountId)
		{
			var customerIdClaim = User.FindFirst("CustomerId");

			if (customerIdClaim == null)
			{
				return Unauthorized("Customer identity not found.");
			}

			var customerId = int.Parse(customerIdClaim.Value);

			var account = _dbContext.Accounts
				.FirstOrDefault(a =>
					a.Id == accountId &&
					a.CustomerId == customerId);

			if (account == null)
			{
				return NotFound("Account not found.");
			}

			var transactions = _dbContext.Transactions
				.Where(t =>
					t.SenderAccountId == accountId ||
					t.ReceiverAccountId == accountId)
				.OrderByDescending(t => t.CreatedAt)
				.ToList();

			return Ok(transactions);
		}

		[HttpGet("me")]
		public IActionResult GetMyTransactions()
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

			var transactions = _dbContext.Transactions
				.Where(t =>
					t.SenderAccountId == account.Id ||
					t.ReceiverAccountId == account.Id)
				.OrderByDescending(t => t.CreatedAt)
				.ToList();

			return Ok(transactions);
		}
	}
}
