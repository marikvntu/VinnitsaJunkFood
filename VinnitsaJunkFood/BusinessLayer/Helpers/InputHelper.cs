using JunkBackEnd.BusinessLayer;
using JunkBackEnd.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;

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
        /// <param name="meal">output of assortment entity</param>
        /// <param name="assortment">in-memory assortment list</param>
        /// <param name="opMessage">operation error message. Blank if no errors occured.</param>
        /// <returns></returns>
        public static bool ValidateAndPrepareMealData(AssortmentEntity meal, ref List<AssortmentEntity> assortment, out string opMessage) {
            opMessage = "";                                              

            if (meal == null){
                opMessage = "Failed: meal data was sent as null";
                return false;
            }

            bool isUnique = SubmitHelper.IsUniqueMeal(meal.EntityName, ref assortment);
            //check if unique name
            if (!isUnique) { 
                opMessage = "Failed: There is already a meal with name: " + meal.EntityName;
                return false;
            }

            return true;
        }


        public static bool ValidateAndPreparePriceListData( string priceListString,
                                                            out string opMessage, 
                                                            out List<PriceListEntity> priceList){
            bool successful = true;            
            priceList = null;
            opMessage = "";           

            priceList = InputHelper.UnwrapPriceListString(priceListString);
            if (priceList == null){                 
                opMessage = "Failed: Could not unparse price list params: " + priceListString;
                return false;
            }

            return successful;
        }

        /// <summary>
        /// Unwraps the price list string.
        /// </summary>
        /// <param name="priceListParams">The price list parameters.</param>
        /// <returns></returns>
        internal static List<PriceListEntity> UnwrapPriceListString(string priceListParams){
            var priceList = new List<PriceListEntity>();
            //in case all entities cleared
            if (priceListParams.Length == 0) { return priceList; }

            float priceValue;
            try {
                // "|" is a separator between price list rows, 
                // '÷' is a separator between assortment name and price 
                foreach (var priceRow in priceListParams.Split('|')){
                    var priceRowArray = priceRow.Split('÷');
                    //replace comma with dot for easier parsing
                    priceValue = float.Parse(priceRowArray[1].Replace(',', '.'), CultureInfo.InvariantCulture);
                    var priceItem = new PriceListEntity(){
                        EntityName = priceRowArray[0],
                        Price = priceValue
                    };
                    priceList.Add(priceItem);
                }
            }
            catch (Exception) {
                priceList = null;
            }
            return priceList;
        }
    }
}