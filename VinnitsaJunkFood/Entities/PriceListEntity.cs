using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JunkBackEnd.Entities
{
    /// <summary>
    /// Is used for to connect price with name and assortment ID
    /// </summary>
    class PriceListEntity : BaseEntity
    {
        /// <summary>
        /// Represents a price of the given assortment in the outlet
        /// </summary>
        public float Price {get; set;}
    }
}
