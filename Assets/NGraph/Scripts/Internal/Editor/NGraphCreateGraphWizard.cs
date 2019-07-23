/***********************************************************************
/*      Copyright Niugnep Software, LLC 2013 All Rights Reserved.      *
/*                                                                     *
/*     THIS WORK CONTAINS TRADE SECRET AND PROPRIETARY INFORMATION     *
/*     WHICH IS THE PROPERTY OF NIUGNEP SOFTWARE, LLC OR ITS           *
/*             LICENSORS AND IS SUBJECT TO LICENSE TERMS.              *
/**********************************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class NGraphCreateGraphWizard : EditorWindow
{
   //static Material sGeneralMaterial;
   //static Material sDefaultPlotMaterial;
   static Color sPlotBackgroundColor = new Color(0, 0, 0, 0.3f);
   static Color sLabelColor = Color.white;
   static Color sMarginColor = new Color(0, 0.1f, 1, 0.3f);
   static Color sAxesColor = Color.white;
   static float sWidth = 800;
   static float sHeight = 500;
   
   public static void Init () {
      //sGeneralMaterial = Resources.Load("Materials/NGraphMat", typeof(Material)) as Material;
      //sDefaultPlotMaterial = Resources.Load("Materials/NGraphPlotAlphaClip", typeof(Material)) as Material;
   }

   public virtual void OnGUI ()
   {
      /*
      GUILayout.BeginHorizontal();
      sGeneralMaterial = EditorGUILayout.ObjectField(sGeneralMaterial, typeof(Material), GUILayout.MinWidth(150f)) as Material;
      GUILayout.Label("Material used by NGraph to draw meshes", GUILayout.MinWidth(10000f));
      GUILayout.EndHorizontal();
      
      GUILayout.BeginHorizontal();
      sDefaultPlotMaterial = EditorGUILayout.ObjectField(sDefaultPlotMaterial, typeof(Material), GUILayout.MinWidth(150f)) as Material;
      GUILayout.Label("Default material used by NGraph to draw plots", GUILayout.MinWidth(10000f));
      GUILayout.EndHorizontal();
      */
      EditorGUILayout.FloatField("Width", sWidth);
      EditorGUILayout.FloatField("Height", sHeight);

      sLabelColor =  EditorGUILayout.ColorField("Color of any labels", sLabelColor);
      sMarginColor =  EditorGUILayout.ColorField("Color of the margin area", sMarginColor);
      sPlotBackgroundColor =  EditorGUILayout.ColorField("Color of plot area", sPlotBackgroundColor);
      sAxesColor =  EditorGUILayout.ColorField("Color of X and Y Axes", sAxesColor);
      NGraphUtils.DrawSeparator();
   }
   
   static public T CreateGraphGo<T>(GameObject pParent) where T : NGraph
   {
      GameObject pGraphGo = new GameObject("Graph");
      pGraphGo.layer = pParent.layer;
      Transform pTransform = pGraphGo.transform;
      pTransform.SetParent(pParent.transform, false);
      pTransform.localPosition = Vector3.zero;
      pTransform.localScale = Vector3.one;
      
      T pGraph = pGraphGo.AddComponent<T>();
      SetupGraph(pGraph);
      return pGraph;
   }
   
   static public void SetupGraph(NGraph pGraph)
   {
      pGraph.PlotBackgroundColor = sPlotBackgroundColor;
      pGraph.MarginBackgroundColor = sMarginColor;
      pGraph.AxisColor = sAxesColor;
      pGraph.GridLinesColorMajor = Color.white;
      pGraph.GridLinesColorMinor = Color.white;
      pGraph.GetComponent<RectTransform>().sizeDelta = new Vector2(sWidth, sHeight);
   }
   
   static public bool ShouldCreate(GameObject go, bool isValid)
   {
      GUI.color = isValid ? Color.green : Color.grey;
      
      GUILayout.BeginHorizontal();
      bool retVal = GUILayout.Button("Add To", GUILayout.Width(76f));
      GUI.color = Color.white;
      GameObject sel = EditorGUILayout.ObjectField(go, typeof(GameObject), true, GUILayout.Width(140f)) as GameObject;
      GUILayout.Label("Select the parent in the Hierarchy View", GUILayout.MinWidth(10000f));
      GUILayout.EndHorizontal();
      
      if (sel != go) Selection.activeGameObject = sel;
      
      if(go == null || go.GetComponent<NGraph>())
         return false;
      
      if (retVal && isValid)
      {
         NGraphUtils.RegisterUndo("Add Graph");
         return true;
      }
      return false;
   }
}
