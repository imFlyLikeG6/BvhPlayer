/***********************************************************************
/*      Copyright Niugnep Software, LLC 2013 All Rights Reserved.      *
/*                                                                     *
/*     THIS WORK CONTAINS TRADE SECRET AND PROPRIETARY INFORMATION     *
/*     WHICH IS THE PROPERTY OF NIUGNEP SOFTWARE, LLC OR ITS           *
/*             LICENSORS AND IS SUBJECT TO LICENSE TERMS.              *
/**********************************************************************/

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*! \brief Class used to draw in the NGUI enviornment.
 *
 *  It is best to use the wizard to create this class.
 *  The wizard will add a Game Object with this component
 *  already installed and setup with defaults.
 * 
 *  Please delete this class if you do not have NGUI.
 */
public class UINgraph : NGraphMesh
{
   public Camera UiCamera;
   
   public int fontSize = 16;
   public Font AxisLabelDynamicFont;   // If not null will always be used over bitmap font
   public UIFont AxisLabelBitmapFont;
   
   public delegate void NguiAxisLabelOverrideCallback(NGraph.Axis asix, UILabel pLabel, float val);
   public NguiAxisLabelOverrideCallback AxisLabelCallback
   {
      get;
      set;
   }
   
   public delegate void NguiDataLabelOverrideCallback(UILabel pLabel, Vector3 pValue, string text);
   public NguiDataLabelOverrideCallback DataLabelCallback
   {
      get;
      set;
   }

   
   public override void Start()
   {
      base.Start();
      
      if(UiCamera == null)
         UiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
   }
   
   protected override void AddAxisLabel(NGraph.Axis axis, GameObject pLabelGameObject, Vector3 pPosition, float val)
   {
      UILabel pLabel = pLabelGameObject.AddComponent<UILabel>();
      pLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
      if(AxisLabelDynamicFont != null)
      {
         pLabel.trueTypeFont = AxisLabelDynamicFont;
         pLabel.fontSize = fontSize;
      }
      else
      {
         pLabel.bitmapFont = AxisLabelBitmapFont;
      }
      pLabel.color = AxisLabelColor;
      
      if(axis == NGraph.Axis.X)
         pLabel.pivot = UIWidget.Pivot.Top;
      else if(axis == NGraph.Axis.Y)
         pLabel.pivot = UIWidget.Pivot.Right;
      pLabel.text = val.ToString();

      pLabelGameObject.transform.localPosition = pPosition;
      
      if(AxisLabelCallback != null)
         AxisLabelCallback(axis, pLabel, val);
   }

   protected override void AddDataSeriesLabel(GameObject pLabelGameObject, Vector3 pPosition, Vector3 pValue, string val)
   {
      UILabel pLabel = pLabelGameObject.GetComponent<UILabel>();
      if(pLabel == null)
      {
         pLabel = pLabelGameObject.AddComponent<UILabel>();
         pLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
         if(AxisLabelDynamicFont != null)
         {
            pLabel.trueTypeFont = AxisLabelDynamicFont;
            pLabel.fontSize = fontSize;
         }
         else
         {
            pLabel.bitmapFont = AxisLabelBitmapFont;
         }
         pLabel.color = AxisLabelColor;
      }

      pLabel.text = val.ToString();
      pLabelGameObject.transform.localPosition = pPosition;
      
      if(DataLabelCallback != null)
         DataLabelCallback(pLabel, pValue, val);
   }
   
   public override void Update ()
   {
      if(!Application.isPlaying)
         return;
      
      base.Update();
      
      /*
      float adjustment = 1f;
      
      mRect = UiCamera.pixelRect;
      if (mRoot != null)
         adjustment = mRoot.pixelSizeAdjustment;
      float rectWidth = mRect.width;
      float rectHeight = mRect.height;
      
      if (adjustment != 1f && rectHeight > 1f)
      {
         float scale = mRoot.activeHeight / rectHeight;
         rectWidth *= scale;
         rectHeight *= scale;
      }
      
      float rootScaleMultiplier = 1/mRoot.transform.localScale.x;
      */
      
   }
   
   
#if UNITY_EDITOR

   int mScreenHeight = 720;
   
   /// <summary>
   /// Draw a visible pink outline for the clipped area.
   /// </summary>
   
   void OnDrawGizmos ()
   {
      if (UiCamera == null || !UiCamera.orthographic) return;
      
      Vector2 size = Vector2.zero;
      
      GameObject go = UnityEditor.Selection.activeGameObject;
      bool selected = (go != null) && (go.GetComponent<UINgraph>() == this);
      
      if (size.x == 0f) size.x = mRectTransform.rect.width;
      if (size.y == 0f) size.y = mRectTransform.rect.height;
     
      UIRoot root = NGUITools.FindInParents<UIRoot>(gameObject);
      if (root != null) size *= root.GetPixelSizeAdjustment(mScreenHeight);
     
      //Transform t = (UiCamera != null ? UiCamera.transform : null);
      Transform t = transform;
      if(t != null)
      {
         Gizmos.matrix = t.localToWorldMatrix;
         
         if(selected)
            Gizmos.color = new Color(0f, 0.5f, 0.5f);
         else
            Gizmos.color = new Color(0.2f, 0.0f, 0.3f);
         
         Gizmos.DrawWireCube(mRectTransform.rect.center, size);
      }
   }
#endif
}
