using System;
using System.Collections.Generic;

namespace myapi_cs.Models
{
    public partial class UserAccess
    {
        public long AccessId { get; set; }
        public long EmployeeId { get; set; }
        public byte[]? AccessDate { get; set; }
        public string AuthToken { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string? Password { get; set; }

        public virtual Employee Employee { get; set; } = null!;
    }
}
