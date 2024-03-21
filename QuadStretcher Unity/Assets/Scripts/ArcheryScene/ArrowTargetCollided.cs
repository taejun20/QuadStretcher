/*
* Writer: Taejun Kim (https://taejunkim.com/), Youngbo Aram Shim, HCI Lab KAIST
* Last Update: 2024. 3. 20
* QuadStretcher: A Forearm-Worn Skin Stretch Display for Bare-Hand Interaction in AR/VR (ACM CHI 2024)
* ACM CHI 24': Conference on Human Factors in Computing Systems.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTargetCollided : MonoBehaviour
{
    [SerializeField]
    Transform arrow, arrowOrigin;
    [SerializeField]
    GameObject target;    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Arrow")
        {
            arrow.GetComponent<Rigidbody>().isKinematic = true;
            StartCoroutine(makeTarget());
        }
    }

    IEnumerator makeTarget()
    {
        yield return new WaitForSeconds(0.5f);
        prepareTarget();
        arrow.gameObject.SetActive(false);
    }

    public void prepareTarget()
    {
        System.Random rand = new System.Random();
        target.SetActive(true);
        target.transform.position = arrowOrigin.position + new Vector3((float)rand.Next(-5, 5), 6f, 10f);
    }
}
