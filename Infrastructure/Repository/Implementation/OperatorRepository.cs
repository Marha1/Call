using Domain.Interfaces;
using Domain.Models;
using Domain.Primitives;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementation;

public class OperatorRepository : BaseRepository<Operator>, IOperatorRepository
{
    public OperatorRepository(ApplicationContext context) : base(context)
    {
    }

    public async Task<List<UserRequest>> GetRequestsByDepartment(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id));

        var department = await _context.Operators
            .Where(x => x.Id == id)
            .Select(x => x.AssignedDepartment)
            .SingleOrDefaultAsync();

        return department == null
            ? new List<UserRequest>()
            : await _context.UserRequests
                .Where(x => x.Topic == department && x.Operator == null && x.Status == RequestStatus.Open)
                .Include(x => x.Attachments)
                .ToListAsync();
    }

    public async Task<IQueryable<UserRequest>> GetRequestsByDepartmentAsync(
        string operatorId,
        ODataQueryOptions<UserRequest> queryOptions,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(operatorId))
            throw new ArgumentNullException(nameof(operatorId));

        var department = await _context.Operators
            .Where(op => op.Id == operatorId)
            .Select(op => op.AssignedDepartment)
            .SingleOrDefaultAsync(cancellationToken);

        if (department == null)
            return Enumerable.Empty<UserRequest>().AsQueryable();

        var query = _context.UserRequests.Where(r => r.Topic == department);

        return queryOptions != null ? (IQueryable<UserRequest>)queryOptions.ApplyTo(query) : query;
    }

    public async Task TakeRequest(string operatorId, Guid requestId)
    {
        if (string.IsNullOrEmpty(operatorId))
            throw new ArgumentNullException(nameof(operatorId));

        var hasActiveRequest = await _context.UserRequests
            .AnyAsync(x => x.OperatorId == operatorId && x.Status == RequestStatus.InProgress);

        if (hasActiveRequest)
            throw new InvalidOperationException("Вы уже взяли в работу другой запрос.");

        var request = await _context.UserRequests
            .FirstOrDefaultAsync(x => x.Id == requestId);

        if (request == null)
            throw new InvalidOperationException("Неверный id запроса.");

        if (request.OperatorId != null)
            throw new InvalidOperationException("Заявка уже назначена другому оператору.");

        request.OperatorId = operatorId;
        request.Status = RequestStatus.InProgress;
        
        await _context.SaveChangesAsync();
    }
}
