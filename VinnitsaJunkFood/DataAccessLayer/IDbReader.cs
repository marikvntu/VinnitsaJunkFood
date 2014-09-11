using JunkBackEnd.Entities;
using System.Collections.Generic;

namespace VinnitsaJunkFood.DataAccessLayer
{
    interface IDbReader
    {
        /// <summary>
        /// Fills all entities from database.
        /// </summary>
        /// <param name="outlets">The outlets.</param>
        /// <param name="assortment">The assortment.</param>
        /// <param name="ingridients">The ingridients.</param>
        /// <param name="locations">The locations.</param>
        /// <param name="commentsDic">The comments dic.</param>
        void FillAllEntitiesFromDb(ref List<OutletEntity> outlets,
                                          ref List<AssortmentEntity> assortment,
                                          ref List<BaseEntity> ingridients,
                                          ref List<GeoLocationEntity> locations,
                                          ref Dictionary<int, List<CommentEntity>> commentsDic);
    }
}
