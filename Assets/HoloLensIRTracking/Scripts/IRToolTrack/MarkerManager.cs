using IRToolTrack;
using System;
using System.IO;
using UnityEditor.Search;
using UnityEngine;

namespace IRToolTrack
{

    public class MarkerManager : MonoBehaviour
    {
        public GameObject markerPrefab;
        public GameObject referencePrefab;
        public IRToolController irToolController;
        public string tool_Config_Paths;
        public string reference_Config_Path;

        void Start()
        {
            //irToolController = FindObjectOfType<IRToolController>();
            string[] toolPathArray = tool_Config_Paths.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string referencePath = reference_Config_Path;
            LoadAndCreateMarkers(toolPathArray, referencePath);

            Transform testControllerTransform = transform.Find("TestController");

            if (testControllerTransform != null)
            {
                // Activate the TestController GameObject
                testControllerTransform.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("TestController not found as a child of MarkerManager.");
            }
        }

        private void LoadAndCreateMarkers(string[] toolPathArray, string referencePath)
        {
            int fid_idx = 0;
            float x_mean = 0.0f;
            float y_mean = 0.0f;
            float z_mean = 0.0f;

            string jsonText = File.ReadAllText(referencePath);
            MarkerConfig config = JsonUtility.FromJson<MarkerConfig>(jsonText);

            foreach (var fiducial in config.fiducials)
            {
                x_mean += fiducial.x / config.count;
                y_mean += fiducial.y / config.count;
                z_mean += fiducial.z / config.count;
            }

            GameObject referenceInstance = Instantiate(referencePrefab, Vector3.zero, Quaternion.identity);
            referenceInstance.name = "reference" + config.id.ToString();

            Transform referenceTransform = referenceInstance.transform.Find("reference");

            // Set the position of the pivot
            Transform pivotTransform = referenceTransform.Find("frame");
            pivotTransform.localPosition = new Vector3(config.pivot.x - x_mean, config.pivot.y - y_mean, config.pivot.z - z_mean);
            pivotTransform.localRotation = Quaternion.Euler(config.pivot.rx, config.pivot.ry, config.pivot.rz);
            fid_idx = 0;
            foreach (var fiducial in config.fiducials)
            {
                string fiducialName = "fiducial" + fid_idx.ToString();
                Transform fiducialTransform = referenceTransform.Find(fiducialName);
                /*                GameObject fiducialObject = new GameObject("Fiducial");
                                fiducialObject.transform.parent = markerInstance.transform;
                                fiducialObject.transform.localPosition = new Vector3(fiducial.x, fiducial.y, fiducial.z);*/

                if (fiducialTransform != null)
                {
                    fiducialTransform.localPosition = new Vector3(fiducial.x - x_mean, fiducial.y - y_mean, fiducial.z - z_mean);
                    fiducialTransform.localScale = new Vector3(config.scale, config.scale, config.scale);
                }
                else
                {
                    Debug.LogWarning($"Fiducial object with name {fiducialName} not found.");
                }
                fid_idx++;
            }

            irToolController.RegisterMarker(config.id, referenceInstance);

            foreach (var toolConfigPath in toolPathArray)
            {
                jsonText = File.ReadAllText(toolConfigPath);
                config = JsonUtility.FromJson<MarkerConfig>(jsonText);

                foreach (var fiducial in config.fiducials)
                {
                    x_mean += fiducial.x / config.count;
                    y_mean += fiducial.y / config.count;
                    z_mean += fiducial.z / config.count;
                }

                GameObject markerInstance = Instantiate(markerPrefab, Vector3.zero, Quaternion.identity);
                markerInstance.name = "tool" + config.id.ToString();

                Transform toolTransform = markerInstance.transform.Find("tool");

                // Set the position of the pivot
                pivotTransform = toolTransform.Find("model");
                pivotTransform.localPosition = new Vector3(config.pivot.x - x_mean, config.pivot.y - y_mean, config.pivot.z - z_mean);
                pivotTransform.localRotation = Quaternion.Euler(config.pivot.rx, config.pivot.ry, config.pivot.rz);
                fid_idx = 0;
                foreach (var fiducial in config.fiducials)
                {
                    string fiducialName = "fiducial" + fid_idx.ToString();
                    Transform fiducialTransform = toolTransform.Find(fiducialName);
                    /*                GameObject fiducialObject = new GameObject("Fiducial");
                                    fiducialObject.transform.parent = markerInstance.transform;
                                    fiducialObject.transform.localPosition = new Vector3(fiducial.x, fiducial.y, fiducial.z);*/

                    if (fiducialTransform != null)
                    {
                        fiducialTransform.localPosition = new Vector3(fiducial.x - x_mean, fiducial.y - y_mean, fiducial.z - z_mean);
                        fiducialTransform.localScale = new Vector3(config.scale, config.scale, config.scale);
                    }
                    else
                    {
                        Debug.LogWarning($"Fiducial object with name {fiducialName} not found.");
                    }
                    fid_idx++;
                }

                irToolController.RegisterMarker(config.id, markerInstance);
            }


        }
    }
}