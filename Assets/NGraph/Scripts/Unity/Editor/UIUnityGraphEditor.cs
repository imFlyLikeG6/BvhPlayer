/***********************************************************************
/*      Copyright Niugnep Software, LLC 2013 All Rights Reserved.      *
/*                                                                     *
/*     THIS WORK CONTAINS TRADE SECRET AND PROPRIETARY INFORMATION     *
/*     WHICH IS THE PROPERTY OF NIUGNEP SOFTWARE, LLC OR ITS           *
/*             LICENSORS AND IS SUBJECT TO LICENSE TERMS.              *
/**********************************************************************/

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(UIUnityGraph))]
public class UIUnityGraphEditor : NGraphEditor
{
   public override void OnInspectorGUI()
   {
      base.OnInspectorGUI();
      NGraphUtils.DrawSeparator();
      
      UIUnityGraph pGraph = (UIUnityGraph)target;
      
      GUILayout.BeginHorizontal();
      Font fnt = (Font)EditorGUILayout.ObjectField(pGraph.AxisLabelDynamicFont, typeof(Font), false, GUILayout.Width(140f));
      if (fnt != pGraph.AxisLabelDynamicFont)
         UndoableAction<UIUnityGraph>( gr => gr.AxisLabelDynamicFont = fnt );

      GUILayout.Label("font used by the labels");
      GUILayout.EndHorizontal();
   }
}
