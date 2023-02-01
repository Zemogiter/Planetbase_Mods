using System;
using System.Collections.Generic;
using System.Xml;
using Planetbase;
using UnityEngine;

namespace CheatModX;

public class GlobalVars : Singleton<GlobalVars>
{
	private string[] sStatus = new string[8]
	{
		StringList.get("health"),
		StringList.get("nutrition"),
		StringList.get("hydration"),
		StringList.get("oxygen"),
		StringList.get("sleep"),
		StringList.get("morale"),
		StringList.get("condition"),
		StringList.get("integrity")
	};

	private Texture2D[] oStatus = new Texture2D[8]
	{
		ResourceList.StaticIcons.Health,
		ResourceList.StaticIcons.Food,
		ResourceList.StaticIcons.Water,
		ResourceList.StaticIcons.Oxygen,
		ResourceList.StaticIcons.Sleep,
		ResourceList.StaticIcons.Morale,
		ResourceList.StaticIcons.Condition,
		ResourceList.StaticIcons.Bot
	};

	private bool[] bSColono = new bool[14];

	private bool[] bGenerar = new bool[4];

	private string[] sGenerar = new string[4]
	{
		StringList.get("power_balance"),
		StringList.get("water_balance"),
		StringList.get("oxygen_balance"),
		StringList.get("wind")
	};

	private Texture2D[] oGenerar = new Texture2D[4]
	{
		ResourceList.StaticIcons.PowerGeneration,
		ResourceList.StaticIcons.WaterGeneration,
		ResourceList.StaticIcons.OxygenGeneration,
		ResourceList.StaticIcons.Wind
	};

	private bool[] bAlmacenes = new bool[4];

	private string[] sAlmacenes = new string[4]
	{
		StringList.get("power_storage"),
		StringList.get("water_storage"),
		StringList.get("laser_charge"),
		StringList.get("oxygen")
	};

	private Texture2D[] oAlmacenes = new Texture2D[4]
	{
		ResourceList.StaticIcons.PowerStorage,
		ResourceList.StaticIcons.Water,
		ResourceList.StaticIcons.Meteor,
		ResourceList.StaticIcons.Oxygen
	};

	private bool[] bSpawn = new bool[30];

	private int[] iRecursos = new int[((TypeList<ResourceType, ResourceTypeList>)TypeList<ResourceType, ResourceTypeList>.getInstance()).mTypeList.Count];

	private int iMaxCantidad = 999;

	public int iMaxContainer = 15000;

	private bool bClear;

	private float fTime;

	private bool bTime;

	private int iMaxPower = 2000000;

	private int iMaxWater = 2000000;

	private int iMaxOxigen = 900;

	public ConstructionComponent oCc;

	public ProductAmounts oPANull;

	private bool bAtmosphere;

	private bool bCheckPlay;

	private ModuleType ModConst = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeHyperdome>();

	public ResourceType oMetal = TypeList<ResourceType, ResourceTypeList>.find<Metal>();

	public ResourceType oBioplastic = TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>();

	private ResourceType oVegetables = TypeList<ResourceType, ResourceTypeList>.find<Vegetables>();

	private ResourceSubtype[] oSVegetables = new ResourceSubtype[10]
	{
		ResourceSubtype.Wheat,
		ResourceSubtype.Maize,
		ResourceSubtype.Rice,
		ResourceSubtype.Peas,
		ResourceSubtype.Potatoes,
		ResourceSubtype.Lettuce,
		ResourceSubtype.Tomatoes,
		ResourceSubtype.Onions,
		ResourceSubtype.Radishes,
		ResourceSubtype.Mushrooms
	};

	private ResourceType oMeal = TypeList<ResourceType, ResourceTypeList>.find<Meal>();

	private ResourceSubtype[] oSMeal = new ResourceSubtype[4]
	{
		ResourceSubtype.Basic,
		ResourceSubtype.Salad,
		ResourceSubtype.Pasta,
		ResourceSubtype.Burger
	};

	private ResourceType oVitromeat = TypeList<ResourceType, ResourceTypeList>.find<Vitromeat>();

