using AutoMapper;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Mapping;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<CategoryCreateDTO, Category>(); //1. з чого перетворити: CategoryCreateDTO -> 2. на що перетворити: Category
        
        CreateMap<Category, CategoryReadDTO>().ForMember(dest => dest.Products,
            opt => opt.MapFrom(src => src.Products.Select(p => p.Id).ToList()));

        CreateMap<CategoryUpdateDTO, Category>()
            .ForMember(dest => dest.ParentId, opt => opt.Condition(src => src.ParentId.HasValue && src.ParentId.Value > 0))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
            {
                if (srcMember == null) return false;
                if (srcMember is int i && i == 0) return false;

                return true;
            }));

        CreateMap<Category, CategoryTreeDTO>();
    }
}
