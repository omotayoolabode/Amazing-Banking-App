namespace Amazings_API.Models
{
	public class Account
	{
		public int Id { get; set; }

		public int CustomerId { get; set; }

		public string AccountNumber { get; set; }

		public decimal Balance { get; set; }
	}
}
