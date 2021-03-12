using AutoMapper;
using RubyRemit.Domain.DTOs;
using RubyRemit.Domain.Entities;

namespace RubyRemit.Infrastructure
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Payment, PaymentDTO>();

            CreateMap<PaymentState, PaymentStateDTO>();
        }
    }
}
