using EFCore_CodeFirst_Test_Example.DTOs;
using EFCore_CodeFirst_Test_Example.Exceptions;
using EFCore_CodeFirst_Test_Example.Services;
using Microsoft.AspNetCore.Mvc;

namespace EFCore_CodeFirst_Test_Example.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamplesController(IDbService dbService) : ControllerBase
{
    // GET /api/examples?filter=abc
    [HttpGet]
    public async Task<IActionResult> GetExamples([FromQuery] string? filter, CancellationToken cancellationToken)
    {
        var result = await dbService.GetExamplesAsync(filter, cancellationToken);
        return Ok(result);
    }

    // GET /api/examples/5?limitCount=5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetExampleById(
        [FromRoute] int id, 
        [FromQuery] int? limitCount, 
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await dbService.GetExampleByIdAsync(id, limitCount, cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }

    // GET /api/examples/advanced?name=abc&minYear=2020&maxValue=100&sortBy=name&descending=true&pageNumber=1&pageSize=10
    [HttpGet("advanced")]
    public async Task<IActionResult> GetAdvancedExamples(
        [FromQuery] string? name,
        [FromQuery] int? minYear,
        [FromQuery] decimal? maxValue,
        [FromQuery] string? sortBy,
        [FromQuery] bool descending = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("PageNumber and PageSize must be greater than or equal to 1.");
        }

        var result = await dbService.GetAdvancedExamplesAsync(
            name, minYear, maxValue, sortBy, descending, pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }

    // GET /api/examples/parents/{parentId:int}/stats
    [HttpGet("parents/{parentId:int}/stats")]
    public async Task<IActionResult> GetParentStats(
        [FromRoute] int parentId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await dbService.GetParentStatsAsync(parentId, cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}





