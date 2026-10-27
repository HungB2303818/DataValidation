using System;
using System.Collections.Generic;

namespace IdentityValidationApp.Views
{
    public interface IUserInfoView
    {
        int CitizenId { get; set; }
        string FullName { get; set; }
        string PersonalId { get; set; }
        string Email { get; set; }
        string PhoneNumber { get; set; }
        string StartDate { get; set; }

        void ShowResult(bool isValid, Dictionary<string, string> errors, string customMessage = "");
        void CloseView(); // Bổ sung phương thức đóng Form

        event EventHandler? ValidateRequested;
    }
}