using Instagram.Enums;

namespace Instagram.ViewModels.User;

public class UserEditVm
{
    public string Name { get; set; }
    public string UserName { get; set; }
    public IFormFile  Avatar { get; set; }
    public string Description { get; set; }
    public string? PhoneNumber { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public bool ChangePassword { get; set; }
}