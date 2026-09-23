using Amazings_API.Models;

namespace Amazings_API.Records
{
	public class UpdateCustomerResponse
	{
		public Customer newRecord { get; set; }
		public Customer oldRecord { get; set; }
	}
}
