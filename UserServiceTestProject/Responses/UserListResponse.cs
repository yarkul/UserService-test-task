using UserServiceTestProject.DbContexts.DbModels;

namespace UserServiceTestProject.Responses
{
    public class UserListResponse: BaseResponse
    {
        public List<User> Users { get; set; }
    }
}
