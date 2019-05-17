using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_Text_Controller : MonoBehaviour {

    public int pool_size;
    public GameObject damage_Text;

    private Queue<GameObject> pool;

    private void Awake()
    {
        pool = new Queue<GameObject>();
    }

    public void Creat_Animator(Vector3 world_Point, int hurt)
    {
        if (pool.Count > 0)
        {
            GameObject cache = pool.Dequeue();

            if (cache != null)
            {
                cache.transform.position = world_Point;
                cache.GetComponent<Damage_Text>().Play_Animator(hurt);
                cache.SetActive(true);
            }
        }
        else
        {
            GameObject cache = Instantiate(damage_Text, gameObject.transform);
            cache.transform.position = world_Point;
            cache.GetComponent<Damage_Text>().Play_Animator(hurt);
        }
    }

    public void Recovery_GameObject(GameObject pool_GameObject)
    {
        pool.Enqueue(pool_GameObject);
        pool_GameObject.SetActive(false);
    }
}
