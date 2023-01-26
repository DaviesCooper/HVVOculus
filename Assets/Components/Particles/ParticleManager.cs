using GPUInstancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleManager : MonoBehaviour
{
    public GPUInstancerPrefab particlePrefab;
    public GPUInstancerPrefabManager prefabManager;
    private List<GPUInstancerPrefab> particles = new List<GPUInstancerPrefab>();
    public Transform particleParent;

    public RawImage particleMask;
    private RenderTexture renderTexture;
    Material material;

    public GameObject crosshairs;

    public GameObject floorObj;

    public Vector3 offset;

    Vector3[] positions = new Vector3[2];

    int CurrentDest = -1;

    bool fixCamera;
    float velocity;

    int numParticles;
    float particleRadius;
    float genRadius;
    float genLength;
    float exclusionRadius;

    bool invert = false;
    float maskRadius = 0.5f;

    public void Awake()
    {
        material = new Material(Shader.Find("Unlit/Mask"));
        renderTexture = new RenderTexture((int)particleMask.rectTransform.rect.width, (int)particleMask.rectTransform.rect.height, 0, RenderTextureFormat.ARGB32);
    }
    public void Reset(
        int numP, float pRad, float genRad,
        float genLen, float exRad, float vel,
        float travLen, bool fix, bool floor,
        bool cross, bool mask, float mRad, bool inv)
    {
        numParticles = numP;
        particleRadius = pRad;
        genRadius = genRad;
        genLength = genLen;
        exclusionRadius = exRad;
        velocity = vel;

        RemoveParticles();
        AddParticles();

        SetTravelLength(travLen);
        SetShowFloor(floor);
        SetShowCrosshairs(cross);
        SetMaskRadius(mRad);
        SetShowMask(mask);
        SetInvertMask(inv);
        SetFixCamera(fix);
    }

    public void SetNumParticles(int num)
    {
        Debug.Log("number of particles set to: " + num);
        numParticles = num;
        RemoveParticles();
        AddParticles();
    }
    public void SetParticleRadius(float rad)
    {
        Debug.Log("particle radius set to: " + rad.ToString("0.00"));
        particleRadius = rad;
        RemoveParticles();
        AddParticles();
    }

    public void SetGenerationRadius(float rad)
    {
        Debug.Log("generation radius set to: " + rad.ToString("0.00"));
        genRadius = rad;
        RemoveParticles();
        AddParticles();
    }

    public void SetGenerationLength(float len)
    {
        Debug.Log("generation length set to: " + len.ToString("0.00"));
        genLength = len;
        RemoveParticles();
        AddParticles();
    }

    public void SetExclusionRadius(float rad)
    {
        Debug.Log("exclusion radius set to: " + rad.ToString("0.00"));
        exclusionRadius = rad;
        RemoveParticles();
        AddParticles();
    }

    public void SetVelocity(float vel)
    {
        Debug.Log("velocity set to: " + vel.ToString("0.00"));
        velocity = vel;
    }
    public void SetTravelLength(float len)
    {
        Debug.Log("travel length set to: " + len.ToString("0.00"));
        positions[0] = new Vector3(0, 0, (len / 2));
        positions[1] = new Vector3(0, 0, (-len / 2));
        CurrentDest = 0;
    }

    public void SetFixCamera(bool fix)
    {
        Debug.Log("Camera fix set to: " + fix);
        fixCamera = fix;
        if (fixCamera)
        {
            particleParent.localPosition = Vector3.zero;
            particleParent.transform.localRotation = Quaternion.Euler(0, 0, 0);

        }
        else
        {
            particleParent.position = Vector3.zero + offset;
            particleParent.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void SetShowFloor(bool show)
    {
        Debug.Log("show mask set to: " + show);
        floorObj.SetActive(show);
    }

    public void SetShowCrosshairs(bool cross)
    {
        Debug.Log("show crosshairs set to: " + cross);
        crosshairs.SetActive(cross);
    }

    public void SetShowMask(bool show)
    {
        Debug.Log("show mask set to: " + show);
        particleMask.gameObject.SetActive(show);
    }

    public void SetMaskRadius(float rad)
    {
        Debug.Log("mask radius set to: " + rad.ToString("0.00"));
        maskRadius = rad;
        /*Texture2D tex = */
        createTexture();//(int)particleMask.rectTransform.rect.width, (int)particleMask.rectTransform.rect.height);
        particleMask.texture = renderTexture;
    }

    public void SetInvertMask(bool invert)
    {
        Debug.Log("invert mask set to: " + invert);
        this.invert = invert;
        /*Texture2D tex = */
        createTexture();//(int)particleMask.rectTransform.rect.width, (int)particleMask.rectTransform.rect.height);
        particleMask.texture = renderTexture;
    }

    void Update()
    {
        if (fixCamera)
        {
            if (CurrentDest >= 0)
            {
                float dist = Vector3.Distance(particleParent.localPosition, positions[CurrentDest]);
                if (dist < 0.05f)
                {
                    CurrentDest = (CurrentDest + 1) % positions.Length;
                }

                particleParent.localPosition = Vector3.MoveTowards(particleParent.localPosition, positions[CurrentDest], Time.deltaTime * velocity);
                particleParent.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            if (CurrentDest >= 0)
            {
                float dist = Vector3.Distance(particleParent.position, positions[CurrentDest] + offset);
                if (dist < 0.05f)
                {
                    CurrentDest = (CurrentDest + 1) % positions.Length;
                }
                particleParent.position = Vector3.MoveTowards(particleParent.position, positions[CurrentDest] + offset, Time.deltaTime * velocity);
                particleParent.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    #region HELPERS
    private void AddParticles()
    {
        for (int i = 0; i < numParticles; i++)
        {
            GPUInstancerPrefab prefabInstance = Instantiate(particlePrefab);
            float zCoord = (Random.value * genLength) - (genLength / 2);
            Vector2 xyCoord = randomVector2();
            prefabInstance.transform.SetParent(particleParent);
            prefabInstance.transform.localPosition = new Vector3(xyCoord.x, xyCoord.y, zCoord);
            prefabInstance.transform.localScale = new Vector3(particleRadius, particleRadius, particleRadius);
            if (!prefabInstance.prefabPrototype.addRuntimeHandlerScript)
                GPUInstancerAPI.AddPrefabInstance(prefabManager, prefabInstance, true);
            particles.Add(prefabInstance);
        }
    }

    private void RemoveParticles()
    {
        while (particles.Count > 0)
        {
            if (!particles[particles.Count - 1].prefabPrototype.addRuntimeHandlerScript)
                GPUInstancerAPI.RemovePrefabInstance(prefabManager, particles[particles.Count - 1]);
            Destroy(particles[particles.Count - 1].gameObject);
            particles.RemoveAt(particles.Count - 1);
        }

    }
    private Vector2 randomVector2()
    {
        float angle = Random.Range(0f, 360f);
        float length = Random.Range(exclusionRadius, genRadius);

        return Rotate(new Vector2(length, 0), angle);
    }

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    private void createTexture()//int width, int height)
    {
        material.SetFloat("_Radius", maskRadius);
        material.SetFloat("_Invert", invert ? 1 : 0);
        Graphics.Blit(Texture2D.whiteTexture, renderTexture, material);
        //float minCoord = Mathf.Min(width, height);
        //Debug.Log(width + " : " + height);
        //// Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        //var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

        //for(int x = 0; x < width; x++)
        //{
        //    for(int y = 0; y < height; y++)
        //    {
        //        Vector2 center = new Vector2(width / 2, height / 2);
        //        Vector2 position = new Vector2(x, y);
        //        if (invert) {
        //            if (Vector2.Distance(center, position) < minCoord / 2 * maskRadius)
        //                texture.SetPixel(x, y, new Color(0, 0, 0, 0));
        //            else
        //                texture.SetPixel(x, y, Color.black);
        //        }
        //        else
        //        {
        //            if (Vector2.Distance(center, position) < minCoord / 2 * maskRadius)
        //                texture.SetPixel(x, y, Color.black);
        //            else
        //                texture.SetPixel(x, y, new Color(0, 0, 0, 0));
        //        }
        //    }
        //}
        //texture.Apply();

        //return texture;

    }

    #endregion
}
