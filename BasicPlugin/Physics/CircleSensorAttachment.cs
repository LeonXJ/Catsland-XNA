using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace Catsland.Plugin.BasicPlugin {
    public abstract class CircleSensorAttachment : SensorAttachmentBase{

#region Properties

        [SerialAttribute]
        protected readonly CatFloat m_radius = new CatFloat(1.0f);
        public float Radius {
            get {
                return m_radius;
            }
            set {
                m_radius.SetValue(MathHelper.Max(0.0f, value));
                UpdateSensor();
                UpdateDebugShape();
            }
        }

#endregion

        public CircleSensorAttachment(Body _body, GameObject _gameObject)
            : base(_body, _gameObject) {

        }

        protected override void UpdateDebugShapeVertex() {
            m_debugShape.SetAsCircle(m_radius, Vector2.Zero);
        }

        protected override Fixture CreateSensor() {
            return FixtureFactory.AttachCircle(m_radius, 0.0f, m_body);
        }
    }
}
