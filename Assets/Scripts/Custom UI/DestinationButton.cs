using System;
using UnityEngine;

[Serializable]
public class DestinationButton: AbstractButton
{
    [SerializeField] protected ScenesManagment managment;
    [SerializeField] protected int target;
    public override void Function()
    {
        managment.ChangeScene(target);
    }

    public override void Init(GameObject obj = default)
    {
        objectRef = obj;

        if (obj != null) obj.TryGetComponent<UIButton>(out manager);
    }
}