
using UnityEngine;
using UnityEngine.U2D;

public class CursorController : MonoBehaviour
{

    public Sprite defaultCursor;
    public Sprite loadingCursor;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    private Texture2D cursorTexture;
    public SpriteAtlas cursorAtlas;

    void Start(){
        GameManager.Instance.changeCursor = this;

        defaultCursor = cursorAtlas.GetSprite(defaultCursor.name);
        loadingCursor = cursorAtlas.GetSprite(loadingCursor.name);
        Default();


    }

    public void Loading(){
        
        cursorTexture = new Texture2D( (int)loadingCursor.rect.width, (int)loadingCursor.rect.height );

        var pixels = loadingCursor.texture.GetPixels(  (int)loadingCursor.textureRect.x, 
                                                (int)loadingCursor.textureRect.y, 
                                                (int)loadingCursor.textureRect.width, 
                                                (int)loadingCursor.textureRect.height );
        
        cursorTexture.SetPixels( pixels );
        cursorTexture.Apply();
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    public void Default(){
        cursorTexture = new Texture2D( (int)defaultCursor.rect.width, (int)defaultCursor.rect.height );

        var pixels = defaultCursor.texture.GetPixels(  (int)defaultCursor.textureRect.x, 
                                                (int)defaultCursor.textureRect.y, 
                                                (int)defaultCursor.textureRect.width, 
                                                (int)defaultCursor.textureRect.height );

        cursorTexture.SetPixels( pixels );
        cursorTexture.Apply();
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

}