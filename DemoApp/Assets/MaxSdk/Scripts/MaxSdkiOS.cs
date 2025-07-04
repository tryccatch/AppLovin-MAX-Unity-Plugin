using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using AppLovinMax.ThirdParty.MiniJson;

/// <summary>
/// iOS AppLovin MAX Unity Plugin implementation
/// </summary>
public class MaxSdkiOS : MaxSdkBase
{
    private delegate void ALUnityBackgroundCallback(string args);

    static MaxSdkiOS()
    {
        InitializeEventExecutor();

#if UNITY_IOS
        _MaxSetBackgroundCallback(BackgroundCallback);
#endif
    }

#if UNITY_IOS

    #region Initialization

    [DllImport("__Internal")]
    private static extern void _MaxSetBackgroundCallback(ALUnityBackgroundCallback backgroundCallback);

    [DllImport("__Internal")]
    private static extern void _MaxInitializeSdk(string serializedAdUnitIds, string serializedMetaData);

    /// <summary>
    /// Initialize the default instance of AppLovin SDK.
    ///
    /// Please make sure that application's Android manifest or Info.plist includes the AppLovin SDK key.
    /// <param name="adUnitIds">
    /// OPTIONAL: Set the MAX ad unit ids to be used for this instance of the SDK. 3rd-party SDKs will be initialized with the credentials configured for these ad unit ids.
    /// This should only be used if you have different sets of ad unit ids / credentials for the same package name.</param>
    /// </summary>
    public static void InitializeSdk(string[] adUnitIds = null)
    {
        var serializedAdUnitIds = (adUnitIds != null) ? string.Join(",", adUnitIds) : "";
        _MaxInitializeSdk(serializedAdUnitIds, GenerateMetaData());
    }

    [DllImport("__Internal")]
    private static extern bool _MaxIsInitialized();

    /// <summary>
    /// Check if the SDK has been initialized.
    /// </summary>
    /// <returns>True if SDK has been initialized</returns>
    public static bool IsInitialized()
    {
        return _MaxIsInitialized();
    }

    #endregion

    #region User Info

    [DllImport("__Internal")]
    private static extern void _MaxSetUserId(string userId);

    /// <summary>
    /// Set an identifier for the current user. This identifier will be tied to SDK events and our optional S2S postbacks.
    /// 
    /// If you're using reward validation, you can optionally set an identifier to be included with currency validation postbacks.
    /// For example, a username or email. We'll include this in the postback when we ping your currency endpoint from our server.
    /// </summary>
    /// 
    /// <param name="userId">The user identifier to be set. Must not be null.</param>
    public static void SetUserId(string userId)
    {
        _MaxSetUserId(userId);
    }

    [DllImport("__Internal")]
    private static extern bool _MaxSetSegmentCollection(string segmentCollectionsJson);

    /// <summary>
    /// Set the <see cref="MaxSegmentCollection"/>.
    /// </summary>
    /// <param name="segmentCollection"> The segment collection to be set. Must not be {@code null}</param>
    public static void SetSegmentCollection(MaxSegmentCollection segmentCollection)
    {
        _MaxSetSegmentCollection(JsonUtility.ToJson(segmentCollection));
    }

    #endregion

    #region MAX

    [DllImport("__Internal")]
    private static extern string _MaxGetAvailableMediatedNetworks();

    /// <summary>
    /// Returns the list of available mediation networks.
    /// 
    /// Please call this method after the SDK has initialized.
    /// </summary>
    public static List<MaxSdkBase.MediatedNetworkInfo> GetAvailableMediatedNetworks()
    {
        var serializedNetworks = _MaxGetAvailableMediatedNetworks();
        return MaxSdkUtils.PropsStringsToList<MaxSdkBase.MediatedNetworkInfo>(serializedNetworks);
    }

    [DllImport("__Internal")]
    private static extern void _MaxShowMediationDebugger();

    /// <summary>
    /// Present the mediation debugger UI.
    /// This debugger tool provides the status of your integration for each third-party ad network.
    ///
    /// Please call this method after the SDK has initialized.
    /// </summary>
    public static void ShowMediationDebugger()
    {
        _MaxShowMediationDebugger();
    }

    [DllImport("__Internal")]
    private static extern void _MaxShowCreativeDebugger();

    /// <summary>
    /// Present the creative debugger UI.
    /// This debugger tool provides information for recently displayed ads.
    ///
    /// Please call this method after the SDK has initialized.
    /// </summary>
    public static void ShowCreativeDebugger()
    {
        _MaxShowCreativeDebugger();
    }

    [DllImport("__Internal")]
    private static extern string _MaxGetAdValue(string adUnitIdentifier, string key);

    /// <summary>
    /// Returns the arbitrary ad value for a given ad unit identifier with key. Returns null if no ad is loaded.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier for which to get the ad value for. Must not be null.</param>
    /// <param name="key">Ad value key. Must not be null.</param>
    /// <returns>Arbitrary ad value for a given key, or null if no ad is loaded.</returns>
    public static string GetAdValue(string adUnitIdentifier, string key)
    {
        var value = _MaxGetAdValue(adUnitIdentifier, key);

        if (string.IsNullOrEmpty(value)) return null;

        return value;
    }

    #endregion

    #region Privacy

    /// <summary>
    /// Get the SDK configuration for this user.
    ///
    /// Note: This method should be called only after SDK has been initialized.
    /// </summary>
    [DllImport("__Internal")]
    private static extern string _MaxGetSdkConfiguration();

