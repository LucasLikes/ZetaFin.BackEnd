using Microsoft.AspNetCore.Mvc;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;

namespace ZetaFin.API.Controllers;

[ApiController]
[Route("api/pre-registrations")]
public class PreRegistrationController : ControllerBase
{
    private readonly IPreRegistrationService _service;

    public PreRegistrationController(IPreRegistrationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Registra interesse na lista de espera da ZetaFin.
    /// Endpoint público — não requer autenticação.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PreRegistrationResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreatePreRegistrationDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("já está na lista"))
        {
            return Conflict(new { error = "whatsapp_already_registered", message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return UnprocessableEntity(new { error = "validation_error", message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new { error = "validation_error", message = ex.Message });
        }
    }

    /// <summary>
    /// Lista todos os pré-cadastros. Requer autenticação.
    /// </summary>
    [HttpGet]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(typeof(IEnumerable<PreRegistrationResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// Busca pré-cadastro por ID. Requer autenticação.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(typeof(PreRegistrationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var all = await _service.GetAllAsync();
        var item = all.FirstOrDefault(x => x.Id == id);
        if (item is null) return NotFound();
        return Ok(item);
    }
}
