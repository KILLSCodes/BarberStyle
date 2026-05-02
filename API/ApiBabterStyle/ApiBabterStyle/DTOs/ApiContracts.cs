using ApiBabterStyle.Model;
using System.ComponentModel.DataAnnotations;

namespace ApiBabterStyle.DTOs;

public record RegisterRequest(
    [property: Required, StringLength(120, MinimumLength = 2)] string Name,
    [property: Required, EmailAddress, StringLength(180)] string Email,
    [property: Required, Phone, StringLength(30)] string Phone,
    [property: Required, StringLength(100, MinimumLength = 8)] string Password);

public record LoginRequest(
    [property: Required, StringLength(180)] string Email,
    [property: Required, StringLength(100, MinimumLength = 1)] string Password);

public record AuthResponse(Guid UserId, string Name, string Email, string Phone, string Role, string Token, DateTime ExpiresAt);

public record CustomerProfileResponse(Guid UserId, string Name, string Email, string Phone, string Role);

public record UpdateProfileRequest(
    [property: Required, StringLength(120, MinimumLength = 2)] string Name,
    [property: Required, Phone, StringLength(30)] string Phone,
    [property: StringLength(100, MinimumLength = 8)] string? Password);

public record ForgotPasswordRequest([property: Required, EmailAddress, StringLength(180)] string Email);

public record ResetPasswordRequest(
    [property: Required, EmailAddress, StringLength(180)] string Email,
    [property: Required, StringLength(120, MinimumLength = 20)] string Token,
    [property: Required, StringLength(100, MinimumLength = 8)] string Password);

public record BarberResponse(Guid Id, string Name, string Specialty);

public record ServiceResponse(Guid Id, string Name, string Description, decimal Price, int DurationMinutes);

public record AdminServiceResponse(Guid Id, string Name, string Description, decimal Price, int DurationMinutes, bool Active);

public record CreateServiceRequest(
    [property: Required, StringLength(100, MinimumLength = 2)] string Name,
    [property: StringLength(500)] string Description,
    [property: Range(0.01, 10000)] decimal Price,
    [property: Range(5, 480)] int DurationMinutes);

public record UpdateServiceRequest(
    [property: Required, StringLength(100, MinimumLength = 2)] string Name,
    [property: StringLength(500)] string Description,
    [property: Range(0.01, 10000)] decimal Price,
    [property: Range(5, 480)] int DurationMinutes,
    bool Active);

public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    string Category,
    decimal Price,
    int StockQuantity,
    string ImageUrl,
    bool Active);

public record CreateProductRequest(
    [property: Required, StringLength(120, MinimumLength = 2)] string Name,
    [property: StringLength(800)] string Description,
    [property: StringLength(80)] string Category,
    [property: Range(0.01, 100000)] decimal Price,
    [property: Range(0, 100000)] int StockQuantity,
    [property: Url, StringLength(800)] string? ImageUrl);

public record UpdateProductRequest(
    [property: Required, StringLength(120, MinimumLength = 2)] string Name,
    [property: StringLength(800)] string Description,
    [property: StringLength(80)] string Category,
    [property: Range(0.01, 100000)] decimal Price,
    [property: Range(0, 100000)] int StockQuantity,
    [property: Url, StringLength(800)] string? ImageUrl,
    bool Active);

public record CreateAppointmentRequest(
    Guid BarberId,
    Guid ServiceId,
    DateTime ScheduledAt,
    [property: StringLength(800)] string? Notes,
    bool CreatePayment = true);

public record RescheduleAppointmentRequest(DateTime ScheduledAt);

public record PublicAppointmentRequest(
    [property: Required, StringLength(120, MinimumLength = 2)] string CustomerName,
    [property: Required, Phone, StringLength(30)] string CustomerPhone,
    [property: EmailAddress, StringLength(180)] string? CustomerEmail,
    [property: Required, StringLength(120, MinimumLength = 2)] string BarberName,
    [property: Required, StringLength(120, MinimumLength = 2)] string ServiceName,
    DateTime ScheduledAt,
    [property: StringLength(120)] string? Unit,
    [property: StringLength(800)] string? Notes,
    bool PayOnline);

public record AppointmentResponse(
    Guid Id,
    Guid BarberId,
    string BarberName,
    Guid ServiceId,
    string ServiceName,
    decimal Price,
    DateTime ScheduledAt,
    string Status,
    string PaymentStatus,
    string? PaymentUrl,
    string? Notes);

public record PublicAppointmentResponse(
    Guid Id,
    string CustomerName,
    string BarberName,
    string ServiceName,
    decimal Price,
    DateTime ScheduledAt,
    string Status,
    string PaymentStatus,
    string PaymentMethod,
    string? PaymentUrl,
    string Message);

public record PublicAppointmentStatusResponse(
    Guid Id,
    string CustomerName,
    string BarberName,
    string ServiceName,
    DateTime ScheduledAt,
    string Status,
    string PaymentStatus,
    string? Notes);

public record AdminAppointmentResponse(
    Guid Id,
    Guid UserId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    Guid BarberId,
    string BarberName,
    Guid ServiceId,
    string ServiceName,
    decimal Price,
    DateTime ScheduledAt,
    string Status,
    string PaymentStatus,
    string? PaymentUrl,
    string? Notes,
    DateTime CreatedAt);

public record MercadoPagoPreferenceRequest(Guid AppointmentId);

public record MercadoPagoPreferenceResponse(
    Guid AppointmentId,
    string? PreferenceId,
    string CheckoutUrl,
    PaymentStatus PaymentStatus);

public record MercadoPagoReturnRequest(
    Guid AppointmentId,
    [property: StringLength(40)] string? PaymentId,
    [property: StringLength(40)] string? Status);

public record MercadoPagoReturnResponse(
    Guid AppointmentId,
    string AppointmentStatus,
    string PaymentStatus,
    string Message);
