using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PublicApi.DTO.v1.Enums;

namespace PublicApi.DTO.v1
{
    public class Question
    {
        public int Id { get; set; }

        public EType? Type { get; set; }

        [MaxLength(128), MinLength(1)]
        public string Text { get; set; } = default!;

        public ICollection<PublicApi.DTO.v1.Answer>? Answers { get; set; }
    }
    
    public class QuestionEdit
    {
        public int Id { get; set; }
        
        [MaxLength(128), MinLength(1)]
        public string Text { get; set; } = default!;
    }
    
    public class QuestionCreate
    {
        [MaxLength(128), MinLength(1)]
        public string Text { get; set; } = default!;
        public EType? Type { get; set; }
    }
}