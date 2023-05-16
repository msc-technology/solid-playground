using Microsoft.AspNetCore.Mvc;
using PathOptimization;
using PathOptimization.PathFinders;
using SolidPathOptimization.Models;
using System.Text.Json;

namespace SolidPathOptimization.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FindPathController : ControllerBase
    {
        private ILogger<FindPathController> Logger { get; }
        private IPathFinderFactory PathFinderFactory { get; }

        public FindPathController(ILogger<FindPathController> logger, IPathFinderFactory pathFinderFactory)
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

            var pathFinder = PathFinderFactory.Create(pathFindingRequest.Map);
            return ComputePath(pathFinder, pathFindingRequest, "plane");
        }

        [HttpPost("Vessel")]
        public ObjectResult Vessel([FromBody] PathFindingRequest pathFindingRequest)
        {
            if (pathFindingRequest is null)
            {
                Logger.LogError("Bad request");
                return BadRequest(string.Empty);
            }

            var pathFinder = PathFinderFactory.Create(pathFindingRequest.Map);
            return ComputePath(pathFinder, pathFindingRequest, "vessel");
        }

        private ObjectResult ComputePath(PathFinder? pathFinder, PathFindingRequest pathFindingRequest, string vehicle)
        {
            if (pathFinder is null)
            {
                Logger.LogInformation("Invalid request {Request}", pathFindingRequest);
                return BadRequest("Invalid request");
            }

            try
            {
                pathFinder.ValidateInputCoordinates(pathFindingRequest.Start, pathFindingRequest.Target, vehicle);
                var path = pathFinder.Find(pathFindingRequest.Start, pathFindingRequest.Target, vehicle);
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