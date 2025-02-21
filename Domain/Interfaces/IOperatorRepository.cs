using Domain.Models;
using Microsoft.AspNetCore.OData.Query;

namespace Domain.Interfaces;

public interface IOperatorRepository
{
    Task<List<UserRequest>> GetRequestsByDepartment(string id);
    Task TakeRequest(string operatorId, Guid requestId);

    Task<IQueryable<UserRequest>> GetRequestsByDepartmentAsync(string operatorId,
        ODataQueryOptions<UserRequest> queryOptions, CancellationToken cancellationToken = default);
}