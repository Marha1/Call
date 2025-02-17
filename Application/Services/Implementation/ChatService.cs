using Application.Dtos;
using Application.Services.Interfaces;
using Domain.Interfaces;
using Domain.Models;
using Domain.Primitives;

namespace Application.Services.Implementation;

public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRequestRepository _requestRepository;

        public ChatService(IChatRepository chatRepository, IUserRequestRepository requestRepository)
        {
            _chatRepository = chatRepository;
            _requestRepository = requestRepository;
        }

        /// <summary>
        /// Сохраняет сообщение в чате.
        /// </summary>
        public async Task SaveMessageAsync(Guid requestId, string userId, string message, CancellationToken cancellationToken = default)
        {
            var chatMessage = new ChatMessage
            {
                TicketId = requestId,
                SenderId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            await _chatRepository.AddAsync(chatMessage, cancellationToken);
            await _chatRepository.SaveChangesAsync();
        }

        

        /// <summary>
        /// Закрывает запрос (тикет).
        /// </summary>
        public async Task CloseRequestAsync(Guid requestId, CancellationToken cancellationToken = default)
        {
            var request = await _requestRepository.GetByIdAsync(requestId, cancellationToken);

            if (request == null)
                throw new KeyNotFoundException($"Запрос с ID {requestId} не найден.");

            request.Status = RequestStatus.Closed;

            await _requestRepository.UpdateAsync(request, cancellationToken);
            await _requestRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Сохраняет рейтинг для чата.
        /// </summary>
        public async Task SaveRatingAsync(Guid requestId, int rating, CancellationToken cancellationToken = default)
        {
            var request = await _requestRepository.GetByIdAsync(requestId, cancellationToken);

            if (request == null)
                throw new KeyNotFoundException($"Запрос с ID {requestId} не найден.");

            request.Raiting = rating;

            await _requestRepository.UpdateAsync(request, cancellationToken);
            await _requestRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Получает список сообщений для чата.
        /// </summary>
        public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(Guid requestId, CancellationToken cancellationToken = default)
        {
            return  await _chatRepository.GetMessagesByRequestIdAsync(requestId);
        }
    }