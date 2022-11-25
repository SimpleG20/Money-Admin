using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class CutoutMaskOffset : MonoBehaviour
{
    [SerializeField] Transform hole, shadow;
    [SerializeField] RectTransform back;
    [SerializeField] Image holeSource, shadowSource;

    Vector2 holeSizeV2;
    Vector3 shadowPosInit;

    [ContextMenu("back Size")]
    void BackSize() => print(back.rect.height);

    private void Awake()
    {
        shadowSource.rectTransform.sizeDelta = new Vector2(back.rect.height, back.rect.width * 3.5f);
        shadowPosInit = shadow.position;
        transform.parent.gameObject.SetActive(false);
    }

    public void Enable(float width, float height, Vector3 pos)
    {
        holeSizeV2 = new Vector2(width, height);
        hole.position = pos;
        holeSource.rectTransform.sizeDelta = holeSizeV2;

        shadow.position = shadowPosInit;
        transform.parent.gameObject.SetActive(true);
    }
}
