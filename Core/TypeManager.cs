using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

/**
 * @file TypeManager
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {
    /**
     * @brief TypeManager load CatsComponent plugins from .dll files and manage them
     * */
    public class TypeManager {
        // dictionary of catComponents
        Dictionary<string, Type> catComponentTypes;
        public Dictionary<string, Type> CatComponents {
            get { return catComponentTypes; }
        }

        Dictionary<string, Type> editorScripts;
        public Dictionary<string, Type> EditorScripts {
            get { return editorScripts; }
        }

        Dictionary<string, Type> consoleCommands = new Dictionary<string, Type>();
        public Dictionary<string, Type> ConsoleCommends {
            get { return consoleCommands; }  
        }

        private Dictionary<string, Type> m_btTreeNodes;
        public Dictionary<string, Type> BTTreeNodes {
            get {
                return m_btTreeNodes;
            }
        }

        // TODO: add more dictionary here
        /**
         * @brief get the type of catComponent. object can be created from type.
         * 
         * @param type_name name of the type
         * 
         * @result type
         * */
        public Type GetCatComponentType(string typeName) {
            if (catComponentTypes != null && catComponentTypes.ContainsKey(typeName)) {
                return catComponentTypes[typeName];
            }
            else {
                return null;
            }
        }

        public Type GetEditorScript(string typeName) {
            if (editorScripts != null && editorScripts.ContainsKey(typeName)) {
                return editorScripts[typeName];
            }
            else {
                return null;
            }
        }

        public Type GetBTTreeNodeType(string _typeName) {
            if (m_btTreeNodes != null && m_btTreeNodes.ContainsKey(_typeName)) {
                return m_btTreeNodes[_typeName];
            }
            else {
                return null;
            }
        }

        /**
         * @brief add a catComponent type to manager
         * 
         * @param type_name the name of the type
         * @param type type
         * */
        public void AddCatComponentType(string typeName, Type type) {
            if (catComponentTypes == null) {
                catComponentTypes = new Dictionary<string, Type>();
            }
            catComponentTypes.Add(typeName, type);
        }

        public void AddEditorScript(string typeName, Type type) {
            if (editorScripts == null) {
                editorScripts = new Dictionary<string, Type>();
            }
            editorScripts.Add(typeName, type);
        }

        /**
         * @brief load plugins from file
         * 
         * @param lib_path the path of the file
         * */
        public void Load_Plugins(string libPath) {
            catComponentTypes = new Dictionary<string, Type>();
            string[] files = Directory.GetFiles(libPath, "*.dll");
            foreach (string file in files) {
                Assembly assembly = Assembly.LoadFrom(file);
//                 Type[] types = assembly.GetTypes();
//                 Console.WriteLine("Load cat component class from: " + assembly.GetName().Name);
//                 foreach (Type type in types) {
//                     Console.WriteLine("- Load class: " + type.Name);
// 
//                     if (type.IsSubclassOf(typeof(CatComponent)) 
//                         && !catComponentTypes.ContainsKey(type.Name)) {
//                         catComponentTypes.Add(type.FullName, type);
//                     }
//                 }
                SearchTypeWithBaseTypeInAssembly(catComponentTypes, assembly, typeof(CatComponent));
            }
        }

        public void Load_EditorScripts(string libPath) {
            editorScripts = new Dictionary<string, Type>();
            string[] files = Directory.GetFiles(libPath, "*.dll");
            foreach (string file in files) {
                Assembly assembly = Assembly.LoadFrom(file);
                SearchTypeWithInterfaceInAssembly(editorScripts, assembly, typeof(IEditorScript));
//                 Type[] types = assembly.GetTypes();
//                 Console.WriteLine("Load editor script class from: " + assembly.GetName().Name);
//                 foreach (Type type in types) {
//                     if (type.GetInterface(typeof(IEditorScript).Name)!=null) {
//                         Console.WriteLine("- Load class: " + type.Name);
//                         editorScripts.Add(type.Name, type);
//                     } 
//                 }
            }
        }

        public void LoadConsoleCommands(string libPath) {
            string[] files = Directory.GetFiles(libPath, "*.dll");
            foreach (string file in files) {
                Assembly assembly = Assembly.LoadFrom(file);
                CatConsole.SearchInAssembly(assembly, consoleCommands);
            }
        }

        public void LoadBTTreeNodes(string libPath) {
            
            m_btTreeNodes = new Dictionary<string, Type>();
            SearchTypeWithBaseTypeInAssembly(m_btTreeNodes, Assembly.GetExecutingAssembly(), typeof(BTNode));
            string[] files = Directory.GetFiles(libPath, "*.dll");
            foreach (string file in files) {
                Assembly assembly = Assembly.LoadFrom(file);
                SearchTypeWithBaseTypeInAssembly(m_btTreeNodes, assembly, typeof(BTNode));
//                 Type[] types = assembly.GetTypes();
//                 Console.WriteLine("Load BTTree nodes class from: " + assembly.GetName().Name);
//                 foreach (Type type in types) {
//                     if (type.IsSubclassOf(typeof(BTNode))) {
//                         Console.WriteLine("- Load class: " + type.Name);
//                         m_btTreeNodes.Add(type.ToString(), type);
//                     }
//                 }
            }
        }

        private static void SearchTypeWithInterfaceInAssembly(Dictionary<string, Type> _dict, Assembly _assembly, Type _type) {
            SearchTypeInAssembly(_dict, _assembly, _type, true);
        }

        private static void SearchTypeWithBaseTypeInAssembly(Dictionary<string, Type> _dict, Assembly _assembly, Type _type) {
            SearchTypeInAssembly(_dict, _assembly, _type, false);
        }

        private static void SearchTypeInAssembly(Dictionary<string, Type> _dict, Assembly _assembly, Type _type, bool _interface = true) {
            Type[] types = _assembly.GetTypes();
            Console.WriteLine("Load class from: " + _assembly.GetName().Name);
            foreach (Type type in types) {
                if ((!_interface && type.IsSubclassOf(_type)) ||
                    (_interface && type.GetInterface(_type.Name) != null)) {
                    Console.WriteLine("- Load class: " + type.Name);
                    _dict.Add(type.ToString(), type);
                }
            }  
        }


    }
}
