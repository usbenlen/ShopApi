using AutoMapper;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Domain.Models;

namespace Shop.Application.Mapping;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductCreateDTO, Product>()
            .ForMember(dest => dest.Images, opt => opt.Ignore());

        CreateMap<Product, ProductReadDTO>()
            .ForMember(dest => dest.Images,
                opt => opt.MapFrom(src =>
                    src.Images.Select(x => x.Url).ToList()));

        CreateMap<ProductUpdateDTO, Product>()
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                {
                    if (srcMember == null) return false;
                    if (srcMember is int i && i == 0) return false;
                    if (srcMember is decimal d && d == 0) return false;

                    return true;
                }));
    }
}