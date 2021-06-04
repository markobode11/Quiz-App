using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#nullable enable

namespace PublicApi.DTO.v1.Identity
{
    public class AppUser
    {
        public int Id { get; set; }
        
        [MaxLength(64)] [MinLength(3)]
        public string Firstname { get; set; } = default!;
        
        public string Email { get; set; } = default!;

        [MaxLength(64)] [MinLength(3)]
        public string Lastname { get; set; } = default!;

        [DisplayName("User Since")] public DateTime UserSince { get; set; } = DateTime.Now;
        
    }
}