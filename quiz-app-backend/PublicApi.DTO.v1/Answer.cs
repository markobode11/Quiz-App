using System.ComponentModel.DataAnnotations;
using Domain.App;

namespace PublicApi.DTO.v1
{
    public class Answer
    {
        public int Id { get; set; }

        [MaxLength(128), MinLength(1)]
        public string Text { get; set; } = default!;

        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }
    }
    
    public class AnswerCreate
    {
        [MaxLength(128), MinLength(1)]
        public string Text { get; set; } = default!;

        public bool IsCorrect { get; set; }
    }
}