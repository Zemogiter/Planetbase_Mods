using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityModManagerNet;
using System.Xml;

namespace TelescopesAreFun
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Max users for telescopes")] public float telescopeMaxInteractions = 5;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class TelescopesAreFun : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new TelescopesAreFun(), modEntry, "TelescopesAreFun");
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

        public const float TelescopeInteractionMoraleREcovery = 360f;

        public override void OnInitialized(ModEntry modEntry)
        {

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }
    }
    public class AIRuleTelescopeInteraction : AiTargetRule
    {        
        public override bool update(Character character)
        {
            Construction targetConstruction = character.getTargetConstruction();
            if (targetConstruction != null && targetConstruction.hasFlag(9175040) && targetConstruction.isOperational() && targetConstruction.getInteractionCount() < TelescopesAreFun.settings.telescopeMaxInteractions)
            {
                if (character.getLocation() == Location.Exterior)
                {
                    Interaction.create<InteractionAirlockEnter>(character, targetConstruction);
                }
                else
                {
                    Interaction.create<InteractionAirlockExit>(character, targetConstruction);
                }
                return true;
            }
            return false;
        }
    }
    public abstract class InteractionTelescope : InteractionConstruction
    {
        public enum Stage
        {
            Wait,
            GoEntry,
            GoChange,
            Change,
            Interact,
            GoDecompress,
            Decompress,
            Exit
        }

        //To-do: figure out the names of telescope animations and add them here
        protected const string AnimationOpenInterior = "airlock_open_interior";
        protected const string AnimationCloseInterior = "airlock_close_interior";
        protected const string AnimationOpenExterior = "airlock_open_exterior";
        protected const string AnimationCloseExterior = "airlock_close_exterior";
        public const float AnimationSpeed = 3f;
        protected Vector3 mEntryPoint;
        //protected Vector3 mChangePoint;
        public Vector3 mInteractionPoint;
        protected Vector3 mDecompressionPoint;
        protected Vector3 mExitPoint;
        protected Stage mStage = Stage.GoChange;
        protected float mStageProgress;
        protected Vector3 mTarget;
        protected CharacterAnimationType mAnimationType;

        public override List<CharacterAnimation> getCharacterAnimations(Character character)
        {
            return null;
        }

        public override void serialize(XmlNode parent, string name)
        {
            base.serialize(parent, name);
            XmlNode lastChild = parent.LastChild;
            Serialization.serializeInt(lastChild, "stage", (int)mStage);
            Serialization.serializeFloat(lastChild, "stage-progress", mStageProgress);
            Serialization.serializeVector3(lastChild, "target", mTarget);
        }

        public override void init(Character character, Selectable construction)
        {
            base.init(character, construction);
            initPoints();
        }

        public override void deserialize(XmlNode node)
        {
            base.deserialize(node);
            mStage = (Stage)Serialization.deserializeInt(node["stage"]);
            mStageProgress = Serialization.deserializeFloat(node["stage-progress"]);
            mTarget = Serialization.deserializeVector3(node["target"]);
            initPoints();
        }
        //To-do: figure out if Telescope has these points, if not try to add them
        private void initPoints()
        {
            Construction construction = mSelectable as Construction;
            mEntryPoint = construction.getPoint("entry_point").position;
            //mChangePoint = construction.getPoint("change_point").position;
            mInteractionPoint = construction.getPoint("interaction_point").position;
            mDecompressionPoint = construction.getPoint("decompression_point").position;
            mExitPoint = construction.getPoint("exit_point").position;
            mCharacter.playWalkAnimation();
            mAnimationType = CharacterAnimationType.Walk;
        }

        public bool isWaiting()
        {
            return mStage == Stage.Wait;
        }

        //To-do: create this method
        public override Construction getCurrentConstruction()
        {
            return getTelescope();
        }

        public override void destroy()
        {
            base.destroy();
        }

        protected abstract void onStageDone();

        protected abstract Vector3 getQueuePosition(int i);
    }
}
