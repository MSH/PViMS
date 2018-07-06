using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using PVIMS.Core.Entities;

namespace PVIMS.Core
{
    public abstract class UserContext
    {
        private static UserContext _userContext;

        public static User CurrentUser
        {
            get { return Current.User; }
        }

        public abstract User User { get; }

        public static UserContext Current
        {
            get
            {
                if (_userContext == null)
                {
                    _userContext = new DefaultUserContext();
                }

                return _userContext;
            }
        }

        public static void SetUpUserContext(UserContext context)
        {
            _userContext = context;
        }

        public abstract bool IsInRole(string role);
    }


}
