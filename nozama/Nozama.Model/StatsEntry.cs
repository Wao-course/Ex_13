namespace Nozama.Model;

public record StatsEntry
{
  public int StatsEntryId { get; set; }
  public List<Product> Products { get; set; }
  public string Term { get; set; } 
  public DateTimeOffset Timestamp { get; set; }
}