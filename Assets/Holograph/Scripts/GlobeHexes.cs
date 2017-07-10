using UnityEngine;
using System.Collections.Generic;

public class GlobeHexes : MonoBehaviour
{
    static Material mat;
    public int hexCount;
    public int textCount;
    private Vector3[] points;
    public float hexSize;
    public float moveSpeed;
    private float[] lifeSpan;
    public float lifeSpanMin;
    public float lifeSpanMax;
    private float[] age;
    private Vector3[] hexPositions;
    private int[] textPointIndices;
    public float fadeAmt;
    public GameObject textPrefab;
    private GameObject[] textMeshes;
    private Transform cam;
    private string[] ips;
    private Color textColor;

    static void CreateMaterial()
    {
        if (!mat)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            mat.SetInt("_ZWrite", 0);
        }
    }

    void Start()
    {
        if (textCount > hexCount)
        {
            textCount = hexCount;
        }
        points = new Vector3[hexCount];
        lifeSpan = new float[hexCount];
        textPointIndices = new int[textCount];
        age = new float[hexCount];
        textMeshes = new GameObject[textCount];
        cam = Camera.main.transform;
        hexPositions = new[] { new Vector3(0.3426278f, -0.1164609f, 0.3450275f), new Vector3(0.3653983f, -0.09772599f, 0.3270068f), new Vector3(0.3847611f, -0.08917058f, 0.3066071f), new Vector3(0.3518311f, -0.1050043f, 0.3393954f), new Vector3(0.341487f, -0.111659f, 0.3477339f), new Vector3(0.3686035f, -0.09476683f, 0.3242689f), new Vector3(0.3833672f, -0.08568296f, 0.3093345f), new Vector3(0.3366631f, -0.1087514f, 0.353315f), new Vector3(0.3524265f, -0.119677f, 0.3338745f), new Vector3(0.3577789f, -0.1068146f, 0.3325435f), new Vector3(0.3567078f, -0.110305f, 0.332555f), new Vector3(0.3720041f, -0.09498817f, 0.3202981f), new Vector3(0.3828735f, -0.09210676f, 0.3080977f), new Vector3(0.3915393f, -0.09109861f, 0.2973179f), new Vector3(0.4269464f, -0.074352f, 0.2493768f), new Vector3(0.4402075f, -0.06864861f, 0.2269493f), new Vector3(0.4491915f, -0.0558368f, 0.2123891f), new Vector3(0.4295219f, -0.06343973f, 0.2479634f), new Vector3(0.4469182f, -0.0639154f, 0.2148939f), new Vector3(0.4492973f, -0.06888255f, 0.2082969f), new Vector3(0.4254771f, -0.05908471f, 0.2558863f), new Vector3(0.4242183f, -0.05649006f, 0.258549f), new Vector3(0.4271646f, -0.05252042f, 0.2545044f), new Vector3(0.3907249f, -0.05275851f, 0.3074925f), new Vector3(0.4019278f, -0.06748432f, 0.2896546f), new Vector3(0.4072418f, -0.0642778f, 0.2828823f), new Vector3(0.415595f, -0.0502978f, 0.2734057f), new Vector3(0.3847023f, -0.06957263f, 0.3117113f), new Vector3(0.4004214f, -0.02605075f, 0.2983016f), new Vector3(0.2695841f, 0.0478093f, 0.4183769f), new Vector3(0.3099171f, 0.0003678203f, 0.3923675f), new Vector3(0.3455557f, 0.03388745f, 0.3597831f), new Vector3(0.4069058f, -0.01010764f, 0.2903881f), new Vector3(0.3488109f, 0.007871032f, 0.3581462f), new Vector3(0.3626097f, 0.07267967f, 0.3365007f), new Vector3(0.3661017f, 0.1476997f, 0.3068448f), new Vector3(0.415469f, 0.04046774f, 0.2752252f), new Vector3(0.3903019f, 0.03896916f, 0.3100744f), new Vector3(0.4332199f, 0.1512741f, 0.1985899f), new Vector3(0.4218517f, 0.1298505f, 0.234906f), new Vector3(0.4532436f, 0.09703466f, 0.1874954f), new Vector3(0.008186892f, -0.3792103f, 0.3257802f), new Vector3(-0.02674335f, -0.349525f, 0.356534f), new Vector3(-0.05365149f, -0.3156599f, 0.3840324f), new Vector3(-0.02149229f, -0.3317634f, 0.3734583f), new Vector3(0.0001244247f, -0.3670867f, 0.3394824f), new Vector3(0.0003935508f, -0.3523078f, 0.3547937f), new Vector3(0.002724309f, -0.3450918f, 0.3618072f), new Vector3(-0.03018118f, -0.329603f, 0.3747677f), new Vector3(-0.02879871f, -0.3659487f, 0.3394878f), new Vector3(-0.04978651f, -0.3528845f, 0.3507059f), new Vector3(-0.07048026f, -0.3328839f, 0.3663623f), new Vector3(-0.06538302f, -0.3292601f, 0.3705574f), new Vector3(-0.07905595f, -0.3635254f, 0.3340657f), new Vector3(-0.09613585f, -0.3716517f, 0.320365f), new Vector3(-0.1135731f, -0.3835213f, 0.3000215f), new Vector3(-0.1038349f, -0.3820677f, 0.3053567f), new Vector3(-0.1168914f, -0.349167f, 0.3382598f), new Vector3(-0.09587266f, -0.3582635f, 0.3353443f), new Vector3(-0.08571544f, -0.3543178f, 0.3422155f), new Vector3(0.004810154f, -0.300395f, 0.3996747f), new Vector3(0.02583532f, -0.2607028f, 0.4258716f), new Vector3(0.02154522f, -0.2636157f, 0.4243151f), new Vector3(0.01448863f, -0.2828792f, 0.4120312f), new Vector3(0.0128661f, -0.2456669f, 0.4352957f), new Vector3(0.01900908f, -0.2929673f, 0.4047334f), new Vector3(0.01972988f, -0.308533f, 0.3929617f), new Vector3(0.02574587f, -0.2917941f, 0.4052081f), new Vector3(0.04683421f, -0.3044083f, 0.3938805f), new Vector3(0.04499422f, -0.3083692f, 0.3910034f), new Vector3(0.0253915f, -0.384784f, 0.3182721f), new Vector3(0.06114845f, -0.3915809f, 0.3048381f), new Vector3(0.05348328f, -0.3681307f, 0.3340964f), new Vector3(0.03011528f, -0.3658437f, 0.3394876f), new Vector3(0.04119839f, -0.3993877f, 0.2979819f), new Vector3(0.04689655f, -0.3713672f, 0.331494f), new Vector3(0.01354705f, -0.3839514f, 0.3199983f), new Vector3(0.001495972f, -0.3696316f, 0.3367044f), new Vector3(-0.007533297f, -0.4212008f, 0.2693202f), new Vector3(-0.04175633f, -0.4156813f, 0.2747105f), new Vector3(-0.04421841f, -0.44154f, 0.2304064f), new Vector3(-0.004599079f, -0.4466725f, 0.2246388f), new Vector3(0.0493947f, -0.4476764f, 0.2171319f), new Vector3(0.069943f, -0.4563183f, 0.1920459f), new Vector3(0.07386287f, -0.4267675f, 0.2498275f), new Vector3(0.03016566f, -0.4210308f, 0.267998f), new Vector3(-0.02263986f, -0.42495f, 0.2624964f), new Vector3(-0.1224916f, -0.4197429f, 0.2425095f), new Vector3(-0.2008246f, -0.396904f, 0.228336f), new Vector3(-0.2352111f, -0.39069f, 0.2050293f), new Vector3(-0.3301257f, -0.3668127f, 0.08041143f), new Vector3(-0.2848966f, -0.4103211f, 0.02169621f), new Vector3(-0.2605625f, -0.4260879f, -0.0235894f), new Vector3(-0.298537f, -0.4005361f, -0.02114183f), new Vector3(-0.3047215f, -0.3959154f, -0.01989028f), new Vector3(-0.4238252f, -0.0943383f, 0.247941f), new Vector3(-0.4596629f, -0.09892786f, 0.1700737f), new Vector3(-0.4612409f, -0.1154167f, 0.154708f), new Vector3(-0.4424309f, 0.006972566f, 0.2328218f), new Vector3(-0.4679064f, -0.04592334f, 0.1701573f), new Vector3(-0.4653915f, -0.1074913f, 0.1478416f), new Vector3(-0.4727246f, -0.1031448f, 0.1260768f), new Vector3(-0.4834309f, -0.07903887f, 0.1002399f), new Vector3(-0.4573521f, -0.04999948f, 0.1957825f), new Vector3(-0.4488679f, -0.03505456f, 0.2174657f), new Vector3(-0.4128154f, -0.1453003f, 0.2418029f), new Vector3(-0.4088562f, -0.1602049f, 0.2391034f), new Vector3(-0.3783211f, -0.220445f, 0.241405f), new Vector3(-0.4324669f, -0.1255257f, 0.217297f), new Vector3(-0.4509159f, -0.1291202f, 0.1732179f), new Vector3(-0.4704636f, -0.116398f, 0.1229559f), new Vector3(-0.4312181f, 0.109166f, 0.2283312f), new Vector3(-0.3751577f, 0.1848848f, 0.2739977f), new Vector3(-0.3619129f, 0.1739693f, 0.2979236f), new Vector3(-0.4112024f, 0.1855829f, 0.2155801f), new Vector3(-0.4457819f, 0.1420351f, 0.1763639f), new Vector3(-0.3949388f, 0.1994796f, 0.2328726f), new Vector3(-0.3713496f, 0.2097017f, 0.2610055f), new Vector3(-0.3877777f, 0.1775869f, 0.2609462f), new Vector3(-0.4231229f, 0.1377274f, 0.2280329f), new Vector3(-0.381427f, 0.2074126f, 0.2479842f), new Vector3(-0.3635215f, 0.1707838f, 0.2978009f), new Vector3(-0.3663827f, 0.1479844f, 0.3063763f), new Vector3(-0.407487f, 0.09127717f, 0.2749976f), new Vector3(-0.3303894f, 0.2587312f, 0.2718474f), new Vector3(-0.3030604f, 0.279718f, 0.28269f), new Vector3(-0.32118f, 0.2722497f, 0.2696756f), new Vector3(-0.31237f, 0.2781029f, 0.2740159f), new Vector3(-0.2901912f, 0.2887849f, 0.2870442f), new Vector3(-0.2781408f, 0.2962022f, 0.2913852f), new Vector3(-0.2546682f, 0.3062668f, 0.3022387f), new Vector3(-0.2412424f, 0.2896665f, 0.3284755f), new Vector3(-0.2345593f, 0.2951074f, 0.3284714f), new Vector3(-0.2554936f, 0.3033618f, 0.3044574f), new Vector3(-0.2764561f, 0.3040102f, 0.2848679f), new Vector3(-0.2859791f, 0.2971904f, 0.2826607f), new Vector3(-0.2672741f, 0.3018407f, 0.2957337f), new Vector3(-0.2424376f, 0.3029226f, 0.3153713f), new Vector3(-0.2359201f, 0.3011695f, 0.3219281f), new Vector3(-0.2670598f, 0.3062628f, 0.2913451f), new Vector3(-0.3045992f, 0.2805785f, 0.280172f), new Vector3(-0.3087029f, 0.2761979f, 0.2800324f), new Vector3(-0.3108044f, 0.2390498f, 0.3102504f), new Vector3(-0.3044209f, 0.2065127f, 0.3386455f), new Vector3(-0.3087401f, 0.1804531f, 0.349452f), new Vector3(-0.3537439f, 0.06352594f, 0.3476028f), new Vector3(-0.3910778f, 0.1100888f, 0.291442f), new Vector3(-0.2743449f, 0.2150254f, 0.3584687f), new Vector3(-0.2378609f, 0.223508f, 0.3787721f), new Vector3(-0.306252f, 0.355505f, -0.1726957f), new Vector3(-0.3535633f, 0.2963973f, -0.1927237f), new Vector3(-0.3764673f, 0.2863008f, -0.1621899f), new Vector3(-0.3749533f, 0.2279137f, -0.2397221f), new Vector3(-0.2561988f, 0.365208f, -0.2257965f), new Vector3(-0.2396333f, 0.3282221f, -0.2912872f), new Vector3(-0.2101108f, 0.3480955f, -0.2910049f), new Vector3(-0.1935612f, 0.3819235f, -0.2582014f), new Vector3(-0.2014474f, 0.3901426f, -0.2391815f), new Vector3(-0.2286137f, 0.3157438f, -0.3131174f), new Vector3(-0.2599408f, 0.3147259f, -0.2887535f), new Vector3(-0.2675029f, 0.3336723f, -0.259042f), new Vector3(-0.29678f, 0.3069657f, -0.2601789f), new Vector3(-0.3232417f, 0.2793463f, -0.2597755f), new Vector3(0.129123f, 0.1094944f, 0.4704656f), new Vector3(0.1757363f, 0.1715147f, 0.4355479f), new Vector3(0.1370786f, 0.1366663f, 0.461013f), new Vector3(0.1471565f, 0.1340194f, 0.4586748f), new Vector3(0.3285268f, 0.07516226f, 0.3693516f), new Vector3(0.3497788f, 0.1691284f, 0.3147215f), new Vector3(0.3630393f, 0.1727975f, 0.2972234f), new Vector3(0.3888392f, 0.1596217f, 0.2707891f), new Vector3(0.4010463f, 0.1435086f, 0.2618597f), new Vector3(0.419722f, 0.1278783f, 0.2397498f), new Vector3(0.4341415f, 0.1261528f, 0.2135602f), new Vector3(0.4397487f, 0.1173815f, 0.2069862f), new Vector3(0.4587728f, 0.09238124f, 0.1760481f), new Vector3(0.4619147f, 0.08034089f, 0.1737317f), new Vector3(0.4420645f, 0.1381514f, 0.1883899f), new Vector3(0.4318362f, 0.1356353f, 0.2124203f), new Vector3(0.3923459f, 0.1802524f, 0.2521432f), new Vector3(0.3699462f, 0.1889215f, 0.2782978f), new Vector3(0.3293668f, 0.2050746f, 0.3153712f), new Vector3(0.3090216f, 0.2055321f, 0.3350571f), new Vector3(0.2987038f, 0.1948455f, 0.3504454f), new Vector3(0.3372957f, 0.1449797f, 0.3394309f), new Vector3(0.3683389f, 0.1426326f, 0.306572f), new Vector3(0.3633872f, 0.1918335f, 0.2848703f), new Vector3(0.388795f, 0.1815405f, 0.2566751f), new Vector3(0.4282123f, 0.1388764f, 0.2175971f), new Vector3(0.4468344f, 0.1096072f, 0.195775f), new Vector3(0.4585726f, 0.09315698f, 0.176167f), new Vector3(0.4660781f, 0.07861265f, 0.1630667f), new Vector3(0.4766456f, 0.03343847f, 0.1472825f), new Vector3(0.431942f, 0.1059233f, 0.2284812f), new Vector3(0.4145897f, 0.1055055f, 0.2588162f), new Vector3(0.3806609f, 0.1226372f, 0.3000998f), new Vector3(0.3590087f, 0.1425285f, 0.3174875f), new Vector3(0.3627163f, 0.1607735f, 0.3042868f), new Vector3(0.3971963f, 0.1660853f, 0.2542686f), new Vector3(0.4141304f, 0.1493755f, 0.2370297f), new Vector3(0.4230693f, 0.1442733f, 0.2240499f), new Vector3(0.4041249f, 0.1837071f, 0.2300712f), new Vector3(0.4071909f, 0.1768071f, 0.2300712f), new Vector3(0.3970746f, 0.1599792f, 0.2583392f), new Vector3(0.3962948f, 0.1387199f, 0.2714936f), new Vector3(0.37624f, 0.1411327f, 0.2975274f), new Vector3(0.358413f, 0.1542885f, 0.3126294f), new Vector3(0.3309678f, 4.440546E-06f, 0.3747817f), new Vector3(0.2565628f, 0.1178588f, 0.4126568f), new Vector3(0.4011308f, 0.05470997f, 0.2934337f), new Vector3(0.3956636f, -0.06898928f, 0.2978086f), new Vector3(0.4049646f, -0.07002768f, 0.2847819f), new Vector3(0.422236f, -0.01299012f, 0.2674875f), new Vector3(0.4732133f, 0.01066464f, 0.1611028f), new Vector3(0.4866816f, -0.04048765f, 0.1072521f), new Vector3(0.4882185f, -0.1050672f, -0.02460605f), new Vector3(0.4873425f, -0.1115884f, -0.006827503f), new Vector3(0.4852118f, -0.1173001f, 0.02851117f), new Vector3(0.4820603f, -0.1208912f, 0.05482703f), new Vector3(0.4762591f, -0.1274876f, 0.08322048f), new Vector3(0.4580931f, -0.1755672f, 0.09657228f), new Vector3(0.4457096f, -0.2131075f, 0.0769949f), new Vector3(0.4343656f, -0.238683f, 0.06600416f), new Vector3(0.4653459f, -0.1555265f, 0.09626055f), new Vector3(0.4736328f, -0.1452612f, 0.06762725f), new Vector3(0.4788401f, -0.1402588f, 0.03224862f), new Vector3(0.4770476f, -0.1494344f, -0.009810597f), new Vector3(0.3282396f, -0.3409811f, -0.1612132f), new Vector3(0.3524571f, -0.3199633f, -0.1529634f), new Vector3(0.376829f, -0.2834203f, -0.1663488f), new Vector3(0.3702739f, -0.2360703f, -0.2390995f), new Vector3(0.3587898f, -0.1829505f, -0.2963043f), new Vector3(0.3554871f, -0.1755181f, -0.304673f), new Vector3(0.3961495f, -0.1996227f, -0.230689f), new Vector3(0.4043901f, -0.2331722f, -0.1791611f), new Vector3(0.3813798f, -0.2478265f, -0.2076796f), new Vector3(0.3579245f, -0.2466803f, -0.2470606f), new Vector3(0.3420251f, -0.2837174f, -0.2291771f), new Vector3(0.3636627f, -0.2973984f, -0.1711898f), new Vector3(0.4261722f, -0.2591326f, -0.03502882f), new Vector3(0.3900253f, -0.3124548f, -0.01594013f), new Vector3(0.2677525f, -0.4129844f, -0.08804852f), new Vector3(0.3123032f, -0.3886438f, -0.03772879f), new Vector3(0.2941578f, -0.3979528f, -0.07146421f), new Vector3(0.2864344f, -0.4004016f, -0.08737725f), new Vector3(0.2925378f, -0.4032865f, -0.04222205f), new Vector3(0.3252631f, -0.3732497f, -0.06992459f), new Vector3(0.347964f, -0.3432684f, -0.1053076f), new Vector3(0.3217404f, -0.3781665f, -0.05893409f), new Vector3(0.2040298f, -0.3532458f, 0.2891212f), new Vector3(-0.05662851f, -0.2510135f, 0.4287027f), new Vector3(-0.07019116f, -0.1919031f, 0.4563411f), new Vector3(-0.06710625f, -0.1930101f, 0.4563385f), new Vector3(-0.0537884f, -0.2353684f, 0.4378487f), new Vector3(-0.04354639f, -0.2331908f, 0.4401435f), new Vector3(-0.06478447f, -0.1763672f, 0.4633542f), new Vector3(-0.0679353f, -0.1751776f, 0.4633542f), new Vector3(-0.03611152f, -0.2623065f, 0.4241344f), new Vector3(-0.1113458f, -0.1713246f, 0.4563441f), new Vector3(-0.1157184f, -0.1805237f, 0.4516892f), new Vector3(-0.1282547f, -0.2474982f, 0.4150842f), new Vector3(-0.09663534f, -0.2911818f, 0.3948103f), new Vector3(-0.1015165f, -0.2895162f, 0.3948103f), new Vector3(-0.1233005f, -0.3252118f, 0.3592139f), new Vector3(-0.1437433f, -0.3310934f, 0.3460041f), new Vector3(-0.1681126f, -0.2531076f, 0.3970847f), new Vector3(-0.1714268f, -0.2472894f, 0.3993262f), new Vector3(-0.2015775f, -0.2193834f, 0.4015429f), new Vector3(-0.154433f, -0.2237754f, 0.4196146f), new Vector3(-0.1599829f, -0.2617511f, 0.3948324f), new Vector3(-0.2063567f, -0.201746f, 0.4083065f), new Vector3(-0.2243695f, -0.2213939f, 0.38813f), new Vector3(-0.2430321f, -0.2354234f, 0.3681192f), new Vector3(-0.2672704f, -0.2516837f, 0.3394436f), new Vector3(-0.1720007f, -0.2432668f, 0.4015392f), new Vector3(-0.1322918f, -0.3065742f, 0.3721695f), new Vector3(-0.04532658f, -0.3013858f, 0.3963755f), new Vector3(-0.04869238f, -0.3594542f, 0.3441228f), new Vector3(-0.2036903f, -0.3246379f, 0.3211215f), new Vector3(-0.2432827f, -0.3051072f, 0.3126077f), new Vector3(-0.3220732f, -0.2714379f, 0.2694236f), new Vector3(-0.3390309f, -0.2673999f, 0.2521011f), new Vector3(-0.3330086f, -0.3033292f, 0.2170157f), new Vector3(-0.3394957f, -0.3122225f, 0.193027f), new Vector3(-0.3754174f, -0.2919104f, 0.1544381f), new Vector3(-0.328912f, -0.3202989f, 0.1980518f), new Vector3(-0.349632f, -0.3190972f, 0.161047f), new Vector3(-0.3185827f, -0.3508188f, 0.1594647f), new Vector3(-0.2802478f, -0.3307142f, 0.2491819f), new Vector3(-0.2652033f, -0.3015168f, 0.2979212f), new Vector3(-0.3529933f, -0.2225919f, 0.2754058f), new Vector3(-0.3769716f, -0.1783416f, 0.2758406f), new Vector3(-0.3103819f, -0.1806581f, 0.3478842f), new Vector3(-0.1670273f, -0.1857307f, 0.4331341f), new Vector3(-0.2224554f, -0.1216671f, 0.4309433f), new Vector3(-0.3614202f, -0.155652f, 0.3084657f), new Vector3(-0.4089993f, -0.09407745f, 0.2717897f), new Vector3(-0.346284f, -0.06455421f, 0.3548552f), new Vector3(-0.3173908f, -0.01828808f, 0.3859119f), new Vector3(-0.2826796f, 0.03943735f, 0.4105314f), new Vector3(-0.2093929f, 0.005807608f, 0.4540076f), new Vector3(-0.3093658f, -0.04389185f, 0.3903393f), new Vector3(-0.3119556f, 0.06171152f, 0.3858424f), new Vector3(-0.3096848f, 0.1176577f, 0.3745018f), new Vector3(-0.4277432f, -0.008171335f, 0.2587866f), new Vector3(-0.4470565f, 0.08037101f, 0.2090022f), new Vector3(-0.4218232f, 0.1131269f, 0.2434499f), new Vector3(-0.3961023f, 0.1435264f, 0.2692665f), new Vector3(-0.2230951f, 0.1936607f, 0.40339f), new Vector3(-0.4655558f, 0.119505f, 0.1377589f), new Vector3(-0.4625885f, 0.1502002f, 0.1159775f), new Vector3(-0.4901535f, 0.09785708f, 0.01325893f), new Vector3(-0.486762f, 0.1141051f, 0.006697863f), new Vector3(-0.4550169f, 0.2063125f, 0.01983261f), new Vector3(-0.4525706f, 0.2124493f, -0.006691635f), new Vector3(-0.3090664f, 0.3914301f, -0.0354858f), new Vector3(-0.2933934f, 0.3983656f, -0.07228258f), new Vector3(-0.2626305f, 0.3286696f, -0.2701863f), new Vector3(-0.229584f, 0.3342936f, -0.292473f), new Vector3(-0.2257201f, 0.3642729f, -0.2576009f), new Vector3(-0.2260614f, 0.3790118f, -0.235051f), new Vector3(-0.2576422f, 0.3834322f, -0.1913071f), new Vector3(-0.2903307f, 0.3789165f, -0.1487656f), new Vector3(-0.2579138f, 0.3984146f, -0.1573085f), new Vector3(-0.2212323f, 0.4039568f, -0.1946084f), new Vector3(-0.2013598f, 0.3989359f, -0.22429f), new Vector3(-0.2019849f, 0.2833323f, 0.3590609f), new Vector3(-0.2161483f, 0.2894376f, 0.3456927f), new Vector3(-0.2172276f, 0.2833413f, 0.350045f), new Vector3(-0.3172943f, 0.2238982f, 0.3149531f), new Vector3(-0.31908f, 0.2690569f, 0.2753136f), new Vector3(-0.2932986f, 0.2239875f, 0.3373519f), new Vector3(-0.2602758f, 0.2026339f, 0.3757605f), new Vector3(-0.2319461f, 0.2266258f, 0.3805836f), new Vector3(-0.3106663f, 0.1573935f, 0.3587684f), new Vector3(-0.3494848f, 0.1198588f, 0.3368919f), new Vector3(0.1359532f, 0.3846814f, 0.2890308f), new Vector3(0.3522304f, 0.01165706f, 0.3546829f), new Vector3(0.3334689f, 0.01081422f, 0.3724002f), new Vector3(0.3790567f, -0.003785759f, 0.3260445f), new Vector3(0.385811f, -0.02119276f, 0.3173352f), new Vector3(0.3607695f, -0.04101464f, 0.3437529f), new Vector3(0.3602851f, -0.04511857f, 0.3437444f), new Vector3(0.376798f, -0.01820141f, 0.3281648f), new Vector3(0.3790596f, -0.01111606f, 0.3258671f), new Vector3(0.367567f, 0.00411737f, 0.338935f), new Vector3(0.3689814f, -0.02002105f, 0.3368251f), new Vector3(0.3760406f, 0.02953875f, 0.3282074f), new Vector3(0.3650692f, 0.04102463f, 0.3391782f), new Vector3(0.4084884f, -0.2848579f, 0.04467157f) };
        ips = new[] { "233.48.106.248", "12.140.78.80", "44.53.174.137", "79.48.235.133", "226.22.161.197", "144.140.226.192", "104.49.204.88", "151.10.226.37", "138.98.150.100", "109.61.1.62", "152.189.11.234", "155.149.139.197", "230.107.230.207", "246.21.6.65", "246.116.59.105", "71.235.51.155", "247.186.121.135", "104.35.235.230", "167.241.3.188", "117.97.56.220", "130.132.165.107", "127.34.158.170", "200.252.107.107", "44.70.19.221", "179.85.248.244", "201.21.131.63", "89.163.155.115", "23.200.186.19", "253.77.113.232", "20.184.85.197", "227.24.158.127", "15.238.111.18", "130.134.188.59", "25.164.37.94", "117.178.65.34", "59.58.45.79", "102.165.205.34", "97.236.100.149", "48.70.5.157", "243.4.109.19", "183.56.251.17", "141.192.68.118", "60.117.116.146", "176.114.219.30", "89.186.111.156", "160.51.167.16", "19.137.168.199", "122.26.10.229", "12.152.150.26", "138.235.27.222", "208.223.60.27", "154.107.76.23", "120.19.133.94", "24.70.129.35", "9.206.89.192", "209.160.220.37", "188.236.176.107", "214.203.207.81", "58.8.73.15", "20.242.114.68", "161.42.206.121", "208.16.37.178", "211.28.10.93", "39.155.232.23", "176.195.111.224", "13.194.79.54", "186.146.36.43", "119.250.168.4", "46.134.125.172", "65.47.123.195", "213.223.200.253", "13.119.150.22", "206.86.199.237", "39.222.237.10", "47.199.52.138", "56.36.234.209", "107.98.186.222", "11.215.3.208", "245.195.87.160", "22.182.47.202", "3.242.98.100", "0.122.145.33", "131.32.91.215", "28.229.216.27", "214.229.11.18", "27.92.106.246", "200.71.139.63", "206.71.135.42", "27.100.196.52", "184.131.55.153", "28.82.34.32", "178.21.203.248", "230.218.159.54", "173.179.19.32", "143.105.85.230", "193.229.236.173", "7.215.82.134", "145.168.80.26", "219.96.94.57", "137.250.132.17" };
        textColor = new Color(.91f, .322f, .322f, 1f);
        for (int i = 0; i < hexCount; ++i)
        {
            points[i] = hexPositions[Random.Range(0, hexPositions.Length)];
            lifeSpan[i] = Random.Range(lifeSpanMin, lifeSpanMax);
            age[i] = Random.Range(0f, lifeSpan[i]);
        }
        for (int i = 0; i < textCount; ++i)
        {
            textPointIndices[i] = i;
        }
        for (int i = 0; i < textCount; ++i)
        {
            int j = Random.Range(0, textCount);
            int temp = textPointIndices[i];
            textPointIndices[i] = textPointIndices[j];
            textPointIndices[j] = temp;
        }
    }
    

    void OnRenderObject()
    {
        CreateMaterial();
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        mat.SetPass(0);
        for (int i = 0; i < hexCount; ++i)
        {
            if (age[i] > lifeSpan[i])
            {
                age[i] = 0f;
                points[i] = hexPositions[Random.Range(0, hexPositions.Length)];
            }
            age[i] += Time.deltaTime;
            float newHexSize = age[i] * hexSize;
            float dToHalfLife = Mathf.Clamp(1f - Mathf.Abs(age[i] * 2f / lifeSpan[i] - 1f), 0f, 1f);
            float newRadius = .5f + Mathf.Sqrt(dToHalfLife) * moveSpeed;
            float opacity = age[i] * 2f < lifeSpan[i] ? 1f : dToHalfLife;
            points[i] = points[i].normalized * newRadius;
            Vector3 x = Vector3.Cross(points[i], new Vector3(0f, 1f, 0f)).normalized;
            Vector3 y = Vector3.Cross(points[i], x).normalized;
            Vector3[] quad1 = new Vector3[4];
            Vector3[] quad2 = new Vector3[4];
            quad1[0] = -y;
            quad1[1] = .866f * x - .5f * y;
            quad1[2] = .866f * x + .5f * y;
            quad1[3] = y;
            quad2[0] = y;
            quad2[1] = -.866f * x + .5f * y;
            quad2[2] = -.866f * x - .5f * y;
            quad2[3] = -y;

            GL.Begin(GL.QUADS);
            GL.Color(new Color(opacity, 0f, 0f, 1f) * fadeAmt);
            for (int k = 0; k < 4; ++k)
            {
                GL.Vertex(newHexSize * quad1[k] + points[i]);
            }
            for (int k = 0; k < 4; ++k)
            {
                GL.Vertex(newHexSize * quad2[k] + points[i]);
            }
            GL.End();
            GL.Begin(GL.LINES);
            GL.Color(new Color(opacity, 0f, 0f, 1f) * fadeAmt);
            GL.Vertex3(0f, 0f, 0f);
            GL.Vertex(points[i]);
            GL.End();
        }
        GL.PopMatrix();

        for (int i = 0; i < textCount; ++i)
        {
            if (textMeshes[i] == null)
            {
                textMeshes[i] = Instantiate(textPrefab, transform);
                textMeshes[i].GetComponent<TextMesh>().text = ips[textPointIndices[i]];
            }
            Transform textTransform = textMeshes[i].transform;
            textTransform.localPosition = points[textPointIndices[i]].normalized * 0.7f;
            textTransform.LookAt(cam);
            textTransform.Rotate(textTransform.up, 180f);
            textMeshes[i].GetComponent<MeshRenderer>().material.color = textColor * fadeAmt;
        }
    }
}