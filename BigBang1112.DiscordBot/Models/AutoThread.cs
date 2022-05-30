namespace BigBang1112.DiscordBot.Models;

public record AutoThread(string Name, AutoThreadOptions Options)
{
    public AutoThread(string name) : this(name, new())
    {
        
    }
}
