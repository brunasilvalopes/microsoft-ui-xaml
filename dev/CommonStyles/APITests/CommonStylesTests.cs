﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Common;
using MUXControlsTestApp.Utilities;
using System.Linq;
using System.Threading;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using MUXControls.TestAppUtils;
using PlatformConfiguration = Common.PlatformConfiguration;
using OSVersion = Common.OSVersion;
using System.Collections.Generic;
using XamlControlsResources = Microsoft.UI.Xaml.Controls.XamlControlsResources;
using Windows.UI.Xaml.Markup;

#if USING_TAEF
using WEX.TestExecution;
using WEX.TestExecution.Markup;
using WEX.Logging.Interop;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
#endif

namespace Windows.UI.Xaml.Tests.MUXControls.ApiTests
{
    [TestClass]
    public class CommonStylesApiTests
    {
        [TestMethod]
        public void VerifyUseCompactResourcesAPI()
        {
            //Verify there is no crash and TreeViewItemMinHeight is not the same when changing UseCompactResources.
            RunOnUIThread.Execute(() =>
            {
                var dict = new XamlControlsResources();
                var height = dict["TreeViewItemMinHeight"].ToString();

                dict.UseCompactResources = true;
                var compactHeight = dict["TreeViewItemMinHeight"].ToString();
                Verify.AreNotEqual(height, compactHeight, "Height in Compact is not the same as default");
                Verify.AreEqual("24", compactHeight, "Height in 24 in Compact");

                dict.UseCompactResources = false;
                var height2 = dict["TreeViewItemMinHeight"].ToString();
                Verify.AreEqual(height, height2, "Height are the same after disabled compact");
            });

            MUXControlsTestApp.Utilities.IdleSynchronizer.Wait();
        }

        [TestMethod]
        public void CornerRadiusFilterConverterTest()
        {
            if (!PlatformConfiguration.IsOsVersionGreaterThan(OSVersion.Redstone4))
            {
                Log.Comment("Corner radius is only available on RS5+");
                return;
            }

            RunOnUIThread.Execute(() => {
                var root = (StackPanel)XamlReader.Load(
                    @"<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                             xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                             xmlns:primitives='using:Microsoft.UI.Xaml.Controls.Primitives'> 
                            <StackPanel.Resources>
                                <primitives:CornerRadiusFilterConverter x:Key='CornerRadiusFilterConverter' />
                                <CornerRadius x:Key='testCornerRadius'>6,6,6,6</CornerRadius>
                            </StackPanel.Resources>
                            <Grid x:Name='TopRadiusGrid'
                                CornerRadius='{Binding Source={StaticResource testCornerRadius}, Converter={StaticResource CornerRadiusFilterConverter}, ConverterParameter=Top}'>
                            </Grid>
                            <Grid x:Name='RightRadiusGrid'
                                CornerRadius='{Binding Source={StaticResource testCornerRadius}, Converter={StaticResource CornerRadiusFilterConverter}, ConverterParameter=Right}'>
                            </Grid>
                            <Grid x:Name='BottomRadiusGrid'
                                CornerRadius='{Binding Source={StaticResource testCornerRadius}, Converter={StaticResource CornerRadiusFilterConverter}, ConverterParameter=Bottom}'>
                            </Grid>
                            <Grid x:Name='LeftRadiusGrid'
                                CornerRadius='{Binding Source={StaticResource testCornerRadius}, Converter={StaticResource CornerRadiusFilterConverter}, ConverterParameter=Left}'>
                            </Grid>
                       </StackPanel>");

                var topRadiusGrid = (Grid)root.FindName("TopRadiusGrid");
                var rightRadiusGrid = (Grid)root.FindName("RightRadiusGrid");
                var bottomRadiusGrid = (Grid)root.FindName("BottomRadiusGrid");
                var leftRadiusGrid = (Grid)root.FindName("LeftRadiusGrid");

                VerifyCornerRadius(topRadiusGrid.CornerRadius, new[] { 6, 6, 0, 0 });
                VerifyCornerRadius(rightRadiusGrid.CornerRadius, new[] { 0, 6, 6, 0 });
                VerifyCornerRadius(bottomRadiusGrid.CornerRadius, new[] { 0, 0, 6, 6 });
                VerifyCornerRadius(leftRadiusGrid.CornerRadius, new[] { 6, 0, 0, 6 });
            });
        }

        private void VerifyCornerRadius(CornerRadius radius, int[] expectedValue)
        {
            Verify.AreEqual(expectedValue[0], radius.TopLeft, "Verify CornerRadius.TopLeft");
            Verify.AreEqual(expectedValue[1], radius.TopRight, "Verify CornerRadius.TopRight");
            Verify.AreEqual(expectedValue[2], radius.BottomRight, "Verify CornerRadius.BottomRight");
            Verify.AreEqual(expectedValue[3], radius.BottomLeft, "Verify CornerRadius.BottomLeft");
        }
    }

