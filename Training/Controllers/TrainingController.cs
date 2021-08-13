using AutoMapper;
using BLL.DTO.Product;
using BLL.Extensions;
using BLL.Filters;
using BLL.Helpers;
using BLL.Interfaces;
using DAL.Model;
using FluentEmail.Core;
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
        private readonly IFluentEmail _singleEmail;

        public TrainingController(IMapper mapper, IProductService productService, IFluentEmail singleEmail)
        {
            _mapper = mapper;
            _productService = productService;
            _singleEmail = singleEmail;
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

        [HttpGet("InsertNewProductToFireBaseAsync")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> InsertNewProductToFireBaseAsync()
        {
            List<Product> products = _productService.GetAll().ToList();
            await _productService.InsertNewProductToFireBaseAsync(_mapper.Map<List<Product>, 
                List<ProductFbDTO>>(products));
            return Ok();
        }

        [HttpGet("GetDataFromFireBase")]
        [ProducesResponseType(typeof(PageResponse<ProductFbDTO>), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetDataFromFireBase(int? pageLength = null,
            int? pageNumber = null, string fieldOrderBy = null, bool orderByDescending = false)
        {
            PageResponse<ProductFbDTO> pageResponse = new PageResponse<ProductFbDTO>(
                pageLength, pageNumber);
            ProductFilter productFilter = new ProductFilter()
            {
                FieldOrderBy = fieldOrderBy,
                OrderByDescending = orderByDescending
            };
            pageResponse.Items = await _productService.GetDataFromFireBase(pageResponse, productFilter);
            return Ok(pageResponse);
        }

        [HttpGet("SendEmail")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> SendEmail()
        {
            await _singleEmail
                .To(@"email")
                .Subject("Email example")
                .Body("Email body")
                .SendAsync(); //this will use the SmtpSender
            return Ok();
        }
    }
}
