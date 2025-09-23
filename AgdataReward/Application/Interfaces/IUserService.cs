using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        UserProfile AddUser(UserProfile user);
        UserProfile? GetUserById(int id);
        IEnumerable<UserProfile> GetAllUsers();
        UserProfile UpdateUser(UserProfile user);
    }
}
