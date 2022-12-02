using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FXEnhancer))]
public class FXEnhancerEditor : PropertyDrawer
{

	void ResetSettings(object userData, string[] options, int selected)
	{
		int realSelection = -1;
		int optionsCount = 0;

		for(int i=0;i<actionsCount;i++)
		{
			if (!serProps[i].boolValue)
			{
				if (selected == optionsCount)
					realSelection = i;
				optionsCount++;
			}
		}
		
		serProps[realSelection].boolValue = true;
       	serProps[realSelection].serializedObject.ApplyModifiedProperties();
	}

	const int actionsCount = 16;
	string[] properties = new string[actionsCount] {"playSound", "playParticles", "playAnimation", "playEvent",
										"cameraShake", "cameraZoom", "playRecoil", "playAbberations",
										"playFreeze", "playTimeScale", "playLight", "playFlash",
										"playVFX", "spawnPhysic", "updateGameManager", "playSFX"}; 
	SerializedProperty[] serProps = new SerializedProperty[actionsCount];

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
		position.height = GetPropertyHeight(property, null);

        EditorGUI.BeginProperty(position, label, property);

		for(int i=0;i<actionsCount;i++)
		{
			serProps[i] = property.FindPropertyRelative(properties[i]);
		}

		EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(ObjectNames.NicifyVariableName(property.name)));

		if (EditorGUI.DropdownButton(new Rect(position.x+EditorGUIUtility.labelWidth, position.y, position.width-EditorGUIUtility.labelWidth, 16f), 
			new GUIContent("Add effect"), FocusType.Keyboard))
		{
			int optionsCount = 0;
			for(int i=0;i<actionsCount;i++)
			{
				if (!serProps[i].boolValue)
					optionsCount++;
			}

			if (optionsCount>0)
			{
				GUIContent[] menuEntries = new GUIContent[optionsCount];

				optionsCount = 0;
				for(int i=0;i<actionsCount;i++)
				{
					if (!serProps[i].boolValue)
					{
						menuEntries[optionsCount]=EditorGUIUtility.TrTextContent(ObjectNames.NicifyVariableName(properties[i]));
						optionsCount++;
					}
				}
				var myrect = new Rect(position.x+EditorGUIUtility.labelWidth, position.y, position.width-EditorGUIUtility.labelWidth, 16f);
				EditorUtility.DisplayCustomMenu(myrect, menuEntries, -1, ResetSettings, null);
			}
		}

		position.y += 18.0f;
		EditorGUI.indentLevel++;
		Rect rect;
		bool newVal;

		for(int i=0;i<actionsCount;i++)
		{
			if (serProps[i].boolValue)
			{
				rect = new Rect(position.x, position.y, position.width, 16f);
				newVal =  EditorGUI.Toggle(rect, ObjectNames.NicifyVariableName(properties[i]), serProps[i].boolValue);
				if (newVal != serProps[i].boolValue)
				{
					serProps[i].boolValue = newVal;
					serProps[i].serializedObject.ApplyModifiedProperties();
				}

				switch(i)
				{
				case 0 : 
					// SOUNDS
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect, property.FindPropertyRelative( "audioClipCount" ) );
						for(int ac=0;ac<property.FindPropertyRelative( "audioClipCount" ).intValue;ac++)
						{
							position.y += 18.0f;
							rect = new Rect(position.x, position.y, position.width, 16f);
							EditorGUI.PropertyField( rect, property.FindPropertyRelative( "audioClipList" ).GetArrayElementAtIndex(ac) );
						}

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						SerializedProperty volRangeMin = property.FindPropertyRelative("volumeRangeMin");
						SerializedProperty volRangeMax = property.FindPropertyRelative("volumeRangeMax");

						float minRange = volRangeMin.floatValue;
						float maxRange = volRangeMax.floatValue;
						if (position.width>480.0f)	
							EditorGUI.MinMaxSlider(rect, new GUIContent("Volume Range ("+minRange.ToString("F2")+","+maxRange.ToString("F2")+")" ), ref minRange, ref maxRange, 0.0f, 1.0f);
						else
							EditorGUI.MinMaxSlider(rect, new GUIContent("Volume Range"), ref minRange, ref maxRange, 0.0f, 1.0f);
						if (minRange != volRangeMin.floatValue)
						{
							volRangeMin.floatValue = minRange;
							volRangeMin.serializedObject.ApplyModifiedProperties();
						}
						if (maxRange != volRangeMax.floatValue)
						{
							volRangeMax.floatValue = maxRange;
							volRangeMax.serializedObject.ApplyModifiedProperties();
						}

						SerializedProperty pitchRangeMin = property.FindPropertyRelative("pitchRangeMin");
						SerializedProperty pitchRangeMax = property.FindPropertyRelative("pitchRangeMax");
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);

						minRange = pitchRangeMin.floatValue;
						maxRange = pitchRangeMax.floatValue;
						if (position.width>440.0f)	
							EditorGUI.MinMaxSlider(rect, new GUIContent("Pitch Range ("+minRange.ToString("F2")+","+maxRange.ToString("F2")+")" ), ref minRange, ref maxRange, 0.05f, 2.0f);
						else
							EditorGUI.MinMaxSlider(rect, new GUIContent("Pitch Range"), ref minRange, ref maxRange, 0.05f, 2.0f);
						if (minRange != pitchRangeMin.floatValue)
						{
							pitchRangeMin.floatValue = minRange;
							pitchRangeMin.serializedObject.ApplyModifiedProperties();
						}
						if (maxRange != pitchRangeMax.floatValue)
						{
							pitchRangeMax.floatValue = maxRange;
							pitchRangeMax.serializedObject.ApplyModifiedProperties();
						}
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "playSoundDelay" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative("audiomixerGroup"));
						EditorGUI.indentLevel--;
					}
					break;
				case 1 :
					// PARTICLES
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "particlesPrefab" ));

						EditorGUI.indentLevel--;
					}
					break;
				case 2 : 
					// ANIMATION
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "animationToLaunch" ));

						EditorGUI.indentLevel--;
					}
					break;
				case 3 : 
					// EVENT
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "eventToLaunch" ));
						position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative( "eventToLaunch" ), true) - 18.0f;

						EditorGUI.indentLevel--;
					}
					break;
				case 4 : 
					// CAMERA SHAKE
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "shakeAmplitude" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "shakeSpring" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "shakeRecoil" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "shakeDamping" ));

						EditorGUI.indentLevel--;
					}
					break;
				case 5 : 
					// CAMERA ZOOM
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "zoomTargetFOV" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "zoomReturnTime" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "zoomRamp" ));

						EditorGUI.indentLevel--;
					}
					break;
				case 6 : 
					// RECOIL
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "recoilTransform" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "recoilAmplitude" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "recoilReturnSpeed" ));

						EditorGUI.indentLevel--;
					}
					break;
				case 7 : 
					// CHROMATIC ABBERATIONS
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "abberationsIntensity" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "abberationsTime" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "abberationsRamp" ));

						EditorGUI.indentLevel--;
					}
					break;
				case 8 : 
					// FREEZE
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "freezeTime" ));

						EditorGUI.indentLevel--;
					}
					break;
				case 9 : 
					// TIME SCALE
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "relativeTime" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "fadeInTime" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "stayTime" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "fadeOutTime" ));

						EditorGUI.indentLevel--;
					}
					break;
				case 10 : 
					// LIGHT
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "lightColor" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "lightRange" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "lightRamp" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "lightTime" ));

						EditorGUI.indentLevel--;
					}
					break;
				case 11 : 
					// SCREEN FLASH
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "flashColor" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "flashTime" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "flashRamp" ));

						EditorGUI.indentLevel--;
					}
					break;
				case 12 : 
					// SPAWN VFX
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "vfxPrefab" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "vfxTime" ));

						EditorGUI.indentLevel--;
					}
					break;
				case 13 : 
					// SPAWN PHYSIC
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "physicPrefab" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "physicOffset" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "ejectionDir" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "ejectionRandom" ));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "physicTime" ));

						EditorGUI.indentLevel--;
					}
					break;
				case 14 : 
					// UPDATE GAMEMANAGER
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField( rect,property.FindPropertyRelative( "scoreToAdd" ));

						EditorGUI.indentLevel--;
					}
					break;

				case 15:
					// SFX
					////////////////////////////////////////////////////////////////////////
					{
						EditorGUI.indentLevel++;

						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField(rect, property.FindPropertyRelative("SFXTime"), new GUIContent("Time"));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField(rect, property.FindPropertyRelative("pitchRamp"));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField(rect, property.FindPropertyRelative("cutoffRamp"));
						position.y += 18.0f;
						rect = new Rect(position.x, position.y, position.width, 16f);
						EditorGUI.PropertyField(rect, property.FindPropertyRelative("SFXMixerGroup"));

						EditorGUI.indentLevel--;
					}
					break;

				}
				position.y += 18.0f;
			}
		}
		EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) 
	{
		float count=16f;
		count += (property.FindPropertyRelative(properties[0]).boolValue)?18f*6f + property.FindPropertyRelative( "audioClipCount" ).intValue*18f : 0f;
		count += (property.FindPropertyRelative(properties[1]).boolValue)?18f*2f : 0f;
		count += (property.FindPropertyRelative(properties[2]).boolValue)?18f*2f : 0f;
		count += (property.FindPropertyRelative(properties[3]).boolValue)?18f*1f + 
			EditorGUI.GetPropertyHeight(property.FindPropertyRelative( "eventToLaunch" ), true) : 0f;
		count += (property.FindPropertyRelative(properties[4]).boolValue)?18f*5f : 0f;
		count += (property.FindPropertyRelative(properties[5]).boolValue)?18f*4f : 0f;
		count += (property.FindPropertyRelative(properties[6]).boolValue)?18f*4f : 0f;
		count += (property.FindPropertyRelative(properties[7]).boolValue)?18f*4f : 0f;
		count += (property.FindPropertyRelative(properties[8]).boolValue)?18f*2f : 0f;
		count += (property.FindPropertyRelative(properties[9]).boolValue)?18f*5f : 0f;
		count += (property.FindPropertyRelative(properties[10]).boolValue)?18f*5f : 0f;
		count += (property.FindPropertyRelative(properties[11]).boolValue)?18f*4f : 0f;
		count += (property.FindPropertyRelative(properties[12]).boolValue)?18f*3f : 0f;
		count += (property.FindPropertyRelative(properties[13]).boolValue)?18f*6f : 0f;
		count += (property.FindPropertyRelative(properties[14]).boolValue)?18f*2f : 0f;
		count += (property.FindPropertyRelative(properties[15]).boolValue)?18f*5f : 0f;
		return count;
	}

	public override bool CanCacheInspectorGUI(SerializedProperty property)
	{
		return false;
	}
}