using Planetbase;
using System;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System.Collections.Generic;

namespace NoIntruders
{
    
    public class NoIntruders : ModBase
    {
		public static new void Init(ModEntry modEntry) => InitializeMod(new NoIntruders(), modEntry, "NoIntruders");

		public Planet Planet { get; private set; }
		public override void OnInitialized(ModEntry modEntry)
		{
			
			
		}

		public override void OnUpdate(ModEntry modEntry, float timeStep)
		{
			//var planet = new NewPlanet();
			//planet.WealthRecalculation(Planet);
			var character = new NewCharacter();
			character.KillIntruders();
			
        }
	}
    public class NewCharacter : Character
    {
		public void KillIntruders()
        {
			List<Character> intruders = getSpecializationCharacters(SpecializationList.IntruderInstance);
			if (intruders != null)
			{
				foreach (Character intruder in intruders)
				{
					// kill intruders instantly.
					if (intruder != null && !intruder.isDead())
					{
						intruder.setArmed(false);
						var dead = new CharacterImplementation();
						dead.SetDead();
					}
				}
			}
		}
		//Overides below are placeholders to prevent compiler errors
		public override float getHeight()
        {
			if (mCurrentAnimation != null && (mCurrentAnimation.getAnimationType() == CharacterAnimationType.Die || mCurrentAnimation.getAnimationType() == CharacterAnimationType.Sleep))
			{
				return 0.5f;
			}
			return 1.75f;
		}

        public override Texture2D getIcon()
        {
			
			return mSpecialization.getIcon();
		}

        public override Bounds getSelectionBounds()
        {
            throw new NotImplementedException();
        }

        protected override List<string> getAnimationNames(CharacterAnimationType animationType)
        {
			throw new NotImplementedException();
		}
    }
    public class NewPlanet : Planet
	{
		public void WealthRecalculation(Planet planet)
		{
			CoreUtils.SetMember("mIntruderMinPrestige", planet, 20f);
		}

	}
	public class CharacterImplementation : Character
	{
		public void SetDead()
		{
			base.setDead();
		}
		//Overides below are placeholders to prevent compiler errors
		public override float getHeight()
		{
			throw new NotImplementedException();
		}

		public override Texture2D getIcon()
		{
			throw new NotImplementedException();
		}

		public override Bounds getSelectionBounds()
		{
			throw new NotImplementedException();
		}

		protected override List<string> getAnimationNames(CharacterAnimationType animationType)
		{
			throw new NotImplementedException();
		}
	}
}
