using System.Linq;

namespace FFXIVVenues.Api.Security
{
    public interface IAuthorizationCheck
    {
        bool CanNot(Operation op, ISecurityScoped entity = null);

        bool Can(Operation op, ISecurityScoped entity = null);
        
        IQueryable<T> Can<T>(Operation op, IQueryable<T> queryable) where T : ISecurityScoped;
    }
}