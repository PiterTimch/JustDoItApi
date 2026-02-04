namespace JustDoItApi.Models.Chat;

public class ChatEditModel
{
    public string? Name { get; set; }
    public long ChatTypeId { get; set; }
    public List<long> UserIds { get; set; } = [];
}
