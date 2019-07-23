using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*! \brief Base class used to draw when native Meshes are used.
 *
 *  This is a base class used to encapsulate implementation types
 * that use the native Mesh class to draw to the screen.
 */
public abstract class NGraphMesh : NGraph
{
   protected Mesh mPlotBackgroundMesh;
   protected MeshRenderer mPlotBackgroundMeshRenderer;

   public NGraphMesh()
   {
      UnityGui = false;
   }
   
   public override void Start()
   {
      UnityGui = false;
      base.Start ();
   }
   /* ---------- */
   /* BACKGROUND */
   /* ---------- */

   protected override void _drawPlotBackground(List<UIVertex> pVertexList)
   {
      if(mPlotBackgroundMesh == null)
      {
         NGraphUtils.AddMesh(mPlotBackgroundGo, out mPlotBackgroundMeshRenderer, out mPlotBackgroundMesh);
      }
      
      mPlotBackgroundMesh.Clear();
      mPlotBackgroundMeshRenderer.material = PlotBackgroundMaterial;
      mPlotBackgroundMeshRenderer.material.SetColor("_TintColor", PlotBackgroundColor);

      List<Vector3> pVertices  = new List<Vector3>();
      List<Vector2> pUvs  = new List<Vector2>();
      List<int> pTriangles  = new List<int>();
      
      UIVertex a = pVertexList [1];
      UIVertex b = pVertexList [2];
      UIVertex c = pVertexList [0];
      UIVertex d = pVertexList [3];

      pVertices.Add(c.position);
      pVertices.Add(a.position);
      pVertices.Add(b.position);
      pVertices.Add(d.position);
      pUvs.Add(c.uv0);
      pUvs.Add(a.uv0);
      pUvs.Add(b.uv0);
      pUvs.Add(d.uv0);
      
      /* 
       * 1a       2b
       * 
       * 
       *    
       * 0c       3d
       * 
       */
      pTriangles.Add(0);
      pTriangles.Add(1);
      pTriangles.Add(2);

      pTriangles.Add(0);
      pTriangles.Add(2);
      pTriangles.Add(3);

      mPlotBackgroundMesh.vertices = pVertices.ToArray();
      mPlotBackgroundMesh.uv = pUvs.ToArray();
      mPlotBackgroundMesh.triangles = pTriangles.ToArray();
   }
   
   /* ---------- */
   /* MESH TICKS */
   /* ---------- */

