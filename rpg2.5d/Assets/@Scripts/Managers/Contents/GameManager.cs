using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GameManager
{
    #region Action

    public event Action<EBroadcastEventType, ECurrencyType, int> OnBroadcastEvent;

    #endregion

    #region GameData
    GameSaveData _saveData = new GameSaveData();
    public GameSaveData SaveData
    {
        get { return _saveData; }
        set { _saveData = value; }
    }
    public int PlayerLevel
    {
        get { return _saveData.PlayerLevel; }
        private set { _saveData.PlayerLevel = value; }
    }

    public int PlayerExp
    {
        get { return _saveData.PlayerExp; }
        private set { _saveData.PlayerExp = value; }
    }

    //public List< /*TemplateId*/int> UnlockedTrainings = new List<int>();

    //public Dictionary<ECurrencyType, Storage> Storages = new Dictionary<ECurrencyType, Storage>();

    public Vector3Int LastWorldPos
    {
        get { return _saveData.LastWorldPos; }
        set { _saveData.LastWorldPos = value; }
    }

    public void BroadcastEvent(EBroadcastEventType eventType, ECurrencyType currencyType = ECurrencyType.None, int value = 0)
    {
        switch (eventType)
        {
            case EBroadcastEventType.KillMonster:
                AddExp(1);
                break;
        }

        OnBroadcastEvent?.Invoke(eventType, currencyType, value);
        if (Managers.Scene.CurrentScene.SceneType == EScene.GameScene)
        {
            //SaveGame();//юс╫ц
        }

    }

    public int GenerateItemDbId()
    {
        int itemDbId = _saveData.ItemDbIdGenerator;
        _saveData.ItemDbIdGenerator++;
        return itemDbId;
    }
    #endregion

    private Hero _playerHero;
    public Hero PlayerHero
    {
        get => _playerHero;
        set { _playerHero = value; }
    }

    public void Init()
    {
        //if (File.Exists(Path) == false)
        //    InitGame();
        //else
        //    LoadGame();

        foreach (var key in Managers.Data.CurrencyDic.Keys)
        {
            Managers.Inventory.MakeItem(key, 0);
        }
    }

    private CameraController _cam;

    public CameraController Cam
    {
        get
        {
            if (_cam == null)
            {
                _cam = Object.FindObjectOfType<CameraController>();
            }

            return _cam;
        }
    }

    #region Player Level
    public void AddExp(int value)
    {
        if (IsMaxLevel())
            return;

        PlayerExp += value;
        if (CanLevelUp())
            TryLevelUp();
    }

    public bool CanLevelUp()
    {
        return (GetExpToNextLevel() - PlayerExp <= 0);
    }

    private void TryLevelUp()
    {
        while (!IsMaxLevel())
        {
            if (CanLevelUp() == false)
                break;

            PlayerExp -= GetExpToNextLevel();
            PlayerLevel++;

            Managers.Game.BroadcastEvent(EBroadcastEventType.PlayerLevelUp, value: PlayerLevel);
        }
    }

    public float GetExpNormalized()
    {
        if (IsMaxLevel())
            return 1f;
        else
            return (float)PlayerExp / GetExpToNextLevel();
    }

    public int GetRemainsExp()
    {
        return GetExpToNextLevel() - PlayerExp;
    }

    private int GetExpToNextLevel()
    {
        PlayerLevelData playerLevelData;
        if (Managers.Data.PlayerLevelDic.TryGetValue(PlayerLevel, out playerLevelData))
        {
            return playerLevelData.Exp;
        }
        else
        {
            Debug.Log("Level invalid: " + PlayerLevel);
            return 100;
        }
    }
    public bool IsMaxLevel()
    {
        return PlayerLevel == Managers.Data.PlayerLevelDic.Count;
    }
    #endregion

}
