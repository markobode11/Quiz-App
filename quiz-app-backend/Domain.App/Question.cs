using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.App.Enums;

namespace Domain.App
{
    public class Question
    {
        public int Id { get; set; }

        [MaxLength(128), MinLength(1)]
        public string Text { get; set; } = default!;

        public EType? Type { get; set; }

        public ICollection<Answer>? Answers { get; set; }
        public ICollection<QuestionInAnswerable>? QuestionInAnswerables { get; set; }
    }
}