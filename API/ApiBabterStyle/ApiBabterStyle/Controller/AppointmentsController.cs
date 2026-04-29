using ApiBabterStyle.Data;
using ApiBabterStyle.DTOs;
using ApiBabterStyle.Model;
using ApiBabterStyle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiBabterStyle.Controller;

[ApiController]
[Authorize]
[Route("api/agendamentos")]
public class AppointmentsController(BarberShopDbContext db, MercadoPagoService mercadoPagoService) : ControllerBase
{
    [HttpGet("meus")]
    public async Task<ActionResult<IReadOnlyList<AppointmentResponse>>> GetMine(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var appointments = await db.Appointments
            .AsNoTracking()
            .Include(appointment => appointment.Barber)
            .Include(appointment => appointment.Service)
            .Where(appointment => appointment.UserId == userId)
            .OrderByDescending(appointment => appointment.ScheduledAt)
            .Select(appointment => ToResponse(appointment))
            .ToListAsync(cancellationToken);

        return Ok(appointments);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentResponse>> Create(CreateAppointmentRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var scheduledAt = request.ScheduledAt.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.ScheduledAt, DateTimeKind.Local).ToUniversalTime()
            : request.ScheduledAt.ToUniversalTime();

        if (scheduledAt <= DateTime.UtcNow)
        {
            return BadRequest(new { message = "Escolha uma data e horario no futuro." });
        }

        var barber = await db.Barbers.FirstOrDefaultAsync(item => item.Id == request.BarberId && item.Active, cancellationToken);
        if (barber is null)
        {
            return NotFound(new { message = "Barbeiro nao encontrado." });
        }

        var service = await db.Services.FirstOrDefaultAsync(item => item.Id == request.ServiceId && item.Active, cancellationToken);
        if (service is null)
        {
            return NotFound(new { message = "Servico nao encontrado." });
        }

        var isBusy = await db.Appointments.AnyAsync(appointment =>
            appointment.BarberId == request.BarberId &&
            appointment.ScheduledAt == scheduledAt &&
            appointment.Status != AppointmentStatus.Cancelled,
            cancellationToken);

        if (isBusy)
        {
            return Conflict(new { message = "Este barbeiro ja possui um agendamento nesse horario." });
        }

        var appointment = new Appointment
        {
            UserId = userId,
            BarberId = barber.Id,
            ServiceId = service.Id,
            ScheduledAt = scheduledAt,
            Notes = request.Notes?.Trim() ?? string.Empty,
            Barber = barber,
            Service = service,
            User = await db.Users.FindAsync([userId], cancellationToken)
        };

        db.Appointments.Add(appointment);
        await db.SaveChangesAsync(cancellationToken);

        if (request.CreatePayment)
        {
            try
            {
                var preference = await mercadoPagoService.CreatePreferenceAsync(appointment, cancellationToken);
                appointment.MercadoPagoPreferenceId = preference.PreferenceId;
                appointment.MercadoPagoInitPoint = preference.CheckoutUrl;
                appointment.PaymentStatus = PaymentStatus.WaitingMercadoPago;
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message, appointmentId = appointment.Id });
            }
        }

        return CreatedAtAction(nameof(GetMine), ToResponse(appointment));
    }

    [HttpPatch("{id:guid}/cancelar")]
    public async Task<ActionResult<AppointmentResponse>> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var appointment = await db.Appointments
            .Include(item => item.Barber)
            .Include(item => item.Service)
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId, cancellationToken);

        if (appointment is null)
        {
            return NotFound(new { message = "Agendamento nao encontrado." });
        }

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.PaymentStatus = appointment.PaymentStatus == PaymentStatus.Paid
            ? appointment.PaymentStatus
            : PaymentStatus.Cancelled;
        await db.SaveChangesAsync(cancellationToken);

        return Ok(ToResponse(appointment));
    }

    private static AppointmentResponse ToResponse(Appointment appointment)
    {
        return new AppointmentResponse(
            appointment.Id,
            appointment.BarberId,
            appointment.Barber?.Name ?? string.Empty,
            appointment.ServiceId,
            appointment.Service?.Name ?? string.Empty,
            appointment.Service?.Price ?? 0,
            appointment.ScheduledAt,
            appointment.Status.ToString(),
            appointment.PaymentStatus.ToString(),
            appointment.MercadoPagoInitPoint,
            appointment.Notes);
    }
}
