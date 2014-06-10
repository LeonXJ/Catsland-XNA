using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework.Input;


namespace HorseRiding {
    public class QTE : CatComponent{

#region  Properties

        private HashSet<Keys> m_maskKey = new HashSet<Keys>();
        private Queue<QTEPack> m_qtePacks = new Queue<QTEPack>();
        private bool m_isInQTE = false;
        private IQTEAction m_actions;

#endregion

        public QTE():base(){}
        public QTE(GameObject _gameObject)
            :base(_gameObject){
        }

        public bool IsInQTE() {
            return m_isInQTE;
        }

        public void ClearQTEQueue() {
            m_isInQTE = false;
            m_qtePacks.Clear();
        }

        public void AppendEvent(QTEPack _qtePack) {
            m_qtePacks.Enqueue(_qtePack);
        }

        public void StartQTE(IQTEAction _qteAction) {
            m_actions = _qteAction;
            m_isInQTE = true;
            StartKeyOnHead();
        }

        public void AddMaskKey(Keys _key) {
            m_maskKey.Add(_key);
        }

        public void ClearMaskKey() {
            m_maskKey.Clear();
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);
            // Warning: QTE must get non-skewed time
            int nonSkewedTime = (int)(timeLastFrame / Mgr<GameEngine>.Singleton.TimeScale);
            UpadateQTE(nonSkewedTime);
        }

        private void StartKeyOnHead() {
            if (m_qtePacks.Count > 0) {
                QTEPack curPack = m_qtePacks.Peek();
                // show the key
            }
            else {
                OnSucess();
            }
        }

        private void OnSucess() {
            if (m_actions != null) {
                m_actions.OnSuccess();
                m_isInQTE = false;
                m_qtePacks.Clear();
            }
        }

        private void OnFail() {
            if (m_actions != null) {
                m_actions.OnFail();
                m_isInQTE = false;
                m_qtePacks.Clear();
            }
        }

        public void UpadateQTE(int _nonSkewedTimeInMS) {
            if (m_isInQTE) {
                if (m_qtePacks.Count > 0) {
                    QTEPack curPack = m_qtePacks.Peek();
                    // check key
                    KeyboardState keyboardState = Keyboard.GetState();

                    if (keyboardState.IsKeyDown(curPack.WaitingKey)) {
                        m_qtePacks.Dequeue();
                        StartKeyOnHead();
                    }
                    else {
                        // time pass
                        curPack.TimeInMS -= _nonSkewedTimeInMS;
                        if (m_actions != null) {
                            m_actions.Announce(curPack.WaitingKey, curPack.TimeInMS);
                        }
                        if (curPack.TimeInMS <= 0) {
                            OnFail();
                        }
                    }
                }
                else {
                    OnSucess();
                }
            }
        }


    }

    public class QTEPack{
#region Properties

        Keys m_waitingKey;
        public Keys WaitingKey {
            get {
                return m_waitingKey;
            }
        }
        int m_waitingTimeInMS;
        public int TimeInMS {
            get {
                return m_waitingTimeInMS;
            }
            set {
                m_waitingTimeInMS = value;
            }
        }

#endregion

        public QTEPack(Keys _key, int _time) {
            m_waitingKey = _key;
            m_waitingTimeInMS = _time;
        }
    }
}
