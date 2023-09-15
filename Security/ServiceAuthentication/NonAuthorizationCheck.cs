using System.Collections.Generic;
using System.Linq;

namespace FFXIVVenues.Api.Security.ServiceAuthentication
{
    public class NonAuthorizationCheck : IAuthorizationCheck
    {

        public bool CanNot(Operation op, ISecurityScoped _ = null) => !Can(op, _);

        public bool Can(Operation op, ISecurityScoped entity = null) => 
            op == Operation.Read && entity?.Approved == true;
        
        public IQueryable<T> Can<T>(Operation op, IQueryable<T> queryable) where T : ISecurityScoped =>
            op == Operation.Read ? queryable.Where(i => i.Approved) : new List<T>().AsQueryable();

    }
}
