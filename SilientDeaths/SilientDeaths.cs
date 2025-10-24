using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace SilientDeaths
{
    public class Settings : ModSettings, IDrawable
    {
        [Draw("Play death sounds inside base? False will mean all three groups will be silient upon death.")] public bool deathSoundInterior = true;
        [Draw("Play death sounds outside base? False will mean all three groups will be silient upon death.")] public bool deathSoundExterior = true;
        [Draw("Play death sounds for colonists?")] public bool deathSoundC = true;
        [Draw("Play death sounds for visitors?")] public bool deathSoundV = false;
        [Draw("Play death sounds for intruders?")] public bool deathSoundI = false;
        public override void Save(ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class SilientDeaths : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public new static void Init(ModEntry modEntry)
        {
            settings = ModSettings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new SilientDeaths(), modEntry, "SilientDeaths");
        }
        static void OnGUI(ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        static bool OnToggle(ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

        public override void OnInitialized(ModEntry modEntry)
        {
            //nothing needed here
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //NOTHING NEEDED HERE
        }
    }
    public class CustomHuman : Human
    {
        protected override void setKo()
        {
            base.setKo();
            var gender = CoreUtils.GetMember<Human,Gender>("mGender", this);
            if (mLocation == Location.Interior && SilientDeaths.settings.deathSoundInterior)
            {
                if (gender == Gender.Male)
                {
                    if (this.getSpecialization() != TypeList<Specialization, SpecializationList>.find<Intruder>() && SilientDeaths.settings.deathSoundI)
                    {
                        playSound(SoundList.getInstance().HumanInteriorMaleDeath);
                    }
                    else if (this.getSpecialization() != TypeList<Specialization, SpecializationList>.find<Visitor>() && SilientDeaths.settings.deathSoundV)
                    {
                        playSound(SoundList.getInstance().HumanInteriorMaleDeath);
                    }
                    else if (this.getSpecialization() != TypeList<Specialization, SpecializationList>.find<Colonist>() && SilientDeaths.settings.deathSoundC)
                    {
                        playSound(SoundList.getInstance().HumanInteriorMaleDeath);
                    }
                }
                else if (gender == Gender.Female && this.getSpecialization() != TypeList<Specialization, SpecializationList>.find<Intruder>())
                {
                    if (this.getSpecialization() != TypeList<Specialization, SpecializationList>.find<Intruder>() && SilientDeaths.settings.deathSoundI)
                    {
                        playSound(SoundList.getInstance().HumanInteriorFemaleDeath);
                    }
                    else if (this.getSpecialization() != TypeList<Specialization, SpecializationList>.find<Visitor>() && SilientDeaths.settings.deathSoundV)
                    {
                        playSound(SoundList.getInstance().HumanInteriorFemaleDeath);
                    }
                    else if (this.getSpecialization() != TypeList<Specialization, SpecializationList>.find<Colonist>() && SilientDeaths.settings.deathSoundC)
                    {
                        playSound(SoundList.getInstance().HumanInteriorFemaleDeath);
                    }
                }
            }
            else if (mLocation == Location.Exterior && SilientDeaths.settings.deathSoundExterior)
            {
                if (gender == Gender.Male)
                {
                    if (this.getSpecialization() != TypeList<Specialization, SpecializationList>.find<Intruder>() && SilientDeaths.settings.deathSoundI)
                    {
                        playSound(SoundList.getInstance().HumanExteriorMaleDeath);
                    }
                    else if (this.getSpecialization() != TypeList<Specialization, SpecializationList>.find<Visitor>() && SilientDeaths.settings.deathSoundV)
                    {
                        playSound(SoundList.getInstance().HumanExteriorMaleDeath);
                    }
                    else if (this.getSpecialization() != TypeList<Specialization, SpecializationList>.find<Colonist>() && SilientDeaths.settings.deathSoundC)
                    {
                        playSound(SoundList.getInstance().HumanExteriorMaleDeath);
                    }
                }
                else if (gender == Gender.Female && this.getSpecialization() != TypeList<Specialization, SpecializationList>.find<Intruder>())
                {
                    if (this.getSpecialization() != TypeList<Specialization, SpecializationList>.find<Intruder>() && SilientDeaths.settings.deathSoundI)
                    {
                        playSound(SoundList.getInstance().HumanExteriorFemaleDeath);
                    }
                    else if (this.getSpecialization() != TypeList<Specialization, SpecializationList>.find<Visitor>() && SilientDeaths.settings.deathSoundV)
                    {
                        playSound(SoundList.getInstance().HumanExteriorFemaleDeath);
                    }
                    else if (this.getSpecialization() != TypeList<Specialization, SpecializationList>.find<Colonist>() && SilientDeaths.settings.deathSoundC)
                    {
                        playSound(SoundList.getInstance().HumanExteriorFemaleDeath);
                    }
                }
            }
            
            mQueuedAnimation = null;
            mQueuedAnchorPoint = null;
            if (isIndicatorLessOrEqual(CharacterIndicator.Oxygen, IndicatorLevel.ExtremelyLow) || isIndicatorLessOrEqual(CharacterIndicator.Hydration, IndicatorLevel.ExtremelyLow))
            {
                playAnimation(new CharacterAnimation(CharacterAnimationType.DieSlow), WrapMode.ClampForever);
            }
            else
            {
                playAnimation(new CharacterAnimation(CharacterAnimationType.Die), WrapMode.ClampForever);
            }
        }
    }
}
