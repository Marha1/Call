using System.Text;
using Application.Services.Implementation;
using Infrastructure;

namespace Application.Middleware;

public class ProfanityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ProfanityFilterService _profanityFilter;
    private readonly ILogger<ProfanityMiddleware> _logger;

    public ProfanityMiddleware(RequestDelegate next, ProfanityFilterService profanityFilter, ILogger<ProfanityMiddleware> logger)
    {
        _next = next;
        _profanityFilter = profanityFilter;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Method == HttpMethods.Post ||
            context.Request.Method == HttpMethods.Put ||
            context.Request.Method == HttpMethods.Patch)
        {
            context.Request.EnableBuffering(); // Позволяет повторно читать тело запроса

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; // Восстанавливаем позицию в потоке, чтобы контроллер мог его прочитать

            if (_profanityFilter.ContainsProfanity(requestBody))
            {
                _logger.LogWarning("Обнаружен мат в запросе: {RequestBody}", requestBody);
                var errorResponse = new 
                {
                    Message = "Иди отсюда,Быдло!",
                    StatusCode = StatusCodes.Status400BadRequest
                };
    
                // Устанавливаем код ошибки и тип контента
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
    
                // Отправляем JSON-ответ
                await context.Response.WriteAsJsonAsync(errorResponse);
    
                return;
            }
        }

        await _next(context);
    }
}
