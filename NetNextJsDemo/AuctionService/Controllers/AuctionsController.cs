﻿using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController(AuctionDbContext auctionDbContext, IMapper mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string? date)
        {
            var query = auctionDbContext.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

            if (!string.IsNullOrEmpty(date))
            {
                query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
            }

            return await query.ProjectTo<AuctionDto>(mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await auctionDbContext.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
                return NotFound();

            return mapper.Map<AuctionDto>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
        {
            var auction = mapper.Map<Auction>(createAuctionDto);
            //TODO: add current user as a seller

            auction.Seller = "testSeller";
            auction.Winner = "testWinner";
            auctionDbContext.Auctions.Add(auction);

            var result = await auctionDbContext.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Couldn't save changes to database");

            return CreatedAtAction(nameof(GetAuctionById),
                new { auction.Id }, mapper.Map<AuctionDto>(auction));
        }


        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var auction = await auctionDbContext.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound();
            //TODO: add current user as a seller and winner

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

            var result = await auctionDbContext.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Couldn't update database");

            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await auctionDbContext.Auctions.FindAsync(id);

            if (auction == null) return NotFound();

            //TODO: add current user as a seller and winner

            auctionDbContext.Remove(auction);

            var result = await auctionDbContext.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Couldn't update database");

            return Ok();
        }
    }
}
