using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementation;

public class UserRequestRepository : BaseRepository<UserRequest>, IUserRequestRepository
{
    public UserRequestRepository(ApplicationContext context) : base(context)
    {
    }

    public async Task<UserRequest?> GetRequestWithAttachmentsAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRequests
            .Include(r => r.Attachments)
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);
    }
    public async Task<IQueryable<UserRequest>> GetUserRequestsAsync(string userId, ODataQueryOptions<UserRequest> queryOptions, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        // Получаем базовый запрос
        var query = _context.UserRequests
            .Where(r => r.UserId == userId)
            .AsQueryable();

        // Применяем OData фильтры, если они переданы
        if (queryOptions != null)
        {
            query = queryOptions.ApplyTo(query) as IQueryable<UserRequest>;
        }

        return await Task.FromResult(query);
    }
}