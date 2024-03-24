
using AutoMapper;
using AppsServer.Protos;
using AppsServer.Models.Entities;

namespace AppsServer.Configs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MasterProduct, ProductResponse>();
            CreateMap<ProductAddRequest, MasterProduct>();
        }
    }
}
