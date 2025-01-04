using api.Data;
using api.Dtos.Stock;
using api.interfaces;
using api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _context;
        public StockRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateAsync(Stock StockModel)
        {
            await _context.Stock.AddAsync(StockModel);
            await _context.SaveChangesAsync();
            return StockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var StockModel = await _context.Stock.FirstOrDefaultAsync(x => x.Id == id);
            if (StockModel == null) return null;
            _context.Stock.Remove(StockModel);
            await _context.SaveChangesAsync();
            return StockModel;
        }

        public async Task<List<Stock>> GetAllAsync()
        {
            return await _context.Stock.ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            var stockModel = await _context.Stock.FindAsync(id);
            return stockModel;
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto)
        {
            var stockModel = await _context.Stock.FindAsync(id);
            if (stockModel == null) return null;
            stockModel.Symbol = stockDto.Symbol;
            stockModel.CompanyName = stockDto.CompanyName;
            stockModel.Purchase = stockDto.Purchase;
            stockModel.LastDiv = stockDto.LastDiv;
            stockModel.Industry = stockDto.Industry;
            stockModel.MarketCap = stockDto.MarketCap;
            await _context.SaveChangesAsync();
            return stockModel;
        }
    }
}