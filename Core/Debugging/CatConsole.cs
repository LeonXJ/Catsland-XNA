using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Catsland.Core {

    public interface IConsoleCommand {
        string GetCommandName();
        void ParseStringParameter(String _parameters);
        object Execute();
    }

    public class CommendPanelPair {
#region Properties

        private IConsoleCommand m_commend;
        public IConsoleCommand Commend {
            get {
                return m_commend;
            }
            set {
                m_commend = value;
            }
        }
        private IConsolePanel m_panel;
        public IConsolePanel Panel {
            set {
                m_panel = value;
            }
            get {
                return m_panel;
            }
        }

#endregion

        public CommendPanelPair(IConsoleCommand _commend,
                                IConsolePanel _panel) {
             m_commend = _commend;
             m_panel = _panel;
        }
    }

    public class CatConsole {

#region Properties

        private Queue<CommendPanelPair> m_commendQueue;
        static private TypeManager typeManager;
        static Dictionary<String, Type> basicCommand;

#endregion

        public CatConsole() {
            m_commendQueue = new Queue<CommendPanelPair>();
            InitBasicCommend();
        }

        public void SetTypeManager(TypeManager _typeManager) {
            typeManager = _typeManager;
        }

        private void InitBasicCommend() {
            basicCommand = new Dictionary<String, Type>();
            SearchInAssembly(Assembly.GetExecutingAssembly(),
                basicCommand);
        }

        static public void SearchInAssembly(Assembly _assembly, Dictionary<string, Type> _commandDict) {
            Type[] types = _assembly.GetTypes();
            Console.Out.WriteLine("Load console commands class from: " + _assembly.GetName().Name);
            foreach (Type type in types) {
                if (type.GetInterface(typeof(IConsoleCommand).Name) != null) {
                    Console.WriteLine("- Load class: " + type.Name);

                    ConstructorInfo constructorInfo = type.GetConstructor(new Type[0] { });
                    IConsoleCommand consoleCommend = constructorInfo.Invoke(new Object[0] { })
                        as IConsoleCommand;
                    _commandDict.Add(consoleCommend.GetCommandName(), type);
                }
            }
        }

        public void PushCommend(IConsoleCommand _commend, 
                                IConsolePanel _panel) {
            m_commendQueue.Enqueue(new CommendPanelPair(_commend, _panel));
        }

        public static IConsoleCommand IntepreteCommendString(string _str) {

            int firstSpaceIndex = _str.IndexOf(' ');
            string command = "";
            string parameterStr = "";
            if (firstSpaceIndex < 0) {
                command = _str;
            }
            else {
                command = _str.Substring(0, firstSpaceIndex);
                parameterStr = _str.Substring(firstSpaceIndex).Trim();
            }
            if (basicCommand.ContainsKey(command)) {
                Type type = basicCommand[command];
                return InstantiateCommandType(type, parameterStr);
            }
            else if (typeManager.ConsoleCommends.ContainsKey(command)) {
                Type type = typeManager.ConsoleCommends[command];
                return InstantiateCommandType(type, parameterStr);
            }
            Console.Out.WriteLine("Cannot interpreted string: " + _str);
            return null;
        }

        private static IConsoleCommand InstantiateCommandType(Type _type, String _parameterStr) {
            ConstructorInfo constructorInfo = _type.GetConstructor(new Type[0] { });
            IConsoleCommand consoleCommand = constructorInfo.Invoke(new Object[0] { })
                as IConsoleCommand;
            consoleCommand.ParseStringParameter(_parameterStr);
            return consoleCommand;
        }

        public void Update() {
            while (m_commendQueue.Count > 0) {
                CommendPanelPair commendPanel = m_commendQueue.Dequeue();
                commendPanel.Panel.GetResult(commendPanel.Commend.Execute());
            }
        }
    }
}
