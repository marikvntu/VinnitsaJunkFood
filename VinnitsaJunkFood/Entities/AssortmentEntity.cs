using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JunkBackEnd.Entities{
    [DataContract]
    public class AssortmentEntity: BaseEntity{        
        //TODO:ADD FUNCTIONALITY FOR INGRIDIENTS
        //public List<int> IngridientsIdList { get; set; }

        [DataMember]
        public string MealDescription { get; set; } 
    }    
}