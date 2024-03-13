using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Subscriber_API.ModelMethods;
using Subscriber_API.Models;
using System.Reflection;
namespace Subscriber_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriberController : ControllerBase
    {
        // GET: SubscriberController
        [HttpGet]
        public List<Subscriber> GetSubscribers()
        {            
            return SubscriberMethods.GetSubscribers(out string errorMsg);            
        }
        [HttpGet("{id:int}")]
        public Subscriber GetSubscriber(int id)
        {
            return SubscriberMethods.GetById(id, out string errorMsg);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult InsertSubscriber([FromBody] Subscriber sub)
        {
            if(SubscriberMethods.InsertSubscriber(sub, out string errorMsg) == 1)
            {
                // Success
                return CreatedAtAction("InsertSubscriber", new { URI = "I am an URI" }, sub);
            }
            else
            {
                // Fail
                return BadRequest(errorMsg);
            }
        }
        [HttpPut]
        public IActionResult UpdateSubscriber([FromBody] Subscriber sub)
        {
            // Mock error
            //return NotFound($"Error:D Subscriber ID not found\n{sub.FirstName}");

            if (SubscriberMethods.GetById(sub.SubscriberId, out string errorMsg1) != null)
            {
                if(SubscriberMethods.UpdateSubscriber(sub, out string errorMsg2) == 1)
                {
                    return CreatedAtAction("UpdateSubscriber", new { ID = sub.SubscriberId }, sub);
                }
                else
                {
                    return BadRequest(errorMsg2);
                }
            }
            // Id not found
            return NotFound($"Error:D Subscriber ID not found\n{errorMsg1}");
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteSubscriber(int id)
        {            
            if(SubscriberMethods.DeleteSubscriber(id, out string errorMsg) == 1)
            {
                return Ok("Delete successful!");
            }
            else
            {
                return BadRequest(errorMsg);
            }
        }
    }
}
