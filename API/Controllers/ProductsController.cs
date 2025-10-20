

using API.DTOs;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


public class ProductsController : BaseApiController
{
     private readonly IGenericRepository<Product> _productsRepo;
     private readonly IGenericRepository<ProductBrand> _productBrandsRepo;
     private readonly IGenericRepository<ProductType> _productTypesRepo;
     private readonly IMapper _mapper;

     public ProductsController(IGenericRepository<Product> productsRepo, IGenericRepository<ProductBrand> productBrandRepo,
          IGenericRepository<ProductType> productTypeRepo, IMapper mapper)
     {
          _mapper=mapper;
        _productsRepo = productsRepo;
        _productBrandsRepo = productBrandRepo;
        _productTypesRepo = productTypeRepo;
     }

     [HttpGet]
     public async Task <ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts()
     {
          var spec = new PeoductsWithTypesAndBrandsSpecification();
          var products = await _productsRepo.ListAsync(spec);
          return Ok(_mapper
               .Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));
     }
     
     [HttpGet("{id}")]
     [ProducesResponseType(StatusCodes.Status200OK)]
     [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
     public async Task <ActionResult<ProductToReturnDto>> GetProduct(int id)
     {
          var spec =  new PeoductsWithTypesAndBrandsSpecification(id);

          var product = await _productsRepo.GetEntityWithSpec(spec);

          if (product == null) return NotFound(new ApiResponse(404));

          return _mapper.Map<Product, ProductToReturnDto>(product);
     }

     [HttpGet("brands")]
     public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
     {
          return Ok(await _productBrandsRepo.ListAllAsync());
     }
     
     [HttpGet("types")]
     public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductTypes()
     {
          return Ok(await _productTypesRepo.ListAllAsync());
     }
    
}