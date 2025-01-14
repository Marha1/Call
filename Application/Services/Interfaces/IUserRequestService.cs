using Application.Dtos.Request;
using Domain.Models;
using Microsoft.AspNetCore.OData.Query;

namespace Application.Services.Interfaces;

public interface IUserRequestService
{
    Task<Guid> CreateRequestAsync(
        string userId,
        string topic,
        string description , IEnumerable<IFormFile> files, CancellationToken cancellationToken = default);
    
    Task<UserRequestDto?> GetRequestAsync(Guid requestId, 
        CancellationToken cancellationToken = default);
    Task<IQueryable<UserRequestDto>> GetAllRequestsAsync(ODataQueryOptions<UserRequest> options, 
        CancellationToken cancellationToken);
    Task CloseRequestAsync(Guid requestId, 
        CancellationToken cancellationToken = default);

    Task<IQueryable<UserRequestDto>> GetUserRequestsByUserIdAsync(string userId,
        ODataQueryOptions<UserRequestDto> queryOptions);

}