	private ResourceSubtype[] oSVitromeat = new ResourceSubtype[3]
	{
		ResourceSubtype.Chicken,
		ResourceSubtype.Pork,
		ResourceSubtype.Beef
	};

	public Texture2D oIconGenerador = ResourceList.StaticIcons.Build;

	public Texture2D oIconStatus = ResourceList.StaticIcons.Health;

	public Texture2D oIconSpawn = ResourceList.StaticIcons.Male;

	public Texture2D oIconContenedor = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeStorage>().getIcon();

	protected List<GuiMenuItem> oMenus = new();

	public void OnGUI()
	{
		bCheckPlay = GameManager.getInstance().getGameState() is GameStateGame;
		if (!bCheckPlay)
		{
			return;
		}
		bAtmosphere = PlanetManager.getCurrentPlanet().getAtmosphereDensity() > Planet.Quantity.None;
		CheckMod();
		if (Selection.getSelected() is ConstructionComponent && Selection.getSelected() is ConstructionComponent constructionComponent && constructionComponent.isOperational())
		{
			if (constructionComponent.getComponentType() is ModSpawnColono)
			{
				SetMenu(0);
			}
			else if (constructionComponent.getComponentType() is ModContenedor)
			{
				SetMenu(1);
			}
		}
	}

	public void onClick(object prm)
	{
		if (prm is int)
		{
			if ((int)prm == 0)
			{
				WinSpawn();
			}
			else if ((int)prm == 1)
			{
				WinGenerador();
			}
			else if ((int)prm == 2)
			{
				WinStatus();
			}
			else if ((int)prm == 3)
			{
				WinContenedor();
			}
			else if ((int)prm == 4)
			{
				Tecnologia();
			}
		}
	}

	private void SetMenu(int i)
	{
		GameStateGame obj = GameManager.getInstance().getGameState() as GameStateGame;
		if (oMenus.Count == 0)
		{
			oMenus.Add(new GuiMenuItem(oIconSpawn, StringList.get("colonists") + " Spawner", onClick, 0, 0));
			oMenus.Add(new GuiMenuItem(oIconGenerador, "Source " + Languajes("Generación", "Generation"), onClick, 1, 0));
			oMenus.Add(new GuiMenuItem(oIconStatus, "Life " + Languajes("Estados", "Status"), onClick, 2, 0));
			oMenus.Add(new GuiMenuItem(oIconContenedor, "Resource " + StringList.get("storage"), onClick, 3, 0));
			oMenus.Add(new GuiMenuItem(ResourceList.StaticIcons.Tech, "Unlock " + StringList.get("techs"), onClick, 4, 0));
		}
		GuiMenu mCurrentMenu = obj.mMenuSystem.mCurrentMenu;
		if (i == 1)
		{
			if (!mCurrentMenu.mItems.Contains(oMenus[1]))
			{
				mCurrentMenu.addItem(oMenus[1]);
			}
			if (!mCurrentMenu.mItems.Contains(oMenus[2]))
			{
				mCurrentMenu.addItem(oMenus[2]);
			}
			if (!mCurrentMenu.mItems.Contains(oMenus[3]))
			{
				mCurrentMenu.addItem(oMenus[3]);
			}
			if (!mCurrentMenu.mItems.Contains(oMenus[4]))
			{
				mCurrentMenu.addItem(oMenus[4]);
			}
		}
		else if (!mCurrentMenu.mItems.Contains(oMenus[0]))
		{
			mCurrentMenu.addItem(oMenus[0]);
		}
	}

	private void SetWin(GuiWindow oWin)
	{
		((GameStateGame)GameManager.getInstance().getGameState()).mGameGui.setWindow(oWin);
	}

	private void onCancel(object prm)
	{
		((GameStateGame)GameManager.getInstance().getGameState()).mGameGui.setWindow(null);
	}

	private void onRecursos(object prm)
	{
		RefInt[] array = (RefInt[])prm;
		if (array != null && array.Length != 0)
		{
			for (int i = 0; i < iRecursos.Length; i++)
			{
				iRecursos[i] = array[i].get();
			}
		}
	}

	private void onClear(object prm)
	{
		bClear = true;
	}

