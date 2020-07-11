using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.Manager.Api.Features.Exceptions;
using Feature.Manager.Api.Features.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Feature.Manager.Api.Features
{
    [ApiController]
    [Route("/api/[controller]")]
    public class FeatureController : ControllerBase
    {
        private readonly IFeatureService _featureService;

        public FeatureController(IFeatureService featureService)
        {
            _featureService = featureService;
        }

        [HttpGet("{featId}")]
        [ProducesResponseType(typeof(Feature), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeature(string featId)
        {
            try
            {
                var result = await _featureService.GetFeatureByFeatId(featId);
                return Ok(result);
            }
            catch (UnknownDbException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "error querying database"
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "unknown error"
                });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(Feature), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateFeature(CreateFeatureRequest request)
        {
            try
            {
                var result = await _featureService.Create(request);
                return Ok(result);
            }
            catch (FeatureAlreadyExistsException e)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "FeatureId already exists",
                    Detail = "feature id needs to be unique"
                });
            }
            catch (FeatureCreatingFailedException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "failed creating a new feature"
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "unknown error"
                });
            }
        }

        [HttpPost("{featId}/resetFeatToken")]
        [ProducesResponseType(typeof(Feature), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetFeatToken(string featId)
        {
            try
            {
                var result = await _featureService.ResetFeatureToken(featId);
                return Ok(result);
            }
            catch (FeatureNotFoundException e)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "FeatureId not found",
                    Detail = "featId is required to reset token",
                });
            }
            catch (FeatureTokenResetFailedException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "failed creating a new feature"
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "unknown error"
                });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Feature>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFeatureList()
        {
            try
            {
                return Ok(await _featureService.GetAllFeatures());
            }
            catch (UnknownDbException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "unknown error"
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "unknown error"
                });
            }
        }
    }
}
