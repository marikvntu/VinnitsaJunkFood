using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JunkBackEnd.BusinessLayer;
using JunkBackEnd.DataAccessLayer;
using JunkBackEnd.Entities;
using VinnitsaJunkFood.DataAccessLayer;
using VinnitsaJunkFood.Logger;

namespace VinnitsaJunkFood.BusinessLayer
{
    public class SiteCore
    {
        #region Private members

        private static readonly Lazy<SiteCore> _instance = new Lazy<SiteCore>(()=> new SiteCore());
        private List<OutletEntity> _outletsList;
        private List<AssortmentEntity> _assortmentList;
        private List<BaseEntity> _ingridientList;
        private List<GeoLocationEntity> _locationsList;

        // k - outletId, v - comments list        
        private Dictionary<int, List<CommentEntity>> _commentsDictionary;

        private string _initialJsonStringCache;
        private readonly FileLogger _logger;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance of SiteCore.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static SiteCore Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        #region Constructor


        /// <summary>
        /// Initializes a new instance of the <see cref="SiteCore"/> class.
        /// </summary>
        private SiteCore()
        {
            _outletsList = new List<OutletEntity>();
            _assortmentList = new List<AssortmentEntity>();
            _ingridientList = new List<BaseEntity>();
            _locationsList = new List<GeoLocationEntity>();
            _commentsDictionary = new Dictionary<int, List<CommentEntity>>();

            _logger = new FileLogger();
            _logger.WriteLog("Site Manager initialized, retrieve entities from DB");
            InitializeEntities();
            _logger.WriteLog("Entities initialized");
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns cached JSON with initial entities for first time request
        /// </summary>
        /// <returns></returns>
        public string ReturnInitialData()
        {
            _logger.WriteLog("Returning cache data...");
            return _initialJsonStringCache;
        }

        /// <summary>
        /// Returns JSON string with comments for particular outlet
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<CommentEntity> GetCommentsForOutlet(int id)
        {
            if (!_commentsDictionary.Keys.Contains(id)) return new List<CommentEntity>();

            _logger.WriteLog("Returning comments for: " + id);
            //return ResponseBuilder.GenerateJsonFromComments(_commentsDictionary[id]);
            return _commentsDictionary[id];
        }

        /// <summary>
        /// Submits the meal.
        /// </summary>
        /// <param name="meal">The meal.</param>
        /// <param name="submitStatus">The submit status.</param>
        /// <returns></returns>
        public bool SubmitMeal(AssortmentEntity meal, out string submitStatus)
        {
            var success = InputHelper.ValidateAndPrepareMealData(meal, ref _assortmentList, out submitStatus);
            if (!success)
            {
                _logger.WriteLog(submitStatus);
                return false;
            }

            success = SubmitHelper.SubmitMeal(ref meal, out submitStatus);
            if (!success)
            {
                _logger.WriteLog(submitStatus);
                return false;
            }

            //add to assortment list, in successful scenario
            _assortmentList.Add(meal);

            RefreshJsonStringCache();

            _logger.WriteLog(submitStatus);
            return true;
        }

        /// <summary>
        /// Updates the price list.
        /// </summary>
        /// <param name="outletId">The outlet identifier.</param>
        /// <param name="priceListParams">The price list parameters.</param>
        /// <param name="submitStatus">The submit status.</param>
        /// <returns></returns>
        public bool UpdatePriceList(int outletId, string priceListParams, out string submitStatus)
        {
            List<PriceListEntity> requestPriceList;
            bool success = InputHelper.ValidateAndPreparePriceListData(priceListParams,
                                                                        out submitStatus,
                                                                        out requestPriceList);

            if (!success)
            {
                _logger.WriteLog(submitStatus);
                return false;
            }

            //check if OutletID exists;
            OutletEntity currentOutlet = FindOutletById(outletId);

            if (currentOutlet == null)
            {
                submitStatus = "Failed: Could not find outlet with ID: " + outletId;
                _logger.WriteLog(submitStatus);
                return false;
            }

            //submit new price list            
            success = SubmitHelper.SubmitPriceList(ref currentOutlet,
                                                requestPriceList,
                                                ref _assortmentList,
                                                out submitStatus);

            if (!success)
            {
                _logger.WriteLog(submitStatus);
                return false;
            }

            //overwrite price list for the given outlet
            currentOutlet.AssortmentPriceList = requestPriceList.
                                                Select(p => new ItemPriceWrapper
                                                {
                                                    AssortmentId = p.EntityID,
                                                    Price = p.Price
                                                }).ToList();

            RefreshJsonStringCache();
            submitStatus = "Success: Price List successfully updated";
            return true;
        }

        /// <summary>
        /// Submits the new outlet.
        /// </summary>
        /// <param name="outlet">The outlet.</param>
        /// <param name="submitStatus">The submit status.</param>
        /// <returns></returns>
        public bool SubmitNewOutlet(OutletEntity outlet, out string submitStatus)
        {
            if (outlet == null)
            {
                submitStatus = "Outlet data was sent as null";
                _logger.WriteLog(submitStatus);
                return false;
            }

            bool success = SubmitHelper.SubmitNewOutlet(ref outlet, out submitStatus);

            if (!success)
            {
                _logger.WriteLog(submitStatus);
                return false;
            }

            _outletsList.Add(outlet);
            _commentsDictionary.Add(outlet.EntityID, new List<CommentEntity>());
            outlet.AssortmentPriceList = new List<ItemPriceWrapper>();
            RefreshJsonStringCache();

            _logger.WriteLog(submitStatus);
            return true;
        }

        public object SubmitComment(CommentEntity comment, out bool successful)
        {
            comment.DateTime = DateTime.Now.ToString("hh:mm on dd.MM.yyyy", CultureInfo.InvariantCulture);
            int outletId = comment.EntityID;

            string operationStatus;
            if (!_commentsDictionary.ContainsKey(outletId))
            {
                operationStatus = "Could not find outletId:" + outletId;
                successful = false;
                return operationStatus;
            }

            successful = SubmitHelper.SubmitComment(outletId, comment, out operationStatus);

            if (!successful) { return operationStatus; }

            InsertCommentConsideringRating(_commentsDictionary[outletId], comment);

            return _commentsDictionary[outletId];
        }

        /// <summary>
        /// Rates the outlet.
        /// </summary>
        /// <param name="outletId">The outlet identifier.</param>
        /// <param name="newRating">The new rating.</param>
        /// <param name="operationStatus">The operation status.</param>
        /// <param name="outlet">The outlet.</param>
        /// <returns></returns>
        public bool RateOutlet(int outletId, int newRating, out string operationStatus, out OutletEntity outlet)
        {
            double newAverageRating;
            int newVoteCount;

            outlet = FindOutletById(outletId);

            bool success = SubmitHelper.RateOutlet(outlet, newRating, out operationStatus, out newAverageRating, out newVoteCount);

            if (!success)
            {
                _logger.WriteLog("Could not add");
                return false;
            }

            //update existing in-memory data
            outlet.OutletRating = newAverageRating;
            outlet.Votes = newVoteCount;

            operationStatus = "Outlet with Id - " + outletId + " was voted with rating :" + newRating + " resulting in new rating: " + newAverageRating;

            RefreshJsonStringCache();
            _logger.WriteLog(operationStatus);
            return true;
        }

        /// <summary>
        /// Votes the comment.
        /// </summary>
        /// <param name="outletId">The outlet identifier.</param>
        /// <param name="commentId">The comment identifier.</param>
        /// <param name="thumbs">The thumbs.</param>
        /// <param name="opResult">The op result.</param>
        /// <returns></returns>
        public bool VoteComment(int outletId, int commentId, string thumbs, out string opResult)
        {
            opResult = "Failed:";

            if (thumbs != "up" && thumbs != "down")
            {
                opResult += "Incorrect parameter name for Thumbs - it is: " + thumbs;
                _logger.WriteLog(opResult);
                return false;
            }

            int voteDelta = thumbs == "up" ? 1 : -1;

            if (!_commentsDictionary.ContainsKey(outletId))
            {
                opResult += "Could not find outletId:" + outletId;
                _logger.WriteLog(opResult);
                return false;
            }

            List<CommentEntity> commentList = _commentsDictionary[outletId];
            CommentEntity comment = commentList.Find(c => c.EntityID == commentId);

            if (comment == default(CommentEntity))
            {
                opResult += "Could not find commentId:" + commentId;
                _logger.WriteLog(opResult);
                return false;
            }

            bool success = SubmitHelper.VoteComment(ref comment, voteDelta, out opResult);
            if (!success)
            {
                _logger.WriteLog(opResult);
                return false;
            }

            SortOutletCommentsByLikes(commentList);

            _logger.WriteLog(String.Format("Comment voted successfully, new rating is {0}", opResult));
            opResult = comment.CommentRating.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        #endregion

        #region Private methods

        private OutletEntity FindOutletById(int outletId)
        {
            return _outletsList.FirstOrDefault(outlet => outlet.EntityID == outletId);
        }

        /// <summary>
        /// Performs sorting of outlet entities by name and meal entity by name
        /// </summary>
        private void SortMainEntitiesByName()
        {
            _outletsList = _outletsList.
                            OrderBy(outlet => outlet.EntityName).
                            ToList();

            _assortmentList = _assortmentList.
                            OrderBy(outlet => outlet.EntityName).
                            ToList();

        }

        private void SortOutletCommentsByLikes(List<CommentEntity> comments)
        {
            List<CommentEntity> top2 = comments.OrderByDescending(c => c.CommentRating).Take(2).ToList();
            CheckAndSwapCommentsInList(top2, comments, 0);
            CheckAndSwapCommentsInList(top2, comments, 1);
        }

        private void CheckAndSwapCommentsInList(List<CommentEntity> topList, List<CommentEntity> fullList, int index)
        {
            if (fullList.Count <= index || topList.Count <= index) { return; }

            if (topList[index] != fullList[index])
            {
                fullList.Remove(topList[index]);
                fullList.Insert(index, topList[index]);
            }
        }

        private void SortCommentsDictionaryByLikes()
        {
            foreach (var commentList in _commentsDictionary)
            {
                SortOutletCommentsByLikes(commentList.Value);
            }
        }

        /// <summary>
        /// Tries to insert as close to the beginning as possible. If 1 and 2 comments are liked, inserts at 3-rd position
        /// </summary>
        /// <param name="list"></param>
        /// <param name="comment"></param>
        private void InsertCommentConsideringRating(List<CommentEntity> list, CommentEntity comment)
        {
            for (int i = 0; i < 2; i++)
            {
                if (list.Count > i && list[i].CommentRating <= 0)
                {
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
        private void RefreshJsonStringCache()
        {
            SortMainEntitiesByName();
            _logger.WriteLog("Refreshing Json string cache...");
            _initialJsonStringCache = JsonHelper.ReturnJsonFromEntities(_outletsList, _assortmentList);
        }

        private void InitializeEntities()
        {
            IDbReader dbReader = new DbReadHelper();
            dbReader.FillAllEntitiesFromDb(ref _outletsList,
                                           ref _assortmentList,
                                           ref _ingridientList,
                                           ref _locationsList,
                                           ref _commentsDictionary);

            RefreshJsonStringCache();
            SortCommentsDictionaryByLikes();
        }

        #endregion
    }
}
