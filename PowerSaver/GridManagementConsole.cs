﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;

namespace PowerSaver
{
    public class GridManagementConsole : ComponentType
    {
        public static string NAME = "Grid Management Console";
        public static string DESCRIPTION = @"An Engineer can use this console to control the resource distribution in case of shortage. This will prevent your most vital modules from shutting down while there are non-vital modules still operating.";

        public GridManagementConsole()
        {
            this.mConstructionCosts = new ResourceAmounts();
            this.mConstructionCosts.add(ResourceTypeList.MetalInstance, 1);
            this.mConstructionCosts.add(ResourceTypeList.BioplasticInstance, 1);
            base.addUsageAnimation(CharacterAnimationType.WorkSeated, CharacterProp.Count, CharacterProp.Count);
            this.mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Engineer>();
            this.mFlags = 264;
            this.mSurveyedConstructionCount = 20;
            this.mPrefabName = "PrefabRadioConsole";

            string language = Profile.getInstance().getLanguage();
            var stringGetter = CoreUtils.GetMember<Dictionary<string,string>>("mStrings",this.GetType());
            if (language == "en")
            {
                stringGetter.Add("component_" + Util.camelCaseToLowercase(this.GetType().Name), NAME);
                stringGetter.Add("tooltip_" + Util.camelCaseToLowercase(this.GetType().Name), DESCRIPTION);
                //StringList.mStrings.Add("component_" + Util.camelCaseToLowercase(this.GetType().Name), NAME);
                //StringList.mStrings.Add("tooltip_" + Util.camelCaseToLowercase(this.GetType().Name), DESCRIPTION);
            }
            // this is needed because the game doesn't use the fallback strings for tooltips
            else if (!StringList.exists("tooltip_" + Util.camelCaseToLowercase(this.GetType().Name)))
            {
                stringGetter.Add("tooltip_" + Util.camelCaseToLowercase(this.GetType().Name), DESCRIPTION);
                //StringList.mStrings.Add("tooltip_" + Util.camelCaseToLowercase(this.GetType().Name), DESCRIPTION);
            }
            var fallbackStringGetter = CoreUtils.GetMember<Dictionary<string, string>>("mStrings", this.GetType());
            fallbackStringGetter.Add("component_" + Util.camelCaseToLowercase(this.GetType().Name), NAME);
            //StringList.mFallbackStrings.Add("component_" + Util.camelCaseToLowercase(this.GetType().Name), NAME);
            fallbackStringGetter.Add("tooltip_" + Util.camelCaseToLowercase(this.GetType().Name), DESCRIPTION);
            //StringList.mFallbackStrings.Add("tooltip_" + Util.camelCaseToLowercase(this.GetType().Name), DESCRIPTION);

            this.initStrings();

            string iconPath = PowerSaver.CONSOLE_ICON_PATH;
            if (File.Exists(iconPath))
            {
                byte[] iconBytes = File.ReadAllBytes(iconPath);
                Texture2D tex = new(0, 0);
                tex.LoadRawTextureData(iconBytes);
                this.mIcon = Util.applyColor(tex);
            }
            else 
            { 
                this.mIcon = ResourceList.StaticIcons.PowerGrid;
            }
        }
    }
}
