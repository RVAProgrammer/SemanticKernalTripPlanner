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
    private readonly ITravelAgent _travelAgent; 

    public TripController(ITripPlanner tripPlanner, IEmbeddingService embeddingService, ITravelAgent travelAgent)
    {
            _tripPlanner = tripPlanner; 
            _embeddingService = embeddingService;
            _travelAgent = travelAgent;
    }    
    [HttpPost("plan/agent")]
    public async Task<IActionResult> PlanTripAgent([FromBody] TripRequest tripRequest)
    {
        _travelAgent.Init();
        var response = await _travelAgent.PlanTrip(tripRequest.TripDescription);
        return new ContentResult {Content = response};
    }
}