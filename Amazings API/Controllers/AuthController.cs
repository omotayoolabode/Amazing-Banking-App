using Amazings_API.Models;
using Amazings_API.Persistence.Data;
using Amazings_API.Records;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Amazings_API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly PasswordHasher<User> _passwordHasher;
		private readonly IConfiguration _configuration;

		public AuthController(
			ApplicationDbContext dbContext,
			IConfiguration configuration)
		{
			_dbContext = dbContext;
			_configuration = configuration;
			_passwordHasher = new PasswordHasher<User>();
		}

		[HttpPost("register")]
		public IActionResult Register([FromBody] RegisterRecord record)
		{
			var customer = _dbContext.Customers
				.FirstOrDefault(c => c.Id == record.CustomerId);

			if (customer == null)
			{
				return NotFound("Customer not found.");
			}

			var existingUser = _dbContext.Users
				.FirstOrDefault(u => u.Username == record.Username);

			if (existingUser != null)
			{
				return BadRequest("Username already exists.");
			}

			var user = new User
			{
				CustomerId = record.CustomerId,
				Username = record.Username,
				CreatedAt = DateTime.UtcNow
			};

			user.PasswordHash = _passwordHasher.HashPassword(
				user,
				record.Password
			);

			_dbContext.Users.Add(user);
			_dbContext.SaveChanges();

			return Ok(new
			{
				message = "Registration successful.",
				userId = user.Id,
				username = user.Username,
				customerId = user.CustomerId
			});
		}

		[HttpPost("login")]
		public IActionResult Login([FromBody] LoginRecord record)
		{
			var user = _dbContext.Users
				.FirstOrDefault(u => u.Username == record.Username);

			if (user == null)
			{
				return Unauthorized("Invalid username or password.");
			}

			var passwordResult = _passwordHasher.VerifyHashedPassword(
				user,
				user.PasswordHash,
				record.Password
			);

			if (passwordResult == PasswordVerificationResult.Failed)
			{
				return Unauthorized("Invalid username or password.");
			}

			var token = GenerateToken(user);

			return Ok(new
			{
				message = "Login successful.",
				token = token
			});
		}

		private string GenerateToken(User user)
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.Username),
				new Claim("CustomerId", user.CustomerId.ToString())
			};

			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(
					_configuration["Jwt:Key"]!
				)
			);

			var credentials = new SigningCredentials(
				key,
				SecurityAlgorithms.HmacSha256
			);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddHours(1),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
