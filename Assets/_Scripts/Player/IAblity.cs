public interface IAblity
{
    public bool enabled { get; set; }
    public void MultiplierUpdate(float total);
}
[System.Serializable]
public struct DisableAblity
{
    public string targetAbilityName;
    public string reason;
    public DisableAblity(string targetAbilityName, string reason)
    {
        this.targetAbilityName = targetAbilityName;
        this.reason = reason;
    }
}
[System.Serializable]
public struct MultiplyAblity
{
    public string targetAbilityName;
    public float multiplier;
    public string reason;
    public MultiplyAblity(string targetAbilityName, float multiplier, string reason)
    {
        this.targetAbilityName = targetAbilityName;
        this.multiplier = multiplier;
        this.reason = reason;
    }
}
