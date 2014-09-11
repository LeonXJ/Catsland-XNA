using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace Catsland.Plugin.BasicPlugin {
    public abstract class RectangleSensorAttachment : SensorAttachmentBase {

#region Properties

        [SerialAttribute]
        protected readonly CatVector2 m_size = new CatVector2(1.0f, 1.0f);
        public Vector2 Size {
            set{
                m_size.SetValue(new Vector2(MathHelper.Max(value.X, 0.0f),
                                            MathHelper.Max(value.Y, 0.0f)));
                UpdateSensor();
                UpdateDebugShape();
            }
            get{
                return m_size;
            }
        }

#endregion

        public RectangleSensorAttachment(Body _body, GameObject _gameObject)
            : base(_body, _gameObject) {

        }

        protected override void UpdateDebugShapeVertex() {
            m_debugShape.SetAsRectangle(m_size, Vector2.Zero);
        }

        protected override Fixture CreateSensor() {
            return FixtureFactory.AttachRectangle(m_size.X, m_size.Y, 0.1f, m_offset, m_body);  
        }

    }
}
