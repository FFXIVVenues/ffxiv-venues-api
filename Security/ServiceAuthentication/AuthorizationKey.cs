#pragma warning disable CS8618

namespace FFXIVVenues.Api.Security.ServiceAuthentication
{
    public class AuthorizationKey
    {

        public string Key { get; set; }
        public string Scope { get; set; }
        public bool Create { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool ReadUnapproved { get; set; }
        public bool Approve { get; set; }

    }
}