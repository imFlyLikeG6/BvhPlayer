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

[ExecuteInEditMode]
/*! \brief Base NGraph class.
 *
 *  This class is used by the different GUI endpoints.  Most of the 
 *  loggic for setting up and drawing a graph is handled here.
 */
public abstract class NGraph : MonoBehaviour
{
   public const float LEVEL_STEP = -0.1f;
   public const int BACKGROUND_LEVEL = 0;
   public const int AXES_LEVEL = 1;
   public const int PLOT_START_LEVEL = 4;

   public bool RedrawOnTranslate = true;
   public bool RedrawNow = true;
   public Vector4 Margin = new Vector4(60, 40, 60, 40);
   public Material DefaultPlotMaterial;
   public float AxesThickness = 4.0f;
   public Material AxisMaterial;
   public Vector2 AxesDrawAt = Vector2.zero;
   public Color AxisColor = Color.white;
   public Color AxisLabelColor = Color.white;
   public Color MarginBackgroundColor = new Color(1, 1, 1, 0.4f);
   public bool DrawXLabel = true;
   public bool DrawYLabel = true;
   public TickStyle XTickStyle = TickStyle.EvenSpaceLowAndHigh;
   public int XNumberOfTicks = 5;
   public TickStyle YTickStyle = TickStyle.EvenSpaceLowAndHigh;
   public int YNumberOfTicks = 5;
   public Material PlotBackgroundMaterial;
   public Color PlotBackgroundColor;

   protected Vector2 mXRange;
   protected Vector2 mYRange;
   protected GameObject mMeshesContainer;
   protected GameObject mDataLabelTopContainer;
   protected RectTransform mRectTransform;

   public delegate void DataSeriesDataLabelCallback(GameObject pLabelGo, Vector3 pPosition, string text);
   protected abstract void AddAxisLabel(NGraph.Axis axis, GameObject pLabelGameObject, Vector3 pPosition, float val);
   protected abstract void AddDataSeriesLabel(GameObject pLabelGameObject, Vector3 pPosition, Vector3 pValue, string val);

   protected GameObject DataLabelTopContainer
   {
      get
      {
         if(mDataLabelTopContainer == null)
         {
            mDataLabelTopContainer = NGraphUtils.AddGameObject(gameObject, -1, "Data Label Containers");
            newDataSeriesDataLabelTopContainerGameObject();
         }
         return mDataLabelTopContainer;
      }
   }

   public bool UnityGui {
      get;
      internal set;
   }

   /** \brief  Axs labels. 
     * 
     *  Used to reference the axis being worked with.
     */
   public enum Axis
   {
      X,
      Y
   }
   
   /** \brief  Tick style for axis.
     */
   public enum TickStyle
   {
      EvenSpace,              /**< Evenly space the tick marks. */
      EvenSpaceHigh,          /**< Evenly space the tick marks, including the max value on the axis. */
      EvenSpaceLow,           /**< Evenly space the tick marks, including the min value on the axis. */
      EvenSpaceLowAndHigh     /**< Evenly space the tick marks, including both the max and min values on the axis. */
   }
   
   /** \brief  The range of the X axis.  (x = min, y = max).
     */
   public Vector2 XRange
   {
      get { return mXRange; }
   }
   
   /** \brief The range of the Y axis.  (x = min, y = max).
     */
   public Vector2 YRange
   {
      get { return mYRange; }
   }

   /** \brief The area in which plots are allowed to draw.
     * The effective plot area after padding has been accounted for.
     */
   public Rect PlotArea
   {
      get
      {
         if(!mRectTransform)
            mRectTransform = gameObject.GetComponent<RectTransform>();
         if(!mRectTransform)
            mRectTransform = gameObject.AddComponent<RectTransform>();
         return new Rect(
            (mRectTransform.rect.xMin + Margin.x),
            (mRectTransform.rect.yMin + Margin.y),
            (mRectTransform.rect.width - Margin.z  - Margin.x),
            (mRectTransform.rect.height - Margin.y - Margin.w));
      }
   }

   public float GridLinesThicknesMajor;
   public Vector2 GridLinesSeparationMajor;
   public Color GridLinesColorMajor;

