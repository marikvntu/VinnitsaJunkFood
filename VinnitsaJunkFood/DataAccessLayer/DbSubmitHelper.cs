﻿using System;
using System.Collections.Generic;
using System.Linq;
using JunkBackEnd.Entities;
using VinnitsaJunkFood.DataAccessLayer;

namespace JunkBackEnd.DataAccessLayer
{
    internal class DbSubmitHelper
    {
        #region DataMembers

        private static readonly DbSubmitHelper _instance = new DbSubmitHelper();

        private readonly string _connectionString =
            "metadata=res://*/DataAccessLayer.VinnitsaDBModel.csdl|res://*/DataAccessLayer.VinnitsaDBModel.ssdl|res://*/DataAccessLayer.VinnitsaDBModel.msl;provider=System.Data.SqlClient;provider connection string=';data source=.\\SQLEXPRESS;initial catalog=JunkDB;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework'";

        private VinnitsaEntities _dbEntities;

        #endregion

        #region Contructor

        private DbSubmitHelper()
        {
            _connectionString = ConnectionHelper.GetConnectionString();
        }

        #endregion

        #region Properties

        public static DbSubmitHelper Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Methods

        internal bool AddAssortmentEntity(string entityName, out int assortmentID, string description = null)
        {
            bool isSuccessful = false;
            var assortmentRow = new tblAssortments
            {
                nvcAssortmentName = entityName,
                nvcDescription = description
            };

            using (_dbEntities = new VinnitsaEntities(_connectionString))
            {
                _dbEntities.AddTotblAssortments(assortmentRow);
                _dbEntities.SaveChanges();

                //assign assortmentID for in-memory variable
                assortmentID = assortmentRow.iAssortmentId;
                isSuccessful = true;
            }

            return isSuccessful;
        }

        internal bool UpdatePriceList(int outletID, List<PriceListEntity> newPriceList)
        {
            bool isSuccessful = false;
            using (_dbEntities = new VinnitsaEntities(_connectionString))
            {
                //delete all rows for this outlet                                
                foreach (tblPriceLists tableRow in _dbEntities.tblPriceLists.Where(row => row.iOutletId == outletID))
                {
                    _dbEntities.tblPriceLists.DeleteObject(tableRow);
                }

                //add remaining rows if any
                foreach (PriceListEntity item in newPriceList)
                {
                    var tableRow = new tblPriceLists();
                    tableRow.fPrice = (decimal) item.Price;
                    tableRow.iAssortmentId = item.EntityID;
                    tableRow.iOutletId = outletID;
                    _dbEntities.tblPriceLists.AddObject(tableRow);
                }

                _dbEntities.SaveChanges();
                isSuccessful = true;
            }

            return isSuccessful;
        }

        internal bool AddOutletEntity(OutletEntity outlet, out int outletId)
        {
            bool isSuccessful = false;
            using (_dbEntities = new VinnitsaEntities(_connectionString))
            {
                var outletRow = new tblOutlets
                {
                    fLatitude = outlet.Latitude,
                    fLongitude = outlet.Longitude,
                    nvcOutletName = outlet.EntityName,
                    rRating = (float) outlet.OutletRating,
                    nvcDescription = outlet.Description,
                    nvcImagePath = outlet.ImageUrl,
                    iVotes = outlet.Votes
                };

                _dbEntities.tblOutlets.AddObject(outletRow);
                _dbEntities.SaveChanges();
                outletId = outletRow.iOutletId;
                isSuccessful = true;
            }

            return isSuccessful;
        }

        internal bool AddCommentEntity(int outletId, CommentEntity comment, out int commentId)
        {
            bool isSuccessful = false;
            commentId = 0;
            using (_dbEntities = new VinnitsaEntities(_connectionString))
            {
                var commentRow = new tblComments
                {
                    iOutletId = outletId,
                    nvcUserName = comment.UserName,
                    dtCommentDateTime = DateTime.Now,
                    nvcCommentText = comment.CommentText,
                    iCommentRating = comment.CommentRating
                };
                _dbEntities.tblComments.AddObject(commentRow);
                _dbEntities.SaveChanges();
                isSuccessful = true;
                commentId = commentRow.iCommentId;
            }
            return isSuccessful;
        }

        internal bool UpdateRating(int outletId, double recalculatedRating, int recalculatedVotes)
        {
            bool isSuccessful = false;
            using (_dbEntities = new VinnitsaEntities(_connectionString))
            {
                tblOutlets outletRow = _dbEntities.tblOutlets.FirstOrDefault(or => or.iOutletId == outletId);

                if (outletRow == null)
                {
                    return false;
                }

                outletRow.iVotes = recalculatedVotes;
                outletRow.rRating = (float) recalculatedRating;

                _dbEntities.SaveChanges();
                isSuccessful = true;
            }
            return isSuccessful;
        }

        internal bool VoteForComment(int commentId, int delta)
        {
            bool isSuccessful = false;
            using (_dbEntities = new VinnitsaEntities(_connectionString))
            {
                tblComments commentRow = _dbEntities.tblComments.FirstOrDefault(c => c.iCommentId == commentId);
                if (commentRow == null)
                {
                    return false;
                }

                commentRow.iCommentRating = commentRow.iCommentRating != null
                    ? commentRow.iCommentRating + delta
                    : delta;

                _dbEntities.SaveChanges();
                isSuccessful = true;
            }
            return isSuccessful;
        }

        #endregion
    }
}