	private void WinContenedor()
	{
		int num = 0;
		RefInt[] array = new RefInt[iRecursos.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new RefInt(iRecursos[i]);
		}
		MyWindow myWindow = new();
		myWindow.Size = 400f;
		((GuiWindow)myWindow).mRootItem.clear();
		GuiSectionItem guiSectionItem = new(StringList.get("resources"));
		foreach (ResourceType item2 in TypeList<ResourceType, ResourceTypeList>.get())
		{
			if (item2.mModel != null)
			{
				GuiRowItem guiRowItem = new(3);
				guiRowItem.addChild(new GuiLabelItem(item2.getName(), item2.getIcon()));
				guiRowItem.addChild(new GuiLabelItem(""));
				GuiAmountSelector item = new(0, iMaxCantidad, 50, array[num]);
				guiRowItem.addChild(item);
				guiSectionItem.addChild(guiRowItem);
			}
			num++;
		}
		((GuiWindow)myWindow).mRootItem.addChild(guiSectionItem);
		GuiRowItem guiRowItem2 = new(3);
		GuiLabelItem guiLabelItem = new(StringList.get("confirm"), ResourceList.StaticIcons.Enable);
		guiLabelItem.addCallback(onRecursos, array);
		GuiLabelItem guiLabelItem2 = new(StringList.get("cancel"), ResourceList.StaticIcons.Cancel);
		guiLabelItem2.addCallback(onCancel, null);
		GuiLabelItem guiLabelItem3 = new(StringList.get("keycode_delete"), ResourceList.StaticIcons.Recycle);
		guiLabelItem3.addCallback(onClear, null);
		guiRowItem2.addChild(guiLabelItem);
		guiRowItem2.addChild(guiLabelItem2);
		guiRowItem2.addChild(guiLabelItem3);
		((GuiWindow)myWindow).mRootItem.addChild(guiRowItem2);
		SetWin(myWindow);
	}

	private void OnGenerar(object prm)
	{
		RefBool[] array = (RefBool[])prm;
		if (array != null && array.Length != 0)
		{
			for (int i = 0; i < bGenerar.Length; i++)
			{
				bGenerar[i] = array[i].get();
			}
		}
	}

	private void OnAlmacenes(object prm)
	{
		RefBool[] array = (RefBool[])prm;
		if (array != null && array.Length != 0)
		{
			for (int i = 0; i < bAlmacenes.Length; i++)
			{
				bAlmacenes[i] = array[i].get();
			}
		}
	}

	private void OnTime(object prm)
	{
		if (prm is RefBool refBool)
		{
			bTime = refBool.get();
		}
	}

	private void OnSetTime(object prm)
	{
		if (prm is GuiAmountSelector guiAmountSelector)
		{
			int num = ((guiAmountSelector.getCurrent() < 6) ? (guiAmountSelector.getCurrent() + 18) : (guiAmountSelector.getCurrent() - 6));
			fTime = (float)num * (1f / 24f);
		}
	}

