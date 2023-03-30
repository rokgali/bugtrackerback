using bugtrackerback.Entities.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bugtrackerback.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketDTO createTicketDTO)
        {


            return Ok();
        }
    }
}
