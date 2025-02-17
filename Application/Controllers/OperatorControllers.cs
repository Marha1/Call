using Domain.Interfaces;
using Infrastructure.ChatHub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Application.Controllers;

[Authorize(Roles = "Operator")]
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatController(IHubContext<ChatHub> hubContext, IChatService chatService)
    {
        _hubContext = hubContext;
        _chatService = chatService;
    }

    /// <summary>
    ///     Отправить сообщение
    /// </summary>
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromQuery] string requestId, [FromQuery] string userId,
        [FromBody] string message)
    {
        await _chatService.SaveMessageAsync(Guid.Parse(requestId), userId, message);
        await _hubContext.Clients.Group(requestId).SendAsync("ReceiveMessage", userId, message);
        return Ok("Сообщение отправлено.");
    }

    /// <summary>
    ///     Закрыть чат
    /// </summary>
    [HttpPost("close")]
    public async Task<IActionResult> CloseChat([FromQuery] string requestId)
    {
        await _chatService.CloseRequestAsync(Guid.Parse(requestId));
        await _hubContext.Clients.Group(requestId).SendAsync("ChatClosed", requestId);
        return Ok("Чат закрыт.");
    }

    /// <summary>
    ///     Отправить рейтинг
    /// </summary>
    [HttpPost("rate")]
    public async Task<IActionResult> SubmitRating([FromQuery] string requestId, [FromQuery] int rating)
    {
        await _chatService.SaveRatingAsync(Guid.Parse(requestId), rating);
        await _hubContext.Clients.Group(requestId).SendAsync("RatingSubmitted", requestId, rating);
        return Ok("Рейтинг отправлен.");
    }

    /// <summary>
    ///     Получить историю чата
    /// </summary>
    [HttpGet("history/{requestId}")]
    public async Task<IActionResult> GetChatHistory(string requestId)
    {
        var messages = await _chatService.GetMessagesAsync(Guid.Parse(requestId));
        return Ok(messages);
    }
}