﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Maps.MapControl.WPF.Core {
    using System;
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ExceptionStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionStrings() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.Maps.MapControl.WPF.Core.ExceptionStrings", typeof(ExceptionStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
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
        ///   Ищет локализованную строку, похожую на Configuration must be loaded before the map loads, and can only be loaded once..
        /// </summary>
        internal static string ConfigurationException_InvalidLoad {
            get {
                return ResourceManager.GetString("ConfigurationException_InvalidLoad", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на XmlReader used to initialize the configuration cannot be null..
        /// </summary>
        internal static string ConfigurationException_NullXml {
            get {
                return ResourceManager.GetString("ConfigurationException_NullXml", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на IProjectable elements can only reside under other IProjectable elements or Map control directly..
        /// </summary>
        internal static string IProjectable {
            get {
                return ResourceManager.GetString("IProjectable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Invalid location..
        /// </summary>
        internal static string LocationToViewportPoint_DefaultException {
            get {
                return ResourceManager.GetString("LocationToViewportPoint_DefaultException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на version and sectionName parameter cannot be empty or null..
        /// </summary>
        internal static string MapConfiguration_GetSection_NonNull {
            get {
                return ResourceManager.GetString("MapConfiguration_GetSection_NonNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Nodes within the same section cannot have the same key &quot;{0}&quot;..
        /// </summary>
        internal static string MapConfiguration_ParseConfiguration_DuplicateNodeKey {
            get {
                return ResourceManager.GetString("MapConfiguration_ParseConfiguration_DuplicateNodeKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Sections cannot have the same sectionName (&quot;{0}&quot;), version (&quot;{1}&quot;) and culture (&quot;{2}&quot;)..
        /// </summary>
        internal static string MapConfiguration_ParseConfiguration_DuplicateSection {
            get {
                return ResourceManager.GetString("MapConfiguration_ParseConfiguration_DuplicateSection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Unsupported root node &quot;{0}&quot; in configuration xml..
        /// </summary>
        internal static string MapConfiguration_ParseConfiguration_InvalidRoot {
            get {
                return ResourceManager.GetString("MapConfiguration_ParseConfiguration_InvalidRoot", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Sections must have version attribution..
        /// </summary>
        internal static string MapConfiguration_ParseConfiguration_InvalidSection_NoVersion {
            get {
                return ResourceManager.GetString("MapConfiguration_ParseConfiguration_InvalidSection_NoVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Unsupported Tag &quot;{0}&quot; in configuration xml..
        /// </summary>
        internal static string MapConfiguration_ParseConfiguration_InvalidTag {
            get {
                return ResourceManager.GetString("MapConfiguration_ParseConfiguration_InvalidTag", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Invalid Result return by MapControlConfiguration service..
        /// </summary>
        internal static string MapConfiguration_WebService_InvalidResult {
            get {
                return ResourceManager.GetString("MapConfiguration_WebService_InvalidResult", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на .
        /// </summary>
        internal static string TileSource_InvalidSubdomain_stringNull {
            get {
                return ResourceManager.GetString("TileSource_InvalidSubdomain_stringNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на .
        /// </summary>
        internal static string TileSource_InvalidSubdomains_DifferentLength {
            get {
                return ResourceManager.GetString("TileSource_InvalidSubdomains_DifferentLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на .
        /// </summary>
        internal static string TileSource_InvalidSubdomains_LengthMoreThan0 {
            get {
                return ResourceManager.GetString("TileSource_InvalidSubdomains_LengthMoreThan0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Invalid type for ApplicationIdCredentialsProvider. Only able to convert strings..
        /// </summary>
        internal static string TypeConverter_InvalidApplicationIdCredentialsProvider {
            get {
                return ResourceManager.GetString("TypeConverter_InvalidApplicationIdCredentialsProvider", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на .
        /// </summary>
        internal static string TypeConverter_InvalidLocationCollection {
            get {
                return ResourceManager.GetString("TypeConverter_InvalidLocationCollection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на .
        /// </summary>
        internal static string TypeConverter_InvalidLocationFormat {
            get {
                return ResourceManager.GetString("TypeConverter_InvalidLocationFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на .
        /// </summary>
        internal static string TypeConverter_InvalidLocationRectFormat {
            get {
                return ResourceManager.GetString("TypeConverter_InvalidLocationRectFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Invalid string or type for MapMode..
        /// </summary>
        internal static string TypeConverter_InvalidMapMode {
            get {
                return ResourceManager.GetString("TypeConverter_InvalidMapMode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на .
        /// </summary>
        internal static string TypeConverter_InvalidPositionOriginFormat {
            get {
                return ResourceManager.GetString("TypeConverter_InvalidPositionOriginFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Invalid format or type for Range. Only able to convert strings with formats, \&quot;{0} {1}\&quot; or \&quot;{0},{1}\&quot; where {0} and {1} are valid doubles or ints..
        /// </summary>
        internal static string TypeConverter_InvalidRangeFormat {
            get {
                return ResourceManager.GetString("TypeConverter_InvalidRangeFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Invalid viewportPoint..
        /// </summary>
        internal static string ViewportPointToLocation_DefaultException {
            get {
                return ResourceManager.GetString("ViewportPointToLocation_DefaultException", resourceCulture);
            }
        }
    }
}
