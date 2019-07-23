/***********************************************************************
/*      Copyright Niugnep Software, LLC 2013 All Rights Reserved.      *
/*                                                                     *
/*     THIS WORK CONTAINS TRADE SECRET AND PROPRIETARY INFORMATION     *
/*     WHICH IS THE PROPERTY OF NIUGNEP SOFTWARE, LLC OR ITS           *
/*             LICENSORS AND IS SUBJECT TO LICENSE TERMS.              *
/**********************************************************************/

using UnityEngine;
using System.Collections;

public class NGraphTakeScreenshot : MonoBehaviour
{    
   private int screenshotCount = 0;
     
   // Check for screenshot key each frame
   void Update()
   {
      // take screenshot on up->down transition of F9 key
      if (Input.GetKeyDown("f9"))
      {        
         string screenshotFilename;
         do
         {
            screenshotCount++;
            screenshotFilename = "screenshot" + screenshotCount + ".png";
         } while (System.IO.File.Exists(screenshotFilename));
         
         ScreenCapture.CaptureScreenshot(screenshotFilename);
      }
   }
}