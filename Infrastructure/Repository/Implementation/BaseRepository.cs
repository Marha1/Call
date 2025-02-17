using Domain.Interfaces;
using Microsoft.AspNetCore.OData.Query;
    namespace Infrastructure.Repository.Implementation;
    /// <summary>
    /// Базовый абстрактный класс репозитория
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationContext _context;

        protected BaseRepository(ApplicationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        /// <summary>
        /// Абстрактное добавление
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken);
        }
        /// <summary>
        /// Абстрактное удаление
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }
        /// <summary>
        /// Абстрактное получение по Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }
        /// <summary>
        /// Абстрактное получение сущность по параметрам
        /// </summary>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IQueryable<T>> GetQueryableAsync(ODataQueryOptions<T> options, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Set<T>().AsQueryable();
            return options != null ? options.ApplyTo(queryable) as IQueryable<T> : queryable;
        }
        /// <summary>
        /// Абстрактное обновление
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }
        /// <summary>
        /// Абстрактное сохранение
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }