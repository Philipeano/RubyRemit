using RubyRemit.Domain.Entities;

namespace RubyRemit.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        public IGenericRepository<Payment> PaymentRepository { get; }


        public IGenericRepository<PaymentState> PaymentStateRepository { get; }


        public void Commit();


        public void RejectChanges();
    }
}