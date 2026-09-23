using System.ComponentModel.DataAnnotations;

namespace Amazings_API.Models
{
	public class User
	{
		[Key]
		public int Id { get; set; }

		public int CustomerId { get; set; }

		public string Username { get; set; }

		public string PasswordHash { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}
