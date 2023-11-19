using Instagram.Models;

namespace Instagram.ViewModels.User;

public class UserProfileVm
{
    public string SourceId { get; set; }
    public Models.User User { get; set; }
    public string TargetId { get; set; }
    public string UserName { get; set; }
    public string Info { get; set; }
    public int PostCount { get; set; }
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public string Avatar { get; set; }
    public bool Subscribed { get; set; }
    public List<Models.Post> Posts { get; set; } = new();
}