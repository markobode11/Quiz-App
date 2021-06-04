namespace Domain.App
{
    public class QuestionInAnswerable
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public Question? Question { get; set; }
        
        public int AnswerableId { get; set; }

        public Answerable? Answerable { get; set; }
    }
}