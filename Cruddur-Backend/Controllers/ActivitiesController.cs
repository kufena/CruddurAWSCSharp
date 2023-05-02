using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CruddurSQL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.CompilerServices;
using Cruddur_Backend.Models;

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
        public IEnumerable<Activities> Get()
        {
            if (User != null)
            {
                logger.LogInformation($"Current user is {User}");
                foreach (var c in User.Claims)
                {
                    logger.LogInformation($"Claim {c}");
                }
            }
            else
            {
                logger.LogInformation("User is NULL!!! What a bore.");
            }

            Activities model1 = new Activities()
            {
                uuid = Guid.NewGuid(),
                created_at = DateTime.Now,
                expires_at = DateTime.Now + TimeSpan.FromDays(10),
                handle = "Andy Douglas",
                likes_count = 4,
                replies_count = 0,
                reposts_count = 0,
                message = "Hello this is activity model 1",
                replies = new List<Replies>()
            };
            return new List<Activities> { model1 };
        }
    }
}
