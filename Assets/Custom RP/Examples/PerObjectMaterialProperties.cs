using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour {

	static int baseColorId = Shader.PropertyToID("_BaseColor");
	static int cutoffId = Shader.PropertyToID("_Cutoff");

	private static MaterialPropertyBlock block;

	[SerializeField] private Color baseColor = Color.white;
	[SerializeField, Range(0f, 1f)] private float cutoff = 0.5f;

	private void OnValidate() {
		if(block == null) {
			block = new MaterialPropertyBlock();
		}
		block.SetColor(baseColorId, baseColor);
		block.SetFloat(cutoffId, cutoff);
		GetComponent<Renderer>().SetPropertyBlock(block);
	}

	private void Awake() {
		OnValidate();
	}
}