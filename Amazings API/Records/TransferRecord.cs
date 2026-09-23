namespace Amazings_API.Records
{
	public record TransferRecord
	{
		public int SenderAccountId { get; set; }

		public int ReceiverAccountId { get; set; }

		public decimal Amount { get; set; }
	}
}
