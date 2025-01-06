using api.Extensions;
using api.interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepository;
        private readonly IPortfolioRepository _portfolioRepository;

        public PortfolioController(UserManager<AppUser> userManager, IStockRepository stockRepository, IPortfolioRepository portfolioRepository)
        {
            _userManager = userManager;
            _stockRepository = stockRepository;
            _portfolioRepository = portfolioRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);
            var userPortfolio = await _portfolioRepository.GetUserPortfolioAsync(appUser!);
            return Ok(userPortfolio.Select( stock => stock.ToStockDto()));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio([FromQuery] string symbol)
        {
            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);
            var stock = await _stockRepository.GetBySymbolAsync(symbol);

            if (stock == null) return BadRequest("Stock not found");

            var userPortfolio = await _portfolioRepository.GetUserPortfolioAsync(appUser!);

            if (userPortfolio.Any(s => s.Id == stock.Id)) return BadRequest("Stock already in portfolio");

            var portfolioModel = new Portfolio
            {
                AppUserId = appUser!.Id,
                StockId = stock.Id
            };

            await _portfolioRepository.CreateAsync(portfolioModel);

            return Created();
        }
    }
}