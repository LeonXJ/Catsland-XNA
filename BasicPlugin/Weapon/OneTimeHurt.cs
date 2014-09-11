using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;

namespace Catsland.Plugin.BasicPlugin {
    public class OneTimeHurt : CatComponent {

        #region Properties

        [SerialAttribute]
        protected readonly CatVector2 m_offset = new CatVector2();
        public Vector2 Offset {
            set {
                m_offset.SetValue(value);
                UpdateSensor();
            }
            get {
                return m_offset.GetValue();
            }
        }

        [SerialAttribute]
        protected readonly CatFloat m_radius = new CatFloat(1.0f);
        public float Radius {
            get {
                return m_radius;
            }
            set {
                m_radius.SetValue(MathHelper.Max(0.1f, value));
                UpdateSensor();
            }
        }

        [SerialAttribute]
        protected readonly CatInteger m_damage = new CatInteger(1);
        public int Damage {
            set {
                m_damage.SetValue(value);
            }
            get {
                return m_damage;
            }
        }

        protected Body m_body;

        [SerialAttribute]
        protected string m_belongToGUID;
        public string BelongGUID {
            set {
                m_belongToGUID = value;
            }
            get {
                return m_belongToGUID;
            }
        }

        private DebugShape m_debugShape;

        #endregion

        public OneTimeHurt(GameObject _gameObject)
            : base(_gameObject) {
        }

        public OneTimeHurt()
            : base() { }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            m_debugShape = new DebugShape();
            m_debugShape.BindToScene(scene);
            m_debugShape.RelateGameObject = m_gameObject;
            UpdateSensor();
        }

        public override void Initialize(Catsland.Core.Scene scene) {
            base.Initialize(scene);
        }

        public override void EditorUpdate(int timeLastFrame) {
            base.EditorUpdate(timeLastFrame);
            m_gameObject.ForceUpdateAbsTransformation();
            m_body.Position = new Vector2(m_gameObject.AbsPosition.X, m_gameObject.AbsPosition.Y)
                  + m_offset;
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);
            m_body.Position = new Vector2(m_gameObject.AbsPosition.X, m_gameObject.AbsPosition.Y)
                  + m_offset;
        }


        protected void UpdateSensor() {
            PhysicsSystem physicsSystem = m_gameObject.Scene.GetPhysicsSystem();
            if (physicsSystem == null) {
                return;
            }
            if (m_body != null) {
                physicsSystem.GetWorld().RemoveBody(m_body);
                m_body = null;
            }
            m_body = BodyFactory.CreateCircle(physicsSystem.GetWorld(), m_radius,
                0.1f, new Tag(3, 0.0f, m_gameObject));
            FixtureCollisionCategroy.SetCollsionCategroy(m_body, FixtureCollisionCategroy.Kind.RoleSensor);
            m_body.BodyType = BodyType.Static;
            m_body.IsSensor = true;
            m_body.CollisionGroup = -1;
            m_body.OnCollision += OnCollision;
            m_debugShape.SetAsCircle(m_radius, m_offset);
        }

        protected bool OnCollision(Fixture _fixtureA, Fixture _fixtureB, Contact _contact) {
            Fixture other = _fixtureA;
            if (_fixtureA.Body == m_body) {
                other = _fixtureB;
            }
            GameObject otherGameObject = FixtureCollisionCategroy.GetGameObject(other);
            if (otherGameObject != null &&
                    otherGameObject.GetComponent(typeof(Vulnerable)) != null) {
                if (m_belongToGUID == "" || m_belongToGUID != otherGameObject.GUID) {
                    Vulnerable vulnerable = otherGameObject.GetComponent(typeof(Vulnerable))
                        as Vulnerable;
                    vulnerable.GetHurt(m_damage);
                }

            }

            return true;
        }

        public override void UnbindFromScene(Scene _scene) {
            base.UnbindFromScene(_scene);
            PhysicsSystem physicsSystem = m_gameObject.Scene.GetPhysicsSystem();
            if (physicsSystem == null) {
                return;
            }
            if (m_body != null) {
                physicsSystem.GetWorld().RemoveBody(m_body);
                m_body = null;
            }
            if (m_debugShape != null) {
                m_debugShape.Destroy(_scene);
            }
        }

        public static string GetMenuNames() {
            return "Weapon|One Time Hurt";
        }
    }
}
