using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PageButton : AbstractButton
{
    public int maxPages;
    private int _currentPage;
    public int currentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            if ((_currentPage == 0 && direction == -1) || (_currentPage == maxPages && direction == 1))
                objectRef.gameObject.SetActive(false);
            else
                objectRef.gameObject.SetActive(true);
        }
    }
    public bool multiplePages = true;

    [SerializeField][Range(-1, 1)] protected int direction;
    [SerializeField] protected List<GameObject> pages;

    [SerializeField] protected TextMeshProUGUI title;
    [SerializeField] protected UnityEvent clickEvent;

    private PageButton other;
    private CancellationTokenSource tokenSource;

    public override void Init(GameObject obj)
    {
        obj.TryGetComponent(out manager);
        objectRef = obj;
        currentPage = 0;
        maxPages = multiplePages ? pages.Count - 1 : 0; 

        other = manager.linkedObjects[0].GetComponent<UIType>().getPageParams();
        tokenSource = new CancellationTokenSource();
    }
    public override void Function()
    {
        if (multiplePages) MultiplePages();
        else SinglePage();
    }

    public void setTitle(string title) => this.title.text = title;

    private void MultiplePages()
    {
        pages[currentPage].GetComponent<UIElement>().HideUi();

        currentPage += direction;
        other.currentPage = currentPage;

        pages[currentPage].GetComponent<UIElement>().ShowUi();
    }
    private void SinglePage()
    {
        clickEvent.Invoke();
    }


    public int getDirection() => direction;
    public async void Disable()
    {
        await Task.Delay(250, tokenSource.Token);
        if (tokenSource.IsCancellationRequested) objectRef.SetActive(false);
    }
    public async void Enable()
    {
        await Task.Delay(250, tokenSource.Token);
        if (tokenSource.IsCancellationRequested) objectRef.SetActive(true);
    }
}