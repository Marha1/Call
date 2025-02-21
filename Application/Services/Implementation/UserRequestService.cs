using Application.Dtos.Request;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using Domain.Primitives;
using Infrastructure.Repository.Implementation;
using Microsoft.AspNetCore.OData.Query;

namespace Application.Services.Implementation;

public class UserRequestService : IUserRequestService
{
    private readonly AttachmentRepository _attachmentRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly IMapper _mapper;
    private readonly IUserRequestRepository _userRequestRepository;

    public UserRequestService(
        IUserRequestRepository userRequestRepository,
        AttachmentRepository attachmentRepository,
        IAttachmentService attachmentService,
        IMapper mapper)
    {
        _userRequestRepository = userRequestRepository;
        _attachmentRepository = attachmentRepository;
        _attachmentService = attachmentService;
        _mapper = mapper;
    }

    /// <summary>
    ///     Создание
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="topic"></param>
    /// <param name="description"></param>
    /// <param name="files"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Guid> CreateRequestAsync(
        string userId,
        Department topic,
        string description,
        IEnumerable<IFormFile> files,
        CancellationToken cancellationToken = default)
    {
        var userRequest = new UserRequest
        {
            UserId = userId,
            Topic = topic,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            Status = 0
        };

        await _userRequestRepository.AddAsync(userRequest, cancellationToken);
        await _userRequestRepository.SaveChangesAsync(cancellationToken);

        foreach (var file in files)
            await _attachmentService.UploadAttachmentAsync(file, userRequest.Id, cancellationToken);

        await _userRequestRepository.SaveChangesAsync(cancellationToken);
        return userRequest.Id;
    }

    /// <summary>
    ///     Получение с фильтрацией(мб уберется)
    /// </summary>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IQueryable<UserRequestDto>> GetAllRequestsAsync(ODataQueryOptions<UserRequest> options,
        CancellationToken cancellationToken)
    {
        var queryable = await _userRequestRepository.GetQueryableAsync(options, cancellationToken);
        return _mapper.ProjectTo<UserRequestDto>(queryable);
    }

    /// <summary>
    ///     Закрытие тикета
    /// </summary>
    /// <param name="requestId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task CloseRequestAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        if (requestId == Guid.Empty)
            throw new ArgumentNullException(nameof(requestId));

        // Получаем запрос по ID
        var request = await _userRequestRepository.GetByIdAsync(requestId, cancellationToken);
        if (request == null) throw new KeyNotFoundException($"Request with ID {requestId} not found.");
        await _userRequestRepository.DeleteAsync(request, cancellationToken);
        await _userRequestRepository.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    ///     Получение тикетов(с фпомощью фильтрации по id юзера)
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IQueryable<GetUserRequestDto> GetUserRequestsByUserIdAsync(string userId,
        ODataQueryOptions<GetUserRequestDto> queryOptions)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        // Получаем запросы пользователя из репозитория
        var userRequests = _userRequestRepository.GetUserRequestsAsync(userId, null);

        // Преобразуем в DTO
        var userRequestDtos = _mapper.ProjectTo<GetUserRequestDto>(userRequests);

        // Если queryOptions не null, применяем фильтрацию
        if (queryOptions != null)
            userRequestDtos = queryOptions.ApplyTo(userRequestDtos) as IQueryable<GetUserRequestDto>;

        return userRequestDtos;
    }

    /// <summary>
    ///     Изменение тикета
    /// </summary>
    /// <param name="requestId"></param>
    /// <param name="userId"></param>
    /// <param name="topic"></param>
    /// <param name="description"></param>
    /// <param name="files"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task UpdateRequest(Guid requestId, UpdateUserRequestDto updateRequest, IEnumerable<IFormFile> files,
        CancellationToken cancellationToken = default)
    {
        if (updateRequest == null) throw new ArgumentNullException(nameof(updateRequest));
        var existingRequest = await _userRequestRepository.GetByIdAsync(requestId, cancellationToken);
        if (existingRequest == null) throw new Exception($"Request with ID {requestId} not found.");

        _mapper.Map(updateRequest, existingRequest);

        await _userRequestRepository.UpdateAsync(existingRequest, cancellationToken);

        if (files != null && files.Any())
            foreach (var file in files)
                await _attachmentService.UploadAttachmentAsync(file, requestId, cancellationToken);
    }

    /// <summary>
    ///     Получение по Id
    /// </summary>
    /// <param name="requestId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<UserRequestDto?> GetRequestAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        var userRequest = await _userRequestRepository.GetRequestWithAttachmentsAsync(requestId, cancellationToken);
        return userRequest == null ? null : _mapper.Map<UserRequestDto>(userRequest);
    }
}