   public float GridLinesThicknesMinor;
   public Vector2 GridLinesSeparationMinor;
   public Color GridLinesColorMinor;
   
   
   protected virtual float guiScale()
   {
      return 1f;
   }
   
   protected virtual Rect adjustPlotArea(Rect pRect)
   {
      return pRect;
   }
   
   
   public virtual void Awake()
   {
      mRectTransform = gameObject.GetComponent<RectTransform>();
      if(!mRectTransform)
         mRectTransform = gameObject.AddComponent<RectTransform>();

      if(!Application.isPlaying)
         return;

      setupGraphHierarchy();
   }
   
   public virtual void Start()
   {
      mRectTransform = gameObject.GetComponent<RectTransform>();
      if(!mRectTransform)
         mRectTransform = gameObject.AddComponent<RectTransform>();

      setupGraphDefaults();
      
      if(!Application.isPlaying)
         return;

      RedrawNow = true;
   }

   protected void OnRectTransformDimensionsChange()
   {
      RedrawNow = true;
   }

   public virtual void setupGraphHierarchy()
   {
      if(mMeshesContainer == null)
         mMeshesContainer = NGraphUtils.AddGameObject(gameObject, 0, "MeshesContainer");
   }
   
   public void setupGraphDefaults()
   {
      if(transform.parent != null)
         gameObject.layer = transform.parent.gameObject.layer;
      if(AxisMaterial == null)
         AxisMaterial = Resources.Load("Materials/NGraphDefault", typeof(Material)) as Material;
      if(PlotBackgroundMaterial == null)
         PlotBackgroundMaterial = Resources.Load("Materials/NGraphDefault", typeof(Material)) as Material;
      if(DefaultPlotMaterial == null)
         DefaultPlotMaterial = Resources.Load("Materials/NGraphPlotAlphaClip", typeof(Material)) as Material;
   }

   public virtual void Update()
   {
    if (mMeshesContainer == null) {
      return;
    }
    if (!Application.isPlaying)
      {
         Transform pLeaked = transform.Find("Data Label Containers");
         if(pLeaked != null)
            DestroyImmediate(pLeaked.gameObject);

         pLeaked = transform.Find("MeshesContainer");
         if(pLeaked != null)
            DestroyImmediate(pLeaked.gameObject);

         return;
      }
   }

   public virtual void LateUpdate()
   {
    if (mMeshesContainer == null) {
      return;
    }
    if (!Application.isPlaying || !gameObject.activeSelf)
         return;

      if(RedrawNow || (RedrawOnTranslate && transform.hasChanged))
      {
         RedrawNow = false;
      
         DrawPlotBackground();
         DrawAxisBackground();
         DrawAxes();
         DrawAxisTicks();
         DrawGrid();
         DrawPlots();
         DrawLevels();

         transform.hasChanged = false;
      }
   }
   
   /** \brief  Set the X and Y axis ranges.
    * 
    * \param xMin The X axis min value.
    * \param xMax The X axis max value.
    * \param yMin The Y axis min value.
    * \param yMax The Y axis max value.
     */
   public void setRanges(float xMin, float xMax, float yMin, float yMax)
   {
      mXRange = new Vector2(xMin, xMax);
      mYRange = new Vector2(yMin, yMax);
      DrawAxes();
      DrawAxisTicks();
   }
   
   /** \brief  Set the just the X axis range.
    * 
    * \param xMin The X axis min value.
    * \param xMax The X axis max value.
    */
   public void setRangeX(float xMin, float xMax)
   {
      mXRange = new Vector2(xMin, xMax);
      DrawAxes();
      DrawAxisTicks();
      DrawPlots();
      DrawLevels();
   }
   
   /** \brief  Set the just the Y axis range.
    * 
    * \param yMin The Y axis min value.
    * \param yMax The Y axis max value.
    */
   public void setRangeY(float yMin, float yMax)
   {
      mYRange = new Vector2(yMin, yMax);
      DrawAxes();
      DrawAxisTicks();
      DrawPlots();
      DrawLevels();
   }
   
