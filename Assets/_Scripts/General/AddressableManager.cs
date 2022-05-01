using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviour
{
    [SerializeField] AssetReference _commonStylesAssetReference;
    StyleSheet _ss;

    void Start()
    {
        // https://www.youtube.com/watch?v=0USXRC9f4Iw
        Addressables.InitializeAsync().Completed += AddressableManager_Completed;
    }

    void AddressableManager_Completed(AsyncOperationHandle<UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator> obj)
    {
        _commonStylesAssetReference.LoadAssetAsync<StyleSheet>().Completed += (sheet) =>
        {
            _ss = (StyleSheet)sheet.Result;
        };

    }

    public StyleSheet GetCommonStyles()
    {
        return _ss;
    }
}
