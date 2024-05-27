using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_TitleScene : UI_Scene
{
    enum GameObjects
    {
        StartButton,
    }

    enum Texts
    {
        SceneStatusText,
    }

    public enum EState
    {
        None = 0,
        CalculatingSize,
        NothingToDownload,
        AskingDownload,
        Downloading,
        DownloadFinished
    }

    Downloader _downloader;
    DownloadProgressStatus progressInfo;
    ESizeUnits _eSizeUnit;
    long curDownloadedSizeInUnit;
    long totalSizeInUnit;

    private EState _currentState = EState.None;

    public EState CurrentState
    {
        get => _currentState;
        set
        {
            _currentState = value;
            UpdateUI();
        }
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));

        //GetObject((int)GameObjects.StartButton).gameObject.SetActive(false);
        GetObject((int)GameObjects.StartButton).BindEvent(() =>
        {
            Debug.Log("StartGame...");
        });
        GetText((int)Texts.SceneStatusText).text = $"Loading Resources";

        return true;
    }

    void UpdateUI()
    {
        
    }
}
