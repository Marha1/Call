using System.Security.Claims;
using Application.Dtos.Request;
using Application.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Application.Controllers;

[Authorize(Roles = "User")]
[ApiController]
[Route("api/[controller]")]
public class UserRequestController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserRequestService _userRequestService;
    private CancellationToken _cancellationToken = default;

    public UserRequestController(IUserRequestService userRequestService, IMapper mapper)
    {
        _userRequestService = userRequestService;
        _mapper = mapper;
    }

    /// <summary>
    ///     Добавление
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="files"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    [HttpPost("AddRequest")]
    public async Task<IActionResult> AddRequest([FromForm] UserRequestCreateDto requestDto,
        [FromForm] IEnumerable<IFormFile> files)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException();
            if (string.IsNullOrEmpty(userId)) return Unauthorized("User not authorized");

            // Вызов сервиса с DTO и идентификатором пользователя
            var requestId =
                await _userRequestService.CreateRequestAsync(userId, requestDto.Topic, requestDto.Description, files);

            return Ok(new { Message = "Request added successfully", RequestId = requestId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    ///     Обновление тикета(текста)
    /// </summary>
    /// <param name="requestId"></param>
    /// <param name="files"></param>
    /// <param name="requestDto"></param>
    /// <returns></returns>
    [HttpPut("update-request")]
    public async Task<IActionResult> UpdateRequest(
        [FromQuery] Guid requestId,
        [FromForm] IEnumerable<IFormFile> files,
        [FromForm] UpdateUserRequestDto requestDto)
    {
        try
        {
            await _userRequestService.UpdateRequest(requestId, requestDto, files, default);

            return Ok(new { Message = "Request updated successfully", RequestId = requestId });
        }
        catch (Exception ex)
        {
            // Логирование ошибки (по возможности)
            Console.WriteLine($"Error updating request: {ex.Message}");

            return StatusCode(500, "Internal server error");
        }
    }


    /// <summary>
    ///     Отмена тикета
    /// </summary>
    /// <param name="requestId"></param>
    /// <returns></returns>
    [HttpDelete("CancelRequest/{requestId}")]
    public async Task<IActionResult> CancelRequest(Guid requestId)
    {
        try
        {
            // Отмена запроса
            await _userRequestService.CloseRequestAsync(requestId, default);
            return Ok(new { Message = "Request successfully canceled." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = "Request not found." });
        }
        catch (ArgumentNullException)
        {
            return BadRequest(new { Message = "Invalid request ID." });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred while canceling the request." });
        }
    }


    /// <summary>
    ///     Получение тикета по Id
    /// </summary>
    /// <param name="requestId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{requestId:guid}")]
    public async Task<IActionResult> GetRequest(Guid requestId, CancellationToken cancellationToken)
    {
        var userRequest = await _userRequestService.GetRequestAsync(requestId, cancellationToken);

        if (userRequest == null)
            return NotFound("Request not found");

        return Ok(userRequest);
    }

    [HttpGet]
    [EnableQuery]
    public async Task<IActionResult> GetUserRequests(ODataQueryOptions<GetUserRequestDto> queryOptions)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { Message = "User ID not found in token." });

        // Получаем все тикеты или отфильтрованные в зависимости от наличия фильтра
        var userRequests = await _userRequestService.GetUserRequestsByUserIdAsync(userId, queryOptions);
        return Ok(userRequests);
    }



    /*[HttpGet] для админа
    [EnableQuery]
    public async Task<IActionResult> GetRequests(ODataQueryOptions<UserRequest> queryOptions)
    {
        try
        {
            var userRequests = await _userRequestService.GetAllRequestsAsync(queryOptions, default);
            return Ok(userRequests);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }*/
}