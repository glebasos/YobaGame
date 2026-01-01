using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YobaGame.Models.Managers
{
    public static class SignalManager
    {
        public delegate void YobasCollidedEventHandler(Vector2 _playerPos, Yoba _yoba, Yoba _otherYoba);
        public static event YobasCollidedEventHandler? YobasCollidedSignal;
        
        public delegate void PlayerSpawnedYobaEventHandler();
        public static event PlayerSpawnedYobaEventHandler? PlayerSpawnedYobaSignal;
        
        public delegate void GetPlayerYobaEventHandler(Yoba _yoba);
        public static event GetPlayerYobaEventHandler? GetPlayerYobaSignal;
        
        public delegate void GetNextYobaEventHandler(Yoba _yoba);
        public static event GetNextYobaEventHandler? GetNextYobaSignal;
        
        public delegate void PlayerReadyHandler();
        public static event PlayerReadyHandler? PlayerReadySignal;
        
        public delegate void NextYobaHandler();
        public static event NextYobaHandler? NextYobaReadySignal;
        
        public delegate void PopHandler();
        public static event PopHandler? PopSignal;
        
        public delegate void BurnHandler();
        public static event BurnHandler? BurnSignal;
        
        public delegate void ExplodeEventHandler(Yoba _yoba, Yoba _otherYoba);
        public static event ExplodeEventHandler? ExplodeSignal;
        
        public delegate void GameOverHandler();
        public static event GameOverHandler? GameOverSignal;
        
        public delegate void LoseHealthHandler();
        public static event LoseHealthHandler? LoseHealthSignal;
        
        public delegate void ComboScoredEventhandler(int _score);
        public static event ComboScoredEventhandler? ComboScoredSignal;
        
        public delegate void YobaFellHandler();
        public static event YobaFellHandler? YobaFellSignal;
        
        public delegate void CameraShakeEventHandler(bool _enable);
        public static event CameraShakeEventHandler? CameraShakeSignal;

        internal static void InvokeYobasCollided(Vector2 _playerPos, Yoba _yoba, Yoba _otherYoba)
        {
            YobasCollidedSignal?.Invoke(_playerPos, _yoba, _otherYoba);
        }
        
        internal static void InvokePlayerSpawnedYoba()
        {
            PlayerSpawnedYobaSignal?.Invoke();
        }
        
        internal static void InvokeGivePlayerYoba(Yoba _yoba)
        {
            GetPlayerYobaSignal?.Invoke(_yoba);
        }
        
        internal static void InvokeGiveNextYoba(Yoba _yoba)
        {
            GetNextYobaSignal?.Invoke(_yoba);
        }
        
        
        internal static void InvokePlayerReady()
        {
            PlayerReadySignal?.Invoke();
        }
        
        internal static void InvokeNextYobaReady()
        {
            NextYobaReadySignal?.Invoke();
        }
        
        internal static void InvokePop()
        {
            PopSignal?.Invoke();
        }
        
        internal static void InvokeBurn()
        {
            BurnSignal?.Invoke();
        }

        internal static void InvokeExplode(Yoba yoba, Yoba otherYoba)
        {
            ExplodeSignal?.Invoke(yoba, otherYoba);
        }
        
        internal static void InvokeGameOver()
        {
            GameOverSignal?.Invoke();
        }
        
        internal static void InvokeLoseHealth()
        {
            LoseHealthSignal?.Invoke();
        }
        
        internal static void InvokeComboScored(int _score)
        {
            ComboScoredSignal?.Invoke(_score);
        }
        
        internal static void InvokeYobaFell()
        {
            YobaFellSignal?.Invoke();
        }
        
        internal static void InvokeCameraShake(bool _enable)
        {
            CameraShakeSignal?.Invoke(_enable);
        }
    }
}
