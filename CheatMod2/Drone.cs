using Planetbase;
using UnityEngine;

namespace CheatModX;

public class Drone : Specialization
{
    public Drone()
    {
        mNamePrefix = "DRO";
        mAi = BotAi.getInstance();
        mIcon = ResourceList.StaticIcons.Surveyed;
        GameObject gameObject = Object.Instantiate(ResourceUtil.loadPrefab("Prefabs/Characters/PrefabBotConstructor"));
        gameObject.setVisibleRecursive(visible: false);
        gameObject.SetActive(true);
        gameObject.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        GameObject gameObject2 = Object.Instantiate(ResourceUtil.loadPrefab("Prefabs/Ships/PrefabShipPersonnelSmallColonist"));
        gameObject2.transform.localScale = new Vector3(0.125f, 0.125f, 0.125f);
        gameObject2.transform.localPosition = new Vector3(0f, 1.25f, 1f);
        gameObject2.transform.parent = gameObject.transform;
        mModel = gameObject;
        mFlags = 6;
        mCharacterType = typeof(Bot);
        mValue = 800;
        mColor = CharacterDefinitions.getInstance().Worker.MainColor;
        mIntegrityFactor = 1f;
        mName = GetType().Name;
        mNamePlural = GetType().Name;
        mSpeedFactor = 3f;
        Selection.refreshOutlines();
    }
}
