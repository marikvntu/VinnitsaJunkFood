namespace JunkBackEnd.Entities{
    public class CommentEntity: BaseEntity {        
        public string CommentText { get; set; }
        public string UserName { get; set; }
        public string DateTime { get; set; }
        public int CommentRating { get; set; }
    }
}
