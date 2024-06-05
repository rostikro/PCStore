using System.ComponentModel.DataAnnotations;

namespace PCStore.ViewModels;

public class ForgotPasswordViewModel
{
    [Microsoft.Build.Framework.Required]
    [Display(Name = "Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
}