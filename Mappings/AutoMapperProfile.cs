using AutoMapper;
using ProductAPI.DTOs;
using ProductAPI.Models;

namespace ProductAPI.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
        

            CreateMap<Product, ProductResponseDto>();


            
            CreateMap<ProductCreateDto, Product>()                
                .ForMember(dest => dest.Id, opt => opt.Ignore())                
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))                
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())                
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Description) ? null : src.Description.Trim()))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Category) ? null : src.Category.Trim()))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Brand) ? null : src.Brand.Trim()));


           
            CreateMap<ProductUpdateDto, Product>()               
                .ForMember(dest => dest.Id, opt => opt.Ignore())              
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())             
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))                
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Description) ? null : src.Description.Trim()))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Category) ? null : src.Category.Trim()))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Brand) ? null : src.Brand.Trim()));



        }
    }
    
}
