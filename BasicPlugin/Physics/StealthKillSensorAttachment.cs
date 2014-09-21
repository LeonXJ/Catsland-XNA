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

        private HashSet<GameObject> m_candidateVictims = new HashSet<GameObject>();
        private GameObject m_victim = null;

        private new SensorCollisionCategroy CollisionCategroy {
            get {
                return (SensorCollisionCategroy)(m_collisionCategroy.GetValue());
            }
        }
        
#endregion

        public StealthKillSensorAttachment(Body _body, GameObject _gameObject)
            : base(_body, _gameObject) {
        }

        public GameObject GetVictim() {
            return m_victim;
        }

        /**
         * @brief calculate the candidate. Call this in every update loop.
         **/ 
        public void Update(int _timeLastFrame) {
            m_victim = null;
            float nearest = float.MaxValue;
            if (m_candidateVictims != null) {
                foreach (GameObject candidate in m_candidateVictims) {
                    Vector2 myPosition = new Vector2(m_gameObject.AbsPosition.X, m_gameObject.AbsPosition.Y);
                    Vector2 canPosition = new Vector2(candidate.AbsPosition.X, candidate.AbsPosition.Y);
                    Vector2 delta = canPosition - myPosition;
                    if (!CheckOrientationForExecution(delta)) {
                        continue;
                    }
                    // do raycast to check if there's anything in the way
                    bool blocked = false;
                    if (delta.LengthSquared() > float.Epsilon) {    // avoid myPosition == canPosition
                        List<Fixture> fixtures = m_gameObject.Scene.GetPhysicsSystem().GetWorld().RayCast(myPosition, canPosition);
                        foreach (Fixture fixture in fixtures) {
                            if (CanObjectBlockStealthKill(fixture)) {
                                blocked = true;
                                break;
                            }
                        }
                    }
                    if (blocked) {
                        continue;
                    }
                    // find the nearest
                    float dist = (myPosition - canPosition).LengthSquared();
                    if (dist < nearest) {
                        nearest = dist;
                        m_victim = candidate;
                    }
                }
            }
        }

        protected override Fixture CreateSensor() {
            Fixture newFixture = base.CreateSensor();
            m_collisionCategroy.SetValue(
                (int)(SensorAttachmentBase.SensorCollisionCategroy.RoleSensor));
            return newFixture;
        }

        protected override bool Collision(Fixture _fixtureA, Fixture _fixtureB, FarseerPhysics.Dynamics.Contacts.Contact _contact) {
            Fixture other = _fixtureA;
            if (m_fixture == _fixtureA) {
                other = _fixtureB;
            }
            GameObject otherGameObject = FixtureCollisionCategroy.GetGameObject(other);
            if (otherGameObject != null &&
                otherGameObject.GetComponent(typeof(CanBeStealthKilled)) != null) {
                m_candidateVictims.Add(otherGameObject);
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
                    && m_candidateVictims.Contains(otherGameObject)) {
                m_candidateVictims.Remove(otherGameObject);
            }
            return;
        }

        /**
         * @brief can killer executes killing in current orientation
         * 
         * @param _delta the vector2 from kill to candidate victim
         **/ 
        private bool CheckOrientationForExecution(Vector2 _delta) {
            if (Math.Abs(m_gameObject.AbsRotationInDegreee.Y) < 90.0f) {
                if (_delta.X < 0.0f) {  // kill faces to right but candidate is on the left
                    return false;
                }
            }
            else if (_delta.X > 0.0f) {  // kill faces to left but candidate is on the right
                return false;
            }
            return true;
        }

        /**
         * @brief if the _fixture is between killer and candidate victim, can the
         *  candidate be executed?
         **/ 
        private bool CanObjectBlockStealthKill(Fixture _fixture) {
            return (FixtureCollisionCategroy.IsSolidBlock(_fixture)
                 || FixtureCollisionCategroy.IsRole(_fixture));
        }
    }
}