    public static SdkConfiguration GetSdkConfiguration()
    {
        var sdkConfigurationStr = _MaxGetSdkConfiguration();
        var sdkConfigurationDict = Json.Deserialize(sdkConfigurationStr) as Dictionary<string, object>;
        return SdkConfiguration.Create(sdkConfigurationDict);
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetHasUserConsent(bool hasUserConsent);

    /// <summary>
    /// Set whether or not user has provided consent for information sharing with AppLovin and other providers.
    /// </summary>
    /// <param name="hasUserConsent"><c>true</c> if the user has provided consent for information sharing with AppLovin. <c>false</c> by default.</param>
    public static void SetHasUserConsent(bool hasUserConsent)
    {
        _MaxSetHasUserConsent(hasUserConsent);
    }

    [DllImport("__Internal")]
    private static extern bool _MaxHasUserConsent();

    /// <summary>
    /// Check if user has provided consent for information sharing with AppLovin and other providers.
    /// </summary>
    /// <returns><c>true</c> if user has provided consent for information sharing. <c>false</c> if the user declined to share information or the consent value has not been set <see cref="IsUserConsentSet">.</returns>
    public static bool HasUserConsent()
    {
        return _MaxHasUserConsent();
    }

    [DllImport("__Internal")]
    private static extern bool _MaxIsUserConsentSet();

    /// <summary>
    /// Check if user has set consent for information sharing.
    /// </summary>
    /// <returns><c>true</c> if user has set a value of consent for information sharing.</returns>
    public static bool IsUserConsentSet()
    {
        return _MaxIsUserConsentSet();
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetDoNotSell(bool doNotSell);

    /// <summary>
    /// Set whether or not user has opted out of the sale of their personal information.
    /// </summary>
    /// <param name="doNotSell"><c>true</c> if the user has opted out of the sale of their personal information.</param>
    public static void SetDoNotSell(bool doNotSell)
    {
        _MaxSetDoNotSell(doNotSell);
    }

    [DllImport("__Internal")]
    private static extern bool _MaxIsDoNotSell();

    /// <summary>
    /// Check if the user has opted out of the sale of their personal information.
    /// </summary>
    /// <returns><c>true</c> if the user has opted out of the sale of their personal information. <c>false</c> if the user opted in to the sell of their personal information or the value has not been set <see cref="IsDoNotSellSet">.</returns>
    public static bool IsDoNotSell()
    {
        return _MaxIsDoNotSell();
    }

    [DllImport("__Internal")]
    private static extern bool _MaxIsDoNotSellSet();

    /// <summary>
    /// Check if the user has set the option to sell their personal information
    /// </summary>
    /// <returns><c>true</c> if user has chosen an option to sell their personal information.</returns>
    public static bool IsDoNotSellSet()
    {
        return _MaxIsDoNotSellSet();
    }

    #endregion

    #region Banners

    [DllImport("__Internal")]
    private static extern void _MaxCreateBanner(string adUnitIdentifier, string bannerPosition, bool isAdaptive);

    [DllImport("__Internal")]
    private static extern void _MaxCreateBannerXY(string adUnitIdentifier, float x, float y, bool isAdaptive);

    /// <summary>
    /// Create a new banner.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to create. Must not be null.</param>
    /// <param name="configuration">The configuration for the banner</param>
    public static void CreateBanner(string adUnitIdentifier, AdViewConfiguration configuration)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "create banner");
        if (configuration.UseCoordinates)
        {
            _MaxCreateBannerXY(adUnitIdentifier, configuration.XCoordinate, configuration.YCoordinate, configuration.IsAdaptive);
        }
        else
        {
            _MaxCreateBanner(adUnitIdentifier, configuration.Position.ToSnakeCaseString(), configuration.IsAdaptive);
        }
    }

    [DllImport("__Internal")]
    private static extern void _MaxLoadBanner(string adUnitIdentifier);

    /// <summary>
    /// Load a new banner ad.
    /// NOTE: The <see cref="CreateBanner()"/> method loads the first banner ad and initiates an automated banner refresh process.
    /// You only need to call this method if you pause banner refresh.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to load. Must not be null.</param>
    public static void LoadBanner(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "load banner");
        _MaxLoadBanner(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetBannerPlacement(string adUnitIdentifier, string placement);

    /// <summary>
    /// Set the banner placement for an ad unit identifier to tie the future ad events to.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to set the placement for. Must not be null.</param>
    /// <param name="placement">Placement to set</param>
    public static void SetBannerPlacement(string adUnitIdentifier, string placement)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set banner placement");
        _MaxSetBannerPlacement(adUnitIdentifier, placement);
    }

    [DllImport("__Internal")]
    private static extern void _MaxStartBannerAutoRefresh(string adUnitIdentifier);

