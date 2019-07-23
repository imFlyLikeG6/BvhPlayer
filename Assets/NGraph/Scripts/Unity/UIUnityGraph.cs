/***********************************************************************
/*      Copyright Niugnep Software, LLC 2014 All Rights Reserved.      *
/*                                                                     *
/*     THIS WORK CONTAINS TRADE SECRET AND PROPRIETARY INFORMATION     *
/*     WHICH IS THE PROPERTY OF NIUGNEP SOFTWARE, LLC OR ITS           *
/*             LICENSORS AND IS SUBJECT TO LICENSE TERMS.              *
/**********************************************************************/

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/*! \brief Class used to draw in the native Unity GUI enviornment.
 *
 *  It is best to use the wizard to create this class.
 *  The wizard will add a Game Object with this component
 *  already installed and setup with defaults.
 * 
 */
public class UIUnityGraph : NGraph
{
   protected CanvasRenderer mCanvasRenderer;
   
   public int fontSize = 16;
   public Font AxisLabelDynamicFont;
   
   public delegate void NguiAxisLabelOverrideCallback(NGraph.Axis asix, Text pLabel, float val);
   public NguiAxisLabelOverrideCallback AxisLabelCallback
   {
      get;
      set;
   }
   
   public delegate void NguiDataLabelOverrideCallback(Text pLabel, Vector3 pValue, string text);
   public NguiDataLabelOverrideCallback DataLabelCallback
   {
      get;
      set;
   }
   
   
   public UIUnityGraph()
   {
      UnityGui = true;
   }
   
   public override void Start()
   {
      UnityGui = true;
      base.Start();

      mCanvasRenderer = gameObject.GetComponent<CanvasRenderer>();
      if(!mCanvasRenderer)
         mCanvasRenderer = gameObject.AddComponent<CanvasRenderer>();
   }

   public override void setupGraphHierarchy()
   {
      base.setupGraphHierarchy();
   }

  public override void breakdown(bool destroyAllContainers) {
    base.breakdown(destroyAllContainers);

    if (destroyAllContainers) {
      Destroy(mCanvasRenderer.gameObject);
      mCanvasRenderer = null;
      foreach (CanvasRenderer cr in mXAxisTicks) {
        Destroy(cr.gameObject);
      }
      mXAxisTicks.Clear();
      foreach (CanvasRenderer cr in mYAxisTicks) {
        Destroy(cr.gameObject);
      }
      mYAxisTicks.Clear();
    }
  }

   /* ---------- */
   /* BACKGROUND */
   /* ---------- */

   protected override void _drawPlotBackground(List<UIVertex> pVertexList)
   {
      Material pMat = new Material (PlotBackgroundMaterial);
      pMat.SetColor("_TintColor", PlotBackgroundColor);

      CanvasRenderer pCanvasRenderer = NGraphUtils.AddCanvasRenderer(mPlotBackgroundGo);
      pCanvasRenderer.Clear();
      pCanvasRenderer.SetMaterial(pMat, null);
      pCanvasRenderer.SetVertices(pVertexList);
   }
   
   protected override void _drawAxisBackground(List<UIVertex> pVertexList)
   {
      Material pMat = new Material (AxisMaterial);
      pMat.SetColor("_TintColor", MarginBackgroundColor);

      CanvasRenderer pCanvasRenderer = NGraphUtils.AddCanvasRenderer(mAxesBackgroundGo);
      pCanvasRenderer.Clear();
      pCanvasRenderer.SetMaterial(pMat, null);
      pCanvasRenderer.SetVertices(pVertexList);
   }

   protected override void _drawAxisTick(Axis axis, int index, GameObject pTickGameObject)
   {
      Material pMat = new Material (AxisMaterial);
      pMat.SetColor("_TintColor", AxisColor);
      CanvasRenderer pCanvasRenderer = NGraphUtils.AddCanvasRenderer(pTickGameObject);
      pCanvasRenderer.SetMaterial(pMat, null);

      mXAxisTicks.Add(pCanvasRenderer);
      if(axis == Axis.X)
         NGraphUtils.DrawRect(new Rect(-AxesThickness/2, AxesThickness/2+4, AxesThickness, -AxesThickness-8), pCanvasRenderer);
      else if(axis == Axis.Y)
         NGraphUtils.DrawRect(new Rect((-AxesThickness/2)-4, AxesThickness/2, AxesThickness+8, -AxesThickness), pCanvasRenderer);
   }
   
   protected override void _addedGridContainer(GameObject pGridContainer)
   {
      NGraphUtils.AddCanvasRenderer(pGridContainer);
   }
   protected override void _drawMajorGridLine(Axis axis, int index, float r, GameObject pGridLineGameObject)
   {
      Material pMat = new Material (AxisMaterial);
      pMat.SetColor("_TintColor", GridLinesColorMajor);
      CanvasRenderer pCanvasRenderer = NGraphUtils.AddCanvasRenderer(pGridLineGameObject);
      pCanvasRenderer.Clear();
      pCanvasRenderer.SetMaterial(pMat, null);

      mXAxisTicks.Add(pCanvasRenderer);
      
      if(axis == Axis.X)
         NGraphUtils.DrawRect(new Rect(adjustPointX(r) - (GridLinesThicknesMajor/2f), adjustPointY(YRange.y), GridLinesThicknesMajor, adjustPointY(YRange.x) - adjustPointY(YRange.y)), pCanvasRenderer);
      else if(axis == Axis.Y)
         NGraphUtils.DrawRect(new Rect(adjustPointX(XRange.x), adjustPointY(r) + (GridLinesThicknesMajor/2f), adjustPointX(XRange.y) - adjustPointX(XRange.x), -GridLinesThicknesMajor), pCanvasRenderer);
   }
   
