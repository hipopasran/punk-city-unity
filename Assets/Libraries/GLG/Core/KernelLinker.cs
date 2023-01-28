using GLG.Ads;
using GLG.UI;
using GLG.Analytics;
using System;
using UnityEngine;

namespace GLG
{
    public class KernelLinker : MonoBehaviour
    {
        public event Action onUpdate;
        public event Action onFixedUpdate;
        public event Action onLateUpdate;

        public MonoBehaviour objForCoroutines;
        public Economic economic;
        public MainConfig config;
        public AdsWrapper ads;
        public GLGUI ui;
        public LevelsManager levelManager;
        public VFXFactory vfxFactory;
        public AnalyticsWrapper analytics;

        #region UNITY MESSAGES
        private void Awake()
        {
            Application.targetFrameRate = 60;
            Kernel.RegisterLinker(this);
        }
        private void Update()
        {
            onUpdate();
        }
        private void FixedUpdate()
        {
            onFixedUpdate();
        }
        private void LateUpdate()
        {
            onLateUpdate();
        }
        private void OnDestroy()
        {
            Kernel.Dispose();
        }
        #endregion
    }
}
