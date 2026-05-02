using ApiBabterStyle.Model;
using System.ComponentModel.DataAnnotations;

namespace ApiBabterStyle.DTOs;

public record RegisterRequest(
    [param: Required, StringLength(120, MinimumLength = 2)] string Name,
    [param: Required, EmailAddress, StringLength(180)] string Email,
    [param: Required, Phone, StringLength(30)] string Phone,
    [param: Required, StringLength(100, MinimumLength = 8)] string Password);

public record LoginRequest(
    [param: Required, StringLength(180)] string Email,
    [param: Required, StringLength(100, MinimumLength = 1)] string Password);

public record AuthResponse(Guid UserId, string Name, string Email, string Phone, string Role, string Token, DateTime ExpiresAt);

public record CustomerProfileResponse(Guid UserId, string Name, string Email, string Phone, string Role);

public record UpdateProfileRequest(
    [param: Required, StringLength(120, MinimumLength = 2)] string Name,
    [param: Required, Phone, StringLength(30)] string Phone,
    [param: StringLength(100, MinimumLength = 8)] string? Password);

public record ForgotPasswordRequest([param: Required, EmailAddress, StringLength(180)] string Email);

public record ResetPasswordRequest(
    [param: Required, EmailAddress, StringLength(180)] string Email,
    [param: Required, StringLength(120, MinimumLength = 20)] string Token,
    [param: Required, StringLength(100, MinimumLength = 8)] string Password);

public record BarberResponse(Guid Id, string Name, string Specialty);

public record ServiceResponse(Guid Id, string Name, string Description, decimal Price, int DurationMinutes);

public record AdminServiceResponse(Guid Id, string Name, string Description, decimal Price, int DurationMinutes, bool Active);

public record CreateServiceRequest(
    [param: Required, StringLength(100, MinimumLength = 2)] string Name,
    [param: StringLength(500)] string Description,
    [param: Range(0.01, 10000)] decimal Price,
    [param: Range(5, 480)] int DurationMinutes);

public record UpdateServiceRequest(
    [param: Required, StringLength(100, MinimumLength = 2)] string Name,
    [param: StringLength(500)] string Description,
    [param: Range(0.01, 10000)] decimal Price,
    [param: Range(5, 480)] int DurationMinutes,
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
    [param: Required, StringLength(120, MinimumLength = 2)] string Name,
    [param: StringLength(800)] string Description,
    [param: StringLength(80)] string Category,
    [param: Range(0.01, 100000)] decimal Price,
    [param: Range(0, 100000)] int StockQuantity,
    [param: Url, StringLength(800)] string? ImageUrl);

public record UpdateProductRequest(
    [param: Required, StringLength(120, MinimumLength = 2)] string Name,
    [param: StringLength(800)] string Description,
    [param: StringLength(80)] string Category,
    [param: Range(0.01, 100000)] decimal Price,
    [param: Range(0, 100000)] int StockQuantity,
    [param: Url, StringLength(800)] string? ImageUrl,
    bool Active);

public record CreateAppointmentRequest(
    Guid BarberId,
    Guid ServiceId,
    DateTime ScheduledAt,
    [param: StringLength(800)] string? Notes,
    bool CreatePayment = true);

public record RescheduleAppointmentRequest(DateTime ScheduledAt);

public record PublicAppointmentRequest(
    [param: Required, StringLength(120, MinimumLength = 2)] string CustomerName,
    [param: Required, Phone, StringLength(30)] string CustomerPhone,
    [param: EmailAddress, StringLength(180)] string? CustomerEmail,
    [param: Required, StringLength(120, MinimumLength = 2)] string BarberName,
    [param: Required, StringLength(120, MinimumLength = 2)] string ServiceName,
    DateTime ScheduledAt,
    [param: StringLength(120)] string? Unit,
    [param: StringLength(800)] string? Notes,
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
    [param: StringLength(40)] string? PaymentId,
    [param: StringLength(40)] string? Status);

public record MercadoPagoReturnResponse(
    Guid AppointmentId,
    string AppointmentStatus,
    string PaymentStatus,
    string Message);