   List<NGraphDataSeries> mDataSeries = new List<NGraphDataSeries>();
   /** \brief  Add a data series type to the graph.
    * 
    *  T must be a child of NGraphDataSeries.
    * \param name Name of this plot.
    * \param pPlotColor Default color for this plot.
    * \param pMaterial Optional default material override for this plot.  Pass null for NGraph default.
    */
   public T addDataSeries<T>(string name, Color pPlotColor, Material pMaterial = null) where T : NGraphDataSeries
   {
      setupGraphHierarchy();
      GameObject pGameObject = NGraphUtils.AddGameObject(mMeshesContainer, PLOT_START_LEVEL * LEVEL_STEP, "Data Series - " + name);
      newDataSeriesGameObject(pGameObject);
      
      T pNGraphDataSeries = pGameObject.AddComponent<T>();
      if(pMaterial == null)
         pNGraphDataSeries.PlotMaterial = DefaultPlotMaterial;
      else
         pNGraphDataSeries.PlotMaterial = pMaterial;
      pNGraphDataSeries.PlotColor = pPlotColor;
      
      mDataSeries.Add(pNGraphDataSeries);

      GameObject pDataLabelContainer = NGraphUtils.AddGameObject(DataLabelTopContainer, -1, "\"" + name + "\"");
      newDataSeriesDataLabelContainerGameObject(pDataLabelContainer);

      List<GameObject> pChildGos = pNGraphDataSeries.setup(
         this,
         pGameObject,
         (GameObject pLabelGo, Vector3 pValue, string text) =>
         {
            if(text == null)
               text = pValue.y.ToString();
            Vector3 pDataLabelPosition = adjustPoint(new Vector2(pValue.x, pValue.y));
            AddDataSeriesLabel(pLabelGo, pDataLabelPosition, pValue, text);
         },
         pDataLabelContainer
      );
      newDataSeriesChildGameObjects(pChildGos);
      pNGraphDataSeries.DrawSeries();
      
      return pNGraphDataSeries;
   }

    public void deleteContainer()
    {
        if (mDataLabelTopContainer != null)
        {
            foreach (Transform child in mDataLabelTopContainer.transform)
            {
                if (mDataLabelTopContainer.transform != child)
                    Destroy(child.gameObject);
            }
        }
    }

   /** \brief  Use to clear graph of all plots.  SHould also be called with true before calling Destroy on graph.
    * 
    * \param destroyAllContainers set to true if you are abou tto Destroy the graph.
    */
   public virtual void breakdown(bool destroyAllContainers) {
      foreach (NGraphDataSeries series in mDataSeries) {
        Destroy(series.gameObject);
      }
      mDataSeries.Clear();

      if (destroyAllContainers) {
        if (mMeshesContainer != null) {
          Destroy(mMeshesContainer.gameObject);
          mMeshesContainer = null;
        }
        if (mDataLabelTopContainer != null) {
          Destroy(mDataLabelTopContainer.gameObject);
          mDataLabelTopContainer = null;
        }
        if (mAxesLabelContainerGo != null) {
          Destroy(mAxesLabelContainerGo.gameObject);
          mAxesLabelContainerGo = null;
        }
        if (mLabelContainerGo != null) {
          Destroy(mLabelContainerGo.gameObject);
          mLabelContainerGo = null;
        }
        if (mAxesGo != null) {
          Destroy(mAxesGo.gameObject);
          mAxesGo = null;
        }
        if (mXAxesGo != null) {
          Destroy(mXAxesGo.gameObject);
          mXAxesGo = null;
        }
        if (mYAxesGo != null) {
          Destroy(mYAxesGo.gameObject);
          mYAxesGo = null;
        }
        if (mAxesBackgroundGo != null) {
          Destroy(mAxesBackgroundGo.gameObject);
          mAxesBackgroundGo = null;
        }
        if (mGridContainer != null) {
          Destroy(mGridContainer.gameObject);
          mGridContainer = null;
        }
      }
   }

   protected virtual void newDataSeriesGameObject(GameObject pGameObject)
   {
   }
   protected virtual void newDataSeriesChildGameObjects(List<GameObject> pChildGos)
   {
   }
   protected virtual void newDataSeriesDataLabelTopContainerGameObject()
   {
   }
   protected virtual void newDataSeriesDataLabelContainerGameObject(GameObject pDataLabelContainer)
   {
   }
   
