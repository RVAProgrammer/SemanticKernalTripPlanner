using Azure.Search.Documents.Models;
using Microsoft.AspNetCore.Mvc;
using SemanticKernelTripPlanner.Application;
using SemanticKernelTripPlanner.Application.DTO;
using SemanticKernelTripPlanner.Application.Services;

namespace SemanticKernalTripPlanner.Controllers;

[ApiController, Route("/trip")]
public class TripController
{
    private readonly ITripPlanner _tripPlanner;
    private readonly IEmbeddingService _embeddingService;

    public TripController(ITripPlanner tripPlanner, IEmbeddingService embeddingService)
    {
            _tripPlanner = tripPlanner; 
            _embeddingService = embeddingService;
    }


    [HttpPost("plan")]
    public async Task<IActionResult> PlanTrip([FromBody] TripRequest tripRequest)
    {
        var response = await _tripPlanner.GetTripPlanWithWeather(tripRequest);
        return new ContentResult {Content = response};
    }
    
    [HttpPost("plan/rag")]
    public async Task<IActionResult> PlanTripRag([FromBody] TripRequest tripRequest)
    {
        var response = await _tripPlanner.GetTripPlanWithWeatherRag(tripRequest);
        return new ContentResult {Content = response};
    }

    [HttpPost("upload-document")]
    public async Task<IActionResult> UploadDocument(IFormFile file)
    {
        var filePath = Path.Combine("uploads", file.FileName);
        
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);    
        }
        
        
        await _embeddingService.ProcessFile(filePath);

        return new OkResult();
    }
}