using MediatR;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.HttpLogging;
using System.Reflection;
using FluentValidation.AspNetCore;
using PaymentGateway.Exceptions;
using FluentValidation;
using PaymentGateway.Services;
using PaymentGateway.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PaymentGateway.Infrastructure;
using System.Security.Authentication;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = $"Just put your key here",
        In = ParameterLocation.Header,
        Name = CustomAuthenticationHandler.HeaderKeyNameXKey,
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Log request/response, more config can be added like skipping headers or log only certain media type
builder.Services.AddHttpLogging(httpLogging =>
    {
        httpLogging.LoggingFields = HttpLoggingFields.All;
        httpLogging.RequestBodyLogLimit = 4096;
        httpLogging.ResponseBodyLogLimit = 4096;
    });

builder.Services.AddFluentValidation(
    options =>
    {
        options.RegisterValidatorsFromAssemblyContaining(typeof(Program));
        options.DisableDataAnnotationsValidation = true;
        options.AutomaticValidationEnabled = true;
    });

////To generalize and unify responses on errors: RFC 7807
builder.Services.AddProblemDetails(
    options =>
    {
        options.IncludeExceptionDetails = (ctx, env) => !builder.Environment.IsProduction();
        options.Map<EntityNotFoundException>(_ => new StatusCodeProblemDetails(StatusCodes.Status404NotFound));
        options.Map<ValidationException>(_ => new StatusCodeProblemDetails(StatusCodes.Status400BadRequest));
    });

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddTransient<IPaymentProcessorService, DummyPaymentProcessorService>();
builder.Services.AddTransient<IFraudDetectionService, DummyFraudDetectionService>();
builder.Services.AddTransient<IIdempotencyService, DummyIdempotencyService>();


builder.Services.AddTransient<IAuthorizationRepository, AuthorizationRepository>();
builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("PaymentGatewayDb"));

builder.Services.AddTransient<ICurrentCallerContext, CurrentcallerContext>();
builder.Services.AddMemoryCache();

builder.Services.AddAuthentication(options => options.DefaultScheme = CustomAuthenticationHandler.SchemaName)
    .AddScheme<CustomAuthenticationSchemaOptions, CustomAuthenticationHandler>(CustomAuthenticationHandler.SchemaName, options => { });

var app = builder.Build();


// Expose swagger only on development env
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
}

app.UseMiddleware<IdempotencyKeyMiddleware>();

app.UseAuthorization();
app.UseHttpLogging();
app.UseProblemDetails(); // to enable ProblemDetails

app.MapControllers();


app.Run();
