/***********************************************************************
/*      Copyright Niugnep Software, LLC 2014 All Rights Reserved.      *
/*                                                                     *
/*     THIS WORK CONTAINS TRADE SECRET AND PROPRIETARY INFORMATION     *
/*     WHICH IS THE PROPERTY OF NIUGNEP SOFTWARE, LLC OR ITS           *
/*             LICENSORS AND IS SUBJECT TO LICENSE TERMS.              *
/**********************************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;

public class NGraphCreateUnityGraphWizard : NGraphCreateGraphWizard
{
   Font mTrueTypeFont = null;
   
   // Add menu named "My Window" to the Window menu
   [MenuItem ("Window/Graph Master/New Native Unity Graph")]
   public static new void Init ()
   {
      NGraphCreateGraphWizard.Init();
      
      // Get existing open window or if none, make a new one:
      NGraphCreateUnityGraphWizard window = (NGraphCreateUnityGraphWizard)EditorWindow.GetWindow(typeof(NGraphCreateUnityGraphWizard));
      window.Show();
   }

   void OnDynamicFont (Object obj)
   {
      mTrueTypeFont = obj as Font;
   }
   
   public override void OnGUI ()
   {
      base.OnGUI();
      
      GUILayout.BeginHorizontal();

      mTrueTypeFont = (Font)EditorGUILayout.ObjectField(mTrueTypeFont, typeof(Font), false, GUILayout.Width(140f));

      GUILayout.Label("font used by the labels");
      GUILayout.EndHorizontal();
      NGraphUtils.DrawSeparator();

      GameObject go = NGraphUtils.SelectedRoot<Canvas>();
      
      if(ShouldCreate(go, go != null && mTrueTypeFont != null))
      {
         UIUnityGraph pGraph = CreateGraphGo<UIUnityGraph>(go);
         pGraph.AxisLabelDynamicFont = mTrueTypeFont;
      }
   }
}
