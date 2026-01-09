using AutoMapper;
using CA1.Application.DTOs;
using CA1.Domain.Entities;

namespace CA1.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
    }
}
