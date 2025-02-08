using Microsoft.AspNetCore.Mvc;
using SemanticKernelTripPlanner.Application;
using SemanticKernelTripPlanner.Application.Configuration;
using SemanticKernelTripPlanner.Application.DTO;
using SemanticKernelTripPlanner.Application.Services;
#pragma warning disable SKEXP0010

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


builder.Services.Configure<AzureOpenAIConfiguration>(builder.Configuration.GetSection(AzureOpenAIConfiguration.SectionName));
builder.Services.Configure<AzureSearchConfiguration>(builder.Configuration.GetSection(AzureSearchConfiguration.SectionName));

builder.Services.AddScoped<ITripPlanner, TripPlanner>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();

