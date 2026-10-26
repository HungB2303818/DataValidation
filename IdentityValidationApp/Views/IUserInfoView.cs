using System;
using System.Collections.Generic;

namespace IdentityValidationApp.Views
{
    public interface IUserInfoView
    {
        string PersonalId { get; }
        string Email { get; }
        string PhoneNumber { get; }

        void ShowResult(bool isValid, Dictionary<string, string> errors, string customMessage = "");

        event EventHandler? ValidateRequested;
    }
}