   public bool removeDataSeries(NGraphDataSeries pNGraphDataSeries)
   {
      if(!Application.isPlaying)
         return false;

      Destroy(pNGraphDataSeries.gameObject);
      return mDataSeries.Remove(pNGraphDataSeries);
   }
   
   public void DrawPlots()
   {
      foreach(NGraphDataSeries pNGraphDataSeries in mDataSeries)
      {
         float z = pNGraphDataSeries.transform.localPosition.z;
         pNGraphDataSeries.DrawSeries();
         pNGraphDataSeries.transform.localPosition = new Vector3(0, 0, z);
      }
    }

   public void DrawLevels()
   {
      NGraphUtils.SortChildren(mMeshesContainer);
   }

   protected GameObject mPlotBackgroundGo = null;
   protected abstract void _drawPlotBackground(List<UIVertex> pVertexList);
   public virtual void DrawPlotBackground()
   {
      if(mMeshesContainer == null) {
        return;
      }
      List<Transform> children = new List<Transform>();
      for (int i = mMeshesContainer.transform.childCount - 1; i >= 0; i--) {
         Transform child = mMeshesContainer.transform.GetChild(i);
         children.Add(child);
         child.SetParent(null);
      }

      if(mPlotBackgroundGo == null)
         mPlotBackgroundGo = NGraphUtils.AddGameObject(mMeshesContainer, 10, "Plot Background");
      
      foreach (Transform child in children) {
         child.SetParent(mMeshesContainer.transform);
      }

      List<UIVertex> pList = new List<UIVertex>(4);

      /* 
       * 1a       2b
       * 
       * 
       *    
       * 0c       3d
       * 
       */
      Rect pPlotArea = PlotArea;
      Vector3 a = new Vector3(pPlotArea.xMin, pPlotArea.yMax, 0);
      Vector3 b = new Vector3(pPlotArea.xMax, pPlotArea.yMax, 0);
      Vector3 c = new Vector3(pPlotArea.xMin, pPlotArea.yMin, 0);
      Vector3 d = new Vector3(pPlotArea.xMax, pPlotArea.yMin, 0);

      UIVertex pUIVertex = new UIVertex();
      
      pUIVertex.position = c;
      pUIVertex.uv0 = new Vector2(1, 1);
      pList.Add(pUIVertex);

      pUIVertex.position = a;
      pUIVertex.uv0 = new Vector2(0, 0);
      pList.Add(pUIVertex);
      
      pUIVertex.position = b;
      pUIVertex.uv0 = new Vector2(0, 1);
      pList.Add(pUIVertex);

      
      pUIVertex.position = d;
      pUIVertex.uv0 = new Vector2(1, 0);
      pList.Add(pUIVertex);

      float z = mPlotBackgroundGo.transform.localPosition.z;
      this._drawPlotBackground(pList);
      mPlotBackgroundGo.transform.localPosition = new Vector3(0, 0, z);
    }
   
   protected GameObject mAxesLabelContainerGo;
   protected GameObject mLabelContainerGo;
   protected virtual void addedAxesLabelContainer()
   {
      // Nothing here.  Used in child classes as simple callback.
   }

