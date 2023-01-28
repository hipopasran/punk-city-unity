using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace GLG.UI
{

    public enum UIType { Screen, Overlay }
    public enum UIEntity { Loading, MainMenu, Advertisement, Level, Win, Fail, Pause, Other }
    public enum UIAnimationType { Instantly, LeftToRight, RighToLeft, UpToDown, DownToUp, ScaleWithOpacity, Opacity }

    public class GLGUI : MonoBehaviour
    {
        public Camera mainCamera;
        private Dictionary<UIEntity, UIController> _mainUI = new Dictionary<UIEntity, UIController>();
        private Dictionary<Type, UIController> _UI = new Dictionary<Type, UIController>();
        public List<UIController> OtherUI { get; private set; } = new List<UIController>();
        public UIController LastOpenedUI { get; private set; }
        public UIController LastOpenedScreen { get; private set; }
        public UIController LastOpenedOverlay { get; private set; }
        public UIController LastOpenedMainUI { get; private set; }
        public UIController LastOpenedOtherUI { get; private set; }
        public bool initializeOnStart = true;

        private void Awake()
        {
            if (initializeOnStart) Initialize();
            DontDestroyOnLoad(mainCamera.gameObject);
        }
        public void Initialize()
        {
            foreach (Transform item in transform)
            {
                UIController controller;
                if (item.TryGetComponent(out controller))
                {
                    _UI.Add(controller.GetType(), controller);
                    if (controller.uIEntity == UIEntity.Other)
                    {
                        OtherUI.Add(controller);
                    }
                    else
                    {
                        _mainUI.Add(controller.uIEntity, controller);
                    }
                    controller.gameObject.SetActive(controller.immunityToHiding || controller.showOnStart);
                }
            }
        }

        #region SHOW
        public T ShowUI<T>(System.Action callback = null) where T : UIController
        {
            UIController item = (T)_UI[typeof(T)];
            if (CheckEqualsToLastOpened(item)) return (T)item;
            item.Show(callback);
            PutToLast(item);
            return (T)item;
        }
        public T ShowUI<T>(T controller, System.Action callback = null) where T : UIController
        {
            if (CheckEqualsToLastOpened(controller)) return (T)controller;
            controller.Show(callback);
            PutToLast(controller);
            return controller;
        }
        #endregion
        #region HIDE
        public void HideMainUI(UIEntity uIEnity, System.Action callback = null)
        {
            if (_mainUI.ContainsKey(uIEnity))
            {
                _mainUI[uIEnity].Hide(callback);
                return;
            }
            else if (callback != null) callback();
#if !FINAL_BUILD
            Debug.Log($"[UI] Can\'t find {uIEnity} UI Controller!");
#endif
        }
        public void HideMainUI(UIEntity uIEnity, UIAnimationType animationType, float animationDuration, System.Action callback)
        {
            if (_mainUI.ContainsKey(uIEnity))
            {
                _mainUI[uIEnity].Hide(animationType, animationDuration, callback);
                return;
            }
            else if (callback != null) callback();
#if !FINAL_BUILD
            Debug.Log($"[UI] Can\'t find {uIEnity} UI Controller!");
#endif
        }
        public void HideOtherUI(string name, System.Action callback = null)
        {
            foreach (var item in OtherUI)
            {
                if (item.name == name)
                {
                    item.Hide(callback);
                    return;
                }
            }
            if (callback != null) callback();
#if !FINAL_BUILD
            Debug.Log($"[UI] Can\'t find {name} UI Controller!");
#endif
        }
        public void HideOtherUI(string name, UIAnimationType animationType, float animationDuration, System.Action callback)
        {
            foreach (var item in OtherUI)
            {
                if (item.name == name)
                {
                    item.Hide(animationType, animationDuration, callback);
                    return;
                }
            }
            if (callback != null) callback();
#if !FINAL_BUILD
            Debug.Log($"[UI] Can\'t find {name} UI Controller!");
#endif
        }
        public void HideUI(string name, System.Action callback = null)
        {
            foreach (var item in OtherUI)
            {
                if (item.name == name)
                {
                    item.Hide(callback);
                    return;
                }
            }
            foreach (var item in _mainUI)
            {
                if (item.Value.name == name)
                {
                    item.Value.Hide(callback);
                    return;
                }
            }
            if (callback != null) callback();
#if !FINAL_BUILD
            Debug.Log($"[UI] Can\'t find {name} UI Controller!");
#endif
        }
        public void HideUI(string name, UIAnimationType animationType, float animationDuration, System.Action callback = null)
        {
            foreach (var item in OtherUI)
            {
                if (item.name == name)
                {
                    item.Hide(animationType, animationDuration, callback);
                    return;
                }
            }
            foreach (var item in _mainUI)
            {
                if (item.Value.name == name)
                {
                    item.Value.Hide(animationType, animationDuration, callback);
                    return;
                }
            }
            if (callback != null) callback();
#if !FINAL_BUILD
            Debug.Log($"[UI] Can\'t find {name} UI Controller!");
#endif
        }
        public void HideLastUI(System.Action callback = null)
        {
            if (LastOpenedUI != null) LastOpenedUI.Hide(callback);
            else if (callback != null) callback();
        }
        public void HideLastUI(UIAnimationType uIanimationType, float animationDelay, System.Action callback = null)
        {
            if (LastOpenedUI != null) LastOpenedUI.Hide(uIanimationType, animationDelay, callback);
            else if (callback != null) callback();
        }
        public void HideLastScreen(System.Action callback = null)
        {
            if (LastOpenedScreen != null) LastOpenedScreen.Hide(callback);
            else if (callback != null) callback();
        }
        public void HideLastScreen(UIAnimationType uIanimationType, float animationDelay, System.Action callback = null)
        {
            if (LastOpenedScreen != null) LastOpenedScreen.Hide(uIanimationType, animationDelay, callback);
            else if (callback != null) callback();
        }
        public void HideLastOverlay(System.Action callback = null)
        {
            if (LastOpenedOverlay != null) LastOpenedOverlay.Hide(callback);
            else if (callback != null) callback();
        }
        public void HideLastOverlay(UIAnimationType uIanimationType, float animationDelay, System.Action callback = null)
        {
            if (LastOpenedOverlay != null) LastOpenedOverlay.Hide(uIanimationType, animationDelay, callback);
            else if (callback != null) callback();
        }
        public void HideAll()
        {
            foreach (var item in _mainUI)
            {
                item.Value.Hide();
            }
            foreach (var item in OtherUI)
            {
                item.Hide();
            }
        }
        public T HideUI<T>(System.Action callback = null) where T : UIController
        {
            UIController item = (T)_UI[typeof(T)];
            item.Hide(callback);
            return (T)item;
        }
        #endregion
        #region GET
        public void Get<T>(out T uiController) where T : UIController
        {
            foreach (var item in _mainUI)
            {
                if (item.Value is T)
                {
                    uiController = item.Value as T;
                    return;
                }
            }
            foreach (var item in OtherUI)
            {
                if (item is T)
                {
                    uiController = item as T;
                    return;
                }
            }
            uiController = null;
        }
        public T Get<T>() where T : UIController => (T)_UI[typeof(T)];
        #endregion
        #region IS ACTIVE
        public bool IsActiveMain(UIEntity uIEnity)
        {
            if (_mainUI.ContainsKey(uIEnity))
            {
                return _mainUI[uIEnity].gameObject.activeSelf;
            }
            return false;
        }
        public bool IsActiveOther(string name)
        {
            foreach (var item in OtherUI)
            {
                if (item.name == name)
                {
                    return item.gameObject.activeSelf;
                }
            }
            return false;
        }
        public bool IsActive(string name)
        {
            foreach (var item in OtherUI)
            {
                if (item.name == name)
                {
                    return item.gameObject.activeSelf;
                }
            }
            foreach (var item in _mainUI)
            {
                if (item.Value.name == name)
                {
                    return item.Value.gameObject.activeSelf;
                }
            }
            return false;
        }
        #endregion

        private void PutToLast(UIController uIController)
        {
            LastOpenedUI = uIController;
            if (uIController.uIType == UIType.Screen)
            {
                LastOpenedScreen = uIController;
            }
            else if (uIController.uIType == UIType.Overlay)
            {
                LastOpenedOverlay = uIController;
            }
            if (uIController.uIEntity == UIEntity.Other)
            {
                LastOpenedOtherUI = uIController;
            }
            else
            {
                LastOpenedMainUI = uIController;
            }
        }
        private bool CheckEqualsToLastOpened(UIController uIController)
        {
            //if (useCallbackAnyway) if (callback != null) callback();
            return uIController.gameObject.activeSelf;
        }
        public void SetSortingOrderAsSiblingIndexes()
        {
            foreach (Transform item in transform)
            {
                item.GetComponent<Canvas>().sortingOrder = item.GetSiblingIndex();
            }
        }
        public static UIAnimationType GetOppositeAnimationType(UIAnimationType uIAnimationType)
        {
            switch (uIAnimationType)
            {
                case UIAnimationType.Instantly:
                    return UIAnimationType.Opacity;
                case UIAnimationType.LeftToRight:
                    return UIAnimationType.RighToLeft;
                case UIAnimationType.RighToLeft:
                    return UIAnimationType.LeftToRight;
                case UIAnimationType.UpToDown:
                    return UIAnimationType.DownToUp;
                case UIAnimationType.DownToUp:
                    return UIAnimationType.UpToDown;
                case UIAnimationType.Opacity:
                    return UIAnimationType.Instantly;
                default:
                    return UIAnimationType.Instantly;
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(GLGUI))]
    public class GLGUIEditor : Editor
    {
        private GLGUI _target;
        private void OnEnable()
        {
            _target = target as GLGUI;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Set sorting order as sibling indexes"))
            {
                _target.SetSortingOrderAsSiblingIndexes();
            }
        }
    }
#endif
}
