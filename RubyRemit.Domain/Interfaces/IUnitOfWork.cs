using RubyRemit.Domain.Entities;
using System;

namespace RubyRemit.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IRepository<Payment> PaymentRepository { get; }

        public IRepository<PaymentState> PaymentStateRepository { get; }


        public void CommitChanges();


        public void RejectChanges();


        public new void Dispose();
    }
}