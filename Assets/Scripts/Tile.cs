using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] GameObject _texture;
    private MeshRenderer _myRend;
    private Material[] _originalMaterial;


    protected void Start()
    {
        _myRend = _texture.GetComponent<MeshRenderer>();
        _originalMaterial = _myRend.materials;
    }

    public void SetOnTile(Character character)
    {
        character.transform.position = transform.position;
    }

    public void ChangeTileTexture(Tile newTexture)
    {
        var tile = Instantiate(newTexture);
        tile.gameObject.SetActive(false);
        MeshRenderer tmp = tile._texture.GetComponent<MeshRenderer>();
        Material[] tmpmat = tmp.materials;
        _myRend.materials = tmpmat;
    }

    public void RevertTileToDefault()
    {
        _myRend.materials = _originalMaterial;
    }
}