   protected override void _addedAxesGameObject(GameObject pAxisGameObject)
   {
      NGraphUtils.AddCanvasRenderer(pAxisGameObject);
   }
   protected override void _addedXAxisGameObject(GameObject pAxisGameObject)
   {
      NGraphUtils.AddCanvasRenderer(pAxisGameObject);
   }
   protected override void _addedYAxisGameObject(GameObject pAxisGameObject)
   {
      NGraphUtils.AddCanvasRenderer(pAxisGameObject);
   }
   List<CanvasRenderer> mXAxisTicks = new List<CanvasRenderer>();
   List<CanvasRenderer> mYAxisTicks = new List<CanvasRenderer>();
   protected override void DrawAxisTicks()
   {
    if (mMeshesContainer == null) {
      return;
    }

    if (!Application.isPlaying)
         return;
      
      if (this.mAxesGo == null)
         return;
      
      foreach (CanvasRenderer pCanvasRenderer in mXAxisTicks)
      {
         Destroy (pCanvasRenderer.gameObject);
      }
      foreach (CanvasRenderer pCanvasRenderer in mYAxisTicks)
      {
         Destroy (pCanvasRenderer.gameObject);
      }
      
      if (mAxesLabelContainerGo != null)
      {
         for (int i = 0; i < mAxesLabelContainerGo.transform.childCount; i++)
            Destroy (mAxesLabelContainerGo.transform.GetChild (i).gameObject);
      }
      mXAxisTicks.Clear();
      mYAxisTicks.Clear();
      
      base.DrawAxisTicks();
   }

   CanvasRenderer mAxisCanvasRendererX;
   CanvasRenderer mAxisCanvasRendererY;
   protected override void _drawAxes(Rect xAxis, Rect yAxis)
   {
      if(mAxisCanvasRendererX == null)
         mAxisCanvasRendererX = NGraphUtils.AddCanvasRenderer (mXAxesGo);
      if(mAxisCanvasRendererY == null)
         mAxisCanvasRendererY = NGraphUtils.AddCanvasRenderer (mYAxesGo);
      
      Material pMat = new Material (AxisMaterial);
      mAxisCanvasRendererX.SetMaterial(pMat, null);
      pMat.SetColor("_TintColor", AxisColor);

      pMat = new Material (AxisMaterial);
      mAxisCanvasRendererY.SetMaterial(pMat, null);
      pMat.SetColor("_TintColor", AxisColor);

      NGraphUtils.DrawRect(xAxis, mAxisCanvasRendererX);
      NGraphUtils.DrawRect(yAxis, mAxisCanvasRendererY);
   }
   
   protected override void addedAxesLabelContainer()
   {
      NGraphUtils.AddCanvasRenderer(mAxesLabelContainerGo);
   }
   
   protected override void newDataSeriesGameObject(GameObject pGameObject)
   {
      NGraphUtils.AddCanvasRenderer(pGameObject);
   }

   protected override void AddAxisLabel(NGraph.Axis axis, GameObject pLabelGameObject, Vector3 pPosition, float val)
   {
      Text pLabel = pLabelGameObject.AddComponent<Text>();
      RectTransform pRectTransform = pLabel.GetComponent<RectTransform>();

      pLabel.font = AxisLabelDynamicFont;
      pLabel.fontSize = fontSize;
      pLabel.color = AxisLabelColor;
      
      if (axis == NGraph.Axis.X)
      {
         pPosition.y -= 10;
         pLabel.alignment = TextAnchor.MiddleCenter;
      } else if (axis == NGraph.Axis.Y)
      {
         pRectTransform.sizeDelta = new Vector2(100, 100);
         pPosition.x -= 55;
         pLabel.alignment = TextAnchor.MiddleRight;
      }
      pLabel.text = val.ToString();

      pLabelGameObject.transform.localPosition = pPosition;
      
      if(AxisLabelCallback != null)
         AxisLabelCallback(axis, pLabel, val);
   }

   protected override void AddDataSeriesLabel(GameObject pLabelGameObject, Vector3 pPosition, Vector3 pValue, string val)
   {
      Text pLabel = pLabelGameObject.GetComponent<Text>();

      if(pLabel == null)
      {
         pLabel = pLabelGameObject.AddComponent<Text>();
         pLabel.font = AxisLabelDynamicFont;
         pLabel.fontSize = fontSize;
         pLabel.color = AxisLabelColor;
      }

      pLabel.text = val.ToString();
      pLabelGameObject.transform.localPosition = pPosition;
      
      if(DataLabelCallback != null)
         DataLabelCallback(pLabel, pValue, val);
   }
   
#if UNITY_EDITOR
   
   /// <summary>
   /// Draw a visible pink outline for the clipped area.
   /// </summary>
   
   void OnDrawGizmos ()
   {
      GameObject go = UnityEditor.Selection.activeGameObject;
      bool selected = (go != null) && (go.GetComponent<NGraph>() == this);
     
      Transform t = transform;
     
      if(t != null)
      {
         Gizmos.matrix = t.localToWorldMatrix;
     
         if(selected)
            Gizmos.color = new Color(0f, 0.5f, 0.5f);
         else
            Gizmos.color = new Color(0.2f, 0.0f, 0.3f);

         Gizmos.DrawWireCube(mRectTransform.rect.center, mRectTransform.rect.size);
      }
   }
#endif
}
