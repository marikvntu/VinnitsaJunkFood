
using System.Runtime.Serialization;
namespace VinnitsaJunkFood.Entities
{
    [DataContract]
    public class FeedbackEntity
    {
        [DataMember]
        public string Subject { get; set; }

        [DataMember]
        public string ContactInfo { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}