   protected abstract void _drawAxisTick(Axis axis, int index, GameObject pTickGameObject);
   protected virtual void DrawAxisTicks()
   {
      if(!Application.isPlaying)
         return;
      
      if(mAxesGo == null)
         return;
      
      AxesDrawAt.x = Mathf.Clamp (AxesDrawAt.x, XRange.x, XRange.y);
      AxesDrawAt.y = Mathf.Clamp (AxesDrawAt.y, YRange.x, YRange.y);
      
      if(mAxesLabelContainerGo == null)
      {
         mAxesLabelContainerGo = NGraphUtils.AddGameObject (gameObject, 0, "Axes Label Container");
         addedAxesLabelContainer();
      }

      int numTicks = XNumberOfTicks;
      if(XTickStyle == TickStyle.EvenSpaceLowAndHigh)
         numTicks--;
      float step = (mXRange.y - mXRange.x) / numTicks;
      if(XTickStyle == TickStyle.EvenSpaceLowAndHigh)
         numTicks++;
      for(int i = 0; i < numTicks; i++)
      {
         GameObject pTickGo = NGraphUtils.AddGameObject(mXAxesGo, AXES_LEVEL * LEVEL_STEP, "Tick X - " + i);
         
         float val = step * (i+1);
         if(XTickStyle == TickStyle.EvenSpace)
            val -= step/2;
         else if(XTickStyle == TickStyle.EvenSpaceLow || XTickStyle == TickStyle.EvenSpaceLowAndHigh)
            val -= step;
         Vector2 pPoint = new Vector2(val + XRange.x, AxesDrawAt.y);
         pPoint = adjustPoint(pPoint);
         
         Vector3 pPosition = pTickGo.transform.localPosition;
         pPosition.x += pPoint.x;
         pPosition.y += pPoint.y;
         pTickGo.transform.localPosition = pPosition;
         
         if(DrawXLabel)
         {
            GameObject pTickLabelGo = NGraphUtils.AddGameObject(mAxesLabelContainerGo.gameObject, AXES_LEVEL * LEVEL_STEP, "Tick Label X - " + i, false);
            pPosition.y -= 4;
            
            AddAxisLabel(Axis.X, pTickLabelGo, pPosition, val + XRange.x);
         }
         this._drawAxisTick(Axis.X, i, pTickGo);
      }
      
      numTicks = YNumberOfTicks;
      if(YTickStyle == TickStyle.EvenSpaceLowAndHigh)
         numTicks--;
      step = (mYRange.y - mYRange.x) / numTicks;
      if(YTickStyle == TickStyle.EvenSpaceLowAndHigh)
         numTicks++;
      for(int i = 0; i < numTicks; i++)
      {
         GameObject pTickGo = NGraphUtils.AddGameObject(mYAxesGo, AXES_LEVEL * LEVEL_STEP, "Tick Y - " + i);
         
         float val = step * (i+1);
         if(YTickStyle == TickStyle.EvenSpace)
            val -= step/2;
         else if(YTickStyle == TickStyle.EvenSpaceLow || YTickStyle == TickStyle.EvenSpaceLowAndHigh)
            val -= step;
         Vector2 pPoint = new Vector2(AxesDrawAt.x, val + YRange.x);
         pPoint = adjustPoint(pPoint);
         
         Vector3 pPosition = pTickGo.transform.localPosition;
         pPosition.x += pPoint.x;
         pPosition.y += pPoint.y;
         pTickGo.transform.localPosition = pPosition;
         
         if(DrawYLabel)
         {
            GameObject pTickLabelGo = NGraphUtils.AddGameObject(mAxesLabelContainerGo.gameObject, 0, "Tick Label Y - " + i, false);
            pPosition.x -= 6;
            
            AddAxisLabel(Axis.Y, pTickLabelGo, pPosition, val + YRange.x);
         }

         this._drawAxisTick(Axis.Y, i, pTickGo);
      }
   }
   
   protected GameObject mAxesGo;
   protected GameObject mXAxesGo;
   protected GameObject mYAxesGo;
   protected abstract void _drawAxes(Rect xAxis, Rect yAxis);
   protected virtual void _addedAxesGameObject(GameObject pAxisGameObject)
   {
   }
   protected virtual void _addedXAxisGameObject(GameObject pAxisGameObject)
   {
   }
   protected virtual void _addedYAxisGameObject(GameObject pAxisGameObject)
   {
   }
   protected void DrawAxes()
   {
      if(!Application.isPlaying || mMeshesContainer == null)
         return;
      
      if(mXRange == Vector2.zero)
         return;
      
      if(mYRange == Vector2.zero)
         return;
      
      AxesDrawAt.x = Mathf.Clamp(AxesDrawAt.x, XRange.x, XRange.y);
      AxesDrawAt.y = Mathf.Clamp(AxesDrawAt.y, YRange.x, YRange.y);
      
      Vector2 pTop      = adjustPoint(new Vector2(AxesDrawAt.x, YRange.y));
      Vector2 pBottom   = adjustPoint(new Vector2(AxesDrawAt.x, YRange.x));
      Vector2 pLeft     = adjustPoint(new Vector2(XRange.x, AxesDrawAt.y));
      Vector2 pRight    = adjustPoint(new Vector2(XRange.y, AxesDrawAt.y));

      if (mAxesGo == null)
      {
         mAxesGo  = NGraphUtils.AddGameObject (mMeshesContainer, AXES_LEVEL * LEVEL_STEP, "Axes");
         _addedAxesGameObject(mAxesGo);
         mXAxesGo = NGraphUtils.AddGameObject (mAxesGo, 0, "X Axes");
         _addedXAxisGameObject(mXAxesGo);
         mYAxesGo = NGraphUtils.AddGameObject (mAxesGo, 0, "Y Axes");
         _addedYAxisGameObject(mYAxesGo);
      }
      
      Rect x = new Rect (pLeft.x, AxesThickness / 2 + pLeft.y, pRight.x - pLeft.x, -AxesThickness);
      Rect y = new Rect(pTop.x - AxesThickness/2, pTop.y, AxesThickness, -(pTop.y - pBottom.y));


      float z1 = mAxesGo.transform.localPosition.z;
      float z2 = mXAxesGo.transform.localPosition.z;
      float z3 = mYAxesGo.transform.localPosition.z;

      this._drawAxes(x, y);

      mAxesGo.transform.localPosition = new Vector3(0, 0, z1);
      mXAxesGo.transform.localPosition = new Vector3(0, 0, z2);
      mYAxesGo.transform.localPosition = new Vector3(0, 0, z3);
    }

