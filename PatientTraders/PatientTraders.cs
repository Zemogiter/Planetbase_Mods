using PlanetbaseModUtilities;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace PatientTraders
{
    [DrawFields(DrawFieldMask.Public)]
    public class TimesSettings
    {
        public static float newStayTime = 560f;
        public static float newTradeTime = 6800f;
        public static float newStateTime = 0f;
    }    
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Settings", Collapsible = true)] public TimesSettings TimesSettings = new();

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnChange()
        {
        }
    }
    public class PatientTraders : ModBase
    {

        public static new void Init(ModEntry modEntry) => InitializeMod(new PatientTraders(), modEntry, "PatientTraders");

        public override void OnInitialized(ModEntry modEntry)
        {
            
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here
        }
        
    }
}
