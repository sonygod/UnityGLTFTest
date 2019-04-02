using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using GLTF;
using GLTF.Schema;
using UnityEngine;
using UnityEngine.UI;
using UnityGLTF.Loader;

namespace UnityGLTF
{

    /// <summary>
    /// Component to load a GLTF scene with
    /// </summary>
    public class GLTFComponent : MonoBehaviour
    {
	    public Text DebugText;
        public string GLTFUri = null;
        public bool Multithreaded = true;
        public bool UseStream = false;
        public bool AppendStreamingAssets = true;
        public bool PlayAnimationOnLoad = true;

        [SerializeField]
        private bool loadOnStart = true;

        [SerializeField]
        private bool MaterialsOnly = false;

        [SerializeField]
        private int RetryCount = 10;
        [SerializeField]
        private float RetryTimeout = 2.0f;
        private int numRetries = 0;


        public int MaximumLod = 300;
        public int Timeout = 8;
        public GLTFSceneImporter.ColliderType Collider = GLTFSceneImporter.ColliderType.None;

        private AsyncCoroutineHelper asyncCoroutineHelper;

        [SerializeField]
        private Shader shaderOverride = null;

        public void SetUri(string uri)
        {
            GLTFUri = uri;
        }


        private float scale = 1;

        void Start()
        {

	        DebugText.text = "version 0.0.1\n";
	        DebugText.text += "unity gltf test sonygodx@gmail.com\n";

            Debug.Log("start");


            URLParameters.Instance.url = GLTFUri;


#if UNITY_WEBGL
               //var x = await URLParameters.Instance.RegisterOnDoneAsync();
               DebugText.text +="webgl\n";
            URLParameters.Instance.RegisterOnDone((up) =>
            {
	            DebugText.text += "RegisterOnDone \n";

                string model= URLParameters.Instance.GetValue("model");
				Debug.Log(string.Format("model={0}\n", model));
	            string type= URLParameters.Instance.GetValue("type");
	            Debug.Log(string.Format("type={0}\n", type));

                string category = URLParameters.Instance.GetValue("category");
                Debug.Log(string.Format("type={0}\n", category));

                DebugText.text += string.Format("model={0}\n",model);
				DebugText.text += string.Format("type={0}\n", type);
				DebugText.text += string.Format("category={0}\n", category);


				string scales = URLParameters.Instance.GetValue("scale");
                if (string.IsNullOrEmpty(category))
				{
					category = "sampleModels";

				}
                DebugText.text += string.Format("category={0}\n", category);
                Debug.Log(string.Format("category={0}\n", category));

                if (string.IsNullOrEmpty(type))
	            {
		            type = "glTF";

                }

                if (string.IsNullOrEmpty(scales))
                {
	                scales = "1";
                }

                scale = float.Parse(scales);
				Debug.Log("scale"+scale);


				
                Debug.Log(string.Format("type ={0}\n", type));
                DebugText.text += string.Format("type={0}\n", type);
                string hl= URLParameters.Instance.Href;
                DebugText.text += string.Format("hl={0}\n",hl);
                Debug.Log(string.Format("hl ={0}\n", hl));

                if (string.IsNullOrEmpty(hl))
                {
	                hl = GLTFUri;
                }
                var url = hl.Substring(0,hl.IndexOf("examples"));


                DebugText.text +="\n parse webgl \n"+url+"\n";
	            if (!String.IsNullOrEmpty(model) && !String.IsNullOrEmpty(type))
	            {
		            //http://127.0.0.1:2000/TechniquesWebGLTest/glTF/TechniquesWebGLTest.gltf
		            https: //cx20.github.io/gltf-test/examples/threejs/index.html?model=Duck&scale=1&type=glTF-Binary


		            var dic= new Dictionary<string, string>();

		            dic["glTF-Binary"]
			            = "glb";
		            dic["glTF"]
			            = "gltf";
		            dic["glTF-Draco"]
			            = "gltf";
		            dic["glTF-Embedded"]
			            = "gltf";
		            var fullURl= url + "/" +category+"/"+ model + "/" + type + "/" + model + "." + dic[type];


		           
                    GLTFUri = fullURl;
                    Debug.Log(string.Format("GLTFUri ={0}\n", GLTFUri));

                    DebugText.text += string.Format("fullURl={0}\n", fullURl);
                    if (DebugText != null)
		            {
			            DebugText.text += "model" + model + "\n" + "type" + type + "\n" + "\n" + url;
			            DebugText.text += "start" + "\n" + "gltfurl" + GLTFUri;
                    }
	Debug.Log("start"+ GLTFUri);
		            Start2();
                }

            });






#else

			
	        Debug.Log("url" + GLTFUri);
	        DebugText.text += "start" + "\n" + "gltfurl" + GLTFUri;
	        Start2();
#endif


        }
        private void Start2()
        {
            if (loadOnStart)
            {
                LoadModel();
            }
        }

