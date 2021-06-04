using Domain.App.Identity;

namespace Domain.App
{
    public class AppUserAnswerable
    {
        public int Id { get; set; }

        public int AppUserId { get; set; }

        public AppUser? AppUser { get; set; }
        
        public int AnswerableId { get; set; }

        public Answerable? Answerable { get; set; }

        public int CorrectAnswers { get; set; }
        
        public int IncorrectAnswers { get; set; }
    }
}