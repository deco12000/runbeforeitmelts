using UnityEngine;
public abstract class Ability : MonoBehaviour
{
    protected abstract bool Enabled { get; set; }
    public abstract void UpdateMultiplier(float total);
}


