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

    // POST /api/examples
    [HttpPost]
    public async Task<IActionResult> AddExample([FromBody] ExampleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await dbService.AddExampleAsync(request, cancellationToken);
            return Created();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }
}



