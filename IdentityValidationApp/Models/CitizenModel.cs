using IdentityValidationApp.Attributes;

namespace IdentityValidationApp.Models
{
    public class CitizenModel
    {
        [PersonalId]
        public string PersonalId { get; set; } = string.Empty;

        [CustomEmail]
        public string Email { get; set; } = string.Empty;

        [VietnamPhone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}