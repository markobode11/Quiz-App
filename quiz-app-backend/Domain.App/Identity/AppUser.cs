using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

#nullable enable

namespace Domain.App.Identity
{
    public class AppUser : IdentityUser<int>
    {
        [MaxLength(64)] [MinLength(2)]
        public string Firstname { get; set; } = default!;

        [MaxLength(64)] [MinLength(2)]
        public string Lastname { get; set; } = default!;

        [DisplayName("User Since")] public DateTime UserSince { get; set; } = DateTime.Now;

        public ICollection<AppUserAnswerable>? UserAnswerables { get; set; }
    }
}