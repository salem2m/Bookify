namespace Bokify.Web.Extensions
{
    public static class UserExtentions
    {
        public static string GetUserid(this ClaimsPrincipal user) =>
            user.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }
}
