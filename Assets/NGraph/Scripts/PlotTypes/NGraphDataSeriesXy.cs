/***********************************************************************
/*      Copyright Niugnep Software, LLC 2013 All Rights Reserved.      *
/*                                                                     *
/*     THIS WORK CONTAINS TRADE SECRET AND PROPRIETARY INFORMATION     *
/*     WHICH IS THE PROPERTY OF NIUGNEP SOFTWARE, LLC OR ITS           *
/*             LICENSORS AND IS SUBJECT TO LICENSE TERMS.              *
/**********************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*! \brief Line and Bar plot types.
 *
 *  Use NGraph.addDataSeries<NGraphDataSeriesXy>(...) to add this plot type
 *  to a graph.
 */
public class NGraphDataSeriesXy : NGraphDataSeries
{
   protected List<Vector2> mData = new List<Vector2>();
   protected float mPlotThickness = 2;
   protected MarkerStyle mMarerkStyle = MarkerStyle.None;
   protected Style mPlotStyle = Style.Line;
   protected float mMarkerWeight = 10;
   protected List<KeyValuePair<GameObject, Mesh>> mMeshes;
   protected List<KeyValuePair<GameObject, CanvasRenderer>> mCanvasRenderers;
   protected Color mMarkerColor;
   protected List<dataLabelStruct> mDataLabels = new List<dataLabelStruct>();

   protected struct dataLabelStruct
   {
      public float xValue;
      public GameObject gameObj;
      public string text;
   }

   /** The plot's marker color. 
     *  If markers are set this color is used to draw them.  It's default is the plot color
     *  but can be changed here.
     */
   public Color MarkerColor
   {
      get
      {
         return mMarkerColor;
      }
      set
      {
         mMarkerColor = value;
         DrawSeries();
      }
   }
   
   public enum Style
   {
      Line,
      Bar
   }
   
   public enum MarkerStyle
   {
      None,
      Box,
      Triangle
   }
   
   /** \brief  The plot's data. 
     * 
     *  This is the data that will be used to draw the graph.
     *  Any values outside of the parent graph's range will be clipped.
     */
   public List<Vector2> Data
   {
      get { return mData; }
      set
      {
         mData = value;
         DrawSeries();
      }
   }
   
   /** \brief  The plot's thickness. 
     * 
     *  This applies to both line and bar types.
     *  Bar graphs will need a larger value to look good.
     */
   public float PlotThickness
   {
      get { return mPlotThickness; }
      set 
      {
         if(mPlotThickness == value)
            return;
         
         mPlotThickness = Mathf.Abs(value);
         DrawSeries();
      }
   }
   
   /** \brief  The plot's marker style.
     * 
     *  A marker will be drawn in the selected style at every data point.
     */
   public MarkerStyle MarkersStyle
   {
      get { return mMarerkStyle; }
      set
      {
         if(mMarerkStyle == value)
            return;
         
         mMarerkStyle = value;
         DrawSeries();
      }
   }
   
   /** \brief The plot's style.
     * 
     *  Change between bar and line graphs here.
     */
   public Style PlotStyle
   {
      get { return mPlotStyle; }
      set
      {
         if(mPlotStyle == value)
            return;
         
         mPlotStyle = value;
         clearMeshes(0);
         clearCanvasRenderers(0);
         clearMarkers();
         DrawSeries();
      }
   }
   
   public float MarkerWeight
   {
      get { return mMarkerWeight; }
      set
      {
         if(mMarkerWeight == value)
            return;
         
         mMarkerWeight = value;
         DrawSeries();
      }
   }

