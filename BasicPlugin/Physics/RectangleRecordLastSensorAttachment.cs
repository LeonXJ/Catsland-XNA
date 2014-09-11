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

        private int m_contactCount = 0;
        private Fixture m_lastContactFixture = null;


#endregion

        public RectangleRecordLastSensorAttachment(Body _body, GameObject _gameObject)
            : base(_body, _gameObject) {
        }

        public bool HasContact() {
            return (m_contactCount > 0);
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
            ++m_contactCount;

            if (m_debugShape != null) {
                m_debugShape.DiffuseColor = Color.Red;
            }

            return true;
        }

        protected override void Separation(Fixture _fixtureA, Fixture _fixtureB) {
            --m_contactCount;
            if (m_contactCount == 0) {
                m_lastContactFixture = null;
                m_debugShape.DiffuseColor = Color.Green;
            }
        }
    }
}
