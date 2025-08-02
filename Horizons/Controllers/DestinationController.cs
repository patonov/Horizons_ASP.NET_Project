using Horizons.Common;
using Horizons.Data;
using Horizons.Data.Models;
using Horizons.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Horizons.Common.ValidationConstants.DestinationValidationConstants;

namespace Horizons.Controllers
{
    public class DestinationController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public DestinationController(ApplicationDbContext applicationDbContext)
        { 
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Add() 
        {
            var model = new AddDestinationViewModel();
            
            var terrains = await _applicationDbContext.Terrains.Select(t => new TerrainViewModel
            { 
            Id = t.Id,
            Name = t.Name,
            }).ToListAsync();

            model.Terrains = terrains;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddDestinationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel = new AddDestinationViewModel();
                var terrains = await _applicationDbContext.Terrains.Select(t => new TerrainViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                }).ToListAsync();

                viewModel.Terrains = terrains;

                return View(viewModel);
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (String.IsNullOrEmpty(userId)) 
            {
                return RedirectToAction("Login", "Account");
            }

            if (!DateTime.TryParseExact(viewModel.PublishedOn, DestinationPublishedOnFormat, null, System.Globalization.DateTimeStyles.None, out var publishedOnDate))
            {
                throw new InvalidOperationException("Invalid date format");
            }

            Destination destination = new Destination()
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                ImageUrl = viewModel.ImageUrl,
                PublishedOn = publishedOnDate,
                TerrainId = viewModel.TerrainId,
                PublisherId = userId,
            };

            await _applicationDbContext.Destinations.AddAsync(destination);
            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
