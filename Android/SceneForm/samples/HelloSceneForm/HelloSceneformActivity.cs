
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Google.AR.Core;
using Google.AR.Sceneform;
using Google.AR.Sceneform.Rendering;
using Google.AR.Sceneform.UX;
using System;
using Uri = Android.Net.Uri;

namespace HelloSceneForm
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Locked, Exported = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class HelloSceneformActivity : AppCompatActivity
    {
        private static double MIN_OPENGL_VERSION = 3.0;

        private ArFragment arFragment;
        private ModelRenderable andyRenderable;

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
            ModelRenderable.InvokeBuilder().SetSource(this, Resource.Raw.andy).Build(((renderable) =>
            {
                andyRenderable = renderable;

            }));

            // var testeo = ModelRenderable.InvokeBuilder().HasSource();

            /*var builder = ModelRenderable.InvokeBuilder();

            var javaClass = Java.Lang.Class.FromType(builder.GetType());

            var asdfdf = (Context)this;
            var javaClass1 = Java.Lang.Class.FromType(asdfdf.GetType());

            var testeo = Resource.Raw.andy;
            var testeo2 = testeo.GetType();
            //var javaClass2 = Java.Lang.Class.FromType(Java.Lang.Integer.Type);
            var contexto = new Context();
            var method = javaClass.GetMethod("setSource", Java.Lang.Class.Context, Java.Lang.Integer.Type);
            method.Invoke(builder, this, Resource.Raw.andy);

            var future = builder.Build();
            var model = future.Get();

            andyRenderable = (ModelRenderable)model;*/

            //add the event handler
            arFragment.TapArPlane += OnTapArPlane;

        }

        private void OnTapArPlane(object sender, BaseArFragment.TapArPlaneEventArgs e)
        {
            if (andyRenderable == null)
                return;

            // Create the Anchor.
            Anchor anchor = e.HitResult.CreateAnchor();
            AnchorNode anchorNode = new AnchorNode(anchor);
            anchorNode.SetParent(arFragment.ArSceneView.Scene);

            // Create the transformable andy and add it to the anchor.
            TransformableNode andy = new TransformableNode(arFragment.TransformationSystem);
            andy.SetParent(anchorNode);
            andy.Renderable = andyRenderable;
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
}


