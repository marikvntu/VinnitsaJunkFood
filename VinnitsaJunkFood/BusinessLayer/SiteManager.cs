using System;
using System.Collections.Generic;
using System.Linq;
using JunkBackEnd.Entities;
using JunkBackEnd.DataAccessLayer;
using System.Globalization;
using VinnitsaJunkFood.Logger;

namespace JunkBackEnd.BusinessLayer{
    public class SiteManager{
#region DataMembers
        private static SiteManager _instance = null;
        private List<OutletEntity> _outletsList;
        private List<AssortmentEntity> _assortmentList;
        private List<BaseEntity> _ingridientList;
        private List<GeoLocationEntity> _locationsList;

        // k - outletId, v - comments list        
        private Dictionary<int, List<CommentEntity>> _commentsDictionary;

        private string _initialJsonStringCache;
        private static object _lockObj = new object();
        private FileLogger _logger;
#endregion

#region Constructor
        private SiteManager(){
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

        /// <summary>
        /// Returns filled OutletsList
        /// </summary>
        public List<OutletEntity> OutletsList {
            get { return _outletsList; }
        }

        /// <summary>
        /// Returns a list of Assortment
        /// </summary>
        public List<AssortmentEntity> AssortmentList {
            get { return _assortmentList; }
        }
        
        /// <summary>
        /// Returns a list of Ingridients
        /// </summary>
        public List<BaseEntity> IngridientList{
            get { return _ingridientList; }
        }
#endregion

#region Methods
        private void initializeEntities(){
            DbReadHelper.Instance.FillAllEntitiesFromDb(ref _outletsList,
                                                        ref _assortmentList, 
                                                        ref _ingridientList,
                                                        ref _locationsList,
                                                        ref _commentsDictionary);
            refreshJsonStringCache();
            sortCommentsDictionaryByLikes();
        }

        /// <summary>
        /// Rebuilds cached JSON string after in-memory entities refresh
        /// </summary>
        private void refreshJsonStringCache(){
            sortMainEntitiesByName();
            _logger.WriteLog("Refreshing Json string cache...");
            _initialJsonStringCache = ResponseBuilder.ReturnJsonFromEntities(_outletsList, _assortmentList);
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
        public string GetCommentsForOutlet(int id){            
            if (_commentsDictionary.Keys.Contains(id)){
                _logger.WriteLog("Returning comments for: " + id.ToString());
                return ResponseBuilder.GenerateJsonFromComments(_commentsDictionary[id]);
            }
            return string.Empty;
        }        

        /// <summary>
        /// Checks and adds new meal
        /// </summary>
        /// <param name="mealJson"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public string SubmitAssortmentEntity(string mealJson, string description = null){           
            //thread-safe saving
            lock (_lockObj){
                string submitStatus = submitMeal(mealJson, description);

                _logger.WriteLog(submitStatus);
                return submitStatus;
            }
        }

        private string submitMeal(string mealJson, string description = null) {
            string submitStatus;
            if (String.IsNullOrWhiteSpace(mealJson)) { return "Failed: Blank json"; }

            AssortmentEntity meal = RequestHelper.DeserializeJson<AssortmentEntity>(mealJson);
            bool isUnique = SubmitHelper.isUniqueMeal(meal.EntityName, ref _assortmentList);
            //check if unique name
            if (!isUnique) { return "Failed: There is already a meal with name: " + meal.EntityName; }

            if (!SubmitHelper.SubmitMeal(ref meal, out submitStatus)) { return submitStatus; };

            //add to assortment list, in successful scenario
            _assortmentList.Add(meal);

            refreshJsonStringCache();
            return submitStatus;
        }

        /// <summary>
        /// Performs high level submit handling of price list from request string
        /// </summary>
        /// <param name="outletID">EntityID of outlet</param>
        /// <param name="priceListParams">Request string from webform containing MealName/price pairs</param>
        /// <returns>status string of the current operation</returns>
        public string SubmitPriceList(int outletID, string priceListParams){
            lock (_lockObj){
                string submitStatus = submitPriceList(outletID, priceListParams);

                _logger.WriteLog(submitStatus);
                return submitStatus;
            }
        }

        private string submitPriceList(int outletID, string priceListParams){
            //check if OutletID exists;
            OutletEntity currentOutlet = findOutletById(outletID);

            if (currentOutlet == null) {return "Failed: Could not find outlet with ID: " + outletID.ToString(); };

            List<PriceListEntity> requestPriceList = RequestHelper.UnwrapPriceListString(priceListParams);
            if (requestPriceList == null) {return "Failed: Could not unparse price list params: " + priceListParams;}

            //submit new price list
            string submtStatus;
            bool successfulSubmit = SubmitHelper.SubmitPriceList(ref currentOutlet,
                                                requestPriceList,
                                                ref _assortmentList,
                                                out submtStatus);

            if (!successfulSubmit){return submtStatus;}

            //overwrite price list for the given outlet
            currentOutlet.AssortmentPriceList = requestPriceList.
                                                Select(p => new ItemPriceWrapper{
                                                    AssortmentId = p.EntityID,
                                                    Price = p.Price
                                                }).ToList();

            refreshJsonStringCache();
            return "Success: Price List successfully updated";
        }

        /// <summary>
        /// Sumbits new outlet and returns operation result
        /// </summary>
        /// <param name="paramJson">OutletEntity in form of JSON</param>
        /// <returns></returns>
        public string SubitNewOutlet(string paramJson){            
            lock (_lockObj) {
                string submitStatus = subitNewOutlet(paramJson);

                _logger.WriteLog(submitStatus);
                return submitStatus;
            }
        }

        private string subitNewOutlet(string paramJson) {
            string submitStatus;
            OutletEntity outlet = RequestHelper.DeserializeJson<OutletEntity>(paramJson);

            if (outlet == null) { return "Could not unparse new outlet string:" + paramJson; }

            bool submitSuccess = SubmitHelper.SubmitNewOutlet(ref outlet, out submitStatus);

            if (!submitSuccess) { return submitStatus; }

            _outletsList.Add(outlet);
            _commentsDictionary.Add(outlet.EntityID, new List<CommentEntity>());
            outlet.AssortmentPriceList = new List<ItemPriceWrapper>();
            refreshJsonStringCache();
            return submitStatus;
        }


        /// <summary>
        /// Submits new comment to the database and in-memory data and returns operation result
        /// </summary>
        /// <param name="outletID">ID of the outlet you want to comment</param>
        /// <param name="userName">User name</param>
        /// <param name="commentText">Text of the comment</param>
        /// <returns></returns>
        public string SubmitComment(int outletID, string userName, string commentText){
            lock (_lockObj){
                CommentEntity comment = new CommentEntity() {
                                                                UserName = userName,
                                                                DateTime = DateTime.Now.ToString("hh:mm on dd.MM.yyyy", CultureInfo.InvariantCulture),
                                                                CommentText = commentText,
                                                                CommentRating = 0
                                                            };
                string operationStatus;
                if (!_commentsDictionary.ContainsKey(outletID)){
                    operationStatus = "Could not find outletId:" + outletID.ToString();
                    _logger.WriteLog(operationStatus);
                    return operationStatus;
                }
                
                bool success = SubmitHelper.SubmitComment(outletID , comment, out operationStatus);
                
                if (!success){
                    _logger.WriteLog(operationStatus);
                    return operationStatus;
                }

                insertCommentConsideringRating(_commentsDictionary[outletID], comment);
                _logger.WriteLog("Comment was added for outletID- " + outletID.ToString() + " from user: " + userName + " with text :\n" + commentText);
                return ResponseBuilder.GenerateJsonFromComments(_commentsDictionary[outletID]);
            }
        }

        /// <summary>
        /// Tries to insert as close to the beginning as possible. If 1 and 2 comments are liked, inserts at 3-rd position
        /// </summary>
        /// <param name="list"></param>
        /// <param name="comment"></param>
        private void insertCommentConsideringRating(List<CommentEntity> list, CommentEntity comment){
            for (int i = 0; i < 2; i++){ 
                if (list.Count > i && list[i].CommentRating <= 0) {
                    list.Insert(i,comment);
                    return;
                }
            }
            if (list.Count > 2)
                { list.Insert(2, comment); }
            else
                { list.Add(comment); }
        }

        public string RateOutlet(int outletID, int newRating){
            OutletEntity outlet = findOutletById(outletID);

            string operationStatus;
            double newAverageRating;
            int newVoteCount;

            if (!SubmitHelper.RateOutlet(outlet, newRating, out operationStatus, out newAverageRating, out newVoteCount)){
                _logger.WriteLog(operationStatus);
                return operationStatus;
            };           

            //update existing in-memory data
            outlet.OutletRating = newAverageRating;
            outlet.Votes = newVoteCount;

            _logger.WriteLog("Outlet with Id - " + outletID.ToString()+ " was voted with rating :" + newRating.ToString() + " resulting in new rating: " + newAverageRating.ToString());

            refreshJsonStringCache();
            return ResponseBuilder.ReturnJsonFromEntitity(outlet);
        }

        public string VoteComment(int outletId, int commentId, string thumbs){
            string opResult = "Failed:";

            if (thumbs != "up" && thumbs != "down"){
                opResult += "Incorrect parameter name for Thumbs - it is: "+ thumbs;
                _logger.WriteLog(opResult);
                return opResult;
            }

            int voteDelta = thumbs == "up" ? 1 : -1;

            if (!_commentsDictionary.ContainsKey(outletId)){
                opResult += "Could not find outletId:" + outletId.ToString();
                _logger.WriteLog(opResult);
                return opResult;
            }

            List<CommentEntity> commentList = _commentsDictionary[outletId];
            CommentEntity comment = commentList.Find(c => c.EntityID == commentId);

            if (comment == default(CommentEntity)){
                opResult += "Could not find commentId:" + commentId.ToString();
                _logger.WriteLog(opResult);
                return opResult;
            }

            bool success = SubmitHelper.VoteComment(ref comment, voteDelta, out opResult);
            if (!success) {
                _logger.WriteLog(opResult);
                return opResult;
            }
            sortOutletCommentsByLikes(commentList);
            _logger.WriteLog(opResult);
            return comment.CommentRating.ToString();
            //return ResponseBuilder.GenerateJsonFromComments(commentList);
        }        

        private OutletEntity findOutletById(int outletID){
            return _outletsList
                   .Where(outlet => outlet.EntityID == outletID)
                   .FirstOrDefault();
        }        

        /// <summary>
        /// Performs sorting of outlet entities by name and meal entity by name
        /// </summary>
        private void sortMainEntitiesByName(){
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
            foreach (var commentList in _commentsDictionary){                
                sortOutletCommentsByLikes(commentList.Value);                
            }            
        }
#endregion        
    }
}
