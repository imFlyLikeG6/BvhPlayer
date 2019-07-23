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

public class NGraphUtils
{
   #if UNITY_EDITOR
   static public void DrawSeparator ()
   {
      GUILayout.Space(12f);
   
      if (Event.current.type == EventType.Repaint)
      {
         Texture2D tex = EditorGUIUtility.whiteTexture;
         Rect rect = GUILayoutUtility.GetLastRect();
         GUI.color = new Color(0f, 0f, 0f, 0.25f);
         GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
         GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
         GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
         GUI.color = Color.white;
      }
   }
   
   static public void RegisterUndo (string name, params Object[] objects)
   {
      if (objects != null && objects.Length > 0)
      {
         #if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
         UnityEditor.Undo.RegisterUndo(objects, name);
         #else
         UnityEditor.Undo.RecordObjects(objects, name);
         #endif
         foreach (Object obj in objects)
         {
            if (obj == null) continue;
            EditorUtility.SetDirty(obj);
         }
      }
   }
   
   static public GameObject SelectedRoot<T>() where T: Component
   {
      GameObject go = Selection.activeGameObject;
      
      // Only use active objects
      if (go != null && !GetActive(go)) go = null;
      
      // Try to find a panel
      T p = (go != null) ? FindInParents<T>(go) : null;
      
      // No selection? Try to find the root automatically
      if (p == null)
      {
         T[] panels = FindActive<T>();
         if (panels.Length > 0) go = panels[0].gameObject;
      }
      return go;
   }
   #endif
   
   static public bool GetActive(GameObject go)
   {
#if UNITY_3_5
      return go && go.active;
#else
      return go && go.activeInHierarchy;
#endif
   }
   
   static public T FindInParents<T> (GameObject go) where T : Component
   {
      if (go == null)
         return null;
      object comp = go.GetComponent<T>();
      
      if (comp == null)
      {
         Transform t = go.transform.parent;
         
         while (t != null && comp == null)
         {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
         }
      }
      return (T)comp;
   }
   
   static public T[] FindActive<T> () where T : Component
   {
#if UNITY_3_5 || UNITY_4_0
      return GameObject.FindSceneObjectsOfType(typeof(T)) as T[];
#else
      return GameObject.FindObjectsOfType(typeof(T)) as T[];
#endif
   }

   public static bool checkForNan(Vector2 v) {
     if (float.IsNaN(v.x) || float.IsNaN(v.y)) {
       return true;
     }
     return false;
   }

