using UnityEngine;

public class MeshBall : MonoBehaviour {

	static int baseColorId = Shader.PropertyToID("_BaseColor");

	[SerializeField] private Mesh mesh = default;
	[SerializeField] private Material material = default;

	private Matrix4x4[] matrices = new Matrix4x4[1023];
	private Vector4[] baseColors = new Vector4[1023];
	private MaterialPropertyBlock block;

	private void Awake() {
		for(int i = 0; i < matrices.Length; i++) {
			matrices[i] = Matrix4x4.TRS(
				Random.insideUnitSphere * 10f, Quaternion.identity, Vector3.one
			);

			baseColors[i] = new Vector4(1f, Random.value /2f, Random.value /2f, 1f);
		}
	}

	void Update() {
		if(block == null) {
			block = new MaterialPropertyBlock();
			block.SetVectorArray(baseColorId, baseColors);
		}
		Graphics.DrawMeshInstanced(mesh, 0, material, matrices, 1023, block);
	}
}