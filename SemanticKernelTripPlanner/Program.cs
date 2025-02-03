using Microsoft.AspNetCore.Mvc;
using SemanticKernelTripPlanner.Application;
using SemanticKernelTripPlanner.Application.Configuration;
using SemanticKernelTripPlanner.Application.DTO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AzureOpenAIConfiguration>(builder.Configuration.GetSection(AzureOpenAIConfiguration.SectionName));
builder.Services.AddScoped<ITripPlanner, TripPlanner>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/plan", async (ITripPlanner tripPlanner, [FromBody] TripRequest tripRequest) =>
{
    var response = await tripPlanner.GetTripPlan(tripRequest);
    return Results.Ok(response);
    
});

app.Run();

