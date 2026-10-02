using System.ComponentModel.DataAnnotations;

namespace Amazings_API.Records
{
	public record RegisterRecord
	{
		[Required]
		[StringLength(50, MinimumLength = 1)]
		public string FirstName { get; set; } = string.Empty;

		[Required]
		[StringLength(50, MinimumLength = 1)]
		public string LastName { get; set; } = string.Empty;

		[Required]
		[EmailAddress]
		[StringLength(256)]
		public string Email { get; set; } = string.Empty;

		[Required]
		[StringLength(20, MinimumLength = 1)]
		public string Phone { get; set; } = string.Empty;

		[Required]
		[RegularExpression(
			@"^[a-zA-Z0-9]{3,30}$",
			ErrorMessage = "Username must be 3 to 30 letters or numbers.")]
		public string Username { get; set; } = string.Empty;

		[Required]
		[MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
		[RegularExpression(
			@"^(?=.*[A-Za-z])(?=.*\d).+$",
			ErrorMessage = "Password must contain at least one letter and one number.")]
		public string Password { get; set; } = string.Empty;
	}
}
