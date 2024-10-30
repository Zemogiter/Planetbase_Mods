using System.Collections.Generic;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;

namespace LandingControl
{
    public abstract class CustomColonistShip : ColonistShip
    {
        public override void onLanded()
        {
            NavigationGraph.getExterior().addBlocker(this.getPosition() + base.getTransform().forward, this.getRadius());

            int numNewColonists = 2;

            LandingPermissions landingPermissions = LandingShipManager.getInstance().getLandingPermissions();
            for (int i = 0; i < numNewColonists; i++)
            {
                Specialization specialization = (!this.mIntruders) ? GetSpecialiation(landingPermissions) : TypeList<Specialization, SpecializationList>.find<Intruder>();
                if (specialization != null)
                {
                    Character.create(specialization, base.getSpawnPosition(i), Location.Exterior);

                    if (!mIntruders)
                    {
                        var specializationP = CoreUtils.GetMember<LandingPermissions, Dictionary<Specialization, RefInt>>("mSpecializationPercentages");
                        specializationP[specialization].set(specializationP[specialization].get() - 1);

                        bool anyAllowed = false;
                        foreach (Specialization spec in SpecializationList.getColonistSpecializations())
                        {
                            if (landingPermissions.getSpecializationPercentage(spec).get() > 0)
                            {
                                anyAllowed = true;
                                break;
                            }
                        }

                        if (!anyAllowed)
                        {
                            var colonistsAllowed = CoreUtils.GetMember<LandingPermissions, RefBool>("mColonistsAllowed");
                            colonistsAllowed.set(false);
                        }
                    }
                }
            }
        }

        public Specialization GetSpecialiation(LandingPermissions landingPermissions)
        {
            List<Specialization> potentialChoices = new List<Specialization>();

            foreach (Specialization specialization in SpecializationList.getColonistSpecializations())
            {
                if (landingPermissions.getSpecializationPercentage(specialization).get() > 0)
                    potentialChoices.Add(specialization);
            }

            if (potentialChoices.Count > 0)
                return potentialChoices[Random.Range(0, potentialChoices.Count)];

            return null;
        }
    }
}