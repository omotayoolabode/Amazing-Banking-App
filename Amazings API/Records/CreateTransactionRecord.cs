namespace Amazings_API.Records
{
	public record CreateTransactionRecord
	{
		public int SenderCustomerId { get; set; }

		public int ReceiverCustomerId { get; set; }

		public decimal Amount { get; set; }
	}
}
