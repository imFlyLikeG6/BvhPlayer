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

/*! \brief Base NGraph Data Series (Plot) class.
 *
 *  This class is used by the different GUI endpoints.  Most of the 
 *  loggic for setting up and drawing a data series is handled here.
 */
public abstract class NGraphDataSeries : MonoBehaviour
{
   protected int mLevel;
   protected Color mPlotColor;
   protected NGraph mGraph;
   protected GameObject mGameObject;
   protected GameObject mDataLabelContainerGo;
   protected NGraph.DataSeriesDataLabelCallback mDataLabelCallback;
   private Material mPlotMaterial;
   
   /** The plot's "z" level. 
     *  Used to reference or change the "z" level of the plot.
     */
   public int Level
   {
      get { return mLevel; }
      set { mLevel = value; redraw(); }
   }
   
   /** \brief  The plot's color. 
     * 
     *  Used to reference or change the plot color.
     */
   public Color PlotColor
   {
      get { return mPlotColor; }
      set { mPlotColor = value; redraw(); }
   }
   
   /** \brief  The plot's Material. 
     * 
     *  Used to reference or change the plot color.
     */
   public Material PlotMaterial
   {
      get { return mPlotMaterial; }
      set { mPlotMaterial = new Material(value); redraw(); }
   }
   
   public virtual void Update()
   {
   }
   
   private void redraw()
   {
      DrawSeries();
   }
   
   protected Vector4 mClipping = Vector4.zero;
   public virtual void DrawSeries()
   {
      if(mGraph == null)
         return;
      
      setClipping();
   }
   
   protected virtual void setClipping()
   {
      if(mGraph == null)
         return;

      if(mGraph.UnityGui)
      {
         RectTransform pRectTransform = mGraph.GetComponent<RectTransform>();
        
         float hOffset = gameObject.transform.position.x - Screen.width / 2;
         float vOffset = gameObject.transform.position.y - Screen.height / 2;

         mClipping.x = -pRectTransform.rect.width / 2 + hOffset + mGraph.Margin.x;
         mClipping.y = pRectTransform.rect.width / 2 + hOffset - mGraph.Margin.z;
         mClipping.z = -pRectTransform.rect.height / 2 + vOffset + mGraph.Margin.y;
         mClipping.w = pRectTransform.rect.height / 2 + vOffset - +mGraph.Margin.z;
      }
      else
      {
         RectTransform pRectTransform = mGraph.GetComponent<RectTransform>();
         mClipping.x = -pRectTransform.rect.width/2 + mGraph.Margin.x;
         mClipping.y = pRectTransform.rect.width/2 - mGraph.Margin.z;
         mClipping.z = -pRectTransform.rect.height/2 + mGraph.Margin.y;
         mClipping.w = pRectTransform.rect.height/2 - mGraph.Margin.w;
      }
   }

  public virtual List<GameObject> setup(NGraph pGraph, GameObject pGameObject, NGraph.DataSeriesDataLabelCallback pDataLabelCallback, GameObject pDataLabelContainer)
   {
      if(mGameObject != null)
         return null;
      
      mGraph = pGraph;
      mGameObject = pGameObject;
      mDataLabelCallback = pDataLabelCallback;

      List<GameObject> pGoList = new List<GameObject>();
      mDataLabelContainerGo = pDataLabelContainer;
      return pGoList;
   }
   
   public virtual void teardown(NGraph pGraph, GameObject pGameObject)
   {
      Destroy(mDataLabelContainerGo);
      mDataLabelContainerGo = null;
      mGraph = null;
      mGameObject = null;
   }
   
   protected void emitSetupError()
   {
      Debug.LogError("Data Series has not been added to NGraph object.  Call \"addDataSeries\" on the graph you wish to add this Data Series to.");
   }
}
