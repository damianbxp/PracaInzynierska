using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class GenTest : MonoBehaviour
{
	public UIManager uiManager;

	[Header("Init Settings")]
	public int numChunks = 4;

	public int numPointsPerAxis = 10;
	public float boundsSize = 10;
	public float isoLevel = 0f;
	public bool useFlatShading;

	[Header("Block")]
	public float width = 0.1f;
	public float lenght = 0.1f;
	public float height = 0.1f;
	public Vector3 blockPosition;


	[Header("References")]
	public ComputeShader meshCompute;
	public ComputeShader densityCompute;
	public ComputeShader editCompute;
	public Material material;

	// Private
	ComputeBuffer triangleBuffer;
	ComputeBuffer triCountBuffer;
	[HideInInspector] public RenderTexture rawDensityTexture;
	[HideInInspector] public RenderTexture processedDensityTexture;
	Chunk[] chunks;

	VertexData[] vertexDataArray;

	int totalVerts;

	// Stopwatches
	System.Diagnostics.Stopwatch timer_fetchVertexData;
	System.Diagnostics.Stopwatch timer_processVertexData;
	RenderTexture originalMap;

	void Start()
	{
		uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

		blockPosition += new Vector3(0, height / 2, 0);

		SetBoundingBox();

		InitTextures();
		CreateBuffers();

		CreateChunks();

		var sw = System.Diagnostics.Stopwatch.StartNew();
		GenerateAllChunks();
		Debug.Log("Generation Time: " + sw.ElapsedMilliseconds + " ms");

		ComputeHelper.CreateRenderTexture3D(ref originalMap, processedDensityTexture);
		ComputeHelper.CopyRenderTexture3D(processedDensityTexture, originalMap);
		CorrectPosition();
	}

	void CorrectPosition() {
		transform.position = blockPosition;
	}

	void SetBoundingBox() {
		boundsSize = Mathf.Max(width, lenght, height);
    }

	void InitTextures()
	{

		// Explanation of texture size:
		// Each pixel maps to one point.
		// Each chunk has "numPointsPerAxis" points along each axis
		// The last points of each chunk overlap in space with the first points of the next chunk
		// Therefore we need one fewer pixel than points for each added chunk
		int size = numChunks * (numPointsPerAxis - 1) + 1;
		Create3DTexture(ref rawDensityTexture, size, "Raw Density Texture");
		Create3DTexture(ref processedDensityTexture, size, "Processed Density Texture");


		processedDensityTexture = rawDensityTexture;

		// Set textures on compute shaders
		densityCompute.SetTexture(0, "DensityTexture", rawDensityTexture);
		editCompute.SetTexture(0, "EditTexture", rawDensityTexture);
		meshCompute.SetTexture(0, "DensityTexture", rawDensityTexture);
	}

	void GenerateAllChunks()
	{
		SetBoundingBox();

		// Create timers:
		timer_fetchVertexData = new System.Diagnostics.Stopwatch();
		timer_processVertexData = new System.Diagnostics.Stopwatch();

		totalVerts = 0;
		ComputeDensity();


		for (int i = 0; i < chunks.Length; i++)
		{
			GenerateChunk(chunks[i]);
		}
		Debug.Log("Total verts " + totalVerts);

		// Print timers:
		Debug.Log("Fetch vertex data: " + timer_fetchVertexData.ElapsedMilliseconds + " ms");
		Debug.Log("Process vertex data: " + timer_processVertexData.ElapsedMilliseconds + " ms");
		Debug.Log("Sum: " + (timer_fetchVertexData.ElapsedMilliseconds + timer_processVertexData.ElapsedMilliseconds));

		CorrectPosition();
	}

	void ComputeDensity()
	{
		// Get points (each point is a vector4: xyz = position, w = density)
		int textureSize = rawDensityTexture.width;

		densityCompute.SetInt("textureSize", textureSize);

		densityCompute.SetFloat("planetSize", boundsSize);
		densityCompute.SetFloat("width", width);
		densityCompute.SetFloat("lenght", lenght);
		densityCompute.SetFloat("height", height);

		ComputeHelper.Dispatch(densityCompute, textureSize, textureSize, textureSize);

	}

	void GenerateChunk(Chunk chunk)
	{


		// Marching cubes
		int numVoxelsPerAxis = numPointsPerAxis - 1;
		int marchKernel = 0;


		meshCompute.SetInt("textureSize", processedDensityTexture.width);
		meshCompute.SetInt("numPointsPerAxis", numPointsPerAxis);
		meshCompute.SetFloat("isoLevel", isoLevel);
		meshCompute.SetFloat("planetSize", boundsSize);
		triangleBuffer.SetCounterValue(0);
		meshCompute.SetBuffer(marchKernel, "triangles", triangleBuffer);

		Vector3 chunkCoord = (Vector3)chunk.id * (numPointsPerAxis - 1);
		meshCompute.SetVector("chunkCoord", chunkCoord);

		ComputeHelper.Dispatch(meshCompute, numVoxelsPerAxis, numVoxelsPerAxis, numVoxelsPerAxis, marchKernel);

		// Create mesh
		int[] vertexCountData = new int[1];
		triCountBuffer.SetData(vertexCountData);
		ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);

		timer_fetchVertexData.Start();
		triCountBuffer.GetData(vertexCountData);

		int numVertices = vertexCountData[0] * 3;

		// Fetch vertex data from GPU

		triangleBuffer.GetData(vertexDataArray, 0, 0, numVertices);
		uiManager.UpdateVertices(numVertices);

		timer_fetchVertexData.Stop();

		//CreateMesh(vertices);
		timer_processVertexData.Start();
		chunk.CreateMesh(vertexDataArray, numVertices, useFlatShading);
		timer_processVertexData.Stop();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			Debug.Log("Generate");
			GenerateAllChunks();
		}
	}



	void CreateBuffers()
	{
		int numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
		int numVoxelsPerAxis = numPointsPerAxis - 1;
		int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
		int maxTriangleCount = numVoxels * 5;
		int maxVertexCount = maxTriangleCount * 3;

		triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
		triangleBuffer = new ComputeBuffer(maxVertexCount, ComputeHelper.GetStride<VertexData>(), ComputeBufferType.Append);
		vertexDataArray = new VertexData[maxVertexCount];
	}

	void ReleaseBuffers()
	{
		ComputeHelper.Release(triangleBuffer, triCountBuffer);
	}

	void OnDestroy()
	{
		ReleaseBuffers();
		foreach (Chunk chunk in chunks)
		{
			chunk.Release();
		}
	}


	void CreateChunks()
	{
		chunks = new Chunk[numChunks * numChunks * numChunks];
		float chunkSize = (boundsSize) / numChunks;
		int i = 0;

		for (int y = 0; y < numChunks; y++)
		{
			for (int x = 0; x < numChunks; x++)
			{
				for (int z = 0; z < numChunks; z++)
				{
					Vector3Int coord = new Vector3Int(x, y, z);
					float posX = (-(numChunks - 1f) / 2 + x) * chunkSize;
					float posY = (-(numChunks - 1f) / 2 + y) * chunkSize;
					float posZ = (-(numChunks - 1f) / 2 + z) * chunkSize;
					Vector3 centre = new Vector3(posX, posY, posZ);

					GameObject meshHolder = new GameObject($"Chunk ({x}, {y}, {z})");
					meshHolder.transform.parent = transform;
					meshHolder.layer = gameObject.layer;

					Chunk chunk = new Chunk(coord, centre, chunkSize, numPointsPerAxis, meshHolder);
					chunk.SetMaterial(material);
					chunks[i] = chunk;
					i++;
				}
			}
		}
	}


	public void Terraform(Vector3 toolPos, float toolRadius, float toolHeight)
	{
		//uiManager.UpdateToolPosition(toolPos);
		toolPos -= blockPosition;
		int editTextureSize = rawDensityTexture.width;
		float editPixelWorldSize = boundsSize / editTextureSize;
		int editRadius = Mathf.CeilToInt(toolRadius / editPixelWorldSize);

		float tx = Mathf.Clamp01((toolPos.x + boundsSize / 2) / boundsSize);
		float ty = Mathf.Clamp01((toolPos.y + boundsSize / 2) / boundsSize);
		float tz = Mathf.Clamp01((toolPos.z + boundsSize / 2) / boundsSize);

		int editX = Mathf.RoundToInt(tx * (editTextureSize - 1));
		int editY = Mathf.RoundToInt(ty * (editTextureSize - 1));
		int editZ = Mathf.RoundToInt(tz * (editTextureSize - 1));


		editCompute.SetInts("toolCentre", editX, editY, editZ);
		editCompute.SetInt("toolHeight", Mathf.CeilToInt(toolHeight / editPixelWorldSize));
		editCompute.SetInt("toolRadius", Mathf.CeilToInt(toolRadius / editPixelWorldSize));

		ComputeHelper.Dispatch(editCompute, editTextureSize, editTextureSize, editTextureSize);


        //ComputeHelper.CopyRenderTexture3D(originalMap, processedDensityTexture);

        //float worldRadius = (editRadius + 1 + ((blurMap) ? blurRadius : 0)) * editPixelWorldSize;
        float worldRadius = ( editRadius + 1 );
        for(int i = 0; i < chunks.Length; i++) {
            Chunk chunk = chunks[i];
            GenerateChunk(chunk);
            if(MathUtility.SphereIntersectsBox(toolPos, worldRadius, chunk.centre, Vector3.one * chunk.size)) {

                chunk.terra = true;

            }
        }
    }

	void Create3DTexture(ref RenderTexture texture, int size, string name)
	{
		//
		var format = UnityEngine.Experimental.Rendering.GraphicsFormat.R32_SFloat;
		if (texture == null || !texture.IsCreated() || texture.width != size || texture.height != size || texture.volumeDepth != size || texture.graphicsFormat != format)
		{
			//Debug.Log ("Create tex: update noise: " + updateNoise);
			if (texture != null)
			{
				texture.Release();
			}
			const int numBitsInDepthBuffer = 0;
			texture = new RenderTexture(size, size, numBitsInDepthBuffer);
			texture.graphicsFormat = format;
			texture.volumeDepth = size;
			texture.enableRandomWrite = true;
			texture.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;


			texture.Create();
		}
		texture.wrapMode = TextureWrapMode.Repeat;
		texture.filterMode = FilterMode.Bilinear;
		texture.name = name;
	}



}