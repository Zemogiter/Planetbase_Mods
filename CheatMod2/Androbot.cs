using System;
using System.Collections.Generic;
using System.Xml;
using static System.Xml.XmlDocument;
using Planetbase;
using UnityEngine;

namespace CheatModX;

public class Androbot : Character
{
	public Color mParticlesColor;

	public Vector3 mLastPosition;

	public ParticleSystemData mDustParticles;

	public float mIntegrityDecayRate;

	public static HashSet<Androbot> mFreeAndrobots;

	[NonSerialized]
	public Human.Gender mGender;

	[NonSerialized]
	public int mSkinColorIndex;

	[NonSerialized]
	public int mHeadIndex;

	[NonSerialized]
	public int mHairColorIndex;

	public Androbot()
	{
		Indicator indicator = new(StringList.get("condition"), ResourceList.StaticIcons.Condition, IndicatorType.Condition, 1f, 3f, SignType.Condition);
		indicator.setLevels(0.1f, 0.2f, 0.3f, 0.5f);
		mIndicators[6] = indicator;
		Indicator indicator2 = new(StringList.get("integrity"), ResourceList.StaticIcons.Bot, IndicatorType.Condition, 1f, 3f, SignType.Condition);
		indicator2.setLevels(0.05f, 0.1f, 0.15f, 0.2f);
		mIndicators[7] = indicator2;
	}

	public override void init(Specialization specialization, Vector3 position, Location location)
	{
		base.init(specialization, position, location);
		mName = mSpecialization.getNamePrefix() + "-" + (Singleton<IdGenerator>.getInstance().generateBot() + 1);
		mIntegrityDecayRate = UnityEngine.Random.Range(9600f, 19200f) / mSpecialization.getIntegrityFactor();
		mGender = ((UnityEngine.Random.Range(0, 5) <= 1) ? Human.Gender.Female : Human.Gender.Male);
		mHeadIndex = UnityEngine.Random.Range(0, specialization.getHeadCount(mGender));
		EthnicParameters ethnicParameters = mSpecialization.getEthnicParameters(mGender, mHeadIndex);
		if (ethnicParameters != null)
		{
			mSkinColorIndex = UnityEngine.Random.Range(0, ethnicParameters.SkinColors.Length);
			mHairColorIndex = UnityEngine.Random.Range(0, ethnicParameters.HairColors.Length);
		}
	}

	public override void createModel()
	{
		if (isSelected())
		{
			restoreModel();
		}
		mMaterials.destroyAll();
		GameObject model = ((anyInteractions() && getFirstInteraction().requiresTracksuit()) ? mSpecialization.createTracksuitModel(mMaterials, mGender, mHeadIndex, mSkinColorIndex, mHairColorIndex) : ((anyInteractions() && getFirstInteraction().requiresUncoveredHead()) ? mSpecialization.createUncoveredModel(mMaterials, mGender, mHeadIndex, mSkinColorIndex, mHairColorIndex) : ((mLocation != 0) ? mSpecialization.createModel(mMaterials, mGender, mHeadIndex, mSkinColorIndex, mHairColorIndex) : mSpecialization.createModel(mMaterials, mGender, mHeadIndex, mSkinColorIndex, mHairColorIndex))));
		setModel(model);
		if (isSelected())
		{
			Selection.refreshOutlines();
		}
	}

	public override void end()
	{
		base.end();
		if (mFreeAndrobots.Contains(this))
		{
			mFreeAndrobots.Remove(this);
		}
		if (mDustParticles != null)
		{
			Singleton<ParticleManager>.getInstance().destroy(mDustParticles);
		}
	}

	protected void serialize(XmlNode parent, string name)
	{
		base.serialize(parent, name);
		Serialization.serializeFloat(parent.LastChild, "integrity-decay-rate", mIntegrityDecayRate);
	}

	protected void deserialize(XmlNode node)
	{
		base.deserialize(node);
		if (node["integrity-decay-rate"] != null)
		{
			mIntegrityDecayRate = Serialization.deserializeFloat(node["integrity-decay-rate"]);
		}
		else
		{
			mIntegrityDecayRate = UnityEngine.Random.Range(9600f, 19200f);
		}
	}

