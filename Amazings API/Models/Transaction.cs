namespace Amazings_API.Models
{
	public class Transaction
	{
		public int Id { get; set; }

		public int? SenderAccountId { get; set; }

		public int? ReceiverAccountId { get; set; }

		public decimal Amount { get; set; }

		public string TransactionType { get; set; }

		public string Direction { get; set; }

		public string Status { get; set; }

		public string Reference { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}
