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

using UnityEngine;
using UnityEditor;
using System.Collections;

public class NGraphCreateNGUIGraphWizard : NGraphCreateGraphWizard
{
#if DYNAMIC_FONT
   UILabelInspector.FontType mType = UILabelInspector.FontType.Unity;
#else
   UILabelInspector.FontType mType = UILabelInspector.FontType.NGUI;
#endif
   Font mTrueTypeFont = null;
   UIFont mBitmapFont = null;
   
   // Add menu named "My Window" to the Window menu
   [MenuItem ("Window/NGraph/New NGUI Graph")]
   public static new void Init ()
   {
      NGraphCreateGraphWizard.Init();
      
      // Get existing open window or if none, make a new one:
      NGraphCreateNGUIGraphWizard window = (NGraphCreateNGUIGraphWizard)EditorWindow.GetWindow(typeof(NGraphCreateNGUIGraphWizard));
      window.Show();
   }
   
   void OnBitmapFont (Object obj)
   {
      mBitmapFont = obj as UIFont;
      mTrueTypeFont = null;
   }
   
   void OnDynamicFont (Object obj)
   {
      mTrueTypeFont = obj as Font;
      mBitmapFont = null;
   }
   
   public override void OnGUI ()
   {
      base.OnGUI();
      
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
         mTrueTypeFont = (Font)EditorGUILayout.ObjectField(mTrueTypeFont, typeof(Font), false, GUILayout.Width(140f));
         if (GUI.changed) mBitmapFont = null;
      }
      else
      {
         mBitmapFont = (UIFont)EditorGUILayout.ObjectField(mBitmapFont, typeof(UIFont), false, GUILayout.Width(140f));
         if (GUI.changed) mTrueTypeFont = null;
      }
      mType = (UILabelInspector.FontType)EditorGUILayout.EnumPopup(mType, GUILayout.Width(62f));
#else
      NGUISettings.bitmapFont = (UIFont)EditorGUILayout.ObjectField(NGUISettings.bitmapFont, typeof(UIFont), false, GUILayout.Width(140f));
      mType = UILabelInspector.FontType.NGUI;
#endif
      
      GUILayout.Label("size", GUILayout.Width(30f));
      EditorGUI.BeginDisabledGroup(mType == UILabelInspector.FontType.NGUI);
      NGUISettings.fontSize = EditorGUILayout.IntField(NGUISettings.fontSize, GUILayout.Width(30f));
      EditorGUI.EndDisabledGroup();
      GUILayout.Label("font used by the labels");
      GUILayout.EndHorizontal();
      NGUIEditorTools.DrawSeparator();
      
      GameObject go = NGUIEditorTools.SelectedRoot();
      
      if(ShouldCreate(go, go != null && (mBitmapFont != null || mTrueTypeFont != null)))
      {
         UINgraph pUINgraph = CreateGraphGo<UINgraph>(go);
         pUINgraph.fontSize = NGUISettings.fontSize;
         if(mType == UILabelInspector.FontType.NGUI)
            pUINgraph.AxisLabelBitmapFont = mBitmapFont;
         else
            pUINgraph.AxisLabelDynamicFont = mTrueTypeFont;
      }
   }
}
