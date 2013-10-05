using System.Runtime.Serialization;
namespace JunkBackEnd.Entities{

    [DataContract]
    public class CommentEntity: BaseEntity {
        [DataMember]
        public string CommentText { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string DateTime { get; set; }

        [DataMember]
        public int CommentRating { get; set; }
    }
}
