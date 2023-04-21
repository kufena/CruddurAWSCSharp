using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CruddurSQL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.CompilerServices;

namespace Cruddur_Backend.Controllers
{
    [Route("api/activities/")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {

        private readonly ILogger<ActivitiesController> logger;

        public ActivitiesController(ILogger<ActivitiesController> logger)
        {
            this.logger = logger;
        }

        [Authorize]
        [HttpGet("home")]
        public IEnumerable<ActivitiesModel> Get()
        {
            ActivitiesModel model1 = new ActivitiesModel()
            {
                created_at = DateTime.Now,
                expires_at = DateTime.Now + TimeSpan.FromDays(10),
                user_uuid = Guid.NewGuid(),
                uuid = Guid.NewGuid(),
                likes_count = 4,
                replies_count = 0,
                reply_to_activity_uuid = 0,
                reposts_count = 0,
                text = "Hello this is activity model 1"
            };
            return new List<ActivitiesModel> { model1 };
        }
    }
}
