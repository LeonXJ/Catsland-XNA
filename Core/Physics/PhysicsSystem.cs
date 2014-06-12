using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Collision;

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
                m_world.ContactManager.PreSolve += PreSolve;
            }
            m_enable = true;
            return true;
        }

        protected void PreSolve(Contact contact, ref Manifold oldManifold) {
            Fixture fixtureA = contact.FixtureA;
            Fixture fixtureB = contact.FixtureB;

            Fixture platform = null;
            Fixture thing = null;
            if(fixtureA.Body.UserData != null){
                if(Tag.Platform == (fixtureA.Body.UserData as Tag)){
                    platform = fixtureA;
                    thing = fixtureB;
                }
            }
            if(fixtureB.Body.UserData != null){
                if(Tag.Platform == (fixtureB.Body.UserData as Tag)){
                    platform = fixtureB;
                    thing = fixtureA;
                }
            }
            if (thing != null && platform != null) {
                Tag thingTag = thing.Body.UserData as Tag;
                if (thing.Body.Position.Y < platform.Body.Position.Y + 0.05f + thingTag.getHalfHeight()) {
                    contact.Enabled = false;
                }
            }
            
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

    public class Tag {
        static readonly public Tag Platform = new Tag(1);
        static readonly public Tag AttachPoint = new Tag(2);

        private int m_kind = 0;
        private float m_halfHeight = 0.0f;

        public Tag(int _kind, float _halfHeight = 0.0f) {
            m_kind = _kind;
            m_halfHeight = _halfHeight;
        }

//         public override bool Equals(object obj) {
//             if (obj is Tag) {
//                 Tag b = obj as Tag;
//                 return b.m_kind == m_kind;
//             }
//             return false;
//         }

        public static bool operator ==(Tag _a, Tag _b) {
            if (_a as object == null && _b as object == null) {
                return true;
            }
            else if (_a as object == null || _b as object == null) {
                return false;
            }
            return _a.m_kind == _b.m_kind;
        }

        public static bool operator !=(Tag _a, Tag _b) {
            if (_a as object == null && _b as object == null) {
                return false;
            }
            else if (_a as object == null || _b as object == null) {
                return true;
            }
            return _a.m_kind != _b.m_kind;
        }

        public float getHalfHeight() {
            return m_halfHeight;
        }
        public void setHalfHeight(float _halfHeight) {
            m_halfHeight = _halfHeight;
        }

//         public static implicit operator Tag(int _type) {
//             switch (_type) {
//                 case 1:
//                     return Tag.Platform;
//                     break;
//                 case 2:
//                     return Tag.AttachPoint;
//                     break;
//                 default:
//                     return new Tag(_type);
//             }
//         }
    }
}
