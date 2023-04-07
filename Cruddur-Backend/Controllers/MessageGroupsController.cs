using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cruddur_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageGroupsController : ControllerBase
    {

        private readonly ILogger<MessageGroupsController> logger;

    }
}
