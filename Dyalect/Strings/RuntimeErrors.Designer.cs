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
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class RuntimeErrors {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal RuntimeErrors() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Dyalect.Strings.RuntimeErrors", typeof(RuntimeErrors).Assembly);
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
        ///   Looks up a localized string similar to Division by zero..
        /// </summary>
        internal static string DivideByZero {
            get {
                return ResourceManager.GetString("DivideByZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unhandled exception occured during an execution of an external function &quot;%FunctionName%&quot;: %Error%.
        /// </summary>
        internal static string ExternalFunctionFailure {
            get {
                return ResourceManager.GetString("ExternalFunctionFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Indices of type &quot;%IndexTypeName%&quot; are not supported by the containers of type &quot;%TypeName%&quot;..
        /// </summary>
        internal static string IndexInvalidType {
            get {
                return ResourceManager.GetString("IndexInvalidType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An object of type &quot;%TypeName%&quot; doesn&apos;t support a &quot;call&quot; operation..
        /// </summary>
        internal static string NotFunction {
            get {
                return ResourceManager.GetString("NotFunction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operation &quot;%Operation%&quot; is not supported by the following type(s): &quot;%TypeName%&quot;..
        /// </summary>
        internal static string OperationNotSupported {
            get {
                return ResourceManager.GetString("OperationNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stack is corrupted..
        /// </summary>
        internal static string StackCorrupted {
            get {
                return ResourceManager.GetString("StackCorrupted", resourceCulture);
            }
        }
    }
}