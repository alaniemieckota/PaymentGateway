using Microsoft.Extensions.Caching.Memory;
using PaymentGateway.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Infrastructure;

public class IdempotencyKeyMiddleware
{
    public const string HeaderKeyNameIdempotencyKey = "Idempotency-Key";

    private readonly RequestDelegate next;

    private readonly IMemoryCache cache; // normally we should use some global cashing service

    public IdempotencyKeyMiddleware(IMemoryCache cache, RequestDelegate next)
    {
        this.cache = cache;
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey(HeaderKeyNameIdempotencyKey))
        {
            throw new ValidationException($"Idempotency key is missing add it to header as: {HeaderKeyNameIdempotencyKey }");
        }

        var idempotencyKeyHeader = context.Request.Headers[HeaderKeyNameIdempotencyKey];
        var cacheKey = $"{idempotencyKeyHeader[0]}_callerId";

        if (this.cache.TryGetValue(cacheKey, out var key))
        {
            throw new RequestNotUniqueException((string)key);
        }

        await this.next(context);
    }
}