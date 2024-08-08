using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.WSA;
using static Define;

public class UIManager
{
    private int _popupOrder = 100;
    private int _toastOrder = 500;

    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    private UI_Scene _sceneUI = null;

    //Toast
    public bool isToastLoaded = false;
    private static ToastUI _toastUI;

    public UI_Scene SceneUI
    {
        set => _sceneUI = value;
        get => _sceneUI;
    }

    private Dictionary<string, UI_Popup> _popups = new Dictionary<string, UI_Popup>();

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    public void CacheAllPopups()
    {
        var list = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(UI_Popup)));

        foreach (Type type in list)
        {
            CachePopupUI(type);
        }

        CloseAllPopupUI();
    }

    public Canvas SetCanvas(GameObject go, bool sort = true, int sortOrder = 0, bool isToast = false)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
        }

        CanvasScaler cs = go.GetOrAddComponent<CanvasScaler>();
        if (cs != null)
        {
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);
        }

        go.GetOrAddComponent<GraphicRaycaster>();

        if (sort)
        {
            canvas.sortingOrder = _popupOrder;
            _popupOrder++;
        }

        if (isToast)
        {
            _toastOrder++;
            canvas.sortingOrder = _toastOrder;
        }

        return canvas;
    }

    public T GetSceneUI<T>() where T : UI_Base
    {
        return _sceneUI as T;
    }

    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"{name}");
        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);
    }

    public T MakeOverlayUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"{name}");
        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        CanvasScaler cs = go.GetOrAddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);

        // CameraCheck Component

        return Util.GetOrAddComponent<T>(go);
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null, bool pooling = false)
        where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"{name}", parent, pooling);
        go.transform.SetParent(parent);
        return Util.GetOrAddComponent<T>(go);
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    public void CachePopupUI(Type type)
    {
        string name = type.Name;

        if (_popups.TryGetValue(name, out UI_Popup popup) == false)
        {
            GameObject go = Managers.Resource.Instantiate(name, Root.transform);
            popup = go.GetComponent<UI_Popup>();
            _popups[name] = popup;
        }

        _popupStack.Push(popup);
    }

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        if (_popups.TryGetValue(name, out UI_Popup popup) == false)
        {
            GameObject go = Managers.Resource.Instantiate(name);
            popup = Util.GetOrAddComponent<T>(go);
            _popups[name] = popup;
        }

        _popupStack.Push(popup);

        popup.transform.SetParent(Root.transform);
        popup.gameObject.SetActive(true);
        _popupOrder++;
        popup.UICanvas.sortingOrder = _popupOrder;
        return popup as T;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        //Managers.Sound.PlayPopupClose();
        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        popup.gameObject.SetActive(false);
        _popupOrder--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public int GetPopupCount()
    {
        return _popupStack.Count;
    }

    public void Clear()
    {
        CloseAllPopupUI();
        Time.timeScale = 1;
        _sceneUI = null;
    }


    #region Toast

    private void PrepareToast()
    {
        if (!isToastLoaded)
        {
            GameObject instance = MonoBehaviour.Instantiate(Managers.Resource.Load<GameObject>("ToastUI"));
            instance.name = "[ TOAST UI ]";
            _toastUI = instance.GetComponent<ToastUI>();
            isToastLoaded = true;
        }
    }

    public void ShowToast(string text, float duration = 1f, EToastColor color = EToastColor.Black, EToastPosition position = EToastPosition.MiddleCenter)
    {
        PrepareToast();
        _toastUI.SetInfo(text, duration, color, position);
    }

    public void Dismiss()
    {
        if (isToastLoaded)
            _toastUI.Dismiss();
    }

    #endregion
}