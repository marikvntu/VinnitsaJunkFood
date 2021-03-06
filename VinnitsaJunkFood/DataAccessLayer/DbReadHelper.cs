﻿using System;
using System.Collections.Generic;
using System.Linq;
using JunkBackEnd.Entities;
using System.Globalization;
using VinnitsaJunkFood.DataAccessLayer;

namespace JunkBackEnd.DataAccessLayer{       
    
    public class DbReadHelper : IDbReader {
#region DataMembers
        private VinnitsaEntities _dbEntities;
        private string m_connectionString; // = "metadata=res://*/DataAccessLayer.VinnitsaDBModel.csdl|res://*/DataAccessLayer.VinnitsaDBModel.ssdl|res://*/DataAccessLayer.VinnitsaDBModel.msl;provider=System.Data.SqlClient;provider connection string=';data source=.\\SQLEXPRESS;initial catalog=JunkDB;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework'";        
#endregion

#region Contructor
        public DbReadHelper(){
            m_connectionString = VinnitsaJunkFood.DataAccessLayer.ConnectionHelper.GetConnectionString();
        }
#endregion

#region Methods
       
        /// <summary>
        /// Fills entities with actual values from DB
        /// </summary>
        /// <param name="outlets"></param>
        /// <param name="assortment"></param>
        /// <param name="ingridients"></param>
        public void FillAllEntitiesFromDb(ref List<OutletEntity> outlets,
                                          ref List<AssortmentEntity> assortment,
                                          ref List<BaseEntity> ingridients,
                                          ref List<GeoLocationEntity> locations,
                                          ref Dictionary<int, List<CommentEntity>> commentsDic){
            try{
                
                using (_dbEntities = new VinnitsaEntities(m_connectionString)){
                    //Fill outlets entities                    
                    var outletQuery = from ol in _dbEntities.tblOutlets
                                      select new OutletEntity() {
                                                                      EntityID = ol.iOutletId,
                                                                      EntityName = ol.nvcOutletName,
                                                                      Latitude = ol.fLatitude,
                                                                      Longitude = ol.fLongitude,
                                                                      ImageUrl = ol.nvcImagePath,
                                                                      OutletRating = (double?)ol.rRating ?? 0,
                                                                      Description = ol.nvcDescription,
                                                                      Votes = (int?) ol.iVotes ?? 0
                                                                };
                    
                    foreach (var item in outletQuery){                        
                        item.AssortmentPriceList = _dbEntities.tblPriceLists.
                            Where(pr => pr.iOutletId == item.EntityID).
                            Select(p => new ItemPriceWrapper()
                            {
                                AssortmentId = p.iAssortmentId,
                                Price = (double)p.fPrice
                            }).ToList();

                        
                        //Attach comment dictionary to each outlet
                        commentsDic.Add(item.EntityID, new List<CommentEntity>());
                        
                        var commentQuery = (from c in _dbEntities.tblComments
                                            where c.iOutletId == item.EntityID
                                            orderby c.iCommentId descending
                                            select new 
                                                        {
                                                            EntityID = c.iCommentId,
                                                            CommentText = c.nvcCommentText,
                                                            UserName = c.nvcUserName,
                                                            DateTime = c.dtCommentDateTime,
                                                            CommentRating = c.iCommentRating ?? 0
                                                        });

                        foreach (var comment in commentQuery){
                            var comm = new CommentEntity(){
                                                                EntityID = comment.EntityID,
                                                                CommentText = comment.CommentText,
                                                                UserName = comment.UserName,
                                                                DateTime = comment.DateTime.ToString("hh:mm on dd.MM.yyyy ", CultureInfo.InvariantCulture),
                                                                CommentRating = comment.CommentRating
                                                            };
                            commentsDic[item.EntityID].Add(comm);    
                        }

                        outlets.Add(item);
                        
                        //Fill simplified version of entities
                        locations.Add(new GeoLocationEntity() {
                                                                    Latitude = item.Latitude,
                                                                    Longitude = item.Longitude
                                                               });
                    }
                    
                    //Fill assortment entities
                    assortment = _dbEntities.tblAssortments.Select(a =>
                            new AssortmentEntity()
                                                    {
                                                        EntityID = a.iAssortmentId,
                                                        EntityName = a.nvcAssortmentName,
                                                        MealDescription = a.nvcDescription
                                                    }).ToList();

                    //Fill ingridient entities
                    ingridients = _dbEntities.tblIngridients.Select(i =>
                        new BaseEntity()
                                        {
                                            EntityID = i.iIngridientId,
                                            EntityName = i.nvcDescription
                                        }).ToList();
                }
            }
            catch (Exception ex){
                Console.WriteLine(ex.Message);
                throw new Exception("DBReadHelper failed to initialize DB with message: " + ex.Message); 
            }
        }
#endregion
    }
}
