using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderBlack : MonoBehaviour {

	private Camera cam = null;
	public Material shadowMaterial = null;
	
//	private List<Renderer> materials = null;
//	private Color currentSunLightColor = Color.black;
//	public Color shadowColor = Color.black;
//	public Light sunLight = null;
	
	// Use this for initialization
	void Awake() {
		cam = GetComponent<Camera>();
	}
	
	void Start() {
		if (this.renderer) {
			originalMaterial = this.renderer.material;
		}
	}
	
	private Material originalMaterial = null;
	void OnWillRenderObject() {
		if(Camera.current == cam) {
			if(originalMaterial == null) {
				originalMaterial = this.renderer.material;
			}
			this.renderer.material = shadowMaterial;
		}
	}
	
	void OnRenderObject() {
		if (this.renderer) {
			this.renderer.material = originalMaterial;
		}
	}
	
//	private List<Material> textures = new List<Material>();
//	
//	public void OnPreRender()
//	{
//		int max = 0;
//		if(materials == null) {
//			materials = new List<Renderer> ();
//			if(GamePlayer.SharedInstance.CharacterModel != null) {
//				materials.AddRange (GamePlayer.SharedInstance.CharacterModel.GetComponentsInChildren<Renderer>());
//			}
//			
//			max = GameController.SharedInstance.Enemies.Count;
//			for (int i = 0; i < max; i++) {
//				Enemy e = GameController.SharedInstance.Enemies[i];
//				if(e == null)
//					continue;
//				
//				materials.AddRange (e.GetComponentsInChildren<Renderer>());
//			}
//		}
//		
//		if(sunLight != null) {
//			currentSunLightColor = sunLight.color;
//			sunLight.color = Color.black;
//		}
//		textures.Clear ();
//		
//		max = materials.Count;
//		for (int i = 0; i < max; i++) {
//			Renderer m = materials[i];
//			if(m == null)
//				continue;
//			textures.Add (m.material);
//			m.material = shadowMaterial;
//		}
//	}
//	
//	public void OnPostRender()
//	{
//		int max = materials.Count;
//		for (int i = 0; i < max; i++) {
//			Renderer m = materials[i];
//			if(m == null)
//				continue;
//			m.material = textures[i];	
//		}
//		
//		if(sunLight != null) {
//			sunLight.color = currentSunLightColor;
//		}
//	}
}
