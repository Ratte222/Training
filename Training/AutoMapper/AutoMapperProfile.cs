using AutoMapper;
using BLL.DTO;
using BLL.DTO.OrderLine;
using BLL.DTO.Product;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Training.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateProductDTO, Product>();
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductDTO, Product>();
            CreateMap<Product, ProductFbDTO>();
            DateTime dateTimeRequest = DateTime.Now.AddDays(-1);//addDays(-1) added for check
            CreateMap<OrderLine, OrderLineDTO>()
                .ForMember(dto => dto.Product, conf => conf.MapFrom(ol => ol.Product.Name))
                .ForMember(dto => dto.DateTimeRequest, conf => conf.MapFrom(ol => dateTimeRequest));
        }
    }
}
