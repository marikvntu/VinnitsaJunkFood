using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JunkBackEnd.Entities;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Globalization;

namespace JunkBackEnd.BusinessLayer
{
    static class RequestHelper{

        public static T DeserializeJson<T>(string json){
            T obj = Activator.CreateInstance<T>();

            try{
                MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
            }
            catch (Exception){
                obj = default(T);
            }
            
            return obj;
        }

        /// <summary>
        /// Used to decode the string from UpdatePriceList request and return List<PriceListEntity>
        /// </summary>
        /// <param name="priceListParams">string from the request</param>
        /// <returns>List<PriceListEntity></returns>
        internal static List<PriceListEntity> UnwrapPriceListString(string priceListParams){
            var priceList = new List<PriceListEntity>();
            //in case all entities cleared
            if (priceListParams.Length == 0) { return priceList; }
            
            float priceValue;            
            try {                
                // "|" is a separator between price list rows, 
                // '÷' is a separator between assortment name and price 
                foreach (var priceRow in priceListParams.Split('|')){
                    var priceRowArray = priceRow.Split('÷');
                    //replace comma with dot for easier parsing
                    priceValue = float.Parse(priceRowArray[1].Replace(',','.'), CultureInfo.InvariantCulture);
                    var priceItem = new PriceListEntity(){
                                                                EntityName = priceRowArray[0],
                                                                Price = priceValue
                                                         };
                    priceList.Add(priceItem);
                }
	        }
	        catch (Exception){
		        priceList = null;
	        }
            return priceList;
        }
    }
}