   public float Reveal
   {
      set
      {
         finalizedReveal = false;
         mElapsedRevealTime = 0.0f;
         mRevealTime = value;
      }
   }
   private float mRevealTime = 0.0f;
   private float mElapsedRevealTime = 0.0f;
   private bool finalizedReveal = false;
   public override void Update()
   {
      if(mGraph == null)
         return;

      mElapsedRevealTime += Time.unscaledDeltaTime;
      if (mElapsedRevealTime >= mRevealTime)
      {
         if(finalizedReveal)
            return;
         finalizedReveal = true;
      }

      float percent = mElapsedRevealTime / mRevealTime;
      if(percent > 1)
         percent = 1;

      if (float.IsNaN(percent)) {
         return;
      }
      
      float b = (mClipping.w - mClipping.z) * percent;
      float a = (mClipping.y - mClipping.x) * percent;
      if(!mGraph.UnityGui)
      {
         if(PlotStyle == Style.Bar)
            mMeshRenderer.material.SetVector("_Clipping", new Vector4(mClipping.x, mClipping.y, mClipping.z, b + mClipping.z));
         else if(PlotStyle == Style.Line)
            mMeshRenderer.material.SetVector("_Clipping", new Vector4(mClipping.x, a + mClipping.x, mClipping.z, mClipping.w));
      }
      else
      {
         applyClipping(a, b);
      }

      // Apply clipping to Markers
      foreach(GameObject pMakerGo in mMarkerGos)
      {
         if(!mGraph.UnityGui)
         {
            if(PlotStyle == Style.Bar)
               pMakerGo.GetComponent<MeshFilter>().GetComponent<Renderer>().material.SetVector("_Clipping", new Vector4(mClipping.x, mClipping.y, mClipping.z, b + mClipping.z));
            else if(PlotStyle == Style.Line)
               pMakerGo.GetComponent<MeshFilter>().GetComponent<Renderer>().material.SetVector("_Clipping", new Vector4(mClipping.x, a + mClipping.x, mClipping.z, mClipping.w));
         }
         else
         {
            CanvasRenderer pCanvasRenderer = pMakerGo.GetComponent<CanvasRenderer>();
            Material pMaterial = new Material(PlotMaterial);
            pMaterial.SetColor("_TintColor", PlotColor);
            if(PlotStyle == Style.Bar)
               pMaterial.SetVector("_Clipping", new Vector4(mClipping.x, mClipping.y, mClipping.z, b + mClipping.z));
            else if(PlotStyle == Style.Line)
               pMaterial.SetVector("_Clipping", new Vector4 (mClipping.x, a + mClipping.x, mClipping.z, mClipping.w));
            pCanvasRenderer.SetMaterial(pMaterial, null);
         }
      }
   }

   protected void applyClipping(float a, float b)
   {
      foreach(KeyValuePair<GameObject, CanvasRenderer> pPair in mCanvasRenderers)
      {
         Material pMaterial = new Material(PlotMaterial);
         pMaterial.SetColor("_TintColor", PlotColor);
         
         if(PlotStyle == Style.Bar)
            pMaterial.SetVector("_Clipping", new Vector4(mClipping.x, mClipping.y, mClipping.z, b + mClipping.z));
         else if(PlotStyle == Style.Line)
            pMaterial.SetVector("_Clipping", new Vector4 (mClipping.x, a + mClipping.x, mClipping.z, mClipping.w));
         pPair.Value.SetMaterial(pMaterial, null);
      }
   }
   
   public override List<GameObject> setup(NGraph pGraph, GameObject pGameObject, NGraph.DataSeriesDataLabelCallback pDataLabelCallback, GameObject pDataLabelContainer)
   {
      List<GameObject> pChildGos = base.setup(pGraph, pGameObject, pDataLabelCallback, pDataLabelContainer);
      mMarkerColor = PlotColor;

      return pChildGos;
   }
   
   public override void teardown(NGraph pGraph, GameObject pGameObject)
   {
      base.teardown(pGraph, pGameObject);
      clearMeshes(0);
      clearCanvasRenderers(0);
   }
   private void clearMeshes(int newCount)
   {
      if(mMeshes == null)
         return;
      
      if(mMeshes.Count == newCount)
         return;
      
      int i = 0;
      foreach(KeyValuePair<GameObject, Mesh> pPair in mMeshes)
      {
         if(++i >= newCount)
         {
            // Destroy both the Mesh object and game object - if we don't destroy the mesh object it will leak in the VOB
            Destroy(pPair.Value);
            Destroy(pPair.Key);
         }
      }
      
      // Remove pairs that have been destroyed
      if(newCount == 0)
         mMeshes.Clear();
      else if(newCount < mMeshes.Count - 1)
         mMeshes.RemoveRange(0, newCount-1);
   }
   private void clearCanvasRenderers(int newCount)
   {
      if(!Application.isPlaying)
         return;

      if(mCanvasRenderers == null)
         return;
      
      if(mCanvasRenderers.Count == newCount)
         return;
      
      int i = 0;
      foreach(KeyValuePair<GameObject, CanvasRenderer> pPair in mCanvasRenderers)
      {
         if(++i >= newCount)
         {
            Destroy(pPair.Key);
         }
      }
      
      // Remove pairs that have been destroyed
      if(newCount == 0)
         mCanvasRenderers.Clear();
      else if(newCount < mCanvasRenderers.Count - 1)
         mCanvasRenderers.RemoveRange(0, newCount-1);
   }

