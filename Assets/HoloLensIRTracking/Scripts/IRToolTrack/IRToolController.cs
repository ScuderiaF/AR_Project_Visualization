using Microsoft.MixedReality.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace IRToolTrack
{

    public class KeyboardInputHandler
    {
        public static bool HandleInput()
        {
            // Check for a specific key press, for example, the spacebar
            if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.Log("Update reference!");
                return true;
            }
            return false;

        }
    }


    public class IRToolController : MonoBehaviour
    {
        
        public string identifier;
        public GameObject[] spheres;
        public bool disableUntilDetection = false;
        public bool disableWhenTrackingLost = false;
        public float secondsLostUntilDisable = 3;
        public float sphere_radius = 6.5f;
        public int max_occluded_spheres = 0;
        public float lowpass_factor_rotation = 0.3f;
        public float lowpass_factor_position = 0.6f;


        private bool update_reference = true;
        private Dictionary<int, GameObject> markers = new Dictionary<int, GameObject>();
        private Dictionary<int, (Vector3 position, Quaternion rotation)> latestPoseData = new Dictionary<int, (Vector3, Quaternion)>();
        private Dictionary<int, float> lastSpottedTimePerMarker = new Dictionary<int, float>();

        public int sphere_count
        {
            get { return spheres.Length; }
        }

        public float[] sphere_positions
        {
            get {
                float[] coordinates = new float[sphere_count*3];
                int cur_coord = 0;
                for (int i = 0; i< sphere_count; i++) {
                    coordinates[cur_coord] = spheres[i].transform.localPosition.x;
                    coordinates[cur_coord + 1] = spheres[i].transform.localPosition.y;
                    coordinates[cur_coord + 2] = spheres[i].transform.localPosition.z;
                    cur_coord += 3;
                }
                return coordinates;
            }
        }

        public event Action<Vector3, Quaternion> OnPoseUpdated;


        void Start()
        {
            //irToolTracking = FindObjectOfType<IRToolTracking>();
#if !UNITY_EDITOR
            if (disableUntilDetection)
            {
                for (int i = 0; i<transform.childCount; i++)
                {
                    var curChild = transform.GetChild(i).gameObject;
                    if (curChild.activeSelf)
                    {
                        childAtIndexActive[i] = true;
                        curChild.SetActive(false);
                    }
                }
                childrenActive = false;
            }
#endif
        }


        bool LoadROMFile(string romFilePath)
        {
            var romFile = Resources.Load(romFilePath);
            return false;
        }

        public enum Status
        {
            Inactive,
            Active
        }
        private Status _subStatus = Status.Active;

        // public void StartTracking()
        // {
        //     if (_subStatus == Status.Active)
        //     {
        //         Debug.Log("Tool tracking already started.");
        //         return;
        //     }
        //     //_listener.Start();
        //     Debug.Log("Started tracking "+identifier);
        //     _subStatus = Status.Active;
        // }

        // public void StopTracking()
        // {
        //     if (_subStatus == Status.Inactive)
        //     {
        //         Debug.Log("Tracking of "+identifier+" already stopped.");
        //         return;
        //     }
        //     //_listener.Stop();
        //     Debug.Log("Stopped tracking " + identifier);
        //     _subStatus = Status.Inactive;
        // }

        public void RegisterMarker(int id, GameObject marker)
        {
            if (!markers.ContainsKey(id))
            {
                markers[id] = marker;
            }
        }

/*        public void ReceivePoseData(int markerId, Vector3 position, Quaternion rotation)
        {
            latestPoseData[markerId] = (position, rotation);
            lastSpotted = Time.time;

            if (!childrenActive)
            {
                ActivateChildren();
            }
        }*/

        public void ReceivePoseData(int markerId, Vector3 position, Quaternion rotation)
        {
            if (markerId != 0 || update_reference == true)
            {
                latestPoseData[markerId] = (position, rotation);
                lastSpottedTimePerMarker[markerId] = Time.time;

                if (markers.TryGetValue(markerId, out GameObject marker))
                {
                    marker.SetActive(true);
                }
                if (markerId == 0) { update_reference = false; }
            }
            
        }

/*        void Update()
        {
            if (_subStatus == Status.Inactive)
                return;
            // Int64 trackingTimestamp = irToolTracking.GetTimestamp();

            foreach (var item in latestPoseData)
            {
                if (markers.TryGetValue(item.Key, out GameObject marker))
                {
                    marker.transform.position = item.Value.position;
                    marker.transform.rotation = item.Value.rotation;
                }
            }

            if (childrenActive && disableWhenTrackingLost && Time.time - lastSpotted > secondsLostUntilDisable)
            {
                DeactivateChildren();
            }
        }*/

        void Update()
        {
            update_reference = KeyboardInputHandler.HandleInput();
            foreach (var item in latestPoseData)
            {
                if (markers.TryGetValue(item.Key, out GameObject marker))
                {
                    marker.transform.position = item.Value.position;
                    marker.transform.rotation = item.Value.rotation;

                    // 检查是否应该禁用 marker
                    if (disableWhenTrackingLost && Time.time - lastSpottedTimePerMarker[item.Key] > secondsLostUntilDisable)
                    {
                        marker.SetActive(false);
                    }
                }
            }
        }

/*        void ActivateChildren()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var curChild = transform.GetChild(i).gameObject;
                if (childAtIndexActive[i])
                {
                    curChild.SetActive(true);
                }
            }
            childrenActive = true;
        }

        void DeactivateChildren()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            childrenActive = false;
        }*/    
    }
}