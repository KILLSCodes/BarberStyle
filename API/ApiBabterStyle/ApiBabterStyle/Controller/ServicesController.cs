using ApiBabterStyle.Data;
using ApiBabterStyle.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiBabterStyle.Controller;

[ApiController]
[Route("api/servicos")]
public class ServicesController(BarberShopDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ServiceResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var services = await db.Services
            .Where(service => service.Active)
            .OrderBy(service => service.Price)
            .Select(service => new ServiceResponse(service.Id, service.Name, service.Description, service.Price, service.DurationMinutes))
            .ToListAsync(cancellationToken);

        return Ok(services);
    }
}