   public int addDataLabel(float xValue, string label = null)
   {
      GameObject pLabelGo = NGraphUtils.AddGameObject(mDataLabelContainerGo, 0, "Plot Label - " + mDataLabels.Count);
      dataLabelStruct str;
      str.xValue = xValue;
      str.gameObj = pLabelGo;
      str.text = label;

      mDataLabels.Add(str);
      int pos = mDataLabels.Count - 1;
      drawDataLabel(mDataLabels[pos]);
      return pos;
   }

   public bool removeDataLabel(int index)
   {
      if(index >= mDataLabels.Count)
         return false;

      Destroy(mDataLabels[index].gameObj);
      mDataLabels.RemoveAt(index);
      return true;
   }
   
   public override void DrawSeries()
   {
      if(mGameObject == null || mData == null)
         return;
      
      base.DrawSeries();
      clearMarkers();
      int meshCount = mData.Count;
      if(mPlotStyle == Style.Line)
         meshCount = 1;
      clearMeshes(meshCount);
      clearCanvasRenderers(meshCount);
      
      List<Vector3> pVertices  = new List<Vector3>();
      List<Vector2> pUvs  = new List<Vector2>();
      List<int> pTriangles  = new List<int>();

      Mesh pMesh;
      CanvasRenderer pCanvasRenderer;
      if (!mGraph.UnityGui && mMeshes == null)
         mMeshes = new List<KeyValuePair<GameObject, Mesh>>();
      else if (mGraph.UnityGui && mCanvasRenderers == null)
         mCanvasRenderers = new List<KeyValuePair<GameObject, CanvasRenderer>>();

      Vector2 pZero = mGraph.adjustPoint(Vector2.zero);
      
      for(int i = 0; i < mData.Count; ++i)
      {
         Vector2 pDataPoint = mData[i];
         pDataPoint = mGraph.adjustPoint(pDataPoint);
         
         if(MarkersStyle != MarkerStyle.None)
            drawMarker(pDataPoint, i);
         
         if(mPlotStyle == Style.Line && i == 0)
            continue;
         
         if(mPlotStyle == Style.Line)
         {
            Vector2 pPrevDataPoint = mData[i-1];
            NGraphUtils.addSegment(mGraph.adjustPoint(pPrevDataPoint), pDataPoint, PlotThickness, pVertices, pUvs, pTriangles, mGraph.UnityGui);
         }
         else if(mPlotStyle == Style.Bar)
         {
            if(mGraph.UnityGui)
            {
               if(i >= mCanvasRenderers.Count)
               {
                  GameObject pChildBarGo = NGraphUtils.AddGameObject(mGameObject, 0, "Bar - " + i);
                  pCanvasRenderer = NGraphUtils.AddCanvasRenderer(pChildBarGo);
                  mCanvasRenderers.Add(new KeyValuePair<GameObject, CanvasRenderer>(pChildBarGo, pCanvasRenderer));
               }
               else
               {
                  pCanvasRenderer = mCanvasRenderers[i].Value;
               }
               
               Material pMat = new Material (PlotMaterial);
               pMat.SetColor("_TintColor", PlotColor);
               pMat.SetVector("_Clipping", new Vector4(mClipping.x, mClipping.y, mClipping.z, mClipping.w));
               
               pCanvasRenderer.SetMaterial(pMat, null);

               float h = pDataPoint.y - pZero.y;
               float top = pZero.y;
               NGraphUtils.DrawRect(new Rect(-mPlotThickness/2 + pDataPoint.x, top, mPlotThickness, h), pCanvasRenderer);
            }
            else
            {
               if(i >= mMeshes.Count)
               {
                  GameObject pChildBarGo = NGraphUtils.AddGameObject(mGameObject, 0, "Bar - " + i);
                  NGraphUtils.AddMesh(pChildBarGo, out mMeshRenderer, out pMesh);
                  mMeshes.Add(new KeyValuePair<GameObject, Mesh>(pChildBarGo, pMesh));
                  mMeshRenderer.material = new Material(PlotMaterial);
                  mMeshRenderer.material.SetColor("_TintColor", PlotColor);
                  mMeshRenderer.material.SetVector("_Clipping", new Vector4(mClipping.x, mClipping.y, mClipping.z, mClipping.w));
               }
               else
               {
                  pMesh = mMeshes[i].Value;
               }
               
               pMesh.Clear();
               NGraphUtils.DrawRect(new Rect(-mPlotThickness/2 + pDataPoint.x, pZero.y, mPlotThickness, pDataPoint.y - pZero.y), pMesh);
            }
         }
      }
      
      if(mPlotStyle == Style.Line)
      {
         if(mGraph.UnityGui)
         {
            if(mCanvasRenderers.Count == 0)
            {
               GameObject pChildBarGo = NGraphUtils.AddGameObject(mGameObject, 0, "Line");
               pCanvasRenderer = NGraphUtils.AddCanvasRenderer(pChildBarGo);
               mCanvasRenderers.Add(new KeyValuePair<GameObject, CanvasRenderer>(pChildBarGo, pCanvasRenderer));
            }
            else
            {
               pCanvasRenderer = mCanvasRenderers[0].Value;
            }

            Vector3[] e = new Vector3[4];
            mGraph.GetComponent<RectTransform>().GetWorldCorners(e);

            Material pMat = new Material (PlotMaterial);
            pMat.SetColor("_TintColor", PlotColor);
            pMat.SetVector("_Clipping", new Vector4(mClipping.x, mClipping.y, mClipping.z, mClipping.w));
            
            pCanvasRenderer.SetMaterial(pMat, null);
            List<UIVertex> vertices = new List<UIVertex>(pVertices.Count);
            for(int i = 0; i < pVertices.Count; i++)
            {
               UIVertex pVertex = new UIVertex();
               pVertex.position = pVertices[i];
               pVertex.uv0 = pUvs[i];
               vertices.Add(pVertex);
            }
            pCanvasRenderer.SetVertices(vertices);
         }
         else
         {
            if(mMeshes.Count == 0)
            {
               GameObject pChildBarGo = NGraphUtils.AddGameObject(mGameObject, 0, "Line");
               NGraphUtils.AddMesh(pChildBarGo, out mMeshRenderer, out pMesh);
               mMeshes.Add(new KeyValuePair<GameObject, Mesh>(pChildBarGo, pMesh));
               mMeshRenderer.material = new Material(PlotMaterial);
               mMeshRenderer.material.SetColor("_TintColor", PlotColor);
               mMeshRenderer.material.SetVector("_Clipping", new Vector4(mClipping.x, mClipping.y, mClipping.z, mClipping.w));
            }
            else
            {
               pMesh = mMeshes[0].Value;
            }
            
            pMesh.Clear();
            pMesh.vertices = pVertices.ToArray();
            pMesh.uv = pUvs.ToArray();
            pMesh.triangles = pTriangles.ToArray();
         }
      }

      // Draw data labels if available
      foreach(dataLabelStruct labelInfo in mDataLabels)
      {
         drawDataLabel(labelInfo);
      }

   }

