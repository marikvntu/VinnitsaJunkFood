using System.Collections.Generic;

namespace JunkBackEnd.Entities
{
    public class AssortmentEntity: BaseEntity{        
        //TODO:ADD FUNCTIONALITY FOR INGRIDIENTS
        //public List<int> IngridientsIdList { get; set; }
        public string MealDescription { get; set; } 
    }

    public class AssortmentJson {
        public string name {get; set;}
        public string description { get; set; }
    }
}