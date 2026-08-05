using SampleApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Keeps generated links (e.g. the Location header on 201) lowercase.
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// RFC 7807 problem+json for error responses, including unhandled exceptions.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IProductService, InMemoryProductService>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    // Raw document at /openapi/v1.json, browsable UI at /swagger.
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "SampleApi v1"));
}
else
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
   .WithName("HealthCheck");

app.Run();
