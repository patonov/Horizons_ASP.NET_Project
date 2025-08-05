using Horizons.Common;
using Horizons.Data;
using Horizons.Data.Models;
using Horizons.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
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

        public async Task<IActionResult> Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IEnumerable<IndexDestinationViewModel> model = await _applicationDbContext.Destinations
                .Where(d => d.IsDeleted == false)
                .Include(d => d.Terrain)
                .Include(d => d.UsersDestinations)
                .Select(d => new IndexDestinationViewModel
                { 
                    Id = d.Id,
                    Name = d.Name,
                    ImageUrl = d.ImageUrl,
                    Terrain = d.Terrain.Name,
                    FavoritesCount = d.UsersDestinations.Count,
                    IsPublisher = userId != null && d.PublisherId == userId,
                    IsFavorite = userId != null && d.UsersDestinations.Any(ud => ud.DestinationId == d.Id && ud.UserId == userId)
                })
                .ToListAsync();

            return View(model);
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

        [HttpGet]
        public async Task<IActionResult> Favorites()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            IEnumerable<FavoriteDestinationViewModel> model = await _applicationDbContext
                .UsersDestinations
                .Where(ud => ud.UserId == userId)
                .Include(ud => ud.Destination)
                .Select(ud => new FavoriteDestinationViewModel 
                { 
                    Id = ud.DestinationId,
                    Name = ud.Destination.Name,
                    Terrain = ud.Destination.Terrain.Name,
                    ImageUrl = ud.Destination.ImageUrl,
                })
                .ToListAsync();
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (await _applicationDbContext.UsersDestinations
                .AnyAsync(ud => ud.UserId == userId && ud.DestinationId == id))
            {
                return RedirectToAction("Index");
            }

            var userDestination = new UserDestination()
            {
                UserId = userId,
                DestinationId = id
            };

            await _applicationDbContext.UsersDestinations.AddAsync(userDestination);
            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveFromFavorites(int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            UserDestination? userDestination = await _applicationDbContext.UsersDestinations
                .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DestinationId == id);
            if (userDestination == null) 
            {
                return RedirectToAction("Favorites", "Destination");
            }

            _applicationDbContext.UsersDestinations.Remove(userDestination);
            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Favorites", "Destination");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id) 
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Destination? destination = await _applicationDbContext.Destinations
                .Include(d => d.Terrain)
                .Include(d => d.Publisher)
                .FirstOrDefaultAsync(d => d.Id == id);
            
            if (destination == null)
            {
                throw new InvalidOperationException("Not found");
            }

            DetailsDestinationViewModel model = new DetailsDestinationViewModel()
            {
                Id = id,
                Name = destination.Name,
                Description = destination.Description,
                ImageUrl = destination.ImageUrl,
                TerrainId = destination.TerrainId,
                Terrain = destination.Terrain.Name,
                PublishedOn = destination.PublishedOn,
                PublisherId = destination.PublisherId,
                Publisher = destination.Publisher.UserName,
                IsPublisher = destination.PublisherId == userId,
                IsFavorite = userId != null && await _applicationDbContext.UsersDestinations
                    .AnyAsync(ud => ud.UserId == userId && ud.DestinationId == id),
            };

            if (model == null)
            {
                if (User?.Identity?.IsAuthenticated == false)
                {
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            TerrainViewModel[] terrainViewModels = await _applicationDbContext.Terrains
                .Select(t => new TerrainViewModel 
                { 
                    Id = t.Id,
                    Name = t.Name,
                }).ToArrayAsync();

            EditDestinationViewModel? model = await _applicationDbContext.Destinations.Where(d => d.Id == id)
                .Select(d => new EditDestinationViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    PublishedOn = d.PublishedOn.ToString("yyyy-MM-dd"),
                    ImageUrl = d.ImageUrl,
                    TerrainId = d.TerrainId,
                    PublisherId = d.PublisherId,
                    Terrains = terrainViewModels
                }).FirstOrDefaultAsync();

            if (model == null) 
            {
                return RedirectToAction("Index");
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (model.PublisherId != userId)
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditDestinationViewModel model)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (model.PublisherId != userId)
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid) 
            { 
                return View(model);
            }

            if (!DateTime.TryParseExact(model.PublishedOn, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, 
                out var publishedDate))
            {
                throw new InvalidOperationException("Invalid date format");
            }
            Destination? destination = await _applicationDbContext.Destinations
                .FirstOrDefaultAsync(d => d.Id == model.Id);

            if (destination == null)
            {
                return View(model);
            }

            destination.Name = model.Name;
            destination.Description = model.Description;
            destination.ImageUrl = model.ImageUrl;
            destination.PublishedOn = publishedDate;
            destination.TerrainId = model.TerrainId;

            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("Details", new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            DeleteDestinationViewModel? model = await _applicationDbContext.Destinations
                .Where(d => d.Id == id && d.PublisherId == userId)
                .Select(d => new DeleteDestinationViewModel 
                { 
                    Id = d.Id,
                    Name = d.Name,
                    PublisherId = d.PublisherId,
                    Publisher = d.Publisher.UserName!,                
                })
                .FirstOrDefaultAsync();

            if (model == null) 
            {
                return RedirectToAction("Index");
            }
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteDestinationViewModel model)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (model.PublisherId != userId) 
            {
                return RedirectToAction("Index");
            }
            
            Destination? destination = await _applicationDbContext.Destinations
                .FirstOrDefaultAsync(d => d.Id == model.Id);

            if (destination == null) 
            {
                return RedirectToAction("Index");
            }

            destination.IsDeleted = true;
            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
