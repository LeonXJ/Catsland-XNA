using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Catsland.Plugin.BasicPlugin;

namespace HorseRiding {
    public class WonderEndlessBlock : EndlessBlock{

#region Properties

        [SerialAttribute]
        private readonly CatVector3 m_randomScale = new CatVector3(0.1f, 0.05f, 0.0f);
        public Vector3 RandomScale{
            set {
                m_randomScale.SetValue(value);
            }
            get {
                return m_randomScale;
            }
        }

        private Random m_random = new Random();

#endregion

        public WonderEndlessBlock() : base() { }
        public WonderEndlessBlock(GameObject _gameObject)
            : base(_gameObject) {

        }

        protected override void PostCreatGameObject(GameObject _gameObject, int _index) {
            _gameObject.Position += m_randomScale *
                new Vector3((float)m_random.NextDouble(),
                            (float)m_random.NextDouble(),
                            (float)m_random.NextDouble());
            // random color
            CatColor color = new CatColor();
            color.SetFromHSV(new Vector4((float)m_random.NextDouble(), 0.9f, 0.8f, 0.0f));
            ModelComponent model = _gameObject.GetComponent(typeof(ModelComponent).ToString())
                as ModelComponent;
            if (model != null) {
                model.GetCatModelInstance().GetMaterial().SetParameter("BiasColor", color);
            }

        }
    }
}
