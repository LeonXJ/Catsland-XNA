﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin.BasicPlugin {
    public class LightSensor : CatComponent{

#region Properties

        private DebugShape m_debugShape = new DebugShape();


#endregion

        public LightSensor(GameObject _gameObject)
            :base(_gameObject){
        }

        public LightSensor() : base() { }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            m_debugShape.SetAsCircle(0.2f, Vector2.Zero);
            m_debugShape.RelateGameObject = m_gameObject;
        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_debugShape.BindToScene(scene);
            }
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);
        }

        public override void EditorUpdate(int timeLastFrame) {
            base.EditorUpdate(timeLastFrame);

            if (Mgr<Scene>.Singleton.m_shadowSystem.IsPointEnlighted(
                new Vector2(m_gameObject.AbsPosition.X, m_gameObject.AbsPosition.Y))) {
                m_debugShape.DiffuseColor = Color.Red;
            }
            else {
                m_debugShape.DiffuseColor = Color.Green;
            }
        
        }

        public static string GetMenuNames() {
            return "Shadow|LightSensor";
        }

    }
}