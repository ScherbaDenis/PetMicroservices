using Answer.Api.Protos;
using Answer.Application.Interfaces;
using Answer.Domain.Entities;
using Grpc.Core;

namespace Answer.Api.Services;

public class UserServiceImpl : UserService.UserServiceBase
{
    private readonly IRepository<User> _userRepository;

    public UserServiceImpl(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public override async Task<UserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid user ID format"));
        }

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        return new UserResponse
        {
            Id = user.Id.ToString(),
            Name = user.Name
        };
    }

    public override async Task<ListUsersResponse> ListUsers(ListUsersRequest request, ServerCallContext context)
    {
        var users = await _userRepository.GetAllAsync();
        var response = new ListUsersResponse();
        
        foreach (var user in users)
        {
            response.Users.Add(new UserResponse
            {
                Id = user.Id.ToString(),
                Name = user.Name
            });
        }

        return response;
    }

    public override async Task<UserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var user = new User
        {
            Name = request.Name
        };

        await _userRepository.AddAsync(user);

        return new UserResponse
        {
            Id = user.Id.ToString(),
            Name = user.Name
        };
    }

    public override async Task<UserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid user ID format"));
        }

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        user.Name = request.Name;
        await _userRepository.UpdateAsync(user);

        return new UserResponse
        {
            Id = user.Id.ToString(),
            Name = user.Name
        };
    }

    public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid user ID format"));
        }

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        await _userRepository.DeleteAsync(id);

        return new DeleteUserResponse
        {
            Success = true
        };
    }
}
