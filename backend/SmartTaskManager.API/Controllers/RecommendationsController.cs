using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTaskManager.API.Data;
using SmartTaskManager.API.Services;
using System.Security.Claims;

namespace SmartTaskManager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/recommendations")]
    public class RecommendationsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly RecommendationService _recommendationService;

        public RecommendationsController(AppDbContext db, RecommendationService recommendationService)
        {
            _db = db;
            _recommendationService = recommendationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRecommendations()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var tasks = await _db.Tasks.Where(t => t.UserId == userId).ToListAsync();
            var recommendations = _recommendationService.GenerateRecommendations(tasks);
            return Ok(recommendations);
        }
    }
}
