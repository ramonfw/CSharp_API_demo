using System;
using System.Collections.Generic;

namespace myapi_cs.Models
{
    public partial class VEmployeeAccess
    {
        public long? EmployeeId { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Title { get; set; }
        public long? ReportsTo { get; set; }
        public byte[]? BirthDate { get; set; }
        public byte[]? HireDate { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? Email { get; set; }
        public byte[]? AccessDate { get; set; }
        public string? AuthToken { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? Password { get; set; }
    }
}
