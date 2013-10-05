using JunkBackEnd.Entities;
using System.Collections.Generic;

namespace VinnitsaJunkFood.DataAccessLayer
{
    interface IDbReader
    {
        void FillAllEntitiesFromDb(ref List<OutletEntity> outlets,
                                          ref List<AssortmentEntity> assortment,
                                          ref List<BaseEntity> ingridients,
                                          ref List<GeoLocationEntity> locations,
                                          ref Dictionary<int, List<CommentEntity>> commentsDic);
    }
}
