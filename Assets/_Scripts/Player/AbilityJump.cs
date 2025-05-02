using UnityEngine;
public class AbilityJump : MonoBehaviour , IAblity
{
    #region IAblity Implement
    bool IAblity.enabled
    {
        get => this.enabled;
        set => this.enabled = value;
    }
    float multiplier;
    void IAblity.MultiplierUpdate(float total)
    {
        multiplier = total;
    }
    #endregion

    




    void Start()
    {
        
    }

}
