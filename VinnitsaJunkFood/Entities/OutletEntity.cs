using System.Collections.Generic;

namespace JunkBackEnd.Entities{    
    public class OutletEntity : BaseEntity{
        public double Latitude { get; set; }

        public double Longitude { get; set; }        

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public double OutletRating { get; set; }

        public int Votes { get; set; }

        /// <summary>
        /// Gets or Sets a dictionary. K- AssortmentID, V - Price
        /// </summary>
        public List<ItemPriceWrapper> AssortmentPriceList { get; set; }        
    }
}