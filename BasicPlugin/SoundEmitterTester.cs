using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;

namespace Catsland.Plugin {
    class SoundEmitterTester : CatComponent {

#region Properties
        int m_accumulateMillionSecond = 0;

#endregion

        public SoundEmitterTester()
            : base() {
        }

        public SoundEmitterTester(GameObject _gameObject)
            : base(_gameObject) {
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            m_accumulateMillionSecond += timeLastFrame;
            if ((m_accumulateMillionSecond > 500))
            {
                SoundEmitter se =
                    m_gameObject.GetComponent(typeof(SoundEmitter).ToString())
                    as SoundEmitter;
                if (se != null) {
                    se.PlaySound("sound\\m4a1", false, 1.0f, 300.0f);
                }
                m_accumulateMillionSecond -= 500;
            }
        }
    }
}
