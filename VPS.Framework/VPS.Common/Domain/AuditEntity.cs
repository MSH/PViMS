using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VPS.Common.Services;

namespace VPS.Common.Domain
{
    public class AuditedEntity<TKey, TUser> : Entity<TKey> where TKey : struct
    {
        public void AuditStamp(TUser user)
        {
            var now = DateService.Current.Now;

            if (this.IsTransient())
            {
                CreatedBy = user;
                Created = now;
                LastUpdated = null;
            }
            else
            {
                LastUpdated = now;
                UpdatedBy = user;
            }
        }

        public DateTime Created { get; protected set; }
        public DateTime? LastUpdated { get; protected set; }
        public virtual TUser UpdatedBy { get; protected set; }
        public virtual TUser CreatedBy { get; protected set; }
    }
}
