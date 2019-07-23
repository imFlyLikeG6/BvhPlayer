/***********************************************************************
/*      Copyright Niugnep Software, LLC 2014 All Rights Reserved.      *
/*                                                                     *
/*     THIS WORK CONTAINS TRADE SECRET AND PROPRIETARY INFORMATION     *
/*     WHICH IS THE PROPERTY OF NIUGNEP SOFTWARE, LLC OR ITS           *
/*             LICENSORS AND IS SUBJECT TO LICENSE TERMS.              *
/**********************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*! \brief Line style plot of an equation.
 *
 *  This plot type takes an equation and plots it for you.
 */
public class NGraphDataSeriesXyEquation : NGraphDataSeriesXy
{
   protected float mResolution = 0.5f;
   protected string mEquation = "";

   /** \brief The plot's resolution of equated points. 
     * 
     *  Starting at the left of most value of the graph (min x) this value represents the step amount along the X asix for the equation to evalute at.
     * 
     *  Setting this value will re-evaluate the plot.
     *  The value must be greater than zero.
     */
   public float Resolution
   {
      get { return mResolution; }
      set
      {
         if(mResolution == value)
            return;
         if(mResolution <= 0)
            return;

         mResolution = value;
         evaluate();
      }
   }

   /** \brief The plot's equation. 
     * 
     *  This is the equation that the plot will use to draw itself from the parent graph's min x to max x values.
     * 
     *  Setting this value will re-evaluate the plot.
     *  The value must be parsable.
     *  Use "x" in your equation for the substatute value.
     *  
     *  Example: x * 4
     */
   public string Equation
   {
      get { return mEquation; }
      set
      {
         if(value == "")
            return;

         if(mEquation == value)
            return;

         mEquation = value;
         evaluate();
      }
   }

   public void evaluate()
   {
      if(mGraph == null)
         return;

      if(mEquation == "")
         return;

      Ngraph.EquationParser pParser = new Ngraph.EquationParser(mEquation);

      mData.Clear();
      for(float val = mGraph.XRange.x; val <= mGraph.XRange.y; val += mResolution)
      {
         List<KeyValuePair<string, double>> valueReplacements = new List<KeyValuePair<string, double>>(1);
         valueReplacements.Add(new KeyValuePair<string, double>("x", val));

         float y = (float)pParser.evalExpression(valueReplacements);
         mData.Add(new Vector2(val, y));
      }

      DrawSeries();
   }

}
