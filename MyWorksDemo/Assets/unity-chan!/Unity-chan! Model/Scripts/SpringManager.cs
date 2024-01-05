//
//SpingManager.cs for unity-chan!
//
//Original Script is here:
//ricopin / SpingManager.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
//Revised by N.Kobayashi 2014/06/24
//           Y.Ebata
//
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace UnityChan
{
	public class SpringManager : MonoBehaviour
	{
		//Kobayashi
		// DynamicRatio is paramater for activated level of dynamic animation 
		public float dynamicRatio = 1.0f;

		//Ebata
		public float			stiffnessForce;
		public AnimationCurve	stiffnessCurve;
		public float			dragForce;
		public AnimationCurve	dragCurve;
		public SpringBone[] springBones;

		void Start ()
		{
			UpdateParameters ();
		}
	
		void Update ()
		{
#if UNITY_EDITOR
		//Kobayashi
		if(dynamicRatio >= 1.0f)
			dynamicRatio = 1.0f;
		else if(dynamicRatio <= 0.0f)
			dynamicRatio = 0.0f;
		//Ebata
		UpdateParameters();
#endif
		}
	
		private void LateUpdate ()
		{
			//Kobayashi
			if (dynamicRatio != 0.0f) {
				for (int i = 0; i < springBones.Length; i++) {
					if (dynamicRatio > springBones [i].threshold) {
						springBones [i].UpdateSpring ();
					}
				}
			}
		}

		private void UpdateParameters ()
		{
			UpdateParameter ("stiffnessForce", stiffnessForce, stiffnessCurve);
			UpdateParameter ("dragForce", dragForce, dragCurve);
		}
	
		private void UpdateParameter (string fieldName, float baseValue, AnimationCurve curve)
		{
			var start = curve.keys [0].time;
			var end = curve.keys [curve.length - 1].time;
			//var step	= (end - start) / (springBones.Length - 1);
		
			var prop = springBones [0].GetType ().GetField (fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
		
			for (int i = 0; i < springBones.Length; i++) {
				//Kobayashi
				if (!springBones [i].isUseEachBoneForceSettings) {
					var scale = curve.Evaluate (start + (end - start) * i / (springBones.Length - 1));
					prop.SetValue (springBones [i], baseValue * scale);
				}
			}
		}
		[ContextMenu("绑定骨骼")]
		public void BindingBones()
		{
			Transform[] transforms = GetComponentsInChildren<Transform>();
            foreach (var item in transforms)
            {
				if (item.name.StartsWith("Q_") || item.name.StartsWith("PJ_"))
				{
					if (item.childCount > 0)
					{
						item.gameObject.AddComponent<SpringBone>();
						item.gameObject.GetComponent<SpringBone>().child = item.GetChild(0);
						item.gameObject.GetComponent<SpringBone>().boneAxis = new Vector3(0, 1, 0);
					}
				}
			}
			springBones = GetComponentsInChildren<SpringBone>();
		}
		[ContextMenu("绑定碰撞体")]
		public void BindingCollier()
		{
			Transform[] transforms = GetComponentsInChildren<Transform>();
            foreach (var item in transforms)
            {
				if (item.name.StartsWith("PJ_"))
				{
					if (item.childCount > 0)
					{
						item.gameObject.GetComponent<SpringBone>().colliders = GetComponentsInChildren<SpringCollider>(); 

                    }
				}
			}
		}
    }
}