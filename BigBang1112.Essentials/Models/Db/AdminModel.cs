namespace BigBang1112.Models.Db;

public class AdminModel
{
    public int Id { get; set; }

    public virtual AccountModel Account { get; set; } = default!;
    public int AccountId { get; set; }
}
