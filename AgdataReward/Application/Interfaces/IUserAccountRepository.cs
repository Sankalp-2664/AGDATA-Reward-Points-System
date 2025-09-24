using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IUserAccountRepository
{
    Task<UserAccount?> GetByUserIdAsync(Guid userId);
    Task UpdateAsync(UserAccount account);
}
