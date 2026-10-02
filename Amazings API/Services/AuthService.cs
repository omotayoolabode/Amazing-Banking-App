using Amazings_API.Models;
using Amazings_API.Persistence.Data;
using Amazings_API.Records;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Amazings_API.Services
{
	public class RegistrationResult
	{
		public bool Succeeded { get; init; }

		public string Message { get; init; } = string.Empty;

		public int? UserId { get; init; }

		public string? Username { get; init; }

		public int? CustomerId { get; init; }

		public static RegistrationResult Fail(string message) =>
			new() { Succeeded = false, Message = message };

		public static RegistrationResult Ok(User user) =>
			new()
			{
				Succeeded = true,
				Message = "Registration successful.",
				UserId = user.Id,
				Username = user.Username,
				CustomerId = user.CustomerId
			};
	}

	public class AuthService
	{
		public const string CustomerRole = "Customer";

		private readonly ApplicationDbContext _dbContext;
		private readonly PasswordHasher<User> _passwordHasher;

		public AuthService(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
			_passwordHasher = new PasswordHasher<User>();
		}

		public RegistrationResult Register(RegisterRecord record)
		{
			var firstName = record.FirstName?.Trim() ?? string.Empty;
			var lastName = record.LastName?.Trim() ?? string.Empty;
			var email = record.Email?.Trim() ?? string.Empty;
			var phone = record.Phone?.Trim() ?? string.Empty;
			var username = record.Username?.Trim() ?? string.Empty;
			var password = record.Password ?? string.Empty;

			if (firstName.Length == 0 ||
				lastName.Length == 0 ||
				email.Length == 0 ||
				phone.Length == 0 ||
				username.Length == 0)
			{
				return RegistrationResult.Fail(
					"All fields are required (whitespace alone is not allowed).");
			}

			if (!IsPasswordValid(password))
			{
				return RegistrationResult.Fail(
					"Password must be at least 8 characters and contain at least one letter and one number.");
			}

			var emailLower = email.ToLower();
			var existingEmail = _dbContext.Customers
				.Any(c => c.Email.ToLower() == emailLower);

			if (existingEmail)
			{
				return RegistrationResult.Fail("That email is already registered.");
			}

			var existingUser = _dbContext.Users
				.Any(u => u.Username == username);

			if (existingUser)
			{
				return RegistrationResult.Fail("That username is taken.");
			}

			using var transaction = _dbContext.Database.BeginTransaction();

			try
			{
				var customer = new Customer
				{
					FirstName = firstName,
					LastName = lastName,
					Email = email,
					Phone = phone
				};

				_dbContext.Customers.Add(customer);
				_dbContext.SaveChanges();

				var user = new User
				{
					CustomerId = customer.Id,
					Username = username,
					Role = CustomerRole,
					CreatedAt = DateTime.UtcNow
				};

				user.PasswordHash = _passwordHasher.HashPassword(user, password);

				_dbContext.Users.Add(user);
				_dbContext.SaveChanges();

				transaction.Commit();

				return RegistrationResult.Ok(user);
			}
			catch (DbUpdateException)
			{
				transaction.Rollback();
				return RegistrationResult.Fail(
					"That username or email is already registered.");
			}
			catch
			{
				transaction.Rollback();
				return RegistrationResult.Fail(
					"Registration failed. No account was created.");
			}
		}

		private static bool IsPasswordValid(string password)
		{
			if (password.Length < 8)
			{
				return false;
			}

			return password.Any(char.IsLetter) && password.Any(char.IsDigit);
		}
	}
}
