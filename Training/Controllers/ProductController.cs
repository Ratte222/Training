using AutoMapper;
using BLL.DTO;
using BLL.DTO.Product;
using BLL.Filters;
using BLL.Helpers;
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
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public ProductController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _productService = productService;
        }

        [HttpPost("CreateProduct")]
        public IActionResult CreateProduct(CreateProductDTO createProductDTO)
        {
            _productService.Create(_mapper.Map<CreateProductDTO, Product>(createProductDTO));
            return Ok("Product created successfully");
        }

        [HttpGet("GetProducts")]
        [ProducesResponseType(typeof(PageResponse<ProductDTO>), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetProducts(int? pageLength = null,
            int? pageNumber = null, string fieldOrderBy = null, bool orderByDescending = false)
        {
            PageResponse<ProductDTO> pageResponse = new PageResponse<ProductDTO>(
                pageLength, pageNumber);
            ProductFilter productFilter = new ProductFilter()
            {
                FieldOrderBy = fieldOrderBy,
                OrderByDescending = orderByDescending
            };
            
            pageResponse.Items = _mapper.Map<IEnumerable<Product>, ICollection<ProductDTO>>(
                _productService.GetProducts(pageResponse, productFilter));
            return Ok(pageResponse);
        }

        [HttpGet("GetProduct")]
        [ProducesResponseType(typeof(ProductDTO), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetProduct(long productId)
        {
            Product product = _productService.Get(productId);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPut("EditProduct")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult EditProduct(ProductDTO productDTO)
        {
            _productService.Update(_mapper.Map<ProductDTO, Product>(productDTO));
            return Ok();
        }

        [HttpPut("DeleteProduct")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteProduct(long productId)
        {
            _productService.Delete(productId);
            return Ok();
        }
    }
}
