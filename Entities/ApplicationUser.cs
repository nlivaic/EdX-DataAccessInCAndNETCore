using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Entities
{
    [Table("ApplicationUser")]
    public class ApplicationUser {
        [Key]
        public int ApplicationUserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int InvalidLoginAttempts { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}