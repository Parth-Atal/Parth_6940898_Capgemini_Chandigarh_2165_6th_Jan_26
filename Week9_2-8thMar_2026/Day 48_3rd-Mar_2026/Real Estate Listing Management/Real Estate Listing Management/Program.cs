using System;
using System.Collections.Generic;
using System.Linq;

namespace Real_Estate_Listing_Management
{
    
    public class RealEstateListing
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }

        public RealEstateListing(int id, string description, decimal price, string location)
        {
            Id = id;
            Description = description;
            Price = price;
            Location = location;
        }
    }

   
    public class RealEstateApp
    {
        private List<RealEstateListing> listings = new List<RealEstateListing>();

        
        public void AddListing(RealEstateListing listing)
        {
            listings.Add(listing);
        }

        
        public void RemoveListing(int listingId)
        {
            listings.RemoveAll(l => l.Id == listingId);
        }

        
        public void UpdateListing(RealEstateListing updatedListing)
        {
            var listing = listings.FirstOrDefault(l => l.Id == updatedListing.Id);

            if (listing != null)
            {
                listing.Description = updatedListing.Description;
                listing.Price = updatedListing.Price;
                listing.Location = updatedListing.Location;
            }
        }

       
        public List<RealEstateListing> GetListings()
        {
            return listings;
        }

        
        public List<RealEstateListing> GetListingsByLocation(string location)
        {
            return listings
                .Where(l => l.Location.Equals(location, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<RealEstateListing> GetListingsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return listings
                .Where(l => l.Price >= minPrice && l.Price <= maxPrice)
                .ToList();
        }
    }
}