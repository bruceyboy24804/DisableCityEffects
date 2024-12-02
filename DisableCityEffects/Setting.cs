using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Buildings;
using Game.Modding;
using Game.Prefabs;
using Game.Settings;
using Game.Simulation;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;
using Unity.Entities;

namespace DisableCityEffects
{
    [FileLocation(nameof(DisableCityEffects))]
    [SettingsUITabOrder(MainTab, ParametersTab)]
    [SettingsUIGroupOrder(LocalModifierGroup, AdvancedGroup)]
    [SettingsUIShowGroupName(LocalModifierGroup, AdvancedGroup)]
    public class Setting : ModSetting
    {
        // Static fields
        public static World World { get; set; }

        // Constants
        public const string MainTab = "Main";
        public const string ParametersTab = "Parameters";
        public const string LocalModifierGroup = "Local Modifier Settings";
        public const string AdvancedGroup = "Advanced Settings";

        

        // Private fields
        private bool _ResetLocalModifierDataButton;
        private bool _SetModifierDataToZeroButton;

        // Constructor
        public Setting(IMod mod) : base(mod)
        {
        }

        // Properties
        [SettingsUISection(MainTab, LocalModifierGroup)]
        [SettingsUIButton]
        public bool SetModifierDataToZeroButton
        {
            get => _SetModifierDataToZeroButton;
            set
            {
                _SetModifierDataToZeroButton = value;
                if (value)
                {
                    LocalModifierDataQuery modiferQuery = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<LocalModifierDataQuery>();
                    modiferQuery.SetModifierDataToZero();
                }
            }
        }

        [SettingsUISection(MainTab, LocalModifierGroup)]
        [SettingsUIButton]
        public bool ResetLocalModifierDataButton
        {
            get => _ResetLocalModifierDataButton;
            set
            {
                _ResetLocalModifierDataButton = value;
                if (value)
                {
                    LocalModifierDataQuery modiferQuery = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<LocalModifierDataQuery>();
                    modiferQuery.ResetLocalModifierData();
                }
            }
        }

        // Methods
        public override void SetDefaults()
        {
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Disable City Effects" },
                { m_Setting.GetOptionTabLocaleID(nameof(Setting.MainTab)), "Main" },
                { m_Setting.GetOptionTabLocaleID(nameof(Setting.ParametersTab)), "Parameters" },

                // Group Labels
                { m_Setting.GetOptionGroupLocaleID(nameof(Setting.LocalModifierGroup)), "Local Modifier Settings" },
                { m_Setting.GetOptionGroupLocaleID(nameof(Setting.AdvancedGroup)), "Advanced Settings" },

                // Button Labels
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetLocalModifierDataButton)), "Reset Local Modifiers" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetLocalModifierDataButton)), "Reset local modifiers to their original values" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SetModifierDataToZeroButton)), "Set Modifiers to Zero" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SetModifierDataToZeroButton)), "Set all local modifiers to zero" }
            };
        }

        public void Unload() { }
    }
}
