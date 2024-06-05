using System.ComponentModel.DataAnnotations;

namespace PCStore.ViewModels;

public class LoginViewModel
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
    public bool SaveCookies { get; set; }
    
    /*public string Email { get; set; }
    
    public string Password { get; set; }*/
}