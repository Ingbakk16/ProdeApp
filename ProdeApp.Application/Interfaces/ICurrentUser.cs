namespace ProdeApp.Application.Interfaces;

public interface ICurrentUser
{
    int? UserId { get; }
    bool IsAuthenticated { get; }
}
