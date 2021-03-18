using AutoMapper;
using RubyRemit.Domain.DTOs;
using RubyRemit.Domain.Entities;
using RubyRemit.Domain.LookUp;
using RubyRemit.Infrastructure.AutoMapperSettings.Converters;

namespace RubyRemit.Infrastructure.AutoMapperSettings.Profiles
{
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<Payment, PaymentDTO>();

            CreateMap<PaymentState, PaymentStateDTO>();

            CreateMap<PaymentStateEnum, string>().ConvertUsing(s => new PaymentStateEnumConverter().Convert(s));
        }
    }
}
