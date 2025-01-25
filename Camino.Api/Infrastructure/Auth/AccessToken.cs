namespace Camino.Api.Infrastructure.Auth
{
    public class AccessToken
    {
        public long Id { get; }
        public string Token { get; }
        public int ExpiresIn { get; }

        public AccessToken(long id, string token, int expiresIn)
        {
            Id = id;
            Token = token;
            ExpiresIn = expiresIn;
        }
    }
}
