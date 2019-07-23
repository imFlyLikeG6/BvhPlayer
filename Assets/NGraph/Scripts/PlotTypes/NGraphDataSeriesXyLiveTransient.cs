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

/*! \brief Continually updating plot.
 *
 *  This plot type takes the last value of "UpdateValue" and appends it to
 *  the plot data.  It does this at the rate specified by "UpdateRate".
 */
public class NGraphDataSeriesXyLiveTransient : NGraphDataSeriesXy
{
   /** \brief The plot's update rate. 
     * 
     *  This is the rate that a new data point will be added to the plot using UpdateValue.
     */
   public float UpdateRate = 2.0f;
   
   /** \brief The plot's next value. 
     * 
     *  This value will be appended to the plot's data the next time
     *  the update rate has elapsed.
     */
   public float UpdateValue = 0.0f;
   
   private float mLastUpdate = 0;
   
   public override void Update()
   {
      mPlotStyle = NGraphDataSeriesXy.Style.Line;
      
      base.Update();
      mLastUpdate += Time.unscaledDeltaTime;
      
      if(mData == null)
         return;
      
      for(int i = 0; i < mData.Count; i++)
      {
         Vector2 pDataPoint = mData[i];
         pDataPoint.x -= Time.unscaledDeltaTime;
         if(pDataPoint.x < mGraph.XRange.x)
            mData.RemoveAt(0);
         else
            mData[i] = pDataPoint;
      }
      
      if(mLastUpdate < UpdateRate)
      {
         DrawSeries();
         return;
      }
      mLastUpdate = 0;
      
      mData.Add(new Vector2(mGraph.XRange.y, UpdateValue));
      
      DrawSeries();
   }
   
   
}
