namespace BigBang1112.Models.Db;

public class AdminModel : DbModel
{
    public virtual AccountModel Account { get; set; } = default!;
    public int AccountId { get; set; }
}
