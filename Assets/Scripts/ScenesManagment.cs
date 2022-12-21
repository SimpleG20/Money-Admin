using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

public class ScenesManagment : MonoBehaviour
{ 
    int currentScene;
    public int sceneTest;
    public GameObject leaveEditBt, makeChangeEditBt, addBt;
    [SerializeField] UIElement[] scenes;
    [SerializeField] UIType[] icons;
    [SerializeField] UIElement backIcons, limitPopUp;

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

        if(currentScene != 1)
        {
            DefaultScene();
        }
    }

    public int getScene() => currentScene;
    public UIElement getCurrentScene() => scenes[currentScene - 1];
    public UIElement[] getScenes() => scenes;
    public UIType[] getIcons() => icons;
    public async void ChangeScene(int index)
    {
        if (index == -1) { Application.Quit(); return; }
        if (index == currentScene) return;

        if (scenes[index - 1].hasAnimator()) UseAnimator(index);
        else ManualProcess(index);

        LeaveScene(index);
        currentScene = index;
        EnterScene(currentScene);

        if (currentScene != 1 && backIcons.getCanvasGroup().alpha != 1) 
        { 
            backIcons.ShowUi();
            await Task.Delay(500);
            limitPopUp.EnableElement(); }
        else if (currentScene == 1) 
        { 
            backIcons.HideUi(); 
            limitPopUp.DisableElement(); 
        }
    }

    #region Scenes Initiations
    private void EnterScene(int scene)
    {
        switch (scene)
        {
            case 1:
                print("Entry Scene");
                break;
            case 2:
                {
                    if (DespesaUI.current.edited)
                        DespesaUI.current.SairEdicao();

                    print("Main Scene");
                }
                break;
            case 3:
                print("Creation Scene");
                break;
            case 4:
                {
                    if (DespesaUI.current.edited)
                        DespesaUI.current.SairEdicao();

                    DespesaUI.current.InitReportScene();

                    print("Report Scene");
                }
                break;
        }
    }
    private void LeaveScene(int scene)
    {
        switch (scene)
        {
            case 1:
                break;
            case 2:
                break; 
            case 3:
                break;
            case 4:
                DespesaUI.current.LeaveReportScene();
                break;
        }
    }
    public void EditScene()
    {
        icons[1].ClickFunction();
        addBt.SetActive(false);
        makeChangeEditBt.SetActive(true);
        leaveEditBt.SetActive(true);
    }
    public void LeaveEditScene()
    {
        addBt.SetActive(true);
        makeChangeEditBt.SetActive(false);
        leaveEditBt.SetActive(false);
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
        DefaultScene();
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
