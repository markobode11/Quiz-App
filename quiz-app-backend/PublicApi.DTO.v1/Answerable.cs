using System.ComponentModel.DataAnnotations;
using EType = PublicApi.DTO.v1.Enums.EType;

namespace PublicApi.DTO.v1
{
    public class Answerable
    {
        public int Id { get; set; }

        [MaxLength(128), MinLength(1)]
        public string Name { get; set; } = default!;

        [MaxLength(256), MinLength(1)]
        public string Description { get; set; } = default!;

        public EType Type { get; set; }
    }
    
    public class AnswerableCreate
    {
        [MaxLength(128), MinLength(1)]
        public string Name { get; set; } = default!;

        [MaxLength(256), MinLength(1)]
        public string Description { get; set; } = default!;

        public EType Type { get; set; }
    }
}