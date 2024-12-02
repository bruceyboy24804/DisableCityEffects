using System;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Game.Simulation;
using Game.UI.InGame;
using Game.Prefabs;
using Unity.Entities;

namespace DisableCityEffects
{
    
    public sealed class Mod : IMod
    {
        
        public static Mod Instance { get; private set; }

       
        internal Setting ActiveSettings { get; private set; }

        
        internal static World ActiveWorld { get; private set; }

      
        public static ILog Log { get; private set; } = LogManager.GetLogger($"{nameof(DisableCityEffects)}.{nameof(Mod)}").SetShowsErrorsInUI(false);

        // Systems managed by the mod
        public static Setting m_Setting;
        public static PrefabSystem _prefabSystem;

       
       
        public void OnLoad(UpdateSystem updateSystem)
        {
            // Set the active instance and world
            Instance = this;
            ActiveWorld = updateSystem.World;

            Log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                Log.Info($"Current mod asset at {asset.path}");

            // Initialize systems
            InitializeSystems(updateSystem);

            // Load and register settings
            m_Setting = new Setting(this);
            if (m_Setting == null)
            {
                Log.Error("Failed to initialize settings.");
                return;
            }
            m_Setting.RegisterInOptionsUI();
            AssetDatabase.global.LoadSettings(nameof(DisableCityEffects), m_Setting, new Setting(this));

            // Load localization
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));

            // Register update phases
            updateSystem.UpdateAt<LocalModifierDataQuery>(SystemUpdatePhase.GameSimulation);

            // Log mod load completion
            Log.Info($"Loaded {nameof(DisableCityEffects)} mod successfully.");
        }

        
        private void InitializeSystems(UpdateSystem updateSystem)
        {
            _prefabSystem = updateSystem.World.GetOrCreateSystemManaged<PrefabSystem>();
        }

        /// <summary>
        /// Called by the game when the mod is disposed of.
        /// </summary>
        public void OnDispose()
        {
            Log.Info(nameof(OnDispose));

            // Unregister and clear settings
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }

            // Clear the instance
            Instance = null;

            Log.Info($"{nameof(DisableCityEffects)} mod disposed successfully.");
        }
    }
}
