using UnityEngine;
public interface IItem
{
    string Name { get; }
    void OnPickItem(EventData ed);

}
