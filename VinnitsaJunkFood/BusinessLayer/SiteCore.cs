using System;
using System.Collections.Generic;
using System.Linq;
using JunkBackEnd.Entities;
using JunkBackEnd.DataAccessLayer;
using System.Globalization;
using VinnitsaJunkFood.Logger;
using VinnitsaJunkFood.BusinessLayer;
using VinnitsaJunkFood.DataAccessLayer;


namespace JunkBackEnd.BusinessLayer{
    public class SiteCore{
#region DataMembers
        
        private List<OutletEntity> _outletsList;
        private List<AssortmentEntity> _assortmentList;
        private List<BaseEntity> _ingridientList;
        private List<GeoLocationEntity> _locationsList;

        // k - outletId, v - comments list        
        private Dictionary<int, List<CommentEntity>> _commentsDictionary;

        private string _initialJsonStringCache;
        private FileLogger _logger;
#endregion

#region Constructor
        protected SiteCore(){
            _outletsList = new List<OutletEntity>();
            _assortmentList = new List<AssortmentEntity>();
            _ingridientList = new List<BaseEntity>();
            _locationsList = new List<GeoLocationEntity>();
            _commentsDictionary = new Dictionary<int, List<CommentEntity>>();

            _logger = new FileLogger();
            _logger.WriteLog("Site Manager initialized, retrieve entities from DB");
            initializeEntities();
            _logger.WriteLog("Entities initialized");
        }
#endregion       

#region Methods

        protected void WriteToLog(string msg) {
            _logger.WriteLog(msg);
        }

        private void initializeEntities(){
            IDbReader dbReader = new DbReadHelper();
            dbReader.FillAllEntitiesFromDb(ref _outletsList,
                                           ref _assortmentList,
                                           ref _ingridientList,
                                           ref _locationsList,
                                           ref _commentsDictionary);

            refreshJsonStringCache();
            sortCommentsDictionaryByLikes();
        }        

        /// <summary>
        /// Returns cached JSON with initial entities for first time request
        /// </summary>
        /// <returns></returns>
        public string ReturnInitialData(){
            _logger.WriteLog("Returning cache data...");
            return _initialJsonStringCache;                    
        }

        /// <summary>
        /// Returns JSON string with comments for particular outlet
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<CommentEntity> GetCommentsForOutlet(int id){
            if (_commentsDictionary.Keys.Contains(id)){
                _logger.WriteLog("Returning comments for: " + id.ToString());
                //return ResponseBuilder.GenerateJsonFromComments(_commentsDictionary[id]);
                return _commentsDictionary[id];
            }
            return new List<CommentEntity>();
        }

        protected string SubmitMeal(AssortmentEntity meal, out bool success) {
            string submitStatus;

            success = InputHelper.ValidateAndPrepareMealData(meal, ref _assortmentList, out submitStatus);
            if (!success) { return submitStatus; }

            success = SubmitHelper.SubmitMeal(ref meal, out submitStatus);
            if (!success) { return submitStatus; };

            //add to assortment list, in successful scenario
            _assortmentList.Add(meal);

            refreshJsonStringCache();

            return submitStatus;
        }

        protected string UpdatePriceList(int outletId, string priceListParams, out bool success) {
            string submitStatus;
            List<PriceListEntity> requestPriceList;

            success = InputHelper.ValidateAndPreparePriceListData(priceListParams,
                                                                        out submitStatus,
                                                                        out requestPriceList);

            if (!success) { return submitStatus; }

            //check if OutletID exists;
            OutletEntity currentOutlet = findOutletById(outletId);

            if (currentOutlet == null) { return "Failed: Could not find outlet with ID: " + outletId.ToString(); };

            //submit new price list            
            success = SubmitHelper.SubmitPriceList(ref currentOutlet,
                                                requestPriceList,
                                                ref _assortmentList,
                                                out submitStatus);

            if (!success) { return submitStatus; }

            //overwrite price list for the given outlet
            currentOutlet.AssortmentPriceList = requestPriceList.
                                                Select(p => new ItemPriceWrapper {
                                                    AssortmentId = p.EntityID,
                                                    Price = p.Price
                                                }).ToList();

            refreshJsonStringCache();
            return "Success: Price List successfully updated";
        }

        protected string AddNewOutlet(OutletEntity outlet, out bool success) {
            string submitStatus;
            success = false;

            if (outlet == null) { return "Outlet data was sent as null"; }

            success = SubmitHelper.SubmitNewOutlet(ref outlet, out submitStatus);

            if (!success) { return submitStatus; }

            _outletsList.Add(outlet);
            _commentsDictionary.Add(outlet.EntityID, new List<CommentEntity>());
            outlet.AssortmentPriceList = new List<ItemPriceWrapper>();
            refreshJsonStringCache();
            return submitStatus;
        }

