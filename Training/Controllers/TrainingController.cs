using AutoMapper;
using BLL.DTO.Product;
using BLL.Extensions;
using BLL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Training.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public TrainingController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _productService = productService;
        }

        [HttpGet("Serialize")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult Serialize()
        {
            Product product = new Product();
            product = _productService.GetAll().OrderBy(nameof(product.Name), false).LastOrDefault();
            if (product == null)
                return NotFound("Product does not exist");
            _productService.Serialize(_mapper.Map<Product, ProductDTO>(product));
            return Ok();
        }

        [HttpGet("Deserialize")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult Deserialize()
        {            
            return Ok(_productService.Deserialize());
        }
    }
}
