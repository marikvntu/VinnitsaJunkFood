using JunkBackEnd.BusinessLayer;
using JunkBackEnd.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VinnitsaJunkFood.Logger;

namespace VinnitsaJunkFood.BusinessLayer
{
    public sealed class SiteManager : SiteCore {

        #region DataMembers
        private static SiteManager _instance = null;
        private static object _lockObj = new object();         
        #endregion

        #region Constructor
        private SiteManager() { } 
        #endregion

        #region Properties
        /// <summary>
        /// Gets an Instance of SiteManager
        /// </summary>
        public static SiteManager Instance{
            get{
                if (_instance == null){
                    _instance = new SiteManager();
                }

                return _instance;
            }
        } 
        #endregion        

        #region Methods
        /// <summary>
        /// Checks and adds new meal
        /// </summary>
        /// <param name="mealJson"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public string SubmitAssortmentEntity(AssortmentEntity mealObject, out bool success) {
            //thread-safe saving
            lock (_lockObj) {
                string submitStatus = SubmitMeal(mealObject, out success);

                WriteToLog(submitStatus);
                return submitStatus;
            }
        }


        /// <summary>
        /// Performs high level submit handling of price list from request string
        /// </summary>
        /// <param name="outletID">EntityID of outlet</param>
        /// <param name="priceListParams">Request string from webform containing MealName/price pairs</param>
        /// <returns>status string of the current operation</returns>
        public string SubmitPriceList(int outletID, string priceListParams, out bool success) {
            lock (_lockObj) {
                string submitStatus = UpdatePriceList(outletID, priceListParams, out success);

                WriteToLog(submitStatus);
                return submitStatus;
            }
        }

        /// <summary>
        /// Sumbits new outlet and returns operation result
        /// </summary>
        /// <param name="paramJson">OutletEntity in form of JSON</param>
        /// <returns></returns>
        public string SubitNewOutlet(OutletEntity outletObject, out bool success) {
            lock (_lockObj) {
                string submitStatus = AddNewOutlet(outletObject, out success);

                WriteToLog(submitStatus);
                return submitStatus;
            }
        }

        /// <summary>
        /// Submits new comment to the database and in-memory data and returns operation result
        /// </summary>
        /// <param name="outletID">ID of the outlet you want to comment</param>
        /// <param name="userName">User name</param>
        /// <param name="commentText">Text of the comment</param>
        /// <returns></returns>
        public object SubmitComment(CommentEntity comment, out bool successful) {
            lock (_lockObj) {
                object result = AddComment(comment, out successful);
                string msg = successful ?
                                "Comment was added for outletID- " + comment.EntityID.ToString() + " from user: " + comment.UserName + " with text :\n" + comment.CommentText
                                : result as string;

                WriteToLog(msg);
                return result;
            }
        }

        public string VoteComment(int outletId, int commentId, string thumbs, out bool successful) {
            lock (_lockObj) {
                string result = ThumbsComment(outletId, commentId, thumbs, out successful);

                WriteToLog(result);
                return result;
            }
        }

        public object RateOutlet(int outletId, int newRating, out bool success) {
            lock (_lockObj) {
                string opResult;
                object result = RateOutlet(outletId, newRating, out success, out opResult);

                WriteToLog(opResult);
                return result;
            }
        }             
        #endregion

    }
}