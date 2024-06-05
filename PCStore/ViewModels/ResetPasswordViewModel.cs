using System.ComponentModel.DataAnnotations;

namespace PCStore.ViewModels;

public class ResetPasswordViewModel
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    
    [Required]
    public string Code { get; set; }
    
    [Required]
    [Display(Name = "New Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Repeat password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}