   protected bool drawDataLabel(dataLabelStruct labelInfo)
   {
      if(mData == null || mData.Count < 1)
         return false;

      float xValue = labelInfo.xValue;
      Vector2 p0 = Vector2.zero;
      Vector2 p1 = Vector2.zero;
      bool dataPointFound = false;
      for(int index = 0; index < mData.Count; index++)
      {
         Vector2 pDataPoint = mData[index];
         p1 = pDataPoint;
         if(index == 0)
            p0 = p1;
         if(pDataPoint.x > xValue)
            break;
         dataPointFound = true;
         p0 = pDataPoint;
      }

      if(!dataPointFound)
         return false;

      float yValue = p0.y + (p1.y - p0.y) + ((xValue - p0.x)/(p1.x - p0.x));

      mDataLabelCallback(labelInfo.gameObj, new Vector3(xValue, yValue, 0), labelInfo.text);

      return true;
   }
   
   protected List<GameObject> mMarkerGos;
   protected void clearMarkers()
   {
      if(mMarkerGos == null)
      {
         mMarkerGos = new List<GameObject>(mData.Count);
         return;
      }

      foreach(GameObject pMakerGo in mMarkerGos)
      {
         if(!mGraph.UnityGui)
            GameObject.Destroy(pMakerGo.GetComponent<MeshFilter>().mesh);
         GameObject.Destroy(pMakerGo);
      }
      
      mMarkerGos = new List<GameObject>(mData.Count);
   }

