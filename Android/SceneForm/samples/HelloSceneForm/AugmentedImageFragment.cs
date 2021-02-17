using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.AR.Sceneform.UX;
using Google.AR.Core;
using Android.Content.Res;
using Android.Graphics;
using Java.IO;

namespace HelloSceneForm
{
    [global::Android.Runtime.Register("com/google/ar/sceneform/samples/hellosceneform/AugmentedImageFragment")]
    class AugmentedImageFragment : ArFragment
    {
        public AugmentedImageFragment()
        {

        }

        private static String TAG = "AugmentedImageFragment";

  // This is the name of the image in the sample database.  A copy of the image is in the assets
  // directory.  Opening this image on your computer is a good quick way to test the augmented image
  // matching.
  public static String DEFAULT_IMAGE_NAME = "tierra.jpg";
  
  public static String MARTE_IMAGE_NAME = "marte.jpg";

  // This is a pre-created database containing the sample image.
  private static String SAMPLE_IMAGE_DATABASE = "sample_database.imgdb";

  // Augmented image configuration and rendering.
  // Load a single image (true) or a pre-generated image database (false).
  private static bool USE_SINGLE_IMAGE = false;

  // Do a runtime check for the OpenGL level available at runtime to avoid Sceneform crashing the
  // application.
  private static double MIN_OPENGL_VERSION = 3.0;
        
    public override void OnAttach(Context context)
    {
        base.OnAttach(context);

        // Check for Sceneform being supported on this device.  This check will be integrated into
        // Sceneform eventually.
        /*if (Build.VERSION.SdkInt < Build.VERSION_CODES.N)
        {
            Log.e(TAG, "Sceneform requires Android N or later");
            SnackbarHelper.getInstance()
                .showError(getActivity(), "Sceneform requires Android N or later");
        }*/

        String openGlVersionString =
            ((ActivityManager)context.GetSystemService(Context.ActivityService))
                .DeviceConfigurationInfo
                .GlEsVersion;
        if (Double.Parse(openGlVersionString) < MIN_OPENGL_VERSION)
        {
            /*Log.e(TAG, "Sceneform requires OpenGL ES 3.0 or later");
            SnackbarHelper.getInstance()
                .showError(getActivity(), "Sceneform requires OpenGL ES 3.0 or later");*/
        }
    }

        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = base.OnCreateView(inflater, container, savedInstanceState);

            // Turn off the plane discovery since we're only looking for images
            PlaneDiscoveryController.Hide();
            PlaneDiscoveryController.SetInstructionView(null);            
            ArSceneView.PlaneRenderer.Enabled = false;
            return view;
        }
        
  protected override Config GetSessionConfiguration(Session session)
        {
            Config config = base.GetSessionConfiguration(session);
            config.SetFocusMode(Google.AR.Core.Config.FocusMode.Auto);
            if (!setupAugmentedImageDatabase(config, session))
            {
                /*SnackbarHelper.getInstance()
                    .showError(getActivity(), "Could not setup augmented image database");*/
            }
            return config;
        }

        private bool setupAugmentedImageDatabase(Config config, Session session)
        {

            AugmentedImageDatabase augmentedImageDatabase;
            var assetManager = Context.Assets;

            Bitmap augmentedImageBitmap = loadAugmentedImageBitmap(assetManager, DEFAULT_IMAGE_NAME);
            Bitmap augmentedImageBitmapMarte = loadAugmentedImageBitmap(assetManager, MARTE_IMAGE_NAME);

            augmentedImageDatabase = new AugmentedImageDatabase(session);
            augmentedImageDatabase.AddImage(DEFAULT_IMAGE_NAME, augmentedImageBitmap);
            augmentedImageDatabase.AddImage(MARTE_IMAGE_NAME, augmentedImageBitmapMarte);

            config.SetAugmentedImageDatabase(augmentedImageDatabase);
            return true;
        }

        private Bitmap loadAugmentedImageBitmap(AssetManager assetManager, string imagePath)
        {
            try
            {
                var test = assetManager.Open(imagePath);
                return BitmapFactory.DecodeStream(test);
            }
            catch (IOException e)
            {                
            }

            return null;
        }
    }
}