   List<MeshRenderer> mXAxisTicks = new List<MeshRenderer>();
   List<MeshRenderer> mYAxisTicks = new List<MeshRenderer>();
   protected override void DrawAxisTicks()
   {
      if (!Application.isPlaying)
         return;
      
      if (this.mAxesGo == null)
         return;
      
      foreach (MeshRenderer pMeshRenderer in mXAxisTicks)
      {
         // Destroy both the Mesh object and game object - if we don't destroy the mesh object it will leak in the VOB
         Destroy (pMeshRenderer.GetComponent<MeshFilter>().mesh);
         Destroy (pMeshRenderer.gameObject);
      }
      foreach (MeshRenderer pMeshRenderer in mYAxisTicks)
      {
         // Destroy both the Mesh object and game object - if we don't destroy the mesh object it will leak in the VOB
         Destroy (pMeshRenderer.GetComponent<MeshFilter>().mesh);
         Destroy (pMeshRenderer.gameObject);
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

   protected override void _drawAxisTick(Axis axis, int index, GameObject pTickGameObject)
   {
      MeshRenderer pMeshRenderer;
      Mesh pMesh;
      NGraphUtils.AddMesh(pTickGameObject, out pMeshRenderer, out pMesh);
      mXAxisTicks.Add(pMeshRenderer);

      pMesh.Clear();
      pMeshRenderer.material = AxisMaterial;
      pMeshRenderer.material.SetColor("_TintColor", AxisColor);

      if(axis == Axis.X)
         NGraphUtils.DrawRect(new Rect(-AxesThickness/2, AxesThickness/2+4, AxesThickness, -AxesThickness-8), pMesh);
      else if(axis == Axis.Y)
         NGraphUtils.DrawRect(new Rect(-AxesThickness/2-4, -AxesThickness/2, AxesThickness+8, AxesThickness), pMesh);
   }

   /* ---------- */
   /* GRID LINES */
   /* ---------- */

   protected override void _addedGridContainer(GameObject pGridContainer)
   {
   }
   protected override void _drawMajorGridLine(Axis axis, int index, float r, GameObject pGridLineGameObject)
   {
      MeshRenderer pMeshRenderer;
      Mesh pMesh;

      NGraphUtils.AddMesh(pGridLineGameObject, out pMeshRenderer, out pMesh);
      mXAxisTicks.Add(pMeshRenderer);

      pMeshRenderer.material = AxisMaterial;
      pMeshRenderer.material.SetColor("_TintColor", GridLinesColorMajor);
      
      if(axis == Axis.X)
         NGraphUtils.DrawRect(new Rect(adjustPointX(r) - (GridLinesThicknesMajor/2f), adjustPointY(YRange.y), GridLinesThicknesMajor, adjustPointY(YRange.x) - adjustPointY(YRange.y)), pMesh);
      else if(axis == Axis.Y)
         NGraphUtils.DrawRect(new Rect(adjustPointX(XRange.x), adjustPointY(r) + (GridLinesThicknesMajor/2f), adjustPointX(XRange.y) - adjustPointX(XRange.x), -GridLinesThicknesMajor), pMesh);

   }
   
   /* ---------- */
   /* GRID LINES */
   /* ---------- */
   
   Mesh mXAxisMesh;
   Mesh mYAxisMesh;
   MeshRenderer mAxisMeshRendererX;
   MeshRenderer mAxisMeshRendererY;
   protected override void _drawAxes(Rect xAxis, Rect yAxis)
   {
      if(mAxisMeshRendererX == null)
         NGraphUtils.AddMesh (mXAxesGo, out mAxisMeshRendererX, out mXAxisMesh);
      if(mAxisMeshRendererY == null)
         NGraphUtils.AddMesh (mYAxesGo, out mAxisMeshRendererY, out mYAxisMesh);

      mAxisMeshRendererX.material = AxisMaterial;
      mAxisMeshRendererX.material.SetColor ("_TintColor", AxisColor);
      mAxisMeshRendererY.material = AxisMaterial;
      mAxisMeshRendererY.material.SetColor ("_TintColor", AxisColor);
      NGraphUtils.DrawRect(xAxis, mXAxisMesh);
      NGraphUtils.DrawRect(yAxis, mYAxisMesh);
   }
   
   /* --------------- */
   /* AXIS BACKGROUND */
   /* --------------- */

   Mesh mAxisBackgroundMesh;
   MeshRenderer mAxisBackgroundMeshRenderer;
   protected override void _drawAxisBackground(List<UIVertex> pVertexList)
   {
      if(mAxisBackgroundMesh == null)
         NGraphUtils.AddMesh(mAxesBackgroundGo, out mAxisBackgroundMeshRenderer, out mAxisBackgroundMesh);

      mAxisBackgroundMesh.Clear();

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
      
      List<Vector3> pVertices  = new List<Vector3>();
      List<Vector2> pUvs  = new List<Vector2>();
      List<int> pTriangles  = new List<int>();

      int idx = 0;
      UIVertex a = pVertexList[idx++];
      UIVertex b = pVertexList[idx++];
      UIVertex c = pVertexList[idx++];
      UIVertex d = pVertexList[idx++];
      UIVertex e = pVertexList[idx++];
      UIVertex f = pVertexList[idx++];
      UIVertex g = pVertexList[idx++];
      UIVertex h = pVertexList[idx++];
      UIVertex i = pVertexList[idx++];
      UIVertex j = pVertexList[idx++];
      UIVertex k = pVertexList[idx++];
      UIVertex l = pVertexList[idx++];
      UIVertex m = pVertexList[idx++];
      UIVertex n = pVertexList[idx++];
      UIVertex o = pVertexList[idx++];
      UIVertex p = pVertexList[idx++];

      //Left
      pVertices.Add(d.position);
      pVertices.Add(c.position);
      pVertices.Add(a.position);
      pVertices.Add(b.position);
      //Bottom
      pVertices.Add(h.position);
      pVertices.Add(g.position);
      pVertices.Add(e.position);
      pVertices.Add(f.position);
      //Right
      pVertices.Add(l.position);
      pVertices.Add(k.position);
      pVertices.Add(i.position);
      pVertices.Add(j.position);
      //Top
      pVertices.Add(p.position);
      pVertices.Add(o.position);
      pVertices.Add(m.position);
      pVertices.Add(n.position);
      
      //Left
      pUvs.Add(c.uv0);
      pUvs.Add(d.uv0);
      pUvs.Add(a.uv0);
      pUvs.Add(b.uv0);
      //Bottom
      pUvs.Add(e.uv0);
      pUvs.Add(f.uv0);
      pUvs.Add(g.uv0);
      pUvs.Add(h.uv0);
      //Right
      pUvs.Add(l.uv0);
      pUvs.Add(k.uv0);
      pUvs.Add(i.uv0);
      pUvs.Add(j.uv0);
      //Right
      pUvs.Add(m.uv0);
      pUvs.Add(n.uv0);
      pUvs.Add(p.uv0);
      pUvs.Add(o.uv0);

      //Left
      int offset = 0;
      pTriangles.Add(offset + 0);
      pTriangles.Add(offset + 2);
      pTriangles.Add(offset + 1);
      
      pTriangles.Add(offset + 1);
      pTriangles.Add(offset + 2);
      pTriangles.Add(offset + 3);

      //Bottom
      offset += 4;
      pTriangles.Add(offset + 0);
      pTriangles.Add(offset + 2);
      pTriangles.Add(offset + 1);
      
      pTriangles.Add(offset + 1);
      pTriangles.Add(offset + 2);
      pTriangles.Add(offset + 3);
      
      //Right
      offset += 4;
      pTriangles.Add(offset + 0);
      pTriangles.Add(offset + 2);
      pTriangles.Add(offset + 1);
      
      pTriangles.Add(offset + 1);
      pTriangles.Add(offset + 2);
      pTriangles.Add(offset + 3);
      
      //Top
      offset += 4;
      pTriangles.Add(offset + 0);
      pTriangles.Add(offset + 2);
      pTriangles.Add(offset + 1);
      
      pTriangles.Add(offset + 1);
      pTriangles.Add(offset + 2);
      pTriangles.Add(offset + 3);


      mAxisBackgroundMesh.vertices = pVertices.ToArray();
      mAxisBackgroundMesh.uv = pUvs.ToArray();
      mAxisBackgroundMesh.triangles = pTriangles.ToArray();
      
      mAxisBackgroundMeshRenderer.material = AxisMaterial;
      mAxisBackgroundMeshRenderer.material.SetColor("_TintColor", MarginBackgroundColor);
   }
}