    /// <summary>
    /// Starts or resumes auto-refreshing of the banner for the given ad unit identifier.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner for which to start auto-refresh. Must not be null.</param>
    public static void StartBannerAutoRefresh(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "start banner auto-refresh");
        _MaxStartBannerAutoRefresh(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxStopBannerAutoRefresh(string adUnitIdentifeir);

    /// <summary>
    /// Pauses auto-refreshing of the banner for the given ad unit identifier.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner for which to stop auto-refresh. Must not be null.</param>
    public static void StopBannerAutoRefresh(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "stop banner auto-refresh");
        _MaxStopBannerAutoRefresh(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxUpdateBannerPosition(string adUnitIdentifier, string bannerPosition);

    /// <summary>
    /// Updates the position of the banner to the new position provided.
    /// </summary>
    /// <param name="adUnitIdentifier">The ad unit identifier of the banner for which to update the position. Must not be null.</param>
    /// <param name="bannerPosition">A new position for the banner. Must not be null.</param>
    public static void UpdateBannerPosition(string adUnitIdentifier, AdViewPosition bannerPosition)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "update banner position");
        _MaxUpdateBannerPosition(adUnitIdentifier, bannerPosition.ToSnakeCaseString());
    }

    [DllImport("__Internal")]
    private static extern void _MaxUpdateBannerPositionXY(string adUnitIdentifier, float x, float y);