   protected MeshRenderer mMeshRenderer;
   protected void drawMarker(Vector2 pDataPoint, int dataPointIndex)
   {
      Mesh pMesh = null;
      GameObject pMarkerGo = NGraphUtils.AddGameObject(mGameObject, NGraph.LEVEL_STEP, "Marker - " + dataPointIndex);
      CanvasRenderer pCanvasRenderer = null;
      if(mGraph.UnityGui)
      {
         pCanvasRenderer = NGraphUtils.AddCanvasRenderer(pMarkerGo);
         Material pMat = new Material (PlotMaterial);
         pMat.SetColor("_TintColor", mMarkerColor);
         pMat.SetVector("_Clipping", new Vector4(mClipping.x, mClipping.y, mClipping.z, mClipping.w));
         
         pCanvasRenderer.SetMaterial(pMat, null);
      }
      else
      {
         NGraphUtils.AddMesh (pMarkerGo, out mMeshRenderer, out pMesh);
         mMeshRenderer.material = PlotMaterial;
         mMeshRenderer.material.SetColor ("_TintColor", mMarkerColor);
         mMeshRenderer.material.SetVector ("_Clipping", new Vector4 (mClipping.x, mClipping.y, mClipping.z, mClipping.w));
      }
      mMarkerGos.Add(pMarkerGo);
      
      List<Vector3> pVertices  = new List<Vector3>();
      List<Vector2> pUvs  = new List<Vector2>();
      List<int> pTriangles  = new List<int>();
      switch(MarkersStyle)
      {
         case MarkerStyle.Box:
         {
            /*
             * c      d
             * 
             * 
             * b      a
             */
            
            pVertices.Add(new Vector3(mMarkerWeight/2 + pDataPoint.x, -mMarkerWeight/2 + pDataPoint.y, 0));
            pVertices.Add(new Vector3(-mMarkerWeight/2 + pDataPoint.x, -mMarkerWeight/2 + pDataPoint.y, 0));
            pVertices.Add(new Vector3(-mMarkerWeight/2 + pDataPoint.x, mMarkerWeight/2 + pDataPoint.y, 0));
            pVertices.Add(new Vector3(mMarkerWeight/2 + pDataPoint.x, mMarkerWeight/2 + pDataPoint.y, 0));
            pUvs.Add(new Vector2(0, 0));
            pUvs.Add(new Vector2(1, 0));
            pUvs.Add(new Vector2(0, 1));
            pUvs.Add(new Vector2(1, 1));
         
            pTriangles.Add(0);
            pTriangles.Add(1);
            pTriangles.Add(2);

            pTriangles.Add(2);
            pTriangles.Add(3);
            pTriangles.Add(0);
            
            break;
         }
         case MarkerStyle.Triangle:
         {
            /*
             *    c
             * 
             * 
             * a      b
             */
         
            pVertices.Add(new Vector3(pDataPoint.x, pDataPoint.y + mMarkerWeight/2, 0));
            pVertices.Add(new Vector3(mMarkerWeight/2 + pDataPoint.x, -mMarkerWeight/2 + pDataPoint.y, 0));
            pVertices.Add(new Vector3(-mMarkerWeight/2 + pDataPoint.x, -mMarkerWeight/2 + pDataPoint.y, 0));
            pUvs.Add(new Vector2(0, 0));
            pUvs.Add(new Vector2(1, 0));
            pUvs.Add(new Vector2(0.5f, 1));
            if(mGraph.UnityGui)
            {
               pVertices.Add(new Vector3(-mMarkerWeight/2 + pDataPoint.x, -mMarkerWeight/2 + pDataPoint.y, 0));
               pUvs.Add(new Vector2(0.5f, 1));
            }

            pTriangles.Add(0);
            pTriangles.Add(1);
            pTriangles.Add(2);
            
            break;
         }
      }

      if(mGraph.UnityGui)
      {
         List<UIVertex> vertices = new List<UIVertex>(pVertices.Count);
         for(int i = 0; i < pVertices.Count; i++)
         {
            UIVertex pVertex = new UIVertex();
            pVertex.position = pVertices[i];
            pVertex.uv0 = pUvs[i];
            vertices.Add(pVertex);
         }
         pCanvasRenderer.SetVertices(vertices);
      }
      else
      {
         pMesh.vertices = pVertices.ToArray ();
         pMesh.uv = pUvs.ToArray ();
         pMesh.triangles = pTriangles.ToArray ();
      }
   }
}
