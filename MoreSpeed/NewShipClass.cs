using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Planetbase.LandingShip;
using UnityEngine;

namespace MoreSpeed
{
    public class NewShip : LandingShip
    {
        public override GameObject getPrefab()
        {
            throw new NotImplementedException();
        }
        private new Planetbase.Module mTargetModule;
        //Overriding this method to bypass the aforementioned glitch
        public override void init(Planetbase.Module targetModule, Size size, VisitorShipType visitorShipType = VisitorShipType.Count)
        {
            mSize = size;
            Vector3 vector = targetModule.getPosition() + Vector3.up * 75f + Vector3.right * UnityEngine.Random.Range(-150f, 150f);
            Vector3 floorPosition = targetModule.getFloorPosition();
            Vector3 vector2 = new Vector3(floorPosition.x, vector.y, floorPosition.z);
            Vector3 vector3 = vector2 - vector;
            vector3.y = 0f;
            mOriginalRotation = Quaternion.LookRotation(vector3.normalized) * Quaternion.Euler(0f, UnityEngine.Random.Range(-45f, 45f), 0f);
            mFinalRotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
            mState = State.Landing;
            mTargetModule = targetModule;
            mObject.transform.position = vector;
            mObject.transform.rotation = mOriginalRotation;
            TimeManager instance = Singleton<TimeManager>.getInstance();
            if (instance.getTimeScale() > 4)
            {
                instance.decreaseSpeed();
            }
        }
        //As above
        public override void postInit()
        {
            TimeManager instance = Singleton<TimeManager>.getInstance();
            if (instance.getTimeScale() > 4)
            {
                instance.decreaseSpeed();
            }
            mModel = UnityEngine.Object.Instantiate(getPrefab());
            mModel.transform.parent = mObject.transform;
            mModel.transform.position = mObject.transform.position;
            mModel.transform.rotation = mObject.transform.rotation;
            mModel.name = "Landing Ship Model";
            mModel.disablePhysics();
            mTargetModule.addTargeter(this);
            mObject.setLayerRecursive(16);
            int childCount = mModel.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject gameObject = mModel.transform.GetChild(i).gameObject;
                if (gameObject.CompareTag("ShipEngine"))
                {
                    if (mObjectEngines == null)
                    {
                        mObjectEngines = new List<GameObject>();
                        mEngineParticles = new List<ParticleSystemData>();
                    }
                    int childCount2 = gameObject.transform.childCount;
                    for (int j = 0; j < childCount2; j++)
                    {
                        GameObject gameObject2 = gameObject.transform.GetChild(j).gameObject;
                        ParticleSystemData particleSystemData = Singleton<ParticleManager>.getInstance().create(ResourceList.getInstance().Particles.Engine);
                        particleSystemData.getGameObject().transform.SetParent(gameObject2.transform, worldPositionStays: false);
                        mEngineParticles.Add(particleSystemData);
                    }
                    mObjectEngines.Add(gameObject);
                }
            }
            mAnimation = mModel.GetComponent<Animation>();
            if (mAnimation != null)
            {
                mAnimationNames = new List<string>();
                foreach (AnimationState item in mAnimation)
                {
                    mAnimationNames.Add(item.name);
                }
            }
            updateEngines(mVelocity.z, 1f);
            if (mState == State.Landed || mState == State.OpeningDoor)
            {
                setEnginesActive(active: false);
                playAnimation("gear_out", WrapMode.Once);
                queueAnimation("door_open");
                mGearDeployed = true;
            }
        }
        private new void updateLanding(float timeStep)
        {
            Vector3 position = getPosition();
            Vector3 floorPosition = mTargetModule.getFloorPosition();
            float magnitude = (position - floorPosition).magnitude;
            if (magnitude < 5f && !mGearDeployed)
            {
                mGearDeployed = true;
                playAnimation("gear_out");
            }
            if (!mAudioSource.isPlaying && magnitude < 25f && magnitude > 5f)
            {
                mAudioSource.play(SoundList.getInstance().ShipEngineLanding, loop: false, Singleton<Profile>.getInstance().getSfxVolumeNormalized());
            }
            if (magnitude < 0.1f)
            {
                setState(State.OpeningDoor);
            }
            else if (magnitude < 1f && Mathf.Abs(mRotationSpeed) < 2f)
            {
                mVelocity *= 0.5f;
                mRotationSpeed *= 0.5f;
                Vector3 normalized = (floorPosition - getPosition()).normalized;
                if (normalized.magnitude > 0.01f)
                {
                    mObject.transform.position += normalized * Mathf.Min(timeStep, magnitude);
                }
            }
            else
            {
                updateLandingControls(timeStep);
                updateFlight(timeStep);
                updateEngines(mVelocity.z, timeStep);
            }
        }
        private new void updateOpeningDoor(float timeStep)
        {
            if (mStateTime > 3f)
            {
                setState(State.Landed);
            }
        }
        private new void updateLanded(float timeStep)
        {
            updateEngines(0f, timeStep);
            SoundDefinition shipIdle = SoundList.getInstance().ShipIdle;
            if (mAudioSource.clip != shipIdle.Clip)
            {
                mAudioSource.play(shipIdle, loop: true, Singleton<Profile>.getInstance().getSfxVolumeNormalized());
            }
            if (canTakeOff())
            {
                setState(State.ClosingDoor);
            }
        }
        private new void updateClosingDoor(float timeStep)
        {
            if (mStateTime > 3f)
            {
                setState(State.TakingOff);
            }
        }
        private new void updateTakingOff(float timeStep)
        {
            mStateTime += timeStep;
            updateTakeoffControls(timeStep);
            updateFlight(timeStep);
            updateEngines(mVelocity.z, timeStep);
            if (mAnimation != null && mGearDeployed && mStateTime > 1f)
            {
                mGearDeployed = false;
                playAnimation("gear_in");
            }
            if (mStateTime > 30f)
            {
                destroyDeferred();
            }
        }

        //overriding previous two methods changed nothing, thrid time the charm
        public override void update(float timeStep)
        {
            mStateTime += timeStep;
            if (mTargetModule.isDestroyed())
            {
                destroyDeferred();
                return;
            }
            if (mState == State.Landing)
            {
                TimeManager instance = Singleton<TimeManager>.getInstance();
                if (instance.getTimeScale() > 4)
                {
                    instance.decreaseSpeed();
                }
                updateLanding(timeStep);
            }
            else if (mState == State.OpeningDoor)
            {
                updateOpeningDoor(timeStep);
            }
            else if (mState == State.Landed)
            {
                updateLanded(timeStep);
            }
            else if (mState == State.ClosingDoor)
            {
                updateClosingDoor(timeStep);
            }
            else if (mState == State.TakingOff)
            {
                updateTakingOff(timeStep);
            }
            base.update(timeStep);
        }
    }
}
