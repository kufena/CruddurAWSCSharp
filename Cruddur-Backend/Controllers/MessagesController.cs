using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cruddur_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {

        private readonly ILogger<MessagesController> logger;

    }
}
