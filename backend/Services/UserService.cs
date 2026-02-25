using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ECommerce.DTOs;
using System.Net.NetworkInformation;
using System.Collections.Generic;
namespace ECommerce.Services
{
    public class UserService:IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
            
        }

        public async Task<List<UserDto>> GetAllUser()
        {
            return await _context.Users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Phone = u.Phone,
                RoleId = u.RoleId,
                IsBlocked = u.IsBlocked


            }).ToListAsync();

        }
        public async Task<UserDto> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            return new UserDto
            {

                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                RoleId = user.RoleId,
                IsBlocked = user.IsBlocked
            };
        }

        public async Task<bool> ToggleBlock(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            var userIdString = user.Id.ToString();

            var hasActiveOrders = await _context.Orders
                .AnyAsync(o =>
                    o.UserId == userIdString &&
                    o.Status != OrderStatus.Pending &&
                    o.Status != OrderStatus.Cancelled &&
                    o.Status != OrderStatus.Delivered);

            if (hasActiveOrders && !user.IsBlocked)
                throw new ApplicationException(
                    "User cannot be blocked while orders are in processing stage");

            user.IsBlocked = !user.IsBlocked;
            await _context.SaveChangesAsync();

            return true;
        }


        //public async Task<bool> DeleteUser(int id)
        //{
        //    var user = await _context.Users.FindAsync(id);
        //    if (user == null)
        //        return false;

        //    _context.Users.Remove(user);
        //    await _context.SaveChangesAsync();

        //    return true;
        //}

    }
}
