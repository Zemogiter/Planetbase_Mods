﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planetbase;
using UnityEngine;

namespace ColonistReport
{
	public class GuiReportsMenu : GuiWindow
	{
		public Dictionary<ColonistsWorkload, GuiIndicatorItem> WorkloadIndicators;

		public GuiReportsMenu() : base(new GuiLabelItem(StringList.get("reports"), ResourceList.StaticIcons.Stats, null, 0, FontSize.Normal), null, null)
		{
			// TODO add help entry
			// this.mHelpId = "stats";

			WorkloadIndicators = new Dictionary<ColonistsWorkload, GuiIndicatorItem>();

			foreach (var workload in WorkloadManager.AllWorkloads)
			{
				WorkloadIndicators.Add(workload, CreateWorkloadIndicator(workload.Name, workload.Icon));
			}
		}

		public override void update()
		{

		}

		public void updateUi()
		{
			GuiSectionItem sectionWorkload = new GuiSectionItem(StringList.get("reports_workload"));

			mRootItem.clear();
			mRootItem.addChild(sectionWorkload);

			foreach (var element in WorkloadIndicators)
			{
				var workload = element.Key;
				var guiIndicator = element.Value;
				var indicator = guiIndicator.mIndicator as WorkloadIndicator;

				if (workload.TotalCharacters > 0)
				{
					guiIndicator.setVisible(true);
					indicator.setValue(workload.DisplayWorkload);
					indicator.setName(workload.Name);
					sectionWorkload.addChild(guiIndicator);
				}
			}
		}

		private GuiIndicatorItem CreateWorkloadIndicator(string key, Texture2D icon)
		{
			WorkloadIndicator indicator = new WorkloadIndicator(StringList.get(key), icon);

			GuiIndicatorItem indicatorItem = new GuiIndicatorItem(indicator);
			indicatorItem.setHeight(GuiStyles.getIconSizeMedium());
			return indicatorItem;
		}

		public override float getWidth()
		{
			return (float)Screen.height * 0.7f;
		}
	}

	public class GuiReportsMenuItem : GuiMenuItem
	{
		public GuiReportsMenuItem(GuiDefinitions.Callback callback) : base(ResourceList.StaticIcons.Stats, StringList.get("reports"), callback)
		{

		}
	}
}
