using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Catsland.Plugin.BasicPlugin {
    public class Environment : CatComponent{

#region Properties

        // Time
        [SerialAttribute]
        private readonly CatInteger m_dayDurationInS = new CatInteger(10);
        public int DayDurationInS {
            set {
                m_dayDurationInS.SetValue((int)MathHelper.Max(value, 0.0f));
            }
            get {
                return m_dayDurationInS;
            }
        }

        private int m_currentMsInDay = 0;
        public float CurrentTimeInRatio {
            get {
                return (float)m_currentMsInDay / (m_dayDurationInS * 1000);
            }
        }

        // Light
        private Color[] m_ambientLightTable;  
        private Color[] m_diffuseLightTable;

        [SerialAttribute]
        private string m_ambientLightMap;
        public string AmbientLightMap {
            set {
                if (UpdateAmbientLightTable(value)) {
                    m_ambientLightMap = value;
                }
            }
            get {
                return m_ambientLightMap;
            }
        }

        [SerialAttribute]
        private string m_diffuseLightMap;
        public string DiffuseLightMap {
            set {
                if (UpdateDiffuseLightTable(value)) {
                    m_diffuseLightMap = value;
                }
            }
            get {
                return m_diffuseLightMap;
            }
        }

        public Color AmbientColor {
            get {
                if (m_ambientLightTable != null) {
                    return m_ambientLightTable
                        [(int)(m_ambientLightTable.Length * CurrentTimeInRatio) % m_diffuseLightTable.Length];
                }
                return Color.Black;
            }
        }

        public Color DiffuseColor {
            get {
                if (m_diffuseLightTable != null) {
                    return m_diffuseLightTable
                        [(int)(m_diffuseLightTable.Length * CurrentTimeInRatio) % m_ambientLightTable.Length];
                }
                return Color.White;
            }
        }

#endregion

        public Environment(GameObject _gameObject)
            : base(_gameObject) { }

        public Environment() : base() { }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            scene.AddSharedObject(typeof(Environment).ToString(), this);
        }

        public override void Initialize(Catsland.Core.Scene scene) {
            base.Initialize(scene);
            UpdateAmbientLightTable(m_ambientLightMap);
            UpdateDiffuseLightTable(m_diffuseLightMap);
        }

        private bool UpdateAmbientLightTable(string _textureMap) {
            return UpdateColorTable(_textureMap, ref m_ambientLightTable);
        }

        private bool UpdateDiffuseLightTable(string _textureMap) {
            return UpdateColorTable(_textureMap, ref m_diffuseLightTable);
        }

        private bool UpdateColorTable(string _textureName, ref Color[] _table) {
            if (_textureName != null && _textureName != "") {
                Texture2D tex = null;
                try {
                    tex = Mgr<CatProject>.Singleton.contentManger
                        .Load<Texture2D>("image\\" + _textureName);
                    _table = new Color[tex.Width];
                    tex.GetData<Color>(0, new Rectangle(0, 0, tex.Width, 1),
                        _table, 0, tex.Width);
                    return true;
                }
                catch (ContentLoadException) {
                    _table = null;
                }
            }
            else {
                _table = null;
            }
            return false;
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);
            m_currentMsInDay += timeLastFrame;
            m_currentMsInDay %= (m_dayDurationInS * 1000);
        }

        public static string GetMenuNames() {
            return "Shadow|Environment";
        }
            

    }
}
