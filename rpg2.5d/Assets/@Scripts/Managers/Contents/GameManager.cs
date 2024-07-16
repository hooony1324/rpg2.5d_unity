using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    #endregion
    //public List< /*TemplateId*/int> UnlockedTrainings = new List<int>();

    //public Dictionary<ECurrencyType, Storage> Storages = new Dictionary<ECurrencyType, Storage>();

    public void Init()
    {
        InitGame();
        //if (File.Exists(Path) == false)
        //    InitGame();
        //else
        //    LoadGame();


    }

    void InitGame()
    {
        // Quest
        var quests = Managers.Data.QuestDic.Values.ToList();
        foreach (QuestData questData in quests)
        {
            QuestSaveData saveData = new QuestSaveData()
            {
                TemplateId = questData.TemplateId,
                State = EQuestState.None,
                TaskProgressCount = new List<int>(),
                TaskStates = new List<EQuestState>(),
                NextResetTime = DateTime.Now,
            };

            for (int i = 0; i < questData.QuestTasks.Count; i++)
            {
                saveData.TaskProgressCount.Add(0);
                saveData.TaskStates.Add(EQuestState.None);
            }

            Managers.Quest.AddQuest(saveData);
        }

        Managers.Quest.MainQuest.StartQuest();

        // Inventory
        foreach (var key in Managers.Data.CurrencyDic.Keys)
        {
            Managers.Inventory.MakeItem(key, 0);
        }
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

    public Vector3Int LastWorldPos
    {
        get { return _saveData.LastWorldPos; }
        set { _saveData.LastWorldPos = value; }
    }

    private Hero _playerHero;
    public Hero PlayerHero
    {
        get => _playerHero;
        set { _playerHero = value; }
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
