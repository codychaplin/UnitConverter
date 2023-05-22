﻿using Android.App;
using Android.Content.PM;
using Android.OS;

namespace UnitConverter;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Window.SetFlags(Android.Views.WindowManagerFlags.NotFocusable, Android.Views.WindowManagerFlags.NotFocusable);
    }
}