using System;

/// �ٿ�ε� ���� ��Ȳ ���� 
public struct DownloadProgressStatus
{
    public long downloadedBytes;// �ٿ�ε�� ����Ʈ ������ 
    public long totalBytes;     // �ٿ�ε� ���� ��ü ������ 
    public long remainedBytes;  // ���� ����Ʈ ������ 
    public float totalProgress; // ��ü ����� 0 ~ 1 

    public DownloadProgressStatus(long downloadedBytes, long totalBytes, long remainedBytes, float totalProgress)
    {
        this.downloadedBytes = downloadedBytes;
        this.totalBytes = totalBytes;
        this.remainedBytes = remainedBytes;
        this.totalProgress = totalProgress;
    }
}

public class DownloadEvents
{
    public event Action Initialized;
    public event Action CatalogUpdated;
    public event Action<long> SizeDownloaded;
    public event Action<DownloadProgressStatus> ProgressUpdated;
    public event Action<bool> Finished;

    public void NotifyInitialized() => Initialized?.Invoke();
    public void NotifyCatalogUpdated() => CatalogUpdated?.Invoke();
    public void NotifySizeDownloaded(long size) => SizeDownloaded?.Invoke(size);
    public void NotifyDownloadProgress(DownloadProgressStatus status) => ProgressUpdated?.Invoke(status);
    public void NotifyDownloadFinished(bool isSuccess) => Finished?.Invoke(isSuccess);
}