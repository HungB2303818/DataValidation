using IdentityValidationApp.Attributes;

namespace IdentityValidationApp.Models
{
    public class CitizenModel
    {
        public int Id { get; set; }

        [FullName]
        public string FullName { get; set; } = string.Empty;

        [PersonalId]
        public string PersonalId { get; set; } = string.Empty;

        [CustomEmail]
        public string Email { get; set; } = string.Empty;

        [VietnamPhone]
        public string PhoneNumber { get; set; } = string.Empty;

        [StartDate]
        public string StartDate { get; set; } = string.Empty;
    }
}