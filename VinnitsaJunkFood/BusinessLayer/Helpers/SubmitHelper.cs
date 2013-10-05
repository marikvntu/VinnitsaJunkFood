using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JunkBackEnd.Entities;

namespace JunkBackEnd.BusinessLayer
{
    public static class SubmitHelper{
#region Enums
        enum cities { Vinnitsa };
#endregion

#region Methods
       internal static bool isUniqueMeal(string entityName, ref List<AssortmentEntity> list){
            bool isUnique = true;
            foreach (var item in list){
                if (item.EntityName == entityName) {
                    return false;
                }
            }

            return isUnique;
        }  
      
        /// <summary>
        /// Submits meal that was specified in meal parameter. NOTE: It must be unique meal, use isUnique function before.
        /// </summary>
        /// <param name="meal">reference AssortmentEntity, at the end EntityId is updated in case of successful DB submit</param>
        /// <param name="opResult"></param>
        /// <returns></returns>
        internal static bool SubmitMeal(ref AssortmentEntity meal, out string opResult){            
            int mealId;
            
            if (meal.EntityName.Length > 50) {
                opResult = "Failed: Meal name cannot exceed 50 characters";
                return true;
            }

            //check if operation was successful
            if (!DataAccessLayer.DbSubmitHelper.Instance.AddAssortmentEntity(meal.EntityName, out mealId) ){
                opResult = "Failed: Exception occurred during adding meal: " + meal.EntityName;
                return false;
            }

            meal.EntityID = mealId;            
                        
            opResult = "Success: Meal " + meal.EntityName + " was added successfully.";
            return true;
        }

        internal static bool SubmitPriceList(ref OutletEntity currentOutlet, List<PriceListEntity> requestPriceList, ref List<AssortmentEntity> assortmentEntitiesList, out string opResult){
            if (requestPriceList.Count == 0) {
                opResult = "Failed: Empty price list";
                return false;
            }

            if (!tryFillAssortmentIDs(ref requestPriceList, ref assortmentEntitiesList)){
                opResult = "Failed: Could not associate all meal names with assortment items";
                return false;
            };

            //compare with existing price list            
            if(currentOutlet.AssortmentPriceList.Count == requestPriceList.Count
                && areSamePriceLists(currentOutlet.AssortmentPriceList, ref requestPriceList)){
                opResult = "Failed: Price list is the same";
                return false;
            }

            if (!DataAccessLayer.
                DbSubmitHelper.
                Instance.
                UpdatePriceList(currentOutlet.EntityID,
                                requestPriceList)) { opResult = "Failed: Could not update DB"; return false; }

            opResult = "Success: Price list updated successfully";
            return true;            
        }

        private static bool areSamePriceLists(List<ItemPriceWrapper> existingList, ref List<PriceListEntity> requestPriceList){
            var existingSortedList = existingList
                                        .OrderBy(row => row.Price)
                                        .ThenBy(row => row.AssortmentId);
            var index = 0;
            foreach (var item in requestPriceList
                                 .OrderBy(row => row.Price)
                                 .ThenBy(row => row.EntityID))
            {
                //add price comparison
                if ((item.EntityID != existingSortedList.ElementAt(index).AssortmentId) ||
                    (item.Price != existingSortedList.ElementAt(index).Price))
                {
                    return false;
                }
                index++;
            }
            return true;
        }

        private static bool tryFillAssortmentIDs(ref List<PriceListEntity> requestPriceList, ref List<AssortmentEntity> assortmentList){
            bool isSuccessful = true;
            try{
                foreach (var item in requestPriceList)
                {
                    item.EntityID = assortmentList
                        .Where(a => a.EntityName == item.EntityName)
                        .Select(a => a.EntityID).First();
                }
            }
            catch (Exception){
                //usually happens if no elements found
                isSuccessful = false;
            }

            return isSuccessful;
        }

        private static bool isInCityLimits(cities city, OutletEntity outlet){
            bool isValid = false;
            switch (city) {
                case cities.Vinnitsa:
                    isValid = (outlet.Latitude > 49.19)
                                && (outlet.Latitude < 49.29)
                                && (outlet.Longitude > 28.39)
                                && (outlet.Longitude < 28.58);
                    break;
                
                default: 
                    break;
            }
            return isValid;
        }

