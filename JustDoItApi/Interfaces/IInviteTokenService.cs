namespace JustDoItApi.Interfaces;

public interface IInviteTokenService
{
    string Generate(long chatId);
    long Validate(string token);
}
