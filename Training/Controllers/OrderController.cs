using AutoMapper;
using AutoMapper.QueryableExtensions;
using BLL.DTO.OrderLine;
using BLL.Helpers;
using BLL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Training.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderLineService _orderLineService;
        private readonly IMapper _mapper;
        public OrderController(IOrderLineService orderLineService, IMapper mapper)
        {
            _orderLineService = orderLineService;
            _mapper = mapper;
        }

        [HttpGet("GetOrderLines")]
        [ProducesResponseType(typeof(PageResponse<OrderLineDTO>), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetProducts(int? pageLength = null,
            int? pageNumber = null)
        {
            PageResponse<OrderLineDTO> pageResponse = new PageResponse<OrderLineDTO>(
                pageLength, pageNumber);
            

            //var configuration = new MapperConfiguration(cfg =>
            //    cfg.CreateMap<OrderLine, OrderLineDTO>()
            //    .ForMember(dto => dto.Product, conf => conf.MapFrom(ol => ol.Product.Name)));
            //pageResponse.Items = _orderLineService.GetAll().Include(i=>i.Product)
            //    .ProjectTo<OrderLineDTO>(configuration).ToList();

            pageResponse.Items = _mapper.ProjectTo<OrderLineDTO>(
                _orderLineService.GetAll().Include(i => i.Product),
                new { dateTimeRequest = DateTime.Now }).ToList();

            return Ok(pageResponse);
        }

        
    }
}
