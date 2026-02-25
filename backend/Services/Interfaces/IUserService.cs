using ECommerce.DTOs;
using ECommerce.Models;

namespace ECommerce.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUser();
        Task<UserDto?> GetById(int id);
        Task<bool> ToggleBlock(int id);
        //Task<bool> DeleteUser(int id);



    }
}
