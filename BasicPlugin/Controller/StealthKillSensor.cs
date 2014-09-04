using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Catsland.Core;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;
using FarseerPhysics.Factories;

namespace Catsland.Plugin.BasicPlugin {
    public class StealthKillSensor {

#region Properties

        [SerialAttribute]
        protected readonly CatVector2 m_offset = new CatVector2();
        public Vector2 Offset {
            set {
                m_offset.SetValue(value);
                UpdateSensor();
                UpdateDebugVertex();
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
                m_radius.SetValue(MathHelper.Max(0.0f, value));
                UpdateSensor();
                UpdateDebugVertex();
            }
        }

        [SerialAttribute]
        private bool m_enable = true;

        private DebugShape m_debugShape;

        protected Body m_body;
        protected Fixture m_fixture;
        private HashSet<GameObject> m_suspectGameObject = new HashSet<GameObject>();

        private GameObject m_toBeKilled;
        public GameObject ToBeKilled {
            get {
                return m_toBeKilled;
            }
        }

        private GameObject m_gameObject;

#endregion

        public StealthKillSensor(GameObject _gameObject, Body _body){
            m_body = _body;
            UpdateSensor();
            m_gameObject = _gameObject;
        }

        public void UnbindFromScene(Scene _scene) {
            if (m_fixture != null) {
                PhysicsSystem physicsSystem = _scene.GetPhysicsSystem();
                m_body.DestroyFixture(m_fixture);
            }
        }

        public void BindToScene(Scene _scene) {
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
//                 m_debugShape = new DebugShape();
//                 m_debugShape.
//                 m_debugShape.BindToScene(_scene);
            }
        }

        public void UpdateSensor(){
            if(m_fixture != null){
                PhysicsSystem physicsSystem = Mgr<Scene>.Singleton.GetPhysicsSystem();
                m_body.DestroyFixture(m_fixture);
            }
            m_fixture = FixtureFactory.AttachCircle(m_radius, 0.0f, m_body, m_offset);
            m_fixture.IsSensor = true;
            m_fixture.OnCollision += OnCollision;
            m_fixture.OnSeparation += OnSeparation;
            m_fixture.CollisionGroup = -1;
        }

        public void UpdateDebugVertex() {
            //m_debugShape.SetAsCircle(m_radius, m_offset);
        }

        protected bool OnCollision(Fixture _fixtureA, Fixture _fixtureB, Contact _contact) {
            Fixture other = _fixtureA;
            if (_fixtureA == m_fixture) {
                other = _fixtureB;
            }
            if (other.Body != null && other.Body.UserData != null) {
                Tag tag = other.Body.UserData as Tag;
                if(tag.GameObject != null && 
                    tag.GameObject.GetComponent(typeof(CanBeStealthKilled)) != null){
                    // can be stealth killed
                    m_suspectGameObject.Add(tag.GameObject);
                }
            }
            return true;
        }

        protected void OnSeparation(Fixture _fixtureA, Fixture _fixtureB) {
            Fixture other = _fixtureA;
            if (_fixtureA == m_fixture) {
                other = _fixtureB;
            }
            if (other.Body != null && other.Body.UserData != null) {
                Tag tag = other.Body.UserData as Tag;
                if (tag.GameObject != null 
                    && m_suspectGameObject.Contains(tag.GameObject)) {
                    m_suspectGameObject.Remove(tag.GameObject);
                }
            }
            return;
        }

        public void Update(int _timeLastFrame) {
            m_toBeKilled = null;
            float nearest = 99999.0f;
            if (m_suspectGameObject != null) {
                foreach (GameObject candidate in m_suspectGameObject) {
                    Vector2 myPosition = new Vector2(m_gameObject.AbsPosition.X, m_gameObject.AbsPosition.Y);
                    Vector2 canPosition = new Vector2(candidate.AbsPosition.X, candidate.AbsPosition.Y);
                    Vector2 delta = canPosition - myPosition;
                    // orientation
                    if (m_gameObject.AbsRotation.Y < MathHelper.ToRadians(10.0f) && m_gameObject.AbsRotation.Y > -MathHelper.ToRadians(10.0f)) {
                        if (delta.X < 0.0f) {  // right
                           continue;
                        }
                    }
                    else if (delta.X > 0.0f) {  // left
                        continue;
                    }
                    // raycast
                    bool blocked = false;
                    List<Fixture> fixtures = Mgr<Scene>.Singleton.GetPhysicsSystem().GetWorld().RayCast(myPosition, canPosition);
                    foreach (Fixture fixture in fixtures) {
                        if (fixture.Body.UserData == null) {
                            blocked = true;
                            break;
                        } 
                        else {
                            Tag tag = fixture.Body.UserData as Tag;
                            if (tag != Tag.Role) {
                                blocked = true;
                                break;
                            }
                        }
                    }
                    if (blocked) {
                        continue;
                    }
                    float dist = (myPosition - canPosition).LengthSquared();
                    if (dist < nearest) {
                        nearest = dist;
                        m_toBeKilled = candidate;
                    }
                }
            }
        }


    }
}