	private void WinGenerador()
	{
		RefBool[] array = new RefBool[bGenerar.Length];
		for (int i = 0; i < bGenerar.Length; i++)
		{
			array[i] = new RefBool(bGenerar[i]);
		}
		RefBool[] array2 = new RefBool[bAlmacenes.Length];
		for (int j = 0; j < bAlmacenes.Length; j++)
		{
			array2[j] = new RefBool(bAlmacenes[j]);
		}
		RefBool refBool = new(bTime);
		RefInt value = new(Convert.ToInt32((fTime * 24f < 6f) ? (fTime * 24f + 18f) : (fTime * 24f - 6f)));
		MyWindow myWindow = new();
		myWindow.Size = 400f;
		((GuiWindow)myWindow).mRootItem.clear();
		GuiSectionItem guiSectionItem = new(Languajes("generación", "generation"));
		for (int k = 0; k < bGenerar.Length; k++)
		{
			GuiLabelItem guiLabelItem = new(sGenerar[k], oGenerar[k]);
			guiLabelItem.addCheckbox(array[k], OnGenerar, array);
			guiSectionItem.addChild(guiLabelItem);
		}
		((GuiWindow)myWindow).mRootItem.addChild(guiSectionItem);
		GuiSectionItem guiSectionItem2 = new(Languajes("Opciones de almacenamiento", "Storage options"));
		for (int l = 0; l < bGenerar.Length; l++)
		{
			GuiLabelItem guiLabelItem2 = new(sAlmacenes[l], oAlmacenes[l]);
			guiLabelItem2.addCheckbox(array2[l], OnAlmacenes, array2);
			guiSectionItem2.addChild(guiLabelItem2);
		}
		((GuiWindow)myWindow).mRootItem.addChild(guiSectionItem2);
		GuiRowItem guiRowItem = new(3);
		GuiSectionItem guiSectionItem3 = new(Languajes("Hora del día", "Time of day"));
		GuiLabelItem guiLabelItem3 = new(Languajes("Fijar hora del día", "fix time of day"), ResourceList.StaticIcons.Day);
		guiLabelItem3.addCheckbox(refBool, OnTime, refBool);
		guiRowItem.addChild(guiLabelItem3);
		guiRowItem.addChild(new GuiLabelItem(""));
		new GuiAmountSelector(0, 23, 1, value, OnSetTime);
		guiRowItem.addChild(new GuiAmountSelector(0, 23, 1, value, OnSetTime));
		guiSectionItem3.addChild(guiRowItem);
		((GuiWindow)myWindow).mRootItem.addChild(guiSectionItem3);
		GuiRowItem guiRowItem2 = new(3);
		guiRowItem2.addChild(new GuiLabelItem(""));
		GuiLabelItem guiLabelItem4 = new(StringList.get("cancel"), ResourceList.StaticIcons.Cancel);
		guiLabelItem4.addCallback(onCancel, null);
		guiRowItem2.addChild(guiLabelItem4);
		guiRowItem2.addChild(new GuiLabelItem(""));
		((GuiWindow)myWindow).mRootItem.addChild(guiRowItem2);
		SetWin(myWindow);
	}

	private void onStatus(object prm)
	{
		RefBool[] array = (RefBool[])prm;
		if (array != null && array.Length != 0)
		{
			for (int i = 0; i < bSColono.Length; i++)
			{
				bSColono[i] = array[i].get();
			}
		}
	}

	private void WinStatus()
	{
		RefBool[] array = new RefBool[bSColono.Length];
		for (int i = 0; i < bSColono.Length; i++)
		{
			array[i] = new RefBool(bSColono[i]);
		}
		MyWindow myWindow = new();
		myWindow.Size = 400f;
		((GuiWindow)myWindow).mRootItem.clear();
		GuiSectionItem guiSectionItem = new("Auto " + StringList.get("overall_status") + " " + StringList.get("colonist"));
		for (int j = 0; j < 6; j++)
		{
			GuiLabelItem guiLabelItem = new(sStatus[j], oStatus[j]);
			guiLabelItem.addCheckbox(array[j], onStatus, array);
			guiSectionItem.addChild(guiLabelItem);
		}
		((GuiWindow)myWindow).mRootItem.addChild(guiSectionItem);
		GuiSectionItem guiSectionItem2 = new("Auto " + StringList.get("overall_status") + " " + StringList.get("specialization_visitor_plural"));
		for (int k = 0; k < 6; k++)
		{
			GuiLabelItem guiLabelItem2 = new(sStatus[k], oStatus[k]);
			guiLabelItem2.addCheckbox(array[k + 6], onStatus, array);
			guiSectionItem2.addChild(guiLabelItem2);
		}
		((GuiWindow)myWindow).mRootItem.addChild(guiSectionItem2);
		GuiSectionItem guiSectionItem3 = new("Auto " + StringList.get("overall_status") + " " + StringList.get("bots"));
		for (int l = 6; l < 8; l++)
		{
			GuiLabelItem guiLabelItem3 = new(sStatus[l], oStatus[l]);
			guiLabelItem3.addCheckbox(array[l + 6], onStatus, array);
			guiSectionItem3.addChild(guiLabelItem3);
		}
		((GuiWindow)myWindow).mRootItem.addChild(guiSectionItem3);
		GuiRowItem guiRowItem = new(3);
		guiRowItem.addChild(new GuiLabelItem(""));
		GuiLabelItem guiLabelItem4 = new(StringList.get("cancel"), ResourceList.StaticIcons.Cancel);
		guiLabelItem4.addCallback(onCancel, null);
		guiRowItem.addChild(guiLabelItem4);
		guiRowItem.addChild(new GuiLabelItem(""));
		((GuiWindow)myWindow).mRootItem.addChild(guiRowItem);
		SetWin(myWindow);
	}

