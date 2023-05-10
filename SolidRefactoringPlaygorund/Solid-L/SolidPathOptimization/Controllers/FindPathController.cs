using Microsoft.AspNetCore.Mvc;
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

        public FindPathController(ILogger<FindPathController> logger)
        {
            Logger = logger;
        }

        [HttpPost("Plane")]
        public ObjectResult Plane([FromBody] PathFindingRequest pathFindingRequest)
        {
            if (pathFindingRequest is null)
            {
                Logger.LogError("Bad request");
                return BadRequest(string.Empty);
            }

            var pathFinder = new PathFinder(pathFindingRequest.Map);
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

            var pathFinder = new PathFinder(pathFindingRequest.Map);
            return ComputePath(pathFinder, pathFindingRequest, "vessel");
        }

        private ObjectResult ComputePath(PathFinder pathFinder, PathFindingRequest pathFindingRequest, string vehicle) 
        { 
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