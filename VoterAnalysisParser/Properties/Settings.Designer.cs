﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VoterAnalysisParser.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.7.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://y384o59d2e.execute-api.us-west-2.amazonaws.com/prod")]
        public string URL_Prod {
            get {
                return ((string)(this["URL_Prod"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("u5HPTwm77q2VxbXRUS6KG3UiKlR0FJEd48CTCoJk")]
        public string api_key_Prod {
            get {
                return ((string)(this["api_key_Prod"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://c59epy4bbl.execute-api.us-east-1.amazonaws.com/dev/edspost")]
        public string URL_QA {
            get {
                return ((string)(this["URL_QA"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("213lxJEq7m1bkcmT2w34x1kj1KCep2j9ES91nrBj")]
        public string api_key_QA {
            get {
                return ((string)(this["api_key_QA"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=enygdb1;Initial Catalog=ElectionProd;Persist Security Info=True;User " +
            "ID=gfxuser;Password=elect2018")]
        public string dbConn_QA {
            get {
                return ((string)(this["dbConn_QA"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=enygdb1;Initial Catalog=ElectionProd;Persist Security Info=True;User " +
            "ID=gfxuser;Password=elect2018")]
        public string dbConn_Prod {
            get {
                return ((string)(this["dbConn_Prod"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ProdMode {
            get {
                return ((bool)(this["ProdMode"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://c59epy4bbl.execute-api.us-east-1.amazonaws.com/dev/edspost")]
        public string URL_Dev {
            get {
                return ((string)(this["URL_Dev"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("213lxJEq7m1bkcmT2w34x1kj1KCep2j9ES91nrBj")]
        public string api_key_Dev {
            get {
                return ((string)(this["api_key_Dev"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://0tkvnhl1q3.execute-api.us-east-2.amazonaws.com/stg/edspost")]
        public string URL_Stg {
            get {
                return ((string)(this["URL_Stg"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("pZXVLzYcvP3avlnH1n6ND9NCtLWMbivOaOhzLD8k")]
        public string api_key_Stg {
            get {
                return ((string)(this["api_key_Stg"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2020_General")]
        public string electionEvent {
            get {
                return ((string)(this["electionEvent"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("stg")]
        public string useURL {
            get {
                return ((string)(this["useURL"]));
            }
        }
    }
}
