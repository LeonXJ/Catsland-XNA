using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Plugin.BasicPlugin {
    interface IParticleUpdater {
        void ParticleUpdate(int timeLastFrame, Particle particle, ParticleEmitter emitter);
    }
}
