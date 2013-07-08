using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JunkBackEnd.Entities;
using JunkBackEnd.DataAccessLayer;
using VinnitsaJunkFood.DataAccessLayer;

namespace JunkBackEnd.DataAccessLayer
{
    class DbSubmitHelper{
#region DataMembers
        private VinnitsaEntities m_dbEntities;
        private string m_connectionString = "metadata=res://*/DataAccessLayer.VinnitsaDBModel.csdl|res://*/DataAccessLayer.VinnitsaDBModel.ssdl|res://*/DataAccessLayer.VinnitsaDBModel.msl;provider=System.Data.SqlClient;provider connection string=';data source=.\\SQLEXPRESS;initial catalog=JunkDB;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework'";
        private static readonly DbSubmitHelper m_instance = new DbSubmitHelper();
#endregion

#region Contructor
        private DbSubmitHelper(){
            m_connectionString = VinnitsaJunkFood.DataAccessLayer.ConnectionHelper.GetConnectionString();
        }
#endregion

#region Properties
        public static DbSubmitHelper Instance{
            get{
                return m_instance;
            }
        }
#endregion

#region Methods
        internal bool AddAssortmentEntity(string entityName, out int assortmentID, string description = null)
        {
            bool isSuccessful = false;            
            var assortmentRow = new tblAssortments(){
                nvcAssortmentName = entityName,
                nvcDescription = description
            };

            using (m_dbEntities = new VinnitsaEntities(m_connectionString)){
                m_dbEntities.AddTotblAssortments(assortmentRow);
                m_dbEntities.SaveChanges();
                
                //assign assortmentID for in-memory variable
                assortmentID = assortmentRow.iAssortmentId;
                isSuccessful = true;
            }

            return isSuccessful;
        }

        internal bool UpdatePriceList(int outletID, List<PriceListEntity> newPriceList)
        {
            bool isSuccessful = false;
            using (m_dbEntities = new VinnitsaEntities(m_connectionString)){                
                //delete all rows for this outlet                                
                foreach (var tableRow in m_dbEntities.tblPriceLists.Where(row => row.iOutletId == outletID)){
                    m_dbEntities.tblPriceLists.DeleteObject(tableRow);                                            
                }
                
                //add remaining rows if any
                foreach (var item in newPriceList){
                    var tableRow = new tblPriceLists();
                    tableRow.fPrice = (decimal)item.Price;
                    tableRow.iAssortmentId = item.EntityID;
                    tableRow.iOutletId = outletID;
                    m_dbEntities.tblPriceLists.AddObject(tableRow);
                }

                m_dbEntities.SaveChanges();
                isSuccessful = true;
            }

            return isSuccessful;
        }

        internal bool AddOutletEntity(OutletEntity outlet, out int outletId){
            bool isSuccessful = false;
            using (m_dbEntities = new VinnitsaEntities(m_connectionString))
            {
                var outletRow = new tblOutlets() {
                                                    fLatitude = outlet.Latitude,
                                                    fLongitude = outlet.Longitude,
                                                    nvcOutletName = outlet.EntityName,
                                                    rRating = (float)outlet.OutletRating,
                                                    nvcDescription = outlet.Description,
                                                    nvcImagePath = outlet.ImageUrl,
                                                    iVotes = outlet.Votes
                                                 };
                
                m_dbEntities.tblOutlets.AddObject(outletRow);
                m_dbEntities.SaveChanges();
                outletId = outletRow.iOutletId;
                isSuccessful = true;
            }

            return isSuccessful;
        }

        internal bool AddCommentEntity(int outletId, CommentEntity comment, out int commentId){
            bool isSuccessful = false;
            commentId = 0;
            using (m_dbEntities = new VinnitsaEntities(m_connectionString)){
                var commentRow = new tblComments() {
                                                        iOutletId = outletId,
                                                        nvcUserName = comment.UserName,                                                        
                                                        dtCommentDateTime = DateTime.Now,
                                                        nvcCommentText = comment.CommentText,
                                                        iCommentRating = comment.CommentRating
                                                   };
                m_dbEntities.tblComments.AddObject(commentRow);
                m_dbEntities.SaveChanges();
                isSuccessful = true;
                commentId = commentRow.iCommentId;
            }
            return isSuccessful;
        }

        internal bool UpdateRating(int outletId, double recalculatedRating, int recalculatedVotes){
            bool isSuccessful = false;
            using (m_dbEntities = new VinnitsaEntities(m_connectionString)){
                var outletRow = m_dbEntities.tblOutlets.Where(or => or.iOutletId == outletId).FirstOrDefault();

                if (outletRow == null) { return false; }

                outletRow.iVotes = recalculatedVotes;
                outletRow.rRating = (float)recalculatedRating;
                
                m_dbEntities.SaveChanges();
                isSuccessful = true;
            }
            return isSuccessful;
        }

        internal bool VoteForComment(int commentId, int delta){
            bool isSuccessful = false;
            using (m_dbEntities = new VinnitsaEntities(m_connectionString)) {
                var commentRow = m_dbEntities.tblComments
                                    .Where(c => c.iCommentId == commentId)
                                    .FirstOrDefault();
                if (commentRow == null) {return false;}

                commentRow.iCommentRating = commentRow.iCommentRating != null ?
                                            commentRow.iCommentRating + delta :
                                            delta;                

                m_dbEntities.SaveChanges();
                isSuccessful = true;
            }            
            return isSuccessful;
        }

#endregion







        
    }
}
