using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using SergeiStarovoitov;

public class PrefabsSpawner : MonoBehaviour
{
    [SerializeField] GameObject mainCube;
    [SerializeField] GameObject parent;
    [SerializeField] GameObject prefabToSpawn;
    [SerializeField] int prefabNum = 10;
    [SerializeField] float spawnDelay = 10;
    [SerializeField] Pooling pooling;

    private UnityEngine.Pool.ObjectPool<GameObject> pool;

    private void Start()
    {
        pool = new UnityEngine.Pool.ObjectPool<GameObject>(() =>
        {
            return Instantiate(prefabToSpawn);
        }, go =>
        {
            gameObject.SetActive(true);
        }, go =>
        {
            gameObject.SetActive(false);
        }, go =>
        {
            Destroy(go);
        }, false, 10, 10000);

            
        StartCoroutine(WaitBeforeSpawn());
    }

    private void SpawnPrefab()
    {
        GameObject cube = null;
        switch (pooling)
        {
            case Pooling.NO: cube = Instantiate(prefabToSpawn, mainCube.transform.position, Random.rotation); break;
            case Pooling.BUILTIN: cube = pool.Get(); break;
            case Pooling.CUSTOM: cube = PoolManager.SpawnObject(prefabToSpawn, mainCube.transform.position, Random.rotation); break;
        }
        
        cube.transform.position = GetRandomPointInsideBoxCollider(mainCube.GetComponent<BoxCollider>());
        cube.transform.SetParent(parent.transform);
        if (cube.GetComponent<PrefabController>())
        {
            cube.GetComponent<PrefabController>().SetConditions();
            cube.GetComponent<PrefabController>().Init(DestroyPrefab);
        }
    }

    private void DestroyPrefab(PrefabController objToDestroy)
    {
        switch (pooling)
        {
            case Pooling.NO: Destroy(objToDestroy.gameObject); break;
            case Pooling.BUILTIN: pool.Release(objToDestroy.gameObject);  break;
            case Pooling.CUSTOM: PoolManager.ReleaseObject(objToDestroy.gameObject); break;
        }
        SpawnPrefab();
    }

    IEnumerator WaitBeforeSpawn()
    {
        yield return new WaitForSeconds(spawnDelay);
        for (int i = 0; i < prefabNum; i++)
        {
            SpawnPrefab();
        }
    }
    public Vector3 GetRandomPointInsideBoxCollider(BoxCollider boxCollider)
    {
        Vector3 point = new Vector3(
            Random.Range(-boxCollider.size.x / 2, boxCollider.size.x / 2),
            Random.Range(-boxCollider.size.y / 2, boxCollider.size.y / 2),
            Random.Range(-boxCollider.size.z / 2, boxCollider.size.z / 2)
        ) + boxCollider.center;
        return boxCollider.transform.TransformPoint(point);
    }

    public enum Pooling
    {
        NO,
        BUILTIN,
        CUSTOM
    }
}
