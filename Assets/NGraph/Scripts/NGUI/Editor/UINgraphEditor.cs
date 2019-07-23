/***********************************************************************
/*      Copyright Niugnep Software, LLC 2013 All Rights Reserved.      *
/*                                                                     *
/*     THIS WORK CONTAINS TRADE SECRET AND PROPRIETARY INFORMATION     *
/*     WHICH IS THE PROPERTY OF NIUGNEP SOFTWARE, LLC OR ITS           *
/*             LICENSORS AND IS SUBJECT TO LICENSE TERMS.              *
/**********************************************************************/

#if !UNITY_3_5 && !UNITY_FLASH
#define DYNAMIC_FONT
#endif

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(UINgraph))]
public class UINgraphEditor : NGraphEditor
{
#if DYNAMIC_FONT
   UILabelInspector.FontType mType = UILabelInspector.FontType.Unity;
#else
   UILabelInspector.FontType mType = UILabelInspector.FontType.NGUI;
#endif

   public override void OnInspectorGUI()
   {
      base.OnInspectorGUI();
      NGraphUtils.DrawSeparator();
      
      UINgraph pGraph = (UINgraph)target;
      
      GUILayout.BeginHorizontal();
      if (NGUIEditorTools.DrawPrefixButton("Font"))
      {
         if (mType == UILabelInspector.FontType.NGUI)
         {
            ComponentSelector.Show<UIFont>(OnBitmapFont);
         }
         else
         {
            ComponentSelector.Show<Font>(OnDynamicFont);
         }
      }

#if DYNAMIC_FONT
      GUI.changed = false;
      
      if (mType == UILabelInspector.FontType.Unity)
      {
         Font fnt = (Font)EditorGUILayout.ObjectField(pGraph.AxisLabelDynamicFont, typeof(Font), false, GUILayout.Width(140f));
         if (fnt != pGraph.AxisLabelDynamicFont)
            UndoableAction<UINgraph>( gr => gr.AxisLabelDynamicFont = fnt );
      }
      else
      {
         UIFont fnt = (UIFont)EditorGUILayout.ObjectField(pGraph.AxisLabelBitmapFont, typeof(UIFont), false, GUILayout.Width(140f));
         if (fnt != pGraph.AxisLabelBitmapFont)
            UndoableAction<UINgraph>( gr => gr.AxisLabelBitmapFont = fnt );
      }
      mType = (UILabelInspector.FontType)EditorGUILayout.EnumPopup(mType, GUILayout.Width(62f));
#else
      UIFont fnt = (UIFont)EditorGUILayout.ObjectField(pGraph.AxisLabelBitmapFont, typeof(UIFont), false, GUILayout.Width(140f));
      if (fnt != pGraph.AxisLabelBitmapFont)
         UndoableAction<UINgraph>( gr => gr.AxisLabelBitmapFont = fnt );
      mType = UILabelInspector.FontType.NGUI;
#endif
      
      GUILayout.Label("size", GUILayout.Width(30f));
      EditorGUI.BeginDisabledGroup(mType == UILabelInspector.FontType.NGUI);
      int i = EditorGUILayout.IntField(pGraph.fontSize, GUILayout.Width(30f));
      if (i != pGraph.fontSize)
         UndoableAction<UINgraph>( gr => gr.fontSize = i );
      EditorGUI.EndDisabledGroup();
      GUILayout.Label("font used by the labels");
      GUILayout.EndHorizontal();
   }
   
   void OnBitmapFont (Object obj)
   {
      UINgraph pGraph = (UINgraph)target;
      pGraph.AxisLabelBitmapFont = obj as UIFont;
      pGraph.AxisLabelDynamicFont = null;
   }
   
   void OnDynamicFont (Object obj)
   {
      UINgraph pGraph = (UINgraph)target;
      pGraph.AxisLabelDynamicFont = obj as Font;
      pGraph.AxisLabelBitmapFont = null;
   }
}
