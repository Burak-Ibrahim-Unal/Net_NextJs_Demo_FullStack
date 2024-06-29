using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _auctionDbContext;
        private readonly IMapper _mapper;

        public AuctionsController(AuctionDbContext auctionDbContext, IMapper mapper)
        {
            _auctionDbContext = auctionDbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
        {
            var auctions = await _auctionDbContext.Auctions
                .Include(x => x.Item)
                .OrderBy(x => x.Item.Make)
                .ToListAsync();

            return _mapper.Map<List<AuctionDto>>(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await _auctionDbContext.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
                return NotFound();

            return _mapper.Map<AuctionDto>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
        {
            var auction = _mapper.Map<Auction>(createAuctionDto);
            //TODO: add curent user as a seller

            auction.Seller = "testSeller";
            auction.Winner = "testWinner";
            _auctionDbContext.Auctions.Add(auction);

            var result = await _auctionDbContext.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Couldnt save changes to database");

            return CreatedAtAction(nameof(GetAuctionById),
                new { auction.Id }, _mapper.Map<AuctionDto>(auction));
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var auction = await _auctionDbContext.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound();
            //TODO: add curent user as a seller and winner

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

            var result = await _auctionDbContext.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Couldnt update database");

            return Ok(); ;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _auctionDbContext.Auctions.FindAsync(id);

            if (auction == null) return NotFound();

            //TODO: add curent user as a seller and winner

            _auctionDbContext.Remove(auction);

            var result = await _auctionDbContext.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Couldnt update database");

            return Ok(); 
        }
    }
}