   public static bool checkForNan(Vector3 v) {
     if (float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z)) {
       return true;
     }
     return false;
   }

   public static bool checkForInf(Vector2 v) {
     if (float.IsInfinity(v.x) || float.IsInfinity(v.y)) {
       return true;
     }
     return false;
   }

   public static bool checkForInf(Vector3 v) {
     if (float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z)) {
       return true;
     }
     return false;
   }

   public static bool checkForNanOrInf(Vector2 v) {
     return checkForNan(v) || checkForInf(v);
   }

   public static bool checkForNanOrInf(Vector3 v) {
     return checkForNan(v) || checkForInf(v);
   }

   public static Mesh newMeshFromUiVerticies(List<UIVertex> uiVertexList) {
      Mesh m = new Mesh();
      List<Vector3> positions = new List<Vector3>(uiVertexList.Count);
      List<Vector3> normals = new List<Vector3>(uiVertexList.Count);
      List<Vector2> uvs0 = new List<Vector2>(uiVertexList.Count);
      List<Vector2> uvs1 = new List<Vector2>(uiVertexList.Count);
      List<Vector2> uvs2 = new List<Vector2>(uiVertexList.Count);
      List<Vector2> uvs3 = new List<Vector2>(uiVertexList.Count);
      foreach (UIVertex vert in uiVertexList) {
         positions.Add(vert.position);
         normals.Add(vert.normal);
         uvs0.Add(vert.uv0);
         uvs1.Add(vert.uv1);
         uvs2.Add(vert.uv2);
         uvs3.Add(vert.uv3);
      }
      m.SetVertices(positions);
      m.SetNormals(normals);
      m.SetUVs(0, uvs0);

      return m;
   }

   public static void addSegment(Vector2 pPrevDataPoint, Vector2 pDataPoint, float thickness, List<Vector3> pVertices, List<Vector2> pUvs, List<int> pTriangles, bool forUnityGui)
   {
      Vector2 line = pPrevDataPoint - pDataPoint;
      Vector2 normal = new Vector2(-line.y, line.x).normalized;
      
      Vector2 a = pPrevDataPoint - (thickness/2 * normal);
      Vector2 b = pPrevDataPoint + (thickness/2 * normal);
      Vector2 c = pDataPoint - (thickness/2 * normal);
      Vector2 d = pDataPoint + (thickness/2 * normal);

      if (checkForNanOrInf(a) || checkForNanOrInf(b) || checkForNanOrInf(c) || checkForNanOrInf(d)) {
         return;
      }

      if (forUnityGui)
      {
         // Create rectangle
         pVertices.Add (new Vector3 (d.x, d.y, 0));
         pVertices.Add (new Vector3 (b.x, b.y, 0));
         pVertices.Add (new Vector3 (a.x, a.y, 0));
         pVertices.Add (new Vector3 (c.x, c.y, 0));
         pUvs.Add (new Vector2 (0, 0));
         pUvs.Add (new Vector2 (0, 1));
         pUvs.Add (new Vector2 (1, 0));
         pUvs.Add (new Vector2 (1, 1));
      }
      else
      {
         // Create triangle 1
         pVertices.Add (new Vector3 (a.x, a.y, 0));
         pVertices.Add (new Vector3 (b.x, b.y, 0));
         pVertices.Add (new Vector3 (c.x, c.y, 0));
         pTriangles.Add (pVertices.Count - 2);
         pTriangles.Add (pVertices.Count - 3);
         pTriangles.Add (pVertices.Count - 1);
         pUvs.Add (new Vector2 (0, 0));
         pUvs.Add (new Vector2 (0, 1));
         pUvs.Add (new Vector2 (1, 1));
      
         // Create triangle 2
         pVertices.Add (new Vector3 (b.x, b.y, 0));
         pVertices.Add (new Vector3 (c.x, c.y, 0));
         pVertices.Add (new Vector3 (d.x, d.y, 0));
         pTriangles.Add (pVertices.Count - 3);
         pTriangles.Add (pVertices.Count - 2);
         pTriangles.Add (pVertices.Count - 1);
         pUvs.Add (new Vector2 (0, 1));
         pUvs.Add (new Vector2 (1, 1));
         pUvs.Add (new Vector2 (1, 0));
      }
   }
   
   public static GameObject AddGameObject(GameObject pParentGo, float zValue, string name, bool setRectTransformToFullStrech = true)
   {
      GameObject pNewGo = new GameObject(name);
      AddGameObject(pParentGo, pNewGo, zValue, setRectTransformToFullStrech);
      SortChildren(pParentGo);

      return pNewGo;
   }

   public static void SortChildren(GameObject pParentGo)
   {
      // Sort the children by z value
      List<Transform> children = new List<Transform>();
      for (int i = pParentGo.transform.childCount - 1; i >= 0; i--)
      {
         Transform child = pParentGo.transform.GetChild (i);
         children.Add(child);
      }
      children.Sort((Transform t1, Transform t2) => { return t2.transform.position.z.CompareTo(t1.transform.position.z); });
      int y = 0;
      foreach (Transform child in children)
      {
         child.SetSiblingIndex(y++);
      }
   }
   
   public static GameObject AddGameObject(GameObject pParentGo, GameObject pWillBeChildGo, float zValue, bool setRectTransformToFullStrech = true)
   {
      pWillBeChildGo.layer = pParentGo.layer;
      pWillBeChildGo.transform.SetParent(pParentGo.transform);
      pWillBeChildGo.transform.localScale = Vector3.one;
      pWillBeChildGo.transform.localPosition = new Vector3(0, 0, zValue);

      NGraph ng = pParentGo.GetComponentInParent<NGraph>();
      if (ng == null) {
        ng = pParentGo.GetComponent<NGraph>();
      }

      if (ng.UnityGui) {
        RectTransform rt = pWillBeChildGo.AddComponent<RectTransform>();
        if (setRectTransformToFullStrech) {
          rt.anchorMin = new Vector2(0, 0);
          rt.anchorMax = new Vector2(1, 1);
          rt.pivot = new Vector2(0.5f, 0.5f);
          rt.offsetMin = Vector2.zero;
          rt.offsetMax = Vector2.zero;
        }
      }
      return pWillBeChildGo;
   }
   
   public static void AddMesh(GameObject pGo, out MeshRenderer pMeshRenderer, out Mesh pMesh)
   {
      pMeshRenderer = pGo.AddComponent<MeshRenderer>();
      pMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
      pMeshRenderer.receiveShadows = false;
      
      pMesh = pGo.AddComponent<MeshFilter>().mesh;
   }

   public static CanvasRenderer AddCanvasRenderer(GameObject pGo) {
      CanvasRenderer pCanvasRenderer = pGo.GetComponent<CanvasRenderer>();
      if (pCanvasRenderer != null) {
        return pCanvasRenderer;
      }

      RectTransform pRectTransform = pGo.GetComponent<RectTransform>();
      if(pRectTransform == null)
      {
         Vector3 pos    = pGo.transform.localPosition;
         Quaternion rot = pGo.transform.localRotation;
         Vector3 scale  = pGo.transform.localScale;

         pRectTransform = pGo.AddComponent<RectTransform>();
         pRectTransform.localPosition = pos;
         pRectTransform.localRotation = rot;
         pRectTransform.localScale = scale;
      }

      pCanvasRenderer = pGo.GetComponent<CanvasRenderer>();
      if (pCanvasRenderer == null) {
        pCanvasRenderer = pGo.AddComponent<CanvasRenderer>();
      }
      return pCanvasRenderer;
   }
   
   public static void DrawRect(Rect pRect, Mesh pMesh)
   {
      pMesh.Clear();
      
      List<Vector3> pVertices  = new List<Vector3>();
      List<Vector2> pUvs  = new List<Vector2>();
      List<int> pTriangles  = new List<int>();
      
      pVertices.Add(new Vector3(pRect.xMin, pRect.yMin, 0));
      pVertices.Add(new Vector3(pRect.xMax, pRect.yMin, 0));
      pVertices.Add(new Vector3(pRect.xMin, pRect.yMax, 0));
      pVertices.Add(new Vector3(pRect.xMax, pRect.yMax, 0));
      pUvs.Add(new Vector2(0, 0));
      pUvs.Add(new Vector2(1, 0));
      pUvs.Add(new Vector2(0, 1));
      pUvs.Add(new Vector2(1, 1));
      
      pTriangles.Add(0);
      if(pRect.height < 0)
         pTriangles.Add(1);
      pTriangles.Add(2);
      if(pRect.height >= 0)
         pTriangles.Add(1);
      
      pTriangles.Add(2);
      if(pRect.height < 0)
         pTriangles.Add(1);
      pTriangles.Add(3);
      if(pRect.height >= 0)
         pTriangles.Add(1);
      
      pMesh.vertices = pVertices.ToArray();
      pMesh.uv = pUvs.ToArray();
      pMesh.triangles = pTriangles.ToArray();
   }

   public static void DrawRect(Rect pRect, CanvasRenderer pCanvasRenderer)
   {
      // don't try to draw zero width of zero height meshes
      if(pRect.xMin == pRect.xMax || pRect.yMin == pRect.yMax) {
         return;
      }

      List<UIVertex> pVertexList = new List<UIVertex>(4);
      UIVertex pUIVertex = new UIVertex();
      
      Vector3 a = new Vector3(pRect.xMin, pRect.yMin, 0);
      Vector3 b = new Vector3(pRect.xMax, pRect.yMin, 0);
      Vector3 c = new Vector3(pRect.xMax, pRect.yMax, 0);
      Vector3 d = new Vector3(pRect.xMin, pRect.yMax, 0);
      /* d     c          a      b
       *             |
       *            OR    
       *             |
       * a     b          d      c
       */
      if(a.y > d.y)
         pUIVertex.position = a;
      else
         pUIVertex.position = d;
      pUIVertex.uv0 = new Vector2(0, 0);
      pVertexList.Add(pUIVertex);

      if(a.y > d.y)
         pUIVertex.position = b;
      else
         pUIVertex.position = c;
      pUIVertex.uv0 = new Vector2(1, 0);
      pVertexList.Add(pUIVertex);

      if(a.y > d.y)
         pUIVertex.position = c;
      else
         pUIVertex.position = b;
      pUIVertex.uv0 = new Vector2(0, 1);
      pVertexList.Add(pUIVertex);

      if(a.y > d.y)
         pUIVertex.position = d;
      else
         pUIVertex.position = a;
      pUIVertex.uv0 = new Vector2(1, 1);
      pVertexList.Add(pUIVertex);

      pCanvasRenderer.SetVertices(pVertexList);
  }
}