   protected GameObject mAxesBackgroundGo;
   protected abstract void _drawAxisBackground(List<UIVertex> pVertexList);
   protected void DrawAxisBackground()
   {
      if(mAxesBackgroundGo == null)
         mAxesBackgroundGo = NGraphUtils.AddGameObject(mMeshesContainer, BACKGROUND_LEVEL * LEVEL_STEP, "Axes Background");

      /* 
       * a  b&m       n&i j
       *    p          o 
       *               
       *              
       *               
       *               l  k
       *     e            f
       * c  d&g           h
       * 
       */
      Rect pRect = mRectTransform.rect;
      Vector3 a = new Vector3(pRect.xMin, pRect.yMax, 0);
      Vector3 b = new Vector3(pRect.xMin + Margin.x, pRect.yMax, 0);
      Vector3 c = new Vector3(pRect.xMin, pRect.yMin, 0);
      Vector3 d = new Vector3(pRect.xMin + Margin.x, pRect.yMin, 0);

      Vector3 e = new Vector3(pRect.xMin + Margin.x, pRect.yMin + Margin.y, 0);
      Vector3 f = new Vector3(pRect.xMax, pRect.yMin + Margin.y, 0);
      Vector3 g = new Vector3(pRect.xMin + Margin.x, pRect.yMin, 0);
      Vector3 h = new Vector3(pRect.xMax, pRect.yMin, 0);
      
      Vector3 i = new Vector3(pRect.xMax - Margin.z, pRect.yMax, 0);
      Vector3 j = new Vector3(pRect.xMax, pRect.yMax, 0);
      Vector3 k = new Vector3(pRect.xMax, pRect.yMin + Margin.y, 0);
      Vector3 l = new Vector3(pRect.xMax - Margin.z, pRect.yMin + Margin.y, 0);

      Vector3 m = new Vector3(pRect.xMin + Margin.x, pRect.yMax, 0);
      Vector3 n = new Vector3(pRect.xMax - Margin.z, pRect.yMax, 0);
      Vector3 o = new Vector3(pRect.xMax - Margin.z, pRect.yMax - Margin.w, 0);
      Vector3 p = new Vector3(pRect.xMin + Margin.x, pRect.yMax - Margin.w, 0);

      List<UIVertex> pList = new List<UIVertex>(6);
      UIVertex pUIVertex = new UIVertex();
      
      
      pUIVertex.position = a;
      pUIVertex.uv0 = new Vector2(0, 1);
      pList.Add(pUIVertex);
      
      pUIVertex.position = b;
      pUIVertex.uv0 = new Vector2(0.5f, 1);
      pList.Add(pUIVertex);
      
      pUIVertex.position = d;
      pUIVertex.uv0 = new Vector2(0.5f, 0.5f);
      pList.Add(pUIVertex);

      pUIVertex.position = c;
      pUIVertex.uv0 = new Vector2(0, 0);
      pList.Add(pUIVertex);
      
      
      pUIVertex.position = e;
      pUIVertex.uv0 = new Vector2(1, 0.5f);
      pList.Add(pUIVertex);
      
      pUIVertex.position = f;
      pUIVertex.uv0 = new Vector2(0, 1);
      pList.Add(pUIVertex);
      
      pUIVertex.position = h;
      pUIVertex.uv0 = new Vector2(0, 1);
      pList.Add(pUIVertex);

      pUIVertex.position = g;
      pUIVertex.uv0 = new Vector2(0, 1);
      pList.Add(pUIVertex);
      
      
      pUIVertex.position = i;
      pUIVertex.uv0 = new Vector2(1, 0.5f);
      pList.Add(pUIVertex);
      
      pUIVertex.position = j;
      pUIVertex.uv0 = new Vector2(0, 1);
      pList.Add(pUIVertex);
      
      pUIVertex.position = k;
      pUIVertex.uv0 = new Vector2(0, 1);
      pList.Add(pUIVertex);
      
      pUIVertex.position = l;
      pUIVertex.uv0 = new Vector2(0, 1);
      pList.Add(pUIVertex);
      
      
      pUIVertex.position = m;
      pUIVertex.uv0 = new Vector2(1, 0.5f);
      pList.Add(pUIVertex);
      
      pUIVertex.position = n;
      pUIVertex.uv0 = new Vector2(0, 1);
      pList.Add(pUIVertex);
      
      pUIVertex.position = o;
      pUIVertex.uv0 = new Vector2(0, 1);
      pList.Add(pUIVertex);
      
      pUIVertex.position = p;
      pUIVertex.uv0 = new Vector2(0, 1);
      pList.Add(pUIVertex);


      float z = mAxesBackgroundGo.transform.localPosition.z;
      this._drawAxisBackground (pList);
      mAxesBackgroundGo.transform.localPosition = new Vector3(0, 0, z);
   }

