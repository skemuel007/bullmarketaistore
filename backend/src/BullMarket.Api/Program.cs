using BullMarket.Api.Configuration;
using BullMarket.Api.Endpoints;
using BullMarket.Api.ExceptionHandling;
using BullMarket.Application;
using BullMarket.Application.Configuration;
using BullMarket.Infrastructure;
using BullMarket.Infrastructure.Persistence.Seeding;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

var commerceOptions = builder.Configuration.GetSection(CommerceOptions.SectionName).Get<CommerceOptions>() ?? new();
builder.Services.AddApplication(commerceOptions);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<FrontendOptions>(builder.Configuration.GetSection(FrontendOptions.SectionName));
var frontendOptions = builder.Configuration.GetSection(FrontendOptions.SectionName).Get<FrontendOptions>() ?? new();
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
{
    if (frontendOptions.AllowedOrigins.Length > 0)
    {
        policy.WithOrigins(frontendOptions.AllowedOrigins).AllowAnyHeader().AllowAnyMethod();
    }
}));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
{
    context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
});

builder.Services.AddOpenApi(options => options.AddDocumentTransformer((document, _, _) =>
{
    document.Info = new OpenApiInfo
    {
        Title = "BullMarket AI Store API",
        Version = "v1",
        Description = "Conventional e-commerce API for developers, creators, and AI builders."
    };
    return Task.CompletedTask;
}));

builder.Services.AddHealthChecks()
    .AddDbContextCheck<BulkMarketAiStoreDbContext>("postgresql", tags: ["ready"]);

var app = builder.Build();

app.UseExceptionHandler();
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "Handled {RequestMethod} {RequestPath} with {StatusCode} in {Elapsed:0.0000} ms";
});
app.UseHttpsRedirection();
app.UseCors("Frontend");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("BullMarket AI Store API"));
}

app.MapCartEndpoints();
app.MapCategoryEndpoints();
app.MapCustomerEndpoints();
app.MapInventoryEndpoints();
app.MapOrderEndpoints();
app.MapProductEndpoints();

app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });
app.MapHealthChecks("/health");

if (builder.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BulkMarketAiStoreDbContext>();
    await dbContext.Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<DevelopmentDataSeeder>().SeedAsync();
}

await app.RunAsync();

public partial class Program;
