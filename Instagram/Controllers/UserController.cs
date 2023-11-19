using Instagram.Models;
using Instagram.Persistence;
using Instagram.Services;
using Instagram.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Instagram.Controllers;

public class UserController: Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly EmailService _emailService;

    public UserController(AppDbContext db, 
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            EmailService emailService)
    {
        _db = db;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserRegisterVm model)
    {
        if (!ModelState.IsValid)
            return View();
        
        byte[] imageData = null;
        if (model.Avatar.Length > 0)
        {
            using (var binaryReader = new BinaryReader(model.Avatar.OpenReadStream()))
            {
                imageData = binaryReader.ReadBytes((int)model.Avatar.Length);
            }
        }

        User? user = new User(
            model.UserName,
            model.Email,
            imageData!,
            model.Password,
            model.Name,
            model.Description,
            model.Gender,
            model.PhoneNumber);

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);
            await _emailService.SendEmailAsync(user.Email, "Регистрация", "Спасибо за регистрацию!");
            return RedirectToAction("Profile", "User", new { userId = user.Id });
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return RedirectToAction("Index", "Home");
    }
    
    
    [HttpGet]
    public async Task<IActionResult> Profile(string userId)
    {
        var user = await _userManager.GetUserAsync(User);
        var usertarget = await _userManager.FindByIdAsync(userId);
        
        if (user == null)
            return NotFound();

        var userSource = String.Empty;
        var userTarget = String.Empty;

        if (user.Id == userId)
        {
            userSource = userId;
            userTarget = userId;
        }
        else
        {
            userSource = user.Id;
            userTarget = userId;
        }
        
        var posts = _db.Posts.Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        var followerCount = _db.Subscriptions.Where(u => u.TargetUserId == usertarget.Id).ToList().Count;
        var followingCout = _db.Subscriptions.Where(u => u.SubscriberId == usertarget.Id).ToList().Count;

        var subscription = _db.Subscriptions
            .FirstOrDefault(s => s.SubscriberId == user.Id && s.TargetUserId == usertarget.Id);

        var vm = new UserProfileVm
        {
            UserName = usertarget.UserName,
            Info = usertarget.Description,
            Avatar = Convert.ToBase64String(usertarget.Avatar),
            PostCount = posts.Count,
            FollowerCount = followerCount,
            FollowingCount = followingCout,
            Posts = posts,
            SourceId = userSource,
            TargetId = userTarget
        };

        if (subscription != null)
            vm.Subscribed = true;
        
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Profile(UserProfileVm vm, string sourceId, string targetId)
    {
        var subscription = new Subscription(targetId, sourceId);
        var user = await _userManager.FindByIdAsync(targetId);

        if (user is null)
            return NotFound();
        
        var subscriberCheck = _db.Subscriptions
            .FirstOrDefault(s => s.SubscriberId == sourceId && s.TargetUserId == targetId);

        bool check = false;

        if (subscriberCheck != null)
        {
            _db.Subscriptions.Remove(subscriberCheck);
            await _db.SaveChangesAsync();
            check = false;
        }
        else
        {
            await _db.Subscriptions.AddAsync(subscription);
            await _db.SaveChangesAsync();
            check = true;
        }
        
        var followerCount = _db.Subscriptions.Where(u => u.TargetUserId == targetId).ToList().Count;
        var followingCout = _db.Subscriptions.Where(u => u.SubscriberId == targetId).ToList().Count;

        var userVm = new UserProfileVm
        {
            UserName = user.UserName,
            Info = user.Description,
            Avatar = Convert.ToBase64String(user.Avatar),
            PostCount = _db.Posts.Where(p => p.UserId == user.Id)
                .Select(p => p.Image != null ? Convert.ToBase64String(p.Image) : null)
                .ToList().Count,
            FollowerCount = followerCount,
            FollowingCount = followingCout,
            Posts = _db.Posts.Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.CreatedAt)
                .ToList(),
            Subscribed = check,
            SourceId = sourceId,
            TargetId = targetId
        };
        
        
        return View(userVm);
    }
    
    
    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new UserLoginVm { ReturnUrl = returnUrl });
    }


    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginVm model)
    {
        User user = await _userManager.FindByEmailAsync(model.Email);
        SignInResult result = await _signInManager.PasswordSignInAsync(
            user,
            model.Password,
            model.RememberMe,
            false
        );

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Неправильный логин и (или) пароль");
    
    return View(model);
        
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOff()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "User");
    }
    
    [HttpGet]
    public IActionResult Search()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Search(string keyword)
    {
        var users = _db.Users
            .Where(u => u.UserName.Contains(keyword) || u.Email.Contains(keyword) || u.Name.Contains(keyword))
            .Select(u => new UserSearchResultVm
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Name = u.Name
            })
            .ToList();

        return View(users);
    }
}