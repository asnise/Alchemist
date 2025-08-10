using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPartical : MonoBehaviour
{
    public GameObject[] partical;

    public void PlayPartical(int index)
    {
        GameObject partical_ = Instantiate(partical[index], transform.position, Quaternion.identity);
        Destroy(partical_, 5f);
    }
}
