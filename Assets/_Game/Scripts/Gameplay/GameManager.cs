using Core.Utils;
using Gameplay.UI;
using System;
using UnityEngine;

namespace Gameplay
{
    public enum GameState
    {
        Idle, 
        Playing,
        Win,
        Lose
    }
    public class GameManager : Singleton<GameManager>
    {
        InGameHUD _hud;
        Board _board;

        GameData _gameData;
        SaveLoadHelper<GameData> _loader = new SaveLoadHelper<GameData>("GameData");
        Timer _gameTimer;
        GameState _state;

        public GameData Data => _gameData;

        protected override void OnInit()
        {
            _gameData = _loader.LoadData();
            UpdateLife();
        }

        public void UpdateLife()
        {
            DateTime now = DateTime.Now;
            if (_gameData.Life < GameConst.MAX_LIFE &&
                now.Ticks - _gameData.lastTimeRegen > TimeSpan.TicksPerSecond * GameConst.REGEN_TIME)
            {
                _gameData.Life++;
                _gameData.lastTimeRegen = now.Ticks;

                _loader.SaveData(_gameData);
            }
        }

        public void OnTouchBegan()
        {
            if (_state == GameState.Idle)
            {
                StartGame();
            }
        }

        public void StartGame()
        {
            _state = GameState.Playing;
            _gameTimer.Duration = 90f;
            _gameTimer.Run();
        }

        public void Restart()
        {
            
        }

        public void UseItem()
        {

        }

        public bool UseFreeze()
        {
            if (_gameData.Freeze > 0)
            {
                _gameData.Freeze--;
                _loader.SaveData(_gameData);
                
            }
            return true;
        }


        public void SetBoard(Board board)
        {
            _board = board;

        }

        public void SetHUD(InGameHUD ui)
        {
            _hud = ui;
        }

        public void OnWin()
        {
            //todo
            Log("OnWin");
        }

        public void OnLose()
        {
            //todo
            Log("OnLose");
        }

        static void Log(string msg)
        {
            Debug.Log($"[GameManager] {msg}");
        }

        public void UpdateGameTime()
        {
            if (_hud != null)
            {
                _hud.UpdateTimeLeft((int)_gameTimer.SecondsLeft);
            }
        }
    }
}

