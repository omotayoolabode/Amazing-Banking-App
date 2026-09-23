namespace Amazings_API.Records
{
	public record RegisterRecord
	{
		public int CustomerId { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }
	}
}
