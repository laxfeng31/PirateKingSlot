using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Mkey;
using System;
using UnityEngine.Events;
using UnityEditor.Events;
using System.Linq;


namespace Slot.Editor
{
	public class AssetHandler
	{
		[OnOpenAsset()]
		public static bool OpenWindow(int instanceId, int line)
		{
			SlotAsset obj = EditorUtility.InstanceIDToObject(instanceId) as SlotAsset;
			if (obj != null)
			{
				SlotAssetWindow.OpenWindow(obj);
				return true;
			}
			return false;
		}
	}
	public class SlotAssetWindow : EditorWindow
	{
		public static SlotAssetWindow window;
		public SlotAsset slot;
		public TypeOfSection selectedType;

		
		


		Vector2 scrollRect;
		int uiGeneralToolBarSelected;
		string[] toolbarItem = new string[] { "Image", "GameObject","Text","Button" };
		int toolbarUiButtonSettingCurrectSelected = 0;
		string[] toolbarUiButtonSetting = new string[] {"Line Plus","Line Minus","Bet Plus","Bet Minus","Back","Menu","AutoSpin","Spin" };
 		bool updateSceneConfirmation = false;
		int currentSelectedLineSetting = 0;
		public static SlotAssetWindow OpenWindow(SlotAsset slot)
		{
			if (window != null)
			{
				window.Close();
			}


			window = CreateInstance<SlotAssetWindow>();
			window.slot = slot;
			window.Init(slot);
			window.titleContent = new GUIContent(window.slot.name);
			

			window.Show();
			return window;
		}
		

		void Init(SlotAsset slot)
		{
			if (slot.slotIcons == null) slot.slotIcons = new List<SlotIcon>();
			if (slot.slotIcontab == null) slot.slotIcontab = new List<int>();
			if (slot.payLines == null) slot.payLines = new List<PayLine>();
		}
		