    [TestClass]
    public class CommonStylesVisualTreeTestSamples
    {
        [TestMethod]
        [TestProperty("TestPass:IncludeOnlyOn", "Desktop")] // The default theme is different on OneCore, leading to a test failure.
        public void VerifyVisualTreeForAppBarAndAppBarToggleButton()
        {
            if (!PlatformConfiguration.IsOsVersion(OSVersion.Redstone5))
            {
                return;
            }

            var xaml = @"<Grid Width='400' Height='400' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'> 
                            <StackPanel>
                                <AppBarButton Icon='Accept' Label='Accept'/>
                                <AppBarToggleButton Icon='Dislike' Label='Dislike'/>
                            </StackPanel>
                       </Grid>";
            VisualTreeTestHelper.VerifyVisualTree(xaml: xaml, 
                masterFilePrefix: "VerifyVisualTreeForAppBarAndAppBarToggleButton");
        }

        [TestMethod]
        public void VerifyVisualTreeExampleLoadAndVerifyForAllThemes()
        {
            if (!PlatformConfiguration.IsOsVersion(OSVersion.Redstone5))
            {
                return;
            }

            var xaml = @"<Grid Width='400' Height='400' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'> 
                       </Grid>";
            VisualTreeTestHelper.VerifyVisualTree(xaml: xaml, 
                masterFilePrefix: "VerifyVisualTreeExampleLoadAndVerifyForAllThemes",
                theme: Theme.All);
        }

        [TestMethod]
        public void VerifyVisualTreeExampleLoadAndVerifyForDarkThemeWithCustomName()
        {
            if (!PlatformConfiguration.IsOsVersion(OSVersion.Redstone5))
            {
                return;
            }

            var xaml = @"<Grid Width='400' Height='400' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'> 
                       </Grid>";
            UIElement root = VisualTreeTestHelper.SetupVisualTree(xaml);
            RunOnUIThread.Execute(() =>
            {
                (root as FrameworkElement).RequestedTheme = ElementTheme.Dark;
            });
            VisualTreeTestHelper.VerifyVisualTree(root: root,
                masterFilePrefix: "VerifyVisualTreeExampleLoadAndVerifyForDarkThemeWithCustomName");
        }

        [TestMethod]
        public void VerifyVisualTreeExampleForLightTheme()
        {
            if (!PlatformConfiguration.IsOsVersion(OSVersion.Redstone5))
            {
                return;
            }

            var xaml = @"<Grid Width='400' Height='400' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'> 
                       </Grid>";
            UIElement root = VisualTreeTestHelper.SetupVisualTree(xaml);
            VisualTreeTestHelper.VerifyVisualTree(root: root,
                masterFilePrefix: "VerifyVisualTreeExampleForLightTheme",
                theme: Theme.Light);
        }

        [TestMethod]
        [TestProperty("TestPass:IncludeOnlyOn", "Desktop")] // The default theme is different on OneCore, leading to a test failure.
        public void VerifyVisualTreeExampleWithCustomerFilter()
        {
            if (!PlatformConfiguration.IsOsVersion(OSVersion.Redstone5))
            {
                return;
            }

            var xaml = @"<Grid Width='400' Height='400' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'> 
                        <TextBlock Text='Abc' />
                       </Grid>";

            VisualTreeTestHelper.VerifyVisualTree(xaml: xaml,
                masterFilePrefix: "VerifyVisualTreeExampleWithCustomerFilter",
                filter: new CustomizedFilter());
        }

        [TestMethod]
        [TestProperty("TestPass:IncludeOnlyOn", "Desktop")] // The default theme is different on OneCore, leading to a test failure.
        public void VerifyVisualTreeExampleWithCustomerPropertyValueTranslator()
        {
            if (!PlatformConfiguration.IsOsVersion(OSVersion.Redstone5))
            {
                return;
            }

            var xaml = @"<Grid Width='400' Height='400' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'> 
                            <TextBlock Text='Abc' />
                       </Grid>";

            VisualTreeTestHelper.VerifyVisualTree(xaml: xaml,
                masterFilePrefix: "VerifyVisualTreeExampleWithCustomerPropertyValueTranslator",
                translator: new CustomizedTranslator());
        }

        class CustomizedFilter : VisualTreeDumper.IFilter
        {

            private static readonly string[] _propertyNamePostfixBlackList = new string[] { "Property", "Transitions", "Template", "Style", "Selector" };

