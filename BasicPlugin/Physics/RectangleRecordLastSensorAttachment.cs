using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin.BasicPlugin {
    public class RectangleRecordLastSensorAttachment : RectangleSensorAttachment {

#region Properties

        private Fixture m_lastContactFixture = null;

#endregion

        public RectangleRecordLastSensorAttachment(Body _body, GameObject _gameObject)
            : base(_body, _gameObject) {
        }

        public Fixture GetLastContactFixture() {
            return m_lastContactFixture;
        }

        protected override bool Collision(Fixture _fixtureA, Fixture _fixtureB, FarseerPhysics.Dynamics.Contacts.Contact _contact) {
            if (_fixtureA == m_fixture) {
                m_lastContactFixture = _fixtureB;
            }
            else {
                m_lastContactFixture = _fixtureA;
            }
            return true;
        }

        protected override void Separation(Fixture _fixtureA, Fixture _fixtureB) {
            if (m_contactCount == 0) {
                m_lastContactFixture = null;
            }
        }
    }
}
