using FFXIVVenues.Api.Persistence;

namespace FFXIVVenues.Api.Security
{
    public class AuthorizationCheck : IAuthorizationCheck
    {
        private readonly AuthorizationKey _key;

        public AuthorizationCheck(AuthorizationKey _key) =>
            this._key = _key;

        public bool CanNot(Operation op, IEntity entity = null) => !Can(op, entity);

        public bool Can(Operation op, IEntity entity = null)
        {
            if (_key.Scope == "all")
                return op switch
                {
                    Operation.Read => true,
                    Operation.ReadInternal => true,
                    Operation.Approve => _key.Approve,
                    Operation.Create => _key.Create,
                    Operation.Update => _key.Update,
                    Operation.Delete => _key.Delete,
                    _ => false
                };

            if (op == Operation.Create)
                return _key.Create;

            if (entity == null)
                return false;

            if (op == Operation.Read)
                return true;

            return entity.OwningKey == _key.Key && op switch
            {
                Operation.Read => true,
                Operation.ReadInternal => true,
                Operation.Approve => _key.Approve,
                Operation.Create => _key.Create,
                Operation.Update => _key.Update,
                Operation.Delete => _key.Delete,
                _ => false
            };
        }
    }

}
