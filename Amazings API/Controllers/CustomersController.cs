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
            try
            {
                var Customer = new Customer()
                {
                    FirstName = record.FirstName,
                    LastName = record.LastName,
                    Email = record.Email,
                    Phone = record.Phone
                };
                _dbContext.Customers.Add(Customer);
                _dbContext.SaveChanges();
                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var customers = _dbContext.Customers.ToList();
                return Ok(customers);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
