using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] GameObject _texture;
    private MeshRenderer _myRend;

    protected void Start()
    {
        _myRend = _texture.GetComponent<MeshRenderer>();
    }

    public void SetOnTile(Character character)
    {
        character.transform.position = transform.position;
    }

    public void ChangeTileTexture(Tile newTexture)
    {
        var tile = Instantiate(newTexture);
        tile.gameObject.SetActive(false);
        Material[] materials = _myRend.materials;
        MeshRenderer tmp = tile._texture.GetComponent<MeshRenderer>();
        Material[] tmpmat = tmp.materials;
        //materials[0] = newTexture._texture.GetComponent<MeshRenderer>().materials;
        _myRend.materials = tmpmat;
    }
}