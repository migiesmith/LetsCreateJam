using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileTipController : MonoBehaviour
{

    [SerializeField] LayerMask _layerMask;
    private Image _background;
    private Text _text;

    // Use this for initialization
    void Start()
    {
        _background = GetComponentInChildren<Image>();
        _text = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, Mathf.Infinity, _layerMask, QueryTriggerInteraction.UseGlobal))
        {
            Tile tile = hit.collider.GetComponentInParent<Tile>();
            string toolTipText = tile.getToolTip();
            if (toolTipText == "")
            {
                _background.enabled = false;
                _text.enabled = false;
            }
            else
            {
                _text.text = toolTipText;
                _background.enabled = true;
                _text.enabled = true;
                this.transform.position = Input.mousePosition;
            }
        }
        else
        {
            _background.enabled = false;
            _text.enabled = false;
        }
    }


}
