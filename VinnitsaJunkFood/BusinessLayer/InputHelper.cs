using JunkBackEnd.BusinessLayer;
using JunkBackEnd.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VinnitsaJunkFood.BusinessLayer{
    
    /// <summary>
    /// This class is needed to validate user input
    /// and to convert string data to specific types
    /// </summary>
    public static class InputHelper{

        
        /// <summary>
        /// Unwraps meal json string, validate and checks uniqueness. If all passed, 
        /// returns true and outputs AssortmentEntity. If failed returns false, and null.
        /// </summary>
        /// <param name="mealJson">Stringified json from request</param>
        /// <param name="assortment">in-memory assortment list</param>
        /// <param name="opMessage">operation error message. Blank if no errors occured.</param>
        /// <param name="meal">output of assortment entity</param>
        /// <returns></returns>
        public static bool ValidateAndPrepareMealData(string mealJson, ref List<AssortmentEntity> assortment, out string opMessage, out AssortmentEntity meal) {
            bool successful = true;
            opMessage = "";
            meal = null;

            if (String.IsNullOrWhiteSpace(mealJson)) { 
                opMessage = "Failed: Blank json";
                return false;
            }

            meal = RequestHelper.DeserializeJson<AssortmentEntity>(mealJson);

            if (meal == null){
                opMessage = "Failed: Could not deserialize meal json";
                return false;
            }

            bool isUnique = SubmitHelper.isUniqueMeal(meal.EntityName, ref assortment);
            //check if unique name
            if (!isUnique) { 
                opMessage = "Failed: There is already a meal with name: " + meal.EntityName;
                return false;
            }

            return successful;
        }


        public static bool ValidateAndPreparePriceListData( string outletIdString, 
                                                            string priceListString,                                                             
                                                            out string opMessage, 
                                                            out List<PriceListEntity> priceList,
                                                            out int outletID){
            bool successful = true;            
            priceList = null;
            opMessage = "";

            if (!int.TryParse(outletIdString, out outletID)){
                opMessage = "Failed: Could not convert " + outletIdString + " into an outletID";
                return false;
            }

            priceList = RequestHelper.UnwrapPriceListString(priceListString);
            if (priceList == null){                 
                opMessage = "Failed: Could not unparse price list params: " + priceListString;
                return false;
            }

            return successful;
        }




        public static bool ValidateAndPrepareRatingData(string outletIdString, 
                                                            string newRatingString, 
                                                            out string opStatus,
                                                            out int outletId, 
                                                            out int newRating){
            opStatus = "";
            outletId = newRating = 0;

            if (!int.TryParse(outletIdString, out outletId)){
                    opStatus = "Failed: Could not convert " + outletIdString + " into an outletID";
                    return false;
            }
            if (!int.TryParse(newRatingString, out newRating)){
                    opStatus = "Failed: Could not convert " + newRatingString + " into a new rating";
                    return false;
            }

            return true;
        }
    }
}