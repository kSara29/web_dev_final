namespace Instagram.ViewModels.Home;

public class HomeVm
{
    public bool Liked = false;
    public string CurrentUserId { get; set; }
    public string UserComment { get; set; }
    public string SourceId { get; set; }
    public string TargetId { get; set; }
    public List<Models.Post> Posts { get; set; } = new();
}