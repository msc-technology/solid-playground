using Microsoft.AspNetCore.Mvc;
using PathOptimization;
using PathOptimization.Registry;
using SolidPathOptimization.Models;
using System.Text.Json;

namespace SolidPathOptimization.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FindPathController : ControllerBase
    {
        private readonly ILogger<FindPathController> Logger;
        private readonly IPathFinderFactory PathFinderFactory;

        public FindPathController(ILogger<FindPathController> logger, IPathFinderFactory pathFinderFactory)
        {
            Logger = logger;
            PathFinderFactory = pathFinderFactory;
        }

        [HttpPost(nameof(Vehicles.Plane))]
        public ObjectResult Plane([FromBody] PathFindingRequest pathFindingRequest)
        {
            return HandlePathRequest(Vehicles.Plane, pathFindingRequest);
        }

        [HttpPost(nameof(Vehicles.Vessel))]
        public ObjectResult Vessel([FromBody] PathFindingRequest pathFindingRequest)
        {
            return HandlePathRequest(Vehicles.Vessel, pathFindingRequest);
        }

        private ObjectResult HandlePathRequest(Vehicles vehicle, PathFindingRequest pathFindingRequest)
        {
            if (pathFindingRequest is null)
            {
                Logger.LogError("Bad request");
                return BadRequest("Bad request");
            }

            IPathFinder? pathFinder = PathFinderFactory.Create(vehicle, pathFindingRequest?.Map!);
            if (pathFinder is null)
            {
                Logger.LogError("Invalid Vehicle");
                return BadRequest("Invalid Vehicle");
            }
            else
            {
                return ComputePath(pathFinder, pathFindingRequest!);
            }
        }

        private ObjectResult ComputePath(IPathFinder pathFinder, PathFindingRequest pathFindingRequest)
        {
            try
            {
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