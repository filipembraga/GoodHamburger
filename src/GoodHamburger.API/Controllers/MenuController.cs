namespace GoodHamburger.API.Controllers;

using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

/// <summary>Controller for managing menu items in the Good Hamburger API.</summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class MenuController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    /// <summary>Initializes a new instance of the MenuController class with the specified product repository.</summary>
    /// <param name="productRepository"></param>
    public MenuController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <summary>Returns the full restaurant menu.</summary>
    /// <remarks>
    /// Products are divided into three categories: Sandwich, Side, and Drink.
    /// Use the product IDs returned here when creating or updating an order.
    /// </remarks>
    /// <response code="200">Menu returned successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMenu()
    {
        var products = await _productRepository.GetAllAsync();
        return Ok(products);
    }
}