using JunkBackEnd.BusinessLayer;
using JunkBackEnd.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using VinnitsaJunkFood.BusinessLayer;
using VinnitsaJunkFood.Entities;
//using VinnitsaJunkFood.Entities.RequestEntities;

namespace VinnitsaJunkFood {

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class JunkService : IJunkService {

        #region Get requests
        /// <summary>
        /// Returns comments for get request
        /// </summary>
        /// <param name="outletId"></param>
        /// <returns></returns>
        public ResponseContainer GetComments(int outletId) {
            List<CommentEntity> list = SiteManager.Instance.GetCommentsForOutlet(outletId);

            var response = new ResponseContainer(true, list);

            return response;
        }

        public ResponseContainer Initialize() {
            return new ResponseContainer(true, SiteManager.Instance.ReturnInitialData());
        }
        #endregion

        #region PostRequests
        public ResponseContainer AddComment(CommentEntity requestData) {
            bool success;

            object result = SiteManager.Instance.SubmitComment(requestData, out success);
            var response = new ResponseContainer(success, result);

            return response;
        }

        public ResponseContainer VoteComment(int commentId, string thumbs, int outletId) {
            if (!validateSessionCache("Comment:" + commentId.ToString(), "Voted")) {
                var errorResponse = new ResponseContainer(false, "You already voted for this comment");
                return errorResponse;
            }

            bool success;
            string result = SiteManager.Instance.VoteComment(outletId, commentId, thumbs, out success);
            var response = new ResponseContainer(success, result);
            return response;
        }

        public ResponseContainer RateOutlet(int outletId, int rating) {
            if (!validateSessionCache("RateId:" + outletId.ToString(), "rated")) {
                var errorResponse = new ResponseContainer(false, "You already rated this outlet");
                return errorResponse;
            }


            bool success;
            object opStatus = SiteManager.Instance.RateOutlet(outletId, rating, out success);

            var response = new ResponseContainer(success, opStatus);
            return response;
        }

        public ResponseContainer AddOutlet(OutletEntity requestData) {
            bool success;
            string opResult = SiteManager.Instance.SubitNewOutlet(requestData, out success);

            var response = new ResponseContainer(success, opResult);

            return response;
        }

        public ResponseContainer SubmitMeal(AssortmentEntity requestData) {
            bool success;
            string result = SiteManager.Instance.SubmitAssortmentEntity(requestData, out success);
            var response = new ResponseContainer(success, result);
            return response;
        }

        public ResponseContainer UpdatePriceList(BaseEntity requestData) {
            bool success = false;
            string result = SiteManager.Instance.SubmitPriceList(requestData.EntityID, requestData.EntityName, out success);
            var response = new ResponseContainer(success, result);
            return response;
        }

        public ResponseContainer SubmitFeedback(FeedbackEntity requestData) {
            string message = requestData.Message.Replace("\n", "<br>") + "<br><br>contact info: " + requestData.ContactInfo;

            bool success = FeedbackHelper.SendMailMessage("junkfood.notification@mail.ru",
                                                            "junkfood.vn@gmail.com",
                                                            "Junkfood feedback: " + requestData.Subject,
                                                            message);

            var response = new ResponseContainer(success, success);
            return response;
        }
        #endregion

        #region PrivateMethods
        private bool validateSessionCache(string key, string value) {
            string cachedObject = HttpContext.Current.Session[key] as string;

            if (cachedObject != null) { return false; };

            cacheObject(key, value);

            return true;
        }

        private void cacheObject(string key, string value) {
            HttpContext.Current.Session.Add(key, value);
        }
        #endregion
    }
}
