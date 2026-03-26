using AutoMapper;
using BankingApi.Models;
using BankingApi.DTOs;

namespace BankingApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // USER VIEW (MASKED)
                CreateMap<Account, UserAccountDTO>()
        .ForMember(dest => dest.MaskedAccountNumber,
            opt => opt.MapFrom(src =>
                src.AccountNumber.Length <= 4
                ? src.AccountNumber
                : new string('X', src.AccountNumber.Length - 4) 
                + src.AccountNumber.Substring(src.AccountNumber.Length - 4)
            ));

            // ADMIN VIEW (FULL)
            CreateMap<Account, AdminAccountDTO>();
        }
    }
}