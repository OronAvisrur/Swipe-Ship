using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop_Swipe_Menu : MonoBehaviour
{
    [SerializeField] private GameObject _scrollbar;
    private float scroll_pos = 0;
    private float[] positions;

    void Update()
    {
        //Get How many ships we have
        positions = new float[transform.childCount];
        float distance = 1f / (positions.Length - 1f);

        //Set the ships cards in positions
        for (int i = 0; i < positions.Length; i++) 
        {
            positions[i] = distance * i;
        }

        if (Input.GetMouseButton(0))
        {
            scroll_pos = _scrollbar.GetComponent<Scrollbar>().value;

        }
        else 
        {
            for (int i = 0; i < positions.Length; i++) 
            {
                if (scroll_pos < positions[i] + (distance / 2) && scroll_pos > positions[i] - (distance / 2)) 
                {
                    _scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(_scrollbar.GetComponent<Scrollbar>().value, positions[i], 0.1f);
                }
            }
        }

        //Set cards in location
        for (int i = 0; i < positions.Length; i++)
        {
            if (scroll_pos < positions[i] + (distance / 2) && scroll_pos > positions[i] - (distance / 2))
            {
               transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);
                for (int j = 0; j < positions.Length; j++) 
                {
                    if (j != 1) 
                    {
                        transform.GetChild(j).localScale = Vector2.Lerp(transform.GetChild(j).localScale, new Vector2(0.8f, 0.8f), 0.1f);
                    }
                }
            }
        }
    }
}