	public override Texture2D getIcon()
	{
		return mSpecialization.getIcon();
	}

	public override string getSubtitle()
	{
		return mSpecialization.getName(mGender);
	}

	public override string getDescription()
	{
		string text = base.getDescription();
		if (Singleton<DebugManager>.getInstance().showExtraDescriptionInfo())
		{
			string text2 = text;
			text = text2 + "Integrity: " + getIndicator(CharacterIndicator.Integrity).getValue() + "\n";
			text2 = text;
			text = text2 + "IntegrityDecayRate: " + mIntegrityDecayRate + "\n";
		}
		return text;
	}

	public override float getMaxSpeed()
	{
		float num = 6f * mSpecialization.getSpeedFactor();
		if (mLocation == Location.Exterior)
		{
			Disaster stormInProgress = Singleton<DisasterManager>.getInstance().getStormInProgress();
			if (stormInProgress != null && mLocation == Location.Exterior)
			{
				float num2 = Mathf.Lerp(1f, 0.25f, stormInProgress.getIntensity());
				num *= num2;
			}
		}
		return num;
	}

	public override bool hasInertia()
	{
		return true;
	}

	public override void recycle()
	{
		ResourceAmounts resourceAmounts = calculateRecycleAmounts();
		if (resourceAmounts == null)
		{
			return;
		}
		foreach (ResourceAmount item in resourceAmounts)
		{
			for (int i = 0; i < item.getAmount(); i++)
			{
				Resource.create(item.getResourceType(), getPosition() + MathUtil.randFlatVector(getRadius()), mLocation).drop(Resource.State.Idle);
			}
		}
	}

	public override float getHeight()
	{
		return 1.75f;
	}

	public override bool isDeleteable(out bool buttonEnabled)
	{
		buttonEnabled = true;
		return true;
	}

	public override ResourceAmounts calculateRecycleAmounts()
	{
		ResourceAmounts resourceAmounts = new();
		resourceAmounts.add(ResourceTypeList.BioplasticInstance, 1);
		resourceAmounts.add(ResourceTypeList.MetalInstance, 1);
		resourceAmounts.add(ResourceTypeList.SparesInstance, 1);
		return resourceAmounts;
	}

	public override void setLocation(Location location)
	{
		base.setLocation(location);
		createModel();
	}

	public bool shouldDecay()
	{
		if (!hasInteraction<InteractionWork>() && !hasInteraction<InteractionBuild>() && !hasInteraction<InteractionRepairBuildable>())
		{
			if (mState == State.Walking)
			{
				return isLoaded();
			}
			return false;
		}
		return true;
	}

	public override void update(float timeStep)
	{
		base.update(timeStep);
		if (shouldDecay())
		{
			decayIndicator(CharacterIndicator.Condition, timeStep / 480f);
		}
		Disaster stormInProgress = Singleton<DisasterManager>.getInstance().getStormInProgress();
		if (stormInProgress != null && !isProtected())
		{
			decayIndicator(CharacterIndicator.Condition, timeStep * stormInProgress.getIntensity() / 600f);
		}
		SolarFlare solarFlare = Singleton<DisasterManager>.getInstance().getSolarFlare();
		if (solarFlare.isInProgress() && !isProtected())
		{
			decayIndicator(CharacterIndicator.Condition, timeStep * solarFlare.getIntensity() / 180f);
		}
		if (mState != State.Ko && !isDead() && isStatusExtremelyLow(CharacterIndicator.Condition) && !isBeingRestored())
		{
			setKo();
		}
		updateDustParticles(timeStep);
	}

	public void updateDustParticles(float timeStep)
	{
		if (mState == State.Walking && mLocation == Location.Exterior)
		{
			if (mDustParticles == null)
			{
				GameObject botDust = PlanetManager.getCurrentPlanet().getBotDust();
				if (botDust != null)
				{
					mDustParticles = Singleton<ParticleManager>.getInstance().create(botDust);
					mDustParticles.getGameObject().transform.SetParent(mModel.transform.parent, worldPositionStays: false);
				}
			}
			if (mDustParticles != null)
			{
				float floorHeight = Singleton<TerrainGenerator>.getInstance().getFloorHeight();
				bool flag = getPosition().y <= floorHeight + 0.1f && mCurrentSpeed > 1f;
				if (flag != mDustParticles.isEmissionEnabled())
				{
					mDustParticles.setEmissionEnabled(flag);
				}
			}
		}
		else if (mDustParticles != null)
		{
			Singleton<ParticleManager>.getInstance().stop(mDustParticles);
			mDustParticles = null;
		}
	}

