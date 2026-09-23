using System.Text;
using Amazings_API.Models;
using Amazings_API.Persistence.Data;
using Amazings_API.Records;
using Microsoft.AspNetCore.Mvc;

namespace Amazings_API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CustomersController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;

		public CustomersController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpPost]
		public IActionResult Create([FromBody] CreateCustomerRecord record)
		{
			var customer = new Customer
			{
				FirstName = record.FirstName,
				LastName = record.LastName,
				Email = record.Email,
				Phone = record.Phone
			};

			_dbContext.Customers.Add(customer);
			_dbContext.SaveChanges();

			return Ok(customer);
		}

		[HttpGet]
		public IActionResult GetAll()
		{
			var customers = _dbContext.Customers.ToList();

			return Ok(customers);
		}

		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			var customer = _dbContext.Customers
				.FirstOrDefault(c => c.Id == id);

			if (customer == null)
			{
				return NotFound();
			}

			return Ok(customer);
		}

		[HttpPut("{id}")]
		public IActionResult Update(
			int id,
			[FromBody] CreateCustomerRecord record)
		{
			var customer = _dbContext.Customers
				.FirstOrDefault(c => c.Id == id);

			if (customer == null)
			{
				return NotFound();
			}

			customer.FirstName = record.FirstName;
			customer.LastName = record.LastName;
			customer.Email = record.Email;
			customer.Phone = record.Phone;

			_dbContext.SaveChanges();

			return Ok(customer);
		}

		[HttpPatch("{id}")]
		public IActionResult PartialUpdate(
			int id,
			[FromBody] CreateCustomerRecord record)
		{
			var customer = _dbContext.Customers
				.FirstOrDefault(c => c.Id == id);

			if (customer == null)
			{
				return NotFound();
			}

			if (record.FirstName != customer.FirstName)
			{
				customer.FirstName = record.FirstName;
			}

			if (record.LastName != customer.LastName)
			{
				customer.LastName = record.LastName;
			}

			if (record.Email != customer.Email)
			{
				customer.Email = record.Email;
			}

			if (record.Phone != customer.Phone)
			{
				customer.Phone = record.Phone;
			}

			_dbContext.SaveChanges();

			return Ok(customer);
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var customer = _dbContext.Customers
				.FirstOrDefault(c => c.Id == id);

			if (customer == null)
			{
				return NotFound();
			}

			_dbContext.Customers.Remove(customer);
			_dbContext.SaveChanges();

			return Ok(customer);
		}
	}
}
