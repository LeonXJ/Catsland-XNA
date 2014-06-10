using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace HorseRiding {
    public class HorseDestinationSensor : StaticSensor{

        public HorseDestinationSensor() : base() { }
        public HorseDestinationSensor(GameObject _gameObject):
            base(_gameObject) {

        }

        protected override bool Enter(Fixture _fixtureA, Fixture _fixtureB, Contact _contact) {
            // make sure we hit player
            if (_fixtureA.UserData == null && _fixtureB.UserData == null) {
                return true;
            }
            Mgr<GameEngine>.Singleton.DoSwitchScene(
                Mgr<CatProject>.Singleton.GetSceneFileAddress("Wonderland"));
            return true;
        }
    }
}
