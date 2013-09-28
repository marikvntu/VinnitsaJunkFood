using JunkBackEnd.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VinnitsaJunkFood
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "JunkService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select JunkService.svc or JunkService.svc.cs at the Solution Explorer and start debugging.

    
    public class JunkService : IJunkService
    {
        
        public string DoWork()
        {
            //return new CommentEntity() {
            //     CommentText = "idi v pizdu"
            //};

            return "ololo";
        }
    }
}