   GameObject mGridContainer;
   protected abstract void _addedGridContainer(GameObject pGridContainer);
   protected abstract void _drawMajorGridLine(Axis axis, int index, float r, GameObject pGridLineGameObject);
   protected void DrawGrid()
   {
      // Make sure it exists
      if(mGridContainer == null)
      {
         mGridContainer = NGraphUtils.AddGameObject(mMeshesContainer, LEVEL_STEP * 2, "GridContainer");
         _addedGridContainer(mGridContainer);
      }

      // Clear the children
      for(int i = 0; i < mGridContainer.transform.childCount; i++)
         Destroy(mGridContainer.transform.GetChild(i).gameObject);

      int index = 0;
      if(GridLinesSeparationMajor.x > 0)
      {
         for(float r = XRange.x; r <= XRange.y; r += GridLinesSeparationMajor.x)
         {
            GameObject pGridLineGo = NGraphUtils.AddGameObject(mGridContainer, 0, "Major X Grid Line - " + r);
            this._drawMajorGridLine(Axis.X, index++, r, pGridLineGo);
         }
      }

      index = 0;
      if(GridLinesSeparationMajor.y > 0)
      {
         for(float r = YRange.x; r <= YRange.y; r += GridLinesSeparationMajor.y)
         {
            GameObject pGridLineGo = NGraphUtils.AddGameObject(mGridContainer, 0, "Major Y Grid Line - " + r);
            this._drawMajorGridLine(Axis.Y, index++, r, pGridLineGo);
         }
      }

      float z = mGridContainer.transform.localPosition.z;
      mGridContainer.transform.localPosition = new Vector3(0, 0, z);
    }
   
   public float adjustPointX(float f)
   {
      float xPercent = (f - XRange.x) / (XRange.y - XRange.x);
      
      Rect pPlotArea = PlotArea;
      float newX = (pPlotArea.width * xPercent) + pPlotArea.xMin;
      
      return newX;
   }
   
   public float adjustPointY(float f)
   {
      float yPercent = (f - YRange.x) / (YRange.y - YRange.x);
      
      Rect pPlotArea = PlotArea;
      float newY = (pPlotArea.height * yPercent) + pPlotArea.yMin;

      return newY;
   }

   public Vector2 adjustPoint(Vector2 pDataPoint)
   {
      pDataPoint.x = adjustPointX(pDataPoint.x);
      pDataPoint.y = adjustPointY(pDataPoint.y);
      return pDataPoint;
   }
}
