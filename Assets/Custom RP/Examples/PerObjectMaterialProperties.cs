using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour {

	static int baseColorId = Shader.PropertyToID("_BaseColor");
	private static MaterialPropertyBlock block;

	[SerializeField] private Color baseColor = Color.white;

	private void OnValidate() {
		if(block == null) {
			block = new MaterialPropertyBlock();
		}
		block.SetColor(baseColorId, baseColor);
		GetComponent<Renderer>().SetPropertyBlock(block);
	}

	private void Awake() {
		OnValidate();
	}
}