	private void OnSpawn(object prm)
	{
		Specialization specialization = prm as Specialization;
		ConstructionComponent constructionComponent = Selection.getSelected() as ConstructionComponent;
		if (specialization != null && constructionComponent.getComponentType() is ModSpawnColono)
		{
			Character.create(specialization, constructionComponent.getPosition(), Location.Interior);
		}
	}

	private void WinSpawn()
	{
		MyWindow myWindow = new();
		myWindow.Size = 400f;
		((GuiWindow)myWindow).mRootItem.clear();
		GuiSectionItem guiSectionItem = new("Spawn - " + StringList.get("colonists"));
		foreach (Specialization item in TypeList<Specialization, SpecializationList>.get())
		{
			if (item != SpecializationList.VisitorInstance && item != SpecializationList.IntruderInstance)
			{
				GuiLabelItem guiLabelItem = new(item.getName(), item.getIcon());
				guiLabelItem.addCallback(OnSpawn, item);
				guiSectionItem.addChild(guiLabelItem);
			}
		}
		((GuiWindow)myWindow).mRootItem.addChild(guiSectionItem);
		GuiRowItem guiRowItem = new(3);
		guiRowItem.addChild(new GuiLabelItem(""));
		GuiLabelItem guiLabelItem2 = new(StringList.get("cancel"), ResourceList.StaticIcons.Cancel);
		guiLabelItem2.addCallback(onCancel, null);
		guiRowItem.addChild(guiLabelItem2);
		guiRowItem.addChild(new GuiLabelItem(""));
		((GuiWindow)myWindow).mRootItem.addChild(guiRowItem);
		SetWin(myWindow);
	}

	public void OnTech(object prm)
	{
		RefBool[] array = (RefBool[])prm;
		int num = 0;
		foreach (Tech item in TypeList<Tech, TechList>.get())
		{
			if (!array[num].get() && Singleton<TechManager>.getInstance().isAcquired(item))
			{
				Singleton<TechManager>.getInstance().mAcquiredTechs.Remove(item);
			}
			else if (array[num].get() && !Singleton<TechManager>.getInstance().isAcquired(item))
			{
				Singleton<TechManager>.getInstance().mAcquiredTechs.Add(item);
			}
			num++;
		}
	}

	private void Tecnologia()
	{
		RefBool[] array = new RefBool[TypeList<Tech, TechList>.get().Count];
		int num = 0;
		MyWindow myWindow = new();
		myWindow.Size = 400f;
		((GuiWindow)myWindow).mRootItem.clear();
		GuiSectionItem guiSectionItem = new(StringList.get("techs"));
		foreach (Tech item in TypeList<Tech, TechList>.get())
		{
			array[num] = new RefBool(Singleton<TechManager>.getInstance().isAcquired(item));
			GuiLabelItem guiLabelItem = new(item.getName(), item.getIcon(), item.getDescription());
			guiLabelItem.addCheckbox(array[num], OnTech, array);
			guiSectionItem.addChild(guiLabelItem);
			num++;
		}
		((GuiWindow)myWindow).mRootItem.addChild(guiSectionItem);
		GuiRowItem guiRowItem = new(3);
		guiRowItem.addChild(new GuiLabelItem(""));
		GuiLabelItem guiLabelItem2 = new(StringList.get("cancel"), ResourceList.StaticIcons.Cancel);
		guiLabelItem2.addCallback(onCancel, null);
		guiRowItem.addChild(guiLabelItem2);
		guiRowItem.addChild(new GuiLabelItem(""));
		((GuiWindow)myWindow).mRootItem.addChild(guiRowItem);
		SetWin(myWindow);
	}

