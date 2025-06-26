using Backend.Domain.Interfaces;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Reflection;

namespace Backend.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DataBaseContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(DataBaseContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<object> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Use reflection to get the ID property value
            return entity.GetType().GetProperty(entity.GetType().Name + "Id")?.GetValue(entity);
        }

        public async Task<object> AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            return new { Success = true, Message = "Entities added successfully." };
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(T entity)
        {
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Apply eager loading for all included related entities
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.Where(predicate).ToListAsync();
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsQueryable().FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync() ?? new List<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
                          Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
            {
                query = include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetByConditionAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            try
            {
                IQueryable<T> query = _dbSet;

                // Apply includes if any
                if (include != null)
                {
                    query = include(query);
                }

                // Apply the condition (filter) with an OrderBy to ensure predictability
                return await query.OrderBy(x => EF.Property<object>(x, GetPrimaryKeyName())).FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while retrieving entity: {ex.Message}", ex);
            }
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<object> UpdateAsync(T entity)
        {
            var entityType = entity.GetType();

            PropertyInfo keyProperty;
            if (entityType.Name == "EntityName")
            {
                keyProperty = entityType.GetProperty("EntityId");
            }
            else
            {
                // Use default convention: ClassName + "Id"
                keyProperty = entityType.GetProperty(entityType.Name + "Id");
            }

            // Get the value of the primary key
            var key = keyProperty?.GetValue(entity);

            if (key == null)
            {
                throw new ArgumentException("Entity key is not set.");
            }

            // Attach and update the entity
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return key;
        }

        public async Task<object> UpdateAsync(T entity, bool trackingOff)
        {
            var entityType = entity.GetType();

            PropertyInfo keyProperty;

            if (entityType.Name == "EntityName")
            {
                keyProperty = entityType.GetProperty("EntityId");
            }
            else
            {
                // Use default convention: ClassName + "Id"
                keyProperty = entityType.GetProperty(entityType.Name + "Id");
            }

            // Get the value of the primary key
            var key = keyProperty?.GetValue(entity);

            if (key == null)
            {
                throw new ArgumentException("Entity key is not set.");
            }

            // Check if the entity is already tracked
            var trackedEntity = _context.ChangeTracker.Entries<T>()
                .FirstOrDefault(e => keyProperty.GetValue(e.Entity).Equals(key));

            if (trackedEntity != null)
            {
                // Detach the existing tracked entity
                _context.Entry(trackedEntity.Entity).State = EntityState.Detached;
            }

            if (trackingOff)
            {
                // Dynamically build a lambda expression to find the entity by key
                var parameter = Expression.Parameter(typeof(T), "e");
                var propertyAccess = Expression.Property(parameter, keyProperty.Name);
                var keyConstant = Expression.Constant(key);
                var equalsExpression = Expression.Equal(propertyAccess, keyConstant);

                var lambda = Expression.Lambda<Func<T, bool>>(equalsExpression, parameter);

                // Use the lambda expression in the query
                var existingEntity = await _dbSet.AsNoTracking().FirstOrDefaultAsync(lambda);

                if (existingEntity == null)
                {
                    throw new KeyNotFoundException("Entity not found.");
                }

                // Copy updated values to the existing entity
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                _context.Entry(existingEntity).State = EntityState.Modified;
            }
            else
            {
                // Attach and update the entity with tracking
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return key;
        }

        // Private helper methods
        private string GetPrimaryKeyName()
        {
            var entityType = typeof(T);
            return entityType.Name + "Id"; // Adjust based on your primary key naming convention
        }

        private object GetPrimaryKeyValue(T entity)
        {
            var entityType = entity.GetType();
            var keyProperty = entityType.GetProperty(GetPrimaryKeyName());
            return keyProperty?.GetValue(entity);
        }

    }
}
