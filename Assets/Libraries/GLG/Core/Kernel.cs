using GLG.Ads;
using GLG.Analytics;
using GLG.UI;
using System;
using UnityEngine;

namespace GLG
{
    public static class Kernel
    {
        private static event Action OnUpdate = () => { };
        private static event Action OnFixedUpdate = () => { };
        private static event Action OnLateUpdate = () => { };

        #region GETTERS
        public static MainConfig Config { get; private set; }
        public static Economic Economic { get; private set; }
        public static MonoBehaviour CoroutinesObject { get; private set; }
        public static AdsWrapper Ad { get; private set; }
        public static GLGUI UI { get; private set; }
        public static LevelsManager LevelsManager { get; private set; }
        public static VFXFactory VFXFactory { get; private set; }
        public static AnalyticsWrapper Analytics { get; private set; }
        #endregion

        #region PUBLIC METHODS
        public static void RegisterLinker(KernelLinker linker)
        {
            Inventory.Load();
            linker.onUpdate += UpdateHandler;
            linker.onFixedUpdate += FixedUpdateHandler;
            linker.onLateUpdate += LateUpdateHandler;
            Config = linker.config;
            Economic = linker.economic;
            CoroutinesObject = linker.objForCoroutines;
            Ad = linker.ads;
            UI = linker.ui;
            LevelsManager = linker.levelManager;
            VFXFactory = linker.vfxFactory;
            Analytics = linker.analytics;
        }
        public static void RegisterManaged(MonoBehaviour script)
        {
            if (script is IManaged imanaged)
            {
                OnUpdate += imanaged.ManagedUpdate;
            }
            if (script is IManagedFixed ifixed)
            {
                OnFixedUpdate += ifixed.ManagedFixedUpdate;
            }
        }
        public static void UnregisterManaged(MonoBehaviour script)
        {
            if (script is IManaged imanaged)
            {
                OnUpdate -= imanaged.ManagedUpdate;
            }
            if (script is IManagedFixed ifixed)
            {
                OnFixedUpdate -= ifixed.ManagedFixedUpdate;
            }
        }
        public static void Dispose()
        {
            OnUpdate = null;
            OnFixedUpdate = null;
            OnLateUpdate = null;
        }
        #endregion

        #region HANDLERS
        private static void UpdateHandler()
        {
            OnUpdate();
        }
        private static void FixedUpdateHandler()
        {
            OnFixedUpdate();
        }
        private static void LateUpdateHandler()
        {
            OnLateUpdate();
        }
        #endregion
    }
}
