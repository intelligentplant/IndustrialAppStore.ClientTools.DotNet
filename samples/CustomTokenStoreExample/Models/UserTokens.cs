using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExampleMvcApplication.Models {
    public class UserTokens {

        [Required]
        [Column(Order = 0)]
        public string UserId { get; set; }

        [Required]
        [Column(Order = 1)]
        public string SessionId { get; set; }

        [Required]
        public string TokenType { get; set; }

        [Required]
        public string AccessToken { get; set; }

        public DateTimeOffset? ExpiryTime { get; set; }

        public string RefreshToken { get; set; }

    }
}
