using System.Runtime.Serialization;
namespace JunkBackEnd.Entities
{
    [DataContract]
    public class BaseEntity{
        [DataMember]
        public int EntityID { get; set; }

        [DataMember]
        public string EntityName { get; set; }
    }
}