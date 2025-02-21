using Application.Dtos.Request;
using Domain.Models;
using Domain.Primitives;
using Microsoft.AspNetCore.OData.Query;

namespace Application.Services.Interfaces;

public interface IUserRequestService
{
    Task<Guid> CreateRequestAsync(
        string userId,
        Department topic,
        string description, IEnumerable<IFormFile> files, CancellationToken cancellationToken = default);

    Task<UserRequestDto?> GetRequestAsync(Guid requestId,
        CancellationToken cancellationToken = default);

    Task<IQueryable<UserRequestDto>> GetAllRequestsAsync(ODataQueryOptions<UserRequest> options,
        CancellationToken cancellationToken);

    Task CloseRequestAsync(Guid requestId,
        CancellationToken cancellationToken = default);

    IQueryable<GetUserRequestDto> GetUserRequestsByUserIdAsync(string userId,
        ODataQueryOptions<GetUserRequestDto> queryOptions);

    Task<List<UserRequest>> GetAllUserRequestsByUserIdAsync(string userId);

    public Task UpdateRequest(Guid requestId, UpdateUserRequestDto updateRequest, IEnumerable<IFormFile> files,
        CancellationToken cancellationToken = default);
}