namespace PublicApi.DTO.v1
{
    public class AppUserAnswerable
    {
        public int Id { get; set; }

        public int AppUserId { get; set; }
        
        public int AnswerableId { get; set; }

        public Answerable? Answerable { get; set; }

        public int CorrectAnswers { get; set; }
        
        public int IncorrectAnswers { get; set; }
    }
    
    public class AppUserAnswerableCreate
    {
        public int AnswerableId { get; set; }
        
        public int CorrectAnswers { get; set; }
        
        public int IncorrectAnswers { get; set; }
    }
}