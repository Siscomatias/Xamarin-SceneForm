
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Google.AR.Core;
using Google.AR.Sceneform;
using Google.AR.Sceneform.Assets;
using Google.AR.Sceneform.Math;
using Google.AR.Sceneform.Rendering;
using Google.AR.Sceneform.UX;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Uri = Android.Net.Uri;

namespace HelloSceneForm
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Locked, Exported = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class HelloSceneformActivity : AppCompatActivity
    {
        private static double MIN_OPENGL_VERSION = 3.0;

        private ArFragment arFragment;
        public static ModelRenderable modeloEdificio;
        public static ModelRenderable modeloTotem;
        public static string GLTF_ASSET = "https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/Duck/glTF/Duck.gltf";
        public static string GLTF_ASSET_EDIFICIO = "https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/Duck/glTF/Duck.gltf";
        private List<AugmentedImageNodeDiccionario> augmentedImageMap = new List<AugmentedImageNodeDiccionario>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //check to see if the device supports SceneForm
            if (!CheckIsSupportedDeviceOrFinish(this))
                return;
        
            //set the content
            SetContentView(Resource.Layout.activity_ux);

            //set the fragment
            arFragment = (ArFragment)SupportFragmentManager.FindFragmentById(Resource.Id.ux_fragment);

            /*var modelo = this.Assets.Open("Totem.fbx");
            
            var testeeo = Uri.FromFile(Java.IO.File.Strea);*/           

            //load and build the model
            /*ModelRenderable.InvokeBuilder().SetSource(this, Resource.Raw.andy).Build(((renderable) =>
            {
                andyRenderable = renderable;

            }));*/

           // var tigre = Uri.Parse("https://storage.googleapis.com/ar-answers-in-search-models/static/Tiger/model.glb");

           // var tigreSource = RenderableSource.InvokeBuilder().SetSource(this, tigre, RenderableSource.SourceType.Gltf2).Build();


            /* ModelRenderable.InvokeBuilder().SetSource(this, tigre).Build(((renderable) =>
             {
                 andyRenderable = renderable;

             }));*/

            ModelRenderable.InvokeBuilder()
            .SetSource(this, RenderableSource.InvokeBuilder().SetSource(
            this,
            Uri.Parse(GLTF_ASSET),
            RenderableSource.SourceType.Gltf2)
            .SetScale(1f)
            .SetRecenterMode(RenderableSource.RecenterMode.Root)
            .Build())
            .SetRegistryId(GLTF_ASSET)
            .Build(((renderable) =>
            {
                modeloTotem = renderable;

            }));

            ModelRenderable.InvokeBuilder()
            .SetSource(this, RenderableSource.InvokeBuilder().SetSource(
            this,
            Uri.Parse(GLTF_ASSET_EDIFICIO),
            RenderableSource.SourceType.Gltf2)
            .SetScale(1f)
            .SetRecenterMode(RenderableSource.RecenterMode.Root)
            .Build())
            .SetRegistryId(GLTF_ASSET_EDIFICIO)
            .Build(((renderable) =>
            {
                modeloEdificio = renderable;

            }));


            //add the event handler
            arFragment.TapArPlane += OnTapArPlane;
            arFragment.ArSceneView.Scene.Update += onUpdateFrame;

        }

        private void onUpdateFrame(object sender, Scene.UpdateEventArgs e)
        {
            Frame frame = arFragment.ArSceneView.ArFrame;

            // If there is no frame, just return.
            if (frame == null)
            {
                return;
            }

            var updatedAugmentedImages = frame.GetUpdatedTrackables(Java.Lang.Class.FromType(typeof(AugmentedImage)));

            foreach (AugmentedImage augmentedImage in updatedAugmentedImages) 
            {                

                if (augmentedImage.TrackingState == TrackingState.Paused)
                {                    
                }
                else if (augmentedImage.TrackingState == TrackingState.Tracking)
                {
                    // Create a new anchor for newly found images.
                    if (!augmentedImageMap.Where(a => a.image.Name == augmentedImage.Name).Any())
                    {
                        var node = new AugmentedImageNode(this, augmentedImage.Name);
                        node.setImage(augmentedImage);

                        var nuevo = new AugmentedImageNodeDiccionario();
                        nuevo.image = augmentedImage;
                        nuevo.node = node;

                        augmentedImageMap.Add(nuevo);
                        arFragment.ArSceneView.Scene.AddChild(node);
                    }
                }
                else if (augmentedImage.TrackingState == TrackingState.Stopped)
                {
                    augmentedImageMap.Remove(augmentedImageMap.Where(a => a.image == augmentedImage).First());
                }

            }
        }

        private void OnTapArPlane(object sender, BaseArFragment.TapArPlaneEventArgs e)
        {
            if (modeloEdificio == null)
                return;



            // Create the Anchor.
            Anchor anchor = e.HitResult.CreateAnchor();
            AnchorNode anchorNode = new AnchorNode(anchor);
            anchorNode.SetParent(arFragment.ArSceneView.Scene);

            // Create the transformable andy and add it to the anchor.
            TransformableNode andy = new TransformableNode(arFragment.TransformationSystem);
            andy.SetParent(anchorNode);
            andy.Renderable = modeloEdificio;
            
            andy.Select();

        }

        public static bool CheckIsSupportedDeviceOrFinish(Activity activity)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.N)
            {
                Toast.MakeText(activity, "Sceneform requires Android N or later", ToastLength.Long).Show();

                activity.Finish();

                return false;
            }

            var openglString = ((ActivityManager)activity.GetSystemService(Context.ActivityService)).DeviceConfigurationInfo.GlEsVersion;

            if (Double.Parse(openglString) < MIN_OPENGL_VERSION)
            {
                Toast.MakeText(activity, "Sceneform requires OpenGL ES 3.0 or later", ToastLength.Long).Show();

                return false;
            }

            return true;

        }


    }

    public class AugmentedImageNode : AnchorNode
    {
        public AugmentedImageNode node { get; set; }
        public AugmentedImage image { get; set; }
        private ModelRenderable modeloTest;        

        public AugmentedImageNode(Context context, string augmentedImage)
        {
            // Upon construction, start loading the models for the corners of the frame.
            if (modeloTest == null)
            {
                //var imageName = HelloSceneformActivity.GLTF_ASSET_EDIFICIO;
                if (augmentedImage == AugmentedImageFragment.DEFAULT_IMAGE_NAME)
                {
                    //imageName = HelloSceneformActivity.GLTF_ASSET;
                    modeloTest = HelloSceneformActivity.modeloEdificio;
                }
                else if (augmentedImage == AugmentedImageFragment.MARTE_IMAGE_NAME)
                {
                    //imageName = HelloSceneformActivity.GLTF_ASSET_EDIFICIO;
                    modeloTest = HelloSceneformActivity.modeloTotem;
                }
                
                /*ModelRenderable.InvokeBuilder()
                .SetSource(context, RenderableSource.InvokeBuilder().SetSource(
                context,
                Uri.Parse(imageName),
                RenderableSource.SourceType.Gltf2)
                .SetScale(1f)
                .SetRecenterMode(RenderableSource.RecenterMode.Root)
                .Build())
                .SetRegistryId(imageName)
                .Build(((renderable) =>
                {
                    modeloTest = renderable;

                }));*/
            }
        }

        /**
         * Called when the AugmentedImage is detected and should be rendered. A Sceneform node tree is
         * created based on an Anchor created from the image. The corners are then positioned based on the
         * extents of the image. There is no need to worry about world coordinates since everything is
         * relative to the center of the image, which is the parent node of the corners.
         */        
        public void setImage(AugmentedImage image)
        {
            this.image = image;

            // If any of the models are not loaded, then recurse when all are loaded.
            /*if (!imagenTest.isDone() || !urCorner.isDone() || !llCorner.isDone() || !lrCorner.isDone())
            {
                
            }*/

            /*CompletableFuture.allOf(imagenTest, urCorner, llCorner, lrCorner)
                    .thenAccept((Void aVoid)->setImage(image))
          .exceptionally(
              throwable-> {
                    Log.e(TAG, "Exception loading", throwable);
                    return null;
                });*/

            // Set the anchor based on the center of the image.
            //SetAnchor(image.CreateAnchor(image.CenterPose));
            
            AnchorNode anchorNode = new AnchorNode(image.CreateAnchor(image.CenterPose));

            this.Anchor = image.CreateAnchor(image.CenterPose);
            // Make the 4 corner nodes.
            //Vector3 localPosition = new Vector3();
            Node cornerNode;
           
            cornerNode = new Node();
            cornerNode.SetParent(this);
            //cornerNode.LocalPosition = localPosition;
            //cornerNode.SetRenderable(imagenTest.getNow(null));
            cornerNode.Renderable = modeloTest;
        }
    }

    public class AugmentedImageNodeDiccionario
    {
        public AugmentedImageNode node { get; set; }
        public AugmentedImage image { get; set; }
    }
}


