using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{

    public GameObject player;
    public Material textureAtlas;
    public static int columnHeight = 16;
    public static int chunkSize = 16;
    public static int worldSize = 2;
    public static int radius = 1;
    public static Dictionary<string, Chunk> chunks;
    public Slider loadingAmount;
    public Camera cam;
    public Button playButton;

    public static string BuildChunkName(Vector3 v)
    {
        return (int)v.x + "_" +
                     (int)v.y + "_" +
                     (int)v.z;
    }

    IEnumerator BuildChunkColumn()
    {
        for (int i = 0; i < columnHeight; i++)
        {
            Vector3 chunkPosition = new Vector3(this.transform.position.x,
                                                i * chunkSize,
                                                this.transform.position.z);
            Chunk c = new Chunk(chunkPosition, textureAtlas);
            c.chunk.transform.parent = this.transform;
            chunks.Add(c.chunk.name, c);
        }

        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            c.Value.DrawChunk();
            yield return null;
        }

    }

    IEnumerator BuildWorld()
    {
        int posx = (int)Mathf.Floor(player.transform.position.x / chunkSize);
        int posz = (int)Mathf.Floor(player.transform.position.z / chunkSize);

        float totalChunks = (Mathf.Pow(radius * 2 + 1, 2) * columnHeight) * 2;
        int processCount = 0;

        for (int z = -radius; z <= radius; z++)
            for (int x = -radius; x <= radius; x++)
                for (int y = 0; y < columnHeight; y++)
                {
                    Vector3 chunkPosition = new Vector3((x + posx) * chunkSize,
                                                        y * chunkSize,
                                                        (posz + z) * chunkSize);
                    Chunk c = new Chunk(chunkPosition, textureAtlas);
                    c.chunk.transform.parent = this.transform;
                    chunks.Add(c.chunk.name, c);
                    yield return null;

                }

        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            c.Value.DrawChunk();
            processCount++;
            loadingAmount.value = processCount / totalChunks * 100;
            yield return null;
        }
        player.SetActive(true);
        loadingAmount.gameObject.SetActive(false);
        cam.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
    }

    public void startBuild()
    {
        StartCoroutine(BuildWorld());
    }

    // Use this for initialization
    void Start()
    {
        player.SetActive(false);
        chunks = new Dictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
