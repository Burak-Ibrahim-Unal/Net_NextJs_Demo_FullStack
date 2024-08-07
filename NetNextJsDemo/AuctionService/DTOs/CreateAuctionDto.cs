﻿namespace AuctionService.DTOs
{
    public class CreateAuctionDto
    {
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int? Year { get; set; }
        public string Color { get; set; } = null!;
        public int? Mileage { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int ReservePrice { get; set; }
        public DateTime AuctionEnd { get; set; }
    }
}