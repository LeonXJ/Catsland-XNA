using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Catsland.Plugin.BasicPlugin;

namespace HorseRiding {
    public class WonderLightSensor : StaticSensor {

        public WonderLightSensor() : base() { }
        public WonderLightSensor(GameObject _gameObject)
            : base(_gameObject) {

        }

        protected override bool Enter(Fixture _fixtureA, Fixture _fixtureB, Contact _contact) {
            if (_fixtureA.UserData == null && _fixtureB.UserData == null) {
                return true;
            }
            ModelComponent modelComponent =
                m_gameObject.GetComponent(typeof(ModelComponent).ToString())
                as ModelComponent;
            if (modelComponent != null) {
                MotionDelegator motionDelegator = Mgr<CatProject>.Singleton.MotionDelegator;
                MovieClip movieClip = motionDelegator.AddMovieClip();
                movieClip.AppendMotion(
                    modelComponent.GetCatModelInstance().GetMaterial().GetParameter("Alpha"), 
                    new CatFloat(0.7f), 500);
                movieClip.Initialize();
            }
            return true;
        }

        protected override void Exit(Fixture _fixtureA, Fixture _fixtureB) {
            if (_fixtureA.UserData == null && _fixtureB.UserData == null) {
                return;
            }
            ModelComponent modelComponent =
                m_gameObject.GetComponent(typeof(ModelComponent).ToString())
                as ModelComponent;
            if (modelComponent != null) {
                MotionDelegator motionDelegator = Mgr<CatProject>.Singleton.MotionDelegator;
                MovieClip movieClip = motionDelegator.AddMovieClip();
                movieClip.AppendMotion(
                    modelComponent.GetCatModelInstance().GetMaterial().GetParameter("Alpha"),
                    new CatFloat(0.0f), 10000);
                movieClip.Initialize();
            }
        }
    }
}
