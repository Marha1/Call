using Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
namespace Infrastructure.ChatHub;
/// <summary>
/// Класс для чата user/operator
/// </summary>
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly ProfanityFilterService _profanityFilter;

    public ChatHub(IChatService chatService,ProfanityFilterService _profanity)
    {
        _chatService = chatService;
        _profanityFilter = _profanity;
    }
    /// <summary>
    /// Для отправки сообщения
    /// </summary>
    /// <param name="requestId"></param>
    /// <param name="userId"></param>
    /// <param name="message"></param>
    public async Task SendMessage(string requestId, string userId, string message)
    {
        if (_profanityFilter.ContainsProfanity(message))
        {
            message = "*****";
        }
        _profanityFilter.ContainsProfanity(message);
        // Сохраняем сообщение через сервис
        await _chatService.SaveMessageAsync(Guid.Parse( requestId), userId, message);

        // Рассылаем сообщение всем участникам группы
        await Clients.Group(requestId.ToString()).SendAsync("ReceiveMessage", userId, message);
    }

    /// <summary>
    /// Закрытие тикета/чата
    /// </summary>
    /// <param name="requestId"></param>
    public async Task CloseChat(string requestId)
    {
        // Закрываем тикет
        await _chatService.CloseRequestAsync(Guid.Parse( requestId));

        // Оповещаем участников о закрытии чата
        await Clients.Group(requestId.ToString()).SendAsync("ChatClosed", requestId);

        // Запрашиваем у пользователя рейтинг
        await Clients.Group(requestId.ToString()).SendAsync("RequestRating", requestId);

        // Удаляем участников из группы
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, requestId.ToString());
    }

    /// <summary>
    /// Метод для рейтинга
    /// </summary>
    /// <param name="requestId"></param>
    /// <param name="rating"></param>
    public async Task SubmitRating(string requestId, int rating)
    {
        // Сохраняем рейтинг через сервис
        await _chatService.SaveRatingAsync(Guid.Parse( requestId), rating);

        // Уведомляем участников о выставленном рейтинге
        await Clients.Group(requestId.ToString()).SendAsync("RatingSubmitted", requestId, rating);
    }

    /// <summary>
    /// Создание/подключение к чату
    /// </summary>
    /// <param name="requestId"></param>
    public async Task JoinChat(string requestId)
    {
        // Подключаем пользователя к группе
        await Groups.AddToGroupAsync(Context.ConnectionId, requestId.ToString());

        // Отправляем уведомление о подключении
        await Clients.Group(requestId.ToString()).SendAsync("UserJoined", Context.ConnectionId);
    }
    /// <summary>
    /// получение сообщений
    /// </summary>
    /// <param name="requestId"></param>
    public async Task LoadChatHistory(string requestId)
    {
        // Получаем историю сообщений из сервиса
        var messages = await _chatService.GetMessagesAsync(Guid.Parse(requestId));

        // Отправляем историю подключенному клиенту
        await Clients.Caller.SendAsync("LoadChatHistory", messages);
    }

    /// <summary>
    /// Отключение от чата при ошибке
    /// </summary>
    /// <param name="exception"></param>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Уведомляем об отключении
        await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);

        // Вызываем базовую логику
        await base.OnDisconnectedAsync(exception);
    }
}
