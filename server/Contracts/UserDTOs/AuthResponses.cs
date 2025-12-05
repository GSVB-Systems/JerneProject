namespace Contracts.UserDTOs;

public class AuthResponses
{
    public record LoginResponse( /* string Jwt */
    );

    public record AuthUserInfo(string Id, string UserName, string Role);
}