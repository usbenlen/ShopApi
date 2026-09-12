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

        CreateMap<ProductImage, ProductImageReadDTO>();

        CreateMap<Product, ProductReadDTO>();

        CreateMap<ProductUpdateDTO, Product>()
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                {
                    if (srcMember == null) return false;

                    return true;
                }));
    }
}