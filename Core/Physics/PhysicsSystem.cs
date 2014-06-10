using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace Catsland.Core {
    public class PhysicsSystem {

        private bool m_enable;
        public World m_world;
        
        public PhysicsSystem() {
            m_enable = false;
            m_world = null;
        }

        public World GetWorld() {
            return m_world;
        }

        public bool Initialize() {
            if (m_world == null) {
                m_world = new World(new Vector2(0.0f, -9.8f));
            }
            m_enable = true;
            return true;
        }

        public void Update(int timeLastFrame) {
            if (m_enable == false) {
                return;
            }
            if (m_world != null) {
                m_world.Step((float)timeLastFrame/1000.0f);
            }
        }

        public void EditorUpdate(int timelastFrame) {
        }

        public void Disable() {
            m_enable = false;
        }

        public void Enable() {
            if (m_world != null) {
                m_enable = true;
            }
        }
    }
}