	public override void setKo()
	{
		base.setKo();
		playAnimation(new CharacterAnimation(CharacterAnimationType.BeingRepaired), WrapMode.ClampForever);
	}

	public override void setDead()
	{
		Message message = new(StringList.get("message_bot_died", getName()), flags: (Character.getCountOfType<Bot>() < 10) ? 1 : 0, icon: ResourceList.StaticIcons.Condition, targetSelectable: this);
		message.setCondensedMessage(Message.DeathBot);
		Singleton<MessageLog>.getInstance().addMessage(message);
		if (mInteractions != null)
		{
			mInteractions.Clear();
		}
		base.setDead();
	}

	public override bool isFree()
	{
		if (base.isFree() || (mCurrentAiRule is AiRuleWanderInterior && mState == State.Walking))
		{
			if (mLocation != 0)
			{
				return NavigationGraph.isExteriorLocationReachable(getPosition());
			}
			return true;
		}
		return false;
	}

	public override void tick(float timeStep)
	{
		base.tick(timeStep);
		if (!isBeingRestored())
		{
			decayIndicator(CharacterIndicator.Integrity, timeStep / mIntegrityDecayRate);
			if (getIndicator(CharacterIndicator.Integrity).isMin())
			{
				decayIndicator(CharacterIndicator.Condition, 1f);
			}
		}
		updateFreeStatus();
		if (isIndicatorLow(CharacterIndicator.Condition))
		{
			setSignEnabled(SignType.Condition, getIndicator(CharacterIndicator.Condition).getSignSeverity());
		}
		else
		{
			setSignDisabled(SignType.Condition);
		}
		if (isKo() && getIndicator(CharacterIndicator.Condition).isMin())
		{
			setDead();
		}
	}

	public override void removeInteraction(Interaction interaction)
	{
		base.removeInteraction(interaction);
		updateFreeStatus();
	}

	public void updateFreeStatus()
	{
		if (isFree())
		{
			if (!mFreeAndrobots.Contains(this))
			{
				mFreeAndrobots.Add(this);
			}
		}
		else if (mFreeAndrobots.Contains(this))
		{
			mFreeAndrobots.Remove(this);
		}
	}

	public override ICollection<Indicator> getIndicators()
	{
		return mIndicators;
	}

	public override bool requiresOxygen()
	{
		return false;
	}

	public override bool needsRestoring()
	{
		if (getIndicator(CharacterIndicator.Condition).isExtremelyLow())
		{
			return !getIndicator(CharacterIndicator.Integrity).isMin();
		}
		return false;
	}

	public override Specialization getRestorerSpecialization()
	{
		return TypeList<Specialization, SpecializationList>.find<Engineer>();
	}

	public override ResourceType getRestoringResource()
	{
		return TypeList<ResourceType, ResourceTypeList>.find<Spares>();
	}

	public override bool restore(float amount)
	{
		return getIndicator(CharacterIndicator.Condition).increase(amount);
	}

	public override string getHelpId()
	{
		return "Androbot" + mSpecialization.GetType().Name;
	}

	public override List<string> getAnimationNames(CharacterAnimationType animationType)
	{
		return mSpecialization.getAnimationNames(animationType, mLocation, mGender);
	}

	public override Bounds getSelectionBounds()
	{
		return new Bounds(getPosition() + Vector3.up * 0.6f, new Vector3(0.9f, 1.2f, 0.9f));
	}

	public static bool anyFree(int flag)
	{
		foreach (Androbot mFreeAndrobot in mFreeAndrobots)
		{
			if (mFreeAndrobot.getSpecialization().hasFlag(flag))
			{
				return true;
			}
		}
		return false;
	}

	static Androbot()
	{
		mFreeAndrobots = new HashSet<Androbot>();
	}
}
