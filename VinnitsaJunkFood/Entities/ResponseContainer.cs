﻿using JunkBackEnd.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VinnitsaJunkFood.Entities
{
    [DataContract]
    [KnownType(typeof(List<CommentEntity>))]
    [KnownType(typeof(OutletEntity))]    
    public class ResponseContainer{

        public ResponseContainer(bool success, object result){
            Success = success;
            Data = success ? result : null;
            ErrorMessage = success ? null : result as string;
        }

        public ResponseContainer(){}

        [DataMember]        
        public object Data { get; set; }

        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}