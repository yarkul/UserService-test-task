using Microsoft.EntityFrameworkCore;
using UserServiceTestProject.DbContexts;
using UserServiceTestProject.DbContexts.DbModels;
using UserServiceTestProject.Responses;

public interface IUserService
{
    Task<UserCreatedResponse> CreateUserAsync(UserCreateRequestDto request);
    Task<UserListResponse> GetAllUsersAsync();
    Task UpdateUserRoleAsync(int userId, string newRole);
}

public class UserService : IUserService
{
    private readonly UserDbContext _dbContext;

    public UserService(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserCreatedResponse> CreateUserAsync(UserCreateRequestDto request)
    {
        var response = new UserCreatedResponse
        {
            Success = true,
            ErrorMessages = new List<string>()
        };

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            response.Success = false;
            response.ErrorMessages.Add("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            response.Success = false;
            response.ErrorMessages.Add("Email is required.");
        }

        var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(request.Email))
        {
            response.Success = false;
            response.ErrorMessages.Add("Invalid email format.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            response.Success = false;
            response.ErrorMessages.Add("Password is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Role))
        {
            response.Success = false;
            response.ErrorMessages.Add("Role is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Role)
            && (!request.Role.Equals("Admin") && !request.Role.Equals("User")))
        {
            response.Success = false;
            response.ErrorMessages.Add("Role must be either 'Admin' or 'User'.");
        }

        if (!response.Success)
        {
            return response;
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = request.Role
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        response.Id = user.Id;

        return response;
    }

    public async Task<UserListResponse> GetAllUsersAsync()
    {
        var dbUsers = await _dbContext.Users.AsNoTracking().ToListAsync();

        var result = new UserListResponse()
        {
            Users = dbUsers
        };

        return result;
    }

    public async Task UpdateUserRoleAsync(int userId, string newRole)
    {
        if (userId <= 0)
        {
            throw new Exception("Invalid user id");
        }

        if (string.IsNullOrWhiteSpace(newRole)
            || (!newRole.Equals("Admin") && !newRole.Equals("User")))
        {
            throw new Exception("Invalid role");
        }

        var user = await _dbContext.Users.FindAsync(userId);

        if (user is null)
        {
            throw new Exception("User not found");
        }
        user.Role = newRole;
        await _dbContext.SaveChangesAsync();
    }
}