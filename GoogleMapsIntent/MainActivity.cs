using Android.App;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Runtime;
using System;
using Android.Content;
using Android.Content.PM;

namespace GoogleMapsIntent
{
    [Activity(Label = "GoogleMapsIntent", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ILocationListener
    {
        TextView textstatus;
        EditText latitude;
        EditText longitude;
        LocationManager locationmanager;
        ProgressDialog progress;
        Button launchgps;
        string mylongitude;
        string mylatitude;
        public void OnLocationChanged(Location location)
        {
            mylongitude = location.Longitude.ToString();
            mylatitude = location.Latitude.ToString();
        }

        public void OnProviderDisabled(string provider)
        {
            textstatus.Text = "Provide Disabled";
        }

        public void OnProviderEnabled(string provider)
        {
            textstatus.Text = "Provider Enabled";
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            latitude = FindViewById<EditText>(Resource.Id.editTextLatitud);
            longitude = FindViewById<EditText>(Resource.Id.editTextLongitud);
            launchgps = FindViewById<Button>(Resource.Id.buttonLaunchIntent);

            textstatus = FindViewById<TextView>(Resource.Id.textViewStatus);

            progress = new Android.App.ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
            progress.SetMessage("Espere...");
            progress.SetCancelable(false);

            launchgps.Click += delegate
            {
                progress.Show();
                var gmmintent = Android.Net.Uri.Parse(string.Format("google.navigation:q={0},{1}&mode=d", latitude.Text, longitude.Text));
                Intent intent = new Intent(Intent.ActionView, gmmintent);

                PackageManager pm = this.PackageManager;

                try
                {
                    pm.GetPackageInfo("com.google.android.apps.maps", 0);
                    if(pm != null)
                    {
                        intent.SetPackage("com.google.android.apps.maps");
                        progress.Hide();
                        StartActivity(intent);
                    }
                    
                }
                catch(PackageManager.NameNotFoundException e)
                {
                    progress.Hide();
                    Toast.MakeText(this, "Google Maps don´t installed in this device.", ToastLength.Long).Show();

                }

            };
        }

        protected override void OnResume()
        {
            base.OnResume();

            locationmanager = GetSystemService(Context.LocationService) as LocationManager;

            progress.Show();
            if (locationmanager.AllProviders.Contains(LocationManager.NetworkProvider) && locationmanager.IsProviderEnabled(LocationManager.NetworkProvider))
            {
                locationmanager.RequestLocationUpdates(LocationManager.NetworkProvider, 2000, 1, this);
            }
            else
            {
                progress.Hide();
                Toast.MakeText(this, "The network provider does not exist or is not enabled", ToastLength.Long).Show();
            }
            progress.Hide();
        }
    }
}

