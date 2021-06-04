using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.App.Enums;

namespace Domain.App
{
    public class Answerable
    {
        public int Id { get; set; }

        [MaxLength(128), MinLength(1)]
        public string Name { get; set; } = default!;

        [MaxLength(256), MinLength(1)]
        public string Description { get; set; } = default!;

        public EType Type { get; set; }

        public ICollection<QuestionInAnswerable>? Questions { get; set; }
        
        public ICollection<AppUserAnswerable>? AppUserAnswerables { get; set; }
    }
}