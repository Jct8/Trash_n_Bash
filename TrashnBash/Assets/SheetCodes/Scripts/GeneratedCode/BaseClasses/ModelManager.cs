using System;
using System.Collections.Generic;
using UnityEngine;

namespace SheetCodes
{
	//Generated code, do not edit!

	public static class ModelManager
	{
        private static Dictionary<DatasheetType, LoadRequest> loadRequests;

        static ModelManager()
        {
            loadRequests = new Dictionary<DatasheetType, LoadRequest>();
        }

        public static void InitializeAll()
        {
            DatasheetType[] values = Enum.GetValues(typeof(DatasheetType)) as DatasheetType[];
            foreach(DatasheetType value in values)
                Initialize(value);
        }
		

        public static void Unload(DatasheetType datasheetType)
        {
            switch (datasheetType)
            {
                case DatasheetType.Upgrades:
                    {
                        if (upgradesModel == null || upgradesModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(upgradesModel);
                        upgradesModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Upgrades, out request))
                        {
                            loadRequests.Remove(DatasheetType.Upgrades);
                            request.resourceRequest.completed -= OnLoadCompleted_UpgradesModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        public static void Initialize(DatasheetType datasheetType)
        {
            switch (datasheetType)
            {
                case DatasheetType.Upgrades:
                    {
                        if (upgradesModel != null && !upgradesModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        upgradesModel = Resources.Load<UpgradesModel>("ScriptableObjects/Upgrades");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Upgrades, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.Upgrades);
                            request.resourceRequest.completed -= OnLoadCompleted_UpgradesModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        public static void InitializeAsync(DatasheetType datasheetType, Action<bool> callback)
        {
            switch (datasheetType)
            {
                case DatasheetType.Upgrades:
                    {
                        if (upgradesModel != null && !upgradesModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.Upgrades))
                        {
                            loadRequests[DatasheetType.Upgrades].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<UpgradesModel>("ScriptableObjects/Upgrades");
                        loadRequests.Add(DatasheetType.Upgrades, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_UpgradesModel;
                        break;
                    }
                default:
                    break;
            }
        }

        private static void OnLoadCompleted_UpgradesModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.Upgrades];
            upgradesModel = request.resourceRequest.asset as UpgradesModel;
            loadRequests.Remove(DatasheetType.Upgrades);
            operation.completed -= OnLoadCompleted_UpgradesModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static UpgradesModel upgradesModel = default;
		public static UpgradesModel UpgradesModel
        {
            get
            {
                if (upgradesModel == null)
                    Initialize(DatasheetType.Upgrades);

                return upgradesModel;
            }
        }
		
        private static void Log(string message)
        {
            Debug.LogWarning(message);
        }
	}
	
    public struct LoadRequest
    {
        public readonly ResourceRequest resourceRequest;
        public readonly List<Action<bool>> callbacks;

        public LoadRequest(ResourceRequest resourceRequest, Action<bool> callback)
        {
            this.resourceRequest = resourceRequest;
            callbacks = new List<Action<bool>>() { callback };
        }
    }
}
