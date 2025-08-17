namespace ProductReviewer.Domain.Constants;

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Reviewer = "Reviewer";

    public static readonly string[] AllRoles = ["Admin", "User", "Reviewer"];
}
