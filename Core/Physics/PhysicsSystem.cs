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
            if (FixtureCollisionCategroy.IsPlatform(fixtureA)) {
                platform = fixtureA;
                thing = fixtureB;
            }
            else if (FixtureCollisionCategroy.IsPlatform(fixtureB)) {
                platform = fixtureB;
                thing = fixtureA;
            }
            if (platform != null && thing != null) {
                AABB platformAABB = new AABB();
                platform.GetAABB(out platformAABB, 0);
                Tag thingTag = thing.Body.UserData as Tag;
                float centroidHeight = 0.0f;
                if (thingTag != null) {
                    centroidHeight = thingTag.getHalfHeight();
                }
                if (thing.Body.Position.Y - centroidHeight + 0.05f < platform.Body.Position.Y + platformAABB.Height / 2.0f) {
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

    public class FixtureCollisionCategroy {
        public enum Kind {
            SolidBlock = 0, // cat1
            OnesideBlock,   // cat2
            EnvironmentSensor,  // cat3
            RoleSensor,     // cat4
            Role,           // cat5
            AttachPoint,    // cat6
            AttachPointSensor,  // cat7
        }

        public static void SetCollsionCategroy(Fixture _fixture, Kind _kind){
            switch(_kind){
                case Kind.SolidBlock:
                    _fixture.CollisionCategories = Category.Cat1;
                    _fixture.CollidesWith = Category.All & ~Category.Cat4;
                    break;
                case Kind.OnesideBlock:
                    _fixture.CollisionCategories = Category.Cat2;
                    _fixture.CollidesWith = Category.All & ~Category.Cat4;
                    break;
                case Kind.EnvironmentSensor:
                    _fixture.CollisionCategories = Category.Cat3;
                    _fixture.CollidesWith = Category.Cat1 | Category.Cat2;
                    break;
                case Kind.RoleSensor:
                    _fixture.CollisionCategories = Category.Cat4;
                    _fixture.CollidesWith = Category.Cat5;
                    break;
                case Kind.Role:
                    _fixture.CollisionCategories = Category.Cat5;
                    _fixture.CollidesWith = Category.All & ~ Category.Cat3 & ~Category.Cat5;
                    break;
                case Kind.AttachPoint:
                    _fixture.CollisionCategories = Category.Cat6;
                    _fixture.CollidesWith = Category.Cat7;
                    break;
                case Kind.AttachPointSensor:
                    _fixture.CollisionCategories = Category.Cat7;
                    _fixture.CollidesWith = Category.Cat6;
                    break;
                default:
                    break;
            }
        }

        public static void SetCoolisionCategroyForRole(Body _body, GameObject _gameObject, float _centroidRise) {
            _body.UserData = new Tag(4, _centroidRise, _gameObject);
            SetCollsionCategroy(_body, Kind.Role);
        }

        public static void SetCollsionCategroy(Body _body, Kind _kind) {
            switch (_kind) {
                case Kind.SolidBlock:
                    _body.CollisionCategories = Category.Cat1;
                    _body.CollidesWith = Category.All & ~Category.Cat4;
                    break;
                case Kind.OnesideBlock:
                    _body.CollisionCategories = Category.Cat2;
                    _body.CollidesWith = Category.All & ~Category.Cat4;
                    break;
                case Kind.EnvironmentSensor:
                    _body.CollisionCategories = Category.Cat3;
                    _body.CollidesWith = Category.Cat1 | Category.Cat2;
                    break;
                case Kind.RoleSensor:
                    _body.CollisionCategories = Category.Cat4;
                    _body.CollidesWith = Category.Cat5;
                    break;
                case Kind.Role:
                    _body.CollisionCategories = Category.Cat5;
                    _body.CollidesWith = Category.All & ~Category.Cat3 & ~Category.Cat5;
                    break;
                case Kind.AttachPoint:
                    _body.CollisionCategories = Category.Cat6;
                    _body.CollidesWith = Category.Cat7;
                    break;
                case Kind.AttachPointSensor:
                    _body.CollisionCategories = Category.Cat7;
                    _body.CollidesWith = Category.Cat6;
                    break;
                default:
                    break;
            }
        }

        public static bool IsRole(Fixture _fixture) {
            return ((_fixture.CollisionCategories & Category.Cat5) != 0x0);
        }

        public static bool IsPlatform(Fixture _fixture) {
            return ((_fixture.CollisionCategories & Category.Cat2) != 0x0);
        }

        public static bool IsPlatform(Body _body) {
            if (_body == null || _body.FixtureList.Count < 0) {
                return false;
            }
            else {
                return IsPlatform(_body.FixtureList[0]);
            }
        }

        public static GameObject GetGameObject(Fixture _fixture) {
            if (_fixture != null) {
                return GetGameObject(_fixture.Body);
            }
            return null;
        }

        public static GameObject GetGameObject(Body _body) {
            if (_body != null && _body.UserData != null) {
                Tag tag = _body.UserData as Tag;
                if (tag != null) {
                    return tag.GameObject;
                }
            }
            return null;
        }
    }

    public class Tag {

        

        static readonly public Tag Platform = new Tag(1);
        static readonly public Tag AttachPoint = new Tag(2);
        static readonly public Tag Sensor = new Tag(3);
        static readonly public Tag Role = new Tag(4);

        private int m_kind = 0;
        private float m_halfHeight = 0.0f;
        private GameObject m_gameObject;
        public GameObject GameObject {
            set {
                m_gameObject = value;
            }
            get {
                return m_gameObject;
            }
        }

        public Tag(int _kind, float _halfHeight = 0.0f, GameObject _gameObject = null) {
            m_kind = _kind;
            m_halfHeight = _halfHeight;
            m_gameObject = _gameObject;
        }

        public override bool Equals(object obj) {
            return this == (obj as Tag);
        }

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

        public override int GetHashCode() {
            return base.GetHashCode();
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
