using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Xml;
using Catsland.Plugin.BasicPlugin;

namespace Catsland.Plugin.BirthdayParty {
    public class ScoreCounter : CatComponent {
        public int score = 0;

        public ScoreCounter(GameObject gameObject)
            : base(gameObject) { }

        public override void Initialize(Catsland.Core.Scene scene) {
            score = 0;
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement counter = doc.CreateElement(typeof(ScoreCounter).Name);
            node.AppendChild(counter);
            return true;
        }
    }
}
