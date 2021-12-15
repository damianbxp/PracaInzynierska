using UnityEngine;

public class BlockGen : MonoBehaviour
{
	public UIManager uiManager;

	[Header("Gen Settings")]
	public int numChunks = 4;

	public int numPointsPerAxis = 10;
	float boundsSize = 10;
	public float isoLevel = 0f;

	float width = 0.1f;
	float lenght = 0.1f;
	float height = 0.1f;
	Vector3 blockPosition;

	public RobotMaster robotMaster;

	[Header("References")]
	public ComputeShader marchingCubesCompute;
	public ComputeShader blockMapCompute;
	public ComputeShader editCompute;
	public Material material;

	ComputeBuffer triangleBuffer;
	ComputeBuffer triCountBuffer;
	[HideInInspector] public RenderTexture rawDensityTexture;
	[HideInInspector] public RenderTexture processedDensityTexture;
	Chunk[] chunks;

	VertexData[] vertexDataArray;

	int totalVerts;

	System.Diagnostics.Stopwatch timer_fetchVertexData;
	System.Diagnostics.Stopwatch timer_processVertexData;
	RenderTexture originalMap;

	void Start()
	{
		
	}

	public void GenerateBlock(Vector3 pos, float width_, float lenght_, float height_) {
        for(int i = 0; i < transform.childCount; i++) {
			Destroy(transform.GetChild(i).gameObject);
        }
		robotMaster.homePoint = pos;

		width = width_ / 1000;
		lenght = lenght_ / 1000;
		height = height_ / 1000;

		blockPosition = pos - new Vector3(0, height / 2, 0);

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
        for(int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).transform.position = blockPosition;
        }
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
		blockMapCompute.SetTexture(0, "DensityTexture", rawDensityTexture);
		editCompute.SetTexture(0, "EditTexture", rawDensityTexture);
		marchingCubesCompute.SetTexture(0, "DensityTexture", rawDensityTexture);
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

		blockMapCompute.SetInt("textureSize", textureSize);

		blockMapCompute.SetFloat("planetSize", boundsSize);
		blockMapCompute.SetFloat("width", width);
		blockMapCompute.SetFloat("lenght", lenght);
		blockMapCompute.SetFloat("height", height);

