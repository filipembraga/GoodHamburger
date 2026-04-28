namespace GoodHamburger.API.Controllers;

using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

/// <summary>Controller for managing orders in the Good Hamburger API.</summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    /// <summary> Initializes a new instance of the OrderController class with the specified order service.</summary>
    /// <param name="orderService"></param>
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>Returns all orders.</summary>
    /// <response code="200">List of orders returned successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllAsync();
        return Ok(orders);
    }

    /// <summary>Returns a single order by ID.</summary>
    /// <param name="id">The order's unique identifier (Guid)</param>
    /// <response code="200">Order found and returned</response>
    /// <response code="404">Order not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var order = await _orderService.GetByIdAsync(id);
            return Ok(order);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>Creates a new order.</summary>
    /// <remarks>
    /// Each order accepts one item per category (Sandwich, Side, Drink).
    /// Duplicate categories return 400.
    ///
    /// Available product IDs:
    /// - 1: X Burger (Sandwich) — $5.00
    /// - 2: X Egg (Sandwich) — $4.50
    /// - 3: X Bacon (Sandwich) — $7.00
    /// - 4: French Fries (Side) — $2.00
    /// - 5: Soft Drink (Drink) — $2.50
    ///
    /// Example request:
    ///
    ///     POST /orders
    ///     {
    ///         "productIds": [1, 4, 5]
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Order created successfully</response>
    /// <response code="400">Duplicate item or invalid product</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] OrderRequestDto dto)
    {
        try
        {
            var order = await _orderService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Updates an existing order.</summary>
    /// <remarks>
    /// Replaces all items in the order with the new product selection.
    /// The same discount rules and duplicate validations apply.
    ///
    /// Available product IDs:
    /// - 1: X Burger (Sandwich) — $5.00
    /// - 2: X Egg (Sandwich) — $4.50
    /// - 3: X Bacon (Sandwich) — $7.00
    /// - 4: French Fries (Side) — $2.00
    /// - 5: Soft Drink (Drink) — $2.50
    /// 
    /// Example request:
    ///
    ///     PUT /orders/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///     {
    ///         "productIds": [2, 5]
    ///     }
    ///
    /// </remarks>
    /// <param name="id">The order's unique identifier (Guid)</param>
    /// <param name="dto">The new product selection for the order</param>
    /// <response code="200">Order updated successfully</response>
    /// <response code="400">Duplicate item or invalid product ID</response>
    /// <response code="404">Order not found</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] OrderRequestDto dto)
    {
        try
        {
            var order = await _orderService.UpdateAsync(id, dto);
            return Ok(order);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Removes an order.</summary>
    /// <param name="id">The order's unique identifier (Guid)</param>
    /// <response code="204">Order deleted successfully</response>
    /// <response code="404">Order not found</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _orderService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}