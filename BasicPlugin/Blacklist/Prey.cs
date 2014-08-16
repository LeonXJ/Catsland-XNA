﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin.BasicPlugin {
    public class Prey : CatComponent{

        /**
         * @brief prey is added to blacklist by itself. Hunters may search for
         *      preys
         */

        public Prey(GameObject _gameObject)
            : base(_gameObject) {

        }

        public Prey()
            : base() {

        }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            Blacklist blacklist = scene.GetSharedObject(typeof(Blacklist).ToString())
                                    as Blacklist;
            if (blacklist != null) {
                blacklist.AddToBlacklist(this);
            }
        }

        public Vector2 GetPointInWorld() {
            Vector3 absPosition = m_gameObject.AbsPosition;
            return new Vector2(absPosition.X, absPosition.Y);
        }

        public static string GetMenuNames() {
            return "Shadow|Prey";
        }
    }
}
