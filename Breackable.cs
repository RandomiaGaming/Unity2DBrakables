using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.Events;
using UnityEditor;
[RequireComponent(typeof(SpriteRenderer))]
public class Breackable : MonoBehaviour
{
    public GameObject BreakPartical;
    public int PixelsPerUnit = 1;
    public Texture2D BreakMap;
    public float ForceToBreak;
    public UnityEvent OnBreak;
    private Texture2D Sprite;
    public float Lifetime = 1;
    private void Start()
    {
        Sprite = GetComponent<SpriteRenderer>().sprite.texture;
    }

    public void Break()
    {
        
        if (Sprite.width != BreakMap.width || Sprite.height != BreakMap.height)
        {
            Debug.LogError("Sprite and BreakMap are not the same size on " + gameObject.name);
        }
        List<Color> colors = foreachpixel(BreakMap);
        List<Texture2D> chunks;
        chunks = BreackIntoChunks(BreakMap, Sprite, colors);
        
        foreach (Texture2D img in chunks)
        {
            GameObject peice = Instantiate(BreakPartical, transform.position, transform.rotation);
            Sprite Reference = GetComponent<SpriteRenderer>().sprite;
            Sprite newsprite = UnityEngine.Sprite.Create(img, Reference.rect, /*Reference.pivot*/ new Vector2(0, 0), Reference.pixelsPerUnit);
            newsprite.name = "Peice";
            
            peice.GetComponent<SpriteRenderer>().sprite = newsprite;
            peice.AddComponent<PolygonCollider2D>();

        }
        OnBreak.Invoke();
        Destroy(gameObject);
    }

    List<Texture2D> BreackIntoChunks(Texture2D BreackMap, Texture2D Original, List<Color> colors)
    {
        List<Texture2D> output = new List<Texture2D>();
        foreach (Color current in colors) {
            Texture2D texture = new Texture2D(BreackMap.width, BreackMap.height);
            for (int height = 0; height < BreackMap.height; height++)
            {
                for (int width = 0; width < BreackMap.width; width++)
                {
                    if(BreackMap.GetPixel(width, height) == current)
                    {
                        
                        texture.SetPixel(width, height, Original.GetPixel(width, height));
                    }
                    else
                    {
                        texture.SetPixel(width, height, new Color(0, 0, 0, 0));
                    }
                }
            }
            texture.filterMode = Sprite.filterMode;
            texture.Apply();
            output.Add(texture);
        }
        return output;
        
    }
    List<Color> foreachpixel(Texture2D Image)
    {
        List<Color> output = new List<Color>();
        for (int height = 0; height < Image.height; height++)
        {
            for (int width = 0; width < Image.width; width++)
            {
                if (!output.Contains(Image.GetPixel(width, height)) && Image.GetPixel(width, height).a == 1)
                {
                    output.Add(Image.GetPixel(width, height));
                }
                
            }
        }
        return output;
    }
    private void OnDrawGizmos()
    {
        if (ForceToBreak > 0 && GetComponent<Rigidbody2D>() == null)
        {
            Debug.LogError("ForceToBreak is greater than 0 but no Rigidbody2D component was added!");
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (ForceToBreak > 0)
        {
            if (GetComponent<Rigidbody2D>() != null)
            {
                if (Mathf.Abs(collision.relativeVelocity.x) + Mathf.Abs(collision.relativeVelocity.y) > ForceToBreak)
                {
                    Break();
                }
            }
        }
    }

}
