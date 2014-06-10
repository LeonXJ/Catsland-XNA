using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * @file Mgr
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {
    /**
     * @brief the template class for singleton, enable accessing globally
     * 
     * example: 
     * Mgr<ExmapleClass>.Singleton = new ExampleClass();
     * so in anywhere, you can access it by:
     * ExampleClass exampleClass = Mgr<ExampleClass>.Singleton;
     * */
    public class Mgr<typename> {
        static typename singleton;
        public static typename Singleton {
            set {
                singleton = value;
            }
            get {
                if (singleton == null) {
                    Console.WriteLine("Null singleton");
                }
                return singleton;
            }
        }
    }
}
