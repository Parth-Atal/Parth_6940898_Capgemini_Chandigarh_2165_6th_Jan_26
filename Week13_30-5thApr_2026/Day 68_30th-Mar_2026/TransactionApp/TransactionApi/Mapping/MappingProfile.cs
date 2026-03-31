using AutoMapper;
using TransactionApi.DTO;
using TransactionApi.Model;

namespace TransactionApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Transaction, TransactionDTO>();
        }
    }
}
