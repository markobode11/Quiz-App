using System.ComponentModel.DataAnnotations;

namespace Domain.App
{
    public class Answer
    {
        public int Id { get; set; }

        [MaxLength(128), MinLength(1)]
        public string Text { get; set; } = default!;

        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }

        public Question? Question { get; set; }
    }
}