using UnityEngine;

public class ComputeShaderTest : MonoBehaviour
{
    [Header("--Data Input & Setup--")]
    public ComputeShader computeShader;
    public RenderTexture TrailTexture;
    public RenderTexture DiffusedTexture;
    public RenderTexture ColourTexture;
    public RenderTexture finalTexture;
    public int RunSpeed;


    [Header("Data")]
    public DATA Data;

    public bool UseDataArray;
    public DATA[] DataArray;
    public int[] DataLength;

    private float runningTotalTime = 0;
    private float timer = 0;
    private int counter = 0;


    ComputeBuffer agentComputeBuffer;
    [SerializeField] AgentSetUp.SpawnMode spawnMode;

    [Header("--Input Parameters--")]
    [SerializeField] int NumberOfAgents;
    [SerializeField] float AgentMovementSpeed;
    [SerializeField] float DecayRate;
    [SerializeField] float DiffuseRate;

    [Header("--Sensor Settings--")]
    [SerializeField] bool RunSense;
    [SerializeField] float SenseOffsetDist;
    [SerializeField] float SensorSize;
    [SerializeField] float sensorAngleSpacing;
    [SerializeField] float TurnSpeed;

    [Header("--Output Dimensions--")]
    [SerializeField] int width;
    [SerializeField] int height;

    AgentSetUp.Agent[] agentsData;

    float TweenVal = 1;

    float red1 = 0;
    float red2 = 0;

    float green1 = 0;
    float green2 = 0;

    float blue1 = 0;
    float blue2 = 0;

    float alpha1 = 0;
    float alpha2 = 0;

    //data passing into the cs
    private void OnEnable()
    {
        if (UseDataArray)
        {
            counter = 0;
            runningTotalTime += DataLength[0];
            Data = DataArray[0];
        }

        Set();

        red2 = Data.red;
        green2 = Data.green;
        blue2 = Data.blue;
        alpha2 = Data.alpha;

        int totalSizeInBytes = sizeof(float) * 3;

        agentComputeBuffer = new ComputeBuffer(NumberOfAgents, totalSizeInBytes);
        agentsData = new AgentSetUp.Agent[NumberOfAgents];

        agentsData = AgentSetUp.SetUpAgents(agentsData, spawnMode, width, height);

        agentComputeBuffer.SetData(agentsData); 

        computeShader.SetBuffer(0, "agents", agentComputeBuffer);

        TweenVal = 1;
    }

    private void OnValidate()
    {
        DataChanged();
    }

    private void DataChanged()
    {
        Set();

        red1 = red2;
        green1 = green2;
        blue1 = blue2;
        alpha1 = alpha2;

        red2 = Data.red;
        green2 = Data.green;
        blue2 = Data.blue;
        alpha2 = Data.alpha;

        TweenVal = 0;
    }

    private void Set()
    {
        NumberOfAgents = Data.NumberOfAgents;
        AgentMovementSpeed = Data.AgentMovementSpeed;
        DecayRate = Data.DecayRate;
        DiffuseRate = Data.DiffuseRate;
        width = Data.width;
        height = Data.height;
        RunSense = Data.RunSense;
        SenseOffsetDist = Data.SenseOffsetDist;
        SensorSize = Data.SensorSize;
        sensorAngleSpacing = Data.sensorAngleSpacing;
        TurnSpeed = Data.TurnSpeed;
        spawnMode = Data.spawnMode;


        computeShader.SetInt("numAgents", NumberOfAgents);
        computeShader.SetFloat("moveSpeed", AgentMovementSpeed);
        computeShader.SetFloat("DecayRate", DecayRate);
        computeShader.SetFloat("DiffuseRate", DiffuseRate);
        computeShader.SetInt("width", width);
        computeShader.SetInt("height", height);
        computeShader.SetFloat("DeltaTime", Time.fixedDeltaTime);

        computeShader.SetBool("RunSense", RunSense);
        computeShader.SetFloat("SenseOffsetDist", SenseOffsetDist);
        computeShader.SetFloat("SensorSize", SensorSize);
        computeShader.SetFloat("sensorAngleSpacing", sensorAngleSpacing);
        computeShader.SetFloat("TurnSpeed", TurnSpeed);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        for (int i = 0; i < RunSpeed; i++)
        {
            RunSimulation();
        }
        Graphics.Blit(finalTexture, dest);
    }

    private void Update()
    {
        TweenVal += 0.1f * Time.deltaTime;
        timer += Time.deltaTime;
        if (TweenVal >= 1)
        {
            TweenVal = 1;
        }

        if (UseDataArray)
        {
            if (timer >= runningTotalTime)
            {
                counter += 1;
                if (counter > DataArray.Length - 1)
                {
                    runningTotalTime = float.MaxValue;
                }
                else
                {
                    runningTotalTime += DataLength[counter];
                    Data = DataArray[counter];
                    DataChanged();
                }
            }
        }
    }

    void FixedUpdate()
    {
        computeShader.SetFloat("time", Time.fixedTime);
    }

    void RunSimulation()
    {
        if (TrailTexture == null)
        {
            TrailTexture = new RenderTexture(width, height, 24)
            {
                graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat,
                enableRandomWrite = true
            };
            TrailTexture.Create();
            TrailTexture.filterMode = FilterMode.Bilinear;
        }

        if (DiffusedTexture == null)
        {
            DiffusedTexture = new RenderTexture(width, height, 24)
            {
                graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat,
                enableRandomWrite = true
            };
            DiffusedTexture.Create();
            DiffusedTexture.filterMode = FilterMode.Bilinear;
        }

        if (ColourTexture == null)
        {
            ColourTexture = new RenderTexture(width, height, 24)
            {
                graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat,
                enableRandomWrite = true
            };
            ColourTexture.Create();
            ColourTexture.filterMode = FilterMode.Bilinear;
        }

        
        computeShader.SetTexture(0, "TrailMap", TrailTexture);
        computeShader.SetTexture(1, "DiffusedTrailMap", DiffusedTexture);
        computeShader.SetTexture(1, "TrailMap", TrailTexture);
        computeShader.SetTexture(2, "InputMap", DiffusedTexture);
        computeShader.SetTexture(2, "ColouredMap", ColourTexture);

        computeShader.SetVector("x", new Vector4(Fade(red1, red2, TweenVal), 0, 0, 0));
        computeShader.SetVector("y", new Vector4(0, Fade(green1, green2, TweenVal), 0, 0));
        computeShader.SetVector("z", new Vector4(0, 0, Fade(blue1, blue2, TweenVal), 0));
        computeShader.SetVector("w", new Vector4(0, 0, 0, Fade(alpha1, alpha2, TweenVal)));

        computeShader.Dispatch(0, NumberOfAgents, 1, 1);
        computeShader.Dispatch(1, width, height, 1);
        computeShader.Dispatch(2, width, height, 1);

        CopyRenderTexture(DiffusedTexture, TrailTexture);
        CopyRenderTexture(ColourTexture, finalTexture);
    }

    // data leak clean up ------------------------------------------------

    private void OnDestroy()
    {
        agentComputeBuffer.Dispose();
    }

    //-------------------utilities----------------------------------------

    public static void CopyRenderTexture(Texture source, RenderTexture target)
    {
        Graphics.Blit(source, target);
    }

    // tween = 0 => col 1
    // tween = 1 => col 2
    public static float Fade(float col1, float col2, float tweenVal)
    {
        return (col2 - col1) * tweenVal + col1;
    }
}
