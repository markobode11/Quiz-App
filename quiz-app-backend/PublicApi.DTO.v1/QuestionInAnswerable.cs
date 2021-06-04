namespace PublicApi.DTO.v1
{
    public class QuestionInAnswerable
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public PublicApi.DTO.v1.Question? Question { get; set; }
        
        public int AnswerableId { get; set; }

        public PublicApi.DTO.v1.Answerable? Answerable { get; set; }
    }
}