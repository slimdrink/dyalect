﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dyalect.Strings {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class LinkerErrors {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal LinkerErrors() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Dyalect.Strings.LinkerErrors", typeof(LinkerErrors).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ошибка при загрузке модуля {0} из сборки {1}: {2}.
        /// </summary>
        internal static string AssemblyModuleLoadError {
            get {
                return ResourceManager.GetString("AssemblyModuleLoadError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Модуль {0} не определён в сборке {1}..
        /// </summary>
        internal static string AssemblyModuleNotFound {
            get {
                return ResourceManager.GetString("AssemblyModuleNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Дублирующееся имя модуля {1}  в сборке {0}..
        /// </summary>
        internal static string DuplicateModuleName {
            get {
                return ResourceManager.GetString("DuplicateModuleName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Неподдерживаемый тип модуля {0} из сборки {1}..
        /// </summary>
        internal static string InvalidAssemblyModule {
            get {
                return ResourceManager.GetString("InvalidAssemblyModule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не найден модуль {0}..
        /// </summary>
        internal static string ModuleNotFound {
            get {
                return ResourceManager.GetString("ModuleNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ошибка при загрузке сборки {0}: {1}.
        /// </summary>
        internal static string UnableLoadAssembly {
            get {
                return ResourceManager.GetString("UnableLoadAssembly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ошибка при чтении файла модуля: {0}.
        /// </summary>
        internal static string UnableReadModule {
            get {
                return ResourceManager.GetString("UnableReadModule", resourceCulture);
            }
        }
    }
}