	private void AddResource(ConstructionComponent ct, int iCantidad, ResourceType oResType, ResourceSubtype oResSType)
	{
		if (iCantidad >= 1)
		{
			for (int i = 0; i < iCantidad; i++)
			{
				Resource resource = Resource.create(oResType, oResSType, ct.getPosition(), Location.Interior);
				ct.embedResource(resource);
				resource.setState(Resource.State.Stored);
			}
		}
	}

	private void CheckMod()
	{
		if (!bCheckPlay)
		{
			return;
		}
		try
		{
			foreach (Construction mConstruction in Construction.mConstructions)
			{
				if (mConstruction == null)
				{
					break;
				}
				if (!(mConstruction is Module))
				{
					continue;
				}
				if (!(mConstruction is Module module))
				{
					break;
				}
				if (module.getModuleType() is ModuleTypeHyperdome)
				{
					bool flag = false;
					foreach (ConstructionComponent component in module.getComponents())
					{
						if (component == null)
						{
							return;
						}
						if (!(component.getComponentType() is ModContenedor) || !component.isOperational())
						{
							continue;
						}
						if (component.getResourceContainer() != null)
						{
							if (bClear)
							{
								component.getResourceContainer().destroyResources();
							}
							bClear = false;
							int num = 0;
							foreach (ResourceType item in TypeList<ResourceType, ResourceTypeList>.get())
							{
								if (item != ResourceTypeList.CoinsInstance && iRecursos[num] > 0)
								{
									int countOf = component.getResourceContainer().getCountOf(item);
									int num2 = iRecursos[num] - countOf;
									if (num2 > 0)
									{
										int num3 = 0;
										if (item == oVegetables)
										{
											int num4 = num2 / 10;
											ResourceSubtype[] array = oSVegetables;
											foreach (ResourceSubtype oResSType in array)
											{
												AddResource(component, num4, item, oResSType);
												num3 += num4;
											}
										}
										else if (item == oVitromeat)
										{
											int num5 = num2 / 3;
											ResourceSubtype[] array = oSVitromeat;
											foreach (ResourceSubtype oResSType2 in array)
											{
												AddResource(component, num5, item, oResSType2);
												num3 += num5;
											}
										}
										else if (item == oMeal)
										{
											int num6 = num2 / 4;
											ResourceSubtype[] array = oSMeal;
											foreach (ResourceSubtype oResSType3 in array)
											{
												AddResource(component, num6, item, oResSType3);
												num3 += num6;
											}
										}
										if (num2 - num3 > 0)
										{
											AddResource(component, num2 - num3, item, ResourceSubtype.None);
										}
									}
								}
								num++;
							}
						}
						EnvironmentManager instance = Singleton<EnvironmentManager>.getInstance();
						if (instance != null)
						{
							if (bTime)
							{
								instance.setTimeOfDay(fTime);
							}
							if (bGenerar[3] && bAtmosphere)
							{
								instance.getWindIndicator().setValue(instance.getWindIndicator().getMax());
							}
						}
						foreach (Character mCharacter in Character.mCharacters)
						{
							bool flag2 = mCharacter is Bot;
							bool flag3 = mCharacter.getSpecialization() == SpecializationList.VisitorInstance;
							bool flag4 = !flag2 && !flag3 && mCharacter.getSpecialization() != SpecializationList.IntruderInstance;
							for (int j = 0; j < 8; j++)
							{
								if (mCharacter.mIndicators[j] != null)
								{
									if (flag4 && j < 6 && bSColono[j])
									{
										((Human)mCharacter).clearCondition();
									}
									if (flag3 && j < 6 && bSColono[j + 6])
									{
										((Human)mCharacter).clearCondition();
									}
									if (flag4 && j < 6 && bSColono[j])
									{
										mCharacter.restoreStatus((CharacterIndicator)j, 1f);
									}
									if (flag3 && j < 6 && bSColono[j + 6])
									{
										mCharacter.restoreStatus((CharacterIndicator)j, 1f);
									}
									if (flag2 && j > 5 && bSColono[j + 6])
									{
										mCharacter.restoreStatus((CharacterIndicator)j, 1f);
									}
								}
							}
						}
						if (bGenerar[0])
						{
							component.getComponentType().mPowerGeneration = iMaxPower;
						}
						else
						{
							component.getComponentType().mPowerGeneration = 1;
						}
						if (bGenerar[1])
						{
							component.getComponentType().mWaterGeneration = iMaxWater;
						}
						else
						{
							component.getComponentType().mWaterGeneration = 1;
						}
						flag = true;
					}
					if (bGenerar[2] && flag)
					{
						module.getModuleType().mOxygenGeneration = iMaxOxigen;
					}
					else
					{
						module.getModuleType().mOxygenGeneration = 1;
					}
				}
				else if (bAlmacenes[0] && module.isOperational() && module.getCategory() == Module.Category.PowerStorage)
				{
					module.mPowerStorageIndicator.setValue(module.mPowerStorageIndicator.getMax());
				}
				else if (bAlmacenes[1] && module.isOperational() && module.getCategory() == Module.Category.WaterStorage)
				{
					module.mWaterStorageIndicator.setValue(module.mWaterStorageIndicator.getMax());
				}
				else if (bAlmacenes[2] && module.isOperational() && module.hasFlag(2097152))
				{
					module.mLaserChargeIndicator.setValue(module.mLaserChargeIndicator.getMax());
				}
				else if (bAlmacenes[3] && module.isBuilt() && mConstruction.requiresOxygen())
				{
					((Construction)module).mOxygenIndicator.setValue(((Construction)module).mOxygenIndicator.getMax());
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
	}

	public string Languajes(string l1, string l2)
	{
		if (Singleton<Profile>.getInstance().getLanguage() == "es")
		{
			return l1;
		}
		return l2;
	}

	/*public void ModSaveData(XmlNode rootNode)
	{
		XmlNode parent = Serialization.createNode(rootNode, "CheatMods");
		string text = "";
		bool[] array = bSColono;
		foreach (bool flag in array)
		{
			text += (flag ? "1" : "0");
		}
		Serialization.serializeString(parent, "Estado", text);
		string text2 = "";
		array = bGenerar;
		foreach (bool flag2 in array)
		{
			text2 += (flag2 ? "1" : "0");
		}
		Serialization.serializeString(parent, "Generar", text2);
		string text3 = "";
		array = bAlmacenes;
		foreach (bool flag3 in array)
		{
			text3 += (flag3 ? "1" : "0");
		}
		Serialization.serializeString(parent, "Almacenes", text3);
		string text4 = "";
		int[] array2 = iRecursos;
		foreach (int num in array2)
		{
			text4 = text4 + ((text4.Length > 0) ? "," : "") + num;
		}
	}*/

	public void ModLoadData(XmlNode node)
	{
		try
		{
			node = node.ParentNode["CheatMods"];
			if (node == null)
			{
				return;
			}
			string text = Serialization.deserializeString(node["Estado"]);
			for (int i = 0; i < text.Length; i++)
			{
				if (i < bSColono.Length)
				{
					bSColono[i] = (text.Substring(i, 1) == "1") ? true : false;
				}
			}
			string text2 = Serialization.deserializeString(node["Generar"]);
			for (int j = 0; j < text2.Length; j++)
			{
				if (j < bGenerar.Length)
				{
					bGenerar[j] = ((text2.Substring(j, 1) == "1") ? true : false);
				}
			}
			string text3 = Serialization.deserializeString(node["Almacenes"]);
			for (int k = 0; k < text3.Length; k++)
			{
				if (k < bAlmacenes.Length)
				{
					bAlmacenes[k] = ((text3.Substring(k, 1) == "1") ? true : false);
				}
			}
			string[] array = Serialization.deserializeString(node["Cantidad"]).Split(',');
			for (int l = 0; l < array.Length; l++)
			{
				if (l < iRecursos.Length)
				{
					iRecursos[l] = Convert.ToInt32(array[l]);
				}
			}
		}
		catch
		{
		}
	}
}
