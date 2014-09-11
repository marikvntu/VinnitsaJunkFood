using System.Collections.Generic;

namespace JunkBackEnd.Entities
{
    public class ResponseWrapperEntity{
        public List<AssortmentEntity> AssortmentList { get; set; }
        public List<OutletEntity> OutletList { get; set; }
    }
}
