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

public class NGraphEditor : Editor
{
   public override void OnInspectorGUI()
   {
      bool b = false;
      float f = 0.0f;
      NGraph pGraph = (NGraph)target;

      NGraphUtils.DrawSeparator();

      b = EditorGUILayout.Toggle("Redraw On Translate", pGraph.RedrawOnTranslate);
      if (b != pGraph.RedrawOnTranslate)
         UndoableAction<NGraph>(gr => gr.RedrawOnTranslate = b);

      // Colors
      Color c = EditorGUILayout.ColorField("Plot Background Color", pGraph.PlotBackgroundColor);
      if (c != pGraph.PlotBackgroundColor)
         UndoableAction<NGraph>( gr => gr.PlotBackgroundColor = c );
      c = EditorGUILayout.ColorField("Margin Background Color", pGraph.MarginBackgroundColor);
      if (c != pGraph.MarginBackgroundColor)
         UndoableAction<NGraph>( gr => gr.MarginBackgroundColor = c );
      c = EditorGUILayout.ColorField("Axis Line Color", pGraph.AxisColor);
      if (c != pGraph.AxisColor)
         UndoableAction<NGraph>( gr => gr.AxisColor = c );
      c = EditorGUILayout.ColorField("Axis Label Color", pGraph.AxisLabelColor);
      if (c != pGraph.AxisLabelColor)
         UndoableAction<NGraph>( gr => gr.AxisLabelColor = c );

      NGraphUtils.DrawSeparator();

      // Axis
      Vector2 vec2;
      Vector4 vec4;
      int i = 0;
      NGraph.TickStyle tickStyle;
      f = EditorGUILayout.FloatField("Axis Thickness", pGraph.AxesThickness);
      if (f != pGraph.AxesThickness)
         UndoableAction<NGraph>( gr => gr.AxesThickness = f );
      tickStyle = (NGraph.TickStyle)EditorGUILayout.EnumPopup("X Axis Tick Style", pGraph.XTickStyle);
      if (tickStyle != pGraph.XTickStyle)
         UndoableAction<NGraph>( gr => gr.XTickStyle = tickStyle );
      tickStyle = (NGraph.TickStyle)EditorGUILayout.EnumPopup("Y Axis Tick Style", pGraph.YTickStyle);
      if (tickStyle != pGraph.YTickStyle)
         UndoableAction<NGraph>( gr => gr.YTickStyle = tickStyle );
      i = EditorGUILayout.IntField("X Axis Tick Count", pGraph.XNumberOfTicks);
      if (i != pGraph.XNumberOfTicks)
         UndoableAction<NGraph>( gr => gr.XNumberOfTicks = i );
      i = EditorGUILayout.IntField("Y Axis Tick Count", pGraph.YNumberOfTicks);
      if (i != pGraph.YNumberOfTicks)
         UndoableAction<NGraph>( gr => gr.YNumberOfTicks = i );
      
      b = EditorGUILayout.Toggle("Draw X Axis Labels", pGraph.DrawXLabel);
      if (b != pGraph.DrawXLabel)
         UndoableAction<NGraph>( gr => gr.DrawXLabel = b );
      b = EditorGUILayout.Toggle("Draw Y Axis Labels", pGraph.DrawYLabel);
      if (b != pGraph.DrawYLabel)
         UndoableAction<NGraph>( gr => gr.DrawYLabel = b );
      vec2 = EditorGUILayout.Vector2Field("Axis Draw At", pGraph.AxesDrawAt);
      if (vec2 != pGraph.AxesDrawAt)
         UndoableAction<NGraph>( gr => gr.AxesDrawAt = vec2 );
      vec4 = EditorGUILayout.Vector4Field("Margins", pGraph.Margin);
      if (vec4 != pGraph.Margin)
         UndoableAction<NGraph>( gr => gr.Margin = vec4 );

      NGraphUtils.DrawSeparator();

      // Grid
      vec2 = EditorGUILayout.Vector2Field("Major Grid Separation (0 for no grid)", pGraph.GridLinesSeparationMajor);
      if (vec2 != pGraph.GridLinesSeparationMajor)
         UndoableAction<NGraph>( gr => gr.GridLinesSeparationMajor = vec2 );
      c = EditorGUILayout.ColorField("Major Grid Color", pGraph.GridLinesColorMajor);
      if (c != pGraph.GridLinesColorMajor)
         UndoableAction<NGraph>( gr => gr.GridLinesColorMajor = c );
      f = EditorGUILayout.FloatField("Major Grid Thickness", pGraph.GridLinesThicknesMajor);
      if (f != pGraph.GridLinesThicknesMajor)
         UndoableAction<NGraph>( gr => gr.GridLinesThicknesMajor = f );
      /*
      vec2 = EditorGUILayout.Vector2Field("Minor Grid Separation (0 for no grid)", pGraph.GridLinesSeparationMinor);
      if (vec2 != pGraph.GridLinesSeparationMinor)
         UndoableAction<NGraph>( gr => gr.GridLinesSeparationMinor = vec2 );
      c = EditorGUILayout.ColorField("Minor Grid Color", pGraph.GridLinesColorMinor);
      if (c != pGraph.GridLinesColorMinor)
         UndoableAction<NGraph>( gr => gr.GridLinesColorMinor = c );
      f = EditorGUILayout.FloatField("Minor Grid Thickness", pGraph.GridLinesThicknesMinor);
      if (f != pGraph.GridLinesThicknesMinor)
         UndoableAction<NGraph>( gr => gr.GridLinesThicknesMinor = f );
      */
      NGraphUtils.DrawSeparator();

      // Materials
      Material mat = null;
      mat = (Material) EditorGUILayout.ObjectField("Plot Material", pGraph.DefaultPlotMaterial, typeof(Material), false);
      if (mat != pGraph.DefaultPlotMaterial)
         UndoableAction<NGraph>( gr => gr.DefaultPlotMaterial = mat );
      mat = (Material) EditorGUILayout.ObjectField("Plot Background Material", pGraph.PlotBackgroundMaterial, typeof(Material), false);
      if (mat != pGraph.PlotBackgroundMaterial)
         UndoableAction<NGraph>( gr => gr.PlotBackgroundMaterial = mat );
      mat = (Material) EditorGUILayout.ObjectField("Axis Line And Tick Material", pGraph.AxisMaterial, typeof(Material), false);
      if (mat != pGraph.AxisMaterial)
         UndoableAction<NGraph>( gr => gr.AxisMaterial = mat );

   }

   protected void UndoableAction<T>(System.Action<T> action) where T : NGraph
   {
      T pGraph = (T)target;

#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
      Undo.RegisterUndo(pGraph, "Inspector");
#else
      Undo.RecordObject(pGraph, "Inspector");
#endif

      action(pGraph);
      pGraph.RedrawNow = true;
   }
}
