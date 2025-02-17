namespace Nozama.Model;

public record StatsEntry
{
  public int StatsEntryId { get; set; }
  public ICollection<Product>? Products { get; set; }
  public string? Term { get; set; } 
  public DateTimeOffset Timestamp { get; set; }
  public string? Data { get; set; } //this property for storing raw JSON data
}