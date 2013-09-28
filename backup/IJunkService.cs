using JunkBackEnd.Entities;
using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace VinnitsaJunkFood
{
    
    [ServiceContract]
    public interface IJunkService
    {
        [OperationContract]

        [WebInvoke (Method = "GET",
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare,
                    UriTemplate = "DoWork")]
        string DoWork();
    }
}
