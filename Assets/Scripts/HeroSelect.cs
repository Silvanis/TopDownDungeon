using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

public class HeroSelect : MonoBehaviour
{
    [SerializeField] private SpriteLibraryAsset hero;
    [SerializeField] private SpriteLibraryAsset heroine;
    [SerializeField] private SpriteLibraryAsset heroineBlue;
    [SerializeField] private SpriteLibraryAsset heroineRed;
    [SerializeField] private SpriteLibraryAsset link;
    [SerializeField] private SpriteLibraryAsset zelda;

    private SpriteLibrary spriteLibrary;
    private SpriteRenderer spriteRenderer;
    private List<SpriteLibraryAsset> list;
    private int currentHero = 0;
    // Start is called before the first frame update
    void Start()
    {
        spriteLibrary = gameObject.GetComponentInChildren<SpriteLibrary>();
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        spriteLibrary.spriteLibraryAsset = hero;
        list = new List<SpriteLibraryAsset>()
        {
            hero, heroine, heroineBlue, heroineRed, link, zelda
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDebugHeroSelect(InputAction.CallbackContext context)
    {
        if (context.performed) 
        {
            //spriteRenderer.enabled = false;
            currentHero++;
            if (currentHero >= list.Count)
            {
                currentHero = 0;
            }

            spriteLibrary.spriteLibraryAsset = list[currentHero];
            //spriteLibrary.RefreshSpriteResolvers();
            //spriteRenderer.enabled = true;
        }
        
    }
}
