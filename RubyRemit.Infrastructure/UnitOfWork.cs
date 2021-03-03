using Microsoft.EntityFrameworkCore;
using RubyRemit.Domain.Entities;
using RubyRemit.Domain.Interfaces;
using System.Linq;

namespace RubyRemit.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RubyRemitContext _dbContext;

        public IGenericRepository<Payment> PaymentRepository => new GenericRepository<Payment>(_dbContext);

        public IGenericRepository<PaymentState> PaymentStateRepository => new GenericRepository<PaymentState>(_dbContext);



        public UnitOfWork(RubyRemitContext dbContext)
        {
            _dbContext = dbContext;
        }


        public void Commit()
        {
            _dbContext.SaveChanges();
        }


        public void RejectChanges()
        {
            if (_dbContext.ChangeTracker.HasChanges())
            {
                var modifiedEntities = _dbContext.ChangeTracker.Entries()
                    .Where(e => e.State != EntityState.Unchanged);

                foreach(var entity in modifiedEntities)
                {
                    if (entity.State == EntityState.Added)
                        entity.State = EntityState.Detached;
                    else
                        entity.Reload();
                }
            }
        }
    }
}
