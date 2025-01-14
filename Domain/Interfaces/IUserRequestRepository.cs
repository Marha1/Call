using Domain.Models;
using Microsoft.AspNetCore.OData.Query;

namespace Domain.Interfaces;

public interface IUserRequestRepository : IRepository<UserRequest>
{
    public Task<UserRequest?> GetRequestWithAttachmentsAsync(Guid requestId,
        CancellationToken cancellationToken = default);

    Task<IQueryable<UserRequest>> GetUserRequestsAsync(string userId,
        ODataQueryOptions<UserRequest> queryOptions,
        CancellationToken cancellationToken = default);
}