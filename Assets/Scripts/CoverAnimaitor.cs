using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class CoverAnimaitor : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private float speed = 0.5f;

    //--------------------------------------------------------------------------------------------

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            parent.gameObject.SetActive(true);
            StartCoroutine(DisableChildren());
        }
    }

    private IEnumerator DisableChildren()
    {

        foreach (Transform child in parent) child.gameObject.SetActive(true);

        foreach (Transform child in parent)
        {
            child.gameObject.SetActive(false);
            yield return new WaitForSeconds(speed);
        }

        parent.gameObject.SetActive(false);
    }
}
