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

        // TODO: add more dictionary here
        /**
         * @brief get the type of catComponent. object can be created from type.
         * 
         * @param type_name name of the type
         * 
         * @result type
         * */
        public Type GetCatComponentType(string typeName) {
            if (catComponentTypes != null) {
                return catComponentTypes[typeName];
            }
            else {
                return null;
            }
        }

        public Type GetEditorScript(string typeName) {
            if (editorScripts != null) {
                return editorScripts[typeName];
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
                Type[] types = assembly.GetTypes();
                Console.WriteLine("Load cat component class from: " + assembly.GetName().Name);
                foreach (Type type in types) {
                    Console.WriteLine("- Load class: " + type.Name);

                    if (type.IsSubclassOf(typeof(CatComponent)) 
                        && !catComponentTypes.ContainsKey(type.Name)) {
                        catComponentTypes.Add(type.FullName, type);
                    }
                }
            }
        }

        public void Load_EditorScripts(string libPath) {
            editorScripts = new Dictionary<string, Type>();
            string[] files = Directory.GetFiles(libPath, "*.dll");
            foreach (string file in files) {
                Assembly assembly = Assembly.LoadFrom(file);
                Type[] types = assembly.GetTypes();
                Console.WriteLine("Load editor script class from: " + assembly.GetName().Name);
                foreach (Type type in types) {
                    if (type.GetInterface(typeof(IEditorScript).Name)!=null) {
                        Console.WriteLine("- Load class: " + type.Name);
                        editorScripts.Add(type.Name, type);
                    } 
                }
            }
        }
    }
}
