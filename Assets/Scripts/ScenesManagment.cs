using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

public class ScenesManagment : MonoBehaviour
{
    //public int sceneTest;
    private int currentScene;
    [SerializeField] UIElement[] scenes;
    [SerializeField] UIElement backIcons;

    private static ScenesManagment instance;
    public static ScenesManagment Instance
    {
        get
        {
            if(instance== null) instance = FindObjectOfType<ScenesManagment>();
            return instance;
        }
        set { instance = value; }
    }

    private void Start()
    {
        currentScene = 1;
    }

    public int getScene() => currentScene;
    public UIElement getCurrentScene() => scenes[currentScene - 1];
    public UIElement[] getScenes() => scenes;

    public void ChangeScene(int index)
    {
        if (index == -1) { Application.Quit(); return; }
        if (index == currentScene) return;

        if (scenes[index - 1].hasAnimator()) UseAnimator(index);
        else ManualProcess(index);

        currentScene = index;
        if (currentScene != 1 && backIcons.getCanvasGroup().alpha != 1) backIcons.ShowUi();
        else if (currentScene == 1) backIcons.HideUi();
    }

    private void UseAnimator(int index)
    {
        scenes[index - 1].ShowUi();
        scenes[currentScene - 1].HideUi();
    }

    private void ManualProcess(int index)
    {
        scenes[index - 1].getCanvasGroup().ChangeAlpha(1);
        scenes[currentScene - 1].getCanvasGroup().ChangeAlpha(0);
    }

    [ContextMenu("Change Scene")]
    void RandomScene()
    {
        
    }

}