        public void LoadNewModel()
        {
            RemoveAll();
            LoadModel();
        }

        public void RemoveAll()
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
        }

        async void LoadModel()
        {
            try
            {
                await Load();
            }
#if WINDOWS_UWP
			catch (Exception)
#else
            catch (HttpRequestException)
#endif
            {
                if (numRetries++ >= RetryCount)
                    throw;

                Debug.LogWarning("Load failed, retrying");
                await Task.Delay((int)(RetryTimeout * 1000));
                Start2();
            }
        }

        public async Task Load()
        {
            asyncCoroutineHelper = gameObject.GetComponent<AsyncCoroutineHelper>() ?? gameObject.AddComponent<AsyncCoroutineHelper>();
            GLTFSceneImporter sceneImporter = null;
            ILoader loader = null;
            try
            {
                if (UseStream)
                {
                    // Path.Combine treats paths that start with the separator character
                    // as absolute paths, ignoring the first path passed in. This removes
                    // that character to properly handle a filename written with it.
                    GLTFUri = GLTFUri.TrimStart(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
                    string fullPath;
                    if (AppendStreamingAssets)
                    {
                        fullPath = Path.Combine(Application.streamingAssetsPath, GLTFUri);
                    }
                    else
                    {
                        fullPath = GLTFUri;
                    }
                    string directoryPath = URIHelper.GetDirectoryName(fullPath);
                    loader = new FileLoader(directoryPath);
                    sceneImporter = new GLTFSceneImporter(
                        Path.GetFileName(GLTFUri),
                        loader,
                        asyncCoroutineHelper
                        );
                }
                else
                {
                    string directoryPath = URIHelper.GetDirectoryName(GLTFUri);
                    loader = new WebRequestLoader(directoryPath, asyncCoroutineHelper);

                    sceneImporter = new GLTFSceneImporter(
                        URIHelper.GetFileFromUri(new Uri(GLTFUri)),
                        loader,
                        asyncCoroutineHelper
                        );

                }

                sceneImporter.SceneParent = gameObject.transform;
                sceneImporter.Collider = Collider;
                sceneImporter.MaximumLod = MaximumLod;
                sceneImporter.Timeout = Timeout;
                sceneImporter.IsMultithreaded= Multithreaded;
                sceneImporter.CustomShaderName = shaderOverride ? shaderOverride.name : null;

                if (MaterialsOnly)
                {
                    var mat = await sceneImporter.LoadMaterialAsync(0);
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.SetParent(gameObject.transform);
                    var renderer = cube.GetComponent<Renderer>();
                    renderer.sharedMaterial = mat;
                }
                else
                {
                    await sceneImporter.LoadSceneAsync();
                }

                // Override the shaders on all materials if a shader is provided
                if (shaderOverride != null)
                {
                    Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers)
                    {
                        renderer.sharedMaterial.shader = shaderOverride;
                    }
                }

                if (PlayAnimationOnLoad)
                {
                    Animation[] animations = sceneImporter.LastLoadedScene.GetComponents<Animation>();
                    foreach (Animation anim in animations)
                    {
                        anim.Play();
                    }
                }

                sceneImporter.LastLoadedScene.GetComponent<Transform>().localScale=new Vector3(scale, scale, scale);
                DebugText.text = "";
            }
            finally
            {
                if (loader != null)
                {
                    sceneImporter?.Dispose();
                    sceneImporter = null;
                    loader = null;
                }
            }
        }
    }
}