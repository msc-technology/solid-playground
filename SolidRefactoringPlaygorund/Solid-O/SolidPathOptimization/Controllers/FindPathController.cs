using Microsoft.AspNetCore.Mvc;
using PathOptimization.Factories;
using PathOptimization.PathFinders;
using SolidPathOptimization.Models;
using System.Text.Json;

namespace SolidPathOptimization.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FindPathController : ControllerBase
    {
        private readonly ILogger<FindPathController> Logger;
        private readonly PathFinderFactory PathFinderFactory;

        public FindPathController(ILogger<FindPathController> logger, PathFinderFactory pathFinderFactory)
        {
            Logger = logger;
            PathFinderFactory = pathFinderFactory;
        }

        [HttpPost("Plane")]
        public ObjectResult Plane([FromBody] PathFindingRequest pathFindingRequest)
        {
            if (pathFindingRequest is null)
            {
                Logger.LogError("Bad request");
                return BadRequest(string.Empty);
            }

            var pathFinder = PathFinderFactory.Create("plane", pathFindingRequest.Map);
            return ComputePath(pathFinder, pathFindingRequest);
        }

        [HttpPost("Vessel")]
        public ObjectResult Vessel([FromBody] PathFindingRequest pathFindingRequest)
        {
            if (pathFindingRequest is null)
            {
                Logger.LogError("Bad request");
                return BadRequest(string.Empty);
            }

            var pathFinder = PathFinderFactory.Create("vessel", pathFindingRequest.Map);
            return ComputePath(pathFinder, pathFindingRequest);
        }

        private ObjectResult ComputePath(PathFinder pathFinder, PathFindingRequest pathFindingRequest)
        {
            try
            {
                pathFinder.ValidateInputCoordinates(pathFindingRequest.Start, pathFindingRequest.Target);
                var path = pathFinder.Find(pathFindingRequest.Start, pathFindingRequest.Target);
                Logger.LogInformation("Path finding completed {Path}", path);
                return Ok(JsonSerializer.Serialize(path));
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogError(ex, "Cannot complete request {Request}", pathFindingRequest);
                return UnprocessableEntity(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Logger.LogError(ex, "Invalid request {Request}", pathFindingRequest);
                return BadRequest(ex.Message);
            }
        }
    }
}