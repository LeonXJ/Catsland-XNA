using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin.BasicPlugin {
    public class StealthKillSensorAttachment : CircleSensorAttachment{

#region Properties

        private HashSet<GameObject> m_suspectGameObject = new HashSet<GameObject>();
        private GameObject m_toBeKilled;

        private new SensorCollisionCategroy CollisionCategroy {
            set {
            }
            get {
                return (SensorCollisionCategroy)(m_collisionCategroy.GetValue());
            }
        }
        
#endregion

        public StealthKillSensorAttachment(Body _body, GameObject _gameObject)
            : base(_body, _gameObject) {
        }

        protected override Fixture CreateSensor() {
            Fixture newFixture = base.CreateSensor();
            m_collisionCategroy.SetValue(
                (int)(SensorAttachmentBase.SensorCollisionCategroy.RoleSensor));
            return newFixture;
        }

        public GameObject GetCandidate() {
            return m_toBeKilled;
        }

        protected override bool Collision(Fixture _fixtureA, Fixture _fixtureB, FarseerPhysics.Dynamics.Contacts.Contact _contact) {
            Fixture other = _fixtureA;
            if (m_fixture == _fixtureA) {
                other = _fixtureB;
            }
            GameObject otherGameObject = FixtureCollisionCategroy.GetGameObject(other);
            if (otherGameObject != null &&
                otherGameObject.GetComponent(typeof(CanBeStealthKilled)) != null) {
                m_suspectGameObject.Add(otherGameObject);
            }
            return true;
        }

        protected override void Separation(Fixture _fixtureA, Fixture _fixtureB) {
            Fixture other = _fixtureA;
            if (_fixtureA == m_fixture) {
                other = _fixtureB;
            }
            GameObject otherGameObject = FixtureCollisionCategroy.GetGameObject(other);
            if (otherGameObject != null
                    && m_suspectGameObject.Contains(otherGameObject)) {
                m_suspectGameObject.Remove(otherGameObject);
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
                    if (m_gameObject.AbsRotationInDegreee.Y < 10.0f && m_gameObject.AbsRotationInDegreee.Y > -10.0f) {
                        if (delta.X < 0.0f) {  // right
                            continue;
                        }
                    }
                    else if (delta.X > 0.0f) {  // left
                        continue;
                    }
                    // raycast
                    bool blocked = false;
                    if (delta.LengthSquared() > 0.01f) {    // avoid myPosition == canPosition
                        List<Fixture> fixtures = Mgr<Scene>.Singleton.GetPhysicsSystem().GetWorld().RayCast(myPosition, canPosition);
                        foreach (Fixture fixture in fixtures) {
                            if (fixture.Body.UserData == null) {
                                blocked = true;
                                break;
                            }
                            else {
                                if (!FixtureCollisionCategroy.IsRole(fixture)) {
                                    blocked = true;
                                    break;
                                }
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
