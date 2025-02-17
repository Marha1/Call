using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementation;

public class OperatorRepository : BaseRepository<Operator>, IOperatorRepository
{
    public OperatorRepository(ApplicationContext context) : base(context)
    {
    }

    public async Task <List<UserRequest>> GetRequestsByDepartment(string id)
    {
        try
        {
            var depart = await _context.Operators.FirstOrDefaultAsync(x => x.Id == id);
            return await _context.UserRequests
                .Where(x => depart != null && x.Topic == depart.AssignedDepartment)
                .Where(s=>s.Operator == null)
                .ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public async Task<IQueryable<UserRequest>> GetRequestsByDepartmentAsync(string operatorId, ODataQueryOptions<UserRequest> queryOptions, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(operatorId))
            throw new ArgumentNullException(nameof(operatorId));

        var department = await _context.Operators
            .Where(op => op.Id == operatorId)
            .Select(op => op.AssignedDepartment)
            .FirstOrDefaultAsync(cancellationToken);

        if (department == 0)
            return Enumerable.Empty<UserRequest>().AsQueryable();

        var query =  _context.UserRequests
            .Where(r => r.Topic == department)
            .AsQueryable();

        if (queryOptions != null)
        {
            query = queryOptions.ApplyTo(query) as IQueryable<UserRequest>;
        }

        return query;
    }

    public async Task TakeRequest(string operatorId, Guid requestId)
    {
        var op = await _context.Operators.FindAsync(operatorId);
        if (op is null) 
            throw new InvalidOperationException("Неверный id оператора.");

        var req = await _context.UserRequests.Include(x => x.Operator).FirstOrDefaultAsync(x => x.Id == requestId);
        if (req is null) 
            throw new InvalidOperationException("Неверный id запроса.");

        if (req.Operator != null)
            throw new InvalidOperationException("Заявка уже назначена другому оператору.");

        req.Operator = op;
        await _context.SaveChangesAsync();
    }   
}