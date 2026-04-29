using ApiBabterStyle.Data;
using ApiBabterStyle.DTOs;
using ApiBabterStyle.Model;
using ApiBabterStyle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ApiBabterStyle.Controller;

[ApiController]
[Route("api/pagamentos/mercado-pago")]
public class PaymentsController(BarberShopDbContext db, MercadoPagoService mercadoPagoService) : ControllerBase
{
    [Authorize]
    [HttpPost("preferencia")]
    public async Task<ActionResult<MercadoPagoPreferenceResponse>> CreatePreference(
        MercadoPagoPreferenceRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var appointment = await db.Appointments
            .Include(item => item.User)
            .Include(item => item.Barber)
            .Include(item => item.Service)
            .FirstOrDefaultAsync(item => item.Id == request.AppointmentId && item.UserId == userId, cancellationToken);

        if (appointment is null)
        {
            return NotFound(new { message = "Agendamento nao encontrado." });
        }

        if (appointment.Status == AppointmentStatus.Cancelled)
        {
            return BadRequest(new { message = "Nao e possivel pagar um agendamento cancelado." });
        }

        (string? PreferenceId, string CheckoutUrl) preference;
        try
        {
            preference = await mercadoPagoService.CreatePreferenceAsync(appointment, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        appointment.MercadoPagoPreferenceId = preference.PreferenceId;
        appointment.MercadoPagoInitPoint = preference.CheckoutUrl;
        appointment.PaymentStatus = PaymentStatus.WaitingMercadoPago;
        await db.SaveChangesAsync(cancellationToken);

        return Ok(new MercadoPagoPreferenceResponse(
            appointment.Id,
            appointment.MercadoPagoPreferenceId,
            appointment.MercadoPagoInitPoint,
            appointment.PaymentStatus));
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(CancellationToken cancellationToken)
    {
        var externalReference = Request.Query["external_reference"].FirstOrDefault();
        var status = Request.Query["status"].FirstOrDefault();
        var paymentId = await TryGetPaymentIdFromRequestAsync(cancellationToken);

        if (paymentId.HasValue)
        {
            var payment = await mercadoPagoService.GetPaymentStatusAsync(paymentId.Value, cancellationToken);
            externalReference = payment.AppointmentId?.ToString() ?? externalReference;
            status = payment.Status ?? status;
        }

        if (!Guid.TryParse(externalReference, out var appointmentId) || string.IsNullOrWhiteSpace(status))
        {
            return Ok();
        }

        var appointment = await db.Appointments.FirstOrDefaultAsync(item => item.Id == appointmentId, cancellationToken);
        if (appointment is null)
        {
            return Ok();
        }

        appointment.PaymentStatus = status.ToLowerInvariant() switch
        {
            "approved" => PaymentStatus.Paid,
            "cancelled" or "rejected" => PaymentStatus.Failed,
            _ => PaymentStatus.WaitingMercadoPago
        };

        await db.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    private async Task<long?> TryGetPaymentIdFromRequestAsync(CancellationToken cancellationToken)
    {
        var queryId = Request.Query["data.id"].FirstOrDefault()
            ?? Request.Query["id"].FirstOrDefault();

        if (long.TryParse(queryId, out var parsedQueryId))
        {
            return parsedQueryId;
        }

        if (Request.ContentLength is null or 0)
        {
            return null;
        }

        using var document = await JsonDocument.ParseAsync(Request.Body, cancellationToken: cancellationToken);
        if (document.RootElement.TryGetProperty("data", out var data) &&
            data.TryGetProperty("id", out var idProperty) &&
            long.TryParse(idProperty.GetString(), out var parsedBodyId))
        {
            return parsedBodyId;
        }

        return null;
    }
}
