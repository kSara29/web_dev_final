## Documentation

### Controllers
#### HomeController
###### Purpose: 
Initializes an instance of the HomeController.

##### Index (GET) Method
###### Purpose: 
Handles the GET request for the home page.
###### Details:
Retrieves the current user using _userManager.GetUserAsync(User). Fetches the list of users that the current user is following. Retrieves posts from users in the following list, including user details, comments, and likes. Loads user details for each comment in the fetched posts. Creates a HomeVm (Home View Model) to pass data to the view. Returns the view with the HomeVm.

##### Index (POST) Method
###### Purpose: 
Handles the POST request for the home page (for handling likes and comments).
###### Details: 
Retrieves the current user using _userManager.GetUserAsync(User). Creates a new Like object for the current user and the specified post. Adds a new comment to the post if vm.UserComment is not null. Checks if the current user has already liked the post, and either adds or removes the like accordingly. Refreshes the list of following users' posts and loads user details for comments. Creates a new HomeVm to pass data to the view. Returns the view with the updated HomeVm.

#### ValidationController
###### Purpose: 
Initializes an instance of the ValidationController.

##### CheckName
###### Purpose: 
Checks if a username is already taken.
###### Details: 
Queries the database to check if any user has the specified username (case-insensitive). Returns true if the username is available (not taken), and false otherwise.

##### EmailCheck
###### Purpose: 
Checks if an email address is already registered.
###### Details: 
Queries the database to check if any user has the specified email address (case-insensitive). Returns true if the email address is available (not registered), and false otherwise.

#### PostController
It contains methods for adding, editing, and deleting posts. Features include handling image uploads, user comments, and likes. The controller uses entity framework for database operations and integrates user authentication. This file is crucial for managing user-generated content and ensuring interactive user engagement on the platform.

### Services
#### EmailService
These methods encapsulate different scenarios for sending emails in the context of a web application, such as welcoming new users, notifying users of profile edits, responding to data requests, and confirming email addresses. Each method builds on the generic SendEmailAsync method, allowing for flexibility and reusability in handling various email-related tasks.

##### SendEmailAsync
###### Purpose: 
Sends a basic email with the specified content to the provided email address.
###### Details: 
Creates a MimeMessage object and sets the sender, recipient, subject, and body. Uses SmtpClient to connect to the SMTP server (Gmail in this case). Authenticates with the SMTP server using hardcoded credentials (consider using secure configuration). Sends the email message asynchronously. Disconnects from the SMTP server.

##### SendWelcomeEmailAsync
###### Purpose: 
Sends a welcome email to a new user.
###### Details: 
Calls SendEmailAsync with a predefined subject and formatted welcome message.

##### SendUserEditEmailAsync
###### Purpose: 
Sends a notification email for successful user profile editing.
###### Details: 
Calls SendEmailAsync with a predefined subject and a message containing the edited profile information.

##### SendUserDataEmailAsync
###### Purpose: 
Sends an email in response to a user's request for profile data.
###### Details: 
Calls SendEmailAsync with a predefined subject and a message containing the requested user data.

##### SendEmailConfirmationAsync
###### Purpose: 
Sends an email for email address confirmation with a provided confirmation link.
###### Details: 
Calls SendEmailAsync with a predefined subject and a message containing the confirmation link.