		private void OnGUI()
		{
			EditorHorizontal(() =>
			{
				#region SideBar
				EditorVentical("Box",()=>
				{
					GUILayout.Label("Slot Setting");
					
					if (GUILayout.Button("Slot Icon"))
					{
						selectedType = TypeOfSection.slotIcon;
					}
					if (GUILayout.Button("Pay Table"))
					{
						selectedType = TypeOfSection.paytable;
					}
					if (GUILayout.Button("Scatter Pay Table"))
					{
						selectedType = TypeOfSection.scatterPayTable;
					}
					if (GUILayout.Button("Special Icon Pay Table"))
					{
						selectedType = TypeOfSection.specialIconPayTable;
					}
					if (GUILayout.Button("Wild Pay Table"))
					{
						selectedType = TypeOfSection.wildPayTable;

					}
					if (GUILayout.Button("Slot Group"))
					{
						selectedType = TypeOfSection.slotGroup;

					}
					
					if (GUILayout.Button("Bonus"))
					{
						selectedType = TypeOfSection.bonusSetting;

					}
					if (GUILayout.Button("Line Setting"))
					{
						selectedType = TypeOfSection.lineSetting;

					}
					if (GUILayout.Button("Feature Setting"))
					{
						selectedType = TypeOfSection.featureSetting;

					}
					if (GUILayout.Button("Other Setting"))
					{
						selectedType = TypeOfSection.otherSlotControlSetting;
					}
                    if (GUILayout.Button("Calculate RTP"))
                    {
                        selectedType = TypeOfSection.RTPSetting;
                        slot.rtp = new RTPSetting();
                        col = new List<slotIconGroup>();

                    }
                    DrawLine();
					DrawLine();
					GUILayout.Label("UI Setting");
					if (GUILayout.Button("Ui General"))
					{
						selectedType = TypeOfSection.UiGeneralSetting;

					}
					if (GUILayout.Button("ARC Helps"))
					{
						selectedType = TypeOfSection.ARCHelpPages;
					}
					//if (GUILayout.Button("Help Setting"))
     //               {
     //                   selectedType = TypeOfSection.HelpSetting;

     //               }
                    //if (GUILayout.Button("Other Setting"))
                    //{
                    //	selectedType = TypeOfSection.UiOtherSetting;

                    //}
                    DrawLine();
					DrawLine();
					GUILayout.Label("Last Save:");
					GUILayout.Label(slot.LastSave);
					if (GUILayout.Button("Save"))
					{
						SavingAsset(slot);
					}
					DrawLine();
					DrawLine();
					GUILayout.Label("Update To Scene");
					if (updateSceneConfirmation)
					{
						DrawLine();
						GUILayout.Label("Confirm?");
						if (GUILayout.Button("Yes"))
						{
							UpdateToScene();
							
							updateSceneConfirmation = false;
						}
						if (GUILayout.Button("No"))
						{
							updateSceneConfirmation = false;
						}
						DrawLine();
					}
					else
					{
						if (GUILayout.Button("Update"))
						{
							updateSceneConfirmation = true;
						}
					}
					
				}, GUILayout.Width(100), GUILayout.ExpandHeight(true));

				#endregion SideBar

				#region Selected Display

				EditorVentical("Box",()=> 
				{
					scrollRect = EditorGUILayout.BeginScrollView(scrollRect, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
					switch (selectedType)
					{
						case TypeOfSection.slotIcon:
							DrawSlotIcon();
							break;
						case TypeOfSection.paytable:
							DrawPayTable();
							break;
						case TypeOfSection.scatterPayTable:
							DrawScatterPayTable();
							break;
						case TypeOfSection.specialIconPayTable:
							DrawSpecialIconPayTable();
							break;
						case TypeOfSection.wildPayTable:
							
							DrawWildPayTable();
							break;
						case TypeOfSection.slotGroup:
							DrawSlotGroup();
							break;
						case TypeOfSection.bonusSetting:
							DrawBonusSetting();
							break;
						case TypeOfSection.lineSetting:
							DrawLineSetting();
							break;
						case TypeOfSection.featureSetting:
							DrawFeatureSetting();
							break;
						case TypeOfSection.otherSlotControlSetting:
							DrawOtherSlotControlSetting();
							break;
						case TypeOfSection.UiGeneralSetting:
							DrawUiGeneralSetting();
							break;
						case TypeOfSection.ARCHelpPages:
							DrawARCHelpSetting();
							break;
						case TypeOfSection.HelpSetting:
							DrawHelpSetting();
							break;
						case TypeOfSection.UiOtherSetting:
							DrawUiOtherSetting();
							break;
						case TypeOfSection.RTPSetting:
							DrawRTP();

							break;
						
					}
					EditorGUILayout.EndScrollView();
				}, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

				#endregion Selected Display

			});


		}
		

		#region Draw UI

		#region DrawSlotIcon
		void DrawSlotIcon()
		{
			EditorGUILayout.LabelField("Slot Icon Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

			EditorVentical(()=> 
			{
				for (int i = 0; i < slot.slotIcons.Count; i++)
				{
					SlotIcon s = slot.slotIcons[i];

					

					EditorVentical("GroupBox", () =>
					{
						Color background = new Color32(0, 0, 0, 0);

						if(slot.scatter_id == i && slot.useScatter)
						{
							background = new Color32(242, 154, 61, 255);
						}
						if (slot.specialIcon_id == i && slot.useSpecialIcon)
						{
							background = new Color32(200, 154, 61, 255);
						}
						if (slot.wild_id == i && slot.useWild)
						{
							background = new Color32(255, 255, 66, 255);
						}

						EditorHorizontal(background,() =>
						{

							if (ConvertSprite(s.iconSprite) != null)
							{
								
								DrawSprite(s.iconSprite, 40);

								EditorVentical(() =>
								{
									EditorHorizontal(() =>
									{
										EditorGUILayout.LabelField(s.iconSprite.name);
										EditorGUILayout.LabelField("ID: " + i);

										if (GUILayout.Button("Remove " + (s.iconSprite ? s.iconSprite.name : "")))
										{
											slot.slotIcons.RemoveAt(i);
											slot.slotIcontab.RemoveAt(i);
											i = 0;
										}
									});

									EditorHorizontal(() =>
									{
										s.iconSprite = (Sprite)EditorGUILayout.ObjectField(s.iconSprite, typeof(Sprite), false);
										if (slot.specialIcon_id == i && slot.useSpecialIcon)
										{
											slot.specialIconWildSub = EditorGUILayout.Toggle("Wild Sub: ", slot.specialIconWildSub);
											if (!EditorGUILayout.Toggle("Is A Special Icon: ", true))
											{
												slot.specialIcon_id = -1;
												slot.useSpecialIcon = false;
												s.useWildSubstitute = false;
												slot.specialIconPayTable.Clear();
											}
											s.useWildSubstitute = slot.specialIconWildSub;
											slot.specialIconFollowSeq = EditorGUILayout.Toggle("Follow Sequence : ", slot.specialIconFollowSeq);
										}
										else if(slot.scatter_id == i && slot.useScatter)
										{
											slot.scatterWildSub = EditorGUILayout.Toggle("Wild Sub: ", slot.scatterWildSub);
											if (!EditorGUILayout.Toggle("Is A Scatter: ", true))
											{
												slot.scatter_id = -1;
												slot.useScatter = false;
												
											}
											s.useWildSubstitute = slot.scatterWildSub;
											slot.scatterFollowSeq = EditorGUILayout.Toggle("Follow Sequence : ", slot.scatterFollowSeq);
										}
										else if(slot.wild_id == i && slot.useWild)
										{
											if (!EditorGUILayout.Toggle("Is A Wild: ", true))
											{
												slot.wild_id = -1;
												slot.useWild = false;
												s.useWildSubstitute = false;
											}
											slot.wild_multiply = EditorGUILayout.IntField("Wild Mutiply:", slot.wild_multiply);
										}
										else
										{
											s.useWildSubstitute = EditorGUILayout.Toggle("Wild Sub: ", s.useWildSubstitute);
											if(EditorGUILayout.Toggle("Is A Wild: ", false))
											{
												slot.wild_id = i;
												slot.useWild = true;
											}
											if (EditorGUILayout.Toggle("Is A Scatter: ", false))
											{
												slot.scatter_id = i;
												slot.useScatter = true;
											}
											if (EditorGUILayout.Toggle("Is A Special Icon: ", false))
											{
												slot.specialIcon_id = i;
												slot.useSpecialIcon = true;
											}
										}
										
									});
								});


							}
						});


						EditorHorizontal(() =>
						{

							EditorVentical("Box", () =>
							{
								
								slot.slotIcontab[i] = GUILayout.Toolbar(slot.slotIcontab[i], new string[] { "Winning Setting", "Feature Setting" });

								switch (slot.slotIcontab[i])
								{
									case (0):
										EditorGUILayout.LabelField("Winning Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
										GUILayout.Space(10);
										DrawLine();

										EditorGUILayout.LabelField("Feature Sprite:");
										s.iconSpriteFeature = (Sprite)EditorGUILayout.ObjectField(s.iconSpriteFeature, typeof(Sprite), false);
										EditorGUILayout.LabelField("Feature Winning Animation Controller:");
										s.animCFeature = (RuntimeAnimatorController)EditorGUILayout.ObjectField(s.animCFeature, typeof(RuntimeAnimatorController), false);
										DrawLine();
										EditorGUILayout.LabelField("Winning Frame:");
										s.addIconSprite = (Sprite)EditorGUILayout.ObjectField(s.addIconSprite, typeof(Sprite), false);
										EditorGUILayout.LabelField("Winning Animation Controller:");
										s.animC = (RuntimeAnimatorController)EditorGUILayout.ObjectField(s.animC, typeof(RuntimeAnimatorController), false);
										EditorGUILayout.LabelField("Winning Sound:");
										s.soundName = EditorGUILayout.TextField(s.soundName);
										EditorGUILayout.LabelField("Time for display win(s):");
										s.timeDisplayAnim = EditorGUILayout.IntSlider(s.timeDisplayAnim, 2, 10);
										EditorGUILayout.LabelField("Display Win Anim when appear while spining");
										s.displayWinAnimWhenAppear = EditorGUILayout.Toggle(s.displayWinAnimWhenAppear);
                                        if (s.displayWinAnimWhenAppear)
                                        {
											EditorGUILayout.LabelField("Display Win Anim when appear while spining sound");
											s.displayWinAnimWhenAppearSound = EditorGUILayout.TextField(s.displayWinAnimWhenAppearSound);

                                        }
                                        else
                                        {
											s.displayWinAnimWhenAppearSound = "";

										}

										if (slot.scatter_id == i || slot.wild_id == i)
										{
											GUILayout.Space(10);
											EditorVentical("HelpBox", () =>
											{
												EditorGUILayout.LabelField("Winning Box");
												if(s.winningBox == null)
                                                {
													s.winningBox = new SlotIcon.LineWinnigBox();

												}
												s.winningBox.enableWinnigLineBox = EditorGUILayout.Toggle("Enable Line Box", s.winningBox.enableWinnigLineBox);
												if (s.winningBox.enableWinnigLineBox)
												{
													EditorLableCenterWithBox("Line Material", () =>
													{
														s.winningBox.lineMaterial = (Material)EditorGUILayout.ObjectField(s.winningBox.lineMaterial, typeof(Material), false);
													});

													EditorLableCenterWithBox("Line Color", () =>
													{
														s.winningBox.lineColor = EditorGUILayout.ColorField(s.winningBox.lineColor);
													});

													EditorLableCenterWithBox("Line Flashing Speed", () =>
													{
														s.winningBox.lineFlashingSpeed = EditorGUILayout.Slider(s.winningBox.lineFlashingSpeed, 1f, 10f);
														
													});

													EditorLableCenterWithBox("Line Width", () =>
													{
														s.winningBox.lineRendererWidth = EditorGUILayout.Slider(s.winningBox.lineRendererWidth, 1f, 10f);

													});

													EditorLableCenterWithBox("Renderer Order", () =>
													{
														s.winningBox.sortingOrder = EditorGUILayout.IntField(s.winningBox.sortingOrder);

													});

													EditorLableCenterWithBox("Wining Box Size", () =>
													{
														EditorHorizontal(() =>
														{
															EditorLableCenterWithBox("Width", () =>
															{
																s.winningBox.lineSize.x = EditorGUILayout.FloatField(s.winningBox.lineSize.x);
																
															});
															EditorLableCenterWithBox("Height", () =>
															{
																s.winningBox.lineSize.y = EditorGUILayout.FloatField(s.winningBox.lineSize.y);
																
															});
														});
													});

												}
											});
											GUILayout.Space(10);
										}
										

										EditorLableCenterWithBox("Additional Win Animation", () => 
										{
											for (int z = 0; z < s.animationWinGroup.Count; z++)
											{
												EditorHorizontal(() => 
												{
													if (GUILayout.Button("Remove "))
													{
														s.animationWinGroup.RemoveAt(z);

														return;
													}
													s.animationWinGroup[z].AnimC = (RuntimeAnimatorController)EditorGUILayout.ObjectField(s.animationWinGroup[z].AnimC, typeof(RuntimeAnimatorController), false);
												}, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
												
											}
											DragAndDropItem<RuntimeAnimatorController>("Drag and Drop Animator",
											(item) => {
												
												s.animationWinGroup.Add(new SlotIconAnimGroup("",item));
												

											});
											
										});
										GUILayout.Space(10);
										DrawLine();
										GUILayout.Space(10);


										EditorGUILayout.LabelField("Effecting Whole Column");
										s.effectWholeColumn = EditorGUILayout.Toggle("Enable:", s.effectWholeColumn);
										if (s.effectWholeColumn)
										{
											EditorGUILayout.LabelField("Prefab for Effecting Column:");
											s.effectWholeColumnGamePrefab = (GameObject)EditorGUILayout.ObjectField(s.effectWholeColumnGamePrefab, typeof(GameObject), false);
											


										}

										if(slot.wild_id == i)
										{
											GUILayout.Space(10);

											EditorVentical("HelpBox", () =>
											{
												EditorGUILayout.LabelField("Use Group Icon");
												s.groupIconSetting.useGroupIcon = EditorGUILayout.Toggle("Enable Group Icon", s.groupIconSetting.useGroupIcon);
												if (s.groupIconSetting.useGroupIcon)
												{
													EditorGUILayout.LabelField("Prefab for Group");
													s.groupIconSetting.groupIconPrefab = (GameObject)EditorGUILayout.ObjectField(s.groupIconSetting.groupIconPrefab, typeof(GameObject), false);
													s.groupIconSetting.groupCount = EditorGUILayout.IntField("Group Count:", s.groupIconSetting.groupCount);
													s.groupIconSetting.posY = EditorGUILayout.FloatField("pos Y", s.groupIconSetting.posY);

												}
											});

										}

										break;
									case (1):
										EditorGUILayout.LabelField("Feature Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
										GUILayout.Space(10);
										DrawLine();

										EditorGUILayout.LabelField("Enable Clone When Win");
										s.effectingOtherCol = EditorGUILayout.Toggle("Enable:", s.effectingOtherCol);
										if (s.effectingOtherCol)
										{
											EditorGUILayout.BeginVertical("HelpBox");

											EditorGUILayout.LabelField("Percentage Split Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
											GUILayout.Space(10);

											for (int j = 0; j < s.listOfPercentageHit.Count; j++)
											{
												EditorGUILayout.BeginHorizontal("GroupBox");

												EditorGUILayout.BeginVertical();
												s.listOfPercentageHit[j].percentageHit = EditorGUILayout.IntField("Percentage: ", s.listOfPercentageHit[j].percentageHit);
												s.listOfPercentageHit[j].quantitySplit = EditorGUILayout.IntField("Quantity: ", s.listOfPercentageHit[j].quantitySplit);
												EditorGUILayout.EndVertical();
												if (GUILayout.Button("Remove", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
												{
													s.listOfPercentageHit.RemoveAt(j);
												}

												EditorGUILayout.EndHorizontal();
											}
											if (GUILayout.Button("Add Percentage Hit"))
											{
												s.listOfPercentageHit.Add(new numberOfSlotIconEffecting());
											}
											EditorGUILayout.EndHorizontal();



											EditorGUILayout.BeginVertical("HelpBox");

											EditorGUILayout.LabelField("Excluded Column", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
											GUILayout.Space(10);

											for (int j = 0; j < s.colExcluded.Count; j++)
											{
												EditorGUILayout.BeginHorizontal("GroupBox");

												EditorGUILayout.BeginVertical();
												s.colExcluded[j] = EditorGUILayout.IntField("Column Id: ", s.colExcluded[j]);

												EditorGUILayout.EndVertical();
												if (GUILayout.Button("Remove", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
												{
													s.colExcluded.RemoveAt(j);
												}

												EditorGUILayout.EndHorizontal();
											}
											if (GUILayout.Button("Add Excluded Column"))
											{
												s.colExcluded.Add(0);
											}
											EditorGUILayout.EndHorizontal();

										}
										break;
								}
							}, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));



						});


					}, GUILayout.ExpandWidth(true), GUILayout.Height(200));


					
				}
			}, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

			
			DragAndDropItem<Sprite>("Drag and Drop Sprites in here",
				(item) => {
					slot.slotIcons.Add(new SlotIcon(item, item, null,null,null,"",null,false,""));
					slot.slotIcontab.Add(0);
					
				});
			DragAndDropItem<GameObject>("Drag SlotControl Here To overide slot icon.",
				(item) => {
					SlotController sC = item.GetComponent<SlotController>();
					if (sC != null)
					{
						slot.slotIcons = new List<SlotIcon>( sC.slotIcons);
						slot.slotIcontab.Clear();
						slot.scatter_id = sC.scatter_id;
						slot.wild_id = sC.wild_id;
						slot.wild_multiply = sC.wild_multiply;
						slot.useWild = sC.useWild;
						slot.useScatter = sC.useScatter;
						slot.scatterWildSub = sC.scatterWildSub;
						slot.scatterFollowSeq = sC.scatterFollowSeq;
						foreach (SlotIcon sI in slot.slotIcons)
						{

							slot.slotIcontab.Add(0);
						}
					}

				});
			
		}
		#endregion DrawSlotIcon

		#region DrawPayTable
		void DrawPayTable()
		{
			EditorGUILayout.LabelField("Pay Table Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
			
			EditorVentical("GroupBox",()=>
			{
				for (int i = 0; i < slot.payLines.Count; i++)
				{


					EditorVentical("GroupBox", () =>
					{

						PayLine p = slot.payLines[i];

						

						EditorHorizontal(() =>
						{
					
							for (int j = 0; j < p.line.Length; j++)
							{
								DrawSelectionIcon(ref p.line[j],true,true,125);

							}
						});

						EditorVentical("GroupBox",() =>
						{
							EditorHorizontal(() =>
							{
								EditorLableCenterWithBox("Free Spin", () =>
								{
									p.freeSpins = EditorGUILayout.IntField(p.freeSpins);
								});
								EditorLableCenterWithBox("Pay", () =>
								{
									p.pay = EditorGUILayout.IntField(p.pay);
								});



							});
							EditorHorizontal(() =>
							{

								EditorLableCenterWithBox("Pay Multiple", () =>
								{
									p.payMult = EditorGUILayout.IntField(p.payMult);
								});
								EditorLableCenterWithBox("Total Bet Multiple", () =>
								{
									p.totalBetMult = EditorGUILayout.FloatField(p.totalBetMult);
								});


							});

						});
						


						EditorVentical("GroupBox", () =>
						{
							EditorHorizontal(() =>
							{
								EditorHorizontal("HelpBox", () =>
								{
									p.reverseWin = EditorGUILayout.Toggle("Rev Win: ", p.reverseWin);
								});

								EditorHorizontal("HelpBox", () =>
								{
									p.specialEffect = EditorGUILayout.Toggle("Show Special Effect: ", p.specialEffect);
								});

								

							});
							EditorHorizontal(() =>
							{
								

								EditorHorizontal("HelpBox", () =>
								{
									p.wildMulti = EditorGUILayout.Toggle("Wild Multiple: ", p.wildMulti);
								});

								EditorHorizontal("HelpBox", () =>
								{

									if (GUILayout.Button("Remove"))
									{
										slot.payLines.RemoveAt(i);
									}
								});

							});
						});


					});




				}

				if (GUILayout.Button("Add New"))
				{
					slot.payLines.Add(new PayLine(new int[5], 0, 0));

				}

				DragAndDropItem<GameObject>("Drag SlotController Here to overide the Paytable",
					(obj) =>
					{
						SlotController sC = obj.GetComponent<SlotController>();
						if (sC != null)
						{
							slot.payLines = new List<PayLine>(sC.payTable);
						}
					}
					);

			});
			
			
			

		}
		#endregion DrawPayTable

		#region DrawScatterPayTable
		void DrawScatterPayTable()
		{
			EditorGUILayout.LabelField("Scatter Pay Table", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
			
			EditorVentical("GroupBox",()=> 
			{
				if(slot.useScatter)
				{
					if(slot.scatter_id == -1)
					{
						slot.scatter_id = 0;
					}
					EditorVentical(() =>
					{
						EditorHorizontal(() =>
						{
							DrawSelectionIcon(ref slot.scatter_id, true, false);
						});

						for (int i = 0; i < slot.scatterPayTable.Count; i++)
						{
							EditorVentical("GroupBox", () =>
							{

								EditorHorizontal("HelpBox", () =>
								{
									EditorVentical("HelpBox", () =>
									{
										EditorHorizontal(() =>
										{
											for (int j = 0; j < slot.scatterPayTable[i].scattersCount; j++)
											{
												EditorHorizontal(() =>
												{
													DrawIcon(slot.scatter_id,()=> {
														slot.scatterPayTable[i].scattersCount--;
													},GUILayout.Width(70),GUILayout.Height(70));
												});

											}
											if (GUILayout.Button("ADD" , GUILayout.Width(70), GUILayout.Height(70)))
											{
												slot.scatterPayTable[i].scattersCount++;

											}
										});
										
										
									});

									
								});

								EditorVentical("HelpBox", () =>
								{
									EditorGUILayout.LabelField("Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
									EditorHorizontal(() =>
									{
										EditorLableCenterWithBox("Pay", () =>
										{
											slot.scatterPayTable[i].pay = EditorGUILayout.IntField(slot.scatterPayTable[i].pay);
										});
										EditorLableCenterWithBox("Free Spin", () =>
										{
											slot.scatterPayTable[i].freeSpins = EditorGUILayout.IntField(slot.scatterPayTable[i].freeSpins);
										});
										EditorLableCenterWithBox("Feature Multiply", () =>
										{
											slot.scatterPayTable[i].featureMultiply = EditorGUILayout.IntField(slot.scatterPayTable[i].featureMultiply);
										});


									});
									EditorHorizontal(() =>
									{
										EditorLableCenterWithBox("Pay Multiply", () =>
										{
											slot.scatterPayTable[i].payMult = EditorGUILayout.IntField(slot.scatterPayTable[i].payMult);
										});
										EditorLableCenterWithBox("Total Bet Multiply", () =>
										{
											slot.scatterPayTable[i].totalBetMult = EditorGUILayout.FloatField(slot.scatterPayTable[i].totalBetMult);
										});
										

									});
									slot.scatterPayTable[i].useSpecialEffect = EditorGUILayout.Toggle("Enable Special Effect", slot.scatterPayTable[i].useSpecialEffect);
								});



								EditorVentical("HelpBox", () =>
								{
									EditorGUILayout.LabelField("In Feature Mode Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
									slot.scatterPayTable[i].enableFeatureMode = EditorGUILayout.Toggle("Enable Feature Mode", slot.scatterPayTable[i].enableFeatureMode);
									if (slot.scatterPayTable[i].enableFeatureMode)
									{

										EditorHorizontal(() =>
										{
											EditorLableCenterWithBox("Pay", () =>
											{
												slot.scatterPayTable[i].featurePay = EditorGUILayout.IntField(slot.scatterPayTable[i].featurePay);
											});
											

										});
										EditorHorizontal(() =>
										{
											EditorLableCenterWithBox("Pay Multiply", () =>
											{
												slot.scatterPayTable[i].featurePayMult = EditorGUILayout.IntField(slot.scatterPayTable[i].featurePayMult);
											});
											EditorLableCenterWithBox("Free Spin", () =>
											{
												slot.scatterPayTable[i].featureFreeSpins = EditorGUILayout.IntField(slot.scatterPayTable[i].featureFreeSpins);
											});
											

										});
									}



								});

								EditorVentical("HelpBox", () =>
								{
									EditorGUILayout.LabelField("Bonus Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

									EditorLableCenterWithBox("Bunos Script", () =>
									{
										slot.scatterPayTable[i].hasBonus = EditorGUILayout.Toggle("Has Bonus", slot.scatterPayTable[i].hasBonus);
									});
									
									if (slot.scatterPayTable[i].hasBonus)
									{


										EditorHorizontal(() =>
										{
											EditorLableCenterWithBox("Bonus Sound", () =>
											{
												slot.scatterPayTable[i].soundBonusTrigger = EditorGUILayout.TextField(slot.scatterPayTable[i].soundBonusTrigger);
											});
											EditorLableCenterWithBox("", () =>
											{
												slot.scatterPayTable[i].enablePauseAutoPlay = EditorGUILayout.Toggle("Enable Pause: ", slot.scatterPayTable[i].enablePauseAutoPlay);
											});
											

										});
									}



								});

								if (GUILayout.Button("Remove", GUILayout.ExpandHeight(true)))
								{
									slot.scatterPayTable.RemoveAt(i);
									return;
								}


							});
							

						}

						if (GUILayout.Button("ADD"))
						{
							slot.scatterPayTable.Add(new ScatterPay());
						}

						
					});
				}
				else
				{
					EditorHorizontal("HelpBox", () =>
					{
						slot.useScatter = EditorGUILayout.Toggle("Enable Scatter: ", slot.useScatter);
					});
				}
				DragAndDropItem<GameObject>("Drag and Drop SlotController here.", (item) => 
				{
					SlotController sC = item.GetComponent<SlotController>();
					if(sC != null)
					{
						slot.scatterPayTable = new List<ScatterPay>(sC.scatterPayTable);
					}
				});
				

			}, GUILayout.ExpandWidth(true));

		}
		#endregion DrawScatterPayTable

		#region DrawSpecialIconPayTable
		void DrawSpecialIconPayTable()
		{
			EditorGUILayout.LabelField("Special Icon Table", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

			EditorVentical("GroupBox", () =>
			{
				if (slot.useSpecialIcon)
				{
					if (slot.specialIcon_id == -1)
					{
						slot.specialIcon_id = 0;
					}
					EditorVentical(() =>
					{
						EditorHorizontal(() =>
						{
							DrawSelectionIcon(ref slot.specialIcon_id, true, false);
						});

						for (int i = 0; i < slot.specialIconPayTable.Count; i++)
						{
							EditorVentical("GroupBox", () =>
							{

								EditorHorizontal("HelpBox", () =>
								{
									EditorVentical("HelpBox", () =>
									{
										EditorHorizontal(() =>
										{
											for (int j = 0; j < slot.specialIconPayTable[i].specialIconsCount; j++)
											{
												EditorHorizontal(() =>
												{
													DrawIcon(slot.specialIcon_id, () => {
														slot.specialIconPayTable[i].specialIconsCount--;
													}, GUILayout.Width(70), GUILayout.Height(70));
												});

											}
											if (GUILayout.Button("ADD", GUILayout.Width(70), GUILayout.Height(70)))
											{
												slot.specialIconPayTable[i].specialIconsCount++;

											}
										});


									});


								});

								EditorVentical("HelpBox", () =>
								{
									EditorGUILayout.LabelField("Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
									EditorHorizontal(() =>
									{
										EditorLableCenterWithBox("Pay", () =>
										{
											slot.specialIconPayTable[i].pay = EditorGUILayout.IntField(slot.specialIconPayTable[i].pay);
										});
										EditorLableCenterWithBox("Free Spin", () =>
										{
											slot.specialIconPayTable[i].freeSpins = EditorGUILayout.IntField(slot.specialIconPayTable[i].freeSpins);
										});

										EditorLableCenterWithBox("Feature Multiply", () =>
										{
											slot.specialIconPayTable[i].featureMultiply = EditorGUILayout.IntField(slot.specialIconPayTable[i].featureMultiply);
										});
									});
									EditorHorizontal(() =>
									{
										EditorLableCenterWithBox("Pay Multiply", () =>
										{
											slot.specialIconPayTable[i].payMult = EditorGUILayout.IntField(slot.specialIconPayTable[i].payMult);
										});
										EditorLableCenterWithBox("Total Bet Multiply", () =>
										{
											slot.specialIconPayTable[i].totalBetMult = EditorGUILayout.FloatField(slot.specialIconPayTable[i].totalBetMult);
										});


									});
									slot.specialIconPayTable[i].useSpecialEffect = EditorGUILayout.Toggle("Enable Special Effect", slot.specialIconPayTable[i].useSpecialEffect);
								});



								EditorVentical("HelpBox", () =>
								{
									EditorGUILayout.LabelField("In Feature Mode Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
									slot.specialIconPayTable[i].enableFeatureMode = EditorGUILayout.Toggle("Enable Feature Mode", slot.specialIconPayTable[i].enableFeatureMode);
									if (slot.specialIconPayTable[i].enableFeatureMode)
									{

										EditorHorizontal(() =>
										{
											EditorLableCenterWithBox("Pay", () =>
											{
												slot.specialIconPayTable[i].featurePay = EditorGUILayout.IntField(slot.specialIconPayTable[i].featurePay);
											});


										});
										EditorHorizontal(() =>
										{
											EditorLableCenterWithBox("Pay Multiply", () =>
											{
												slot.specialIconPayTable[i].featurePayMult = EditorGUILayout.IntField(slot.specialIconPayTable[i].featurePayMult);
											});
											EditorLableCenterWithBox("Free Spin", () =>
											{
												slot.specialIconPayTable[i].featureFreeSpins = EditorGUILayout.IntField(slot.specialIconPayTable[i].featureFreeSpins);
											});


										});
									}



								});

								EditorVentical("HelpBox", () =>
								{
									EditorGUILayout.LabelField("Bonus Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

									EditorLableCenterWithBox("Bunos Script", () =>
									{
										slot.specialIconPayTable[i].hasBonus = EditorGUILayout.Toggle("Has Bonus", slot.specialIconPayTable[i].hasBonus);
									});

									if (slot.specialIconPayTable[i].hasBonus)
									{


										EditorHorizontal(() =>
										{
											EditorLableCenterWithBox("Bonus Sound", () =>
											{
												slot.specialIconPayTable[i].soundBonusTrigger = EditorGUILayout.TextField(slot.specialIconPayTable[i].soundBonusTrigger);
											});
											EditorLableCenterWithBox("", () =>
											{
												slot.specialIconPayTable[i].enablePauseAutoPlay = EditorGUILayout.Toggle("Enable Pause: ", slot.specialIconPayTable[i].enablePauseAutoPlay);
											});


										});
									}



								});

								if (GUILayout.Button("Remove", GUILayout.ExpandHeight(true)))
								{
									slot.specialIconPayTable.RemoveAt(i);
									return;
								}


							});


						}

						if (GUILayout.Button("ADD"))
						{
							slot.specialIconPayTable.Add(new SpecialIconPay());
						}


					});
				}
				else
				{
					
					EditorHorizontal("HelpBox", () =>
					{
						slot.useSpecialIcon = EditorGUILayout.Toggle("Enable Scatter: ", slot.useSpecialIcon);
					});
				}
				DragAndDropItem<GameObject>("Drag and Drop SlotController here.", (item) =>
				{
					SlotController sC = item.GetComponent<SlotController>();
					if (sC != null)
					{
						slot.specialIconPayTable = new List<SpecialIconPay>(sC.specialIconPayTable);
					}
				});


			}, GUILayout.ExpandWidth(true));

		}
		#endregion DrawSpecialIconPayTable

		#region DrawWildPayTable
		void DrawWildPayTable()
		{
			EditorGUILayout.LabelField("Wild Pay Table", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

			EditorVentical("GroupBox", () =>
			{
				if (slot.useWild)
				{
					if (slot.wild_id == -1)
					{
						slot.wild_id = 0;
					}
					EditorVentical(() =>
					{
						EditorHorizontal(() =>
						{
							DrawSelectionIcon(ref slot.wild_id, true, false);
						});

						for (int i = 0; i < slot.wildPayTable.Count; i++)
						{

							EditorVentical("GroupBox", () =>
							{

								EditorHorizontal("HelpBox", () =>
								{
									EditorVentical("HelpBox", () =>
									{
										EditorHorizontal(() =>
										{
											for (int j = 0; j < slot.wildPayTable[i].wildCount; j++)
											{
												EditorHorizontal(() =>
												{
													DrawIcon(slot.wild_id, () => {
														slot.wildPayTable[i].wildCount--;
													}, GUILayout.Width(70), GUILayout.Height(70));
												});

											}
											if (GUILayout.Button("ADD", GUILayout.Width(70), GUILayout.Height(70)))
											{
												slot.wildPayTable[i].wildCount++;

											}
										});


									});


								});

								EditorVentical("HelpBox", () =>
								{
									EditorGUILayout.LabelField("Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
									EditorHorizontal(() =>
									{
										EditorLableCenterWithBox("Pay", () =>
										{
											slot.wildPayTable[i].pay = EditorGUILayout.IntField(slot.wildPayTable[i].pay);
										});
										EditorLableCenterWithBox("Free Spin", () =>
										{
											slot.wildPayTable[i].freeSpins = EditorGUILayout.IntField(slot.wildPayTable[i].freeSpins);
										});
										EditorLableCenterWithBox("Feature Multiply", () =>
										{
											slot.wildPayTable[i].featureMultiply = EditorGUILayout.IntField(slot.wildPayTable[i].featureMultiply);
										});

									});
									EditorHorizontal(() =>
									{
										EditorLableCenterWithBox("Pay Multiply", () =>
										{
											slot.wildPayTable[i].payMult = EditorGUILayout.IntField(slot.wildPayTable[i].payMult);
										});
										EditorLableCenterWithBox("Total Bet Multiply", () =>
										{
											slot.wildPayTable[i].totalBetMult = EditorGUILayout.FloatField(slot.wildPayTable[i].totalBetMult);
										});
										

									});
									slot.wildPayTable[i].useSpecialEffect = EditorGUILayout.Toggle("Enable Special Effect", slot.wildPayTable[i].useSpecialEffect);
								});



								EditorVentical("HelpBox", () =>
								{
									EditorGUILayout.LabelField("In Feature Mode Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
									slot.wildPayTable[i].enableFeatureMode = EditorGUILayout.Toggle("Enable Feature Mode", slot.wildPayTable[i].enableFeatureMode);
									if (slot.wildPayTable[i].enableFeatureMode)
									{

										EditorHorizontal(() =>
										{
											EditorLableCenterWithBox("Pay", () =>
											{
												slot.wildPayTable[i].featurePay = EditorGUILayout.IntField(slot.wildPayTable[i].featurePay);
											});
											

										});
										EditorHorizontal(() =>
										{
											EditorLableCenterWithBox("Pay Multiply", () =>
											{
												slot.wildPayTable[i].featurePayMult = EditorGUILayout.IntField(slot.wildPayTable[i].featurePayMult);
											});
											EditorLableCenterWithBox("Free Spin", () =>
											{
												slot.wildPayTable[i].featureFreeSpins = EditorGUILayout.IntField(slot.wildPayTable[i].featureFreeSpins);
											});
											

										});
									}



								});

								EditorVentical("HelpBox", () =>
								{
									EditorGUILayout.LabelField("Bonus Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

									EditorLableCenterWithBox("Bunos Script", () =>
									{
										slot.wildPayTable[i].hasBonus = EditorGUILayout.Toggle("Has Bonus", slot.wildPayTable[i].hasBonus);
									});
									
									if (slot.wildPayTable[i].hasBonus)
									{


										EditorHorizontal(() =>
										{
											EditorLableCenterWithBox("Bonus Sound", () =>
											{
												slot.wildPayTable[i].soundBonusTrigger = EditorGUILayout.TextField(slot.wildPayTable[i].soundBonusTrigger);
											});
											EditorLableCenterWithBox("", () =>
											{
												slot.wildPayTable[i].enablePauseAutoPlay = EditorGUILayout.Toggle("Enable Pause: ", slot.wildPayTable[i].enablePauseAutoPlay);
											});
											

										});
									}



								});

								if (GUILayout.Button("Remove", GUILayout.ExpandHeight(true)))
								{
									slot.wildPayTable.RemoveAt(i);
									return;
								}


							});


						}

						if (GUILayout.Button("ADD"))
						{
							slot.wildPayTable.Add(new WildPay());
						}


					});
				}
				else
				{
					EditorHorizontal("HelpBox", () =>
					{
						slot.useWild = EditorGUILayout.Toggle("Enable Scatter: ", slot.useWild);
					});
				}
				DragAndDropItem<GameObject>("Drag and Drop SlotController here.", (item) =>
				{
					SlotController sC = item.GetComponent<SlotController>();
					if (sC != null)
					{
						slot.wildPayTable = new List<WildPay>(sC.wildPayTable);
					}
				});


			}, GUILayout.ExpandWidth(true));

		}
		#endregion DrawWildPayTable

		#region DrawSlotGroup
		void DrawSlotGroup()
		{
			EditorGUILayout.LabelField("Slot Group Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
			if (GUILayout.Button("ADD SLOT GROUP", GUILayout.Width(200)))
			{
				slot.slotGroups.Add(new SlotGroupSetting());


			}
			EditorHorizontal(() =>
			{
				//if(slot.slotGroups.Count == 0)
				//{
				//	slot.slotGroups = new List<SlotGroupSetting>() { 
				//		new SlotGroupSetting(),
				//		new SlotGroupSetting(),
				//		new SlotGroupSetting(),
				//		new SlotGroupSetting(),
				//		new SlotGroupSetting()
				//	};
				//}
				
				for (int i = 0; i< slot.slotGroups.Count; i++)
				{
					
					int removeId = i;
					bool hasChanges = false;

					EditorVentical("Box",() =>
					{
						EditorGUILayout.LabelField("Slot Group "+ (i+1), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
						EditorDrawTrans("Transform", slot.slotGroups[i].transSetting, null);
						EditorVentical("HelpBox", () =>
						{
							EditorGUILayout.LabelField("Additional Spin before stop", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
							EditorLableCenterWithBox("Additional Spin", () =>
							{
								slot.slotGroups[i].additionalSpinBeforeStop = EditorGUILayout.IntField(slot.slotGroups[i].additionalSpinBeforeStop);
							});


						}, GUILayout.Width(300));
						EditorVentical("HelpBox", () =>
						{
							EditorGUILayout.LabelField("Override Masking Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
							slot.slotGroups[i].changeMaskingTrans = EditorGUILayout.Toggle("Enable", slot.slotGroups[i].changeMaskingTrans);
							if (slot.slotGroups[i].changeMaskingTrans)
							{
								EditorDrawTrans("Masking Transform", slot.slotGroups[i].maskingTransSetting, null);

								EditorDrawTrans("Special Effect Transform", slot.slotGroups[i].specialEffectTransSetting, null);
							}
							
						}, GUILayout.Width(300));
						if (slot.slotGroups[i].listOfRayCastTrans.Count == 0)
                        {
							SlotController slotC = FindObjectOfType<SlotController>();
							if(slotC != null)
                            {
								if(slotC.slotGroupsBeh.Length > i)
                                {
									
									foreach(RayCaster rC in slotC.slotGroupsBeh[i].RayCasters)
                                    {
										slot.slotGroups[i].listOfRayCastTrans.Add(new TransfomSetting(rC.transform));
										
									}
									

								}
								

							}
						}
						EditorVentical("HelpBox", () => 
						{
							EditorGUILayout.LabelField("RayCast setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

							if (GUILayout.Button("ADD RAYCAST"))
							{
								slot.slotGroups[i].listOfRayCastTrans.Add(new TransfomSetting());


							}
							DrawLine();
							for (int j = 0; j < slot.slotGroups[i].listOfRayCastTrans.Count; j++)
							{
								EditorDrawTrans("Raycast Trans", slot.slotGroups[i].listOfRayCastTrans[j], null);
								if (GUILayout.Button("REMOVE RAYCAST"))
								{
									slot.slotGroups[i].listOfRayCastTrans.RemoveAt(j);


									break;
								}
								DrawLine();
							}

						}, GUILayout.Width(300));
						
						
						if (GUILayout.Button("REMOVE SLOT GROUP"))
						{
							slot.slotGroups.RemoveAt(removeId);
							hasChanges = true;

							return;
						}
						EditorVentical("HelpBox", () => {
							EditorGUILayout.LabelField("Row Icon Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
							
							for (int j = 0; j < slot.slotGroups[i].symbOrder.Count; j++)
							{
								if(slot.slotIcons.Count < slot.slotGroups[i].symbOrder[j])
								{
									slot.slotGroups[i].symbOrder.RemoveAt(j);
									if (slot.slotGroups[i].symbOrderFeature.Count > j)
									{
										slot.slotGroups[i].symbOrderFeature.RemoveAt(j);

									}
									break;
									
								}
								else
								{
									int idFeature = -1;
									if (slot.slotGroups[i].symbOrderFeature == null) slot.slotGroups[i].symbOrderFeature = new List<int>();
									if (slot.slotGroups[i].symbOrderFeature.Count <= j)
									{
										slot.slotGroups[i].symbOrderFeature.Add(slot.slotGroups[i].symbOrder[j]);
										
									}
									idFeature = slot.slotGroups[i].symbOrderFeature[j];
									int id = slot.slotGroups[i].symbOrder[j];
									EditorHorizontal(() => {
										EditorVentical("Box", () =>
										{
											EditorGUILayout.LabelField("Standard", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

											DrawSelectionIcon(ref id, true, false);
										});
										EditorVentical("Box", () =>
										{
											EditorGUILayout.LabelField("Feature", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

											DrawSelectionIcon(ref idFeature, true, false);
										});
									});
									


									slot.slotGroups[i].symbOrder[j] = id;
									slot.slotGroups[i].symbOrderFeature[j] = idFeature;
									if (GUILayout.Button("REMOVE"))
									{
										slot.slotGroups[i].symbOrder.RemoveAt(j);
										slot.slotGroups[i].symbOrderFeature.RemoveAt(j);
										break;
									}
								}
								
							}
							
							if (GUILayout.Button("ADD NEW"))
							{
								slot.slotGroups[i].symbOrder.Add(0);
								slot.slotGroups[i].symbOrderFeature.Add(0);
							}
						}, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));



					}, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    if (hasChanges)
                    {
						break;
                    }
				}
				
			}, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			


			

		}
		#endregion DrawSlotGroup

		#region DrawFeatureSetting
		void DrawFeatureSetting()
		{
			EditorGUILayout.LabelField("Feature Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

			EditorVentical("GroupBox", () => 
			{
				EditorLableCenterWithBox("Feature Multiply", () =>
				{
					slot.featureMultiply = EditorGUILayout.IntField(slot.featureMultiply);
				});
				
			});
			DragAndDropItem<GameObject>("Drag and Drop SlotController here.", (item) =>
			{
				SlotController sC = item.GetComponent<SlotController>();
				if (sC != null)
				{
					slot.featureMultiply = sC.featureMultiply;
				}
			});
		}
		#endregion DrawFeatureSetting

		#region DrawBonusSetting
		void DrawBonusSetting()
		{
			EditorGUILayout.LabelField("Bonus Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
			EditorLableCenterWithBox("Bonus Prefab", () =>
			{
				slot.bonusGameObj = (GameObject)EditorGUILayout.ObjectField(slot.bonusGameObj, typeof(GameObject), false);
			});
			if(slot.bonusGameObj != null)
			{
				EditorLableCenterWithBox("disable Bonus On Feature", () =>
				{
					slot.disableBonusOnFeature = EditorGUILayout.Toggle(slot.disableBonusOnFeature);
				});
				EditorLableCenterWithBox("Number Of Pick", () =>
				{
					slot.numberOfBonusPick = EditorGUILayout.IntField(slot.numberOfBonusPick);
				});
				EditorLableCenterWithBox("Number Of Minimum Pick", () =>
				{
					slot.numberOfMinBonusPick = EditorGUILayout.IntField(slot.numberOfMinBonusPick);
				});
				EditorLableCenterWithBox("initial feature Multiply Win", () =>
				{
					slot.initialMultiplyWin = EditorGUILayout.IntField(slot.initialMultiplyWin);
				});
				EditorLableCenterWithBox("initial FreeSpin Win", () =>
				{
					slot.initialFreeSpinWin = EditorGUILayout.IntField(slot.initialFreeSpinWin);
				});
				EditorLableCenterWithBox("initial Multiply Win By Bet", () =>
				{
					slot.initialMultiplyWinByBet = EditorGUILayout.IntField(slot.initialMultiplyWinByBet);
				});
				EditorVentical("GroupBox", () =>
				{
					for (int i = 0; i < slot.bonusChoices.Count; i++)
					{
						EditorGUILayout.LabelField("ID: " + i, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
						EditorVentical("HelpBox", () =>
						{
							EditorLableCenterWithBox("Freespin Win", () =>
							{
								slot.bonusChoices[i].NumberOfFreeSpin = EditorGUILayout.IntField(slot.bonusChoices[i].NumberOfFreeSpin);
							});
							EditorLableCenterWithBox("Bet Multiply", () =>
							{
								slot.bonusChoices[i].NumberOfMultiply = EditorGUILayout.IntField(slot.bonusChoices[i].NumberOfMultiply);
							});
							EditorLableCenterWithBox("Feature Multiply", () =>
							{
								slot.bonusChoices[i].WinMultiply = EditorGUILayout.IntField(slot.bonusChoices[i].WinMultiply);
							});
							EditorLableCenterWithBox("Add Feature Multiply instead", () =>
							{
								slot.bonusChoices[i].AddFeatureMultiply = EditorGUILayout.Toggle(slot.bonusChoices[i].AddFeatureMultiply);
							});
							EditorLableCenterWithBox("Trigger End Game", () =>
							{
								slot.bonusChoices[i].TriggerEndGame = EditorGUILayout.Toggle(slot.bonusChoices[i].TriggerEndGame);
							});
							if (GUILayout.Button("Remove", GUILayout.ExpandHeight(true)))
							{
								slot.bonusChoices.RemoveAt(i);
								return;
							}
						}, GUILayout.Height(100));
					}


					if (GUILayout.Button("ADD"))
					{
						slot.bonusChoices.Add(new BonusGameChoicesType());
					}

				});
				//DragAndDropItem<GameObject>("Drag and Drop SlotController here.", (item) =>
				//{
				//	SlotController sC = item.GetComponent<SlotController>();
				//	if (sC != null)
				//	{
				//		slot.featureMultiply = sC.featureMultiply;
				//	}
				//});
			}

		}
		#endregion DrawBonusSetting


		#region LineSetting
		void DrawLineSetting()
		{
			EditorGUILayout.LabelField("Line Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
			if(slot.listOfLineSetting.Count > 0)
			{
				EditorHorizontal("HelpBox", () => 
				{
					
					List<string> lineName = new List<string>();
					for (int i = 0; i < slot.listOfLineSetting.Count; i++)
					{
						lineName.Add("Line " + (i + 1));
					}
					if(currentSelectedLineSetting > slot.listOfLineSetting.Count -1)
                    {
						currentSelectedLineSetting = slot.listOfLineSetting.Count - 1;

					}
					currentSelectedLineSetting = EditorGUILayout.Popup("Select Line", currentSelectedLineSetting, lineName.ToArray());
					if (GUILayout.Button("ADD Line"))
					{
						slot.listOfLineSetting.Add(new LineSetting());
					}

				}, GUILayout.Width(500));

				if (slot.listOfLineSetting.Count > 0)
				{
					LineSetting lS = slot.listOfLineSetting[currentSelectedLineSetting];
					if (lS.animationWinLine == null)
                    {
						lS.animationWinLine = new LineSetting.AnimationWinLine();
                    }
					EditorVentical(() =>
					{
						EditorVentical("HelpBox", () =>
						{
							EditorGUILayout.LabelField("Line " + (currentSelectedLineSetting + 1), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
							EditorVentical("GroupBox", () =>
							{
								EditorGUILayout.LabelField("Win Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
								EditorHorizontal(() =>
								{
									for (int j = 0; j < slot.slotGroups.Count; j++)
									{
										if(slot.listOfLineSetting[currentSelectedLineSetting].slotGroupBehavior.Count -1 < j)
                                        {
											slot.listOfLineSetting[currentSelectedLineSetting].slotGroupBehavior.Add(new SlotRayCastId());

										}
										SlotRayCastId sRCId = slot.listOfLineSetting[currentSelectedLineSetting].slotGroupBehavior[j];
										EditorVentical(() =>
										{
											for (int k = 0; k < slot.slotGroups[j].listOfRayCastTrans.Count; k++)
											{
												var style = new GUIStyle(GUI.skin.button);
												Texture2D texNormal;
												Texture2D texSelected;
												texNormal = new Texture2D(1, 1);
												texSelected = new Texture2D(1, 1);
												texSelected.SetColor(new Color32(155, 155, 155, 255));
												texNormal.SetColor(new Color32(255, 255, 255, 255));
												string buttonName = "Select";
												if(sRCId.ids.Count -1 < k)
                                                {
													sRCId.ids.Add(-1);

												}
												if (sRCId.ids[k] >= 0)
												{
													style.normal.background = texSelected;
													buttonName = "Selected";
												}
												else
												{
													style.normal.background = texNormal;
												}
												
												if (GUILayout.Button(buttonName, style, GUILayout.Height(100)))
												{
													sRCId.Reset();
													sRCId.ids[k] = 1;
												}
											}

											if(sRCId.ids.Count > slot.slotGroups[j].listOfRayCastTrans.Count)
                                            {
												List<int> ids = new List<int>();
												for(int z = 0; z< slot.slotGroups[j].listOfRayCastTrans.Count; z++)
                                                {
													ids.Add(sRCId.ids[z]);

												}
												sRCId.ids = ids;

											}
										});
									}

								});
							});

							EditorLableCenterWithBox("Line Material", () =>
							{
								lS.lineMaterial = (Material)EditorGUILayout.ObjectField(lS.lineMaterial, typeof(Material), false);
							});

							EditorLableCenterWithBox("Line Color", () =>
							{
								lS.lineColor = EditorGUILayout.ColorField(lS.lineColor);
							});

							EditorLableCenterWithBox("Line Flashing Speed", () =>
							{
								lS.lineFlashingSpeed = EditorGUILayout.Slider(lS.lineFlashingSpeed, 1f, 10f);
								for (int l = 0; l < slot.listOfLineSetting.Count; l++) 
								{
									slot.listOfLineSetting[l].lineFlashingSpeed = lS.lineFlashingSpeed;
								}

							});

							EditorLableCenterWithBox("Line Width", () =>
							{
								lS.lineRendererWidth = EditorGUILayout.Slider(lS.lineRendererWidth, 1f, 10f);
								
								for (int l = 0; l < slot.listOfLineSetting.Count; l++)
								{
									slot.listOfLineSetting[l].lineRendererWidth = lS.lineRendererWidth;
								}
							});

							EditorLableCenterWithBox("Line Speed", () =>
							{
								lS.lineSpeed = EditorGUILayout.Slider(lS.lineSpeed, 50f, 200f);
								for (int l = 0; l < slot.listOfLineSetting.Count; l++)
								{
									slot.listOfLineSetting[l].lineSpeed = lS.lineSpeed;
								}
							});

							EditorLableCenterWithBox("Box Display Type", () =>
							{
								lS.typeOfBoxWinning = (WinningBoxSequenceType)EditorGUILayout.EnumPopup(lS.typeOfBoxWinning);
								for (int l = 0; l < slot.listOfLineSetting.Count; l++)
								{
									slot.listOfLineSetting[l].typeOfBoxWinning = lS.typeOfBoxWinning;
								}
							});

							EditorLableCenterWithBox("Wining Box Size", () =>
							{
								EditorHorizontal(() =>
								{
									EditorLableCenterWithBox("Width", () =>
									{
										lS.boxSizeWidth = EditorGUILayout.FloatField(lS.boxSizeWidth);
										for (int l = 0; l < slot.listOfLineSetting.Count; l++)
										{
											slot.listOfLineSetting[l].boxSizeWidth = lS.boxSizeWidth;
										}
									});
									EditorLableCenterWithBox("Height", () =>
									{
										lS.boxSizeHeight = EditorGUILayout.FloatField(lS.boxSizeHeight);
										for (int l = 0; l < slot.listOfLineSetting.Count; l++)
										{
											slot.listOfLineSetting[l].boxSizeHeight = lS.boxSizeHeight;
										}
									});
								});
							});
							
							lS.animationWinLine.enableAnimationLine = EditorGUILayout.Toggle("Enable Animation Line", lS.animationWinLine.enableAnimationLine);
                            if (lS.animationWinLine.enableAnimationLine)
                            {
								lS.animationWinLine.sortingOrder = EditorGUILayout.IntField("Sorting Order", lS.animationWinLine.sortingOrder);
								lS.animationWinLine.animC = (RuntimeAnimatorController)EditorGUILayout.ObjectField(lS.animationWinLine.animC, typeof(RuntimeAnimatorController), false);
								Rect r = EditorVentical("GroupBox", () =>
								{
									EditorGUILayout.LabelField("Transform", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
									EditorLableCenterWithBox("Position", () =>
									{
										EditorHorizontal(() =>
										{
											lS.animationWinLine.position.x = EditorGUILayout.FloatField(lS.animationWinLine.position.x);
											lS.animationWinLine.position.y = EditorGUILayout.FloatField(lS.animationWinLine.position.y);
											lS.animationWinLine.position.z = EditorGUILayout.FloatField(lS.animationWinLine.position.z);
										});
									});
									EditorLableCenterWithBox("Rotation", () =>
									{
										EditorHorizontal(() =>
										{
											lS.animationWinLine.rotation.x = EditorGUILayout.FloatField(lS.animationWinLine.rotation.x);
											lS.animationWinLine.rotation.y = EditorGUILayout.FloatField(lS.animationWinLine.rotation.y);
											lS.animationWinLine.rotation.z = EditorGUILayout.FloatField(lS.animationWinLine.rotation.z);

										});
									});
									EditorLableCenterWithBox("Scale", () =>
									{
										EditorHorizontal(() =>
										{
											lS.animationWinLine.scale.x = EditorGUILayout.FloatField(lS.animationWinLine.scale.x);
											lS.animationWinLine.scale.y = EditorGUILayout.FloatField(lS.animationWinLine.scale.y);
											lS.animationWinLine.scale.z = EditorGUILayout.FloatField(lS.animationWinLine.scale.z);
										});
									});

									


								});

								DragAndDropItem<GameObject>(r, (obj) =>
								{
									Transform trans = obj.GetComponent<Transform>();
									if (trans)
									{
										lS.animationWinLine.scale = trans.localScale;
										lS.animationWinLine.position = trans.localPosition;
										lS.animationWinLine.rotation = trans.localEulerAngles;
									}
								});

							}

							lS.enableButton = EditorGUILayout.Toggle("Enable Choose Line", lS.enableButton);
							if (GUILayout.Button("Remove"))
							{
								slot.listOfLineSetting.RemoveAt(currentSelectedLineSetting);
								return;
							}

						}, GUILayout.Width(500));
						EditorVentical("HelpBox", () =>
						{
							EditorGUILayout.LabelField("Button " + (currentSelectedLineSetting + 1), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
							
							EditorHorizontal(() =>
							{
								EditorVentical(() =>
								{
									EditorLableCenterWithBox("Normal", () =>
									{
										lS.uiButtonSetting.sprite = (Sprite)EditorGUILayout.ObjectField(lS.uiButtonSetting.sprite, typeof(Sprite), false);

									});
									if (lS.uiButtonSetting.sprite)
										DrawSprite(lS.uiButtonSetting.sprite, 100);
								});
								EditorVentical(() =>
								{
									EditorLableCenterWithBox("Pressed", () =>
									{
										lS.uiButtonSetting.pressedSprite = (Sprite)EditorGUILayout.ObjectField(lS.uiButtonSetting.pressedSprite, typeof(Sprite), false);

									});
									if (lS.uiButtonSetting.pressedSprite)
										DrawSprite(lS.uiButtonSetting.pressedSprite, 100);
								});
							});

							EditorDrawRectSetting("Button Rect", lS.uiButtonSetting, null);

							Rect rec = EditorDrawStateImageContol(lS.uiButtonSetting.listOfSpriteState);
							DragAndDropItem<GameObject>(rec, (obj) =>
							{
								StateControl sC = obj.GetComponent<StateControl>();
								if (sC != null)
								{
									lS.uiButtonSetting.listOfSpriteState = new List<SpriteState>(sC.listOfSpriteState);
								}

							});

						}, GUILayout.Width(500));
						EditorVentical("HelpBox", () =>
						{
							EditorGUILayout.LabelField("Button (Second) " + (currentSelectedLineSetting + 1), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
							
							EditorHorizontal(() =>
							{
								EditorVentical(() =>
								{
									EditorLableCenterWithBox("Normal", () =>
									{
										lS.uiButtonSecondSetting.sprite = (Sprite)EditorGUILayout.ObjectField(lS.uiButtonSecondSetting.sprite, typeof(Sprite), false);

									});
									if (lS.uiButtonSecondSetting.sprite)
										DrawSprite(lS.uiButtonSecondSetting.sprite, 100);
								});
								EditorVentical(() =>
								{
									EditorLableCenterWithBox("Pressed", () =>
									{
										lS.uiButtonSecondSetting.pressedSprite = (Sprite)EditorGUILayout.ObjectField(lS.uiButtonSecondSetting.pressedSprite, typeof(Sprite), false);

									});
									if (lS.uiButtonSecondSetting.pressedSprite)
										DrawSprite(lS.uiButtonSecondSetting.pressedSprite, 100);
								});
							});

							EditorDrawRectSetting("Button Rect", lS.uiButtonSecondSetting, null);

							Rect rec = EditorDrawStateImageContol(lS.uiButtonSecondSetting.listOfSpriteState);
							DragAndDropItem<GameObject>(rec, (obj) =>
							{
								StateControl sC = obj.GetComponent<StateControl>();
								if (sC != null)
								{
									lS.uiButtonSecondSetting.listOfSpriteState = new List<SpriteState>(sC.listOfSpriteState);
								}

							});

						}, GUILayout.Width(500));
					});
				}
				

				
				
			}
            else
            {
				EditorVentical("HelpBox", () =>
				{
					EditorGUILayout.LabelField("Add Single line", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
					if (GUILayout.Button("ADD Line"))
					{
						slot.listOfLineSetting.Add(new LineSetting());
					}

				}, GUILayout.Width(500));
				EditorVentical("HelpBox", () =>
				{
					EditorGUILayout.LabelField("Auto Generate All Possible way", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
					EditorLableCenterWithBox("Line Material", () =>
					{
						slot.lineAutoSetting.lineMaterial = (Material)EditorGUILayout.ObjectField(slot.lineAutoSetting.lineMaterial, typeof(Material), false);
					});

					EditorLableCenterWithBox("Line Color", () =>
					{
						slot.lineAutoSetting.lineColor = EditorGUILayout.ColorField(slot.lineAutoSetting.lineColor);
					});

					EditorLableCenterWithBox("Line Flashing Speed", () =>
					{
						slot.lineAutoSetting.lineFlashingSpeed = EditorGUILayout.Slider(slot.lineAutoSetting.lineFlashingSpeed, 1f, 10f);
						

					});

					EditorLableCenterWithBox("Line Width", () =>
					{
						slot.lineAutoSetting.lineRendererWidth = EditorGUILayout.Slider(slot.lineAutoSetting.lineRendererWidth, 1f, 10f);

						
					});

					EditorLableCenterWithBox("Line Speed", () =>
					{
						slot.lineAutoSetting.lineSpeed = EditorGUILayout.Slider(slot.lineAutoSetting.lineSpeed, 50f, 200f);
						
					});

					EditorLableCenterWithBox("Box Display Type", () =>
					{
						slot.lineAutoSetting.typeOfBoxWinning = (WinningBoxSequenceType)EditorGUILayout.EnumPopup(slot.lineAutoSetting.typeOfBoxWinning);
						
					});

					EditorLableCenterWithBox("Wining Box Size", () =>
					{
						EditorHorizontal(() =>
						{
							EditorLableCenterWithBox("Width", () =>
							{
								slot.lineAutoSetting.boxSizeWidth = EditorGUILayout.FloatField(slot.lineAutoSetting.boxSizeWidth);
								
							});
							EditorLableCenterWithBox("Height", () =>
							{
								slot.lineAutoSetting.boxSizeHeight = EditorGUILayout.FloatField(slot.lineAutoSetting.boxSizeHeight);
								
							});
						});
					});


					if (GUILayout.Button("Generate"))
					{
						List<int> colIds = new List<int>();
						for(int i = 0; i< slot.slotGroups.Count; i++)
                        {
							colIds.Add(slot.slotGroups[i].listOfRayCastTrans.Count);

						}
						GenerateAllPossibleLines(colIds);
					}

				}, GUILayout.Width(500));
			}
			EditorVentical("HelpBox", () =>
			{
				EditorGUILayout.LabelField("Clear All", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
				if (GUILayout.Button("Clear"))
				{
					slot.listOfLineSetting.Clear();
				}

			}, GUILayout.Width(500));
			//DragAndDropItem<GameObject>("Drag And Drop Line Gameobject", (obj) => 
			//{
			//	LineBehavior lineB = obj.GetComponent<LineBehavior>();
			//	if (lineB)
			//	{
			//		SlotController sC = FindObjectOfType<SlotController>();
			//		List<SlotGroupBehavior> slotGB = new List<SlotGroupBehavior>(sC.slotGroupsBeh);
			//		List<SlotRayCastId> slotGroupBehavior = new List<SlotRayCastId>();
			//		for(int j = 0; j<slotGB.Count; j++)
			//		{
			//			SlotGroupBehavior sBG = slotGB[j];

			//			SlotRayCastId sRayCastId = new SlotRayCastId();
			//			for(int i = 0; i< sBG.RayCasters.Length; i++)
			//			{

			//				if (lineB.rayCasters[j] == sBG.RayCasters[i])
			//				{
			//					sRayCastId.ids[i] = 1;

			//				}
			//				else
			//				{

			//					sRayCastId.ids[i] = -1;
			//				}
			//			}
			//			slotGroupBehavior.Add(sRayCastId);
			//		}
			//		LineSetting lS = new LineSetting();
			//		lS.slotGroupBehavior = new List<SlotRayCastId>(slotGroupBehavior);

			//		lS.lineMaterial = lineB.lineMaterial;
			//		lS.lineColor = lineB.lineColor;
			//		lS.lineFlashingSpeed = lineB.lineFlashingSpeed;
			//		lS.lineRendererWidth = lineB.lineRendererWidth;
			//		lS.lineSpeed = lineB.lineSpeed;
			//		lS.typeOfBoxWinning = lineB.typeOfBoxWinning;
			//		lS.boxSizeWidth = lineB.boxSizeWidth;

			//		lS.boxSizeHeight = lineB.boxSizeHeight; 


			//		if(lineB.uiButton != null)
			//		{
			//			lS.enableButton = true;

			//			UILineButtonBehavior uiLineButton = lineB.uiButton.GetComponent<UILineButtonBehavior>();
			//			if(uiLineButton != null)
			//			{
			//				lS.uiButtonSetting.sprite = uiLineButton.NormalSprite;
			//				lS.uiButtonSetting.pressedSprite = uiLineButton.PressedSprite;

			//			}

			//			RectTransform buttonRect = lineB.uiButton.GetComponent<RectTransform>();
			//			if(buttonRect != null)
			//			{
			//				lS.uiButtonSetting.position = new Vector2(buttonRect.anchoredPosition.x, buttonRect.anchoredPosition.y);
			//				lS.uiButtonSetting.size = buttonRect.sizeDelta;
			//				lS.uiButtonSetting.scale = buttonRect.localScale;
			//			}

			//			StateControl stateC = lineB.uiButton.GetComponent<StateControl>();
			//			if(stateC != null)
			//			{
			//				lS.uiButtonSetting.listOfSpriteState = stateC.listOfSpriteState;
			//			}

			//		}
			//		else if(lineB.uiButtonLeft != null)
			//		{
			//			lS.enableButton = true;

			//			UILineButtonBehavior uiLineButton = lineB.uiButtonLeft.GetComponent<UILineButtonBehavior>();
			//			if (uiLineButton != null)
			//			{
			//				lS.uiButtonSecondSetting.sprite = uiLineButton.NormalSprite;
			//				lS.uiButtonSecondSetting.pressedSprite = uiLineButton.PressedSprite;

			//			}

			//			RectTransform buttonRect = lineB.uiButtonLeft.GetComponent<RectTransform>();
			//			if (buttonRect != null)
			//			{
			//				lS.uiButtonSecondSetting.position = new Vector2(buttonRect.anchoredPosition.x, buttonRect.anchoredPosition.y);
			//				lS.uiButtonSecondSetting.size = buttonRect.sizeDelta;
			//				lS.uiButtonSecondSetting.scale = buttonRect.localScale;
			//			}

			//			StateControl stateC = lineB.uiButtonLeft.GetComponent<StateControl>();
			//			if (stateC != null)
			//			{
			//				lS.uiButtonSecondSetting.listOfSpriteState = stateC.listOfSpriteState;
			//			}
			//		}
			//		else
			//		{
			//			lS.enableButton = false;
			//		}

			//		slot.listOfLineSetting.Add(lS);


			//	}
			//});
		}

		void GenerateAllPossibleLines(List<int> iconCount)
        {
		
			List<int> colCount = new List<int>() ;
			foreach(int id in iconCount)
            {
				colCount.Add(0);

			}
			int totalPossible = 1;
			foreach(int c in iconCount)
            {
				totalPossible *= c;

			}


			for (int i = 0; i< totalPossible; i++)
            {

				
				for(int j = 0; j< colCount.Count; j++)
                {
					if(colCount[j] >= iconCount[j])
                    {
						colCount[j] = 0;
						if(j < colCount.Count - 1)
                        {
							colCount[j + 1]++;

						}
						
					}
					
                }
				
				List<SlotRayCastId> sRCID = new List<SlotRayCastId>();
				
				for (int ids = 0; ids<iconCount.Count;ids++)
                {
					sRCID.Add(new SlotRayCastId(colCount[ids], iconCount[ids]));
					
				}
				slot.listOfLineSetting.Add(
					new LineSetting(sRCID,
					slot.lineAutoSetting.lineMaterial,
					slot.lineAutoSetting.lineColor,
					slot.lineAutoSetting.lineFlashingSpeed,
					slot.lineAutoSetting.lineRendererWidth,
					slot.lineAutoSetting.lineSpeed,
					slot.lineAutoSetting.typeOfBoxWinning,
					slot.lineAutoSetting.boxSizeWidth,
					slot.lineAutoSetting.boxSizeHeight));

				colCount[0]++;
			
				

			}

		}
        #endregion LineSetting

        #region DrawOtherSlotControlSetting
        void DrawOtherSlotControlSetting()
		{
			EditorGUILayout.LabelField("Other Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

			EditorVentical("GroupBox", () =>
			{
				
				EditorVentical("HelpBox", () =>
				{
					slot.useARCbetSystem = EditorGUILayout.Toggle("ARC Bet", slot.useARCbetSystem);
					EditorGUILayout.LabelField("Spin Option", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
					EditorLableCenterWithBox("Spin Time", () =>
					{
						slot.mainRotateTime = EditorGUILayout.FloatField(slot.mainRotateTime);
					});
					EditorLableCenterWithBox("Special Effect Time", () =>
					{
						slot.specialEffectTime = EditorGUILayout.IntField(slot.specialEffectTime);
					});
					EditorLableCenterWithBox("Force Stop Time", () =>
					{
						slot.forceStopTime = EditorGUILayout.FloatField(slot.forceStopTime);
					});

					EditorGUILayout.LabelField("Number Display Type", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
					EditorLableCenterWithBox("Decimal Display", () =>
					{
						slot.decimalDisplay = EditorGUILayout.TextField(slot.decimalDisplay);
					});
					
					slot.enableRandomIconOnStart = EditorGUILayout.Toggle("Enable Random Icon on Start", slot.enableRandomIconOnStart);
					slot.enableSpecialEffectDuringAuto = EditorGUILayout.Toggle("Special Effect On AutoSpin",slot.enableSpecialEffectDuringAuto);
					slot.disableDisplayStandardWinOnFeature = EditorGUILayout.Toggle("Disable standard Win Info on Feature", slot.disableDisplayStandardWinOnFeature);
					slot.enableDisplayWinInfoOnFeature = EditorGUILayout.Toggle("Enable Win Info on Feature", slot.enableDisplayWinInfoOnFeature);
					slot.onlyAddCoinOnEndFeature = EditorGUILayout.Toggle("Only Add Coin on End Feature", slot.onlyAddCoinOnEndFeature);
					slot.enableDisplayWinLineOnAuto = EditorGUILayout.Toggle("Enable Win Line on Auto", slot.enableDisplayWinLineOnAuto);
					slot.enableDisplayZeroFreespin = EditorGUILayout.Toggle("Enable display zero freespin", slot.enableDisplayZeroFreespin);
					slot.usePresetBet = EditorGUILayout.Toggle("Enable preset bet", slot.usePresetBet);
					if (slot.usePresetBet)
					{
						EditorVentical("HelpBox", () => {
							EditorGUILayout.LabelField("Bet Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
							
							for(int b = 0; b < slot.presetBetIncrease.Count; b++)
							{
								bool isRemove = false;
								EditorHorizontal(() => {
									slot.presetBetIncrease[b] = EditorGUILayout.FloatField((float)slot.presetBetIncrease[b]);
									if (GUILayout.Button("Remove"))
									{
										isRemove = true;
										slot.presetBetIncrease.RemoveAt(b);
										
									}
								});
								if (isRemove)
								{
									break;
								}
							}
							if (GUILayout.Button("ADD 100"))
							{
								if(slot.presetBetIncrease.Count > 0)
                                {
									slot.presetBetIncrease.Add(slot.presetBetIncrease[slot.presetBetIncrease.Count - 1] + 100);
								}
                                else
                                {
									slot.presetBetIncrease.Add(100);
								}
								
							}
							if (GUILayout.Button("ADD"))
							{
								slot.presetBetIncrease.Add(0);
							}
							slot.enableStartPresetBet = EditorGUILayout.Toggle("Enable Preset on Start", slot.enableStartPresetBet);
                            if (slot.enableStartPresetBet)
                            {
								List<string> idsPresetBet = new List<string>();
								for(int id = 0; id< slot.presetBetIncrease.Count; id++)
                                {
									idsPresetBet.Add(slot.presetBetIncrease[id].ToString("n2"));
								}
								
								slot.currentSelectedPresetId = EditorGUILayout.Popup("Pick A bet", slot.currentSelectedPresetId, idsPresetBet.ToArray());

							}
						}, GUILayout.Width(400));
						
					}
				}, GUILayout.Width(300));
				EditorVentical("HelpBox", () =>
				{
					EditorGUILayout.LabelField("Slot Icon Size", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
					EditorLableCenterWithBox("Width", () =>
					{
						slot.tileSize.x = EditorGUILayout.FloatField(slot.tileSize.x);
					});
					EditorLableCenterWithBox("Height", () =>
					{
						slot.tileSize.y = EditorGUILayout.FloatField(slot.tileSize.y);
					});

				}, GUILayout.Width(400));
				EditorVentical("HelpBox", () =>
				{
					EditorGUILayout.LabelField("Sound", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
					EditorLableCenterWithBox("Sound Prefabs", () =>
					{
						slot.soundGameObj = (GameObject)EditorGUILayout.ObjectField(slot.soundGameObj, typeof(GameObject),false);
						
					});
					

				}, GUILayout.Width(400));
				
			});

			DragAndDropItem<GameObject>("Drag and Drop SlotController here.", (item) =>
			{
				SlotController sC = item.GetComponent<SlotController>();
				if (sC != null)
				{
					slot.mainRotateTime = sC.mainRotateTime;
					
					slot.tileSize = new Vector2( sC.tileX, sC.tileY);
				}
			});
		}
		#endregion DrawOtherSlotControlSetting

		#region DrawRTP

		Vector2 scrollRect2;
		Vector2 scrollRect3;
		[Serializable]
		public class slotIconGroup
		{
			public List<slotIconPercent> groupIcon;

			public slotIconGroup()
			{
				groupIcon = new List<slotIconPercent>();
			}

			public List<int> GetPattern(int scatter, ref float repeatPercent, ref int idRepeat)
			{
				List<int> pattern = new List<int>();
				List<int> temp = new List<int>();
				
				foreach(slotIconPercent p  in groupIcon)
				{
					int count = (int)(p.percent * 1000);
					for(int i = 0; i< count; i++)
					{
						pattern.Add(p.slotIconId);
					}
				}

				//shuffer

				return Randomize(scatter, pattern, ref repeatPercent, ref idRepeat);
			}

		}
		public static List<int> Randomize(int scatter ,List<int> list, ref float percentRepeat,ref int idRepeat)
		{
			List<int> randomizedList = new List<int>();
			System.Random rnd = new System.Random();
			int previousId = -100;
			
			int counter = 0;
			int scatterCounter = 0;
			bool gotScatter = false;
			float counterRepeat = 0;
			int repeatId = -100;
			bool scatterRepeat = false;
			while (list.Count > 0)
			{
				int index = rnd.Next(0, list.Count); //pick a random item from the master list
				counter ++;
				

				if (previousId != list[index] || counter > 100)
				{
					
					if (scatter == list[index])
					{
						gotScatter = true;
						
					}
					if (gotScatter)
					{
						scatterCounter++;
						
						if (scatterCounter >= 3)
						{
							
							if (scatter == list[index] && counter < 100)
							{
								
								scatterRepeat = true;
							}
							else
							{
								if(scatter == list[index])
								{
									idRepeat = scatter;
								}
								gotScatter = false;
								scatterCounter = 0;
								scatterRepeat = false;
							}
						}
					}
					
					if (!scatterRepeat)
					{
						if (list.Count == 1)
						{
							if (list[index] == randomizedList[0])
							{
								counterRepeat++;
								repeatId = previousId;
							}
						}

						if (previousId == list[index])
						{
							counterRepeat++;
							repeatId = previousId;
						}
						counter = 0;
						previousId = list[index];
						randomizedList.Add(list[index]); //place it at the end of the randomized list
						list.RemoveAt(index);
					}
					
				}
				
			}
			idRepeat = repeatId;
			percentRepeat = counterRepeat / (float)randomizedList.Count * 100;
			
			return randomizedList;
		}
		[Serializable]
		public class slotIconPercent
		{
			public float setPercent;
			public float percent;
			public int slotIconId;
		}
		[Serializable]
		public class SavePattern
		{
			public List<int> sysbolId = new List<int>();
			public float repeatRate;
			public int repeatId;
		}

		float RTPCal = 0;
		List<float> payRateGroup = new List<float>();
		int totalLine = 1;
		int numberOfRaycast = 3;
		List<slotIconGroup> col = new List<slotIconGroup>();
		
		
		List<SavePattern> save = new List<SavePattern>();
		

		string savePath = "";
		string saveFileName = "";
		int repeatSpinTest = 10;
		
		string pathStoreWinSet = "";
		string saveReportPath = "";
		float winRateAbove = 5;
		string displayCalDummyDone = "";
		List<string> resultCon = new List<string>() { "", "", "", "", "" };
		void DrawRTP()
		{
			//EditorGUILayout.LabelField("RTP", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
			EditorVentical("Box", () => 
			{
				scrollRect2 = EditorGUILayout.BeginScrollView(scrollRect2,  GUILayout.ExpandWidth(true));
				for(int i = 0; i < slot.slotGroups.Count; i++)
                {
					if(slot.rtp.col.Count -1 < i)
                    {
						slot.rtp.col.Add(new List<ProbabilitySlotIcon>());

					}
					RtpCol(slot.rtp.col[i]);
				}
				
				
				EditorGUILayout.EndScrollView();
			});

			

			EditorHorizontal(() =>
			{
				

					for(int i = 0; i< slot.slotGroups.Count; i++)
					{
						if(col.Count - 1 < i)
						{
							col.Add(new slotIconGroup());
						}
						if (slot.rtp.col.Count - 1 < i)
						{
							slot.rtp.col.Add(new List<ProbabilitySlotIcon>());
						}
						slotIconGroup s = col[i];
						CalculateSlotIconPercent(ref s, slot.rtp.col[i], numberOfRaycast);
						col[i] = s;
					}
					


					


					

				EditorHorizontal("Box", () =>
				{
					EditorVentical(() =>
					{
						totalLine = EditorGUILayout.IntField("Total Line Bet", totalLine);
						numberOfRaycast = EditorGUILayout.IntField("Number of Raycast per col", numberOfRaycast);
						for(int i = 0; i< slot.slotGroups.Count; i++)
                        {
							int id = i;
							if(save.Count - 1 < i)
                            {
								save.Add(new SavePattern());
                            }
							if (resultCon.Count - 1 < i)
							{
								resultCon.Add("");
							}
							
							EditorVentical("HelpBox", () =>
							{
								if (GUILayout.Button("COL " + (id + 1)))
								{

									save[id].sysbolId = col[id].GetPattern(slot.scatter_id, ref save[id].repeatRate, ref save[id].repeatId);
									if (save[id].repeatId <= -10)
										resultCon[id] = "col" + (id + 1) + " >> Done";
									else
										resultCon[id] = "col" + (id + 1) + " >> Repeat: " + save[id].repeatRate + "% id: " + save[id].repeatId;

								}
								EditorGUILayout.LabelField(resultCon[id]);
							});
						}
						

						
						savePath = EditorGUILayout.TextField("Path:", savePath);
						saveFileName = EditorGUILayout.TextField("FIle Name:", saveFileName);
						if (GUILayout.Button("Generate Result"))
						{
							string saveString = "[";
							for (int i = 0; i< save.Count; i++)
                            {
								if(i != 0)
                                {
									saveString += ",";

								}
								saveString += JsonUtility.ToJson(save[i]);
							}
							saveString += "]";


							System.IO.File.WriteAllText(savePath + saveFileName, saveString);
						}
					}, GUILayout.Width(300));


					EditorVentical(() => {
						repeatSpinTest = EditorGUILayout.IntField("Repeat :", repeatSpinTest);
						saveReportPath = EditorGUILayout.TextField("Save Report Path :", saveReportPath);
						winRateAbove = EditorGUILayout.FloatField("Store Win When Rate above", winRateAbove);
						pathStoreWinSet = EditorGUILayout.TextField("Store Win Set Path :", pathStoreWinSet);


						if (GUILayout.Button("Generate Pattern"))
						{

							List<List<int>> temp = new List<List<int>>();
							foreach(SavePattern sP in save)
                            {
								temp.Add(sP.sysbolId);
							}
							
							EditorCoroutines.Execute(GeneratePayLineArrayString(saveReportPath, pathStoreWinSet,
								repeatSpinTest, 1
							));

						}
					}, GUILayout.Width(300));


					EditorVentical(() =>
					{
						slot.gameId = EditorGUILayout.IntField("GameId :", slot.gameId);
						slot.typeOfSql = (SqlType)EditorGUILayout.EnumPopup(slot.typeOfSql);
						if (GUILayout.Button("Generate Sql Win"))
						{


							EditorCoroutines.Execute(GenerateSqlStoreSet(saveReportPath + "sql.sql", pathStoreWinSet, slot.typeOfSql));

						}
					}, GUILayout.Width(300));
					
					
					
					//EditorGUILayout.LabelField(displayCalDummyDone);
				}, GUILayout.ExpandHeight(true));
			}, GUILayout.ExpandHeight(true));


		}
		
		List<int> GetResultRow(int[] pattern, int totalCol)
		{
			List<int> temp = new List<int>();
			int rand = UnityEngine.Random.Range(0, pattern.Length);
			
			for(int i = 0; i< totalCol; i++)
			{
				
				temp.Add(pattern[rand]);
				
				if (rand < pattern.Length -1)
				{
					rand++;
				}
				else
				{
					rand = 0;
				}
				

			}
			


			return temp;
		}
		[System.Serializable]
		class LoadPattern
		{
			public List<SavePattern> savedPattern = new List<SavePattern>();
		}

		[System.Serializable]
		class JsonPayline
		{
			public int[] line;
			public int pay;
			public float totalBetMult;
			public int freespin;
			public bool reverseWin;
			public bool onlyWinOneWay;

			public JsonPayline(int[] line, int pay, float totalBetMult, int freespin, bool reverseWin, bool onlyWinOneWay)
			{
				this.line = line;
				this.pay = pay;
				this.totalBetMult = totalBetMult;
				this.freespin = freespin;
				this.reverseWin = reverseWin;
				this.onlyWinOneWay = onlyWinOneWay;
			}
		}
		[System.Serializable]
		class ListOfPayline
		{
			public List<JsonPayline> listOfPayline;
			public ListOfPayline()
			{
				listOfPayline = new List<JsonPayline>();
			}
		}
		[System.Serializable]
		public class JsonScatterPay
		{
			public int scattersCount;
			public int pay;
			public int freeSpins;
			public int payMult = 1;
			public int featureMultiply = 0;
			public float totalBetMult = 0;
			

			
			public bool enableFeatureMode = false;
			public int featurePay;
			public int featureFreeSpins;
			public int featurePayMult = 1;
			public int bonusTrigger = 0;

			public JsonScatterPay(ScatterPay paytable)
			{
				this.scattersCount = paytable.scattersCount;
				this.pay = paytable.pay;
				this.freeSpins = paytable.freeSpins;
				this.payMult = paytable.payMult;
				this.totalBetMult = paytable.totalBetMult;
				this.enableFeatureMode = paytable.enableFeatureMode;
				this.featureMultiply = paytable.featureMultiply;
				this.featurePay = paytable.featurePay;
				this.featureFreeSpins = paytable.featureFreeSpins;
				this.featurePayMult = paytable.featurePayMult;
				this.bonusTrigger = paytable.hasBonus ? 1:0;
			}
		}
		[System.Serializable]
		public class JsonSpecialIconPay
		{
			public int SpecialIconsCount;
			public int pay;
			public int freeSpins;
			public int payMult = 1;
			public int featureMultiply = 0;
			public float totalBetMult = 0;



			public bool enableFeatureMode = false;
			public int featurePay;
			public int featureFreeSpins;
			public int featurePayMult = 1;
			public int bonusTrigger = 0;

			public JsonSpecialIconPay(SpecialIconPay paytable)
			{
				this.SpecialIconsCount = paytable.specialIconsCount;
				this.pay = paytable.pay;
				this.freeSpins = paytable.freeSpins;
				this.payMult = paytable.payMult;
				this.totalBetMult = paytable.totalBetMult;
				this.enableFeatureMode = paytable.enableFeatureMode;
				this.featurePay = paytable.featurePay;
				this.featureMultiply = paytable.featureMultiply;
				this.featureFreeSpins = paytable.featureFreeSpins;
				this.featurePayMult = paytable.featurePayMult;
				this.bonusTrigger = paytable.hasBonus? 1 : 0;
			}
		}
		[System.Serializable]
		class ListOfScatter
		{
			public List<JsonScatterPay> listOfScatter;
			public ListOfScatter()
			{
				listOfScatter = new List<JsonScatterPay>();
			}
		}
		[System.Serializable]
		class ListOfSpecialIcon 
		{
			public List<JsonSpecialIconPay> listOfSpecialIcon;
			public ListOfSpecialIcon()
			{
				listOfSpecialIcon = new List<JsonSpecialIconPay>();
			}
		}
		[System.Serializable]
		public class JsonWildPay
		{
			public int wildCount;
			public int pay;
			public int freeSpins;
			public int payMult = 1;
			public int featureMultiply = 0;
			public float totalBetMult = 0;



			public bool enableFeatureMode = false;
			public int featurePay;
			public int featureFreeSpins;
			public int featurePayMult = 1;

			public int bonusTrigger = 0;
			public JsonWildPay(WildPay paytable)
			{
				this.wildCount = paytable.wildCount;
				this.pay = paytable.pay;
				this.freeSpins = paytable.freeSpins;
				this.payMult = paytable.payMult;
				this.totalBetMult = paytable.totalBetMult;
				this.enableFeatureMode = paytable.enableFeatureMode;
				this.featurePay = paytable.featurePay;
				this.featureFreeSpins = paytable.featureFreeSpins;
				this.featurePayMult = paytable.featurePayMult;
				this.bonusTrigger = paytable.hasBonus ? 1:0;
			}
		}
		[System.Serializable]
		class JsonBonusPaytable
		{
			public int freespin;
			public int totalBetMultiply;
			public int featureWinMultiply;
			public bool triggerEndGame;
			public bool addFeatureMultiply;

			public JsonBonusPaytable(int freespin, int totalBetMultiply, int featureWinMultiply, bool triggerEndGame, bool addFeatureMultiply)
			{
				this.freespin = freespin;
				this.totalBetMultiply = totalBetMultiply;
				this.featureWinMultiply = featureWinMultiply;
				this.triggerEndGame = triggerEndGame;
				this.addFeatureMultiply = addFeatureMultiply;
			}
		}

		[Serializable]
		class ListOfBonusPay
		{
			public bool disableBonusOnFeature = false;
			public int numberOfBonusPick = 4;
			public int numberOfMinBonusPick = 4;
			public int initialMultiplyWin = 1;

			public int initialFreeSpinWin = 0;

			public int initialMultiplyWinByBet = 0;
			public List<JsonBonusPaytable> bonusList = new List<JsonBonusPaytable>();
		}
		[System.Serializable]
		class ListOfWild
		{
			public List<JsonWildPay> listOfWild;
			public ListOfWild()
			{
				listOfWild = new List<JsonWildPay>();
			}
		}

		[System.Serializable]
		class Icon
		{
			public int id;
			public bool effectWholeColumn;
			public bool effectingOtherCol = false;
			public List<numberOfSlotIconEffecting> listOfPercentageHit;
			public List<int> colExcluded;

			public Icon(int id, bool effectWholeColumn, bool effectingOtherCol, List<numberOfSlotIconEffecting> listOfPercentageHit, List<int> colExcluded)
			{
				this.id = id;
				this.effectWholeColumn = effectWholeColumn;
				this.effectingOtherCol = effectingOtherCol;
				this.listOfPercentageHit = listOfPercentageHit;
				this.colExcluded = colExcluded;
			}
		}
		[System.Serializable]
		class ListIcon
		{
			public List<Icon> listOfIcon;
			public ListIcon()
			{
				listOfIcon = new List<Icon>();
			}

		}

		IEnumerator GenerateSqlStoreSet(string path, string pathStoreWinSet, SqlType typeOfResult)
		{
			StoreWinSet sets = JsonUtility.FromJson<StoreWinSet>(System.IO.File.ReadAllText(pathStoreWinSet));
			string sql = "";
			int counter = 0;
			foreach (WinSet set in sets.listOfWinSet)
			{
				if(set.winRate < winRateAbove)
				{
					continue;
				}
				if(typeOfResult == SqlType.FeatureOnly && !set.isFeature)
				{
					continue;
				}else if(typeOfResult == SqlType.NonFeatureOnly && set.isFeature)
				{
					continue;
				}
				string isFeature = set.isFeature ? "1" : "0";
				sql += "INSERT INTO `slot_king`.`slot_game_store_pattern` " +
							"(`slot_game_store_pattern_game_id`, " +
								"`slot_game_store_pattern_rate`, " +
								"`slot_game_store_pattern_icon_id_pattern`, " +
								"`slot_game_store_pattern_win_id_pattern`," +
								"`slot_game_store_pattern_is_feature`) " +
							"VALUES (" +
								"'" + slot.gameId + "', " +
								"'" + set.winRate.ToString() + "', " +
								"'" + set.iconIdPattern + "', " +
								"'" + set.winIdPattern + "'," +
								"'"+ isFeature + "');\n";
				counter++;
				Debug.Log(counter);
				
				yield return null;
			}
			Debug.Log("All Done >> " +counter);
			System.IO.File.WriteAllText(path, sql);
		}

		IEnumerator GeneratePayLineArrayString(string path, string pathStoreWinSet, int spinRepeat, int bet)
		{

			string log = "";
			System.IO.File.WriteAllText(path+"report.json", "");
			string iconJson = "";
			string linePatternSet = "";
			string paytableJson = "";
			string scatterPaytableJson = "";
			string specialIconPaytableJson = "";
			string wildPaytableJson = "";
			string bonusPaytable = "";
			displayCalDummyDone = "Start";
			float featureWinAccu = 0;
			int scatterHit = 0;
			int specialIconHit = 0;
			int wildHit = 0;
			int bonusHit = 0;
			float bonusWinTotal = 0;
			int writeCounter = 0;
			List<int> slotGroupType = new List<int>();


			foreach (SlotGroupSetting sGS in slot.slotGroups)
			{
				slotGroupType.Add(sGS.listOfRayCastTrans.Count);
			}
			int sumTotalBet = 0;
			float sumTotalPay = 0;
			int freespinCounter = 0;
			int totalFreespin = 0;
			float highestWin = 0;
			int highestWinId = 0;
			bool featureMode = false;


			bool startFeature = false;
			float featureAccumulate = 0;
			string featureRateDetails = "";

			int featureMultiply = slot.featureMultiply;
			StoreWinSet set = new StoreWinSet();
			set.name = slot.name;
			if (!string.IsNullOrEmpty(System.IO.File.ReadAllText(pathStoreWinSet)))
			{
				set = JsonUtility.FromJson<StoreWinSet>(System.IO.File.ReadAllText(pathStoreWinSet));
			}
			 
			string jsonNormal = "{\"savedPattern\":" + System.IO.File.ReadAllText(path + "normal.json") + "}";
			string jsonFeature = "{\"savedPattern\":" + System.IO.File.ReadAllText(path + "feature.json") + "}";
			LoadPattern normal = JsonUtility.FromJson<LoadPattern>(jsonNormal);
			LoadPattern featurePattern = JsonUtility.FromJson<LoadPattern>(jsonFeature);
			ListIcon listIc = new ListIcon();
			int iconCounter = 0;
			foreach (SlotIcon sc in slot.slotIcons)
			{
				listIc.listOfIcon.Add(new Icon(iconCounter, sc.effectWholeColumn, sc.effectingOtherCol, sc.listOfPercentageHit, sc.colExcluded));
				iconCounter++;
			}



			iconJson = JsonUtility.ToJson(listIc);

			List<PayLine> payline = GeneratePaytable(false);
			ListOfPayline listPayLine = new ListOfPayline();

			foreach (PayLine p in payline)
			{
				listPayLine.listOfPayline.Add(new JsonPayline(p.line, p.pay, Mathf.Round(p.totalBetMult * 100f) / 100f, p.freeSpins, p.reverseWin, p.onlyWinOneWay));
			}

			paytableJson = JsonUtility.ToJson(listPayLine);

			ListOfBonusPay bonusTable = new ListOfBonusPay();
			foreach (BonusGameChoicesType b in slot.bonusChoices)
			{
				
				bonusTable.bonusList.Add(new JsonBonusPaytable(b.NumberOfFreeSpin, b.NumberOfMultiply, b.WinMultiply,b.TriggerEndGame,b.AddFeatureMultiply));
			}
			bonusTable.initialFreeSpinWin = slot.initialFreeSpinWin;
			bonusTable.initialMultiplyWin = slot.initialMultiplyWin;
			bonusTable.initialMultiplyWinByBet = slot.initialMultiplyWinByBet;
			bonusTable.numberOfBonusPick = slot.numberOfBonusPick;
			bonusTable.numberOfMinBonusPick = slot.numberOfMinBonusPick;
			bonusTable.disableBonusOnFeature = slot.disableBonusOnFeature;
			bonusPaytable = JsonUtility.ToJson(bonusTable);
			ListOfScatter listScatterPaytable = new ListOfScatter();
			if (slot.useScatter)
				foreach (ScatterPay scatter in slot.scatterPayTable)
				{
				
					listScatterPaytable.listOfScatter.Add(new JsonScatterPay(scatter));
				
				}
			scatterPaytableJson = JsonUtility.ToJson(listScatterPaytable);
			//////specialIcon paytable
			ListOfSpecialIcon listSpecialIconPaytable = new ListOfSpecialIcon();
			if(slot.useSpecialIcon)
				foreach (SpecialIconPay specialIcon in slot.specialIconPayTable)
				{
				
					listSpecialIconPaytable.listOfSpecialIcon.Add(new JsonSpecialIconPay(specialIcon));

				}
			specialIconPaytableJson = JsonUtility.ToJson(listSpecialIconPaytable);

			//////wildPaytable
			ListOfWild listWildPaytable = new ListOfWild();
			if (slot.useWild)
				foreach (WildPay w in slot.wildPayTable)
				{
				
					listWildPaytable.listOfWild.Add(new JsonWildPay(w));
				}
			wildPaytableJson = JsonUtility.ToJson(listWildPaytable);

			for (int repeat = 0; repeat < spinRepeat; repeat++)
			{
				writeCounter ++;
				if(writeCounter >= 1000)
				{
					string loadS = System.IO.File.ReadAllText(path + "report.json");
					loadS += log;
					System.IO.File.WriteAllText(path + "report.json", loadS);
					log = "";
				}
				LoadPattern pattern = null;
				/////Generate random pattern
				if (freespinCounter > 0)
				{
					if (!startFeature)
					{
						startFeature = true;
						featureRateDetails +="**************\n Round: "+ (repeat + 1);
						
						featureAccumulate = 0;


					}
					pattern = featurePattern;
					featureMode = true;
					freespinCounter--;
				}
				else
				{
					if (startFeature)
					{
						float featureWinRate = featureAccumulate / (bet * slot.listOfLineSetting.Count);
						featureRateDetails += "\n Win: " + featureAccumulate;
						featureRateDetails += "\n Rate: " + featureWinRate + "\n";
					}
					startFeature = false;
					pattern = normal;
					featureMode = false;
				}
				List<int> spinResult = new List<int>();
				List<int> spinId = new List<int>();
				List<List<int>> generatedResultRow = new List<List<int>>();
				bool hasAffectedOtherCol = false;
				int numberOfAffected = 0;
				int iconIdAffected = -1;
				List<int> idColAffected = new List<int>();
				for(int i = 0; i< pattern.savedPattern.Count; i++)
				{

					List<int> temp = new List<int>(GetResultRow(pattern.savedPattern[i].sysbolId.ToArray(), slotGroupType[i]));
					spinId.AddRange(temp);
					if (featureMode)
					{
						for(int j = 0; j< temp.Count; j++)
						{
							if (slot.slotIcons[temp[j]].effectingOtherCol)
							{
								for(int k=0; k< slot.slotIcons[temp[j]].listOfPercentageHit.Count; k++)
								{
									numberOfSlotIconEffecting nOSE = slot.slotIcons[temp[j]].listOfPercentageHit[k];

									if ( UnityEngine.Random.Range(0,100) <= nOSE.percentageHit)
									{
										idColAffected.Clear();
										hasAffectedOtherCol = true;
										numberOfAffected = nOSE.quantitySplit;
										iconIdAffected = temp[j];
										idColAffected.Add(i);
										idColAffected.AddRange(slot.slotIcons[temp[j]].colExcluded);
									}
								}
							}
						}
					}
					generatedResultRow.Add(temp);
				}
				if (hasAffectedOtherCol)
				{
					
					while(numberOfAffected > 0)
					{
						int colId = UnityEngine.Random.Range(0, generatedResultRow.Count);
						if (!idColAffected.Contains(colId))
						{
							idColAffected.Add(colId);
							generatedResultRow[colId][UnityEngine.Random.Range(0, generatedResultRow[colId].Count)] = iconIdAffected;
							numberOfAffected--;
						}
					}

					
				}

				for(int i = 0; i< generatedResultRow.Count; i++)
				{
					for(int j = 0; j< generatedResultRow[i].Count; j++)
					{
						List<int> temp = new List<int>();
						if (slot.slotIcons[generatedResultRow[i][j]].effectWholeColumn)
						{	

							
							for (int k = 0; k < slotGroupType[i]; k++)
							{
								temp.Add(generatedResultRow[i][j]);
							}
							generatedResultRow[i] = temp;
							break;
						}
						
					}
					spinResult.AddRange(generatedResultRow[i]);
				}
				Debug.Log("spinResult "+spinResult.Count);

				log += "\n\n\n----------(" + (repeat + 1)+")----------- \n";
				if (featureMode)
				{
					log += "\n ***** Is Feature Mode : " + freespinCounter+" *****\n";
				}

				log += "\ngenerated pattern: [";
				//spinResult = new List<int>() { 12, 1, 3, 1, 12, 0, 11, 11, 11, 3, 12, 0, 0, 9, 1};
				
				string spinResultStr = "[";

				for (int i = 0; i < spinResult.Count; i++)
				{
					log += spinResult[i];
					spinResultStr += spinResult[i];
					if (i < spinResult.Count - 1)
					{
						log += ",";
						spinResultStr += ",";
					}
				}
				spinResultStr += "]";
				log += "]\n \n";

				string spinIdStr = "[";
				for (int i = 0; i < spinId.Count; i++)
				{

					spinIdStr += spinId[i];
					if (i < spinId.Count - 1)
					{

						spinIdStr += ",";
					}
				}
				spinIdStr += "]";




				/////Scattter Win
				ScatterPay scatterWin = null;
				
				if (slot.scatterFollowSeq)
				{
					foreach (ScatterPay scatter in slot.scatterPayTable)
					{
						int c = 0;
						int scatterCount = 0;
						bool gotScatter = false;
						bool wildWin = false;
						int idCol = 0;
						for (int j = 0; j < spinResult.Count; j++)
						{
							//log += c + "  spinResult " + spinResult[j] + " \n \n";
							if (!gotScatter)
							{
								if (slot.scatterWildSub)
								{
									if (spinResult[j] == slot.wild_id)
									{
										//log += " got wild >>>> \n \n";
										wildWin = true;
									}
								}
								//log += spinResult[j] + " >>>>> " + slot.scatter_id + " wildWin " + wildWin + " \n \n";
								if (spinResult[j] == slot.scatter_id || wildWin)
								{
									//log += "Got scatter \n \n";
									gotScatter = true;
									scatterCount++;

									if (scatterCount >= scatter.scattersCount)
									{
										ScatterPay temp = new ScatterPay(scatter);

										if (wildWin)
										{
											temp.pay = temp.pay * slot.wild_multiply;
											temp.featurePay = temp.featurePay * slot.wild_multiply;
											temp.totalBetMult = temp.totalBetMult * slot.wild_multiply;
											wildWin = false;
										}
										if (scatterWin == null)
										{
											//log += " assign >>>> " + temp.scattersCount + " \n \n";
											scatterWin = temp;

										}
										else if (scatterWin.pay < temp.pay || scatterWin.totalBetMult < temp.totalBetMult ||
										   scatterWin.freeSpins < temp.freeSpins)
										{
											//log += " high win >>>> " + temp.scattersCount + " \n \n";
											scatterWin = temp;
										}
										//log += ">>>>>>>>>>>>>>>>>>>>>>>>>> \n \n";
										break;
									}
								}
							}

							c++;
							
							if (c >= slotGroupType[idCol])
							{
								c = 0;
								idCol++;
								if (gotScatter)
								{
									//log += ">>>>>>>>>> reset win >>>> \n \n";
									gotScatter = false;
									wildWin = false;
								}
								else
								{
									//log += ">>>>>>>>>> no win >>>> \n \n";
									break;
								}


							}


						}


					}

				}
				else
				{
					int scatterCount = 0;
					
					for (int j = 0; j < spinResult.Count; j++)
					{
						if (spinResult[j] == slot.scatter_id)
						{
							scatterCount++;
						}
						if (spinResult[j] == slot.wild_id)
						{
							if (slot.scatterWildSub)
							{
								scatterCount++;
							}
						}
					}
					foreach (ScatterPay scatter in slot.scatterPayTable)
					{
						ScatterPay temp = new ScatterPay(scatter);
						if (scatter.scattersCount <= scatterCount)
						{
							if (scatterWin == null)
							{
								//log += " assign >>>> " + temp.scattersCount + " \n \n";
								scatterWin = temp;

							}
							else if (scatterWin.pay < temp.pay || scatterWin.totalBetMult < temp.totalBetMult ||
							   scatterWin.freeSpins < temp.freeSpins)
							{
								//log += " high win >>>> " + temp.scattersCount + " \n \n";
								scatterWin = temp;
							}
						}
						
					}
				}

				/////SpecialIcon Win
				SpecialIconPay specialIconWin = null;

				if (slot.specialIconFollowSeq)
				{
					foreach (SpecialIconPay specialIcon in slot.specialIconPayTable)
					{
						int c = 0;
						int idCol = 0;
						int specialIconCount = 0;
						bool gotSpecialIcon = false;
						bool wildWin = false;

						for (int j = 0; j < spinResult.Count; j++)
						{
							
							if (!gotSpecialIcon)
							{
								if (slot.specialIconWildSub)
								{
									if (spinResult[j] == slot.wild_id)
									{
										
										wildWin = true;
									}
								}
								
								if (spinResult[j] == slot.specialIcon_id || wildWin)
								{

									gotSpecialIcon = true;
									specialIconCount++;

									if (specialIconCount >= specialIcon.specialIconsCount)
									{
										SpecialIconPay temp = new SpecialIconPay(specialIcon);

										if (wildWin)
										{
											temp.pay = temp.pay * slot.wild_multiply;
											temp.featurePay = temp.featurePay * slot.wild_multiply;
											temp.totalBetMult = temp.totalBetMult * slot.wild_multiply;
											wildWin = false;
										}
										if (specialIconWin == null)
										{
											
											specialIconWin = temp;

										}
										else if (specialIconWin.pay < temp.pay || specialIconWin.totalBetMult < temp.totalBetMult ||
										   specialIconWin.freeSpins < temp.freeSpins)
										{
											
											specialIconWin = temp;
										}
										
										break;
									}
								}
							}

							c++;
							if (c >= slotGroupType[idCol])
							{
								c = 0;
								idCol++;
								if (gotSpecialIcon)
								{
									//log += ">>>>>>>>>> reset win >>>> \n \n";
									gotSpecialIcon = false;
									wildWin = false;
								}
								else
								{
									//log += ">>>>>>>>>> no win >>>> \n \n";
									break;
								}


							}


						}


					}

				}
				else
				{
					int specialIconCount = 0;

					for (int j = 0; j < spinResult.Count; j++)
					{
						if (spinResult[j] == slot.specialIcon_id)
						{
							specialIconCount++;
						}
						if (spinResult[j] == slot.wild_id)
						{
							if (slot.specialIconWildSub)
							{
								specialIconCount++;
							}
						}
					}
					foreach (SpecialIconPay specialIcon in slot.specialIconPayTable)
					{
						SpecialIconPay temp = new SpecialIconPay(specialIcon);
						if (specialIcon.specialIconsCount <= specialIconCount)
						{
							if (specialIconWin == null)
							{
								
								specialIconWin = temp;

							}
							else if (specialIconWin.pay < temp.pay || specialIconWin.totalBetMult < temp.totalBetMult ||
							   specialIconWin.freeSpins < temp.freeSpins)
							{
								
								specialIconWin = temp;
							}
						}

					}
				}

				///Wild Win
				WildPay wildWinPay = null;
				int wildCount = 0;
				
				for (int j = 0; j < spinResult.Count; j++)
				{
					
					if (spinResult[j] == slot.wild_id)
					{
						wildCount++;
					}
				}
				//Debug.Log("wildCount "+wildCount);
				foreach (WildPay wild in slot.wildPayTable)
				{
					WildPay tempWildWin = wild;
					
					if (wild.wildCount <= wildCount)
					{
						
						if (wildWinPay == null)
						{

							wildWinPay = tempWildWin;

						}
						else if (wildWinPay.pay < tempWildWin.pay || wildWinPay.totalBetMult < tempWildWin.totalBetMult ||
						   wildWinPay.freeSpins < tempWildWin.freeSpins)
						{
							
							wildWinPay = tempWildWin;
						}
					}

				}

				/////Assign patter to line
				List<List<int>> lineSet = new List<List<int>>();
				linePatternSet = "{\"linePatternSet\":[";
				for (int i = 0; i < slot.listOfLineSetting.Count; i++)
				{
					List<int> ids = new List<int>();

					//log += "---------("+(i+1)+")--------- \n";
					//Debug.Log("---------(" + (i + 1) + ")--------- ");
					linePatternSet += "{\"pattern\":[";
					int counter = 0;
					
					for (int k = 0; k < slot.listOfLineSetting[i].slotGroupBehavior.Count; k++)
					{


						SlotRayCastId sRCId = slot.listOfLineSetting[i].slotGroupBehavior[k];
						
						for (int l = 0; l < sRCId.ids.Count; l++)
						{

							if (sRCId.ids[l] >= 0)
							{
								//log += spinResult[counter] +" | ";
								linePatternSet += "1";
								
								ids.Add(spinResult[counter]);

								
							}
							else
							{
								linePatternSet += "0";
							}
							if(k != slot.listOfLineSetting[i].slotGroupBehavior.Count - 1 || l != sRCId.ids.Count -1)
							{
								linePatternSet += ",";
							}
							else
							{
								linePatternSet += "]";
							}
							

							counter++;

						}

					}
					if (i !=  slot.listOfLineSetting.Count -1)
					{
						linePatternSet += "},";
					}
					else
					{
						linePatternSet += "}]}";
					}
					//log += "\n ";
					lineSet.Add(ids);


				}
				

				/////check win
				
				

				List<PayLine> winPayline = new List<PayLine>();
				List<int> lineWinId = new List<int>();
				for (int i = 0; i < lineSet.Count; i++)
				{
					//log += "+++++++++++++++ LINE (" + (i + 1) + ") +++++++++++++++\n\n";
					PayLine win = null;
					PayLine winTemp = null;
					PayLine winBackward = null;

					for (int j = 0; j < payline.Count; j++)
					{
						//log += "Line ("+j+") : ";
						bool isLose = false;
						for (int k = 0; k < payline[j].line.Length; k++)
						{
							//log += payline[j].line[k] + " > "+ lineSet[i][k] + " | ";
							if (payline[j].line[k] >= 0)
							{
								
								if (payline[j].line[k] != lineSet[i][k])
								{
									isLose = true;
									//log += "\n (lose) \n";
									break;

								}


							}


						}

						//log += "\n";


						if (!isLose)
						{
							//log += "\n (Win) \n";
							winTemp = payline[j];



							if (winTemp != null)
							{
								if (winTemp.reverseWin)
								{
									if (winBackward == null)
									{
										winBackward = winTemp;

									}
									else
									{
										if (winBackward.pay < winTemp.pay || winBackward.freeSpins < winTemp.freeSpins || winBackward.totalBetMult < winTemp.totalBetMult)
										{

											winBackward = winTemp;

										}

									}
								}
								else
								{
									if (win == null)
									{
										win = winTemp;
										//log += "\n Assign Win \n";
									}
									else
									{
										if (win.pay < winTemp.pay || win.freeSpins < winTemp.freeSpins || win.totalBetMult < winTemp.totalBetMult)
										{

											win = winTemp;
											//log += "\n New Win Higher \n";
										}

									}
								}


							}



						}



					}

					if (win != null || winBackward != null)
					{
						lineWinId.Add(i + 1);
					}

					if (win != null && winBackward == null)
					{
						//log += "line : " + (i + 1) + "\n";
						//log += "win On : ";
						//foreach (int id in win.line)
						//{
						//log += id + " | ";
						//}
						//log += "\n";
						winPayline.Add(win);
					}
					else if (win == null && winBackward != null)
					{

						winPayline.Add(winBackward);



					}
					else if (win != null && winBackward != null)
					{

						if (win.onlyWinOneWay || winBackward.onlyWinOneWay)
						{

							if (win.pay > winBackward.pay)
							{
								winPayline.Add(win);
							}
							else
							{
								winPayline.Add(winBackward);
							}
						}
						else
						{
							winPayline.Add(win);
							winPayline.Add(winBackward);
							//log += "win On (reverse) : ";
							//foreach (int id in winBackward.line)
							//{
							//log += id + " | ";
							//}
							//log += "\n";

						}


					}

					//log += "++++++++++++++++++++++++++++++\n";


				}

				float totalPay = 0;
				int freespin = 0;
				if (scatterWin != null)
				{
					
					if (featureMode && scatterWin.enableFeatureMode)
					{
						
						log += "\n scatterWin.pay : " + scatterWin.featurePay;
						
						totalPay += scatterWin.featurePay * bet;
						
						log += "\n scatter total pay: " + ((scatterWin.featurePay * bet));
						freespin += scatterWin.featureFreeSpins;
					}
					else
					{
						
						log += "\n scatterWin.pay : " + scatterWin.pay;
						log += "\n scatterWin.totalBetMult : " + scatterWin.totalBetMult;
                        
						
						totalPay += scatterWin.pay * bet;
						totalPay += scatterWin.totalBetMult * bet * slot.listOfLineSetting.Count;
						log += "\n scatter total pay: " + ((scatterWin.pay * bet) + (scatterWin.totalBetMult * bet * slot.listOfLineSetting.Count));
						freespin += scatterWin.freeSpins;
					}
					
					int totalFeatureWinMultiply = 0;
					
					if (scatterWin.hasBonus)
					{
						log += "\n Enter Bonus >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>";
						BonusGenerateWin(ref log,ref totalFeatureWinMultiply,ref totalPay,ref freespin,ref bonusHit,bet,featureMode);
					}
					
					featureMultiply = totalFeatureWinMultiply;
					if (!featureMode)
					{
						log += "\n scatterWin.FeatureMultiply : " + scatterWin.featureMultiply;
						featureMultiply += scatterWin.featureMultiply;
					}
					scatterHit++;
				}

				if (specialIconWin != null)
				{
					log += "\n specialIcon win : " + specialIconWin.specialIconsCount;
					if (featureMode && specialIconWin.enableFeatureMode)
					{

						log += "\n specialIconWin.pay : " + specialIconWin.featurePay;

						totalPay += specialIconWin.featurePay * bet;

						log += "\n specialIcon total pay: " + ((specialIconWin.featurePay * bet));
						freespin += specialIconWin.featureFreeSpins;
					}
					else
					{

						log += "\n specialIconWin.pay : " + specialIconWin.pay;
						log += "\n specialIconWin.totalBetMult : " + specialIconWin.totalBetMult;
						
						totalPay += specialIconWin.pay * bet;
						totalPay += specialIconWin.totalBetMult * bet * slot.listOfLineSetting.Count;
						log += "\n specialIcon total pay: " + ((specialIconWin.pay * bet) + (specialIconWin.totalBetMult * bet * slot.listOfLineSetting.Count));
						freespin += specialIconWin.freeSpins;
					}

					int totalFeatureWinMultiply = 0;
					
					if (specialIconWin.hasBonus)
					{
						BonusGenerateWin(ref log, ref totalFeatureWinMultiply, ref totalPay, ref freespin, ref bonusHit, bet, featureMode);

					}

					featureMultiply = totalFeatureWinMultiply;
					if (!featureMode)
					{
						log += "\n specialIconWin.FeatureMultiply : " + specialIconWin.featureMultiply;
						featureMultiply += specialIconWin.featureMultiply;
					}
					specialIconHit++;
				}

				if (wildWinPay != null)
				{
					log += "\n wild win : " + wildWinPay.wildCount;
					if(featureMode && wildWinPay.enableFeatureMode)
					{
						log += "\n wild.pay : " + wildWinPay.featurePay;
						totalPay += wildWinPay.featurePay * bet;
						log += "\n wild total pay: " + ((wildWinPay.featurePay * bet) );
						freespin += wildWinPay.featureFreeSpins;
					}
					else
					{
						log += "\n wild.pay : " + wildWinPay.pay;
						log += "\n wild.totalBetMult : " + wildWinPay.totalBetMult;
						totalPay += wildWinPay.pay * bet;
						totalPay += wildWinPay.totalBetMult * bet * slot.listOfLineSetting.Count;
						log += "\n wild total pay: " + ((wildWinPay.pay * bet) + (wildWinPay.totalBetMult * bet * slot.listOfLineSetting.Count));
						freespin += wildWinPay.freeSpins;
					}
					
					int totalFeatureWinMultiply = 0;
					if (wildWinPay.hasBonus)
					{
						
						BonusGenerateWin(ref log, ref totalFeatureWinMultiply, ref totalPay, ref freespin, ref bonusHit, bet, featureMode);

					}
					featureMultiply = totalFeatureWinMultiply;
					if (!featureMode)
					{
						log += "\n wildWinPay.FeatureMultiply : " + wildWinPay.featureMultiply;
						featureMultiply += wildWinPay.featureMultiply;
					}
					wildHit++;
				}
				bonusWinTotal += totalPay;

				log += "\n Line win : ";
				foreach (int id in lineWinId)
				{
					log += id + " | ";
				}

				foreach (PayLine p in winPayline)
				{
					totalPay += p.pay * bet;
					totalPay += p.totalBetMult * bet * slot.listOfLineSetting.Count;
					freespin += p.freeSpins;

				}
				freespinCounter += freespin;
				if(totalPay > highestWin)
				{
					highestWin = totalPay;
					highestWinId = (repeat + 1);
				}

				totalFreespin += freespin;
				if (!featureMode)
				{
					sumTotalBet += bet * slot.listOfLineSetting.Count;
					if (freespin == 0 && (totalPay / (bet * slot.listOfLineSetting.Count)) > winRateAbove)
					{
						float rate = Mathf.Round(((float)totalPay / (float)(bet * slot.listOfLineSetting.Count)) * 100f) / 100f;
						set.CheckAndAdd(new WinSet(spinIdStr, spinResultStr, rate, false));
						//sqlNormal += "INSERT INTO `slot_king`.`slot_game_store_pattern` " +
						//	"(`slot_game_store_pattern_game_id`, " +
						//		"`slot_game_store_pattern_rate`, " +
						//		"`slot_game_store_pattern_icon_id_pattern`, " +
						//		"`slot_game_store_pattern_win_id_pattern`," +
						//		"`slot_game_store_pattern_is_feature`) " +
						//	"VALUES (" +
						//		"'" + slot.gameId + "', " +
						//		"'" + rate + "', " +
						//		"'" + spinIdStr + "', " +
						//		"'" + spinResultStr + "'," +
						//		"'0');\n";


					}
				}
				else
				{

					if (featureMultiply > slot.featureMultiply) totalPay = totalPay * featureMultiply;
					else totalPay = totalPay * slot.featureMultiply;
					featureWinAccu += totalPay;
					if (freespin == 0 && (totalPay / (bet * slot.listOfLineSetting.Count)) > winRateAbove)
					{
						float rate = Mathf.Round(((float)totalPay / (float)(bet * slot.listOfLineSetting.Count)) * 100f) / 100f;
						set.CheckAndAdd(new WinSet(spinIdStr, spinResultStr, rate, true));
						//sqlFeature += "INSERT INTO `slot_king`.`slot_game_store_pattern` " +
						//	"(`slot_game_store_pattern_game_id`, " +
						//		"`slot_game_store_pattern_rate`, " +
						//		"`slot_game_store_pattern_icon_id_pattern`, " +
						//		"`slot_game_store_pattern_win_id_pattern`," +
						//		"`slot_game_store_pattern_is_feature`) " +
						//	"VALUES (" +
						//		"'" + slot.gameId + "', " +
						//		"'" + rate + "', " +
						//		"'" + spinIdStr + "', " +
						//		"'" + spinResultStr + "'," +
						//		"'1');\n";


					}
				}
				log += "\n totalPay: " + totalPay;
				log += "\n freespin: " + freespin;
				if (startFeature)
				{
					
					
					featureAccumulate += totalPay;
					
				}
				

				sumTotalPay += totalPay;
				//displayCalDummyDone = repeat + " >> Done";
				Debug.Log(repeat + " >> Done");
				yield return null;
			}
			float totalWin = Mathf.Ceil((float)((float)sumTotalPay / (float)sumTotalBet) * 100);
			log = System.IO.File.ReadAllText(path + "report.json") + log;
			string saveString = "[";
			for (int i = 0; i < slotGroupType.Count; i++)
			{
				if (i != 0)
				{
					saveString += ",";

				}
				saveString += slotGroupType[i];
			}
			saveString += "]";
			string generalSetting = "" +
				"{" +
					"\"wild_id\":" + slot.wild_id +","+
					"\"wild_multiply\":" + slot.wild_multiply + "," +
					"\"scatter_id\":" + slot.scatter_id + "," +
					"\"scatterWildSub\":\"" + slot.scatterWildSub + "\"," +
					"\"scatterFollowSeq\":\"" + slot.scatterFollowSeq + "\"," +
					"\"specialIcon_id\":" + slot.specialIcon_id + "," +
					"\"specialIconWildSub\":\"" + slot.specialIconWildSub + "\"," +
					"\"specialIconFollowSeq\":\"" + slot.specialIconFollowSeq + "\"," +
					"\"featureMultiply\":" + slot.featureMultiply + "," +
					"\"slot\":" + saveString + "" +
				"}";

			string s = "************************************** \n" +
				"Total Spin: " + spinRepeat + "\n" +
				"-----(Scatter)-------\n"+
				"Scatter Hit : " + scatterHit + "\n" +
				"Scatter Hit rate : " + ((float)((float)scatterHit / (float)spinRepeat) *100)+"%\n"+
				"---------------------\n" +
				"-----(SpecialIcon)-------\n" +
				"SpecialIcon Hit : " + specialIconHit + "\n" +
				"SpecialIcon Hit rate : " + ((float)((float)specialIconHit / (float)spinRepeat) * 100) + "%\n" +
				"---------------------\n" +
				"-----(Wild)-------\n" +
				"Wild Hit : " + wildHit + "\n" +
				"Wild Hit rate : " + ((float)((float)wildHit / (float)spinRepeat) * 100) + "%\n" +
				"---------------------\n" +
				"-----(Bonus)-------\n" +
				"Bonus Hit : " + bonusHit + "\n" +
				"Bonus Hit rate : " + ((float)((float)bonusHit / (float)spinRepeat) * 100) + "%\n" +
				"Bonus Win: "+ bonusWinTotal+"\n"+
				"---------------------\n" +
				"-----(Feature)-------\n" +
				"Total freespin won: " + totalFreespin + " \n" +
				"Total Feature won: " + featureWinAccu + "\n" +
				"win/Spin: "+(totalFreespin != 0? (featureWinAccu / totalFreespin): 0)+"\n"+
				">>>>>>>>>>>>>>>>>>>>>>>\n"+
				featureRateDetails+
				">>>>>>>>>>>>>>>>>>>>>>>\n" +
				"---------------------\n" +
				"Total Bet: " + sumTotalBet + "\n" +
				"Total Pay: " + sumTotalPay + "\n" +
				"Total Pay (exculded Feature): "+ (sumTotalPay - featureWinAccu)+"\n"+
				"RTP : " + totalWin + "% \n" +
				"Highest Win on (" + highestWinId + ") : " + highestWin + " \n"+
				"\n\n**************************************\n\n" +
				"General Setting >> \n" + generalSetting + "\n\n" +
				"Icon List >> \n" + iconJson + "\n\n" +
				"line >> \n" +linePatternSet+ "\n\n" +
				"paytable List >> \n" + paytableJson + "\n\n" +
				"scatter paytable List >> \n" + scatterPaytableJson + "\n\n" +
				"specialIcon paytable List >> \n" + specialIconPaytableJson + "\n\n" +
				"wild paytable List >> \n" + wildPaytableJson + "\n\n" +
				"Bonus paytable >> \n" + bonusPaytable + "\n\n" +
				
				log + "\n\n";

			



			s += ">>> Normal pattern >> \n" + jsonNormal;
			s += "\n\n>>> Feature pattern >> \n" + jsonFeature;
			System.IO.File.WriteAllText(pathStoreWinSet, JsonUtility.ToJson(set));
			//System.IO.File.WriteAllText(path + "sqlNormal.sql", sqlNormal);
			//System.IO.File.WriteAllText(path + "sqlFeature.sql", sqlFeature);
			System.IO.File.WriteAllText(path + "report.json", s);
			Debug.Log("All Done");
		}

		void BonusGenerateWin(
			ref string log,
			ref int totalFeatureWinMultiply,
			ref float totalPay,
			ref int freespin,
			ref int bonusHit,
			int bet,
			bool featureMode )
        {
			log += "\n Enter bonus";
			if (!slot.disableBonusOnFeature || !featureMode)
			{
				totalFeatureWinMultiply = slot.initialMultiplyWin;
				totalPay += slot.initialMultiplyWinByBet * bet * slot.listOfLineSetting.Count;
				freespin += slot.initialFreeSpinWin;
				bonusHit++;
				List<int> pickedBonusId = new List<int>();
				List<int> endGameList = new List<int>();
				for (int z = 0; z < slot.bonusChoices.Count; z++)
				{
                    if (slot.bonusChoices[z].TriggerEndGame)
                    {
						endGameList.Add(z);

					}
                    else
                    {
						pickedBonusId.Add(z);
					}
					
				}
				log += "\n\n >> bonusWin >>";
				endGameList = Shuffle(endGameList);
				pickedBonusId = Shuffle(pickedBonusId);
				
				for (int i = 0; i < endGameList.Count; i++)
				{
					pickedBonusId.Insert(UnityEngine.Random.Range(slot.numberOfMinBonusPick, pickedBonusId.Count), endGameList[i]);
				}
				bool hasEnd = false;
				for (int z = 0; z < slot.numberOfBonusPick; z++)
				{

					BonusGameChoicesType winBonus = slot.bonusChoices[pickedBonusId[z]];
                    
					if (hasEnd) break;
					log += "\n bonusWin id:" + pickedBonusId[z];

					log += "\n Total bet Multiply: " + winBonus.NumberOfMultiply;
					log += "\n Number Of FreeSpin: " + winBonus.NumberOfFreeSpin;
					log += "\n feature multiply: " + winBonus.WinMultiply;
					if (winBonus.TriggerEndGame)
					{
						hasEnd = true;

					}
					if (winBonus.WinMultiply > 0)
					{
						totalFeatureWinMultiply += winBonus.WinMultiply;
					}
					totalPay += winBonus.NumberOfMultiply * bet * slot.listOfLineSetting.Count;
					freespin += winBonus.NumberOfFreeSpin;
				}
			}
		}
		List<int> Shuffle(List<int> listToShuffle)
		{

			for (int i = 0; i < listToShuffle.Count; i++)
			{
				int temp = listToShuffle[i];
				int randomIndex = UnityEngine.Random.Range(i, listToShuffle.Count);
				listToShuffle[i] = listToShuffle[randomIndex];
				listToShuffle[randomIndex] = temp;
			}

			return listToShuffle;
		}
		void CalculateSlotIconPercent(ref slotIconGroup col, List<ProbabilitySlotIcon> listOfPro, int numberOfRayCast)
		{
			float totalColPercent = 0;
			int counter = 0;
			foreach (ProbabilitySlotIcon p in listOfPro)
			{
				totalColPercent += p.percentage;
				
				if (col.groupIcon.Count <= counter)
				{
					col.groupIcon.Add(new slotIconPercent());
				}
				col.groupIcon[counter].setPercent = p.percentage;
				col.groupIcon[counter].slotIconId = p.slotIconId;
				counter++;
			}

			for (int i = 0; i < col.groupIcon.Count; i++)
			{
				float effectWholeColumnChanges = 1f;

				if (slot.slotIcons[col.groupIcon[i].slotIconId].effectWholeColumn)
				{
					effectWholeColumnChanges = numberOfRayCast;
				}
				col.groupIcon[i].percent = col.groupIcon[i].setPercent * effectWholeColumnChanges / totalColPercent;

			}
		}

		void RtpCol(List<ProbabilitySlotIcon> col)
		{
			int id = 0;
			EditorHorizontal("HelpBox", () =>
			{
				foreach (SlotIcon sI in slot.slotIcons)
				{
					EditorHorizontal( () =>
					{
						DrawIcon(id, 50);

						EditorLableCenterWithBox("%", () =>
						{
							if (col.Count <= id)
							{
								ProbabilitySlotIcon p = new ProbabilitySlotIcon();
								p.percentage = 0f;
								p.slotIconId = id;
								col.Add(p);

							}

							col[id].percentage = EditorGUILayout.Slider(col[id].percentage, 0f, 1f);


						});


					});

					id++;
				}

			}, GUILayout.Width(100));
		}

		List<PayLine> GeneratePaytable(bool withScatter = true)
		{
			List<PayLine> payTableFull = new List<PayLine>();
			List<PayLine> tempPayline = new List<PayLine>( slot.payLines);
			List<PayLine> temScatterFollowSeq = new List<PayLine>();
			for (int i = 0; i < slot.scatterPayTable.Count; i++)
			{
				PayLine p = new PayLine();
				List<int> id = new List<int>();
				p.pay = slot.scatterPayTable[i].pay;
				p.payMult = slot.scatterPayTable[i].payMult;
				p.totalBetMult = slot.scatterPayTable[i].totalBetMult;
			
				p.wildMulti = slot.scatterWildSub;
				p.freeSpins = slot.scatterPayTable[i].freeSpins;
				for (int j = 0; j < 5; j++)
				{
					if (j < slot.scatterPayTable[i].scattersCount)
					{
						id.Add(slot.scatter_id);
					}
					else
					{
						id.Add(-1);
					}

				}
				p.line = id.ToArray();
				temScatterFollowSeq.Add(p);

			}
			if (slot.scatterWildSub && withScatter)
			{
				tempPayline.AddRange(temScatterFollowSeq);
			}
			

			for (int j = 0; j < tempPayline.Count; j++)
			{

				payTableFull.Add(new PayLine(tempPayline[j]));
				
				if (slot.useWild)
				{
					PayLine p = tempPayline[j];
					List<PayLine> tempWildLine = new List<PayLine>();
					tempWildLine = new List<PayLine>(p.GetWildLines(FindObjectOfType<SlotController>()));
					for (int l = 0; l < tempWildLine.Count; l++)
					{
						if (tempWildLine[l].wildMulti)
						{
							tempWildLine[l].pay = tempWildLine[l].pay * slot.wild_multiply;
							tempWildLine[l].totalBetMult = tempWildLine[l].totalBetMult * slot.wild_multiply;
						}
						tempWildLine[l].reverseWin = tempPayline[j].reverseWin;
					}
					
					payTableFull.AddRange(tempWildLine);
				}
			}
			List<PayLine> tempReverseLine = new List<PayLine>();
			for (int k = 0; k < payTableFull.Count; k++)
			{
				if (payTableFull[k].reverseWin)
				{
					PayLine pay = new PayLine(payTableFull[k]);

					pay.reverseWin = true;
					pay.onlyWinOneWay = false;
					payTableFull[k].reverseWin = false;
					payTableFull[k].onlyWinOneWay = false;
					Array.Reverse(pay.line);

					tempReverseLine.Add(pay);
				}


			}
			payTableFull.AddRange(tempReverseLine);

			if (!slot.scatterWildSub && withScatter)
			{
				payTableFull.AddRange(temScatterFollowSeq);
			}

			return payTableFull;
		}

        #endregion DrawRTP

        #region DrawUiGeneralSetting
        void DrawUiGeneralSetting()
		{
			EditorGUILayout.LabelField("Ui General Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

			EditorVentical("GroupBox", () => 
			{
				uiGeneralToolBarSelected = GUILayout.Toolbar(uiGeneralToolBarSelected, toolbarItem);
				EditorVentical("GroupBox", () =>
				{
					switch (uiGeneralToolBarSelected)
					{
						case 0:
							DrawUiGeneralImage();
							break;
						case 1:
							DrawUiGeneralGameObj();
							break;
						case 2:
							DrawUiGeneralText();
							break;
						case 3:
							DrawUiGeneralButton();
							break;
					}
				});
			});
			


		}
		void DrawUiGeneralImage()
		{
			EditorHorizontal(() => 
			{
				EditorGUILayout.LabelField("Setup Images", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft });
				
			});
			
			DrawLine();
			for (int i = 0; i < slot.uiGeneralImagesSetting.Count; i++)
			{
				ImageSetting imageSetting = slot.uiGeneralImagesSetting[i];
				EditorVentical("HelpBox", () =>
				{
					EditorHorizontal(new Color32(0, 0, 0, 255), () =>
					{
						EditorGUILayout.LabelField(" ", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
						if (GUILayout.Button("Remove", GUILayout.Width(100)))
						{
							slot.uiGeneralImagesSetting.RemoveAt(i);
							return;
						}
					});

					EditorHorizontal(() =>
					{
						if (imageSetting.sprite != null)
						{
							DrawSprite(imageSetting.sprite, 300);
						}

						EditorVentical(() =>
						{
							EditorLableCenterWithBox("Name", () =>
							{
								imageSetting.name = EditorGUILayout.TextField(imageSetting.name);
							});
							EditorLableCenterWithBox("Sprite", () =>
							{
								imageSetting.sprite = (Sprite)EditorGUILayout.ObjectField(imageSetting.sprite, typeof(Sprite), false);
							});
							EditorVentical("HelpBox", () => 
							{
								imageSetting.enableSorting = EditorGUILayout.Toggle("Enable Sorting Layer", imageSetting.enableSorting);
                                if (imageSetting.enableSorting)
                                {
									EditorLableCenterWithBox("Sorting Order", () =>
									{
										imageSetting.sortingOrder = EditorGUILayout.IntField(imageSetting.sortingOrder);
									});
									
								}
							});
							EditorDrawRectSetting("Image Rect Setting",imageSetting, null);
						});
					});
					Rect rec = EditorDrawStateImageContol(imageSetting.listOfSpriteState);
					DragAndDropItem<GameObject>(rec, (obj) =>
					{
						StateControl sC = obj.GetComponent<StateControl>();
						if (sC != null)
						{
							imageSetting.listOfSpriteState = new List<SpriteState>(sC.listOfSpriteState);
						}

					});
					


				});

			}
			if (GUILayout.Button("Add Image"))
			{
				slot.uiGeneralImagesSetting.Add(new ImageSetting());
				return;
			}
			EditorHorizontal(()=> 
			{
				
				EditorVentical("HelpBox", () =>
				{

					EditorVentical(new Color32(120, 120, 120, 255), () =>
					{

						if (GUILayout.Button("APPLY TO SCENE"))
						{
							UpdateUiSettingImageToScene();
						}
					});

				}, GUILayout.Width(100));
				DragAndDropItem<GameObject>("Drag And Drop Scene gameobject", (obj) => 
				{
					Image image = obj.GetComponent<Image>();
					if(image == null)
					{
						return;
					
					}

					ImageSetting imageSetting = new ImageSetting();
					imageSetting.sprite = image.sprite;

					RectTransform rect = obj.GetComponent<RectTransform>();
					if(rect != null)
					{

						imageSetting.position = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y);
						imageSetting.size = rect.sizeDelta;
						imageSetting.scale = rect.localScale;
						
					}

					imageSetting.name = obj.name;
					if (imageSetting.enableSorting)
					{

						Canvas cv = obj.AddComponent<Canvas>();
						cv.overrideSorting = true;
						cv.sortingOrder = imageSetting.sortingOrder;

					}
					StateControl sC = obj.GetComponent<StateControl>();
					if(sC != null)
					{
						imageSetting.listOfSpriteState = new List<SpriteState>(sC.listOfSpriteState);
					}
					slot.uiGeneralImagesSetting.Add(imageSetting);


				});
			});


		}
		void DrawUiGeneralGameObj()
		{

			EditorHorizontal(() =>
			{
				EditorGUILayout.LabelField("Setup GameObjects", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft });
				
			});
			DrawLine();
			for (int i = 0; i < slot.uiGeneralGameobjSetting.Count; i++)
			{
				UiGameobjectSetting uiGameObjSetting = slot.uiGeneralGameobjSetting[i];
				EditorVentical("HelpBox", () =>
				{
					EditorHorizontal(new Color32(0, 0, 0, 255), () =>
					{
						EditorGUILayout.LabelField(" ", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
						if (GUILayout.Button("Remove", GUILayout.Width(100)))
						{
							slot.uiGeneralGameobjSetting.RemoveAt(i);
							return;
						}
					});
					EditorLableCenterWithBox("Prefab", () =>
					{
						uiGameObjSetting.gameObjectPrefab = (GameObject)EditorGUILayout.ObjectField(uiGameObjSetting.gameObjectPrefab, typeof(GameObject), false);
					});
					
					EditorDrawRectSetting("GameObj rect setting",uiGameObjSetting, null);

				});
			}
			if (GUILayout.Button("Add GameObj"))
			{
				slot.uiGeneralGameobjSetting.Add(new UiGameobjectSetting());
				return;
			}
			EditorHorizontal(() =>
			{
				EditorVentical("HelpBox", () =>
				{

					EditorVentical(new Color32(120, 120, 120, 255), () =>
					{

						if (GUILayout.Button("APPLY TO SCENE"))
						{
							UpdateUiSettingUiGameObjToScene();
						}
					});

				}, GUILayout.Width(100));
				
			});

		}


		void DrawUiGeneralText()
		{

			EditorHorizontal(() =>
			{
				EditorGUILayout.LabelField("Setup Texts", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft });
				
			});
			DrawLine();
			for (int i = 0; i < slot.uiGeneralTextSetting.Count; i++)
			{
				UiTextSetting uiTextSetting = slot.uiGeneralTextSetting[i];
				EditorVentical("HelpBox", () => 
				{
					EditorVentical("GroupBox", () => 
					{
						EditorHorizontal(new Color32(0, 0, 0, 255), () =>
						{
							EditorGUILayout.LabelField(" ", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
							if (GUILayout.Button("Remove", GUILayout.Width(100)))
							{
								slot.uiGeneralTextSetting.RemoveAt(i);
								return;
							}
						});

						EditorVentical(() =>
						{
							EditorLableCenterWithBox("Text Name", () =>
							{
								uiTextSetting.name = EditorGUILayout.TextField(uiTextSetting.name);
							});
							EditorLableCenterWithBox("Text Field", () =>
							{
								uiTextSetting.content = EditorGUILayout.TextField(uiTextSetting.content);
							});
							EditorLableCenterWithBox("Type", () =>
							{
								uiTextSetting.uiTextType = (UiTextSettingType)EditorGUILayout.EnumPopup(uiTextSetting.uiTextType);
								
								
							});
							
							
							EditorHorizontal(() =>
							{
								EditorVentical(() => {
									Rect r = EditorVentical("HelpBox", () =>
									{
										EditorVentical("GroupBox", () =>
										{

											EditorGUILayout.LabelField("Text Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

											EditorLableCenterWithBox("Font", () =>
											{
												uiTextSetting.font = (Font)EditorGUILayout.ObjectField(uiTextSetting.font, typeof(Font), false);
											});

											EditorLableCenterWithBox("Font Size", () =>
											{
												uiTextSetting.fontSize = EditorGUILayout.IntField(uiTextSetting.fontSize);
											});

											EditorLableCenterWithBox("Text Anchor", () =>
											{
												uiTextSetting.textAnchor = (TextAnchor)EditorGUILayout.EnumPopup(uiTextSetting.textAnchor);
											});

											EditorLableCenterWithBox("", () =>
											{
												uiTextSetting.bestFit = EditorGUILayout.Toggle("Best Fit", uiTextSetting.bestFit);

												if (uiTextSetting.bestFit)
												{
													DrawLine();
													uiTextSetting.textSizeMin = EditorGUILayout.IntField("Min: ", uiTextSetting.textSizeMin);
													uiTextSetting.textSizeMax = EditorGUILayout.IntField("Max: ", uiTextSetting.textSizeMax);
												}
											});

											EditorLableCenterWithBox("Color", () =>
											{
												uiTextSetting.textColor = EditorGUILayout.ColorField(uiTextSetting.textColor);
											});

										});


									});

									DragAndDropItem<GameObject>(r, (obj) =>
									{
										Text text = obj.GetComponent<Text>();
										if (text != null)
										{
											uiTextSetting.font = text.font;
											uiTextSetting.fontSize = text.fontSize;
											uiTextSetting.textAnchor = text.alignment;
											uiTextSetting.bestFit = text.resizeTextForBestFit;
											uiTextSetting.textSizeMax = text.resizeTextMaxSize;
											uiTextSetting.textSizeMin = text.resizeTextMinSize;
											uiTextSetting.textColor = text.color;
										}
									});

									EditorVentical("HelpBox", () => {
										uiTextSetting.enableSorting = EditorGUILayout.Toggle("Enable enable sorting", uiTextSetting.enableSorting);

                                        if (uiTextSetting.enableSorting)
                                        {
											uiTextSetting.sortingOrder = EditorGUILayout.IntField("Sorting Order", uiTextSetting.sortingOrder);

										}


									});



								});
								
								EditorVentical(() => 
								{
									EditorVentical("HelpBox", () =>
									{
										EditorDrawRectSetting("Rect setting", uiTextSetting, null);
									});

									Rect r2 = EditorVentical("HelpBox", () => 
									{
										EditorVentical("GroupBox", () => 
										{
											uiTextSetting.enableOutline = EditorGUILayout.Toggle("Enable out line", uiTextSetting.enableOutline);
											if (uiTextSetting.enableOutline)
											{
												EditorLableCenterWithBox("Outline Color", () =>
												{
													uiTextSetting.outlineColor = EditorGUILayout.ColorField(uiTextSetting.outlineColor);
												});

												EditorLableCenterWithBox("Outline Distance X", () =>
												{
													uiTextSetting.outlineDistance.x = EditorGUILayout.FloatField(uiTextSetting.outlineDistance.x);
												});
												EditorLableCenterWithBox("Outline Distance Y", () =>
												{
													uiTextSetting.outlineDistance.y = EditorGUILayout.FloatField(uiTextSetting.outlineDistance.y);
												});


											}
										});
										
									});

									DragAndDropItem<GameObject>(r2, (obj) => 
									{
										Outline line = obj.GetComponent<Outline>();
										if (line)
										{
											uiTextSetting.enableOutline = true;
											uiTextSetting.outlineColor = line.effectColor;
											uiTextSetting.outlineDistance = line.effectDistance;

										}
									});
									EditorVentical("HelpBox", () =>
									{
										EditorVentical("GroupBox", () =>
										{
											uiTextSetting.enableGradient = EditorGUILayout.Toggle("Enable gradient", uiTextSetting.enableGradient);
											if (uiTextSetting.enableGradient)
											{
												EditorLableCenterWithBox("Top Color", () =>
												{
													uiTextSetting.topColor = EditorGUILayout.ColorField(uiTextSetting.topColor);
												});

												EditorLableCenterWithBox("Bottom Color", () =>
												{
													uiTextSetting.bottomColor = EditorGUILayout.ColorField(uiTextSetting.bottomColor);
												});


											}
										});
									});


								});
								
							});

						});
					});
					

				});

			}
			if (GUILayout.Button("Add Text"))
			{
				slot.uiGeneralTextSetting.Add(new UiTextSetting());
				return;
			}
			EditorHorizontal(() =>
			{
				EditorVentical("HelpBox", () =>
				{

					EditorVentical(new Color32(120, 120, 120, 255), () =>
					{

						if (GUILayout.Button("APPLY TO SCENE"))
						{
							UpdateUiSettingTextToScene();
						}
					});

				}, GUILayout.Width(100));

			});
			EditorHorizontal(() =>
			{
				EditorVentical("HelpBox", () =>
				{

					DragAndDropItem<GameObject>("Drag Text Gameobject here", (obj) => 
					{

						Text text = obj.GetComponent<Text>();
						if (text == null) return;


						UiTextSetting uiTextSetting = new UiTextSetting();

						uiTextSetting.name = obj.name;

						uiTextSetting.font = text.font;
						uiTextSetting.fontSize = text.fontSize;
						uiTextSetting.textAnchor = text.alignment;
						uiTextSetting.bestFit = text.resizeTextForBestFit;
						uiTextSetting.textSizeMax = text.resizeTextMaxSize;
						uiTextSetting.textSizeMin = text.resizeTextMinSize;
						uiTextSetting.textColor = text.color;
						uiTextSetting.content = text.text;

						RectTransform rect = obj.GetComponent<RectTransform>();

						uiTextSetting.size = rect.sizeDelta;
						uiTextSetting.position = rect.anchoredPosition;
						uiTextSetting.scale = rect.localScale;

						Outline line = obj.GetComponent<Outline>();
						if (line)
						{
							uiTextSetting.enableOutline = true;
							uiTextSetting.outlineColor = line.effectColor;
							uiTextSetting.outlineDistance = line.effectDistance;
						}
						else
						{
							uiTextSetting.enableOutline = false;
						}

						SlotMenuController slotMenu = FindObjectOfType<SlotMenuController>();

						if(slotMenu.BalanceSumText == text) uiTextSetting.uiTextType = UiTextSettingType.BalanceSumText;
						else if (slotMenu.BalanceDiamondSumText == text) uiTextSetting.uiTextType = UiTextSettingType.BalanceDiamondSumText;
						else if(slotMenu.TotalBetSumText == text) uiTextSetting.uiTextType = UiTextSettingType.TotalBetSumText;
						else if (slotMenu.LinesCountText == text) uiTextSetting.uiTextType = UiTextSettingType.LinesCountText;
						else if (slotMenu.LineBetSumText == text) uiTextSetting.uiTextType = UiTextSettingType.LineBetSumText;
						else if (slotMenu.FreeSpinCountText == text) uiTextSetting.uiTextType = UiTextSettingType.FreeSpinCountText;
						else if (slotMenu.WinCoinText == text) uiTextSetting.uiTextType = UiTextSettingType.WinCoinText;
						else if (slotMenu.ErrorInfo == text) uiTextSetting.uiTextType = UiTextSettingType.ErrorInfo;
						else if (slotMenu.MiniJackpotSumText == text) uiTextSetting.uiTextType = UiTextSettingType.MiniJackpotSumText;
						else if (slotMenu.MaxiJackpotSumText == text) uiTextSetting.uiTextType = UiTextSettingType.MaxiJackpotSumText;
						else if (slotMenu.MegaJackpotSumText == text) uiTextSetting.uiTextType = UiTextSettingType.MegaJackpotSumText;
						else if (slotMenu.resultWinText == text) uiTextSetting.uiTextType = UiTextSettingType.resultWinText;
						else if (slotMenu.totalLineWinCount == text) uiTextSetting.uiTextType = UiTextSettingType.totalLineWinCount;
						else if (slotMenu.LineBetSumWithWordsText == text) uiTextSetting.uiTextType = UiTextSettingType.LineBetSumWithWordsText;
						else if (slotMenu.LinesCountWithWordsText == text) uiTextSetting.uiTextType = UiTextSettingType.LinesCountWithWordsText;
						else if (slotMenu.totalFeatureWinInfoText == text) uiTextSetting.uiTextType = UiTextSettingType.totalFeatureWinInfoText;



						slot.uiGeneralTextSetting.Add(uiTextSetting);

					});

				}, GUILayout.Width(300));

			});

		}
		void DrawUiGeneralButton()
		{
			EditorGUILayout.LabelField("Setup Buttons", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
			DrawLine();
			toolbarUiButtonSettingCurrectSelected = GUILayout.Toolbar(toolbarUiButtonSettingCurrectSelected, toolbarUiButtonSetting);
			UiButtonSetting uiButtonSetting = slot.LinePlus;
			string title = "";
			EditorVentical("GroupBox", () =>
			{
				switch (toolbarUiButtonSettingCurrectSelected)
				{
					case 0:
						title = "Line Plus";
						
						uiButtonSetting = slot.LinePlus;

						break;
					case 1:
						title = "Line Minus";
						
						uiButtonSetting = slot.LineMinus;
						break;
					case 2:
						
						title = "Bet Plus";

						uiButtonSetting = slot.BetPlus;
						break;
					case 3:
						
						title = "Bet Minus";

						uiButtonSetting = slot.BetMinus;
						break;
					case 4:
						
						title = "Back";

						uiButtonSetting = slot.Back;
						break;
					case 5:
						
						title = "Menu";

						uiButtonSetting = slot.Menu;
						break;
					case 6:
						
						title = "AutoSpin";

						uiButtonSetting = slot.AutoSpin;
						break;
					case 7:
						
						title = "Spin";

						uiButtonSetting = slot.Spin;
						break;
				}


				EditorGUILayout.LabelField(title, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
				EditorHorizontal(() =>
				{
					EditorVentical("HelpBox", () =>
					{
						EditorGUILayout.LabelField("Normal", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

						uiButtonSetting.sprite = (Sprite)EditorGUILayout.ObjectField(uiButtonSetting.sprite, typeof(Sprite), false);
						if (uiButtonSetting.sprite != null)
						{
							DrawSprite(uiButtonSetting.sprite, 300);
						}


					});
					EditorVentical("HelpBox", () =>
					{
						EditorGUILayout.LabelField("Pressed", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

						uiButtonSetting.pressedSprite = (Sprite)EditorGUILayout.ObjectField(uiButtonSetting.pressedSprite, typeof(Sprite), false);
						if (uiButtonSetting.pressedSprite != null)
						{
							DrawSprite(uiButtonSetting.pressedSprite, 300);
						}


					});
				});
				if(title == "Bet Plus" || title == "Bet Minus")
                {
					EditorVentical("HelpBox", () =>
					{

						uiButtonSetting.enableLongPress = EditorGUILayout.Toggle("Enable Long Press", uiButtonSetting.enableLongPress);
						if (uiButtonSetting.enableLongPress)
						{
							EditorLableCenterWithBox("Long Press Start time", () =>
							{
								uiButtonSetting.longPressStartTime = EditorGUILayout.FloatField(uiButtonSetting.longPressStartTime);
							});

							EditorLableCenterWithBox("Interval time", () =>
							{
								uiButtonSetting.longPressIntervalTime = EditorGUILayout.FloatField(uiButtonSetting.longPressIntervalTime);
							});
						}


					});
				}
				EditorVentical("HelpBox", () =>
				{
					

					uiButtonSetting.enableSorting = EditorGUILayout.Toggle("Enable Sorting", uiButtonSetting.enableSorting);
					if (uiButtonSetting.enableSorting)
					{
						EditorLableCenterWithBox("Sorting Order", () =>
						{
							uiButtonSetting.sortingOrder = EditorGUILayout.IntField(uiButtonSetting.sortingOrder);
						});
					}


				});
				EditorHorizontal(() =>
				{
					EditorDrawRectSetting("Line Plus Rect", uiButtonSetting, null);

				});
				Rect rec = EditorDrawStateImageContol(uiButtonSetting.listOfSpriteState);
				DragAndDropItem<GameObject>(rec, (obj) =>
				{
					StateControl sC = obj.GetComponent<StateControl>();
					if (sC != null)
					{
						uiButtonSetting.listOfSpriteState = new List<SpriteState>(sC.listOfSpriteState);
					}

				});

				DragAndDropItem<GameObject>("Drag and drop Scene GameObject", (obj) =>
				{
					StateControl sC = obj.GetComponent<StateControl>();
					if (sC != null)
					{
						uiButtonSetting.listOfSpriteState = new List<SpriteState>(sC.listOfSpriteState);
					}

					RectTransform rect = obj.GetComponent<RectTransform>();
					if (rect)
					{
						uiButtonSetting.scale = rect.localScale;
						uiButtonSetting.position = rect.anchoredPosition;
						uiButtonSetting.size = rect.sizeDelta;
					}

					Image image = obj.GetComponent<Image>();
					if (image != null)
					{
						uiButtonSetting.sprite = image.sprite;
					}

					Button button = obj.GetComponent<Button>();
					if (button != null)
					{
						uiButtonSetting.pressedSprite = button.spriteState.pressedSprite;
					}

				});

			});
			EditorHorizontal(() =>
			{
				EditorVentical("HelpBox", () =>
				{

					EditorVentical(new Color32(120, 120, 120, 255), () =>
					{

						if (GUILayout.Button("APPLY TO SCENE"))
						{
							UpdateUiButtonSettingToScene();
						}
					});

				}, GUILayout.Width(100));

			});
		}
        #endregion DrawUiGeneralSetting

        #region DrawHelpSetting
        void DrawARCHelpSetting()
        {
			EditorGUILayout.LabelField("Help Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
			if(slot.ARChelpInfoSprites == null || slot.ARChelpInfoSprites.Count == 0)
            {
				SlotController sC = FindObjectOfType<SlotController>();
				if(sC != null)
                {
					slot.ARChelpInfoSprites = new List<Sprite>(sC.helpPagesSprite);
					slot.ARChelpInfoSize = sC.helpPagesSize;
				}
			}
			if(slot.ARChelpInfoSprites == null)
            {
				slot.ARChelpInfoSprites = new List<Sprite>();
				slot.ARChelpInfoSize = Vector2.zero;

			}
			EditorVentical("GroupBox", () =>
			{
				

				EditorVentical("HelpBox", () => {
					EditorLableCenterWithBox("Width", () =>
					{
						slot.ARChelpInfoSize.x = EditorGUILayout.FloatField(slot.ARChelpInfoSize.x);
					});
					EditorLableCenterWithBox("Height", () =>
					{
						slot.ARChelpInfoSize.y = EditorGUILayout.FloatField(slot.ARChelpInfoSize.y);
					});
					
					EditorGUILayout.LabelField("Help Page Sprites", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

					for (int b = 0; b < slot.ARChelpInfoSprites.Count; b++)
					{
						bool isRemove = false;
						EditorHorizontal(() => {
							slot.ARChelpInfoSprites[b] = (Sprite)EditorGUILayout.ObjectField(slot.ARChelpInfoSprites[b], typeof(Sprite), false);
							if (GUILayout.Button("Remove"))
							{
								isRemove = true;
								slot.ARChelpInfoSprites.RemoveAt(b);

							}
						});
						if (isRemove)
						{
							break;
						}
					}
					
					if (GUILayout.Button("ADD"))
					{
						slot.ARChelpInfoSprites.Add(null);
					}
					
				}, GUILayout.Width(400));

			});
			
		}
		void DrawHelpSetting()
		{
			EditorGUILayout.LabelField("Help Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

			EditorVentical("GroupBox", () =>
			{
				EditorVentical("HelpBox", () =>
				{

					for (int i = 0; i < slot.helpGroup.Count; i++)
					{

						EditorVentical("GroupBox", () =>
						{
							HelpPageGroup helpGroup = slot.helpGroup[i];
							EditorHorizontal(() =>
							{
								EditorLableCenterWithBox("Group Name", () =>
								{
									if (i != 0)
									{
										helpGroup.name = EditorGUILayout.TextField(helpGroup.name, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

									}
									else
									{
										EditorGUILayout.LabelField("Help", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
										helpGroup.name = "Help";
									}
								});
								


								if (GUILayout.Button("REMOVE GROUP", GUILayout.ExpandHeight(true)))
								{
									slot.helpGroup.RemoveAt(i);
								}

							}, GUILayout.Height(20));

							EditorVentical("GroupBox", () =>
							{
								for (int j = 0; j < helpGroup.helpPageSettings.Count; j++)
								{

									HelpPageSetting helpPage = helpGroup.helpPageSettings[j];
									EditorHorizontal(() =>
									{
										EditorGUILayout.LabelField("PAGE " + (j + 1), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
										if (GUILayout.Button("REMOVE PAGE"))
										{
											helpGroup.helpPageSettings.RemoveAt(j);
										}
									});

									EditorVentical("HelpBox", () =>
									{
										EditorGUILayout.LabelField("Page Sprite", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
										helpPage.pageImage = (Sprite)EditorGUILayout.ObjectField(helpPage.pageImage, typeof(Sprite), false);
										if (helpPage.pageImage != null)
										{
											DrawSprite(helpPage.pageImage, 300);
											
										}

									}, GUILayout.Width(300));

									EditorVentical("HelpBox", () =>
									{
										EditorGUILayout.LabelField("Button Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
										for (int k = 0; k < helpPage.buttonsSetting.Count; k++)
										{
											ButtonSetting buttonSetting = helpPage.buttonsSetting[k];
											EditorVentical("HelpBox", () =>
											{
												EditorGUILayout.LabelField("Button Image", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
												buttonSetting.sprite = (Sprite)EditorGUILayout.ObjectField(buttonSetting.sprite, typeof(Sprite), false);
												if (buttonSetting.sprite != null)
												{
													DrawSprite(buttonSetting.sprite, 300);
													
												}
												EditorDrawRectSetting("Button Rect Setting",buttonSetting.rect, () => 
												{
													EditorLableCenterWithBox("Button Action", () =>
													{
														buttonSetting.buttonAction = (HelpPageButtonType)EditorGUILayout.EnumPopup(buttonSetting.buttonAction);
													});
													if(buttonSetting.buttonAction == HelpPageButtonType.OpenHelpGroup)
													{
														EditorLableCenterWithBox("Open Group Name", () =>
														{
															EditorHorizontal(() =>
															{

																List<string> names = new List<string>();

																foreach (HelpPageGroup s in slot.helpGroup)
																{
																	names.Add(s.name);
																}
																int id = 0;
																if (names.Contains(buttonSetting.openGroupName))
																{
																	id = names.IndexOf(buttonSetting.openGroupName);
																}

																id = EditorGUILayout.Popup(id, names.ToArray());
																buttonSetting.openGroupName = names[id];

															});
														});
													}
													if(buttonSetting.buttonAction == HelpPageButtonType.JumpPage)
													{
														EditorLableCenterWithBox("Page", () =>
														{
															buttonSetting.pageJumpId = EditorGUILayout.IntField(buttonSetting.pageJumpId);
														});
														
													}
													
												});
												

												if (GUILayout.Button("Remove Button"))
												{
													helpPage.buttonsSetting.RemoveAt(k);
												}

											}, GUILayout.Width(300));


										}
										if (GUILayout.Button("ADD BUTTON"))
										{
											helpPage.buttonsSetting.Add(new ButtonSetting());
										}
									});



									EditorGUILayout.Space();

									EditorVentical(new Color32(120, 120, 120, 255), () => { });
									EditorGUILayout.Space();

								}
								if (GUILayout.Button("ADD PAGE"))
								{
									helpGroup.helpPageSettings.Add(new HelpPageSetting());
								}
							});
						});



					}

					if (GUILayout.Button("ADD GROUP"))
					{
						slot.helpGroup.Add(new HelpPageGroup());
					}


				});

			});
			EditorVentical("HelpBox", () =>
			{


				if (GUILayout.Button("APPLY TO SCENE"))
				{
					//UpdateHelpUIToScene();

				}

			},GUILayout.Width(100));
			DragAndDropItem<GameObject>("Drag And Drop HelpPage Here to override..", (obj) => 
			{
				HelpPage hP = obj.GetComponent<HelpPage>();
				if (hP != null)
				{

					slot.helpGroup = new List<HelpPageGroup>( hP.helpGroup);
					
				}
			});
		}
		#endregion DrawHelpSetting

		#region DrawUiOtherSetting
		void DrawUiOtherSetting()
		{
			EditorGUILayout.LabelField("UI Other Setting", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

			EditorVentical("GroupBox", () =>
			{
				EditorVentical("HelpBox", () =>
				{
					EditorLableCenterWithBox("", () =>
					{
						if (GUILayout.Button("Start"))
						{
							//SlotGroupResult s = new SlotGroupResult(new SlotColIds(new List<int> { 9, 8, 4 }));

							//foreach (int i in s.row.col)
							//{
							//	Debug.Log(i);
							//}
							//Debug.Log(JsonUtility.ToJson(s));

							SlotServerResult s = JsonUtility.FromJson<SlotServerResult>("{\"pattern\":[5, 7, 2, 9, 8, 2, 2, 8, 0, 6, 11, 1, 5, 9, 3], \"totalwon\":{ \"pay\":5,\"freespin\":0}}");
							Debug.Log("pay: " + s.totalwon.pay);
							Debug.Log("Freespin: " + s.totalwon.freespin);
							string idlist = "(";
							foreach (int id in s.pattern)
							{
								idlist += id.ToString() + ",";
							}
							idlist += ")";
							Debug.Log(idlist);

						}
					});
					
					
				}, GUILayout.Width(100));

			});

			
		}
        #endregion DrawUiOtherSetting

        #region Update To Scene
		void UpdateToScene()
		{
			UpdateSlotControlToScene();
			UpdateUiSettingUiGameObjToScene();
			UpdateUiLineSettingToScene();
			UpdateUiSettingImageToScene();
			UpdateUiSettingTextToScene();
			UpdateUiButtonSettingToScene();
			UpdateHelpUIToScene();
			UpdateSoundToScene();
		}

		void UpdateSlotControlToScene()
		{
			SlotController slotC = FindObjectOfType<SlotController>();
			SlotMenuController slotM = FindObjectOfType<SlotMenuController>();
			UiGeneralSetting uGS = FindObjectOfType<UiGeneralSetting>();
			if (slotM != null)
			{
				slotM.enableDisplayZeroFreespin = slot.enableDisplayZeroFreespin;
				slotM.usePresetBet = slot.usePresetBet;
				slotM.presetBetIncrease.Clear();
				for(int i=0; i< slot.presetBetIncrease.Count; i++)
				{
					slotM.presetBetIncrease.Add(double.Parse(slot.presetBetIncrease[i].ToString("n2")));
				}
				slotM.enableStartPresetBet = slot.enableStartPresetBet;
				slotM.currentSelectedPresetId = slot.currentSelectedPresetId;
			}
			if (slotC != null)
			{
				slotC.slotIcons = slot.slotIcons.ToArray();
				slotC.payTable = new List<PayLine>(slot.payLines);
				slotC.scatter_id = slot.scatter_id;
				slotC.wild_id = slot.wild_id;
				slotC.wild_multiply = slot.wild_multiply;
				slotC.useWild = slot.useWild;
				slotC.useScatter = slot.useScatter;
				slotC.scatterWildSub = slot.scatterWildSub;
				slotC.scatterFollowSeq = slot.scatterFollowSeq;
				slotC.scatterPayTable = new List<ScatterPay>(slot.scatterPayTable);
				slotC.wildPayTable = new List<WildPay>(slot.wildPayTable);
				slotC.featureMultiply = slot.featureMultiply;
				slotC.mainRotateTime = slot.mainRotateTime;
				slotC.specialEffectTimeInterval = slot.specialEffectTime;
				slotC.enableSpecialEffectDuringAuto = slot.enableSpecialEffectDuringAuto;
				slotC.disableDisplayStandardWinOnFeature = slot.disableDisplayStandardWinOnFeature;
				slotC.enableDisplayWinInfoOnFeature = slot.enableDisplayWinInfoOnFeature;
				slotC.onlyAddCoinOnEndFeature = slot.onlyAddCoinOnEndFeature;
				slotC.useARCbetSystem = slot.useARCbetSystem;
				slotC.enableDisplayWinLineOnAuto = slot.enableDisplayWinLineOnAuto;
				slotC.forceStopTime = slot.forceStopTime;
				slotC.decimalDisplay = slot.decimalDisplay;
				slotC.tileX = slot.tileSize.x;
				slotC.tileY = slot.tileSize.y;
				if(slot.ARChelpInfoSprites.Count > 0)
                {
					slotC.helpPagesSize = slot.ARChelpInfoSize;
					slotC.helpPagesSprite = slot.ARChelpInfoSprites.ToArray();
                }
				slotC.specialIcon_id = slot.specialIcon_id;
				slotC.useSpecialIcon = slot.useSpecialIcon;
				slotC.specialIconFollowSeq = slot.specialIconFollowSeq;

				slotC.specialIconPayTable = new List<SpecialIconPay>(slot.specialIconPayTable);
				if (slotC.bonusScript != null)
				{
					GameObject bonus = slotC.bonusScript.gameObject;
					if (bonus != null)
					{
						GameObject.DestroyImmediate(bonus);
					}
				}
				
				if(slot.bonusGameObj != null)
				{
					
					GameObject newBonus = Instantiate(slot.bonusGameObj, GameObject.FindGameObjectWithTag("GameCanvas").transform);
					newBonus.name = slot.bonusGameObj.name;
					BonusGame newBonusGame = newBonus.GetComponent<BonusGame>();
					slotC.bonusScript = newBonusGame;
				}

				for (int i = 0; i < slotC.slotGroupsBeh.Length; i++)
				{
					GameObject.DestroyImmediate(slotC.slotGroupsBeh[i].gameObject);
				}
				List<SlotGroupBehavior> listSGB = new List<SlotGroupBehavior>();
				for(int i = 0; i < slot.slotGroups.Count; i++)
                {
					GameObject obj = Instantiate(uGS.slotGroupSetting.slotGroupPrefab, uGS.slotGroupSetting.SlotGroupParentTrans);
					SlotGroupBehavior sGB = obj.GetComponent<SlotGroupBehavior>();
					sGB.randomStartPosition = slot.enableRandomIconOnStart;
					SlotGroupEditorHelper sGS = obj.GetComponent<SlotGroupEditorHelper>();
                    if (slot.slotGroups[i].changeMaskingTrans)
                    {
						sGS.maskTrans.localPosition = slot.slotGroups[i].maskingTransSetting.position;
						sGS.maskTrans.localEulerAngles = slot.slotGroups[i].maskingTransSetting.rotation;
						sGS.maskTrans.localScale = slot.slotGroups[i].maskingTransSetting.scale;
						sGS.specialEffectTrans.localPosition = slot.slotGroups[i].specialEffectTransSetting.position;
						sGS.specialEffectTrans.localEulerAngles = slot.slotGroups[i].specialEffectTransSetting.rotation;
						sGS.specialEffectTrans.localScale = slot.slotGroups[i].specialEffectTransSetting.scale;
						
					}
					sGB.addRotate = slot.slotGroups[i].additionalSpinBeforeStop;

					sGS.RemoveAllRaycasters();
					for (int j = 0; j < slot.slotGroups[i].listOfRayCastTrans.Count; j++)
                    {
						sGS.AddRaycast(slot.slotGroups[i].listOfRayCastTrans[j].position);

					}
					
					listSGB.Add(sGB);

				}
				slotC.slotGroupsBeh = listSGB.ToArray();

				for (int i = 0; i< slotC.slotGroupsBeh.Length; i++)
				{
					slotC.slotGroupsBeh[i].symbOrder = slot.slotGroups[i].symbOrder;
					slotC.slotGroupsBeh[i].symbOrderFeature = slot.slotGroups[i].symbOrderFeature;
					slotC.slotGroupsBeh[i].transform.localPosition = slot.slotGroups[i].transSetting.position;
					slotC.slotGroupsBeh[i].transform.localEulerAngles = slot.slotGroups[i].transSetting.rotation;
					slotC.slotGroupsBeh[i].transform.localScale = slot.slotGroups[i].transSetting.scale;
				}




			}
		}

		void UpdateSoundToScene()
		{
			GameObject soundOnScene = GameObject.FindGameObjectWithTag("Sound");
			if(soundOnScene != null)
			{
				GameObject.DestroyImmediate(soundOnScene);
			}
			if(slot.soundGameObj != null)
			{
				GameObject newSound = Instantiate(slot.soundGameObj);
				newSound.name = slot.soundGameObj.name;
			}
			
		}


		void UpdateUiSettingImageToScene()
		{
			UiGeneralSetting uGS = FindObjectOfType<UiGeneralSetting>();
			if (uGS != null)
			{


				uGS.uiImageSetting.uiImageSceneTrans.Clear();
	
				uGS.uiImageSetting.listOfImagesSetting = new List<ImageSetting>(slot.uiGeneralImagesSetting);
				for (int i = 0; i < uGS.uiImageSetting.listOfImagesSetting.Count; i++)
				{
					ImageSetting iS = uGS.uiImageSetting.listOfImagesSetting[i];
					
					GameObject obj = new GameObject();
					obj.transform.SetParent(uGS.uiImageSetting.uiImageSceneTrans);
					Image image = obj.AddComponent<Image>();
					string name = iS.name;
					if (String.IsNullOrEmpty(name))
					{
						name = "Image (" + iS.sprite.name + ")";
					}

					obj.name = name;

					image.sprite = iS.sprite;

					RectTransform rect = obj.GetComponent<RectTransform>();
					rect.sizeDelta = iS.size;
					rect.anchoredPosition = new Vector2(iS.position.x, iS.position.y);
					rect.localScale = iS.scale;

					StateControl sC = obj.AddComponent<StateControl>();
					sC.listOfSpriteState = new List<SpriteState>(iS.listOfSpriteState);
					sC.uiImage = image;
					if (iS.enableSorting)
					{

						Canvas cv = obj.AddComponent<Canvas>();
						cv.overrideSorting = true;
						cv.sortingOrder = iS.sortingOrder;

					}
				}

			}
		}

		void UpdateUiLineSettingToScene()
		{
			UiGeneralSetting uGS = FindObjectOfType<UiGeneralSetting>();
			if (uGS != null)
			{
				uGS.uiLineSetting.UiLineButtonSceneTrans.Clear();
				uGS.uiLineSetting.lineSceneTrans.Clear();
				uGS.uiLineSetting.listOfLineSetting = new List<LineSetting>(slot.listOfLineSetting);
				List<SlotGroupBehavior> listOfSlotGB = new List<SlotGroupBehavior>(FindObjectOfType<SlotController>().slotGroupsBeh);
				for (int i = 0; i< uGS.uiLineSetting.listOfLineSetting.Count; i++)
				{
					LineSetting lS = uGS.uiLineSetting.listOfLineSetting[i];
					GameObject lineObj = Instantiate(uGS.uiLineSetting.linePrefab, uGS.uiLineSetting.lineSceneTrans);
					lineObj.name = "Line " + (i + 1);
					LineBehavior lineB = lineObj.GetComponent<LineBehavior>();

					lineB.number = i;
					
					lineB.animationWinLine.enableAnimationLine = lS.animationWinLine.enableAnimationLine;
                    if (lS.animationWinLine.enableAnimationLine)
                    {
						GameObject animLineGO = new GameObject();
						animLineGO.name = "Line Anim Win" + (i + 1);
						animLineGO.transform.SetParent(lineObj.transform);
						animLineGO.transform.localPosition = lS.animationWinLine.position;
						animLineGO.transform.localEulerAngles = lS.animationWinLine.rotation;
						animLineGO.transform.localScale = lS.animationWinLine.scale;
						SpriteRenderer sp = animLineGO.AddComponent<SpriteRenderer>();
						sp.sortingOrder = lS.animationWinLine.sortingOrder;
						Animator anim = animLineGO.AddComponent<Animator>();
						anim.runtimeAnimatorController = lS.animationWinLine.animC;
						lineB.animationWinLine.animationLineGameObj = animLineGO;
					}


					List<RayCaster> listRays = new List<RayCaster>();
					for (int j = 0; j < listOfSlotGB.Count; j++)
					{
						for(int k = 0; k< lS.slotGroupBehavior[j].ids.Count; k++)
						{
							if(lS.slotGroupBehavior[j].ids[k] == 1)
							{
								listRays.Add(listOfSlotGB[j].RayCasters[k]);
							}
						}
					}
					lineB.rayCasters = listRays.ToArray();
					lineB.typeOfLine = TypeOfWinningLine.line;
					lineB.lineMaterial = lS.lineMaterial;
					lineB.lineColor = lS.lineColor;
					lineB.lineFlashingSpeed = lS.lineFlashingSpeed;
					lineB.lineRendererWidth = lS.lineRendererWidth;
					lineB.lineSpeed = lS.lineSpeed;
					lineB.typeOfBoxWinning = lS.typeOfBoxWinning;
					lineB.boxSizeWidth = lS.boxSizeWidth;
					lineB.boxSizeHeight = lS.boxSizeHeight;

					LineCreator lC = lineObj.GetComponent<LineCreator>();
					if(lC != null)
					{
						lC.SetInitial();

					}

					GameObject buttonObj = new GameObject("LineButton " + (i + 1));
					buttonObj.transform.SetParent(uGS.uiLineSetting.UiLineButtonSceneTrans);
					RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
					buttonRect.sizeDelta = lS.uiButtonSetting.size;
					buttonRect.anchoredPosition = new Vector2(lS.uiButtonSetting.position.x, lS.uiButtonSetting.position.y);
					buttonRect.localScale = lS.uiButtonSetting.scale;

					Image image = buttonObj.AddComponent<Image>();
					image.sprite = lS.uiButtonSetting.sprite;
					image.preserveAspect = true;

					Button button = buttonObj.AddComponent<Button>();

					button.transition = Selectable.Transition.None;

					UILineButtonBehavior uiLineButtonB = buttonObj.AddComponent<UILineButtonBehavior>();
					uiLineButtonB.NormalSprite = lS.uiButtonSetting.sprite;
					uiLineButtonB.PressedSprite = lS.uiButtonSetting.pressedSprite;

					uiLineButtonB.button = button;

					StateControl sC = buttonObj.AddComponent<StateControl>();
					sC.listOfSpriteState = lS.uiButtonSetting.listOfSpriteState;
					sC.uiImage = image;

					GameObject buttonObj2 = new GameObject("LineButton (second) " + (i + 1));
					buttonObj2.transform.SetParent(uGS.uiLineSetting.UiLineButtonSceneTrans);
					RectTransform buttonRect2 = buttonObj2.AddComponent<RectTransform>();
					buttonRect2.sizeDelta = lS.uiButtonSecondSetting.size;
					buttonRect2.anchoredPosition = new Vector2(lS.uiButtonSecondSetting.position.x, lS.uiButtonSecondSetting.position.y);
					buttonRect2.localScale = lS.uiButtonSecondSetting.scale;

					Image image2 = buttonObj2.AddComponent<Image>();
					image2.sprite = lS.uiButtonSecondSetting.sprite;
					image2.preserveAspect = true;

					Button button2 = buttonObj2.AddComponent<Button>();

					button2.transition = Selectable.Transition.None;

					UILineButtonBehavior uiLineButtonB2 = buttonObj2.AddComponent<UILineButtonBehavior>();
					uiLineButtonB2.NormalSprite = lS.uiButtonSecondSetting.sprite;
					uiLineButtonB2.PressedSprite = lS.uiButtonSecondSetting.pressedSprite;

					uiLineButtonB2.button = button2;

					StateControl sC2 = buttonObj.AddComponent<StateControl>();
					sC2.listOfSpriteState = lS.uiButtonSecondSetting.listOfSpriteState;
					sC2.uiImage = image;
					
					if (lS.enableButton)
					{
						lineB.uiButton = uiLineButtonB;
						lineB.uiButtonLeft = uiLineButtonB2;
					}





				}
			}
		}

		void UpdateUiSettingUiGameObjToScene()
		{
			UiGeneralSetting uGS = FindObjectOfType<UiGeneralSetting>();
			if (uGS != null)
			{

				uGS.uiGameobjectSetting.UiGameObjectSceneTrans.Clear();
				uGS.uiGameobjectSetting.listOfUiGameobjSetting = new List<UiGameobjectSetting>(slot.uiGeneralGameobjSetting);
				for (int i = 0; i < uGS.uiGameobjectSetting.listOfUiGameobjSetting.Count; i++)
				{
					UiGameobjectSetting gS = uGS.uiGameobjectSetting.listOfUiGameobjSetting[i];
					GameObject obj = Instantiate(gS.gameObjectPrefab, uGS.uiGameobjectSetting.UiGameObjectSceneTrans);

					
					string name = gS.gameObjectPrefab.name;
					
					obj.name = name;

					RectTransform rect = obj.GetComponent<RectTransform>();
					rect.sizeDelta = gS.size;
					rect.anchoredPosition = new Vector2(gS.position.x, gS.position.y);
					rect.localScale = gS.scale;

				}

			}
		}

		void UpdateUiSettingTextToScene()
		{
			UiGeneralSetting uGS = FindObjectOfType<UiGeneralSetting>();
			if (uGS != null)
			{
				uGS.uiTextSetting.UiTextSceneTrans.Clear();
				uGS.uiTextSetting.listOfUiTextSetting = slot.uiGeneralTextSetting;
				for (int i = 0; i < uGS.uiTextSetting.listOfUiTextSetting.Count; i++)
				{
					UiTextSetting tS = uGS.uiTextSetting.listOfUiTextSetting[i];
					string name = tS.name;
					GameObject obj = new GameObject((string.IsNullOrEmpty(tS.name)) ? "text ("+tS.uiTextType+")" : tS.name);
					obj.transform.SetParent(uGS.uiTextSetting.UiTextSceneTrans);
					
					RectTransform rect = obj.AddComponent<RectTransform>();
					rect.localScale = tS.scale;
					rect.sizeDelta = tS.size;
					rect.anchoredPosition = tS.position;
					
					if (tS.enableOutline)
					{

						Outline line = obj.AddComponent<Outline>();
						line.effectColor = tS.outlineColor;
						line.effectDistance = tS.outlineDistance;
					}
					if (tS.enableGradient)
					{

						Gradient1 line = obj.AddComponent<Gradient1>();
						line.topColor = tS.topColor;
						line.bottomColor = tS.bottomColor;
					}
					if (tS.enableSorting)
					{

						Canvas cv = obj.AddComponent<Canvas>();
						cv.overrideSorting= true;
						cv.sortingOrder = tS.sortingOrder;
						
					}

					Text text = obj.AddComponent<Text>();
					text.font = tS.font;
					text.fontSize = tS.fontSize;
					text.alignment = tS.textAnchor;
					text.resizeTextForBestFit = tS.bestFit;
					text.resizeTextMaxSize = tS.textSizeMax;
					text.resizeTextMinSize = tS.textSizeMin;
					text.color = tS.textColor;
					text.horizontalOverflow = HorizontalWrapMode.Overflow;
					text.verticalOverflow = VerticalWrapMode.Overflow;
					SlotMenuController slotMenu = FindObjectOfType<SlotMenuController>();
					text.text = tS.content;
					switch (tS.uiTextType)
					{
						
						case UiTextSettingType.BalanceSumText:
							if (slotMenu)
							{
								slotMenu.BalanceSumText = text;
							}
							break;
						case UiTextSettingType.BalanceDiamondSumText:
							if (slotMenu)
							{
								slotMenu.BalanceDiamondSumText = text;
							}
							break;
						case UiTextSettingType.TotalBetSumText:
							if (slotMenu)
							{
								slotMenu.TotalBetSumText = text;
							}
							break;
						case UiTextSettingType.LinesCountText:
							if (slotMenu)
							{
								slotMenu.LinesCountText = text;
							}
							break;
						case UiTextSettingType.LineBetSumText:
							if (slotMenu)
							{
								slotMenu.LineBetSumText = text;
							}
							break;
						case UiTextSettingType.FreeSpinCountText:
							if (slotMenu)
							{
								slotMenu.FreeSpinCountText = text;
							}
							break;
						case UiTextSettingType.WinCoinText:
							if (slotMenu)
							{
								slotMenu.WinCoinText = text;
							}
							break;
						case UiTextSettingType.ErrorInfo:
							if (slotMenu)
							{
								slotMenu.ErrorInfo = text;
							}
							break;
						case UiTextSettingType.MiniJackpotSumText:
							if (slotMenu)
							{
								slotMenu.MiniJackpotSumText = text;
							}
							break;
						case UiTextSettingType.MaxiJackpotSumText:
							if (slotMenu)
							{
								slotMenu.MaxiJackpotSumText = text;
							}
							break;
						case UiTextSettingType.MegaJackpotSumText:
							if (slotMenu)
							{
								slotMenu.MegaJackpotSumText = text;
							}
							break;
						case UiTextSettingType.resultWinText:
							if (slotMenu)
							{
								slotMenu.resultWinText = text;
							}
							break;
						case UiTextSettingType.totalLineWinCount:
							if (slotMenu)
							{
								slotMenu.totalLineWinCount = text;
							}
							break;
						case UiTextSettingType.LineBetSumWithWordsText:
							if (slotMenu)
							{
								slotMenu.LineBetSumWithWordsText = text;
							}
							break;
						case UiTextSettingType.LinesCountWithWordsText:
							if (slotMenu)
							{
								slotMenu.LinesCountWithWordsText = text;
							}
							break;
						case UiTextSettingType.totalFeatureWinInfoText:
							if (slotMenu)
							{
								slotMenu.totalFeatureWinInfoText = text;
							}
							break;
						case UiTextSettingType.standardLineWin:
							if (slotMenu)
							{
								slotMenu.standardLineWin = text;
							}
							break;
					}
					
				}
			}
		}

		void UpdateUiButtonSettingToScene()
		{
			UiGeneralSetting uGS = FindObjectOfType<UiGeneralSetting>();
			
			
			if (uGS != null)
			{
			
				uGS.uiButtonSetting.UiButtonSceneTrans.Clear();
				uGS.uiButtonSetting.LinePlus = slot.LinePlus;
				
				UiButtonSettingUpdate(uGS.uiButtonSetting.LinePlus, uGS.uiButtonSetting.UiButtonSceneTrans, "LinePlus","LinesPlus_Click" );

				uGS.uiButtonSetting.LineMinus = slot.LineMinus;

				UiButtonSettingUpdate(uGS.uiButtonSetting.LineMinus, uGS.uiButtonSetting.UiButtonSceneTrans, "LineMinus", "LinesMinus_Click");

				uGS.uiButtonSetting.BetPlus = slot.BetPlus;

				Button buttonPlus = UiButtonSettingUpdate(uGS.uiButtonSetting.BetPlus, uGS.uiButtonSetting.UiButtonSceneTrans, "BetPlus", "LineBetPlus_Click");
                if (slot.BetPlus.enableLongPress)
                {
					BetLongPress bLP = buttonPlus.gameObject.AddComponent<BetLongPress>();
					bLP.timeStart = slot.BetPlus.longPressStartTime;
					bLP.timeInterval = slot.BetPlus.longPressIntervalTime;
					bLP.IsIncrease = true;
				}

				uGS.uiButtonSetting.BetMinus = slot.BetMinus;

				Button buttonMinus = UiButtonSettingUpdate(uGS.uiButtonSetting.BetMinus, uGS.uiButtonSetting.UiButtonSceneTrans, "BetMinus", "LineBetMinus_Click");
				if (slot.BetMinus.enableLongPress)
				{
					BetLongPress bLP = buttonMinus.gameObject.AddComponent<BetLongPress>();
					bLP.timeStart = slot.BetMinus.longPressStartTime;
					bLP.timeInterval = slot.BetMinus.longPressIntervalTime;
					bLP.IsIncrease = false;
				}
				uGS.uiButtonSetting.Back = slot.Back;

				UiButtonSettingUpdate(uGS.uiButtonSetting.Back, uGS.uiButtonSetting.UiButtonSceneTrans, "Back", "Lobby_Click");

				uGS.uiButtonSetting.Menu = slot.Menu;

				UiButtonSettingUpdate(uGS.uiButtonSetting.Menu, uGS.uiButtonSetting.UiButtonSceneTrans, "Menu", "MainMenu_Click");

				uGS.uiButtonSetting.AutoSpin = slot.AutoSpin;
				Button AutoSpin = UiButtonSettingUpdate(uGS.uiButtonSetting.AutoSpin, uGS.uiButtonSetting.UiButtonSceneTrans, "Auto", "AutoSpin_Clicking");
				AutoSpin.gameObject.AddComponent<StartButtonBehavior>();
				uGS.uiButtonSetting.Spin = slot.Spin;
				
				Button Spin = UiButtonSettingUpdate(uGS.uiButtonSetting.Spin, uGS.uiButtonSetting.UiButtonSceneTrans, "Spin", "Spin_Clicking");
				Spin.gameObject.AddComponent<StartButtonBehavior>();
				
				SlotMenuController sMC = FindObjectOfType<SlotMenuController>();
				sMC.spinButton = Spin;
				sMC.autoButton = AutoSpin;

			}
		}

		Button UiButtonSettingUpdate(UiButtonSetting uBS, Transform parent, string name, string callaction)
		{
			Button button = null;
			CreateUIButton(uBS, parent, name, ref button);

			SlotMenuController sMC = FindObjectOfType<SlotMenuController>();
			var CloseTargetInfo = UnityEvent.GetValidMethodInfo(sMC, callaction, new Type[0]);
			UnityAction methodDelegate = System.Delegate.CreateDelegate(typeof(UnityAction), sMC, CloseTargetInfo) as UnityAction;
			UnityEventTools.AddVoidPersistentListener(button.onClick, methodDelegate);
			return button;
		}

		GameObject CreateUIButton(UiButtonSetting uBS, Transform parent, string name, ref Button button)
		{
			GameObject obj = new GameObject(name);
			obj.transform.SetParent(parent);

			RectTransform rect = obj.AddComponent<RectTransform>();
			rect.localScale = uBS.scale;
			rect.sizeDelta = uBS.size;
			rect.anchoredPosition = uBS.position;

			Image image = obj.AddComponent<Image>();
			image.sprite = uBS.sprite;
            if (uBS.enableSorting)
            {
				Canvas cv = obj.AddComponent<Canvas>();
				obj.AddComponent<GraphicRaycaster>();
				cv.overrideSorting = true;
				cv.sortingOrder = uBS.sortingOrder;

			}
			button = obj.AddComponent<Button>();
			button.transition = Selectable.Transition.SpriteSwap;
			UnityEngine.UI.SpriteState sprState = new UnityEngine.UI.SpriteState();
			sprState.pressedSprite = uBS.pressedSprite;
			button.targetGraphic = image;
			button.spriteState = sprState;

			StateControl stateControl = obj.AddComponent<StateControl>();
			stateControl.uiImage = image;
			stateControl.listOfSpriteState = uBS.listOfSpriteState;

			return obj;
		}


		void UpdateHelpUIToScene()
		{
			HelpPage helpPage;
			helpPage = FindObjectOfType<HelpPage>();
			if (helpPage)
			{
				helpPage.helpGroup = new List<HelpPageGroup>(slot.helpGroup);
			}
			else return;

			GameObject[] listOfOldPageGroup = GameObject.FindGameObjectsWithTag("HelpUiPanel");

			foreach (GameObject obj in listOfOldPageGroup)
			{
				GameObject.DestroyImmediate(obj);
			}

			for (int i = 0; i < slot.helpGroup.Count; i++)
			{
				Transform parent = helpPage.gameObject.transform;
				GameObject helpObj = Instantiate(helpPage.helpPagePrefab, parent);
				helpObj.name = slot.helpGroup[i].name;
				pageScroll pageS = helpObj.GetComponent<pageScroll>();
				Transform pageContentTrans = pageS.content.transform;
				RectTransform contentRect = pageS.content;
				pageS.content.gameObject.SetActive(false);
				pageContentTrans.Clear();
				StateControl stateCon = helpObj.GetComponent<StateControl>();
				pageS.content.transform.Clear();

				if (i != 0)
				{

					stateCon.listOfGameObjState.Clear();
					GameObjectState gOS = new GameObjectState();
					gOS.idName = slot.helpGroup[i].name;
					gOS.listOfGameObjToEnable.Add(pageContentTrans.gameObject);

					stateCon.listOfGameObjState.Add(gOS);

				}


				int counter = 0;

				foreach (HelpPageSetting HPS in slot.helpGroup[i].helpPageSettings)
				{

					counter++;
					GameObject page = new GameObject("page " + counter);
					Transform pageTrans = page.transform;

					pageTrans.SetParent(pageContentTrans);

					pageTrans.localScale = new Vector3(1, 1, 1);

					RectTransform pageRect = page.AddComponent<RectTransform>() as RectTransform;
					pageRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, contentRect.sizeDelta.y);
					Image pageImage = page.AddComponent<Image>() as Image;
					pageImage.sprite = HPS.pageImage;

					int buttonCounter = 0;
					foreach (ButtonSetting BS in HPS.buttonsSetting)
					{
						buttonCounter++;
						GameObject buttonObj = new GameObject("button " + buttonCounter);

						buttonObj.transform.SetParent(pageRect);
						buttonObj.transform.localScale = BS.rect.scale;
						RectTransform buttonRect = buttonObj.AddComponent<RectTransform>() as RectTransform;

						buttonRect.sizeDelta = BS.rect.size;
						buttonRect.anchoredPosition = new Vector2(BS.rect.position.x, BS.rect.position.y);

						Image buttonImage = buttonObj.AddComponent<Image>() as Image;
						if (BS.sprite == null)
						{
							buttonImage.color = new Color(1, 1, 1, 0);
						}
						else
						{
							buttonImage.sprite = BS.sprite;
						}


						Button buttonB = buttonObj.AddComponent<Button>() as Button;


						switch (BS.buttonAction)
						{
							case HelpPageButtonType.Close:
								var CloseTargetInfo = UnityEvent.GetValidMethodInfo(contentRect.gameObject, "SetActive", new Type[] { typeof(bool) });
								UnityAction<bool> CloseMethodDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), contentRect.gameObject, CloseTargetInfo) as UnityAction<bool>;
								UnityEventTools.AddBoolPersistentListener(buttonB.onClick, CloseMethodDelegate, false);
								break;
							case HelpPageButtonType.JumpPage:
								var JumpPageTargetInfo = UnityEvent.GetValidMethodInfo(pageS, "pageTo", new Type[] { typeof(int) });
								UnityAction<int> JumpPageMethodDelegate = Delegate.CreateDelegate(typeof(UnityAction<int>), pageS, JumpPageTargetInfo) as UnityAction<int>;
								UnityEventTools.AddIntPersistentListener(buttonB.onClick, JumpPageMethodDelegate, BS.pageJumpId);
								break;
							case HelpPageButtonType.NextPage:

								if (counter <= slot.helpGroup[i].helpPageSettings.Count)
								{
									var NextPageTargetInfo = UnityEvent.GetValidMethodInfo(pageS, "pageTo", new Type[] { typeof(int) });
									UnityAction<int> NextPageMethodDelegate = Delegate.CreateDelegate(typeof(UnityAction<int>), pageS, NextPageTargetInfo) as UnityAction<int>;
									UnityEventTools.AddIntPersistentListener(buttonB.onClick, NextPageMethodDelegate, counter);
								}



								break;
							case HelpPageButtonType.PreviousPage:
								if (counter > 1)
								{
									var NextPageTargetInfo = UnityEvent.GetValidMethodInfo(pageS, "pageTo", new Type[] { typeof(int) });
									UnityAction<int> NextPageMethodDelegate = Delegate.CreateDelegate(typeof(UnityAction<int>), pageS, NextPageTargetInfo) as UnityAction<int>;
									UnityEventTools.AddIntPersistentListener(buttonB.onClick, NextPageMethodDelegate, counter - 2);
								}
								break;
							case HelpPageButtonType.OpenHelpGroup:

								StateControllerManager sCM = FindObjectOfType<StateControllerManager>();

								var OpenHelpGroupTargetInfo = UnityEvent.GetValidMethodInfo(sCM, "ChangeStateN", new Type[] { typeof(string) });
								UnityAction<string> OpenHelpGroupMethodDelegate = Delegate.CreateDelegate(typeof(UnityAction<string>), sCM, OpenHelpGroupTargetInfo) as UnityAction<string>;
								UnityEventTools.AddStringPersistentListener(buttonB.onClick, OpenHelpGroupMethodDelegate, BS.openGroupName);

								break;
						}
					}
				}

			}
		}

		#endregion Update To Scene

		#endregion Draw UI

		#region Editor Extend

		Rect EditorHorizontal(Color color, Action innerAction, params GUILayoutOption[] options)
		{
			GUIStyle style = new GUIStyle(GUI.skin.box);
			Texture2D selectedTex = new Texture2D(2, 2);
			selectedTex.SetColor(color);

			style.normal.background = selectedTex;

			Rect r = EditorGUILayout.BeginHorizontal(options);
			GUI.Box(r, "", style);

			if (innerAction != null) innerAction.Invoke();

			EditorGUILayout.EndHorizontal();
			return r;
		}

		Rect EditorHorizontal(GUIStyle style, Action innerAction, params GUILayoutOption[] options)
		{

			Rect r = EditorGUILayout.BeginHorizontal(style, options);
			
			if(innerAction != null) innerAction.Invoke();
			
			EditorGUILayout.EndHorizontal();
			return r;
		}

		Rect EditorHorizontal(Action innerAction, params GUILayoutOption[] options)
		{
			Rect r =EditorGUILayout.BeginHorizontal(options);

			if (innerAction != null) innerAction.Invoke();

			EditorGUILayout.EndHorizontal();
			return r;
		}
		Rect EditorHorizontal(Action<Rect> innerAction, params GUILayoutOption[] options)
		{
			Rect r = EditorGUILayout.BeginHorizontal(options);

			if (innerAction != null) innerAction.Invoke(r);

			EditorGUILayout.EndHorizontal();
			return r;
		}
		Rect EditorVentical(Color color, Action innerAction, params GUILayoutOption[] options)
		{
			GUIStyle style = new GUIStyle(GUI.skin.box);
			Texture2D selectedTex = new Texture2D(2, 2);
			selectedTex.SetColor(color);
			style.normal.background = selectedTex;
			Rect r = EditorGUILayout.BeginVertical(options);
			GUI.Box(r, "", style);

			if (innerAction != null) innerAction.Invoke();

			EditorGUILayout.EndVertical();
			return r;
		}

		Rect EditorVentical(GUIStyle style, Action innerAction, params GUILayoutOption[] options)
		{
			Rect r =EditorGUILayout.BeginVertical(style, options);

			if (innerAction != null) innerAction.Invoke();

			EditorGUILayout.EndVertical();
			return r;
		}
		Rect EditorVentical(GUIStyle style, Action<Rect> innerAction, params GUILayoutOption[] options)
		{
			Rect r = EditorGUILayout.BeginVertical(style, options);

			if (innerAction != null) innerAction.Invoke(r);

			EditorGUILayout.EndVertical();
			return r;
		}

		Rect EditorVentical(Action innerAction, params GUILayoutOption[] options)
		{
			Rect r = EditorGUILayout.BeginVertical(options);

			if (innerAction != null) innerAction.Invoke();

			EditorGUILayout.EndVertical();

			return r;
		}

		void EditorLableCenterWithBox(string lable, Action action)
		{
			EditorVentical(new Color32(120, 120, 120, 255), () =>
			{
				if (!string.IsNullOrEmpty(lable))
				{
					EditorGUILayout.LabelField(lable, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
				}
				
				if (action != null) action.Invoke();

			});
		}

		void EditorDrawTrans(string lable,TransfomSetting transSetting, Action extendAction)
		{

			Rect r = EditorVentical("GroupBox", () =>
			{
				EditorGUILayout.LabelField(lable, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
				EditorLableCenterWithBox("Position", ()=> 
				{
					EditorHorizontal(() =>
					{
						transSetting.position.x = EditorGUILayout.FloatField(transSetting.position.x);
						transSetting.position.y = EditorGUILayout.FloatField(transSetting.position.y);
						transSetting.position.z = EditorGUILayout.FloatField(transSetting.position.z);
					});
				});
				EditorLableCenterWithBox("Rotation", () =>
				{
					EditorHorizontal(() =>
					{
						transSetting.rotation.x = EditorGUILayout.FloatField(transSetting.rotation.x);
						transSetting.rotation.y = EditorGUILayout.FloatField(transSetting.rotation.y);
						transSetting.rotation.z = EditorGUILayout.FloatField(transSetting.rotation.z);

					});
				});
				EditorLableCenterWithBox("Scale", () =>
				{
					EditorHorizontal(() =>
					{
						transSetting.scale.x = EditorGUILayout.FloatField(transSetting.scale.x);
						transSetting.scale.y = EditorGUILayout.FloatField(transSetting.scale.y);
						transSetting.scale.z = EditorGUILayout.FloatField(transSetting.scale.z);
					});
				});
				
				if (extendAction != null) extendAction.Invoke();
				

			});

			DragAndDropItem<GameObject>(r, (obj) => 
			{
				Transform trans = obj.GetComponent<Transform>();
				if (trans)
				{
					transSetting.scale = trans.localScale;
					transSetting.position = trans.localPosition;
					transSetting.rotation = trans.localEulerAngles;
				}
			});

		}



		void EditorDrawRectSetting(string lable, RectSetting rectSetting, Action extendAction)
		{

			Rect r = EditorVentical("GroupBox", () =>
			{
				EditorGUILayout.LabelField(lable, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
				EditorLableCenterWithBox("Position", () =>
				{
					EditorHorizontal(() =>
					{
						rectSetting.position.x = EditorGUILayout.FloatField(rectSetting.position.x);
						rectSetting.position.y = EditorGUILayout.FloatField(rectSetting.position.y);

					});
				});
				EditorLableCenterWithBox("Size", () =>
				{
					EditorHorizontal(() =>
					{
						rectSetting.size.x = EditorGUILayout.FloatField(rectSetting.size.x);
						rectSetting.size.y = EditorGUILayout.FloatField(rectSetting.size.y);

					});
				});
				EditorLableCenterWithBox("Scale", () =>
				{
					EditorHorizontal(() =>
					{
						rectSetting.scale.x = EditorGUILayout.FloatField(rectSetting.scale.x);
						rectSetting.scale.y = EditorGUILayout.FloatField(rectSetting.scale.y);
						rectSetting.scale.z = EditorGUILayout.FloatField(rectSetting.scale.z);
					});
				});

				if (extendAction != null) extendAction.Invoke();


			});

			DragAndDropItem<GameObject>(r, (obj) =>
			{
				RectTransform rect = obj.GetComponent<RectTransform>();
				if (rect)
				{
					rectSetting.scale = rect.localScale;
					rectSetting.position = rect.anchoredPosition;
					rectSetting.size = rect.sizeDelta;
				}
			});

		}

		Rect EditorDrawStateImageContol(List<SpriteState> listOfSpriteState)
		{
			return EditorVentical("GroupBox", () =>
			{
				EditorHorizontal(() =>
				{
					EditorGUILayout.LabelField("STATE GROUP", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
					if (GUILayout.Button("ADD STATE", GUILayout.Width(100)))
					{
						listOfSpriteState.Add(new SpriteState());
					}
				});



				for (int j = 0; j < listOfSpriteState.Count; j++)
				{
					SpriteState spriteState = listOfSpriteState[j];

					EditorHorizontal("GroupBox", () =>
					{
						EditorVentical(() =>
						{
							EditorLableCenterWithBox("State Name", () =>
							{
								spriteState.IdName = EditorGUILayout.TextField(spriteState.IdName);
							});
							EditorLableCenterWithBox("Sprite From", () =>
							{
								spriteState.CurrentSprite = (Sprite)EditorGUILayout.ObjectField(spriteState.CurrentSprite, typeof(Sprite), false);
							});
							EditorLableCenterWithBox("To", () =>
							{
								spriteState.ChangeToSprite = (Sprite)EditorGUILayout.ObjectField(spriteState.ChangeToSprite, typeof(Sprite), false);
							});
							EditorLableCenterWithBox("", () =>
							{
								if (GUILayout.Button("Remove"))
								{
									listOfSpriteState.RemoveAt(j);
									return;
								}
							});
						});


						EditorVentical("HelpBox", () =>
						{
							EditorLableCenterWithBox("Display", null);


							if (spriteState.CurrentSprite != null)
							{
								EditorGUILayout.LabelField("From");
								DrawSprite(spriteState.CurrentSprite, 300);

							}
							if (spriteState.ChangeToSprite != null)
							{
								EditorGUILayout.LabelField("To");
								DrawSprite(spriteState.ChangeToSprite, 300);

							}
						});

					});



				}
			});


		}

		void DrawLine()
		{
			Texture2D normalTex = new Texture2D(2, 2);
			normalTex.SetColor(new Color(0, 0, 0, 1));
			
			GUILayout.Box(normalTex, GUILayout.ExpandWidth(true), GUILayout.Height(1));
		}

		void DragAndDropItem<T>(Rect rect, Action<T> callback)
		{
			if (rect.Contains(Event.current.mousePosition))
			{
				if (Event.current.type == EventType.DragUpdated)
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

					Event.current.Use();
				}
				else if (Event.current.type == EventType.DragPerform)
				{


					for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
					{
						if (DragAndDrop.objectReferences[i] is T item)
						{

							if (callback != null)
							{
								callback.Invoke(item);
							}


						}

					}
					Event.current.Use();
				}
			}
		}

		void DragAndDropItem<T>(string label, Action<T> callback)
		{
			Rect myRect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true), GUILayout.Height(50));
			GUI.Box(myRect, label);
			if (myRect.Contains(Event.current.mousePosition))
			{
				if (Event.current.type == EventType.DragUpdated)
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

					Event.current.Use();
				}
				else if (Event.current.type == EventType.DragPerform)
				{


					for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
					{
						if (DragAndDrop.objectReferences[i] is T item)
						{

							if (callback != null)
							{
								callback.Invoke(item);
							}


						}

					}
					Event.current.Use();
				}
			}
		}

		#endregion Editor Extend

		#region Image display
		protected Texture ConvertSprite(Sprite itemImage)
		{
			if(itemImage == null)
			{
				return null;
			}
			if (itemImage.rect.width != itemImage.texture.width)
			{
				Texture2D newText = new Texture2D((int)itemImage.rect.width, (int)itemImage.rect.height);
				Color[] newColors = itemImage.texture.GetPixels((int)itemImage.textureRect.x,
															 (int)itemImage.textureRect.y,
															 (int)itemImage.textureRect.width,
															 (int)itemImage.textureRect.height);
				newText.SetPixels(newColors);
				newText.Apply();
				return newText;
			}
			else
				return itemImage.texture;
		}

		Vector2 GetSpriteSize(Sprite sprite, float fixWidth)
		{
			float height = sprite.rect.height / sprite.rect.width * fixWidth;

			return new Vector2(fixWidth, height);


		}

		void DrawSelectionIcon(ref int iconId, bool displayImage = true, bool displayAny = true, float width= 0)
		{
			List<string> iconNames = new List<string>();
			GUILayoutOption[] optionPop = new GUILayoutOption[] {
				GUILayout.ExpandWidth(true)
			};
			if (width > 0)
			{
				optionPop = new GUILayoutOption[] {
					GUILayout.Width(width)
				};
			}
			foreach (SlotIcon s in slot.slotIcons)
			{
				if (s.iconSprite != null) iconNames.Add(s.iconSprite.name);
			}
			if(displayAny)
				iconNames.Add("Any");

			EditorGUILayout.BeginVertical("HelpBox", optionPop);

			int id = iconId;
			if (displayAny)
			{
				if (id == -1)
				{
					id = slot.slotIcons.Count;
				}
			}
			
			iconId = EditorGUILayout.Popup("", id, iconNames.ToArray(), optionPop);

			if (iconId != slot.slotIcons.Count)
			{
				if (slot.slotIcons[iconId].iconSprite != null && displayImage)
				{
					
					GUILayoutOption[] option = new GUILayoutOption[] { 
						GUILayout.ExpandWidth(true)
					};
					if(width > 0)
					{
						option = new GUILayoutOption[] {
							GUILayout.Width(width),
							GUILayout.Height(GetSpriteSize(slot.slotIcons[iconId].iconSprite, width).y)
						};
					}
					GUILayout.Box(ConvertSprite(slot.slotIcons[iconId].iconSprite),option);
				}
					
			}
			else
			{
				if (displayAny) iconId = -1;
			}
			EditorGUILayout.EndVertical();
		}

		void DrawIcon(int iconId, Action onClick = null, params GUILayoutOption[] options)
		{
			if (iconId != slot.slotIcons.Count)
			{
				if (slot.slotIcons[iconId].iconSprite != null)
				{
					if(GUILayout.Button(ConvertSprite(slot.slotIcons[iconId].iconSprite), options))
					{
						if (onClick != null) onClick.Invoke();
					}
				}
					
			}
		}

		void DrawIcon(int iconId, float width)
		{
			if (iconId != slot.slotIcons.Count)
			{
				if (slot.slotIcons[iconId].iconSprite != null)
				{
					DrawSprite(slot.slotIcons[iconId].iconSprite, width);
					
				}

			}
		}

		void DrawSprite(Sprite sprite, float width)
		{
			GUILayout.Box(ConvertSprite(sprite), GUILayout.Width(width), GUILayout.Height(GetSpriteSize(sprite, width).y));
		}

        #endregion Image diaplay



        #region Save 

        void OnEnable()
		{
			AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
		}

		void OnDisable()
		{
			SavingAsset(slot);
			AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
		}
		public void OnBeforeAssemblyReload()
		{
			SavingAsset(slot);
			

		}

		private void SavingAsset(SlotAsset slot)
		{
			if (slot)
			{
				slot.LastSave = DateTime.Now.ToString("MM/dd/yyyy H:mm");
				slot.Save();
				EditorUtility.SetDirty(slot);

			}
		}
		public void OnAfterAssemblyReload()
		{


		}

        #endregion Save


    }

    public static class Texture2DExtensions
	{

		public static void SetColor(this Texture2D tex2, Color32 color)
		{


			var fillColorArray = tex2.GetPixels32();

			for (var i = 0; i < fillColorArray.Length; ++i)
			{
				fillColorArray[i] = color;
			}

			tex2.SetPixels32(fillColorArray);

			tex2.Apply();
		}
	}
	public static class TransformEx
	{
		public static Transform Clear(this Transform transform)
		{
			var tempList = transform.Cast<Transform>().ToList();
			foreach (var child in tempList)
			{
				GameObject.DestroyImmediate(child.gameObject);
			}
	
			return transform;
		}
	}

	public enum TypeOfSection
	{
		slotIcon,
		paytable,
		scatterPayTable,
		lineSetting,
		wildPayTable,
		slotGroup,
		bonusSetting,
		featureSetting,
		RTPSetting,
		otherSlotControlSetting,
		UiGeneralSetting,
		HelpSetting,
		ARCHelpPages,
		UiOtherSetting,
		Testing,
		specialIconPayTable,
	}
	[System.Serializable]
	public class SlotServerResult
	{
		public List<int> pattern;
		public PayWinServer totalwon;

	}
	[System.Serializable]
	public class PayWinServer
	{
		public double pay;
		public int freespin;
	}

	[System.Serializable]
	public class WinSet
	{
		public string iconIdPattern;
		public string winIdPattern;
		public float winRate;
		public bool isFeature;

		public WinSet(string iconIdPattern, string winIdPattern, float winRate, bool isFeature)
		{
			this.iconIdPattern = iconIdPattern;
			this.winIdPattern = winIdPattern;
			this.winRate = winRate;
			this.isFeature = isFeature;
		}

		public bool CheckIsSame(WinSet winSet)
		{
			bool isSame = false;

			if(iconIdPattern == winSet.iconIdPattern && isFeature == winSet.isFeature)
			{
				isSame = true;
			}

			return isSame;

		}
	}

	[System.Serializable]
	public class StoreWinSet
	{
		public string name;
		public List<WinSet> listOfWinSet = new List<WinSet>();

		public void CheckAndAdd(WinSet winSet)
		{
			bool hasSame = false;
			foreach(WinSet set in listOfWinSet)
			{
				if (set.CheckIsSame(winSet))
				{
					hasSame = true;
					break;
				}
			}

			if (!hasSame)
			{
				listOfWinSet.Add(winSet);
			}
		}
	}
}

