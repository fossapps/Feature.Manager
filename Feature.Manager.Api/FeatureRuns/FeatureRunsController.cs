using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.Manager.Api.FeatureRuns.Exceptions;
using Feature.Manager.Api.FeatureRuns.ViewModels;
using Feature.Manager.Api.Features.Exceptions;
using Feature.Manager.Api.Features.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Feature.Manager.Api.FeatureRuns
{
    [ApiController]
    [Route("/api/[controller]")]
    public class FeatureRunsController : ControllerBase
    {
        private readonly IFeatureRunService _featureRunService;

        public FeatureRunsController(IFeatureRunService featureRunService)
        {
            _featureRunService = featureRunService;
        }

        [HttpGet("{featId}/runs")]
        [ProducesResponseType(typeof(IEnumerable<FeatureRun>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFeatureRuns(string featId)
        {
            try
            {
                return Ok(await _featureRunService.GetRunsForFeatureByFeatId(featId));
            }
            catch (UnknownDbException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "internal server error"
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "internal server error"
                });
            }
        }

        [HttpGet("{runId}")]
        [ProducesResponseType(typeof(FeatureRun), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRunById(string runId)
        {
            try
            {
                var result = await _featureRunService.GetById(runId);
                if (result == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Not found",
                    });
                }

                return Ok(result);
            }
            catch (UnknownDbException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "internal server error"
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "internal server error"
                });
            }
        }

        [HttpGet("runningFeatures")]
        [ProducesResponseType(typeof(IEnumerable<RunningFeature>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRunningFeatures()
        {
            try
            {
                var result = await _featureRunService.GetRunningFeatures();
                return Ok(result);
            }
            catch (UnknownDbException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "internal server error"
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "internal server error"
                });
            }
        }

        [HttpPost("stop")]
        [ProducesResponseType(typeof(IEnumerable<RunningFeature>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StopRun(StopFeatureRunRequest request)
        {
            try
            {
                var result = await _featureRunService.StopFeatureRun(request);
                return Ok(result);
            }
            catch (InvalidStopResultValueException)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Stop Request value is invalid"
                });
            }
            catch (FeatureRunNotFoundException)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "feature run not found"
                });
            }
            catch (UnknownDbException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "internal server error"
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "internal server error"
                });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(FeatureRun), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddFeatureRun(CreateFeatureRunRequest request)
        {
            try
            {
                var result = await _featureRunService.CreateFeatureRun(request);
                return Ok(result);
            }
            catch (FeatureAlreadyRunningException)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Feature is already running"
                });
            }
            catch (FeatureNotFoundException)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "feature id is invalid"
                });
            }
            catch (UnknownDbException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "internal server error"
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "internal server error"
                });
            }
        }
    }
}
