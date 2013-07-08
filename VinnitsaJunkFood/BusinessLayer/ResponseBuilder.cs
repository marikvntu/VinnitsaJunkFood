using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using JunkBackEnd.Entities;
using System.Runtime.Serialization.Json;
using System.IO;

namespace JunkBackEnd.BusinessLayer
{
    public static class ResponseBuilder{
        public static string ReturnJsonFromEntities(List<BaseEntity> entities){
            return  entities!=null ? 
                    GenerateJsonFromObject(entities): 
                    null;
        }

        public static string ReturnJsonFromEntities(List<OutletEntity> outlets, List<AssortmentEntity> assortment){
            if ((outlets == null) || (assortment == null)) { return null; }
            
            var wrapper = new ResponseWrapperEntity(){
                AssortmentList = assortment,
                OutletList = outlets
            };            
                        
            return GenerateJsonFromObject(wrapper);
        }

        public static string GenerateJsonFromGeolocations(List<GeoLocationEntity> locations){
            if (locations == null) { return null; }

            return GenerateJsonFromObject(locations);
        }

        public static string GenerateJsonFromComments(List<CommentEntity> comments){
            if (comments == null) { return null; }

            return GenerateJsonFromObject(comments);
        }       
        
        private static string GenerateJsonFromObject(object entities){
            string jsonString;
            try{
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(entities.GetType());
                MemoryStream memoryStream = new MemoryStream();
                serializer.WriteObject(memoryStream, entities);
                jsonString = Encoding.UTF8.GetString(memoryStream.ToArray());
                memoryStream.Close();
            }
            catch (Exception ex){
                throw ex;
            }

            return jsonString;
        }
    
        internal static string ReturnJsonFromEntitity(BaseEntity entity){
            if (entity == null) { return null; }

            return GenerateJsonFromObject(entity);
        }

    }
}
