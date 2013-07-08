using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JunkBackEnd.Entities
{
    public class ResponseWrapperEntity{
        public List<AssortmentEntity> AssortmentList { get; set; }
        public List<OutletEntity> OutletList { get; set; }
    }
}
