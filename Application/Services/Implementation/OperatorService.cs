using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.OData.Query;

namespace Application.Services.Implementation;

public class OperatorService
{
    private readonly IOperatorRepository _operatorRepository;

    public OperatorService(IOperatorRepository operatorRepository)
    {
        _operatorRepository = operatorRepository;
    }

    public async Task<List<UserRequest>> GetRequestsByDepartmentAsync(string operatorId)
    {
        return await _operatorRepository.GetRequestsByDepartment(operatorId);
    }

    public async Task<IQueryable<UserRequest>> GetRequestsByDepartmentAsync(string operatorId,
        ODataQueryOptions<UserRequest> queryOptions, CancellationToken cancellationToken = default)
    {
        return await _operatorRepository.GetRequestsByDepartmentAsync(operatorId, queryOptions, cancellationToken);
    }
    public async Task<UserRequest?> GetCurrentRequestAsync(string operatorId)
    {
        return await _operatorRepository.GetCurrentRequest(operatorId);
    }

    public async Task TakeRequestAsync(string operatorId, Guid requestId)
    {
        await _operatorRepository.TakeRequest(operatorId, requestId);
    }
}