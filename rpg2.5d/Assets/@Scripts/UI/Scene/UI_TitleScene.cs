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

        
        GetObject((int)GameObjects.StartButton).BindEvent(() =>
        {
            Debug.Log("StartGame...");
            Managers.Scene.LoadScene(EScene.GameScene);
        });
        GetText((int)Texts.SceneStatusText).text = $"Loading Resources";
        GetObject((int)GameObjects.StartButton).gameObject.SetActive(false);

        _downloader = gameObject.GetOrAddComponent<Downloader>();
        _downloader.DownloadLabel = "Preload";

        return true;
    }

    IEnumerator Start()
    {
        yield return _downloader.StartDownload((events) =>
        {
            events.Initialized += OnInitialized;
            events.CatalogUpdated += OnCatalogUpdated;
            events.SizeDownloaded += OnSizeDownloaded;
            events.ProgressUpdated += OnProgress;
            events.Finished += OnFinished;
        });
    }

    void UpdateUI()
    {
        switch (CurrentState)
        {
            case EState.CalculatingSize:
                GetText((int)Texts.SceneStatusText).text = "�ٿ�ε� ������ �������� �ֽ��ϴ�. ��ø� ��ٷ��ּ���.";
                break;
            case EState.NothingToDownload:
                GetText((int)Texts.SceneStatusText).text = "�ٿ�ε� ���� �����Ͱ� �����ϴ�.";
                break;
            case EState.AskingDownload:
                GetText((int)Texts.SceneStatusText).text = $"�ٿ�ε带 �����ðڽ��ϱ� ? �����Ͱ� ���� ���� �� �ֽ��ϴ�. <color=green>({$"{this.totalSizeInUnit}{this._eSizeUnit})</color>"}";
                break;
            case EState.Downloading:
                GetText((int)Texts.SceneStatusText).text = $"�ٿ�ε����Դϴ�. ��ø� ��ٷ��ּ���. {(progressInfo.totalProgress * 100).ToString("0.00")}% �Ϸ�";
                break;
            case EState.DownloadFinished:
                GetText((int)Texts.SceneStatusText).text = $"�ٿ�ε�Ϸ�, ���� �ε� �� �ڵ�����";

                // Load ����
                Managers.Resource.LoadAllAsync<Object>("Preload", (key, count, totalCount) =>
                {
                    GetText((int)Texts.SceneStatusText).text = $"�ε��� : {key} {count}/{totalCount}";

                    if (count == totalCount)
                    {
                        GetText((int)Texts.SceneStatusText).text = $"�ε� �Ϸ�";
                        GetObject((int)GameObjects.StartButton).gameObject.SetActive(true);
                        Managers.Data.Init();
                        Managers.Game.Init();

                    }
                });
                break;
        }
    }
    private void OnInitialized()
    {
        _downloader.GoNext();
    }

    private void OnCatalogUpdated()
    {
        _downloader.GoNext();
    }

    private void OnSizeDownloaded(long size)
    {
        Debug.Log($"�ٿ�ε� �Ϸ� ! : {Util.GetConvertedByteString(size, ESizeUnits.KB)} ({size}����Ʈ)");

        if (size == 0)
        {
            CurrentState = EState.DownloadFinished;
        }
        else
        {
            _eSizeUnit = Util.GetProperByteUnit(size);
            totalSizeInUnit = Util.ConvertByteByUnit(size, _eSizeUnit);

            CurrentState = EState.AskingDownload;

            CurrentState = EState.Downloading;
            _downloader.GoNext();
        }
    }

    private void OnProgress(DownloadProgressStatus newInfo)
    {
        bool changed = this.progressInfo.downloadedBytes != newInfo.downloadedBytes;

        progressInfo = newInfo;

        if (changed)
        {
            UpdateUI();

            curDownloadedSizeInUnit = Util.ConvertByteByUnit(newInfo.downloadedBytes, _eSizeUnit);
        }
    }

    private void OnFinished(bool isSuccess)
    {
        CurrentState = EState.DownloadFinished;
        _downloader.GoNext();
    }
}
