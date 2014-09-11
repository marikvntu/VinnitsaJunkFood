using System.Collections.Generic;
using System.ServiceModel.Activation;
using System.Web;
using JunkBackEnd.BusinessLayer;
using JunkBackEnd.Entities;
using VinnitsaJunkFood.BusinessLayer;
using VinnitsaJunkFood.BusinessLayer.Helpers;
using VinnitsaJunkFood.Entities;

namespace VinnitsaJunkFood
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class JunkService : IJunkService
    {
        #region Get requests

        /// <summary>
        ///     Returns comments for get request
        /// </summary>
        /// <param name="outletId"></param>
        /// <returns></returns>
        public ResponseContainer GetComments(int outletId)
        {
            List<CommentEntity> list = SiteCore.Instance.GetCommentsForOutlet(outletId);

            var response = new ResponseContainer(true, list);

            return response;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        public ResponseContainer Initialize()
        {
            return new ResponseContainer(true, SiteCore.Instance.ReturnInitialData());
        }

        #endregion

        #region PostRequests

        /// <summary>
        /// Adds the comment.
        /// </summary>
        /// <param name="requestData">The request data.</param>
        /// <returns></returns>
        public ResponseContainer AddComment(CommentEntity requestData)
        {
            bool success;

            object result = SiteCore.Instance.SubmitComment(requestData, out success);
            var response = new ResponseContainer(success, result);

            return response;
        }

        /// <summary>
        /// Votes the comment.
        /// </summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <param name="thumbs">The thumbs.</param>
        /// <param name="outletId">The outlet identifier.</param>
        /// <returns></returns>
        public ResponseContainer VoteComment(int commentId, string thumbs, int outletId)
        {
            if (!ValidateSessionCache("Comment:" + commentId, "Voted"))
            {
                var errorResponse = new ResponseContainer(false, "You already voted for this comment");
                return errorResponse;
            }

            string result;
            bool success = SiteCore.Instance.VoteComment(outletId, commentId, thumbs, out result);
            var response = new ResponseContainer(success, result);
            return response;
        }

        /// <summary>
        /// Rates the outlet.
        /// </summary>
        /// <param name="outletId">The outlet identifier.</param>
        /// <param name="rating">The rating.</param>
        /// <returns></returns>
        public ResponseContainer RateOutlet(int outletId, int rating)
        {
            if (!ValidateSessionCache("RateId:" + outletId, "rated"))
            {
                var errorResponse = new ResponseContainer(false, "You already rated this outlet");
                return errorResponse;
            }

            OutletEntity outlet;
            string operationResult;
            bool success = SiteCore.Instance.RateOutlet(outletId, rating, out operationResult, out outlet);

            var response = new ResponseContainer(success, operationResult);
            return response;
        }

        /// <summary>
        /// Adds the outlet.
        /// </summary>
        /// <param name="requestData">The request data.</param>
        /// <returns></returns>
        public ResponseContainer AddOutlet(OutletEntity requestData)
        {
            string opResult;
            bool success = SiteCore.Instance.SubmitNewOutlet(requestData, out opResult);

            var response = new ResponseContainer(success, opResult);
            return response;
        }

        /// <summary>
        /// Submits the meal.
        /// </summary>
        /// <param name="requestData">The request data.</param>
        /// <returns></returns>
        public ResponseContainer SubmitMeal(AssortmentEntity requestData)
        {
            string result;
            bool success = SiteCore.Instance.SubmitMeal(requestData, out result);
            
            var response = new ResponseContainer(success, result);
            return response;
        }

        /// <summary>
        /// Updates the price list.
        /// </summary>
        /// <param name="requestData">The request data.</param>
        /// <returns></returns>
        public ResponseContainer UpdatePriceList(BaseEntity requestData)
        {
            string result;
            bool success = SiteCore.Instance.UpdatePriceList(requestData.EntityID, requestData.EntityName,
                out result);
            
            var response = new ResponseContainer(success, result);
            return response;
        }

        /// <summary>
        /// Submits the feedback.
        /// </summary>
        /// <param name="requestData">The request data.</param>
        /// <returns></returns>
        public ResponseContainer SubmitFeedback(FeedbackEntity requestData)
        {
            string message = requestData.Message.Replace("\n", "<br>") + "<br><br>contact info: " +
                             requestData.ContactInfo;

            bool success = FeedbackHelper.SendMailMessage("junkfood.notification@mail.ru",
                "junkfood.vn@gmail.com",
                "Junkfood feedback: " + requestData.Subject,
                message);

            var response = new ResponseContainer(success, success);
            return response;
        }

        #endregion

        #region Private Methods

        private bool ValidateSessionCache(string key, string value)
        {
            var cachedObject = HttpContext.Current.Session[key] as string;

            if (cachedObject != null)
            {
                return false;
            }

            cacheObject(key, value);

            return true;
        }

        private void cacheObject(string key, string value)
        {
            HttpContext.Current.Session.Add(key, value);
        }

        #endregion
    }
}