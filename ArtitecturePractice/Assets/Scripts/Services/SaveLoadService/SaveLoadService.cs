﻿using Data.Player;
using UnityEngine;
using Watchers.SaveLoadWatchers;
using Services.PersistentProgress;

namespace Services.SaveLoadService
{
    public class SaveLoadService : ISaveLoadService
    {
        public SaveLoadService(ISaveLoadInstancesWatcher saveLoadInstancesWatcher, IPersistentProgressService persistentProgressService)
        {
            _saveLoadInstancesWatcher = saveLoadInstancesWatcher;
            _persistentProgressService = persistentProgressService;
        }

        private readonly ISaveLoadInstancesWatcher _saveLoadInstancesWatcher;
        private readonly IPersistentProgressService _persistentProgressService;

        private const string ProgressKey = "ProgressKey";

        public void SaveProgress()
        {
            foreach (var progressSavable in _saveLoadInstancesWatcher.ProgressSavableInstances)
            {
                progressSavable.UpdateProgress(_persistentProgressService.PlayerProgress);
            }

            PlayerPrefs.SetString(ProgressKey, _persistentProgressService.PlayerProgress.ToJson());
        }

        public PlayerProgress LoadProgress()
        {
            var prefs = PlayerPrefs.GetString(ProgressKey)?.ToDeserialized<PlayerProgress>();
            return prefs;
        }
    }
}