using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Plugin.BasicPlugin {
    public class TestEchoCommand : IConsoleCommand{

        private string m_str;
        
        public string GetCommandName() {
            return "echo";
        }

        public void ParseStringParameter(String _parameters) {
            m_str = _parameters;
        }

        public object Execute() {
            Console.Out.WriteLine(m_str);
            ContentManager man = Mgr<CatProject>.Singleton.contentManger;
            string back = man.RootDirectory;
            man.Unload();
            Mgr<CatProject>.Singleton.contentManger.RootDirectory = m_str;
            
            string[] files = Directory.GetFiles(m_str);
            foreach (string file in files) {
                string name = Path.GetFileNameWithoutExtension(file);
                Texture2D tex = man.Load<Texture2D>(name);
                FileStream fs = new FileStream("D:\\A\\" + name + ".png", FileMode.Create);
                tex.SaveAsPng(fs, tex.Width, tex.Height);
                fs.Close();
            }
            man.RootDirectory = back;

            return "Echo says: " + m_str;
        }
    }
}