		ComputeHelper.Dispatch(blockMapCompute, textureSize, textureSize, textureSize);

	}

	public void UpdateChunks() {
        foreach(var chunk in chunks) {
			GenerateChunk(chunk);
        }
    }

	void GenerateChunk(Chunk chunk)
	{


		// Marching cubes
		int numVoxelsPerAxis = numPointsPerAxis - 1;
		int marchKernel = 0;


		marchingCubesCompute.SetInt("textureSize", processedDensityTexture.width);
		marchingCubesCompute.SetInt("numPointsPerAxis", numPointsPerAxis);
		marchingCubesCompute.SetFloat("isoLevel", isoLevel);
		marchingCubesCompute.SetFloat("planetSize", boundsSize);
		triangleBuffer.SetCounterValue(0);
		marchingCubesCompute.SetBuffer(marchKernel, "triangles", triangleBuffer);

		Vector3 chunkCoord = (Vector3)chunk.id * (numPointsPerAxis - 1);
		marchingCubesCompute.SetVector("chunkCoord", chunkCoord);

		ComputeHelper.Dispatch(marchingCubesCompute, numVoxelsPerAxis, numVoxelsPerAxis, numVoxelsPerAxis, marchKernel);

		// Create mesh
		int[] vertexCountData = new int[1];
		triCountBuffer.SetData(vertexCountData);
		ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);

		timer_fetchVertexData.Start();
		triCountBuffer.GetData(vertexCountData);

		int numVertices = vertexCountData[0] * 3;

		// Fetch vertex data from GPU

		triangleBuffer.GetData(vertexDataArray, 0, 0, numVertices);
		//uiManager.UpdateVertices(numVertices);

		timer_fetchVertexData.Stop();

		//CreateMesh(vertices);
		timer_processVertexData.Start();
		chunk.CreateMesh(vertexDataArray, numVertices);
		timer_processVertexData.Stop();
	}


	void CreateBuffers()
	{
		//int numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
		int numVoxelsPerAxis = numPointsPerAxis - 1;
		int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
		int maxTriangleCount = numVoxels * 5;
		int maxVertexCount = maxTriangleCount * 3;

		if(triangleBuffer!=null) triangleBuffer.Release();//<-
		if(triCountBuffer!=null) triCountBuffer.Release();//<-
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
		if(chunks != null) {
			foreach(Chunk chunk in chunks) {
				chunk.Release();
			}
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


	public void ModifyBlock(Transform toolTransform, float toolRadius, float toolHeight, float toolAngle)
	{
		Vector3 toolPos = toolTransform.position - blockPosition;
		int editTextureSize = rawDensityTexture.width;
		float editPixelWorldSize = boundsSize / editTextureSize;
		int editRadius = Mathf.CeilToInt(toolRadius / editPixelWorldSize);

		float tx = Mathf.Clamp01((toolPos.x + boundsSize / 2) / boundsSize);
		float ty = Mathf.Clamp01((toolPos.y + boundsSize / 2) / boundsSize);
		float tz = Mathf.Clamp01((toolPos.z + boundsSize / 2) / boundsSize);

		int editX = Mathf.RoundToInt(tx * (editTextureSize - 1));
		int editY = Mathf.RoundToInt(ty * (editTextureSize - 1));
		int editZ = Mathf.RoundToInt(tz * (editTextureSize - 1));

		Vector3 toolEndPos =toolPos - toolTransform.up * toolHeight;

		float tEndx = Mathf.Clamp01(( toolEndPos.x + boundsSize / 2 ) / boundsSize);
		float tEndy = Mathf.Clamp01(( toolEndPos.y + boundsSize / 2 ) / boundsSize);
		float tEndz = Mathf.Clamp01(( toolEndPos.z + boundsSize / 2 ) / boundsSize);

		int editEndX = Mathf.RoundToInt(tEndx * ( editTextureSize - 1 ));
		int editEndY = Mathf.RoundToInt(tEndy * ( editTextureSize - 1 ));
		int editEndZ = Mathf.RoundToInt(tEndz * ( editTextureSize - 1 ));


		editCompute.SetInts("toolCenter", editX, editY, editZ);
		editCompute.SetInts("toolEnd",editEndX, editEndY, editEndZ);
		editCompute.SetInt("toolHeight", Mathf.CeilToInt(toolHeight / editPixelWorldSize));
		editCompute.SetInt("toolRadius", Mathf.CeilToInt(toolRadius / editPixelWorldSize));
		editCompute.SetFloat("toolAngleCos", -Mathf.Cos(Mathf.Deg2Rad * toolAngle));
		ComputeHelper.Dispatch(editCompute, editTextureSize, editTextureSize, editTextureSize);

		//Transform debugSphere = GameObject.Find("DebugSphere").transform;
		//debugSphere.position = blockPosition + toolPos + toolTransform.up * toolHeight/2;
		//debugSphere.localScale = Vector3.one * toolHeight;

		for(int i = 0; i < chunks.Length; i++) {
            Chunk chunk = chunks[i];
            if(MathUtility.SphereIntersectsBox(toolPos + toolTransform.up * toolHeight / 2, toolHeight/1.5f, chunk.centre, Vector3.one * chunk.size)) {
                chunk.terra = true;
                GenerateChunk(chunk);
            }
        }
    }

	void Create3DTexture(ref RenderTexture texture, int size, string name)
	{
		var format = UnityEngine.Experimental.Rendering.GraphicsFormat.R32_SFloat;
		if (texture == null || !texture.IsCreated() || texture.width != size || texture.height != size || texture.volumeDepth != size || texture.graphicsFormat != format)
		{
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