using UnityEngine;
public class SkillItem : Item
{
    public bool isSugar;
    public bool _isUsed;
    protected override bool isUsed => _isUsed;
    public int _probablity = 7;
    public override int probablity => _probablity;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Player.I.isDead) return;
            if (_isUsed) return;
            _isUsed = true;
            EventHub.I.Invoke<EventGetItem>(new GetItemData("SkillItem"));
            base.DeSpawn();
        }
    }
}
