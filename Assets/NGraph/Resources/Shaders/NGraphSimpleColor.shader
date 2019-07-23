Shader "NGraph/SimpleColor"
{
   Properties
   {
      _TintColor ("Tint Color", Color) = (1,1,1,1)
   }
   
   SubShader
   {
      Lighting Off
      ZWrite Off
      Cull Back
      Blend SrcAlpha OneMinusSrcAlpha
      Tags {"Queue" = "Transparent"}
      Color[_TintColor]
      Pass { }
   } 
   FallBack "Unlit/Transparent"
}