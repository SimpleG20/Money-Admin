using UnityEngine;

public abstract class AbstractButton
{
    protected UIButton manager;
    protected GameObject objectRef;
    public void getManager(UIButton _manager)
    {
        manager = _manager;
    }

    public abstract void Init(GameObject obj = default);
    public abstract void Function();

    public GameObject getObject() => objectRef;
    public void setObject(GameObject obj) => objectRef = obj;
}