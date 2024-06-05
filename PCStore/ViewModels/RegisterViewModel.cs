using System.ComponentModel.DataAnnotations;

namespace PCStore.ViewModels;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    
    [Required]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Repeat password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}