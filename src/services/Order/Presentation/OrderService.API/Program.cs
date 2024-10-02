using OrderService.Persistence;
using OrderService.Application;
using OrderService.Application.Features.Commands.CreateOrder;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/orders", async (CreateOrderCommandRequest request, IMediator mediator) =>
{
    var result = await mediator.Send(request);
    return Results.Ok(result);
})
.WithName("CreateOrder")
.Produces<CreateOrderCommandResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithOpenApi();

app.Run();