using Cinemachine;
using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro.EditorUtilities;
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
        private set 
        {
            _saveData.PlayerLevel = value;
            Managers.Game.PlayerHero.OnPlayerStatChanged?.Invoke();
        }
    }

    public int PlayerExp
    {
        get { return _saveData.PlayerExp; }
        private set 
        { 
            _saveData.PlayerExp = value;
            Managers.Game.PlayerHero.OnPlayerStatChanged?.Invoke();
        }
    }
    #endregion

    public void Init()
    {
        if (File.Exists(Path) == false)
            InitGame();
        else
            LoadGame();
    }

    #region Save & Load
    public string Path
    {
        get { return Application.persistentDataPath + "/SaveData.json"; }
    }

    #endregion

    void InitGame()
    {
        Managers.Game.SaveData.LastWorldPos = new Vector3(-10, 1, -3);

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

    public void SaveGame()
    {
        LastWorldPos = PlayerHero.Position;

        // Quest
        {
            SaveData.AllQuests.Clear();
            foreach (Quest quest in Managers.Quest.AllQuests.Values)
            {
                SaveData.AllQuests.Add(quest.SaveData);
            }
        }

        // Item
        {
            SaveData.Items.Clear();
            foreach (var item in Managers.Inventory.AllItems)
                SaveData.Items.Add(item.SaveData);
        }

        string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData);
        File.WriteAllText(Path, jsonStr);
    }

    public void LoadGame()
    {
        string fileStr = File.ReadAllText(Path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(fileStr);

        if (data != null)
            Managers.Game.SaveData = data;

        // Quest
        {
            Managers.Quest.Clear();

            foreach (QuestSaveData questSaveData in data.AllQuests)
            {
                Quest quest = Managers.Quest.AddQuest(questSaveData);
            }
            Managers.Quest.AddUnknownQuests();
        }

        //Item
        {
            Managers.Inventory.Clear();

            for (int i = 0; i < data.Items.Count; i++)
            {
                Managers.Inventory.AddItem(data.Items[i]);
            }
        }

        Debug.Log($"Save Game Loaded : {Path}");
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
            //SaveGame();
        }

    }

    public int GenerateItemDbId()
    {
        int itemDbId = _saveData.ItemDbIdGenerator;
        _saveData.ItemDbIdGenerator++;
        return itemDbId;
    }

    public Vector3 LastWorldPos
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
            _cam ??= Camera.main.gameObject.GetComponent<CameraController>();
            
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

    public int GetExpToNextLevel()
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
