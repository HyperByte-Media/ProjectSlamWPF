namespace ProjectSlam.Data.Models;

public class GlobalMoveset
{
    public int MoveId { get; set; }
    public string MoveCategory { get; set; } = string.Empty;
    public string MoveName { get; set; } = string.Empty;
    public int DamageAmount { get; set; }
    public bool IsFinisher { get; set; }
    public bool IsSignature { get; set; }
}
