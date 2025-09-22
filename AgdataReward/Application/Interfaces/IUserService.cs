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
        User AddUser(User user);
        User? GetUserById(int id);
        IEnumerable<User> GetAllUsers();
        User UpdateUser(User user);
    }
}