        internal static bool SubmitNewOutlet(ref OutletEntity outlet, out string opResult){
            int outletId;

            if (String.IsNullOrWhiteSpace(outlet.EntityName)){
                opResult = "Error: Enter outlet name correctly";
                return false;
            }

            if (outlet.EntityName.Length > 50){
                opResult = "Error: Outlet name may not exceed 50 characters";
                return false;
            }

            if (outlet.Description.Length > 2000) {
                opResult = "Error: Description may not exceed 2000 characters";
                return false;
            }

            if (outlet.ImageUrl.Length > 2000){
                opResult = "Error: Image Url may not exceed 2000 characters";
                return false;
            }

            if (!isInCityLimits(cities.Vinnitsa, outlet)){
                opResult = "Error: The outlet must be in city limits";
                return false;
            }

            if (String.IsNullOrWhiteSpace(outlet.ImageUrl)){
                outlet.ImageUrl = null;
            }

            try{
                if (DataAccessLayer.DbSubmitHelper.Instance.AddOutletEntity(outlet, out outletId)) {
                    outlet.EntityID = outletId;
                };
            }
            catch (Exception ex){
                opResult = "Error occurred during saving new outlet to DB" + ex.Message;
                return false;
            }

            opResult = "Success: Outlet was added";
            return true;
        }

        internal static bool SubmitComment(int outletId ,CommentEntity comment, out string operationStatus){
            if (comment.UserName.Length > 50) { operationStatus = "Failed: Username cannot be longer than 50 characters."; return false; }
            if (comment.CommentText.Length > 500) { operationStatus = "Failed: Comment cannot be longer than 500 characters."; return false; }
            int commentId;

            try{
                if (!DataAccessLayer.DbSubmitHelper.Instance.AddCommentEntity(outletId, comment, out commentId)){
                    operationStatus = "Error occurred during saving new comment to DB. Unspecified error";
                    return false;
                };
            }
            catch (Exception ex){
                operationStatus = "Error occurred during saving new comment to DB. Message:" + ex.Message;
                return false;
            }
            //over-write OutletID with CommentId
            //We stored OutletID in EntityID to pass the object as a parameter
            comment.EntityID = commentId;
            operationStatus = "Success";
            return true;
        }

        /// <summary>
        /// Saves rating in the DB, outputs operation status, new mean rating and new nyber of votes
        /// </summary>
        /// <param name="outlet">specify outlet you want to rate</param>
        /// <param name="newRating">new rating voted by user</param>
        /// <param name="operationStatus">operation status string</param>
        /// <param name="calculatedRating">new mean rating, in case of successful operation</param>
        /// <param name="calculatedVotes">new vote count, in case of successful operation</param>
        /// <returns></returns>
        internal static bool RateOutlet(OutletEntity outlet, int newRating, out string operationStatus, out double calculatedRating, out int calculatedVotes){
            calculatedRating = 0;
            calculatedVotes = 0;
            if (outlet == null) { 
                operationStatus = "Error: Could not find outlet by Id";                                 
                return false;
            }            

            bool success;
            operationStatus = "Success";

            //recalculate rating with regard to number of votes
            calculatedVotes = outlet.Votes + 1;
            calculatedRating = (outlet.OutletRating * outlet.Votes + newRating) / (calculatedVotes);

            success = DataAccessLayer.DbSubmitHelper.Instance.UpdateRating(outlet.EntityID, calculatedRating, calculatedVotes);
            
            if (!success){
                operationStatus = "Error occurred during saving rating to DB.";                
            }           

            return success;
        }

        /// <summary>
        /// Saves comment rating in the DB, outputs is successful status, and updates the comment rating 
        /// </summary>
        /// <param name="comment">referenced comment object, its rating is updated in case of successful operation</param>
        /// <param name="voteDelta">int value for vote delta +1 or -1</param>
        /// <param name="opResult">Outputs the new comment rating if successful, or the error message</param>
        /// <returns></returns>
        internal static bool VoteComment(ref CommentEntity comment, int delta, out string opResult){
            bool success = true;
            opResult = "Comment with id :" + comment.EntityID.ToString() + " was voted with delta " + delta.ToString() + " successfully";

            if (comment.CommentRating + delta == int.MinValue || comment.CommentRating + delta == int.MaxValue) {
                opResult = "Terminal rating acheived";
                return false;
            }

            success = DataAccessLayer.DbSubmitHelper.Instance.VoteForComment(comment.EntityID, delta);
            if (!success) {
                opResult = "Could not save comment to DB";
                return false;
            }

            comment.CommentRating += delta;            
            return success;
        }
#endregion    
        
    }
}
