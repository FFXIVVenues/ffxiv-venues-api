using System;
using System.Collections.Generic;
using System.Linq;

namespace FFXIVVenues.Api.Security.ServiceAuthentication
{
    public class AuthorizationCheck : IAuthorizationCheck
    {
        private readonly AuthorizationKey _key;

        public AuthorizationCheck(AuthorizationKey _key) =>
            this._key = _key;

        public bool CanNot(Operation op, ISecurityScoped entity = null) => !Can(op, entity);

        public bool Can(Operation op, ISecurityScoped entity = null)
        {
            if (op == Operation.Create && entity != null)
                throw new InvalidOperationException("Cannot authorise Create permission against an existing item.");
            
            if (op == Operation.Create)
                return _key.Create;
            
            if (entity == null)
                return false;
            
            if (op == Operation.Read && entity.Approved)
                return true;
            
            if (_key.Scope == "all" || (_key.Scope == "approved" && entity.Approved))
                return op switch
                {
                    Operation.Read => true,
                    Operation.Approve => _key.Approve,
                    Operation.Create => _key.Create,
                    Operation.Update => _key.Update,
                    Operation.Delete => _key.Delete,
                    _ => false
                };

            return entity.ScopeKey == _key.Key && op switch
            {
                Operation.Read => true,
                Operation.Approve => _key.Approve,
                Operation.Create => _key.Create,
                Operation.Update => _key.Update,
                Operation.Delete => _key.Delete,
                _ => false
            };
        }

        public IQueryable<T> Can<T>(Operation op, IQueryable<T> queryable) where T : ISecurityScoped
        {
            if (op == Operation.Create)
                throw new InvalidOperationException("Cannot items venues on Create permission.");
            
            var opAuthorised = op switch
            {
                Operation.Read => true,
                Operation.Approve => _key.Approve,
                Operation.Create => _key.Create,
                Operation.Update => _key.Update,
                Operation.Delete => _key.Delete,
                _ => false
            };

            if (!opAuthorised)
                return new List<T>().AsQueryable();

            if (_key.Scope == "all")
                return queryable;
            
            if (op == Operation.Read || _key.Scope == "approved")
                return queryable.Where(i => i.Approved || i.ScopeKey == _key.Key);
            
            return queryable.Where(i => i.ScopeKey == _key.Key);
        }
        
        
    }
    
    

}
