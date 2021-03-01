using RubyRemit.Domain.Entities;
using System;

namespace RubyRemit.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IGenericRepository<Payment> PaymentRepository { get; }

        public IGenericRepository<PaymentState> PaymentStateRepository { get; }


        public void CommitChanges();


        public void RejectChanges();


        public new void Dispose();
    }
}