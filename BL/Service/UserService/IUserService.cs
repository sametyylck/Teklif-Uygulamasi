using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.UserService
{
    public interface IUserService
    {
        List<int> GetId();
    }
}
