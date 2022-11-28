using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

public class ScenesManagment : MonoBehaviour
{
    private delegate void scenesInit();
    private Dictionary<int, scenesInit> initiations;
    public int sceneTest;

    int currentScene;
    [SerializeField] UIElement[] scenes;
    [SerializeField] UIElement backIcons;

    public static ScenesManagment Instance
    {
        get
        {
            if(instance== null) instance = FindObjectOfType<ScenesManagment>();
            return instance;
        }
        set { instance = value; }
    }
    private static ScenesManagment instance;

    private void Awake()
    {
        initiations = new Dictionary<int, scenesInit>() { {1, Scene1 },{2, Scene2 },{3, Scene3 },{4, Scene4 } };

        if(currentScene != 1)
        {
            DefaultScene();
        }
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

        initiations[currentScene]();
    }

    #region Scenes Initiations
    private void Scene1()
    {
        
    }
    private void Scene2()
    {

    }
    private void Scene3()
    {

    }
    private void Scene4()
    {

    }
    #endregion

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

    #region ContextMenu
    [ContextMenu("Change Scene")]
    void RandomScene()
    {
        scenes[currentScene - 1].gameObject.SetActive(false);
        scenes[currentScene - 1].gameObject.GetComponent<CanvasGroup>().alpha = 0;

        scenes[sceneTest - 1].gameObject.SetActive(true);
        scenes[sceneTest - 1].gameObject.GetComponent<CanvasGroup>().alpha = 1;

        currentScene = sceneTest;
    }
    [ContextMenu("Default Scene")]
    void DefaultScene()
    {
        currentScene = 1;
        for(int i=0; i < scenes.Length; i++)
        {
            if (i != 0)
            {
                scenes[i].gameObject.SetActive(false);
                scenes[i].gameObject.GetComponent<CanvasGroup>().alpha = 0;
            }
            else
            {
                scenes[i].gameObject.SetActive(true);
                scenes[i].gameObject.GetComponent<CanvasGroup>().alpha = 1;
            }
        }
    }
    #endregion
}
