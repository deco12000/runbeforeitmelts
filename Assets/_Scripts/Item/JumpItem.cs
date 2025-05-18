using UnityEngine;

public class JumpItem : Item
{
    public bool _isUsed;
    protected override bool isUsed => _isUsed;
    public int _probablity = 7;
    protected override int probablity => _probablity;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Player.I.isDead) return;
            if (_isUsed) return;
            _isUsed = true;
            EventHub.I.Invoke<EventGetItem>(new GetItemData("JumpItem"));
            base.DeSpawn();
        }
    }
}