            private static readonly string[] _propertyNameBlackList = new string[] { "Interactions", "ColumnDefinitions", "RowDefinitions",
            "Children", "Resources", "Transitions", "Dispatcher", "TemplateSettings", "ContentTemplate", "ContentTransitions",
            "ContentTemplateSelector", "Content", "ContentTemplateRoot", "XYFocusUp", "XYFocusRight", "XYFocusLeft", "Parent",
            "Triggers", "RequestedTheme", "XamlRoot", "IsLoaded", "BaseUri", "Resources"};

            private static readonly Dictionary<string, string> _knownPropertyValueDict = new Dictionary<string, string> {
                {"Padding", "0,0,0,0"},
                {"IsTabStop", "False" },
                {"IsEnabled", "True"},
                {"IsLoaded", "True" },
                {"HorizontalContentAlignment", "Center" },
                {"FontSize", "14" },
                {"TabIndex", "2147483647" },
                {"AllowFocusWhenDisabled", "False" },
                {"CharacterSpacing", "0" },
                {"BorderThickness", "0,0,0,0"},
                {"FocusState", "Unfocused"},
                {"IsTextScaleFactorEnabled", "True" },
                {"UseSystemFocusVisuals","False" },
                {"RequiresPointer","Never" },
                {"IsFocusEngagementEnabled","False" },
                {"IsFocusEngaged","False" },
                {"ElementSoundMode","Default" },
                {"CornerRadius","0,0,0,0" },
                {"BackgroundSizing","InnerBorderEdge" },
                {"Width","NaN" },
                {"Name","" },
                {"MinWidth","0" },
                {"MinHeight","0" },
                {"MaxWidth","∞" },
                {"MaxHeight","∞" },
                {"Margin","0,0,0,0" },
                {"Language","en-US" },
                {"HorizontalAlignment","Stretch" },
                {"Height","NaN" },
                {"FlowDirection","LeftToRight" },
                {"RequestedTheme","Default" },
                {"FocusVisualSecondaryThickness","1,1,1,1" },
                {"FocusVisualPrimaryThickness","2,2,2,2" },
                {"FocusVisualMargin","0,0,0,0" },
                {"AllowFocusOnInteraction","True" },
                {"Visibility","Visible" },
                {"UseLayoutRounding","True" },
                {"RenderTransformOrigin","0,0" },
                {"AllowDrop","False" },
                {"Opacity","1" },
                {"ManipulationMode","System" },
                {"IsTapEnabled","True" },
                {"IsRightTapEnabled","True" },
                {"IsHoldingEnabled","True" },
                {"IsHitTestVisible","True" },
                {"IsDoubleTapEnabled","True" },
                {"CanDrag","False" },
                {"IsAccessKeyScope","False" },
                {"ExitDisplayModeOnAccessKeyInvoked","True" },
                {"AccessKey","" },
                {"KeyTipHorizontalOffset","0" },
                {"XYFocusRightNavigationStrategy","Auto" },
                {"HighContrastAdjustment","Application" },
                {"TabFocusNavigation","Local" },
                {"XYFocusUpNavigationStrategy","Auto" },
                {"XYFocusLeftNavigationStrategy","Auto" },
                {"XYFocusKeyboardNavigation","Auto" },
                {"XYFocusDownNavigationStrategy","Auto" },
                {"KeyboardAcceleratorPlacementMode","Auto" },
                {"CanBeScrollAnchor","False" },
                {"Translation","<0, 0, 0>" },
                {"Scale","<1, 1, 1>" },
                {"RotationAxis","<0, 0, 1>" },
                {"CenterPoint","<0, 0, 0>" },
                {"Rotation","0" },
                {"TransformMatrix","{ {M11:1 M12:0 M13:0 M14:0} {M21:0 M22:1 M23:0 M24:0} {M31:0 M32:0 M33:1 M34:0} {M41:0 M42:0 M43:0 M44:1} }"},
            };

            public bool ShouldVisitPropertyValuePair(string propertyName, string value)
            {
                string v = _knownPropertyValueDict.ContainsKey(propertyName) ? _knownPropertyValueDict[propertyName] : VisualTreeDumper.ValueNULL;
                return !(v.Equals(value) || (!string.IsNullOrEmpty(value) && value.StartsWith("Exception")));
            }

            public bool ShouldVisitElement(string elementName)
            {
                return true;
            }

            public bool ShouldVisitProperty(string propertyName)
            {
                return (_propertyNamePostfixBlackList.Where(item => propertyName.EndsWith(item)).Count()) == 0 &&
                    !_propertyNameBlackList.Contains(propertyName);
            }
        }

        class CustomizedTranslator : VisualTreeDumper.DefaultPropertyValueTranslator // Add prefix MyValue to all Value
        {
            public override string PropertyValueToString(string propertyName, object value)
            {
                return "MyValue" + base.PropertyValueToString(propertyName, value);
            }
        }
    }
}
