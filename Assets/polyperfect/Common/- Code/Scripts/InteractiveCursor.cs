using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;

namespace Polyperfect.Common
{
    public class InteractiveCursor : PolyMono
    {
        public override string __Usage => "Follows the mouse and responds to contexts.";
        readonly List<RaycastResult> _hits = new List<RaycastResult>();

        static InteractiveCursor Instance
        {
            get
            {
                if (!instance)
                    instance = new GameObject("Interactive Cursor").AddComponent<InteractiveCursor>();
                return instance;
            }
        }

        static InteractiveCursor instance;

        public static event Action<RaycastResult> CursorUpdate
        {
            add => Instance.cursorUpdate += value;
            remove => Instance.cursorUpdate -= value;
        }

        event Action<RaycastResult> cursorUpdate;

        public static RaycastResult CursorInfo;
        void Update()
        {
            _hits.Clear();
            
            var pointer = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            EventSystem.current.RaycastAll(pointer, _hits);
            CursorInfo = _hits.FirstOrDefault();
            
            if (!CursorInfo.gameObject)
                return;
            
            cursorUpdate?.Invoke(CursorInfo);
        }
    }
}