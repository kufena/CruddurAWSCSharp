using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cruddur_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {

        private readonly ILogger<ActivitiesController> logger;

    }
}
