using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Relationships.Entities;

namespace Relationships.Data {
    public class ValueObjectInterceptor : ISaveChangesInterceptor {
        //private readonly string _connectionString;
        //private SaveChangesAudit _audit;

        public ValueObjectInterceptor() {
            //_connectionString = connectionString;
        }

        //public async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        //    DbContextEventData eventData,
        //    InterceptionResult<int> result,
        //    CancellationToken cancellationToken = default) {
        //    _audit = CreateAudit(eventData.Context);

        //    using var auditContext = new AuditContext(_connectionString);

        //    auditContext.Add(_audit);
        //    await auditContext.SaveChangesAsync();

        //    return result;
        //}

        public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result) {
            //_audit = CreateAudit(eventData.Context);

            //using var auditContext = new AuditContext(_connectionString);
            //auditContext.Add(_audit);
            //auditContext.SaveChanges();

            var modified = eventData.Context.ChangeTracker.Entries().Where(x => x.Entity is IValueObject && (x.State == EntityState.Modified || x.State == EntityState.Added));

            foreach (var entry in modified) {
                if (entry.Entity is Address) {
                    var key = ((IValueObject)entry.Entity).UniqueKey;
                    var dbset = eventData.Context.Set<Address>();
                    var entity = dbset.AsNoTracking().FirstOrDefault(x => x.UniqueKey == key);
                    if (entity != null) {
                        entry.State = EntityState.Detached;
                        entry.CurrentValues.SetValues(entity);
                        ((Address)entry.Entity).AddressId = entity.AddressId;
                        //entry.State = EntityState.Unchanged;
                        Console.Out.WriteLine(((Address)entry.Entity).AddressId);
                        entry.Reload();
                    }
                }
            }

            return result;
        }

        public int SavedChanges(SaveChangesCompletedEventData eventData, int result) {
            //using var auditContext = new AuditContext(_connectionString);

            //auditContext.Attach(_audit);
            //_audit.Succeeded = true;
            //_audit.EndTime = DateTime.UtcNow;

            //auditContext.SaveChanges();

            return result;
        }

        //public async ValueTask<int> SavedChangesAsync(
        //    SaveChangesCompletedEventData eventData,
        //    int result,
        //    CancellationToken cancellationToken = default) {
        //    using var auditContext = new AuditContext(_connectionString);

        //    auditContext.Attach(_audit);
        //    _audit.Succeeded = true;
        //    _audit.EndTime = DateTime.UtcNow;

        //    await auditContext.SaveChangesAsync(cancellationToken);

        //    return result;
        //}

        #region SaveChangesFailed
        public void SaveChangesFailed(DbContextErrorEventData eventData) {
            //using var auditContext = new AuditContext(_connectionString);

            //auditContext.Attach(_audit);
            //_audit.Succeeded = false;
            //_audit.EndTime = DateTime.UtcNow;
            //_audit.ErrorMessage = eventData.Exception.Message;

            //auditContext.SaveChanges();
        }

        //public async Task SaveChangesFailedAsync(
        //    DbContextErrorEventData eventData,
        //    CancellationToken cancellationToken = default) {
        //    using var auditContext = new AuditContext(_connectionString);

        //    auditContext.Attach(_audit);
        //    _audit.Succeeded = false;
        //    _audit.EndTime = DateTime.UtcNow;
        //    _audit.ErrorMessage = eventData.Exception.InnerException?.Message;

        //    await auditContext.SaveChangesAsync(cancellationToken);
        //}
        #endregion

        #region CreateAudit
        //private static SaveChangesAudit CreateAudit(DbContext context) {
        //    context.ChangeTracker.DetectChanges();

        //    var audit = new SaveChangesAudit { AuditId = Guid.NewGuid(), StartTime = DateTime.UtcNow };

        //    foreach (var entry in context.ChangeTracker.Entries()) {
        //        var auditMessage = entry.State switch {
        //            EntityState.Deleted => CreateDeletedMessage(entry),
        //            EntityState.Modified => CreateModifiedMessage(entry),
        //            EntityState.Added => CreateAddedMessage(entry),
        //            _ => null
        //        };

        //        if (auditMessage != null) {
        //            audit.Entities.Add(new EntityAudit { State = entry.State, AuditMessage = auditMessage });
        //        }
        //    }

        //    return audit;

        //    string CreateAddedMessage(EntityEntry entry)
        //        => entry.Properties.Aggregate(
        //            $"Inserting {entry.Metadata.DisplayName()} with ",
        //            (auditString, property) => auditString + $"{property.Metadata.Name}: '{property.CurrentValue}' ");

        //    string CreateModifiedMessage(EntityEntry entry)
        //        => entry.Properties.Where(property => property.IsModified || property.Metadata.IsPrimaryKey()).Aggregate(
        //            $"Updating {entry.Metadata.DisplayName()} with ",
        //            (auditString, property) => auditString + $"{property.Metadata.Name}: '{property.CurrentValue}' ");

        //    string CreateDeletedMessage(EntityEntry entry)
        //        => entry.Properties.Where(property => property.Metadata.IsPrimaryKey()).Aggregate(
        //            $"Deleting {entry.Metadata.DisplayName()} with ",
        //            (auditString, property) => auditString + $"{property.Metadata.Name}: '{property.CurrentValue}' ");
        //}
        #endregion
    }
}
