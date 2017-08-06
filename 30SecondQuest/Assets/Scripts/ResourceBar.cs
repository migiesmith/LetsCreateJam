using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBar : MonoBehaviour
{

    [SerializeField] private GameObject _resPrefab;
    [SerializeField] private Vector3 _offset;

    [SerializeField] private bool _centered = false;

    void Start()
    {

    }

    public void setValue(int val)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
                Destroy(transform.GetChild(i).gameObject);
        }

        Vector3 startOffset = Vector3.zero;
        if (_centered)
        {
            startOffset -= _offset * val * 0.25f;
			startOffset.Scale(transform.lossyScale);
        }
        for (int i = 0; i < val; ++i)
        {
            GameObject res = Instantiate(_resPrefab);
            res.SetActive(true);
            res.transform.localScale = transform.lossyScale;
            res.transform.position = transform.position + new Vector3(
                                        startOffset.x + _offset.x * transform.lossyScale.x * i,
                                        startOffset.y + _offset.y * transform.lossyScale.y * i,
                                        startOffset.z + _offset.z * transform.lossyScale.z * i);
            res.transform.parent = transform;
        }
    }

    void Update()
    {

    }
}