    /// <summary>
    /// Updates the position of the banner to the new coordinates provided.
    /// </summary>
    /// <param name="adUnitIdentifier">The ad unit identifier of the banner for which to update the position. Must not be null.</param>
    /// <param name="x">The X coordinate (horizontal position) of the banner relative to the top left corner of the screen.</param>
    /// <param name="y">The Y coordinate (vertical position) of the banner relative to the top left corner of the screen.</param>
    /// <seealso cref="GetBannerLayout">
    /// The banner is placed within the safe area of the screen. You can use this to get the absolute position of the banner on screen.
    /// </seealso>
    public static void UpdateBannerPosition(string adUnitIdentifier, float x, float y)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "update banner position");
        _MaxUpdateBannerPositionXY(adUnitIdentifier, x, y);
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetBannerWidth(string adUnitIdentifier, float width);

    /// <summary>
    /// Overrides the width of the banner in points.
    /// </summary>
    /// <param name="adUnitIdentifier">The ad unit identifier of the banner for which to override the width for. Must not be null.</param>
    /// <param name="width">The desired width of the banner in points</param>
    public static void SetBannerWidth(string adUnitIdentifier, float width)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set banner width");
        _MaxSetBannerWidth(adUnitIdentifier, width);
    }

    [DllImport("__Internal")]
    private static extern void _MaxShowBanner(string adUnitIdentifier);

    /// <summary>
    /// Show banner at a position determined by the 'CreateBanner' call.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to show. Must not be null.</param>
    public static void ShowBanner(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "show banner");
        _MaxShowBanner(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxDestroyBanner(string adUnitIdentifier);

    /// <summary>
    /// Remove banner from the ad view and destroy it.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to destroy. Must not be null.</param>
    public static void DestroyBanner(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "destroy banner");
        _MaxDestroyBanner(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxHideBanner(string adUnitIdentifier);

    /// <summary>
    /// Hide banner.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to hide. Must not be null.</param>
    public static void HideBanner(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "hide banner");
        _MaxHideBanner(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetBannerBackgroundColor(string adUnitIdentifier, string hexColorCodeString);

    /// <summary>
    /// Set non-transparent background color for banners to be fully functional.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to set background color for. Must not be null.</param>
    /// <param name="color">A background color to set for the ad</param>
    public static void SetBannerBackgroundColor(string adUnitIdentifier, Color color)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set background color");
        _MaxSetBannerBackgroundColor(adUnitIdentifier, MaxSdkUtils.ParseColor(color));
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetBannerExtraParameter(string adUnitIdentifier, string key, string value);

    /// <summary>
    /// Set an extra parameter for the banner ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to set the extra parameter for. Must not be null.</param>
    /// <param name="key">The key for the extra parameter. Must not be null.</param>
    /// <param name="value">The value for the extra parameter.</param>
    public static void SetBannerExtraParameter(string adUnitIdentifier, string key, string value)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set banner extra parameter");
        _MaxSetBannerExtraParameter(adUnitIdentifier, key, value);
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetBannerLocalExtraParameter(string adUnitIdentifier, string key, IntPtr value);

    [DllImport("__Internal")]
    private static extern void _MaxSetBannerLocalExtraParameterJSON(string adUnitIdentifier, string key, string json);

    /// <summary>
    /// Set a local extra parameter for the banner ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to set the local extra parameter for. Must not be null.</param>
    /// <param name="key">The key for the local extra parameter. Must not be null.</param>
    /// <param name="value">The value for the local extra parameter. Accepts the following types: <see cref="IntPtr"/>, <c>null</c>, <c>IList</c>, <c>IDictionary</c>, <c>string</c>, primitive types</param>
    public static void SetBannerLocalExtraParameter(string adUnitIdentifier, string key, object value)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set banner local extra parameter");

        if (value == null || value is IntPtr)
        {
            var intPtrValue = value == null ? IntPtr.Zero : (IntPtr) value;
            _MaxSetBannerLocalExtraParameter(adUnitIdentifier, key, intPtrValue);
        }
        else
        {
            _MaxSetBannerLocalExtraParameterJSON(adUnitIdentifier, key, SerializeLocalExtraParameterValue(value));
        }
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetBannerCustomData(string adUnitIdentifier, string customData);

    /// <summary>
    /// The custom data to tie the showing banner ad to, for ILRD and rewarded postbacks via the <c>{CUSTOM_DATA}</c> macro. Maximum size is 8KB.
    /// </summary>
    /// <param name="adUnitIdentifier">Banner ad unit identifier of the banner to set the custom data for. Must not be null.</param>
    /// <param name="customData">The custom data to be set.</param>
    public static void SetBannerCustomData(string adUnitIdentifier, string customData)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set banner custom data");
        _MaxSetBannerCustomData(adUnitIdentifier, customData);
    }

    [DllImport("__Internal")]
    private static extern string _MaxGetBannerLayout(string adUnitIdentifier);

    /// <summary>
    /// The banner position on the screen. When setting the banner position via <see cref="CreateBanner(string, float, float)"/> or <see cref="UpdateBannerPosition(string, float, float)"/>,
    /// the banner is placed within the safe area of the screen. This returns the absolute position of the banner on screen.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner for which to get the position on screen. Must not be null.</param>
    /// <returns>A <see cref="Rect"/> representing the banner position on screen.</returns>
    public static Rect GetBannerLayout(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "get banner layout");
        var positionRect = _MaxGetBannerLayout(adUnitIdentifier);
        return GetRectFromString(positionRect);
    }

    #endregion

    #region MRECs

    [DllImport("__Internal")]
    private static extern void _MaxCreateMRec(string adUnitIdentifier, string mrecPosition);

    [DllImport("__Internal")]
    private static extern void _MaxCreateMRecXY(string adUnitIdentifier, float x, float y);

    /// <summary>
    /// Create a new MREC.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC to create. Must not be null.</param>
    /// <param name="configuration">The configuration for the MREC.</param>
    public static void CreateMRec(string adUnitIdentifier, AdViewConfiguration configuration)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "create MREC");
        if (configuration.UseCoordinates)
        {
            _MaxCreateMRecXY(adUnitIdentifier, configuration.XCoordinate, configuration.YCoordinate);
        }
        else
        {
            _MaxCreateMRec(adUnitIdentifier, configuration.Position.ToSnakeCaseString());
        }
    }

    [DllImport("__Internal")]
    private static extern void _MaxLoadMRec(string adUnitIdentifier);

    /// <summary>
    /// Load a new MREC ad.
    /// NOTE: The <see cref="CreateMRec()"/> method loads the first MREC ad and initiates an automated MREC refresh process.
    /// You only need to call this method if you pause MREC refresh.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC to load. Must not be null.</param>
    public static void LoadMRec(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "load MREC");
        _MaxLoadMRec(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetMRecPlacement(string adUnitIdentifier, string placement);

    /// <summary>
    /// Set the MREC placement for an ad unit identifier to tie the future ad events to.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC to set the placement for. Must not be null.</param>
    /// <param name="placement">Placement to set</param>
    public static void SetMRecPlacement(string adUnitIdentifier, string placement)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set MREC placement");
        _MaxSetMRecPlacement(adUnitIdentifier, placement);
    }

    [DllImport("__Internal")]
    private static extern void _MaxStartMRecAutoRefresh(string adUnitIdentifier);

    /// <summary>
    /// Starts or resumes auto-refreshing of the MREC for the given ad unit identifier.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC for which to start auto-refresh. Must not be null.</param>
    public static void StartMRecAutoRefresh(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "start MREC auto-refresh");
        _MaxStartMRecAutoRefresh(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxStopMRecAutoRefresh(string adUnitIdentifeir);

    /// <summary>
    /// Pauses auto-refreshing of the MREC for the given ad unit identifier.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC for which to stop auto-refresh. Must not be null.</param>
    public static void StopMRecAutoRefresh(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "stop MREC auto-refresh");
        _MaxStopMRecAutoRefresh(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxUpdateMRecPosition(string adUnitIdentifier, string mrecPosition);

    /// <summary>
    /// Updates the position of the MREC to the new position provided.
    /// </summary>
    /// <param name="adUnitIdentifier">The ad unit identifier of the MREC for which to update the position. Must not be null.</param>
    /// <param name="mrecPosition">A new position for the MREC. Must not be null.</param>
    public static void UpdateMRecPosition(string adUnitIdentifier, AdViewPosition mrecPosition)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "update MREC position");
        _MaxUpdateMRecPosition(adUnitIdentifier, mrecPosition.ToSnakeCaseString());
    }

    [DllImport("__Internal")]
    private static extern void _MaxUpdateMRecPositionXY(string adUnitIdentifier, float x, float y);

    /// <summary>
    /// Updates the position of the MREC to the new coordinates provided.
    /// </summary>
    /// <param name="adUnitIdentifier">The ad unit identifier of the MREC for which to update the position. Must not be null.</param>
    /// <param name="x">The X coordinate (horizontal position) of the MREC relative to the top left corner of the screen.</param>
    /// <param name="y">The Y coordinate (vertical position) of the MREC relative to the top left corner of the screen.</param>
    /// <seealso cref="GetMRecLayout">
    /// The MREC is placed within the safe area of the screen. You can use this to get the absolute position Rect of the MREC on screen.
    /// </seealso>
    public static void UpdateMRecPosition(string adUnitIdentifier, float x, float y)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "update MREC position");
        _MaxUpdateMRecPositionXY(adUnitIdentifier, x, y);
    }

    [DllImport("__Internal")]
    private static extern void _MaxShowMRec(string adUnitIdentifier);

    /// <summary>
    /// Show MREC at a position determined by the 'CreateMRec' call.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC to show. Must not be null.</param>
    public static void ShowMRec(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "show MREC");
        _MaxShowMRec(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxDestroyMRec(string adUnitIdentifier);

    /// <summary>
    /// Remove MREC from the ad view and destroy it.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC to destroy. Must not be null.</param>
    public static void DestroyMRec(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "destroy MREC");
        _MaxDestroyMRec(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxHideMRec(string adUnitIdentifier);

    /// <summary>
    /// Hide MREC.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC to hide. Must not be null.</param>
    public static void HideMRec(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "hide MREC");
        _MaxHideMRec(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetMRecExtraParameter(string adUnitIdentifier, string key, string value);

    /// <summary>
    /// Set an extra parameter for the MREC ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC to set the extra parameter for. Must not be null.</param>
    /// <param name="key">The key for the extra parameter. Must not be null.</param>
    /// <param name="value">The value for the extra parameter.</param>
    public static void SetMRecExtraParameter(string adUnitIdentifier, string key, string value)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set MREC extra parameter");
        _MaxSetMRecExtraParameter(adUnitIdentifier, key, value);
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetMRecLocalExtraParameter(string adUnitIdentifier, string key, IntPtr value);

    [DllImport("__Internal")]
    private static extern void _MaxSetMRecLocalExtraParameterJSON(string adUnitIdentifier, string key, string json);

    /// <summary>
    /// Set a local extra parameter for the MREC ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC to set the local extra parameter for. Must not be null.</param>
    /// <param name="key">The key for the local extra parameter. Must not be null.</param>
    /// <param name="value">The value for the local extra parameter. Accepts the following types: <see cref="IntPtr"/>, <c>null</c>, <c>IList</c>, <c>IDictionary</c>, <c>string</c>, primitive types</param>
    public static void SetMRecLocalExtraParameter(string adUnitIdentifier, string key, object value)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set MREC local extra parameter");

        if (value == null || value is IntPtr)
        {
            var intPtrValue = value == null ? IntPtr.Zero : (IntPtr) value;
            _MaxSetMRecLocalExtraParameter(adUnitIdentifier, key, intPtrValue);
        }
        else
        {
            _MaxSetMRecLocalExtraParameterJSON(adUnitIdentifier, key, SerializeLocalExtraParameterValue(value));
        }
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetMRecCustomData(string adUnitIdentifier, string value);

    /// <summary>
    /// The custom data to tie the showing MREC ad to, for ILRD and rewarded postbacks via the <c>{CUSTOM_DATA}</c> macro. Maximum size is 8KB.
    /// </summary>
    /// <param name="adUnitIdentifier">MREC Ad unit identifier of the banner to set the custom data for. Must not be null.</param>
    /// <param name="customData">The custom data to be set.</param>
    public static void SetMRecCustomData(string adUnitIdentifier, string customData)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set MREC custom data");
        _MaxSetMRecCustomData(adUnitIdentifier, customData);
    }

    [DllImport("__Internal")]
    private static extern string _MaxGetMRecLayout(string adUnitIdentifier);

    /// <summary>
    /// The MREC position on the screen. When setting the banner position via <see cref="CreateMRec(string, float, float)"/> or <see cref="UpdateMRecPosition(string, float, float)"/>,
    /// the banner is placed within the safe area of the screen. This returns the absolute position of the MREC on screen.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC for which to get the position on screen. Must not be null.</param>
    /// <returns>A <see cref="Rect"/> representing the banner position on screen.</returns>
    public static Rect GetMRecLayout(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "get MREC layout");
        var positionRect = _MaxGetMRecLayout(adUnitIdentifier);
        return GetRectFromString(positionRect);
    }

    #endregion

    #region Interstitials

    [DllImport("__Internal")]
    private static extern void _MaxLoadInterstitial(string adUnitIdentifier);

    /// <summary>
    /// Start loading an interstitial.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the interstitial to load. Must not be null.</param>
    public static void LoadInterstitial(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "load interstitial");
        _MaxLoadInterstitial(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern bool _MaxIsInterstitialReady(string adUnitIdentifier);

    /// <summary>
    /// Check if interstitial ad is loaded and ready to be displayed.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the interstitial ad to check if it's ready to be displayed. Must not be null.</param>
    /// <returns>True if the ad is ready to be displayed</returns>
    public static bool IsInterstitialReady(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "check interstitial loaded");
        return _MaxIsInterstitialReady(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxShowInterstitial(string adUnitIdentifier, string placement, string customData);

    /// <summary>
    /// Present loaded interstitial for a given placement to tie ad events to. Note: if the interstitial is not ready to be displayed nothing will happen.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the interstitial to load. Must not be null.</param>
    /// <param name="placement">The placement to tie the showing ad's events to</param>
    /// <param name="customData">The custom data to tie the showing ad's events to. Maximum size is 8KB.</param>
    public static void ShowInterstitial(string adUnitIdentifier, string placement = null, string customData = null)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "show interstitial");

        if (IsInterstitialReady(adUnitIdentifier))
        {
            _MaxShowInterstitial(adUnitIdentifier, placement, customData);
        }
        else
        {
            MaxSdkLogger.UserWarning("Not showing MAX Ads interstitial: ad not ready");
        }
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetInterstitialExtraParameter(string adUnitIdentifier, string key, string value);

    /// <summary>
    /// Set an extra parameter for the ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the interstitial to set the extra parameter for. Must not be null.</param>
    /// <param name="key">The key for the extra parameter. Must not be null.</param>
    /// <param name="value">The value for the extra parameter.</param>
    public static void SetInterstitialExtraParameter(string adUnitIdentifier, string key, string value)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set interstitial extra parameter");
        _MaxSetInterstitialExtraParameter(adUnitIdentifier, key, value);
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetInterstitialLocalExtraParameter(string adUnitIdentifier, string key, IntPtr value);

    [DllImport("__Internal")]
    private static extern void _MaxSetInterstitialLocalExtraParameterJSON(string adUnitIdentifier, string key, string json);

    /// <summary>
    /// Set a local extra parameter for the ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the interstitial to set the local extra parameter for. Must not be null.</param>
    /// <param name="key">The key for the local extra parameter. Must not be null.</param>
    /// <param name="value">The value for the local extra parameter. Accepts the following types: <see cref="IntPtr"/>, <c>null</c>, <c>IList</c>, <c>IDictionary</c>, <c>string</c>, primitive types</param>
    public static void SetInterstitialLocalExtraParameter(string adUnitIdentifier, string key, object value)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set interstitial local extra parameter");

        if (value == null || value is IntPtr)
        {
            var intPtrValue = value == null ? IntPtr.Zero : (IntPtr) value;
            _MaxSetInterstitialLocalExtraParameter(adUnitIdentifier, key, intPtrValue);
        }
        else
        {
            _MaxSetInterstitialLocalExtraParameterJSON(adUnitIdentifier, key, SerializeLocalExtraParameterValue(value));
        }
    }

    #endregion

    #region App Open Ads

    [DllImport("__Internal")]
    private static extern void _MaxLoadAppOpenAd(string adUnitIdentifier);

    /// <summary>
    /// Start loading an app open ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the app open ad to load. Must not be null.</param>
    public static void LoadAppOpenAd(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "load app open ad");
        _MaxLoadAppOpenAd(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern bool _MaxIsAppOpenAdReady(string adUnitIdentifier);

    /// <summary>
    /// Check if app open ad ad is loaded and ready to be displayed.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the app open ad ad to check if it's ready to be displayed. Must not be null.</param>
    /// <returns>True if the ad is ready to be displayed</returns>
    public static bool IsAppOpenAdReady(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "check app open ad loaded");
        return _MaxIsAppOpenAdReady(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxShowAppOpenAd(string adUnitIdentifier, string placement, string customData);

    /// <summary>
    /// Present loaded app open ad for a given placement to tie ad events to. Note: if the app open ad is not ready to be displayed nothing will happen.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the app open ad to load. Must not be null.</param>
    /// <param name="placement">The placement to tie the showing ad's events to</param>
    /// <param name="customData">The custom data to tie the showing ad's events to. Maximum size is 8KB.</param>
    public static void ShowAppOpenAd(string adUnitIdentifier, string placement = null, string customData = null)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "show app open ad");

        if (IsAppOpenAdReady(adUnitIdentifier))
        {
            _MaxShowAppOpenAd(adUnitIdentifier, placement, customData);
        }
        else
        {
            MaxSdkLogger.UserWarning("Not showing MAX Ads app open ad: ad not ready");
        }
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetAppOpenAdExtraParameter(string adUnitIdentifier, string key, string value);

    /// <summary>
    /// Set an extra parameter for the ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the app open ad to set the extra parameter for. Must not be null.</param>
    /// <param name="key">The key for the extra parameter. Must not be null.</param>
    /// <param name="value">The value for the extra parameter.</param>
    public static void SetAppOpenAdExtraParameter(string adUnitIdentifier, string key, string value)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set app open ad extra parameter");
        _MaxSetAppOpenAdExtraParameter(adUnitIdentifier, key, value);
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetAppOpenAdLocalExtraParameter(string adUnitIdentifier, string key, IntPtr value);

    [DllImport("__Internal")]
    private static extern void _MaxSetAppOpenAdLocalExtraParameterJSON(string adUnitIdentifier, string key, string json);

    /// <summary>
    /// Set a local extra parameter for the ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the app open ad to set the local extra parameter for. Must not be null.</param>
    /// <param name="key">The key for the local extra parameter. Must not be null.</param>
    /// <param name="value">The value for the local extra parameter. Accepts the following types: <see cref="IntPtr"/>, <c>null</c>, <c>IList</c>, <c>IDictionary</c>, <c>string</c>, primitive types</param>
    public static void SetAppOpenAdLocalExtraParameter(string adUnitIdentifier, string key, object value)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set app open ad local extra parameter");

        if (value == null || value is IntPtr)
        {
            var intPtrValue = value == null ? IntPtr.Zero : (IntPtr) value;
            _MaxSetAppOpenAdLocalExtraParameter(adUnitIdentifier, key, intPtrValue);
        }
        else
        {
            _MaxSetAppOpenAdLocalExtraParameterJSON(adUnitIdentifier, key, SerializeLocalExtraParameterValue(value));
        }
    }

    #endregion

    #region Rewarded

    [DllImport("__Internal")]
    private static extern void _MaxLoadRewardedAd(string adUnitIdentifier);

    /// <summary>
    /// Start loading an rewarded ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the rewarded ad to load. Must not be null.</param>
    public static void LoadRewardedAd(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "load rewarded ad");
        _MaxLoadRewardedAd(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern bool _MaxIsRewardedAdReady(string adUnitIdentifier);

    /// <summary>
    /// Check if rewarded ad ad is loaded and ready to be displayed.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the rewarded ad to check if it's ready to be displayed. Must not be null.</param>
    /// <returns>True if the ad is ready to be displayed</returns>
    public static bool IsRewardedAdReady(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "check rewarded ad loaded");
        return _MaxIsRewardedAdReady(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxShowRewardedAd(string adUnitIdentifier, string placement, string customData);

    /// <summary>
    /// Present loaded rewarded ad for a given placement to tie ad events to. Note: if the rewarded ad is not ready to be displayed nothing will happen.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the interstitial to load. Must not be null.</param>
    /// <param name="placement">The placement to tie the showing ad's events to</param>
    /// <param name="customData">The custom data to tie the showing ad's events to. Maximum size is 8KB.</param>
    public static void ShowRewardedAd(string adUnitIdentifier, string placement = null, string customData = null)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "show rewarded ad");

        if (IsRewardedAdReady(adUnitIdentifier))
        {
            _MaxShowRewardedAd(adUnitIdentifier, placement, customData);
        }
        else
        {
            MaxSdkLogger.UserWarning("Not showing MAX Ads rewarded ad: ad not ready");
        }
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetRewardedAdExtraParameter(string adUnitIdentifier, string key, string value);

    /// <summary>
    /// Set an extra parameter for the ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the rewarded ad to set the extra parameter for. Must not be null.</param>
    /// <param name="key">The key for the extra parameter. Must not be null.</param>
    /// <param name="value">The value for the extra parameter.</param>
    public static void SetRewardedAdExtraParameter(string adUnitIdentifier, string key, string value)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set rewarded extra parameter");
        _MaxSetRewardedAdExtraParameter(adUnitIdentifier, key, value);
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetRewardedAdLocalExtraParameter(string adUnitIdentifier, string key, IntPtr value);

    [DllImport("__Internal")]
    private static extern void _MaxSetRewardedAdLocalExtraParameterJSON(string adUnitIdentifier, string key, string json);

    /// <summary>
    /// Set a local extra parameter for the ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the rewarded ad to set the local extra parameter for. Must not be null.</param>
    /// <param name="key">The key for the local extra parameter. Must not be null.</param>
    /// <param name="value">The value for the local extra parameter. Accepts the following types: <see cref="IntPtr"/>, <c>null</c>, <c>IList</c>, <c>IDictionary</c>, <c>string</c>, primitive types</param>
    public static void SetRewardedAdLocalExtraParameter(string adUnitIdentifier, string key, object value)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set rewarded ad local extra parameter");

        if (value == null || value is IntPtr)
        {
            var intPtrValue = value == null ? IntPtr.Zero : (IntPtr) value;
            _MaxSetRewardedAdLocalExtraParameter(adUnitIdentifier, key, intPtrValue);
        }
        else
        {
            _MaxSetRewardedAdLocalExtraParameterJSON(adUnitIdentifier, key, SerializeLocalExtraParameterValue(value));
        }
    }

    #endregion

    #region Event Tracking

    [DllImport("__Internal")]
    private static extern void _MaxTrackEvent(string name, string parameters);

    /// <summary>
    /// Track an event using AppLovin.
    /// </summary>
    /// <param name="name">An event from the list of pre-defined events may be found in MaxEvents.cs as part of the AppLovin SDK framework. Must not be null.</param>
    /// <param name="parameters">A dictionary containing key-value pairs further describing this event.</param>
    public static void TrackEvent(string name, IDictionary<string, string> parameters = null)
    {
        _MaxTrackEvent(name, Json.Serialize(parameters));
    }

    #endregion

    #region Settings

    [DllImport("__Internal")]
    private static extern void _MaxSetMuted(bool muted);

    /// <summary>
    /// Set whether to begin video ads in a muted state or not.
    ///
    /// Please call this method after the SDK has initialized.
    /// </summary>
    /// <param name="muted"><c>true</c> if video ads should being in muted state.</param>
    public static void SetMuted(bool muted)
    {
        _MaxSetMuted(muted);
    }

    [DllImport("__Internal")]
    private static extern bool _MaxIsMuted();

    /// <summary>
    /// Whether video ads begin in a muted state or not. Defaults to <c>false</c>.
    ///
    /// Note: Returns <c>false</c> if the SDK is not initialized.
    /// </summary>
    /// <returns><c>true</c> if video ads begin in muted state.</returns>
    public static bool IsMuted()
    {
        return _MaxIsMuted();
    }

    [DllImport("__Internal")]
    private static extern bool _MaxSetVerboseLogging(bool enabled);

    /// <summary>
    /// Toggle verbose logging of AppLovin SDK. If enabled AppLovin messages will appear in standard application log accessible via console. All log messages will have "AppLovinSdk" tag.
    /// </summary>
    /// <param name="enabled"><c>true</c> if verbose logging should be enabled.</param>
    public static void SetVerboseLogging(bool enabled)
    {
        _MaxSetVerboseLogging(enabled);
    }

    [DllImport("__Internal")]
    private static extern bool _MaxIsVerboseLoggingEnabled();

    /// <summary>
    ///  Whether or not verbose logging is enabled.
    /// </summary>
    /// <returns><c>true</c> if verbose logging is enabled.</returns>
    public static bool IsVerboseLoggingEnabled()
    {
        return _MaxIsVerboseLoggingEnabled();
    }

    [DllImport("__Internal")]
    private static extern bool _MaxSetCreativeDebuggerEnabled(bool enabled);

    /// <summary>
    /// Whether the creative debugger will be displayed on fullscreen ads after flipping the device screen down twice. Defaults to true.
    /// </summary>
    /// <param name="enabled"><c>true</c> if the creative debugger should be enabled.</param>
    public static void SetCreativeDebuggerEnabled(bool enabled)
    {
        _MaxSetCreativeDebuggerEnabled(enabled);
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetTestDeviceAdvertisingIdentifiers(string[] advertisingIdentifiers, int size);

    /// <summary>
    /// Enable devices to receive test ads, by passing in the advertising identifier (IDFA/GAID) of each test device.
    /// Refer to AppLovin logs for the IDFA/GAID of your current device.
    /// </summary>
    /// <param name="advertisingIdentifiers">String list of advertising identifiers from devices to receive test ads.</param>
    public static void SetTestDeviceAdvertisingIdentifiers(string[] advertisingIdentifiers)
    {
        if (IsInitialized())
        {
            MaxSdkLogger.UserError("Test Device Advertising Identifiers must be set before SDK initialization.");
            return;
        }

        _MaxSetTestDeviceAdvertisingIdentifiers(advertisingIdentifiers, advertisingIdentifiers.Length);
    }

    [DllImport("__Internal")]
    private static extern bool _MaxSetExceptionHandlerEnabled(bool enabled);

    /// <summary>
    /// Whether or not the native AppLovin SDKs listen to exceptions. Defaults to <c>true</c>.
    /// </summary>
    /// <param name="enabled"><c>true</c> if the native AppLovin SDKs should not listen to exceptions.</param>
    public static void SetExceptionHandlerEnabled(bool enabled)
    {
        _MaxSetExceptionHandlerEnabled(enabled);
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetExtraParameter(string key, string value);

    /// <summary>
    /// Set an extra parameter to pass to the AppLovin server.
    /// </summary>
    /// <param name="key">The key for the extra parameter. Must not be null.</param>
    /// <param name="value">The value for the extra parameter. May be null.</param>
    public static void SetExtraParameter(string key, string value)
    {
        HandleExtraParameter(key, value);

        _MaxSetExtraParameter(key, value);
    }

    [DllImport("__Internal")]
    private static extern IntPtr _MaxGetSafeAreaInsets();

    /// <summary>
    /// Get the native insets in pixels for the safe area.
    /// These insets are used to position ads within the safe area of the screen.
    /// </summary>
    public static SafeAreaInsets GetSafeAreaInsets()
    {
        // Use an int array instead of json serialization for performance
        var insetsPtr = _MaxGetSafeAreaInsets();
        var insets = new int[4];
        Marshal.Copy(insetsPtr, insets, 0, 4);

        // Convert from points to pixels
        var screenDensity = MaxSdkUtils.GetScreenDensity();
        for (var i = 0; i < insets.Length; i++)
        {
            insets[i] *= (int) screenDensity;
        }

        return new SafeAreaInsets(insets);
    }

    #endregion

    #region Private

    [MonoPInvokeCallback(typeof(ALUnityBackgroundCallback))]
    internal static void BackgroundCallback(string propsStr)
    {
        HandleBackgroundCallback(propsStr);
    }

    #endregion

    #region Obsolete

    [Obsolete("This API has been deprecated and will be removed in a future release. Please use CreateBanner(string adUnitIdentifier, AdViewConfiguration configuration) instead.")]
    public static void CreateBanner(string adUnitIdentifier, BannerPosition bannerPosition)
    {
        // AdViewPosition and BannerPosition share identical enum values, so casting is safe
        CreateBanner(adUnitIdentifier, new AdViewConfiguration((AdViewPosition) bannerPosition));
    }

    [Obsolete("This API has been deprecated and will be removed in a future release. Please use CreateBanner(string adUnitIdentifier, AdViewConfiguration configuration) instead.")]
    public static void CreateBanner(string adUnitIdentifier, float x, float y)
    {
        CreateBanner(adUnitIdentifier, new AdViewConfiguration(x, y));
    }

    [Obsolete("This API has been deprecated and will be removed in a future release. Please use UpdateBannerPosition(string adUnitIdentifier, AdViewPosition bannerPosition) instead.")]
    public static void UpdateBannerPosition(string adUnitIdentifier, BannerPosition bannerPosition)
    {
        // AdViewPosition and BannerPosition share identical enum values, so casting is safe
        UpdateBannerPosition(adUnitIdentifier, (AdViewPosition) bannerPosition);
    }

    [Obsolete("This API has been deprecated and will be removed in a future release. Please use CreateMRec(string adUnitIdentifier, AdViewConfiguration configuration) instead.")]
    public static void CreateMRec(string adUnitIdentifier, AdViewPosition mrecPosition)
    {
        CreateMRec(adUnitIdentifier, new AdViewConfiguration(mrecPosition));
    }

    [Obsolete("This API has been deprecated and will be removed in a future release. Please use CreateMRec(string adUnitIdentifier, AdViewConfiguration configuration) instead.")]
    public static void CreateMRec(string adUnitIdentifier, float x, float y)
    {
        CreateMRec(adUnitIdentifier, new AdViewConfiguration(x, y));
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetSdkKey(string sdkKey);

    [Obsolete("This API has been deprecated and will be removed in a future release. Please set your SDK key in the AppLovin Integration Manager.")]
    public static void SetSdkKey(string sdkKey)
    {
        _MaxSetSdkKey(sdkKey);
        Debug.LogWarning("MaxSdk.SetSdkKey() has been deprecated and will be removed in a future release. Please set your SDK key in the AppLovin Integration Manager.");
    }

    #endregion

#endif
}
