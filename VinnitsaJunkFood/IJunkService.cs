using JunkBackEnd.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using VinnitsaJunkFood.Entities;
//using VinnitsaJunkFood.Entities.RequestEntities;

namespace VinnitsaJunkFood{
    [ServiceContract]    
    public interface IJunkService{

        [OperationContract]
        [WebInvoke(Method = "GET",
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare,
                    UriTemplate = "GetComments?Id={outletId}")]
        ResponseContainer GetComments(int outletId);
        
        [OperationContract]
        [WebInvoke(Method = "GET",
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare,
                    UriTemplate = "Initialize")]
        ResponseContainer Initialize();
        
        [OperationContract]
        [WebInvoke(Method = "POST",
                    ResponseFormat = WebMessageFormat.Json,
                    RequestFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.WrappedRequest,
                    UriTemplate = "AddNewComment")]
        ResponseContainer AddComment(CommentEntity requestData);

        [OperationContract]
        [WebInvoke(Method = "POST",
                    ResponseFormat = WebMessageFormat.Json,
                    RequestFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.WrappedRequest,
                    UriTemplate = "/SubmitMeal")]
        ResponseContainer SubmitMeal(AssortmentEntity requestData);

        [OperationContract]
        [WebInvoke(Method = "POST",
                    ResponseFormat = WebMessageFormat.Json,
                    RequestFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.WrappedRequest,
                    UriTemplate = "AddNewOutlet")]
        ResponseContainer AddOutlet(OutletEntity requestData);

        [OperationContract]
        [WebInvoke(Method = "POST",
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare,
                    UriTemplate = "VoteComment?CommentId={commentId}&Thumbs={thumbs}&OutletId={outletId}")]
        ResponseContainer VoteComment(int commentId, string thumbs, int outletId);

        [OperationContract]
        [WebInvoke(Method = "POST",
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare,
                    UriTemplate = "RateOutlet?OutletId={outletId}&Rating={rating}")]
        ResponseContainer RateOutlet(int outletId, int rating);



        [OperationContract]
        [WebInvoke(Method = "POST",
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.WrappedRequest,
                    UriTemplate = "UpdatePriceList")]
        ResponseContainer UpdatePriceList(BaseEntity requestData);

        [OperationContract]
        [WebInvoke(Method = "POST",
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.WrappedRequest,
                    UriTemplate = "SubmitFeedback")]
        ResponseContainer SubmitFeedback(FeedbackEntity requestData);

    }
}