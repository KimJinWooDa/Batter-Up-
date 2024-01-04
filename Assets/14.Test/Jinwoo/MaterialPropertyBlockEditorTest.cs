using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class MaterialPropertyBlockEditorTest : MonoBehaviour
{
    [SerializeField, Optional]
    private MaterialPropertyBlockEditor _materialBlock;
    
    [SerializeField, Optional]
    private MeshRenderer _meshRenderer;

    private static readonly int _highlightShaderID = Shader.PropertyToID("_Highlight");
    
    public bool highlight; 
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(_materialBlock != null)
            {
                highlight = !highlight;
                _materialBlock.MaterialPropertyBlock.SetFloat(_highlightShaderID, highlight ? 1f : 0f);
                _materialBlock.UpdateMaterialPropertyBlock();
            }
        }
        
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(_meshRenderer != null)
            {
                highlight = !highlight;
                MaterialPropertyBlock propBlock = new MaterialPropertyBlock();

                _meshRenderer.GetPropertyBlock(propBlock);
    
                propBlock.SetFloat(_highlightShaderID,  highlight ? 1f : 0f);

                _meshRenderer.SetPropertyBlock(propBlock);
            }
        }
        
    }
}