        protected object AddComment(CommentEntity comment, out bool successful) {
            comment.DateTime = DateTime.Now.ToString("hh:mm on dd.MM.yyyy", CultureInfo.InvariantCulture);
            int outletId = comment.EntityID;

            string operationStatus;
            if (!_commentsDictionary.ContainsKey(outletId)) {
                operationStatus = "Could not find outletId:" + outletId.ToString();
                successful = false;
                return operationStatus;
            }

            successful = SubmitHelper.SubmitComment(outletId, comment, out operationStatus);

            if (!successful) { return operationStatus; }

            insertCommentConsideringRating(_commentsDictionary[outletId], comment);

            return _commentsDictionary[outletId];
        }

        protected object RateOutlet(int outletId, int newRating, out bool success, out string operationStatus) {
            success = true;
            double newAverageRating;
            int newVoteCount;

            OutletEntity outlet = findOutletById(outletId);

            success = SubmitHelper.RateOutlet(outlet, newRating, out operationStatus, out newAverageRating, out newVoteCount);

            if (!success) { return operationStatus; };

            //update existing in-memory data
            outlet.OutletRating = newAverageRating;
            outlet.Votes = newVoteCount;

            operationStatus = "Outlet with Id - " + outletId.ToString() + " was voted with rating :" + newRating.ToString() + " resulting in new rating: " + newAverageRating.ToString();

            refreshJsonStringCache();
            return outlet;
        }

        protected string ThumbsComment(int outletId, int commentId, string thumbs, out bool success) {
            string opResult = "Failed:";
            success = false;

            if (thumbs != "up" && thumbs != "down") {
                opResult += "Incorrect parameter name for Thumbs - it is: " + thumbs;
                return opResult;
            }

            int voteDelta = thumbs == "up" ? 1 : -1;

            if (!_commentsDictionary.ContainsKey(outletId)) {
                opResult += "Could not find outletId:" + outletId.ToString();
                return opResult;
            }

            List<CommentEntity> commentList = _commentsDictionary[outletId];
            CommentEntity comment = commentList.Find(c => c.EntityID == commentId);

            if (comment == default(CommentEntity)) {
                opResult += "Could not find commentId:" + commentId.ToString();
                return opResult;
            }

            success = SubmitHelper.VoteComment(ref comment, voteDelta, out opResult);
            if (!success) {
                return opResult;
            }

            sortOutletCommentsByLikes(commentList);

            return comment.CommentRating.ToString();
        }

        #region PrivateMethods
        private OutletEntity findOutletById(int outletID) {
            return _outletsList
                   .Where(outlet => outlet.EntityID == outletID)
                   .FirstOrDefault();
        }

        /// <summary>
        /// Performs sorting of outlet entities by name and meal entity by name
        /// </summary>
        private void sortMainEntitiesByName() {
            _outletsList = _outletsList.
                            OrderBy(outlet => outlet.EntityName).
                            ToList();

            _assortmentList = _assortmentList.
                            OrderBy(outlet => outlet.EntityName).
                            ToList();

        }

        private void sortOutletCommentsByLikes(List<CommentEntity> comments) {
            List<CommentEntity> top2 = comments.OrderByDescending(c => c.CommentRating).Take(2).ToList();
            checkAndSwapCommentsInList(top2, comments, 0);
            checkAndSwapCommentsInList(top2, comments, 1);
        }

        private void checkAndSwapCommentsInList(List<CommentEntity> topList, List<CommentEntity> fullList, int index) {
            if (fullList.Count <= index || topList.Count <= index) { return; }

            if (topList[index] != fullList[index]) {
                fullList.Remove(topList[index]);
                fullList.Insert(index, topList[index]);
            }
        }

        private void sortCommentsDictionaryByLikes() {
            foreach (var commentList in _commentsDictionary) {
                sortOutletCommentsByLikes(commentList.Value);
            }
        }

        /// <summary>
        /// Tries to insert as close to the beginning as possible. If 1 and 2 comments are liked, inserts at 3-rd position
        /// </summary>
        /// <param name="list"></param>
        /// <param name="comment"></param>
        private void insertCommentConsideringRating(List<CommentEntity> list, CommentEntity comment) {
            for (int i = 0; i < 2; i++) {
                if (list.Count > i && list[i].CommentRating <= 0) {
                    list.Insert(i, comment);
                    return;
                }
            }
            if (list.Count > 2) { list.Insert(2, comment); }
            else { list.Add(comment); }
        }

        /// <summary>
        /// Rebuilds cached JSON string after in-memory entities refresh
        /// </summary>
        private void refreshJsonStringCache() {
            sortMainEntitiesByName();
            _logger.WriteLog("Refreshing Json string cache...");
            _initialJsonStringCache = JsonHelper.ReturnJsonFromEntities(_outletsList, _assortmentList);
        }
        #endregion
#endregion
    }
}
